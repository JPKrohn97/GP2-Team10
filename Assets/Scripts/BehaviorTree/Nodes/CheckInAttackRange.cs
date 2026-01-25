using UnityEngine;

namespace BehaviorTree
{
    public class CheckInAttackRange : Node
    {
        private Transform transform;
        private float attackRange;

        public CheckInAttackRange(Transform transform, float range)
        {
            this.transform = transform;
            this.attackRange = range;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            float distance = Vector3.Distance(transform.position, target.position);
            return state = distance <= attackRange ? NodeState.Success : NodeState.Failure;
        }
    }
}