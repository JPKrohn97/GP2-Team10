using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class RangedBossEnemy : BehaviorTreeBase
{
    [Header("General Settings")]
    public float detectionRange = 25f;
    public float attackRange = 15f;
    public LayerMask playerLayer;

    [Header("Combat Settings")]
    public float lightAttackCooldown = 2f;
    public float heavyAttackCooldown = 4f;
    public float heavyAttackChance = 0.3f;
    public Transform firePoint;

    [Header("Dash Back Behavior")]
    public float closeRangeThreshold = 5f;
    public float dashBackDistance = 8f;

    [Header("Patrol")]
    public Transform[] waypoints;

    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInRange(transform, detectionRange, playerLayer),
                new TaskGoToTarget(transform, agent, attackRange, animator),
                new TaskRangedBossAttack(transform, agent, firePoint, animator,
                    lightAttackCooldown, heavyAttackCooldown, heavyAttackChance,
                    dashBackDistance, closeRangeThreshold)
            }),
            new TaskPatrol(transform, agent, waypoints, animator)
        });

        return root;
    }
}