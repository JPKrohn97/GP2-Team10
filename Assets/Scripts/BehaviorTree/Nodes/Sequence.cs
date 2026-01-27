using System.Collections.Generic;

namespace BehaviorTree
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Failure:
                        return state = NodeState.Failure;
                    case NodeState.Running:
                        return state = NodeState.Running;
                    case NodeState.Success:
                        continue;
                }
            }
            
            return state = NodeState.Success;
        }
    }
}