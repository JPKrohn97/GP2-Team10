using BehaviorTree;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public enum EnemyMutationType
    {
        Sword,
        Dash,
        Range
    }
    
    public EnemyMutationType mutationType;
    
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private EnemyAnimatorController animatorController;
    [SerializeField] private Collider interactionCollider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider damageSlider;
    [SerializeField] private GameObject healthCanvas;

    [Header("Damage Feedback")]
    [SerializeField] private float damageFlashDuration = 0.2f;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private bool freezeOnDamage = true;
    
    [Header("Death Effect")]
    [SerializeField] private float deathEffectLifetime = 2f;

    private BehaviorTreeBase behaviorTree;
    private NavMeshAgent navAgent;
    private Animator animator;
    private int currentHealth;
    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private bool isFlashing = false;
    public bool isBoss = false;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;

        behaviorTree = GetComponent<BehaviorTreeBase>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (animatorController == null)
            animatorController = GetComponentInChildren<EnemyAnimatorController>();

        if (interactionCollider != null)
            interactionCollider.enabled = false;
        
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length][];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;
            originalMaterials[i] = new Material[materials.Length];
            
            for (int j = 0; j < materials.Length; j++)
            {
                originalMaterials[i][j] = materials[j];
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
       ManagerVibration.Vibrate(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
        healthSlider.value -= (float)damage / maxHealth;
        var slider = damageSlider.value;
        DOTween.To(() => slider, x => damageSlider.value = x, (float)currentHealth / maxHealth, 0.5f).SetEase(Ease.OutSine);

        SoundManager.Instance.PlaySound(SoundManager.Instance.EnemyHurt, gameObject);

        if (!isFlashing && damageMaterial != null)
        {
            StartCoroutine(DamageFlash());
        }
        
        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator DamageFlash()
    {
        isFlashing = true;
        
        bool wasBehaviorTreeEnabled = behaviorTree != null && behaviorTree.enabled;
        bool wasNavAgentEnabled = navAgent != null && navAgent.enabled;
        Vector3 storedVelocity = navAgent != null ? navAgent.velocity : Vector3.zero;
        float originalAnimatorSpeed = animator != null ? animator.speed : 1f;
        
        if (freezeOnDamage)
        {
            if (behaviorTree != null)
                behaviorTree.enabled = false;
            
            if (navAgent != null && navAgent.enabled && navAgent.isOnNavMesh)
            {
                navAgent.velocity = Vector3.zero;
                navAgent.isStopped = true;
            }
            
            if (animator != null)
                animator.SetTrigger("HitReaction");
        }
        
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                Material[] newMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = damageMaterial;
                }
                renderer.materials = newMaterials;
            }
        }
        
        yield return new WaitForSeconds(damageFlashDuration);
        
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].materials = originalMaterials[i];
            }
        }
        
        if (freezeOnDamage && !IsDead)
        {
            if (behaviorTree != null && wasBehaviorTreeEnabled)
                behaviorTree.enabled = true;
            
            if (navAgent != null && wasNavAgentEnabled && navAgent.isOnNavMesh)
            {
                navAgent.isStopped = false;
            }
            
            if (animator != null)
                animator.speed = originalAnimatorSpeed;
        }
        
        isFlashing = false;
    }

    private void Die()
    {
        IsDead = true;
        healthCanvas.SetActive(false);

        BehaviorTreeBase enemyBT = GetComponent<BehaviorTreeBase>();
        if (enemyBT != null)
        {
            if (enemyBT is BossEnemyBT || enemyBT is MeleeBossEnemy || enemyBT is RangedBossEnemy)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.BossDies, gameObject);
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.EnemyDies, gameObject);
            }
        }
        ManagerCinemachine.Instance.TriggerFinisherCamera();
        if (isBoss)
        {
           GameManager.Instance.OnBossDefeated();
            SoundManager.Instance?.PlayMusic(SoundManager.Instance.Regular);


        }

        // Disable behavior tree FIRST to prevent further updates
        if (behaviorTree != null)
            behaviorTree.enabled = false;
        
        StopAllCoroutines();
        
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].materials = originalMaterials[i];
            }
        }
        
        // Spawn death VFX using ObjectPool
        GameObject deathEffect = ManagerObjectPool.Instance.Spawn(
            ObjectPoolType.DeathHit, 
            transform.position, 
            Quaternion.identity
        );
        
        // Auto-despawn after lifetime
        if (deathEffect != null)
        {
            DOVirtual.DelayedCall(deathEffectLifetime, () =>
            {
                ManagerObjectPool.Instance.Despawn(ObjectPoolType.DeathHit, deathEffect);
            });
        }

        if (animatorController != null)
            animatorController.EnableRagdoll();

        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.isStopped = true;
            navAgent.enabled = false;
        }

        if (interactionCollider != null)
            interactionCollider.enabled = true;

        Destroy(gameObject, 30f);
    }

    public void ConsumeBody()
    {
        Destroy(gameObject);
    }
}
