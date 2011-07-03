using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace Network.NHibernate
{
    public class Mapping
    {
        public void ApplyTo(Configuration configuration)
        {
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
                    bag.Key(x => x.Column("NodeId"));
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
                    manyToOne.Cascade(Cascade.All);
                });
                mapping.ManyToOne(x => x.Node, manyToOne =>
                {
                    manyToOne.Column("RelatedNodeId");
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