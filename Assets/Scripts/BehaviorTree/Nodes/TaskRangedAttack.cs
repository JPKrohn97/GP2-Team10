using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class TaskRangedAttack : Node
    {
        private Transform transform;
        private Transform firePoint;
        private GameObject projectilePrefab;
        private Animator animator;
        private NavMeshAgent agent;
        private float attackCooldown;
        private float lastAttackTime;
        private float projectileSpeed;

        private EnemyHealth enemyHealth;

        public TaskRangedAttack(
            Transform transform,
            NavMeshAgent agent,
            Transform firePoint,
            GameObject projectile,
            Animator animator,
            float cooldown,
            float speed,
            EnemyHealth enemyHealth 
        )
        {
            this.transform = transform;
            this.agent = agent;
            this.firePoint = firePoint;
            this.projectilePrefab = projectile;
            this.animator = animator;
            this.attackCooldown = cooldown;
            this.projectileSpeed = speed;
            this.enemyHealth = enemyHealth;

            agent.updateRotation = false;
        }

        public override NodeState Evaluate()
        {
            if (enemyHealth != null && enemyHealth.IsDead)
                return state = NodeState.Failure;

            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            agent.isStopped = true;
            EnemyFacing.FaceTarget(transform, target);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                if (enemyHealth != null && enemyHealth.IsDead)
                    return state = NodeState.Failure;

                lastAttackTime = Time.time;
                animator?.SetTrigger("Attack");

                if (projectilePrefab != null && firePoint != null&& !enemyHealth.IsDead )
                {
                    GameObject projectile = Object.Instantiate(
                        projectilePrefab,
                        firePoint.position,
                        firePoint.rotation
                    );

                    float zDir = target.position.z > transform.position.z ? 1f : -1f;
                    Vector3 shootDir = new Vector3(0, 0, zDir);

                    if (projectile.TryGetComponent<Rigidbody>(out var rb))
                        rb.linearVelocity = shootDir * projectileSpeed;
                }
            }

            return state = NodeState.Running;
        }
    }
}
