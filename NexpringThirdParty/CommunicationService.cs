using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace NexpringThirdParty
{
    class CommunicationService
    {
        public static CommunicationService instance = new CommunicationService();
        private HttpClient client = new HttpClient(App.filter);

        private string GetXmlString(string requestURI)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(requestURI, UriKind.Absolute);

            try
            {
                HttpResponseMessage response = Task.Run(async () => { return await client.SendRequestAsync(request); }).Result;
                var content = response.Content.ReadAsStringAsync().GetResults();
                return content;
            }
            catch(Exception)
            {
                return "ERROR";
            }
        }
    }
}
