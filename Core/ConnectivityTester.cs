using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBypass.Core
{
    public class EndpointStatus
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsReachable { get; set; }
        public long LatencyMs { get; set; }
        public int StatusCode { get; set; }
        public string Details { get; set; } = string.Empty;
    }

    public static class ConnectivityTester
    {
        private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = true,
            CheckCertificateRevocationList = false
        })
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        static ConnectivityTester()
        {
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        }

        public static async Task<List<EndpointStatus>> TestAllEndpointsAsync(CancellationToken ct = default)
        {
            var targets = new (string Name, string Url)[]
            {
                ("Discord Web Ana Sayfa", "https://discord.com"),
                ("Discord API Gateway", "https://discord.com/api/v10/gateway"),
                ("Discord Medya & CDN", "https://cdn.discordapp.com/emojis/1.png"),
                ("Discord Durum Sunucusu", "https://status.discord.com")
            };

            var results = new List<EndpointStatus>();

            foreach (var target in targets)
            {
                var status = await TestEndpointAsync(target.Name, target.Url, ct);
                results.Add(status);
            }

            return results;
        }

        public static async Task<EndpointStatus> TestEndpointAsync(string name, string url, CancellationToken ct = default)
        {
            var result = new EndpointStatus
            {
                Name = name,
                Url = url
            };

            var sw = Stopwatch.StartNew();
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
                sw.Stop();

                result.LatencyMs = sw.ElapsedMilliseconds;
                result.StatusCode = (int)response.StatusCode;
                result.IsReachable = response.IsSuccessStatusCode || (int)response.StatusCode < 500;
                result.Details = $"HTTP {result.StatusCode} ({(result.IsReachable ? "Erişilebilir" : "Engelli/Hata")})";
            }
            catch (Exception ex)
            {
                sw.Stop();
                result.LatencyMs = sw.ElapsedMilliseconds;
                result.IsReachable = false;
                result.StatusCode = 0;
                result.Details = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        public static async Task<long> PingHostAsync(string host)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(host, 3000);
                if (reply.Status == IPStatus.Success)
                {
                    return reply.RoundtripTime;
                }
            }
            catch { }
            return -1;
        }
    }
}
