namespace Neo4jClient.SchemaManager
{
    using System.IO;
    using System.Net;
    using System.Text;

    using Microsoft.VisualBasic.FileIO;

    using Newtonsoft.Json;

    public interface ISchemaReader
    {
        #region Public Methods and Operators

        string[] GetSchemaIndexData(IGraphClient graphClient);

        string GetSchemaTxt(IGraphClient graphClient);

        #endregion
    }

    public class SchemaReader : ISchemaReader
    {
         #region Constants and Fields

        private readonly IHttpManager httpManager;

        #endregion

        #region Constructors and Destructors

        public SchemaReader(IHttpManager httpManager)
        {
            this.httpManager = httpManager;
        }

        #endregion

        #region Public Methods and Operators

        public string[] GetSchemaIndexData(IGraphClient graphClient)
        {
            string[] indexData = null;
            string schemaTxt = this.ReadSchemaTxt(graphClient);
            if (schemaTxt != string.Empty)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(schemaTxt);
                var textParser = new TextFieldParser(new MemoryStream(byteArray));
                textParser.SetDelimiters(",");
                string[] dataPair = textParser.ReadFields(); 

                if (dataPair != null)
                {
                    indexData =
                        dataPair[0].TrimEnd(new[] { '\r', '\n' })
                            .Replace(@"\r\n\r\n", "\t")
                            .Replace(@"\r\n", ";")
                            .Split('\t');
                }
            }
            return indexData;
        }

        public virtual string GetSchemaTxt(IGraphClient graphClient)
        {
            return this.ReadSchemaTxt(graphClient);
        }

        #endregion

        #region Methods

        private string ReadSchemaTxt(IGraphClient graphClient)
        {
            var commandObj = new { command = "schema", engine = "shell" };
            string jsonCommand = JsonConvert.SerializeObject(commandObj);
            HttpWebRequest commandRequest = this.httpManager.GetConsoleCommandRequest(graphClient, jsonCommand);
            string result = this.httpManager.GetCommandResponse(commandRequest).Trim(new[] { '[', ']', ' ' });

            return result;
        }

        #endregion
    }
}