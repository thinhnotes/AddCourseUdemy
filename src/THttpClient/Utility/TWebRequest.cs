using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace THttpClient.Utility
{
    public class TWebRequest
    {
        private string _userAngent;

        protected TWebRequest()
        {
            CookieCollection = new CookieCollection();
            CookieContainer = new CookieContainer();
        }

        protected TWebRequest(CookieCollection cookie)
        {
            CookieCollection = cookie;
        }

        protected CookieCollection CookieCollection { get; set; }
        protected CookieContainer CookieContainer { get; set; }
        protected bool AutoRedirect { get; set; }
        protected string Location { get; set; }
        protected string Referer { get; set; }
        protected string Accept { get; set; }

        protected TypeRequest Type { get; set; }

        protected string UserAngent
        {
            get
            {
                return _userAngent ??
                       "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.104 Safari/537.36";
            }
            set { _userAngent = value; }
        }

        protected bool Gzip { get; set; }
        private Uri Uri { get; set; }

        private Uri UriHost
        {
            get { return new Uri(Uri.Scheme + "://" + Uri.Host); }
        }

        private async Task<HttpResponseMessage> GetResponse(string url, string postData = null, string method = "GET")
        {
            Uri = new Uri(url);
            using (var client = InitHttpClient())
            {

                HttpResponseMessage response;
                if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    var contentType = "application/x-www-form-urlencoded";
                    if (Type == TypeRequest.Normal)
                    {
                        contentType = "application/x-www-form-urlencoded";
                    }
                    else
                    {
                        contentType = "application/json";
                    }

                    var request = new HttpRequestMessage(HttpMethod.Post, Uri) { Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded") };


                    response = await client.SendAsync(request);

                    //response = await client.PostAsync(Uri, new StringContent(postData, Encoding.UTF8, contentType));
                }
                else
                {
                    response = await client.GetAsync(Uri);
                }
                CookieCollection = CookieContainer.GetCookies(UriHost);
                return response;
            }
        }


        private async Task<Stream> GetStream(string url, string postData = null, string method = "GET")
        {
            HttpResponseMessage response = null;
            try
            {
                response = await GetResponse(url, postData, method);

            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
                throw;
            }

            if (AutoRedirect == false)
            {
                if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.Found)
                {
                    // Do something...
                    Location = response.Headers.Location.AbsoluteUri;
                }
            }
            //CookieCollection.Add(response.Cookies);
            CookieCollection = new CookieCollection();
            Stream responseStream = await response.Content.ReadAsStreamAsync();

            var cookieHeaders = response.Headers.Where(pair => pair.Key == "Set-Cookie").ToList();
            foreach (var cookie in cookieHeaders.FirstOrDefault().Value)
            {
                var cookieValues = cookie.Split(';');
                var keyCookie = cookieValues.First();
                var keyValue = keyCookie.Split('=');
                if (!string.IsNullOrEmpty(keyValue[1]))
                {
                    //var addCookie = new Cookie(keyValue[0], keyValue[1]);
                    //foreach (var cookieValue in cookieValues)
                    //{
                        
                    //}
                    //if (keyCookie.Contains("access_token") || keyCookie.Contains("client_id"))
                        CookieContainer.SetCookies(UriHost, keyCookie);
                }
            }
            CookieCollection = CookieContainer.GetCookies(UriHost);
            return responseStream;
        }

        private async Task<string> GetContent(string url, string postData = null, string method = "GET")
        {
            using (Stream stream = await GetStream(url, postData, method))
            {
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public async Task<string> GetAsync(string url, bool autoRedirect = false)
        {
            AutoRedirect = autoRedirect;
            return await GetContent(url);
        }

        public string Get(string url, bool autoRedirect = false)
        {
            return GetAsync(url, autoRedirect).Result;
        }

        protected async Task<string> PostAsync(string url, string data, TypeRequest type = TypeRequest.Normal, bool autoRedirect = false)
        {
            AutoRedirect = autoRedirect;
            Type = type;
            return await GetContent(url, data, "POST");
        }

        public string Post(string url, string data, TypeRequest type = TypeRequest.Normal, bool autoRedirect = false)
        {
            return PostAsync(url, data, type, autoRedirect).Result;
        }

        //public string Post(string url, params KeyValuePair<string, string>[] data)
        //{
        //    return PostAsync(url, data).Result;
        //}

        private HttpClient InitHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = AutoRedirect,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            if (handler.CookieContainer == null)
            {
                handler.CookieContainer = new CookieContainer();
            }
            //handler.CookieContainer = CookieContainer;
            handler.CookieContainer = new CookieContainer();
            if (CookieCollection != null)
            {
                handler.CookieContainer.Add(Uri, CookieCollection);
                //var cookieCollection = handler.CookieContainer.GetCookies(UriHost);
                //foreach (Cookie cookie in CookieCollection)
                //{
                //    //foreach (Cookie currentCookie in cookieCollection)
                //    //{
                //        //if (currentCookie.Name != cookie.Name)
                //        //{
                            
                //        //}
                //    //}
                //}
            }
            var client = new HttpClient(handler);
            SetInitHttpClient(client);
            return client;
        }

        protected void SetInitHttpClient(HttpClient httpClient)
        {
            var stringAccept = string.IsNullOrWhiteSpace(Accept) ? "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8" : Accept;

            httpClient.DefaultRequestHeaders.Add("Accept", stringAccept);
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en,vi;q=0.8,en-US;q=0.6");
            if (!string.IsNullOrWhiteSpace(Referer))
            {
                httpClient.DefaultRequestHeaders.Referrer = new Uri(Referer);
            }
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAngent);
        }

        //private CookieCollection GetCookies(CookieContainer cookies)
        //{
        //    var cookies = new CookieCollection();
        //    Hashtable table = (Hashtable)cookies.GetType().InvokeMember("m_domainTable",
        //        BindingFlags.NonPublic |
        //        BindingFlags.GetField |
        //        BindingFlags.Instance,
        //        null,
        //        cookies,
        //        new object[] { });



        //    foreach (var key in table.Keys)
        //    {
        //        foreach (Cookie cookie in cookies.GetCookies(new Uri(string.Format("http://{0}/", key))))
        //        {
        //            cookies.Add(cookie);
        //        }
        //    }
        //    return cookies;
        //}
    }

    public enum TypeRequest
    {
        Normal,
        Json
    }
}