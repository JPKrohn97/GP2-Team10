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

    [Header("Heavy Ground Pound Attack")]
    public int shockwaveDamage = 30;
    public float shockwaveRadius = 10f;
    public float shockwaveSpeed = 15f;
    public float heavyAttackCooldown = 1.5f;
    public float heavyAttackChance = 0.3f;

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
                new TaskMeleeBossAttack(transform, agent, animator,
                    lightAttackCooldown, heavyAttackCooldown, heavyAttackChance)
            }),
            new TaskPatrol(transform, agent, waypoints, animator)
        });

        return root;
    }
}