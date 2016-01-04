namespace Neo4jClient.SchemaManager
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;

    using Neo4jClient;
    using Neo4jClient.Execution;
    using Neo4jClient.SchemaManager.Properties;

    public interface IHttpManager
    {
        #region Public Methods and Operators

        string GetCommandResponse(HttpWebRequest request);

        HttpWebRequest GetConsoleCommandRequest(IGraphClient graphClient, string jsonCommand);

        #endregion
    }

    public class HttpManager : IHttpManager
    {
      
        #region Public Methods and Operators

        public virtual string GetCommandResponse(HttpWebRequest request)
        {
            var response = (HttpWebResponse)request.GetResponse();
            string responseTxt = string.Empty;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream);
                    responseTxt = reader.ReadToEnd();
                    reader.Close();
                }
            }
            response.Close();
            return responseTxt;
        }

        public virtual HttpWebRequest GetConsoleCommandRequest(IGraphClient graphClient, string jsonCommand)
        {
            string absoluteUri = graphClient.RootEndpoint.AbsoluteUri.TrimEnd('/');
            string Url = absoluteUri.Substring(0, absoluteUri.LastIndexOf('/')) + Resources.CONSOLEPATH;
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = Resources.METHOD;
            request.Accept = Resources.ACCEPT;
            request.Host = Resources.HOST;
            request.KeepAlive = true;
            request.ContentType = Resources.CONTENTTYPE;
            request.Referer = Resources.REFERER;
            request.Headers.Add("Authorization", this.GetAuthorizationHeaderValue(graphClient.ExecutionConfiguration));
            byte[] postBuffer = Encoding.UTF8.GetBytes(jsonCommand);
            request.ContentLength = postBuffer.Length;
            Stream postDataStream = request.GetRequestStream();
            postDataStream.Write(postBuffer, 0, postBuffer.Length);
            postDataStream.Close();
            return request;
        }

        #endregion

        #region Methods

        private string GetAuthorizationHeaderValue(ExecutionConfiguration executionConfig)
        {
            byte[] encoded =
                Encoding.ASCII.GetBytes(string.Format("{0}:{1}", executionConfig.Username, executionConfig.Password));
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(encoded)).ToString();
        }

        #endregion
    }
}