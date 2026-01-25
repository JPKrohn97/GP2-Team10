using System.Collections.Generic;

namespace BehaviorTree
{
    // Returns Success if any child returns Success (OR logic)
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure: continue;
                    case NodeState.Success: return state = NodeState.Success;
                    case NodeState.Running: return state = NodeState.Running;
                }
            }
            return state = NodeState.Failure;
        }
    }
}