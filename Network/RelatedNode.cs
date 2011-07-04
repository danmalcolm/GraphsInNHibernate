using System;

namespace Network
{
    public class RelatedNode
    {
        public RelatedNode(Node end, Relationship relationship)
        {
            End = end;
            Relationship = relationship;
        }

        protected RelatedNode() {}
        
        public virtual Node End { get; protected set; }

        public virtual Relationship Relationship { get; protected set; }
    }
}