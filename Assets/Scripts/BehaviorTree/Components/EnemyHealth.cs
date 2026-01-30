using BehaviorTree;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class EnemyHealth : MonoBehaviour, IDamageable
{
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
    [SerializeField] private GameObject deathEffectPrefab;

    private BehaviorTreeBase behaviorTree;
    private NavMeshAgent navAgent;
    private Animator animator;
    private int currentHealth;
    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private bool isFlashing = false;

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
        
        // Cache all renderers and their original materials
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length][];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            // Create a copy of the materials array to avoid modifying shared materials
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
       
        healthSlider.value -= (float)damage / maxHealth;
        var slider = damageSlider.value;
        DOTween.To(() => slider, x => damageSlider.value = x, (float)((float)currentHealth / (float)maxHealth), 0.5f).SetEase(Ease.OutSine);

        // Flash damage material when taking damage
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
        
        // Store state before freezing
        bool wasBehaviorTreeEnabled = behaviorTree != null && behaviorTree.enabled;
        bool wasNavAgentEnabled = navAgent != null && navAgent.enabled;
        Vector3 storedVelocity = navAgent != null ? navAgent.velocity : Vector3.zero;
        float originalAnimatorSpeed = animator != null ? animator.speed : 1f;
        
        // Freeze enemy
        if (freezeOnDamage)
        {
            if (behaviorTree != null)
                behaviorTree.enabled = false;
            
            if (navAgent != null && navAgent.enabled)
            {
                navAgent.velocity = Vector3.zero;
                navAgent.isStopped = true;
            }
            
            if (animator != null)
                animator.SetTrigger("HitReaction");
        }
        
        // Change all materials to damage material
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
        
        // Wait for flash duration
        yield return new WaitForSeconds(damageFlashDuration);
        
        // Restore original materials
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].materials = originalMaterials[i];
            }
        }
        
        // Unfreeze enemy (only if not dead)
        if (freezeOnDamage && !IsDead)
        {
            if (behaviorTree != null && wasBehaviorTreeEnabled)
                behaviorTree.enabled = true;
            
            if (navAgent != null && wasNavAgentEnabled)
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
        
        // Stop any ongoing flash
        StopAllCoroutines();
        
        // Restore original materials before ragdoll
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].materials = originalMaterials[i];
            }
        }
        
        // Spawn death VFX effect
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (animatorController != null)
            animatorController.EnableRagdoll();

        if (behaviorTree != null)
            behaviorTree.enabled = false;

        if (interactionCollider != null)
            interactionCollider.enabled = true;

        Destroy(gameObject, 30f);
    }

    public void ConsumeBody()
    {
        Destroy(gameObject);
    }
}
