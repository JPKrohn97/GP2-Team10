using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class BossEnemyBT : BehaviorTreeBase
{
    [Header("General Settings")]
    public float detectionRange = 20f;
    public float attackRange = 3f;
    public LayerMask playerLayer;

    [Header("Light Attack")]
    public float lightAttackCooldown = 1f;
    public int lightAttackDamage = 15;

    [Header("Heavy Attack")]
    public float heavyAttackCooldown = 4f;
    public int heavyAttackDamage = 40;
    public float heavyAttackChance = 0.3f; // 30% chance for heavy attack

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
            // Boss attack sequence
            new Sequence(new List<Node>
            {
                new CheckPlayerInRange(transform, detectionRange, playerLayer),
                new TaskGoToTarget(transform, agent, attackRange),
                new TaskBossAttack(transform, animator, 
                    lightAttackCooldown, lightAttackDamage,
                    heavyAttackCooldown, heavyAttackDamage, 
                    heavyAttackChance)
            }),
            // Patrol
            new TaskPatrol(transform, agent, waypoints, animator)
        });

        return root;
    }
}