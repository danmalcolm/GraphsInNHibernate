using System;
using System.Linq;
using System.Text;

namespace Network
{
    public static class NodeDisplayExtensions
    {
        public static string DisplayConnections(this Node node, int maxDepth)
        {
            var connections = new StringBuilder();
            node.DisplayConnectionsInternal(null, 0, connections, maxDepth);
            return connections.ToString();
        }

        public static void DisplayConnectionsInternal(this Node node, Node parent, int depth, StringBuilder connections, int maxDepth)
        {
            connections.AppendFormat("{0} Node {1}", ">".PadLeft(depth * 2, '-'), node.Name);
            connections.AppendLine();

            if(depth < maxDepth)
            {
                foreach(var connection in node.Connections)
                {
                    if(parent == null || connection.End != parent)
                    {
                        DisplayConnectionsInternal(connection.End, node, depth + 1, connections, maxDepth);
                    }
                }
                
            }
        }
    }
}