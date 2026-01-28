using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class RangedEnemyBT : BehaviorTreeBase
{
    [Header("Settings")]
    public float detectionRange = 15f;
    public float attackRange = 12f;
    public float attackCooldown = 2f;
    public float projectileSpeed = 10f;
    public LayerMask playerLayer;

    [Header("Shooting")]
    public Transform firePoint;
    public GameObject projectilePrefab;

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
        EnemyHealth health = GetComponent<EnemyHealth>(); // ✅

        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInRange(transform, detectionRange, playerLayer),
                new CheckInAttackRange(transform, attackRange),
                new TaskRangedAttack(transform, agent, firePoint, projectilePrefab,
                    animator, attackCooldown, projectileSpeed, health) // ✅
            }),
            new TaskPatrol(transform, agent, waypoints, animator)
        });

        return root;
    }
}