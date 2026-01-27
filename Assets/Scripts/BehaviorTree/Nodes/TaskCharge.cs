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
                // Start charging
                isCharging = true;
                chargeTimer = chargeDuration;
                chargeDirection = (target.position - transform.position).normalized;
                agent.speed = chargeSpeed;
                animator?.SetTrigger("Charge");
            }

            if (isCharging)
            {
                chargeTimer -= Time.deltaTime;
                
                // Continue charge in the chosen direction
                agent.SetDestination(new Vector3(0, transform.position.y + chargeDirection.y, transform.position.z + chargeDirection.z*10f));

                if (chargeTimer <= 0)
                {
                    // End charge
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