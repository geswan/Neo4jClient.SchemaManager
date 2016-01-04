namespace Neo4jClient.SchemaManager
{
    using System.Collections.Generic;

    using Neo4jClient;

    public interface IConsoleRequestManager
    {
        #region Public Methods and Operators

        void DropAllIndexes(IGraphClient graphClient);

        void DropConstraint(IGraphClient graphClient, string label, string property);

        void DropIndex(IGraphClient graphClient, string label, string property);

        List<IIndexMetadata> ListAllIndexes(IGraphClient graphClient);

        #endregion
    }

    public class ConsoleRequestManager : IConsoleRequestManager
    {
        #region Constants and Fields

        private readonly IIndexHelper indexHelper;

        #endregion

        #region Constructors and Destructors

        public ConsoleRequestManager(IIndexHelper indexHelper)
        {
            this.indexHelper = indexHelper;
        }

        #endregion

        #region Public Methods and Operators

        public void DropAllIndexes(IGraphClient graphClient)
        {
            List<IIndexMetadata> indexMetadataList = this.indexHelper.ListAllIndexes(graphClient);
            foreach (IIndexMetadata indexMetadata in indexMetadataList)
            {
                if (indexMetadata.Type == TypeId.Index)
                {
                    this.indexHelper.DropIndex(graphClient, indexMetadata.Label, indexMetadata.Property);
                }
                else if (indexMetadata.Type == TypeId.Constraint)
                {
                    this.indexHelper.DropConstraint(graphClient, indexMetadata.Label, indexMetadata.Property);
                }
            }
        }

        public void DropConstraint(IGraphClient graphClient, string label, string property)
        {
            this.indexHelper.DropConstraint(graphClient, label, property);
        }

        public void DropIndex(IGraphClient graphClient, string label, string property)
        {
            this.indexHelper.DropIndex(graphClient, label, property);
        }

        public List<IIndexMetadata> ListAllIndexes(IGraphClient graphClient)
        {
            return this.indexHelper.ListAllIndexes(graphClient);
        }

        #endregion
    }
}