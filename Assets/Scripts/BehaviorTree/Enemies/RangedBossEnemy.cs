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
    public Transform firePoint;

    [Header("Light Projectile Attack")]
    public float lightAttackCooldown = 2f;
    public int lightAttackDamage = 12;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    [Header("Heavy AOE Attack")]
    public float heavyAttackCooldown = 6f;
    public int heavyAttackDamage = 25;
    public float aoeRadius = 3f;
    public float aoeSpeed = 8f;
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
                new TaskRangedBossAttack(transform, agent, firePoint, animator,
                    lightAttackCooldown, lightAttackDamage,
                    projectilePrefab, projectileSpeed,
                    heavyAttackCooldown, heavyAttackDamage,
                    aoeRadius, aoeSpeed, heavyAttackChance)
            }),
            new TaskPatrol(transform, agent, waypoints, animator)
        });

        return root;
    }
}