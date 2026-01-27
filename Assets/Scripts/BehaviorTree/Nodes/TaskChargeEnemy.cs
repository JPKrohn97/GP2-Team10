using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class TaskChargeEnemy : Node
    {
        private Transform transform;
        private NavMeshAgent agent;
        private Animator animator;
        private Transform[] waypoints;
        private LayerMask playerLayer;
        
        // Settings
        private float detectionRange;
        private float maxChargeSpeed;
        private float chargeDuration;
        private float chargeCooldown;
        private float accelerationTime;
        private string chargeTrigger;
        
        // State
        private enum EnemyState { Patrolling, Charging, WaitingCooldown }
        private EnemyState currentState = EnemyState.Patrolling;
        
        // Charge state
        private float chargeTimer;
        private float chargeDirectionZ;
        private float currentChargeSpeed;
        private float chargeStartTime;
        private float lastChargeTime = -999f;
        
        // Patrol state
        private int currentWaypointIndex = 0;
        private float waitTime = 1f;
        private float waitCounter = 0f;
        private bool isWaitingAtWaypoint = false;

        public TaskChargeEnemy(Transform transform, NavMeshAgent agent, Animator animator,
            Transform[] waypoints, LayerMask playerLayer, float detectionRange,
            float chargeSpeed, float chargeDuration, float chargeCooldown, 
            float accelerationTime = 0.3f, string chargeTrigger = "Charge")
        {
            this.transform = transform;
            this.agent = agent;
            this.animator = animator;
            this.waypoints = waypoints;
            this.playerLayer = playerLayer;
            this.detectionRange = detectionRange;
            this.maxChargeSpeed = chargeSpeed;
            this.chargeDuration = chargeDuration;
            this.chargeCooldown = chargeCooldown;
            this.accelerationTime = accelerationTime;
            this.chargeTrigger = chargeTrigger;
            
            agent.updateRotation = false;
        }

        public override NodeState Evaluate()
        {
            Transform target = FindPlayer();
            bool cooldownReady = Time.time - lastChargeTime >= chargeCooldown;
            
            switch (currentState)
            {
                case EnemyState.Patrolling:
                    if (target != null && cooldownReady)
                    {
                        StartCharge(target);
                        currentState = EnemyState.Charging;
                    }
                    else
                    {
                        Patrol();
                    }
                    break;
                    
                case EnemyState.Charging:
                    if (UpdateCharge())
                    {
                        lastChargeTime = Time.time;
                        currentState = EnemyState.WaitingCooldown;
                    }
                    break;
                    
                case EnemyState.WaitingCooldown:
                    if (target != null && cooldownReady)
                    {
                        StartCharge(target);
                        currentState = EnemyState.Charging;
                    }
                    else if (target == null)
                    {
                        currentState = EnemyState.Patrolling;
                    }
                    else
                    {
                        // Player in range but cooldown not ready - stand still and face player
                        agent.velocity = Vector3.zero;
                        agent.isStopped = true;
                        EnemyFacing.FaceTarget(transform, target);
                    }
                    break;
            }
            
            return state = NodeState.Running;
        }

        private Transform FindPlayer()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
            
            Transform closestTarget = null;
            float closestDistance = float.MaxValue;
            
            foreach (var col in colliders)
            {
                float distanceZ = Mathf.Abs(col.transform.position.z - transform.position.z);
                if (distanceZ < closestDistance)
                {
                    closestDistance = distanceZ;
                    closestTarget = col.transform;
                }
            }
            
            return closestDistance <= detectionRange ? closestTarget : null;
        }

        private void StartCharge(Transform target)
        {
            EnemyFacing.FaceTarget(transform, target);
            
            chargeTimer = chargeDuration;
            chargeStartTime = Time.time;
            currentChargeSpeed = 0f;
            chargeDirectionZ = target.position.z > transform.position.z ? 1f : -1f;
            
            agent.ResetPath();
            agent.isStopped = false;
            
            if (!string.IsNullOrEmpty(chargeTrigger))
                animator?.SetTrigger(chargeTrigger);
        }

        private bool UpdateCharge()
        {
            chargeTimer -= Time.deltaTime;
            
            float timeSinceStart = Time.time - chargeStartTime;
            float accelerationProgress = Mathf.Clamp01(timeSinceStart / accelerationTime);
            currentChargeSpeed = Mathf.Lerp(0f, maxChargeSpeed, EaseInQuad(accelerationProgress));
            
            agent.velocity = new Vector3(0, 0, chargeDirectionZ) * currentChargeSpeed;

            if (chargeTimer <= 0)
            {
                agent.velocity = Vector3.zero;
                return true; // Charge finished
            }
            
            return false; // Still charging
        }

        private void Patrol()
        {
            if (waypoints == null || waypoints.Length == 0)
                return;
            
            agent.isStopped = false;

            if (isWaitingAtWaypoint)
            {
                agent.velocity = Vector3.zero;
                waitCounter += Time.deltaTime;
                if (waitCounter >= waitTime)
                {
                    isWaitingAtWaypoint = false;
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                }
                return;
            }
            
            Transform wp = waypoints[currentWaypointIndex];
            EnemyFacing.FaceDirection(transform, wp.position);
            
            float distanceZ = Mathf.Abs(transform.position.z - wp.position.z);
            
            if (distanceZ < 0.5f)
            {
                isWaitingAtWaypoint = true;
                waitCounter = 0f;
                agent.velocity = Vector3.zero;
            }
            else
            {
                Vector3 destination = EnemyFacing.GetConstrainedPosition(transform.position, wp.position);
                agent.SetDestination(destination);
            }
        }
        
        private float EaseInQuad(float t) => t * t;
    }
}