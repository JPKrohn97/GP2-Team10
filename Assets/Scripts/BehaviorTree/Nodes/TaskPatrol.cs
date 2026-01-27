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

        public TaskPatrol(Transform transform, NavMeshAgent agent, Transform[] waypoints)
        {
            this.transform = transform;
            this.agent = agent;
            this.waypoints = waypoints;
        }

        public override NodeState Evaluate()
        {
            if (waypoints == null || waypoints.Length == 0)
                return state = NodeState.Failure;
            
            if (!agent.enabled || !agent.isOnNavMesh)
                return state = NodeState.Running;
            
            agent.updateRotation = false;
            agent.isStopped = false;

            if (isWaiting)
            {
                agent.velocity = Vector3.zero;
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
            }
            else
            {
                Vector3 destination = EnemyFacing.GetConstrainedPosition(transform.position, wp.position);
                agent.SetDestination(destination);
            }
            
            return state = NodeState.Running;
        }
    }
}