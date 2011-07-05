using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using NHibernate;
using NUnit.Framework;

namespace Tests.NetworkPersistenceSpecs
{
    /// <summary>
    /// base class for specs
    /// </summary>
    public class PersistenceSpecification : ContextSpecification
    {
        /// <summary>
        /// Index used for sequence of nodes named from A - E
        /// </summary>
        protected const int A = 0;
        protected const int B = 1;
        protected const int C = 2;
        protected const int D = 3;
        protected const int E = 4;

        private readonly ISessionFactory sessionFactory = TestConfigurationSource.SessionFactory;
        protected ConnectionQuality LowConnectionQuality = new ConnectionQuality(2, 1);
        protected ConnectionQuality MediumConnectionQuality = new ConnectionQuality(5, 1);
        protected ConnectionQuality HighConnectionQuality = new ConnectionQuality(10, 1);

        protected void InNewSession(Action<ISession> action)
        {
            using (ISession session = sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                action(session);
                transaction.Commit();
            }
        }

        protected void AssertNodesNotRelated(Node originalNodeA, Node originalNodeB)
        {
            InNewSession(session =>
            {
                var startNode = session.GetNode(originalNodeA.Id);
                Assert.That(startNode.IsConnectedTo(originalNodeB), Is.False, string.Format("{0} is related to {1}", startNode, originalNodeB));
            });
            InNewSession(session =>
            {
                var endNode = session.GetNode(originalNodeB.Id);
                Assert.That(endNode.IsConnectedTo(originalNodeA), Is.False, string.Format("{0} is related to {1}", endNode, originalNodeA));
            });
        }

        protected void AssertNodesRelated(Node originalNodeA, Node originalNodeB)
        {
            InNewSession(session =>
            {
                var startNode = session.GetNode(originalNodeA.Id);
                Assert.That(startNode.IsConnectedTo(originalNodeB), Is.True, string.Format("{0} is not related to {1}", startNode, originalNodeB));
            });
            InNewSession(session =>
            {
                var endNode = session.GetNode(originalNodeB.Id);
                Assert.That(endNode.IsConnectedTo(originalNodeA), Is.True, string.Format("{0} is not related to {1}", endNode, originalNodeA));
            });
        }

        /// <summary>
        /// Creates sequence of nodes, named A, B, C, D and E
        /// </summary>
        /// <returns></returns>
        protected List<Node> CreateNodesAtoE()
        {
            return "ABCDE".Select(name => new Node(name.ToString())).ToList();
        }
    }

    public class when_saving_node_with_no_related_nodes : PersistenceSpecification
    {
        private Node original;

        protected override void because()
        {
            original = new Node("A");
            InNewSession(session => session.SaveOrUpdate(original));
        }

        [Test]
        public void node_should_be_saved()
        {
            InNewSession(session =>
            {
                var retrieved = session.GetNode(original.Id);
                Assert.That(retrieved, Is.Not.Null);
            });
        }
    }

    [TestFixture]
    public class when_saving_node_with_single_connected_node : PersistenceSpecification
    {
        private Node originalStartNode;
        private Node originalEndNode;

        protected override void because()
        {
            InNewSession(session => 
            { 
                originalStartNode = new Node("A");
                originalEndNode = new Node("B");
                originalStartNode.AddConnection(originalEndNode, HighConnectionQuality, MediumConnectionQuality);
                session.SaveOrUpdate(originalStartNode); 
            });
        }

        [Test]
        public void start_node_should_be_saved()
        {
            InNewSession(session =>
            {
                var retrieved = session.GetNode(originalStartNode.Id);
                Assert.That(retrieved, Is.Not.Null);
            });
        }

        [Test]
        public void start_node_on_end_node_should_point_to_owner()
        {
            InNewSession(session =>
            {
                var retrieved = session.GetNode(originalStartNode.Id);
                var relatedNode = retrieved.Connections.Single();
                Assert.That(relatedNode, Is.Not.Null);
                Assert.That(relatedNode.Start, Is.SameAs(retrieved));
            });
        }

        [Test]
        public void end_node_should_be_saved()
        {
            InNewSession(session =>
            {
                var retrieved = session.GetNode(originalEndNode.Id);
                Assert.That(retrieved, Is.Not.Null);
            });
        }

        [Test]
        public void relationship_should_be_saved_between_start_and_end_nodes()
        {
            AssertNodesRelated(originalStartNode, originalEndNode);
        }

        [Test]
        public void related_nodes_on_end_node_should_be_saved()
        {
            InNewSession(session =>
            {
                var endNode = session.GetNode(originalEndNode.Id);
                Assert.That(endNode.Connections.Count, Is.EqualTo(1));
                var startNode = endNode.Connections.Single().End;
                Assert.That(startNode, Is.EqualTo(originalStartNode));
            });
        }
    }

    public class when_saving_node_network_5_levels_deep : PersistenceSpecification
    {
        private List<Node> originalNodes = new List<Node>();
       
        protected override void because()
        {
            InNewSession(session =>
            {
                originalNodes = "ABCDE".Select(name => new Node(name.ToString())).ToList(); // node for each char
                originalNodes[A].AddConnection(originalNodes[B], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[B].AddConnection(originalNodes[C], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[C].AddConnection(originalNodes[D], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[D].AddConnection(originalNodes[E], HighConnectionQuality, MediumConnectionQuality);
                session.SaveOrUpdate(originalNodes[0]);
            });
        }

        [Test]
        public void all_nodes_should_be_saved()
        {
            InNewSession(session =>
            {
                foreach(var originalNode in originalNodes)
                {
                    var retrieved = session.GetNode(originalNode.Id);
                    Assert.That(retrieved, Is.Not.Null, "original node {0} not saved", originalNode);
                }
            });
        }

        [Test]
        public void each_node_should_be_related_to_siblings()
        {
            AssertNodesRelated(originalNodes[0], originalNodes[1]);
            AssertNodesRelated(originalNodes[1], originalNodes[2]);
            AssertNodesRelated(originalNodes[2], originalNodes[3]);
            AssertNodesRelated(originalNodes[3], originalNodes[4]);
        }
    }

    public class when_unlinking_middle_node_from_network_5_levels_deep : PersistenceSpecification
    {
        private List<Node> originalNodes = new List<Node>();
        
        protected override void because()
        {
            InNewSession(session =>
            {
                originalNodes = CreateNodesAtoE(); // node for each char
                originalNodes[A].AddConnection(originalNodes[B], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[B].AddConnection(originalNodes[C], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[C].AddConnection(originalNodes[D], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[D].AddConnection(originalNodes[E], HighConnectionQuality, MediumConnectionQuality);
                session.SaveOrUpdate(originalNodes[A]);
            });

            InNewSession(session =>
            {
                var nodeB = session.GetNode(originalNodes[B].Id);
                var nodeC = session.GetNode(originalNodes[C].Id);
                var nodeD = session.GetNode(originalNodes[D].Id);
                nodeB.RemoveConnection(nodeC);
                nodeD.RemoveConnection(nodeC);
            });
        }

        [Test]
        public void all_nodes_should_still_exist_in_database()
        {
            InNewSession(session =>
            {
                foreach (var originalNode in originalNodes)
                {
                    var retrieved = session.Get<Node>(originalNode.Id);
                    Assert.That(retrieved, Is.Not.Null, "original node {0} not saved", originalNode);
                }
            });
        }

        [Test]
        public void outer_nodes_should_remain_related()
        {
            AssertNodesRelated(originalNodes[A], originalNodes[B]);
            AssertNodesRelated(originalNodes[D], originalNodes[E]);
        }

        [Test]
        public void unlinked_nodes_should_not_be_related()
        {
            AssertNodesNotRelated(originalNodes[B], originalNodes[C]);
            AssertNodesNotRelated(originalNodes[C], originalNodes[D]);
        }

    }

    public class when_deleting_middle_node_from_network_5_levels_deep : PersistenceSpecification
    {
        private List<Node> originalNodes = new List<Node>();

        protected override void because()
        {
            InNewSession(session =>
            {
                originalNodes = CreateNodesAtoE(); // node for each char
                originalNodes[A].AddConnection(originalNodes[B], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[B].AddConnection(originalNodes[C], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[C].AddConnection(originalNodes[D], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[D].AddConnection(originalNodes[E], HighConnectionQuality, MediumConnectionQuality);
                session.SaveOrUpdate(originalNodes[A]);
            });

            InNewSession(session =>
            {
                var nodeC = session.GetNode(originalNodes[C].Id);
                nodeC.RemoveAllConnections();
                session.Delete(nodeC);
            });
        }

        [Test]
        public void remaining_nodes_should_still_exist_in_database()
        {
            InNewSession(session =>
            {
                var remainingNodes = originalNodes.Where(x => x.Name != "C");
                foreach (var node in remainingNodes)
                {
                    var retrieved = session.Get<Node>(node.Id);
                    Assert.That(retrieved, Is.Not.Null, "original node {0} not retrieved", node);
                }
            });
        }

        [Test]
        public void outer_nodes_should_remain_related()
        {
            AssertNodesRelated(originalNodes[A], originalNodes[B]);
            AssertNodesRelated(originalNodes[D], originalNodes[E]);
        }

        [Test]
        public void relationship_should_be_removed_from_nodes_originally_linked_to_middle_node()
        {
            InNewSession(session =>
            {
                var nodeB = session.GetNode(originalNodes[B].Id);
                Assert.That(nodeB.Connections.Count, Is.EqualTo(1));

                var nodeD = session.GetNode(originalNodes[D].Id);
                Assert.That(nodeD.Connections.Count, Is.EqualTo(1));
            });
        }

    }

    public class when_unlinking_one_of_many_related_nodes : PersistenceSpecification
    {
        private List<Node> originalNodes = new List<Node>();

        protected override void because()
        {
            InNewSession(session =>
            {
                originalNodes = CreateNodesAtoE();
                originalNodes[A].AddConnection(originalNodes[B], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[A].AddConnection(originalNodes[C], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[A].AddConnection(originalNodes[D], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[A].AddConnection(originalNodes[E], HighConnectionQuality, MediumConnectionQuality);
                session.SaveOrUpdate(originalNodes[A]);
            });

            InNewSession(session =>
            {
                var nodeA = session.GetNode(originalNodes[A].Id);
                var nodeB = session.GetNode(originalNodes[B].Id);
                nodeA.RemoveConnection(nodeB);
            });
        }

        [Test]
        public void all_nodes_should_still_exist_in_database()
        {
            InNewSession(session =>
            {
                foreach (var originalNode in originalNodes)
                {
                    var retrieved = session.Get<Node>(originalNode.Id);
                    Assert.That(retrieved, Is.Not.Null, "original node {0} not saved", originalNode);
                }
            });
        }

        [Test]
        public void relationships_with_other_nodes_should_not_be_removed()
        {
            AssertNodesRelated(originalNodes[A], originalNodes[C]);
            AssertNodesRelated(originalNodes[A], originalNodes[D]);
            AssertNodesRelated(originalNodes[A], originalNodes[E]);
        }

        [Test]
        public void unlinked_nodes_should_not_be_related()
        {
            AssertNodesNotRelated(originalNodes[A], originalNodes[B]);
        }
    }

    public class when_retrieving_network_of_5_nodes : PersistenceSpecification
    {
        private List<Node> originalNodes = new List<Node>();

        protected override void because()
        {
            InNewSession(session =>
            {
                originalNodes = CreateNodesAtoE();
                originalNodes[A].AddConnection(originalNodes[B], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[A].AddConnection(originalNodes[C], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[A].AddConnection(originalNodes[D], HighConnectionQuality, MediumConnectionQuality);
                originalNodes[A].AddConnection(originalNodes[E], HighConnectionQuality, MediumConnectionQuality);
                session.SaveOrUpdate(originalNodes[A]);
            });

            InNewSession(session =>
            {
                var nodeA = session.GetNode(originalNodes[A].Id);
                nodeA.DisplayConnections(10);
            });
        }

        [Test]
        public void unlinked_nodes_should_not_be_related()
        {
            AssertNodesNotRelated(originalNodes[A], originalNodes[B]);
        }
    }

    public static class SessionExtensions
    {
        public static Node GetNode(this ISession session, Guid id)
        {
            var node = session.GetNodeOrDefault(id);
            Assert.That(node, Is.Not.Null);
            return node;
        }

        public static Node GetNodeOrDefault(this ISession session, Guid id)
        {
            var node = session.Get<Node>(id);
            Assert.That(node, Is.Not.Null);
            return node;
        }
    }
}