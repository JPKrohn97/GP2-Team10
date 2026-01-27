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
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            float distance = Vector3.Distance(transform.position, target.position);
            
            if (distance > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
                return state = NodeState.Running;
            }
            
            agent.isStopped = true;
            return state = NodeState.Success;
        }
    }
}