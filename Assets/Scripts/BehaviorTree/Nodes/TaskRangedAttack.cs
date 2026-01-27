using UnityEngine;

namespace BehaviorTree
{
    public class TaskRangedAttack : Node
    {
        private Transform transform;
        private Transform firePoint;
        private GameObject projectilePrefab;
        private Animator animator;
        private float attackCooldown;
        private float lastAttackTime;
        private float projectileSpeed;

        public TaskRangedAttack(Transform transform, Transform firePoint, GameObject projectile, 
            Animator animator, float cooldown, float speed)
        {
            this.transform = transform;
            this.firePoint = firePoint;
            this.projectilePrefab = projectile;
            this.animator = animator;
            this.attackCooldown = cooldown;
            this.projectileSpeed = speed;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            // Rotate towards the player
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                animator?.SetTrigger("Attack");
                
                // Shoot projectile
                if (projectilePrefab != null && firePoint != null)
                {
                    GameObject projectile = Object.Instantiate(projectilePrefab, 
                        firePoint.position, firePoint.rotation);
                    
                    Vector3 shootDir = (target.position - firePoint.position).normalized;
                    if (projectile.TryGetComponent<Rigidbody>(out var rb))
                        rb.linearVelocity = shootDir * projectileSpeed;
                }
            }

            return state = NodeState.Running;
        }
    }
}