namespace Neo4jClient.SchemaManager
{
    public class ConsoleRequestManagerFactory
    {
        #region Public Methods and Operators

        public IConsoleRequestManager Get()
        {
            return
                new ConsoleRequestManager(
              new IndexHelper(new SchemaReader(new HttpManager()), new IndexMetadataFactory()));
        }

        #endregion
    }
}