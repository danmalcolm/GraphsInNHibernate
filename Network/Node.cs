using System;
using System.Collections.Generic;
using System.Linq;

namespace Network
{
    public class Node
    {
        public Node(string name) : this()
        {
            Name = name;
        }

        protected Node()
        {
            RelatedNodes = new List<RelatedNode>();
        }

        public virtual Guid Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual IList<RelatedNode> RelatedNodes { get; protected set; }

        public virtual void LinkTo(Node node, int distance)
        {
            if (node == this)
            {
                throw new InvalidOperationException("Cannot relate a Node to itself");
            }
            if (RelatedNodes.Any(x => x.Node == node))
            {
                throw new ArgumentException("This node is already related to the node", "node");
            }
            var relationship = new Relationship(distance);
            var relatedNode = new RelatedNode(node, relationship);
            RelatedNodes.Add(relatedNode);
            node.AddRelatedNodeInternal(this, relationship);
        }

        protected void AddRelatedNodeInternal(Node node, Relationship relationship)
        {
            var relatedNode = new RelatedNode(node, relationship);
            RelatedNodes.Add(relatedNode);
        }

        public virtual bool Equals(Node other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Node)) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
