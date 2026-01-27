using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            // Always check from the beginning (priority order)
            foreach (Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Running:
                        return state = NodeState.Running;
                    case NodeState.Success:
                        return state = NodeState.Success;
                    case NodeState.Failure:
                        continue;
                }
            }
            
            return state = NodeState.Failure;
        }
    }
}