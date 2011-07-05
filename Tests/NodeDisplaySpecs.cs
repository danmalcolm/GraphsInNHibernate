using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using NUnit.Framework;

namespace Tests.NodeDisplaySpecs
{
    public class when_displaying_network_of_5_nodes : ContextSpecification
    {
        private string connections;

        /// <summary>
        /// Creates sequence of nodes, named A, B, C, D and E, links them in a
        /// A -> B -> C -> D -> E and returns the start node (node A)
        /// </summary>
        /// <returns></returns>
        protected Node BuildLinkedSequenceOfNodesAtoE()
        {
            var nodes = "ABCDE".Select(name => new Node(name.ToString())).ToList();
            var quality = new ConnectionQuality(10, 1);
            nodes[0].AddConnection(nodes[1], quality, quality);
            nodes[1].AddConnection(nodes[2], quality, quality);
            nodes[2].AddConnection(nodes[3], quality, quality);
            nodes[3].AddConnection(nodes[4], quality, quality);
            return nodes.First();
        }

        protected override void because()
        {
            var start = BuildLinkedSequenceOfNodesAtoE();
            connections = start.DisplayConnections(3);
            Console.WriteLine(connections);
        }

        [Test]
        public void should_render_nice_summary()
        {
            Assert.That(connections, Is.Not.Empty);
        }
    }
}