﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Kusto.Data;
using Kusto.Data.Net.Client;
using Kusto.Data.Common;
using Kusto.Cloud.Platform.Data;
using System.Collections.Concurrent;

namespace Diagnostics.DataProviders
{
    class KustoSDKClient : IKustoClient
    {
        private string _requestId;
        private string _kustoApiQueryEndpoint;
        private string _appKey;
        private string _clientId;
        private string _dbName;
        private string _aadAuthority;


        /// <summary>
        /// Failover Cluster Mapping.
        /// </summary>
        public ConcurrentDictionary<string, string> FailoverClusterMapping { get; set; }


        public KustoSDKClient(KustoDataProviderConfiguration config, string requestId)
        {
            _requestId = requestId;
            _kustoApiQueryEndpoint = config.KustoApiEndpoint + ":443";
            _appKey = config.AppKey;
            _clientId = config.ClientId;
            _dbName = config.DBName;
            _aadAuthority = config.AADAuthority;
            FailoverClusterMapping = config.FailoverClusterNameCollection;
        }

        private ICslQueryProvider client(string cluster)
        {
            KustoConnectionStringBuilder connectionStringBuilder = new KustoConnectionStringBuilder(_kustoApiQueryEndpoint.Replace("{cluster}", cluster), _dbName);
            connectionStringBuilder.FederatedSecurity = true;
            connectionStringBuilder.ApplicationClientId = _clientId;
            connectionStringBuilder.ApplicationKey = _appKey;
            connectionStringBuilder.Authority = _aadAuthority;
            return Kusto.Data.Net.Client.KustoClientFactory.CreateCslQueryProvider(connectionStringBuilder);
        }

        public async Task<DataTable> ExecuteQueryAsync(string query, string cluster, string database, int timeoutSeconds, string requestId = null, string operationName = null)
        {
            throw new NotImplementedException();
        }

        public async Task<DataTable> ExecuteQueryAsync(string query, string cluster, string database, string requestId = null, string operationName = null)
        {
            throw new NotImplementedException();
        }

        public async Task<KustoQuery> GetKustoQueryAsync(string query, string cluster, string database)
        {
            throw new NotImplementedException();
        }

        public async Task<KustoQuery> GetKustoQueryAsync(string query, string cluster, string database, string operationName = null)
        {
            throw new NotImplementedException();
        }
    }
}
