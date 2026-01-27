using UnityEngine;

namespace BehaviorTree
{
    public abstract class BehaviorTreeBase : MonoBehaviour
    {
        protected Node root = null;

        protected void Start()
        {
            root = SetupTree();
        }

        private void Update()
        {
            root?.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}