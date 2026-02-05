using UnityEngine;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour
{
    private PlayerController player;

    [Header("Combo Settings")]
    public int comboStep = 0;
    public float lastClickTime = -999f;
    public float comboResetTime = 1.2f;

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

    [SerializeField] private Transform bitePos;
    [SerializeField] private Collider leftAttackCollider;
    [SerializeField] private Collider rightAttackCollider;

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
            if (player != null) player.IsFinalComboActive = false;
        }
    }

    public void Attack()
    {
        if (isFinalAttackInProgress) return;
        
        if (Time.time - lastClickTime > comboResetTime)
            comboStep = 0;

        lastClickTime = Time.time;

        //DisableAllColliders();

        bool isFinal = (comboStep == 2);
        if (player != null) player.IsFinalComboActive = isFinal;

        if (isFinal)
        {
            isFinalAttackInProgress = true;
            finalTimer = finalAttackDuration;
        }

        player.AnimationEvents.PlayComboAnimation(comboStep);
        PerformAttackStep();

        comboStep = (comboStep + 1) % 3;
    }

    private void PerformAttackStep()
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
    }

    public void EnableLeftAttackCollider()
    {
        if (rightAttackCollider != null) rightAttackCollider.enabled = false;
        if (leftAttackCollider != null) leftAttackCollider.enabled = true;
    }

    public void EnableRightAttackCollider()
    {
        if (leftAttackCollider != null) leftAttackCollider.enabled = false;
        if (rightAttackCollider != null) rightAttackCollider.enabled = true;
    }

    public void DisableLeftAttackCollider()
    {
        if (leftAttackCollider != null) leftAttackCollider.enabled = false;
    }

    public void DisableRightAttackCollider()
    {
        if (rightAttackCollider != null) rightAttackCollider.enabled = false;
    }

    public void SpawnBiteParticle()
    {
        if (ManagerObjectPool.Instance == null || bitePos == null) return;
        //ManagerObjectPool.Instance.Spawn(ObjectPoolType.BiteParticle, bitePos);
    }
}
