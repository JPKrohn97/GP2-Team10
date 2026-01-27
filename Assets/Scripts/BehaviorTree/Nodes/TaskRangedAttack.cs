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

            // Instant direction change - left/right only
            EnemyFacing.FaceTarget(transform, target);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                animator?.SetTrigger("Attack");
                
                // Shoot projectile - only on Z-axis
                if (projectilePrefab != null && firePoint != null)
                {
                    GameObject projectile = Object.Instantiate(projectilePrefab, 
                        firePoint.position, firePoint.rotation);
                    
                    // Projectile direction only on Z-axis
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