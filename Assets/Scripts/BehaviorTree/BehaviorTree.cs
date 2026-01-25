using System.Collections.Generic;

namespace BehaviorTree
{
    public enum NodeState { Running, Success, Failure }

    public abstract class Node
    {
        protected NodeState state;
        public Node parent;
        protected List<Node> children = new List<Node>();
        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public Node() { parent = null; }
        public Node(List<Node> children) : this()
        {
            foreach (Node child in children)
                Attach(child);
        }

        public void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public abstract NodeState Evaluate();

        public void SetData(string key, object value) => dataContext[key] = value;

        public object GetData(string key)
        {
            if (dataContext.TryGetValue(key, out object value))
                return value;
            return parent?.GetData(key);
        }

        public bool ClearData(string key)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }
            return parent != null && parent.ClearData(key);
        }
    }
}