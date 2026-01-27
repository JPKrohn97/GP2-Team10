using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class ChargingEnemyBT : BehaviorTreeBase
{
    [Header("Settings")]
    public float detectionRange = 12f;
    public float chargeSpeed = 15f;
    public float chargeDuration = 1.5f;
    public float chargeCooldown = 3f;
    public float accelerationTime = 0.3f;
    public LayerMask playerLayer;
    
    [Header("Animation")]
    public string chargeTriggerName = "Charge";

    [Header("Patrol")]
    public Transform[] waypoints;
    
    [Header("Debug")]
    public bool showDebug = true;

    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    protected override Node SetupTree()
    {
        // Single node handles all logic (state machine)
        Node root = new TaskChargeEnemy(
            transform, agent, animator, waypoints, playerLayer,
            detectionRange, chargeSpeed, chargeDuration, chargeCooldown, 
            accelerationTime, chargeTriggerName
        );

        return root;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (showDebug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}