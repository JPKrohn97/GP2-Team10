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
            object target = GetData("target");
            if (target == null)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
                if (colliders.Length > 0)
                {
                    parent.parent.SetData("target", colliders[0].transform);
                    return state = NodeState.Success;
                }
                return state = NodeState.Failure;
            }

            Transform targetTransform = (Transform)target;
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            
            // Hysteresis to prevent flickering
            if (distance > detectionRange * 1.2f)
            {
                ClearData("target");
                return state = NodeState.Failure;
            }
            return state = NodeState.Success;
        }
    }
}