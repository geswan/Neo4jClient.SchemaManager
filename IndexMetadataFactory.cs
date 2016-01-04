namespace Neo4jClient.SchemaManager
{
    using System.Collections.Generic;

    public interface IIndexMetadataFactory
    {
        IIndexMetadata Get();

        List<IIndexMetadata> GetList();
    }

    public  class IndexMetadataFactory : IIndexMetadataFactory
    {
      public IIndexMetadata Get()
      {
          return new IndexMetadata();
      }
      public List<IIndexMetadata> GetList()
      {
          return new List<IIndexMetadata>();
      }
    }
}
