using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ToolMail.Constant
{
    public class ProxyConstant
    {
        private readonly List<string> _apiKeys;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _proxySemaphores;
        private readonly int _maxTasksPerProxy;

        public ProxyConstant(List<string> apiKeys, int maxTasksPerProxy)
        {
            _apiKeys = apiKeys;
            _proxySemaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
            _maxTasksPerProxy = maxTasksPerProxy;
        }

        public async Task<List<string>> GetProxiesAsync()
        {
            var proxies = new List<string>();
            var tasks = _apiKeys.Select(apiKey => GetProxyAsync(apiKey, proxies)).ToArray();
            await Task.WhenAll(tasks);
            return proxies;
        }

        private async Task GetProxyAsync(string apiKey, List<string> proxies)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"https://api.example.com/get-proxy?apiKey={apiKey}");
                response.EnsureSuccessStatusCode();
                var proxy = await response.Content.ReadAsStringAsync();
                proxies.Add(proxy);
                _proxySemaphores[proxy] = new SemaphoreSlim(_maxTasksPerProxy);
            }
        }

        public async Task UseProxyAsync(string proxy, Func<Task> task)
        {
            var semaphore = _proxySemaphores[proxy];
            await semaphore.WaitAsync();
            try
            {
                await task();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
