using System.Collections.Generic;
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
        // Same structure as MeleeEnemy and RangedEnemy
        Node root = new Selector(new List<Node>
        {
            // Charge sequence (priority)
            new Sequence(new List<Node>
            {
                new CheckPlayerInRange(transform, detectionRange, playerLayer),
                new TaskCharge(transform, agent, animator, chargeSpeed,
                    chargeDuration, chargeCooldown, 0.3f, chargeTriggerName)
            }),
            // Patrol (when player is not in range)
            new TaskPatrol(transform, agent, waypoints, animator)
        });

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
