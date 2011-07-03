namespace Network
{
    public class RelatedNode
    {
        public RelatedNode(Node node, Relationship relationship)
        {
            Node = node;
            Relationship = relationship;
        }

        protected RelatedNode() {}

        public virtual Node Node { get; protected set; }

        public virtual Relationship Relationship { get; protected set; }
    }
}