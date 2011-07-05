using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace Network.NHibernate
{
    public class Mapping
    {
        public void ApplyTo(Configuration configuration)
        {
            var mapper = new ModelMapper();
            mapper.Class<Node>(node =>
            {
                node.Id(x => x.Id, map =>
                {
                    map.Column("NodeId");
                    map.Generator(Generators.GuidComb);
                });
                node.Property(x => x.Name, map =>
                {
                    map.Length(50);
                    map.Column("NodeName");
                });
                node.Bag(x => x.RelatedNodes, bag =>
                {
                    bag.Key(key => 
                    { 
                        key.Column("StartNodeId");
                        key.ForeignKey("FK_RelatedNode_Node_StartNodeId");
                    });
                    bag.Table("RelatedNode");
                    bag.Cascade(Cascade.All.Include(Cascade.Remove));
                    bag.Fetch(CollectionFetchMode.Subselect);
                });
            });

            mapper.Component<RelatedNode>(relatedNode =>
            {
                relatedNode.Parent(x => x.Start);
                relatedNode.ManyToOne(x => x.End, manyToOne =>
                {
                    manyToOne.Column("EndNodeId");
                    manyToOne.ForeignKey("FK_RelatedNode_Node_EndNodeId");
                    manyToOne.Cascade(Cascade.Persist);
                    manyToOne.NotNullable(true);
                });
                relatedNode.ManyToOne(x => x.Relationship, manyToOne =>
                {
                    manyToOne.Column("RelationshipId");
                    manyToOne.ForeignKey("FK_RelatedNode_Relationship_RelationshipId");
                    manyToOne.Cascade(Cascade.All.Include(Cascade.Remove));
                    manyToOne.NotNullable(true);
                });
            });

            mapper.Class<Relationship>(relationship =>
            {
                relationship.Id(x => x.Id, map =>
                {
                    map.Column("RelationshipId");
                    map.Generator(Generators.GuidComb);
                });
                relationship.Property(x => x.Distance);
            });
            configuration.AddMapping(mapper.CompileMappingForAllExplicitAddedEntities());
        }
    }
}