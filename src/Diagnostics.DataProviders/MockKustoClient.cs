﻿using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Diagnostics.Tests")]
namespace Diagnostics.DataProviders
{
    internal class MockKustoClient : IKustoClient
    {
        internal static bool ShouldHeartbeatSucceed = false;
        internal static int HeartBeatRuns = 0;

        public async Task<DataTable> ExecuteQueryAsync(string query, string cluster, string database, int timeoutSeconds, string requestId = null, string operationName = null)
        {
            return await ExecuteQueryAsync(query, cluster, database, requestId, operationName);
        }

            public async Task<DataTable> ExecuteQueryAsync(string query, string cluster, string database, string requestId = null, string operationName = null)
        {
            if (string.IsNullOrWhiteSpace(cluster))
            {
                throw new ArgumentNullException("cluster");
            }

            if (string.IsNullOrWhiteSpace(database))
            {
                throw new ArgumentNullException("database");
            }

            if (!string.IsNullOrWhiteSpace(operationName))
            {
                switch (operationName.ToLower())
                {
                    case KustoOperations.GetTenantIdForWindows:
                    case KustoOperations.GetTenantIdForLinux:
                        return await GetFakeTenantIdResults();
                    case KustoOperations.GetLatestDeployment:
                        return await GetLatestDeployment();
                    default:
                        return await GetTestA();
                }
            }

            switch (query)
            {
                case "TestA":
                    return await GetTestA();
                case "Heartbeat":
                    ++HeartBeatRuns;
                    if (ShouldHeartbeatSucceed) {
                        return await GetFakeTenantIdResults();
                    }
                    break;
            }

            return new DataTable();
        }

        private Task<DataTable> GetFakeTenantIdResults()
        {
            DataTable table = new DataTable();

            table.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Tenant", typeof(string)),
                new DataColumn("PublicHost", typeof(string))
            });

            table.Rows.Add(new string[2] { Guid.NewGuid().ToString(), "fakestamp.cloudapp.net" });

            return Task.FromResult(table);
        }

        private Task<DataTable> GetTestA()
        {
            DataTable table = new DataTable();

            table.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("TestColumn", typeof(string))
            });

            return Task.FromResult(table);
        }

        private Task<DataTable> GetLatestDeployment()
        {
            DataTable table = new DataTable();

            table.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("LatestDeployment", typeof(DateTime))
            });

            table.Rows.Add(new object[] { DateTime.UtcNow });

            return Task.FromResult(table);
        }

        public Task<KustoQuery> GetKustoQueryAsync(string query, string cluster, string database)
        {
            return GetKustoQueryAsync(query, cluster, database, null);
        }

        public Task<KustoQuery> GetKustoQueryAsync(string query, string cluster, string database, string operationName)
        {
            if (string.IsNullOrWhiteSpace(cluster))
            {
                throw new ArgumentNullException("cluster");
            }

            if (string.IsNullOrWhiteSpace(database))
            {
                throw new ArgumentNullException("database");
            }
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException("query");
            }
            KustoQuery k = new KustoQuery
            {
                Url = "https://fakekusto.windows.net/q=somequery",
                KustoDesktopUrl = "https://fakekusto.windows.net/q=somequery",
                Text = query
            };
            return Task.FromResult(k);
        }
    }
}
