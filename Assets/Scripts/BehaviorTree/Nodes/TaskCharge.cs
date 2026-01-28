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
        private float originalSpeed;
        private float chargeDuration;
        private float chargeCooldown;
        private string chargeTrigger;

        private float lastChargeTime = -999f;
        private bool isCharging = false;
        private float chargeEndTime;
        private Vector3 chargeDestination;

        public TaskCharge(Transform transform, NavMeshAgent agent, Animator animator,
            float chargeSpeed, float chargeDuration, float cooldown,
            float accelerationTime = 0.3f, string chargeTrigger = "Charge")
        {
            this.transform = transform;
            this.agent = agent;
            this.animator = animator;
            this.chargeSpeed = chargeSpeed;
            this.originalSpeed = agent.speed;
            this.chargeDuration = chargeDuration;
            this.chargeCooldown = cooldown;
            this.chargeTrigger = chargeTrigger;

            agent.updateRotation = false;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
            {
                StopCharge();
                return state = NodeState.Failure;
            }

            if (isCharging)
            {
                if (Time.time >= chargeEndTime)
                {
                    EndCharge();
                    return state = NodeState.Success;
                }

                agent.isStopped = false;
                agent.SetDestination(chargeDestination);
                return state = NodeState.Running;
            }

            // Cooldown
            if (Time.time - lastChargeTime < chargeCooldown)
            {
                agent.isStopped = true;
                agent.ResetPath();
                EnemyFacing.FaceTarget(transform, target);
                return state = NodeState.Running;
            }

            // Start charge
            StartCharge(target);
            return state = NodeState.Running;
        }

        private void StartCharge(Transform target)
        {
            float direction = target.position.z > transform.position.z ? 1f : -1f;

            // Calculate destination past player
            Vector3 destination = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z + (direction * 20f)
            );

            // Validate on NavMesh
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(destination, out hit, 2f, NavMesh.AllAreas))
            {
                // Fallback: shorter distance
                destination.z = transform.position.z + (direction * 10f);
                if (!NavMesh.SamplePosition(destination, out hit, 2f, NavMesh.AllAreas))
                {
                    return; // Can't find valid destination
                }
            }

            chargeDestination = hit.position;
            isCharging = true;
            chargeEndTime = Time.time + chargeDuration;

            EnemyFacing.FaceDirection(transform, destination);
            SafeSetTrigger(chargeTrigger);

            agent.ResetPath();
            agent.speed = chargeSpeed;
            agent.isStopped = false;
            agent.SetDestination(chargeDestination);
        }

        private void EndCharge()
        {
            isCharging = false;
            lastChargeTime = Time.time;
            agent.speed = originalSpeed;
            agent.isStopped = true;
            agent.ResetPath();
        }

        private void StopCharge()
        {
            if (isCharging)
            {
                isCharging = false;
                agent.speed = originalSpeed;
            }
            agent.isStopped = true;
            agent.ResetPath();
        }

        private void SafeSetTrigger(string paramName)
        {
            if (animator != null && HasParameter(paramName, AnimatorControllerParameterType.Trigger))
                animator.SetTrigger(paramName);
        }

        private bool HasParameter(string paramName, AnimatorControllerParameterType type)
        {
            foreach (var param in animator.parameters)
                if (param.name == paramName && param.type == type)
                    return true;
            return false;
        }
    }
}