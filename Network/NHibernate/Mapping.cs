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
                    bag.Key(key => 
                    { 
                        key.Column("StartNodeId");
                        key.ForeignKey("FK_RelatedNode_Node_StartNodeId");
                    });
                    bag.Table("RelatedNode");
                    bag.Cascade(Cascade.All);
                    bag.Fetch(CollectionFetchMode.Subselect);
                });
            });

            mapper.Component<RelatedNode>(component =>
            {
                component.Parent(x => x.Start);
                component.ManyToOne(x => x.End, manyToOne =>
                {
                    manyToOne.Column("EndNodeId");
                    manyToOne.ForeignKey("FK_RelatedNode_Node_EndNodeId");
                    manyToOne.Cascade(Cascade.Persist);
                });
                component.ManyToOne(x => x.Relationship, manyToOne =>
                {
                    manyToOne.Column("RelationshipId");
                    manyToOne.ForeignKey("FK_RelatedNode_Relationship_RelationshipId");
                    manyToOne.Cascade(Cascade.Persist.Include(Cascade.Remove));
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