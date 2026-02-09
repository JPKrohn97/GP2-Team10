using UnityEngine;

namespace BehaviorTree
{
    public class CheckInAttackRange : Node
    {
        private Transform transform;
        private float attackRange;
        private float verticalTolerance = 2f;

        public CheckInAttackRange(Transform transform, float attackRange, float verticalTolerance = 2f)
        {
            this.transform = transform;
            this.attackRange = attackRange;
            this.verticalTolerance = verticalTolerance;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            // Check distance on Z-axis
            float distanceZ = Mathf.Abs(target.position.z - transform.position.z);
            
            // Check vertical difference (Y-axis)
            float distanceY = Mathf.Abs(target.position.y - transform.position.y);
            
            // Both conditions must be met
            if (distanceZ <= attackRange && distanceY <= verticalTolerance)
                return state = NodeState.Success;
                
            return state = NodeState.Failure;
        }
    }
}