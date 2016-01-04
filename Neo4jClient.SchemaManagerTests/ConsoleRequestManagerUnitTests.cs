namespace Neo4jClient.SchemaManagerTests
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Neo4jClient;
    using Neo4jClient.Cypher;
    using Neo4jClient.SchemaManager;

    using NSubstitute;

    /// <summary>
    /// Summary description for ConsoleRequestManagerUnitTests
    /// </summary>
    [TestClass]
    public class ConsoleRequestManagerUnitTests
    {
        private static IGraphClient graphClientSub;
          private static List<IIndexMetadata> indexMetadataList;
          [ClassInitialize]
          public static void ClassInit(TestContext context)
          {
              indexMetadataList = new List<IIndexMetadata>
            {
                new IndexMetadata
                {
                    Constraint = "IS UNIQUE",
                    Label = "User",
                    Property = "UserId",
                    Status = "ASSERT",
                    Type = TypeId.Constraint
                },
                new IndexMetadata
                {
                    Constraint = null,
                    Label = "Person",
                    Property = "name",
                    Status = "ONLINE",
                    Type = TypeId.Index
                },
                new IndexMetadata
                {
                    Constraint = null,
                    Label = "Movie",
                    Property = "title",
                    Status = "ONLINE",
                    Type = TypeId.Index
                },
                new IndexMetadata
                {
                    Constraint = null,
                    Label = "User",
                    Property = "UserId",
                    Status = "ONLINE",
                    Type = TypeId.ConstrainedIndex
                }
            };

          }



        [TestInitialize]
        public void TestInitialise()
        {
            graphClientSub = Substitute.For<IGraphClient>();
            var cypher = Substitute.For<ICypherFluentQuery>();
            graphClientSub.Cypher.Returns(cypher);
         
        }



        [TestMethod]
        public void DropAllIndexesCallsDropIndexCorrectNumberOfTimes()
        {
            var indexMetadataFactory = Substitute.For<IIndexMetadataFactory>();
            var schemaReader = Substitute.For<ISchemaReader>();
            var indexHelperSub = Substitute.For<IndexHelper>(schemaReader,indexMetadataFactory);

            indexHelperSub.ListAllIndexes(graphClientSub).Returns(indexMetadataList);
            var consoleRequestManager = new ConsoleRequestManager(indexHelperSub);
            consoleRequestManager.DropAllIndexes(graphClientSub);
            indexHelperSub.Received(2).DropIndex(graphClientSub, Arg.Any<string>(), Arg.Any<string>());
        }

        [TestMethod]
        public void DropAllIndexesCallsDropConstraintCorrectNumberOfTimes()
        {
            var indexMetadataFactory = Substitute.For<IIndexMetadataFactory>();
            var schemaReader = Substitute.For<ISchemaReader>();
            var indexHelperSub = Substitute.For<IndexHelper>(schemaReader, indexMetadataFactory);

            indexHelperSub.ListAllIndexes(graphClientSub).Returns(indexMetadataList);
            var consoleRequestManager = new ConsoleRequestManager(indexHelperSub);
            consoleRequestManager.DropAllIndexes(graphClientSub);
            indexHelperSub.Received(1).DropConstraint(graphClientSub, Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
