using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class TaskCharge : Node
    {
        private Transform transform;
        private NavMeshAgent agent;
        private Animator animator;
        private float chargeSpeed;
        private float normalSpeed;
        private float chargeDuration;
        private float chargeCooldown;
        private int chargeDamage;

        private bool isCharging = false;
        private float chargeTimer = 0f;
        private float cooldownTimer = 0f;
        private Vector3 chargeDirection;

        public TaskCharge(Transform transform, NavMeshAgent agent, Animator animator,
            float chargeSpeed, float chargeDuration, float cooldown, int damage)
        {
            this.transform = transform;
            this.agent = agent;
            this.animator = animator;
            this.chargeSpeed = chargeSpeed;
            this.normalSpeed = agent.speed;
            this.chargeDuration = chargeDuration;
            this.chargeCooldown = cooldown;
            this.chargeDamage = damage;
            
            // Disable automatic NavMeshAgent rotation
            agent.updateRotation = false;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                return state = NodeState.Running;
            }

            if (!isCharging)
            {
                // Instant direction change before charge
                EnemyFacing.FaceTarget(transform, target);
                
                // Start charging - Z-axis only
                isCharging = true;
                chargeTimer = chargeDuration;
                
                // Charge direction only on Z-axis
                float zDirection = target.position.z > transform.position.z ? 1f : -1f;
                chargeDirection = new Vector3(0, 0, zDirection);
                
                agent.speed = chargeSpeed;
                animator?.SetTrigger("Charge");
            }

            if (isCharging)
            {
                chargeTimer -= Time.deltaTime;
                
                // Continue charge in chosen direction (Z-axis only)
                Vector3 chargeTarget = transform.position + chargeDirection * 10f;
                chargeTarget.x = transform.position.x; // Lock X-axis
                agent.SetDestination(chargeTarget);

                if (chargeTimer <= 0)
                {
                    isCharging = false;
                    agent.speed = normalSpeed;
                    cooldownTimer = chargeCooldown;
                    return state = NodeState.Success;
                }
            }

            return state = NodeState.Running;
        }
    }
}