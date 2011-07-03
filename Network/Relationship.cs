using System;

namespace Network
{
    public class Relationship
    {
        public Relationship(int distance)
        {
            Distance = distance;
        }

        protected Relationship()
        { }

        public virtual Guid Id { get; protected set; }

        public virtual int Distance { get; protected set; }
    }
}