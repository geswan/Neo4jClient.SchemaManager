namespace Neo4jClient.SchemaManager
{
    using System.Collections.Generic;

    using Neo4jClient;

    public static class GraphClientExtensions
    {
        #region Constants and Fields

        private static readonly IConsoleRequestManager consoleRequestManager = new ConsoleRequestManagerFactory().Get();

        #endregion

        #region Public Methods and Operators

        public static void DropAllIndexes(this IGraphClient graphClient)
        {
            consoleRequestManager.DropAllIndexes(graphClient);
        }

        public static void DropConstraint(this IGraphClient graphClient, string label, string property)
        {
            consoleRequestManager.DropConstraint(graphClient, label, property);
        }

        public static void DropIndex(this IGraphClient graphClient, string label, string property)
        {
            consoleRequestManager.DropIndex(graphClient, label, property);
        }

        public static List<IIndexMetadata> ListAllIndexes(this IGraphClient graphClient)
        {
            return consoleRequestManager.ListAllIndexes(graphClient);
        }

        #endregion
    }
}