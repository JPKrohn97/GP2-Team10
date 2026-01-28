using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class MeleeEnemyBT : BehaviorTreeBase
{
    [Header("Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;
    public LayerMask playerLayer;


    [Header("Patrol")]
    public Transform[] waypoints;

    [Header("Components")]
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
            // Attack sequence (priority)
            new Sequence(new List<Node>
            {
                new CheckPlayerInRange(transform, detectionRange, playerLayer),
                new TaskGoToTarget(transform, agent, attackRange),
                new TaskAttack(transform, animator, attackCooldown, attackDamage)
            }),
            // Patrol (when player is not in range)
            new TaskPatrol(transform, agent, waypoints, animator)
        });

        return root;
    }
    public void EnableCollider() 
    {
    
    }
}