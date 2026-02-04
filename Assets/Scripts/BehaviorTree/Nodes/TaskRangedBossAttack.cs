using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class TaskRangedBossAttack : Node
    {
        private Transform transform;
        private Transform firePoint;
        private Animator animator;
        private NavMeshAgent agent;
        
        private float lightCooldown;
        private float heavyCooldown;
        private float heavyChance;
        private float dashBackDistance;
        private float closeRangeThreshold;

        private float lightTimer = 0f;
        private float heavyTimer = 0f;
        private int attacksSinceLastHeavy = 0;
        private bool isDashing = false;
        private float dashTimer = 0f;
        private float dashDuration = 0.5f;
        
        private bool shouldDashAfterAttack = false;
        private float attackFinishTimer = 0f;
        private float attackFinishDelay = 0.5f;
        
        private float emergencyDashCooldown = 3f;
        private float emergencyDashTimer = 0f;
        
        private Vector3 dashTarget;
        private float dashSpeed = 16f;

        public TaskRangedBossAttack(
            Transform transform,
            NavMeshAgent agent,
            Transform firePoint,
            Animator animator,
            float lightCooldown,
            float heavyCooldown,
            float heavyChance,
            float dashBackDistance,
            float closeRangeThreshold)
        {
            this.transform = transform;
            this.agent = agent;
            this.firePoint = firePoint;
            this.animator = animator;
            this.lightCooldown = lightCooldown;
            this.heavyCooldown = heavyCooldown;
            this.heavyChance = heavyChance;
            this.dashBackDistance = dashBackDistance;
            this.closeRangeThreshold = closeRangeThreshold;

            if (agent != null)
                agent.updateRotation = false;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            // Handle dash backwards
            if (isDashing)
            {
                if (agent != null)
                    agent.isStopped = true;
                
                Vector3 direction = (dashTarget - transform.position).normalized;
                float step = dashSpeed * Time.deltaTime;
                Vector3 newPosition = transform.position + new Vector3(direction.x * step, 0, direction.z * step);
                
                NavMeshHit hit;
                if (NavMesh.SamplePosition(newPosition, out hit, 1f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                }
                
                dashTimer -= Time.deltaTime;
                float distanceToTarget = Vector3.Distance(transform.position, dashTarget);
                
                if (dashTimer <= 0f || distanceToTarget < 0.5f)
                {
                    isDashing = false;
                }
                return state = NodeState.Running;
            }
            
            // Handle dash delay after close-range attack
            if (shouldDashAfterAttack)
            {
                attackFinishTimer -= Time.deltaTime;
                if (attackFinishTimer <= 0f)
                {
                    shouldDashAfterAttack = false;
                    DashBackwards(target);
                }
                return state = NodeState.Running;
            }
            
            // Only stop agent when NOT dashing and NOT preparing to dash
            if (agent != null && !isDashing && !shouldDashAfterAttack)
                agent.isStopped = true;
                
            EnemyFacing.FaceTarget(transform, target);

            if (IsAttackAnimationPlaying())
            {
                return state = NodeState.Running;
            }

            // Check if player is too close - EMERGENCY DASH
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            
            emergencyDashTimer -= Time.deltaTime;

            if (distanceToPlayer < closeRangeThreshold && emergencyDashTimer <= 0)
            {
                lightTimer = lightCooldown;
                emergencyDashTimer = emergencyDashCooldown;
                animator?.SetTrigger("Attack");
                SoundManager.Instance.PlaySound(SoundManager.Instance.BasicBossAttack,transform.gameObject);
                ShootNormalProjectile();
                
                shouldDashAfterAttack = true;
                attackFinishTimer = attackFinishDelay;
                
                return state = NodeState.Running;
            }

            lightTimer -= Time.deltaTime;
            heavyTimer -= Time.deltaTime;

            // Normal attack pattern when at safe distance
            if (lightTimer <= 0)
            {
                bool heavyAvailable = heavyTimer <= 0;
                bool canDashBack = CanDashBackwards(target);
                
                bool shouldUseHeavy;
                if (canDashBack)
                {
                    shouldUseHeavy = heavyAvailable && (Random.value < heavyChance || attacksSinceLastHeavy >= 3);
                }
                else
                {
                    shouldUseHeavy = heavyAvailable && Random.value < 0.5f;
                }
                
                // HEAVY ATTACK: Multi-angle projectiles
                if (shouldUseHeavy)
                {
                    heavyTimer = heavyCooldown;
                    lightTimer = lightCooldown;
                    attacksSinceLastHeavy = 0;
                    SoundManager.Instance.PlaySound(SoundManager.Instance.FireBlast,transform.gameObject);
                    ShootMultiAngleProjectiles(target);
                    
                    return state = NodeState.Running;
                }
                // LIGHT ATTACK: Single projectile
                else
                {
                    lightTimer = lightCooldown;
                    
                    if (heavyAvailable)
                    {
                        attacksSinceLastHeavy++;
                    }
                    
                    animator?.SetTrigger("Attack");
                    ShootNormalProjectile();
                    
                    return state = NodeState.Running;
                }
            }

            return state = NodeState.Running;
        }

        private bool IsAttackAnimationPlaying()
        {
            if (animator == null) return false;
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            bool isPlayingLightAttack = stateInfo.IsName("LightAttack");
            bool isPlayingDash = stateInfo.IsName("Jump");
            
            return isPlayingLightAttack || isPlayingDash;
        }

        private void ShootNormalProjectile()
        {
            if (firePoint == null)
                return;

            Transform target = (Transform)GetData("target");
            if (target == null)
                return;

            Vector3 baseDirection = (target.position - firePoint.position).normalized;
            Vector3 horizontalDirection = new Vector3(baseDirection.x, 0, baseDirection.z).normalized;

            GameObject projectile = ManagerObjectPool.Instance.Spawn(
                ObjectPoolType.BossProjectile,
                firePoint.position,
                Quaternion.LookRotation(horizontalDirection)
            );

            if (projectile != null)
            {
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.linearVelocity = horizontalDirection * 30f;
                }
            }
        }

        private void ShootMultiAngleProjectiles(Transform target)
        {
            GameObject multiShot = new GameObject("MultiAngleShot");
            multiShot.transform.position = firePoint.position;
            
            var multiAngle = multiShot.AddComponent<MultiAngleProjectile>();
            multiAngle.Initialize(firePoint, target, 15, 25f);
        }

        private bool CanDashBackwards(Transform target)
        {
            if (agent == null) return false;
            
            Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
            Vector3 dashDestination = transform.position + directionAwayFromPlayer * dashBackDistance;
            
            NavMeshHit hit;
            return NavMesh.SamplePosition(dashDestination, out hit, dashBackDistance, NavMesh.AllAreas);
        }

        private void DashBackwards(Transform target)
        {
            if (agent == null)
                return;
            
            Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
            Vector3 dashDestination = transform.position + directionAwayFromPlayer * dashBackDistance;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(dashDestination, out hit, dashBackDistance, NavMesh.AllAreas))
            {
                dashTarget = hit.position;
                isDashing = true;
                dashTimer = dashDuration;
                
                animator?.SetTrigger("Dash");
            }
        }
    }
}