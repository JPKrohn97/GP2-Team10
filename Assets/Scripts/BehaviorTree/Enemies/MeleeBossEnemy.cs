using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class MeleeBossEnemy : BehaviorTreeBase
{
    [Header("General Settings")]
    public float detectionRange = 20f;
    public float attackRange = 3f;
    public LayerMask playerLayer;

    [Header("Light Attack")]
    public float lightAttackCooldown = 1.5f;
    public int lightAttackDamage = 15;
    public float lightAttackRange = 2.5f;

    [Header("Heavy Ground Pound Attack")]
    public float heavyAttackCooldown = 5f;
    public int heavyAttackDamage = 30;
    public float shockwaveRadius = 8f;
    public float shockwaveSpeed = 5f;
    public float heavyAttackChance = 0.25f;
    [Tooltip("Time between animation start and shockwave spawn - gives player time to react")]
    public float attackTelegraphDelay = 0.5f;

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
                new TaskMeleeBossAttack(transform, animator,
                    lightAttackCooldown, lightAttackDamage, lightAttackRange,
                    heavyAttackCooldown, heavyAttackDamage,
                    shockwaveRadius, shockwaveSpeed, heavyAttackChance, 
                    attackTelegraphDelay)
            }),
            new TaskPatrol(transform, agent, waypoints, animator)
        });

        return root;
    }
}