using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class TaskPatrol : Node
    {
        private Transform transform;
        private NavMeshAgent agent;
        private Transform[] waypoints;
        private int currentWaypointIndex = 0;
        private float waitTime = 1f;
        private float waitCounter = 0f;
        private bool isWaiting = false;
        private Animator animator;

        public TaskPatrol(Transform transform, NavMeshAgent agent, Transform[] waypoints, Animator animator)
        {
            this.transform = transform;
            this.agent = agent;
            this.waypoints = waypoints;
            this.animator = animator;
        }

        public override NodeState Evaluate()
        {
            if (waypoints == null || waypoints.Length == 0)
                return state = NodeState.Failure;

            if (!agent.enabled || !agent.isOnNavMesh)
                return state = NodeState.Running;

            agent.updateRotation = false;

            // Only enable movement if not controlled by another node
            if (agent.isStopped)
            {
                agent.isStopped = false;
            }

            if (isWaiting)
            {
                agent.velocity = Vector3.zero;
                animator?.SetBool("isWalking", false);
                waitCounter += Time.deltaTime;

                if (waitCounter >= waitTime)
                {
                    isWaiting = false;
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                }
                return state = NodeState.Running;
            }

            Transform wp = waypoints[currentWaypointIndex];
            EnemyFacing.FaceDirection(transform, wp.position);

            float distanceZ = Mathf.Abs(transform.position.z - wp.position.z);

            if (distanceZ < 0.5f)
            {
                isWaiting = true;
                waitCounter = 0f;
                agent.velocity = Vector3.zero;
                animator?.SetBool("isWalking", false);
            }
            else
            {
                Vector3 destination = EnemyFacing.GetConstrainedPosition(transform.position, wp.position);
                agent.SetDestination(destination);
                animator?.SetBool("isWalking", true);
            }

            return state = NodeState.Running;
        }

        // Called when patrol is interrupted - reset waiting state
        public void ResetPatrol()
        {
            isWaiting = false;
            waitCounter = 0f;
        }
    }
}