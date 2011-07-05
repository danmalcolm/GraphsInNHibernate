using System;

namespace Network
{
    public class RelatedNode : IEquatable<RelatedNode>
    {
        public RelatedNode(Node start, Node end, Relationship relationship)
        {
            Start = start;
            End = end;
            Relationship = relationship;
        }

        protected RelatedNode() {}
        
        public virtual Node Start { get; protected set; }

        public virtual Node End { get; protected set; }

        public virtual Relationship Relationship { get; protected set; }

        #region equals 
        public bool Equals(RelatedNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Start, Start) && Equals(other.End, End);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RelatedNode)) return false;
            return Equals((RelatedNode) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode()*397) ^ End.GetHashCode();
            }
        }

        public static bool operator ==(RelatedNode left, RelatedNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RelatedNode left, RelatedNode right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}