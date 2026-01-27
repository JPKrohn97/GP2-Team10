using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class TaskGoToTarget : Node
    {
        private Transform transform;
        private NavMeshAgent agent;
        private float attackRange;

        public TaskGoToTarget(Transform transform, NavMeshAgent agent, float attackRange)
        {
            this.transform = transform;
            this.agent = agent;
            this.attackRange = attackRange;
            
            // Disable automatic NavMeshAgent rotation
            agent.updateRotation = false;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            // Instant direction change
            EnemyFacing.FaceTarget(transform, target);

            float distance = Vector3.Distance(transform.position, target.position);
            
            if (distance > attackRange)
            {
                agent.isStopped = false;
                
                // Movement only on Z-axis (keep enemy's X and Y)
                Vector3 destination = EnemyFacing.GetConstrainedPosition(transform.position, target.position);
                agent.SetDestination(destination);
                
                return state = NodeState.Running;
            }
            
            agent.isStopped = true;
            return state = NodeState.Success;
        }
    }
}