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
        private float maxChargeDistance = 12f;
        private float accelerationTime;
        private float chargeCooldown;
        private string chargeTrigger;

        private float lastChargeTime = -999f;
        private bool isCharging = false;
        private float chargeEndTime;
        private float chargeStartTime;
        private Vector3 chargeDirection;
        private Vector3 chargeStartPosition;
        private Quaternion lockedRotation;
        private float currentChargeSpeed = 0f;

        public TaskCharge(Transform transform, NavMeshAgent agent, Animator animator,
            float chargeSpeed, float chargeDuration, float cooldown,
            float accelerationTime = 0.5f, string chargeTrigger = "Charge")
        {
            this.transform = transform;
            this.agent = agent;
            this.animator = animator;
            this.chargeSpeed = chargeSpeed;
            this.originalSpeed = agent.speed;
            this.chargeDuration = chargeDuration;
            this.chargeCooldown = cooldown;
            this.chargeTrigger = chargeTrigger;
            this.accelerationTime = accelerationTime;

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
                float distanceTraveled = Vector3.Distance(chargeStartPosition, transform.position);
                
                if (Time.time >= chargeEndTime || distanceTraveled >= maxChargeDistance)
                {
                    EndCharge();
                    return state = NodeState.Success;
                }

                // LOCK rotation
                transform.rotation = lockedRotation;
                
                // SMOOTH ACCELERATION with clamped progress
                float timeSinceStart = Time.time - chargeStartTime;
                float chargeProgress = Mathf.Clamp01(timeSinceStart / accelerationTime);
                currentChargeSpeed = Mathf.Lerp(0f, chargeSpeed, chargeProgress);
                
                // Apply velocity
                agent.isStopped = false;
                agent.velocity = chargeDirection * currentChargeSpeed;
                
                // Debug to see acceleration
                if (Time.frameCount % 10 == 0)
                {
                    Debug.Log($"<color=green>Charging - Progress: {chargeProgress:F2}, Speed: {currentChargeSpeed:F2}/{chargeSpeed}</color>");
                }
                
                return state = NodeState.Running;
            }

            // Cooldown
            if (Time.time - lastChargeTime < chargeCooldown)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                EnemyFacing.FaceTarget(transform, target);
                return state = NodeState.Running;
            }

            // Start charge
            StartCharge(target);
            return state = NodeState.Running;
        }

        private void StartCharge(Transform target)
        {
            float directionZ = target.position.z - transform.position.z;
            float direction = directionZ > 0 ? 1f : -1f;

            chargeDirection = new Vector3(0, 0, direction).normalized;
            
            EnemyFacing.FaceTarget(transform, target);
            lockedRotation = transform.rotation;
            
            chargeStartPosition = transform.position;
            chargeStartTime = Time.time;
            currentChargeSpeed = 0f; // Start from 0
            
            isCharging = true;
            chargeEndTime = Time.time + chargeDuration;
            SoundManager.Instance.PlaySound(SoundManager.Instance.ChargedAttack);

            SafeSetTrigger(chargeTrigger);
            
            Debug.Log($"<color=cyan>Charge Started! Acceleration: 0 ? {chargeSpeed} over {accelerationTime}s</color>");
        }

        private void EndCharge()
        {
            isCharging = false;
            lastChargeTime = Time.time;
            currentChargeSpeed = 0f;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            
            float totalDistance = Vector3.Distance(chargeStartPosition, transform.position);
            Debug.Log($"<color=yellow>Charge Ended - Distance: {totalDistance:F2}, Final Speed: {currentChargeSpeed:F2}</color>");
        }

        private void StopCharge()
        {
            if (isCharging)
            {
                isCharging = false;
                currentChargeSpeed = 0f;
            }
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
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