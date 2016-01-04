namespace Neo4jClient.SchemaManagerTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Neo4jClient;
    using Neo4jClient.Cypher;
    using Neo4jClient.Execution;
    using Neo4jClient.SchemaManager;

    using NSubstitute;

    [TestClass]
    public class SchemaReaderUnitTests
    {
        private static IGraphClient graphClientSub;
      
     
          [TestInitialize]
        public void TestInitialise()
        {
            graphClientSub = Substitute.For<IGraphClient>();
            var cypher = Substitute.For<ICypherFluentQuery>();
            graphClientSub.Cypher.Returns(cypher);
        }

        [TestMethod]
        public void GetConsoleCommandRequestReceivesCorrectCommand()
        {

            graphClientSub.RootEndpoint.Returns(new Uri("http://localhost:7474/db/data"));
            graphClientSub.ExecutionConfiguration.Returns(
                new ExecutionConfiguration { Username = "Jones", Password = "David" });
            IHttpManager httpManger = Substitute.For<IHttpManager>();
           ISchemaReader schemaReader = new SchemaReader(httpManger);
            httpManger.GetCommandResponse(null).Returns(string.Empty);
            const string expectedCommand = "{\"command\":\"schema\",\"engine\":\"shell\"}";
            schemaReader.GetSchemaTxt(graphClientSub);
            httpManger.Received()
                .GetConsoleCommandRequest(graphClientSub, Arg.Is<string>(s => string.Equals(s, expectedCommand)));
        }

      
    }
}
