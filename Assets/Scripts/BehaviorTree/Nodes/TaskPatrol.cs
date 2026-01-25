using System.Collections.Generic;
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

            if (isWaiting)
            {
                waitCounter += Time.deltaTime;
                if (waitCounter >= waitTime)
                {
                    isWaiting = false;
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                }
            }
            else
            {
                Transform wp = waypoints[currentWaypointIndex];
                if (Vector3.Distance(transform.position, wp.position) < 0.5f)
                {
                    isWaiting = true;
                    waitCounter = 0f;
                }
                else
                {
                    agent.SetDestination(wp.position);
                }
            }
            return state = NodeState.Running;
        }
    }
}