namespace Neo4jClient.SchemaManagerTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Neo4jClient;
    using Neo4jClient.Cypher;
    using Neo4jClient.SchemaManager;

    using NSubstitute;

    [TestClass]
    public class IndexHelperUnitTests
    {
        #region Constants and Fields

        private static IGraphClient graphClientSub;

        #endregion

        #region Public Methods and Operators

        [TestMethod]
        public void DropConstraintCallsCypherDropUniqueConstraintWithCorrectParams()
        {
            var indexMetadataFactory = Substitute.For<IIndexMetadataFactory>();
            var schemaReader = Substitute.For<ISchemaReader>();
            var indexHelper = new IndexHelper(schemaReader, indexMetadataFactory);
            string param1 = "c:Person";
            string param2 = "c.name";
            indexHelper.DropConstraint(graphClientSub, "Person", "name");
            graphClientSub.Cypher.Received().DropUniqueConstraint(param1, param2);
        }

        [TestMethod]
        public void DropIndexCallsCypherDropWithCorrectParams()
        {
            var indexMetadataFactory = Substitute.For<IIndexMetadataFactory>();
            var schemaReader = Substitute.For<ISchemaReader>();
            var indexHelper = new IndexHelper(schemaReader, indexMetadataFactory);
            string expected = "INDEX ON :Person(name)";
            indexHelper.DropIndex(graphClientSub, "Person", "name");
            graphClientSub.Cypher.Received().Drop(expected);
        }

        [TestInitialize]
        public void TestInitialise()
        {
            graphClientSub = Substitute.For<IGraphClient>();
            var cypher = Substitute.For<ICypherFluentQuery>();
            graphClientSub.Cypher.Returns(cypher);
        }

        #endregion
    }
}