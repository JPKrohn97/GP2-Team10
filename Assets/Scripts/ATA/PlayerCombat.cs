using UnityEngine;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour
{
    private PlayerController player;

    [Header("Combo Settings")]
    public int comboStep = 0;
    public float lastClickTime = -999f;
    public float comboResetTime = 1.2f;

    [Header("Combo State")]
    public bool IsFinalComboActive { get; set; }

    [Header("Final Attack Guard")]
    private bool isFinalAttackInProgress = false;
    public float finalAttackDuration = 0.6f;
    private float finalTimer;

    [Header("Attack Movement")]
    public float attackStepForce = 5f;
    public float attackStepDuration = 0.15f;
    
    [Header("Dash Attack Settings")] 
    public LayerMask enemyLayer; 
    public int dashDamage = 20;
    public GameObject dashTrailPrefab;
    
    [Header("Sword Combo")]
    public int swordComboStep = 0;
    public float swordComboResetTime = 1.2f;

    [SerializeField] private Transform bitePos;
    [SerializeField] private Collider leftAttackCollider;
    [SerializeField] private Collider rightAttackCollider;
    [SerializeField] private Collider rightSwordAttackCollider;

    private Tween stopTween;

    void Start()
    {
        player = GetComponent<PlayerController>();
        DisableAllColliders();
    }

    void Update()
    {
        if (!isFinalAttackInProgress) return;

        finalTimer -= Time.deltaTime;
        if (finalTimer <= 0f)
        {
            isFinalAttackInProgress = false;
            IsFinalComboActive = false;
        }
    }

    public void Attack()
    {
        if (isFinalAttackInProgress) return;
        
        if (Time.time - lastClickTime > comboResetTime)
            comboStep = 0;

        lastClickTime = Time.time;

        bool isFinal = (comboStep == 2);
        IsFinalComboActive = isFinal;

        if (isFinal)
        {
            isFinalAttackInProgress = true;
            finalTimer = finalAttackDuration;
        }

        player.AnimationEvents.PlayComboAnimation(comboStep);
        PerformAttackStep();

        comboStep = (comboStep + 1) % 3;
    }

    public void PerformAttackStep()
    {
        if (player == null || player.RB == null) return;

        stopTween?.Kill();
        player.RB.linearVelocity = Vector3.zero;

        Vector3 attackDir = player.transform.forward;
        attackDir.y = 0f;

        player.RB.AddForce(attackDir.normalized * attackStepForce, ForceMode.Impulse);

        stopTween = DOVirtual.DelayedCall(attackStepDuration, () =>
        {
            if (player != null && player.RB != null && player.IsGrounded)
                player.RB.linearVelocity = Vector3.zero;
        });
    }

    private void DisableAllColliders()
    {
        if (leftAttackCollider != null) leftAttackCollider.enabled = false;
        if (rightAttackCollider != null) rightAttackCollider.enabled = false;
        
        if (rightSwordAttackCollider != null) rightSwordAttackCollider.enabled = false;
    }

    public void EnableLeftAttackCollider() => leftAttackCollider.enabled = true;
    public void EnableRightAttackCollider() => rightAttackCollider.enabled = true;
    public void DisableLeftAttackCollider() => leftAttackCollider.enabled = false;
    public void DisableRightAttackCollider() => rightAttackCollider.enabled = false;
    public void EnableRightSwordAttackCollider() => rightSwordAttackCollider.enabled = true;
    public void DisableRightSwordAttackCollider() => rightSwordAttackCollider.enabled = false;

    public void SpawnBiteParticle()
    {
        if (ManagerObjectPool.Instance == null || bitePos == null) return;
        ManagerObjectPool.Instance.Spawn(ObjectPoolType.BiteParticle, bitePos);
    }
}