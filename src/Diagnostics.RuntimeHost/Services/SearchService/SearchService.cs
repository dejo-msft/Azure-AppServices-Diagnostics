using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Diagnostics.DataProviders.DataProviderConfigurations;
using Diagnostics.DataProviders.TokenService;
using Newtonsoft.Json;

namespace Diagnostics.RuntimeHost.Services
{
    public interface ISearchService : IDisposable
    {
        Task<HttpResponseMessage> SearchDetectors(string requestId, string query, Dictionary<string, string> parameters);

        Task<HttpResponseMessage> SearchUtterances(string requestId, string query, string[] detectorUtterances, Dictionary<string, string> parameters);

        Task<HttpResponseMessage> TriggerTraining(string requestId, string trainingConfig, Dictionary<string, string> parameters);

        Task<HttpResponseMessage> TriggerModelRefresh(string requestId, Dictionary<string, string> parameters);
    }

    public class SearchService : ISearchService
    {
        private string QueryDetectorsUrl;
        private string QueryUtterancesUrl;
        private string RefreshModelUrl;
        private string FreeModelUrl;
        private string TriggerTrainingUrl;
        private string TriggerModelRefreshUrl;
        private HttpClient _httpClient;
        SearchServiceProviderConfiguration configuration;

        public SearchService(IServiceProvider services)
        {
            var dataSourcesConfigService = (IDataSourcesConfigurationService)services.GetService(typeof(IDataSourcesConfigurationService));
            configuration = dataSourcesConfigService.Config.SearchServiceProviderConfiguration;
            QueryDetectorsUrl = configuration.SearchEndpoint + "/queryDetectors";
            QueryUtterancesUrl = configuration.SearchEndpoint + "/queryUtterances";
            TriggerTrainingUrl = configuration.TrainingEndpoint + "/triggerTraining";
            TriggerModelRefreshUrl = configuration.SearchEndpoint + "/refreshModel";
            InitializeHttpClient();
        }

        public Task<HttpResponseMessage> Get(HttpRequestMessage request)
        {
            return _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> SearchDetectors(string requestId, string query, Dictionary<string, string> parameters)
        {
            parameters.Add("text", query);
            parameters.Add("requestId", requestId ?? string.Empty);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, QueryDetectorsUrl);
            string authToken = await SearchServiceTokenService.Instance.GetAuthorizationTokenAsync();
            request.Headers.Add("Authorization", authToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
            return await Get(request);
        }

        public async Task<HttpResponseMessage> SearchUtterances(string requestId, string query, string[] detectorUtterances, Dictionary<string, string> parameters)
        {
            parameters.Add("detector_description", query);
            parameters.Add("detector_utterances", JsonConvert.SerializeObject(detectorUtterances));
            parameters.Add("requestId", requestId);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, QueryUtterancesUrl);
            string authToken = await SearchServiceTokenService.Instance.GetAuthorizationTokenAsync();
            request.Headers.Add("Authorization", authToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
            return await Get(request);
        }

        public async Task<HttpResponseMessage> TriggerTraining(string requestId, string trainingConfig, Dictionary<string, string> parameters)
        {
            parameters.Add("trainingConfig", trainingConfig);
            parameters.Add("requestId", requestId);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, TriggerTrainingUrl);
            string authToken = await SearchServiceTokenService.Instance.GetAuthorizationTokenAsync();
            request.Headers.Add("Authorization", authToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
            return await Get(request);
        }

        public async Task<HttpResponseMessage> TriggerModelRefresh(string requestId, Dictionary<string, string> parameters)
        {
            parameters.Add("requestId", requestId);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, AppendQueryStringParams(TriggerModelRefreshUrl, parameters));
            string authToken = await SearchServiceTokenService.Instance.GetAuthorizationTokenAsync();
            request.Headers.Add("Authorization", authToken);
            return await Get(request);
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }
        }

        private void InitializeHttpClient()
        {
            _httpClient = new HttpClient
            {
                MaxResponseContentBufferSize = Int32.MaxValue,
                Timeout = TimeSpan.FromSeconds(90)
            };

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private string AppendQueryStringParams(string url, Dictionary<string, string> additionalQueryParams)
        {
            var uriBuilder = new UriBuilder(url);
            var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (string key in additionalQueryParams.Keys)
            {
                queryParams.Add(key, additionalQueryParams.GetValueOrDefault(key));
            }

            uriBuilder.Query = queryParams.ToString();
            return uriBuilder.ToString();
        }
    }
}
