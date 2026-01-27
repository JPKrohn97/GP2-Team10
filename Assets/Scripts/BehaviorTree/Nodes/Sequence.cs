using System.Collections.Generic;

namespace BehaviorTree
{
    // Returns Success if all children return Success (AND logic)
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure: return state = NodeState.Failure;
                    case NodeState.Success: continue;
                    case NodeState.Running:
                        anyChildRunning = true;
                        continue;
                }
            }
            return state = anyChildRunning ? NodeState.Running : NodeState.Success;
        }
    }
}