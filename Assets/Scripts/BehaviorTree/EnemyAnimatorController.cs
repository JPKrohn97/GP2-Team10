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
        {
            rightHitCollider.enabled = true;
            Debug.Log("Right Hit Collider Enabled");
        }
    }

    public void EnableLeftHitCollider()
    {
        if (leftHitCollider)
        {
            leftHitCollider.enabled = true;
            Debug.Log("Left Hit Collider Enabled");
        }
    }

    public void DisableRightHitCollider()
    {
        if (rightHitCollider)
        {
            rightHitCollider.enabled = false;
            Debug.Log("Right Hit Collider Disabled");
        }
    }

    public void DisableLeftHitCollider()
    {
        if (leftHitCollider)
        {
            leftHitCollider.enabled = false;
            Debug.Log("Left Hit Collider Disabled");
        }
    }
    #endregion

    #region Ragdoll
    public void EnableRagdoll()
    {
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
            rb.detectCollisions = isRagdoll;
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
}