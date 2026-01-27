using UnityEngine;

namespace BehaviorTree
{
    public class CheckInAttackRange : Node
    {
        private Transform transform;
        private float attackRange;

        public CheckInAttackRange(Transform transform, float attackRange)
        {
            this.transform = transform;
            this.attackRange = attackRange;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            // Check distance only on Z-axis
            float distance = Mathf.Abs(target.position.z - transform.position.z);
            
            if (distance <= attackRange)
                return state = NodeState.Success;
                
            return state = NodeState.Failure;
        }
    }
}