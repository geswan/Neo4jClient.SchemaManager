namespace Neo4jClient.SchemaManager
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Neo4jClient;

    public interface IIndexHelper
    {
        List<IIndexMetadata> ListAllIndexes(IGraphClient graphClient);

        void DropConstraint(IGraphClient graphClient, string label, string property);

        void DropIndex(IGraphClient graphClient, string label, string property);
    }

    public  class IndexHelper : IIndexHelper
    {
       private class IndexGroup
        {
            #region Public Properties

            public string[] Indexes { get; set; }

            #endregion
        }

        private readonly Regex constriantRegex =
         new Regex(
             " +ON +\\(.*?:(?<Label>.*?)\\) *(?<Status>[A-Z]*).*?\\.(?<Pro"
             + "perty>.*?) (?<Constraint>[A-Z]* ?[A-Z]*)",
             RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private readonly Regex indexRegex =
            new Regex(
                " +ON +:(?<Label>.*?)\\((?<Property>.*?)\\) +(?<Status>[A-Z]*) +(?<Constraint>.*?)$",
                RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private readonly ISchemaReader schemaReader;

        private readonly IIndexMetadataFactory indexMetadataFactory;

        public IndexHelper(ISchemaReader schemaReader, IIndexMetadataFactory indexMetadataFactory)
        {
            this.schemaReader = schemaReader;
            this.indexMetadataFactory = indexMetadataFactory;
        }
      

     public virtual List<IIndexMetadata> ListAllIndexes(IGraphClient graphClient)
     {
         var indexMetadataList = this.indexMetadataFactory.GetList();
         string[] indexData = this.schemaReader.GetSchemaIndexData(graphClient);
         if (indexData != null)
         {


             List<IndexGroup> indexConstraintGroups =
                 indexData.Select(g => new IndexGroup { Indexes = g.Trim(';').Split(';') }).ToList();

             indexMetadataList.AddRange(this.BuildConstraintList(indexConstraintGroups));
             indexMetadataList.AddRange(this.BuildIndexList(indexConstraintGroups));
         }
         return indexMetadataList;
      }

      private IEnumerable<IIndexMetadata> BuildConstraintList(List<IndexGroup> indexConstraintGroups)
      {
          var metadataList = this.indexMetadataFactory.GetList();
          if (indexConstraintGroups.Count > 1 && indexConstraintGroups[1].Indexes[0].Contains("Constraints"))
          {
              for (int i = 1; i < indexConstraintGroups[1].Indexes.Length; i++)
              {
                  MatchCollection coll = this.constriantRegex.Matches(indexConstraintGroups[1].Indexes[i]);
                  foreach (Match m in coll)
                  {
                      IIndexMetadata indexMetadata = this.BuildIndexMetadata(m);

                      indexMetadata.Type = TypeId.Constraint;
                      indexMetadata.Constraint = m.Groups["Constraint"].Value;
                      indexMetadata.Status = indexMetadata.Constraint;
                      metadataList.Add(indexMetadata);
                  }
              }
          }
          return metadataList;
      }

      private IIndexMetadata BuildIndexMetadata(Match m)
      {
          var indexMetadata = this.indexMetadataFactory.Get(); 
          indexMetadata.Label = m.Groups["Label"].Value;
          indexMetadata.Property = m.Groups["Property"].Value;
        //  indexMetadata.Status = m.Groups["Status"].Value;

          return indexMetadata;
      }

      private IEnumerable<IIndexMetadata> BuildIndexList(List<IndexGroup> indexConstraintGroups)
      {
          var indexMetadataList = this.indexMetadataFactory.GetList();
          if (indexConstraintGroups.Count > 0 && indexConstraintGroups[0].Indexes[0].Contains("Indexes"))
          {
              for (int i = 1; i < indexConstraintGroups[0].Indexes.Length; i++)
              {
                  MatchCollection coll = this.indexRegex.Matches(indexConstraintGroups[0].Indexes[i]);
                  foreach (Match m in coll)
                  {
                      IIndexMetadata indexMetadata = this.BuildIndexMetadata(m);
                      string constraint = m.Groups["Constraint"].Value;
                      indexMetadata.Status = m.Groups["Status"].Value;
                      indexMetadata.Type = constraint == string.Empty ? TypeId.Index : TypeId.ConstrainedIndex;
                      indexMetadataList.Add(indexMetadata);
                  }
              }
          }
          return indexMetadataList;
      }
      public  void DropConstraint(IGraphClient graphClient, string label, string property)
      {
          string firstArg = string.Format("c:{0}", label);
          string secondtArg = string.Format("c.{0}", property);
          graphClient.Cypher.DropUniqueConstraint(firstArg, secondtArg).ExecuteWithoutResults();
      }

      public  void DropIndex(IGraphClient graphClient, string label, string property)
      {
          string dropString = string.Format("INDEX ON :{0}({1})", label, property);
          graphClient.Cypher.Drop(dropString).ExecuteWithoutResults();
      }


    }
}
