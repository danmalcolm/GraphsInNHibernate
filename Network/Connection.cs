using System;

namespace Network
{
    public class Connection : IEquatable<Connection>
    {
        public Connection(Node start, Node end, ConnectionQuality quality)
        {
            Start = start;
            End = end;
            Quality = quality;
        }

        protected Connection() {}
        
        public virtual Node Start { get; protected set; }

        public virtual Node End { get; protected set; }

        public virtual ConnectionQuality Quality { get; protected set; }

        public void UpdateQuality(ConnectionQuality quality)
        {
            Quality = quality;
        }

        #region equals 
        public bool Equals(Connection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Start, Start) && Equals(other.End, End);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Connection)) return false;
            return Equals((Connection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode()*397) ^ End.GetHashCode();
            }
        }

        public static bool operator ==(Connection left, Connection right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Connection left, Connection right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}