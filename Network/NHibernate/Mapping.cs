using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace Network.NHibernate
{
    public class Mapping
    {
        public void ApplyTo(Configuration configuration)
        {
            // It's a small model, so we're mapping everything explicitly

            var mapper = new ModelMapper();
            mapper.Class<Node>(mapping =>
            {
                mapping.Id(x => x.Id, map =>
                {
                    map.Column("NodeId");
                    map.Generator(Generators.GuidComb);
                });
                mapping.Property(x => x.Name, map =>
                {
                    map.Length(50);
                    map.Column("NodeName");
                });
                mapping.Bag(x => x.RelatedNodes, bag =>
                {
                    bag.Key(key =>
                    {
                        key.Column("NodeId");
                        key.ForeignKey("FK_RelatedNode_Node_NodeId");
                    });
                    bag.Table("RelatedNode");
                    bag.Cascade(Cascade.All);
                    bag.Fetch(CollectionFetchMode.Subselect);
                });
            });

            mapper.Component<RelatedNode>(mapping =>
            {
                mapping.ManyToOne(x => x.Relationship, manyToOne =>
                {
                    manyToOne.Column("RelationshipId");
                    manyToOne.ForeignKey("FK_RelatedNode_Relationship_RelationshipId");
                    manyToOne.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                });
                mapping.ManyToOne(x => x.Node, manyToOne =>
                {
                    manyToOne.Column("OtherNodeId");
                    manyToOne.ForeignKey("FK_RelatedNode_Node_OtherNodeId");
                    manyToOne.Cascade(Cascade.All);
                });
            });

            mapper.Class<Relationship>(mapping =>
            {
                mapping.Id(x => x.Id, map =>
                {
                    map.Column("RelationshipId");
                    map.Generator(Generators.GuidComb);
                });
                mapping.Property(x => x.Distance);
            });
            configuration.AddMapping(mapper.CompileMappingForAllExplicitAddedEntities());
        }
    }
}