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
    public int chargeDamage = 25;
    public LayerMask playerLayer;

    [Header("Patrol")]
    public Transform[] waypoints;

    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            // Charge sequence
            new Sequence(new List<Node>
            {
                new CheckPlayerInRange(transform, detectionRange, playerLayer),
                new TaskCharge(transform, agent, animator, chargeSpeed, 
                    chargeDuration, chargeCooldown, chargeDamage)
            }),
            // Patrol
            new TaskPatrol(transform, agent, waypoints)
        });

        return root;
    }
}