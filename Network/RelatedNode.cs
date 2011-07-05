using System;

namespace Network
{
    public class RelatedNode
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


    }
}