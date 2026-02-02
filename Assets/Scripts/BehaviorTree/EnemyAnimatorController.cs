using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorController : MonoBehaviour
{
    [Header("Weapon Colliders")]
    [SerializeField] private Collider rightHitCollider;
    [SerializeField] private Collider leftHitCollider;

    [Header("Ragdoll")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody[] ragdollRigidbodies;
    [SerializeField] private Collider[] ragdollColliders;

    [Header("Components to Disable on Death")]
    [SerializeField] private Collider mainCollider;
    [SerializeField] private NavMeshAgent agent;

    [Header("Boss Abilities")]
    public MeleeBossEnemy meleeBossEnemy;
    [SerializeField] private GameObject groundShockwavePrefab;

    private void Awake()
    {
        // Auto-find ragdoll rigidbodies if not assigned
        if (ragdollRigidbodies == null || ragdollRigidbodies.Length == 0)
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        
        // Disable weapon colliders by default
        if (rightHitCollider) rightHitCollider.enabled = false;
        if (leftHitCollider) leftHitCollider.enabled = false;

        // Setup ragdoll (disabled at start)
        SetRagdollState(false);
    }

    #region Weapon Colliders (Animation Events)
    public void EnableRightHitCollider()
    {
        if (rightHitCollider)
            rightHitCollider.enabled = true;
    }

    public void EnableLeftHitCollider()
    {
        if (leftHitCollider)
            leftHitCollider.enabled = true;
    }

    public void DisableRightHitCollider()
    {
        if (rightHitCollider)
            rightHitCollider.enabled = false;
    }

    public void DisableLeftHitCollider()
    {
        if (leftHitCollider)
            leftHitCollider.enabled = false;
    }
    
    public void DisableAllWeaponColliders()
    {
        if (rightHitCollider) rightHitCollider.enabled = false;
        if (leftHitCollider) leftHitCollider.enabled = false;
    }
    #endregion

    #region Ragdoll
    public void EnableRagdoll()
    {
        // Disable weapon colliders immediately
        DisableAllWeaponColliders();
        
        if (animator) animator.enabled = false;
        if (mainCollider) mainCollider.enabled = false;
        if (agent) agent.enabled = false;
        
        SetRagdollState(true);
    }

    private void SetRagdollState(bool isRagdoll)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !isRagdoll;
        }
        foreach (Collider col in ragdollColliders)
        {
            col.enabled = isRagdoll;
        }
    }

    // Editor helper to auto-find ragdoll rigidbodies
    [ContextMenu("Auto-Find Ragdoll Rigidbodies")]
    private void FindRagdollRigidbodies()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        Debug.Log($"Found {ragdollRigidbodies.Length} rigidbodies for ragdoll");
    }
    #endregion

    #region Boss Abilities (Animation Events)
    public void StartGroundPound()
    {
        Debug.Log("<color=magenta>=== StartGroundPound() called from animation event! ===</color>");
        SpawnGroundShockwave();
    }

    private void SpawnGroundShockwave()
    {
        Debug.Log($"<color=magenta>SpawnGroundShockwave() - Prefab: {groundShockwavePrefab != null}, Boss: {meleeBossEnemy != null}</color>");
        
        if (groundShockwavePrefab == null)
        {
            Debug.LogError("Ground Shockwave Prefab is NOT assigned in Inspector!");
            return;
        }

        if (meleeBossEnemy == null)
        {
            Debug.LogError("MeleeBossEnemy reference is missing! Assign it in Inspector!");
            return;
        }

        // Spawn at boss position, NOT as child
        Vector3 spawnPos = transform.position;
        spawnPos.y = 0.5f; // Ground level
        
        GameObject shockwaveObj = Instantiate(groundShockwavePrefab, transform);
        
        Debug.Log($"<color=cyan>Shockwave GameObject instantiated at {spawnPos}</color>");
        
        GroundShockwave shockwave = shockwaveObj.GetComponent<GroundShockwave>();
        if (shockwave != null)
        {
            shockwave.Initialize(
                meleeBossEnemy.shockwaveDamage,
                meleeBossEnemy.shockwaveRadius,
                meleeBossEnemy.shockwaveSpeed
            );
            
            Debug.Log($"<color=green>Ground Shockwave initialized! Damage: {meleeBossEnemy.shockwaveDamage}, Radius: {meleeBossEnemy.shockwaveRadius}, Speed: {meleeBossEnemy.shockwaveSpeed}</color>");
        }
        else
        {
            Debug.LogError("GroundShockwave component NOT found on prefab! Add the script to Shock_Wave prefab!");
        }
    }
    #endregion
}