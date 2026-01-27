using UnityEngine;

namespace BehaviorTree
{
    public class CheckPlayerInRange : Node
    {
        private Transform transform;
        private float detectionRange;
        private LayerMask playerLayer;

        public CheckPlayerInRange(Transform transform, float range, LayerMask playerLayer)
        {
            this.transform = transform;
            this.detectionRange = range;
            this.playerLayer = playerLayer;
        }

        public override NodeState Evaluate()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
            
            if (colliders.Length > 0)
            {
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
                
                if (closestTarget != null && closestDistance <= detectionRange)
                {
                    // Store in ROOT node so all siblings can access it
                    GetRoot().SetData("target", closestTarget);
                    return state = NodeState.Success;
                }
            }
            
            // Clear from root
            GetRoot().ClearData("target");
            return state = NodeState.Failure;
        }
    }
}