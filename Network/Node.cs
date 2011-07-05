using System;
using System.Collections.Generic;
using System.Linq;

namespace Network
{
    public class Node : IEquatable<Node>
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

        public virtual void LinkTo(Node end, int distance)
        {
            if (end == this)
            {
                throw new InvalidOperationException("Cannot relate a Node to itself");
            }
            if (RelatedNodes.Any(x => x.End == end))
            {
                throw new ArgumentException("This node is already related to the node", "end");
            }
            var relationship = new Relationship(distance);
            var relatedNode = new RelatedNode(this, end, relationship);
            RelatedNodes.Add(relatedNode);
            end.AddRelatedNodeInternal(this, relationship);
        }

        protected void AddRelatedNodeInternal(Node end, Relationship relationship)
        {
            var relatedNode = new RelatedNode(this, end, relationship);
            RelatedNodes.Add(relatedNode);
        }

        public virtual bool IsRelatedTo(Node node)
        {
            return RelatedNodes.Any(x => x.End == node);
        }

        public virtual RelatedNode GetLink(Node node)
        {
            if(!IsRelatedTo(node))
            {
                throw new ArgumentException("Not related to node " + node);
            }
            return RelatedNodes.Single(x => x.End == node);
        }

        public virtual void UnlinkFrom(Node node)
        {
            if (!IsRelatedTo(node))
            {
                throw new ArgumentException("Not related to node " + node);
            }
            var relatedNode = GetLink(node);
            RelatedNodes.Remove(relatedNode);
            node.RemoveRelatedNodeInternal(this);
        }

        protected void RemoveRelatedNodeInternal(Node node)
        {
            var relatedNode = RelatedNodes.Single(x => x.End == node);
            RelatedNodes.Remove(relatedNode);
        }

        #region Equals / hashcode 

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
            if (!typeof (Node).IsAssignableFrom(obj.GetType())) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(Node left, Node right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Node left, Node right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Name: {0}", Name);
        }

        #endregion

        
    }
}
