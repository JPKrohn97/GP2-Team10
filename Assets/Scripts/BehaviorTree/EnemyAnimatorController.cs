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
    }
    #endregion

    #region Boss Abilities (Animation Events)
    public void StartGroundPound()
    {
        SpawnGroundShockwave();
    }

    private void SpawnGroundShockwave()
    {
        if (meleeBossEnemy == null)
        {
            Debug.LogError("MeleeBossEnemy reference is missing! Assign it in Inspector!");
            return;
        }

        Vector3 spawnPos = transform.position+transform.forward/2 + Vector3.up/10;

        
        
        
        GameObject shockwaveObj = ManagerObjectPool.Instance.Spawn(
            ObjectPoolType.BossGroundShockwave,
            spawnPos,
            Quaternion.identity
        );
        
        if (shockwaveObj != null)
        {
            GroundShockwave shockwave = shockwaveObj.GetComponent<GroundShockwave>();
            if (shockwave != null)
            {
                shockwave.Initialize(
                    meleeBossEnemy.shockwaveDamage,
                    meleeBossEnemy.shockwaveRadius,
                    meleeBossEnemy.shockwaveSpeed
                );
            }
            else
            {
                Debug.LogError("GroundShockwave component NOT found on pooled prefab!");
            }
        }
    }
    #endregion
}