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
            Connections = new List<Connection>();
        }

        public virtual Guid Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual IList<Connection> Connections { get; protected set; }

        /// <summary>
        /// Adds connection between this and the specified end node
        /// </summary>
        /// <param name="end"></param>
        /// <param name="quality">The properties of the connection between this node and the end node</param>
        /// <param name="endQuality">The quality of the connection between the end node and this node</param>
        public virtual void AddConnection(Node end, ConnectionQuality quality, ConnectionQuality endQuality)
        {
            if (end == this)
            {
                throw new InvalidOperationException("Cannot add a connection from a node to itself");
            }
            if (Connections.Any(x => x.End == end))
            {
                throw new ArgumentException("A connection already exists with this node", "end");
            }
            var relatedNode = new Connection(this, end, quality);
            Connections.Add(relatedNode);
            end.AddConnectionInternal(this, endQuality);
        }

        protected void AddConnectionInternal(Node end, ConnectionQuality quality)
        {
            var relatedNode = new Connection(this, end, quality);
            Connections.Add(relatedNode);
        }

        public virtual bool IsConnectedTo(Node node)
        {
            return Connections.Any(x => x.End == node);
        }

        public virtual Connection GetConnection(Node node)
        {
            if(!IsConnectedTo(node))
            {
                throw new ArgumentException("Not connected to node " + node);
            }
            return Connections.Single(x => x.End == node);
        }

        public virtual void RemoveConnection(Node node)
        {
            if (!IsConnectedTo(node))
            {
                throw new ArgumentException("Not related to node " + node);
            }
            var relatedNode = GetConnection(node);
            Connections.Remove(relatedNode);
            node.RemoveConnectionInternal(this);
        }

        protected void RemoveConnectionInternal(Node end)
        {
            var relatedNode = Connections.Single(x => x.End == end);
            Connections.Remove(relatedNode);
        }

        public virtual void RemoveAllConnections()
        {
            foreach (Node end in Connections.Select(x => x.End))
            {
                end.RemoveConnectionInternal(this);
            }
            Connections.Clear();
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
