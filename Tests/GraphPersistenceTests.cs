using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using NHibernate;
using NUnit.Framework;
using NHibernate.Linq;

namespace Tests.GraphPersistenceSpecs
{
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
                Assert.That(startNode.IsRelatedTo(originalNodeB), Is.False, string.Format("{0} is related to {1}", startNode, originalNodeB));
            });
            InNewSession(session =>
            {
                var endNode = session.GetNode(originalNodeB.Id);
                Assert.That(endNode.IsRelatedTo(originalNodeA), Is.False, string.Format("{0} is related to {1}", endNode, originalNodeA));
            });
        }

        protected void AssertNodesRelated(Node originalNodeA, Node originalNodeB)
        {
            InNewSession(session =>
            {
                var startNode = session.GetNode(originalNodeA.Id);
                Assert.That(startNode.IsRelatedTo(originalNodeB), Is.True, string.Format("{0} is not related to {1}", startNode, originalNodeB));
            });
            InNewSession(session =>
            {
                var endNode = session.GetNode(originalNodeB.Id);
                Assert.That(endNode.IsRelatedTo(originalNodeA), Is.True, string.Format("{0} is not related to {1}", endNode, originalNodeA));
            });
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
    public class when_saving_node_with_single_related_node : PersistenceSpecification
    {
        private Node originalStartNode;
        private Node originalEndNode;

        protected override void because()
        {
            InNewSession(session => 
            { 
                originalStartNode = new Node("A");
                originalEndNode = new Node("B");
                originalStartNode.LinkTo(originalEndNode, 10);
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
                Assert.That(endNode.RelatedNodes.Count, Is.EqualTo(1));
                var startNode = endNode.RelatedNodes.Single().Node;
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
                originalNodes[A].LinkTo(originalNodes[B], 10);
                originalNodes[B].LinkTo(originalNodes[C], 15);
                originalNodes[C].LinkTo(originalNodes[D], 20);
                originalNodes[D].LinkTo(originalNodes[E], 25);
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
        private Relationship relationshipBetweenAandB;

        protected override void because()
        {
            InNewSession(session =>
            {
                originalNodes = "ABCDE".Select(name => new Node(name.ToString())).ToList(); // node for each char
                originalNodes[A].LinkTo(originalNodes[B], 10);
                originalNodes[B].LinkTo(originalNodes[C], 15);
                originalNodes[C].LinkTo(originalNodes[D], 20);
                originalNodes[D].LinkTo(originalNodes[E], 25);
                session.SaveOrUpdate(originalNodes[A]);
                relationshipBetweenAandB = originalNodes[A].GetLink(originalNodes[B]).Relationship;
            });

            InNewSession(session =>
            {
                var nodeB = session.GetNode(originalNodes[B].Id);
                var nodeC = session.GetNode(originalNodes[C].Id);
                var nodeD = session.GetNode(originalNodes[D].Id);
                nodeB.UnlinkFrom(nodeC);
                nodeD.UnlinkFrom(nodeC);
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

        [Test]
        public void relationship_between_b_and_c_should_be_removed()
        {
            InNewSession(session =>
            {
                var relationship = session.Get<Relationship>(relationshipBetweenAandB.Id);
                Assert.That(relationship, Is.Null);
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
                originalNodes = "ABCDE".Select(name => new Node(name.ToString())).ToList(); // create nodes A to E
                originalNodes[A].LinkTo(originalNodes[B], 10);
                originalNodes[A].LinkTo(originalNodes[C], 15);
                originalNodes[A].LinkTo(originalNodes[D], 20);
                originalNodes[A].LinkTo(originalNodes[E], 25);
                session.SaveOrUpdate(originalNodes[A]);
            });

            InNewSession(session =>
            {
                var nodeA = session.GetNode(originalNodes[A].Id);
                var nodeB = session.GetNode(originalNodes[B].Id);
                nodeA.UnlinkFrom(nodeB);
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