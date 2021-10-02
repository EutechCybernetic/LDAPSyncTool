using RestSharp;
using System.Collections.Generic;
namespace LDAPSyncTool 
{
   
    public class iviva
    {
        private string url = "";
        private string apiKey = "";
        public iviva(IVIVAConfig config)
        {
            this.apiKey = config.ApiKey;
            this.url = config.Url;
        }
        public string Execute(string service, object body)
        {
            var client = new RestClient(this.url);
            var request = new RestRequest($"/api/{service}", Method.POST);
            request.AddHeader("Authorization", $"APIKEY {this.apiKey}");
            request.AddJsonBody(body);
            request.OnBeforeRequest = (IHttp http) => {
                Log.Debug($"POST {http.Url}");
                foreach(var h in http.Headers)
                {
                    Log.Debug($"{h.Name}: {h.Value}");
                }
                Log.Debug(" ");
                Log.Debug($"{http.RequestBody}");
            };
            var response = client.Execute(request);
            var status = (int)response.StatusCode;
            Log.Debug($"Response: {status}");
            var content = response.Content;
            Log.Debug($"{content}");
            if (status >= 400)
            {
                throw new APIException(content);
            }
            return content;

        }
    }
}