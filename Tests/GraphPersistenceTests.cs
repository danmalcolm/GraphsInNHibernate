using System;
using System.Linq;
using Network;
using NHibernate;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GraphPersistenceTests
    {
        private readonly ISessionFactory sessionFactory = TestConfigurationSource.SessionFactory;

        private void InNewSession(Action<ISession> action)
        {
            using (ISession session = sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                action(session);
                transaction.Commit();
            }
        }

        [Test]
        public void CanLinkNodes1LevelDeep()
        {
            Guid id = Guid.Empty;
            InNewSession(session =>
            {
                var nodeA = new Node("A");
                var nodeB = new Node("B");
                nodeA.LinkTo(nodeB, 10);
                session.SaveOrUpdate(nodeA);
                id = nodeA.Id;
            });

            InNewSession(session =>
            {
                var nodeA = session.Get<Node>(id);
                Assert.That(nodeA.RelatedNodes.Count, Is.EqualTo(1));
                var nodeB = nodeA.RelatedNodes.Single().Node;
                Assert.That(nodeB.RelatedNodes.Count, Is.EqualTo(1));
            });
        }

        [Test]
        public void CanLinkNodes5LevelsDeep()
        {
            Guid id = Guid.Empty;
            InNewSession(session =>
            {
                var nodeA = new Node("A");
                var nodeB = new Node("B");
                var nodeC = new Node("C");
                var nodeD = new Node("D");
                var nodeE = new Node("E");
                var nodeF = new Node("F");
                nodeA.LinkTo(nodeB, 10);
                nodeB.LinkTo(nodeC, 15);
                nodeC.LinkTo(nodeD, 15);
                nodeD.LinkTo(nodeE, 15);
                nodeE.LinkTo(nodeF, 15);
                session.SaveOrUpdate(nodeA);
                id = nodeA.Id;
            });

            InNewSession(session =>
            {
                var nodeA = session.Get<Node>(id);
                Assert.That(nodeA.RelatedNodes.Count, Is.EqualTo(1));
            });
        }

        [Test]
        public void CanSaveNode()
        {
            var node = new Node("A");
            InNewSession(session => session.SaveOrUpdate(node));
            InNewSession(session =>
            {
                var retrieved = session.Get<Node>(node.Id);
                Assert.That(retrieved, Is.Not.Null);
            });
        }
    }
}