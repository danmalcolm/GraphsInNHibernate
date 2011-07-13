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
                node.Bag(x => x.Connections, bag =>
                {
                    bag.Key(key => 
                    { 
                        key.Column("StartNodeId");
                        key.ForeignKey("FK_RelatedNode_Node_StartNodeId");
                    });
                    bag.Table("Connection");
                    bag.Cascade(Cascade.All.Include(Cascade.Remove));
                    bag.Fetch(CollectionFetchMode.Subselect);
                });
            });

            mapper.Component<Connection>(connection =>
            {
                connection.Parent(x => x.Start);
                connection.ManyToOne(x => x.End, manyToOne =>
                {
                    manyToOne.Column("EndNodeId");
                    manyToOne.ForeignKey("FK_RelatedNode_Node_EndNodeId");
                    manyToOne.Cascade(Cascade.Persist);
                    manyToOne.NotNullable(true);
                });
                connection.Component(x => x.Quality, c => {});
            });

            mapper.Component<ConnectionQuality>(connectionQuality =>
            {
                connectionQuality.Property(x => x.UploadMbps);
                connectionQuality.Property(x => x.DownloadMbps);
            });
            configuration.AddMapping(mapper.CompileMappingForAllExplicitAddedEntities());
        }
    }
}