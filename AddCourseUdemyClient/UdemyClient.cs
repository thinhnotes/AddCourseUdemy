using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using AddCourseUdemyClient.Model;
using Newtonsoft.Json;
using THttpWebRequest;

namespace AddCourseUdemyDemo
{
    public class UdemyClient : TWebRequest
    {
        public bool Login(string user, string pass)
        {
            var csrfToken = GetCsrfToken();

            string urlData = $"csrfmiddlewaretoken={csrfToken}&locale=en_US&email={WebUtility.UrlEncode(user)}&password={WebUtility.UrlEncode(pass)}";
            Location = null;
            CookieCollection.Add(new Cookie("csrftoken", csrfToken, "/", "www.udemy.com"));
            Referer = "https://www.udemy.com/join/login-popup/?next=/user/edit-profile/";
            Post("https://www.udemy.com/join/login-popup/", urlData);
            if (Location != null)
            {
                Referer = null;
                return true;
            }
            return false;
        }

        public string GetCsrfToken()
        {
            Get("https://www.udemy.com");
            string url = "https://www.udemy.com/join/login-popup/";
            var content = Get(url);
            var regex = new Regex("name='csrfmiddlewaretoken' value='(?<key>.+)'");
            return regex.Match(content).Groups["key"]?.Value;
        }

        public int AddLink(string url)
        {
            if (CheckRegisteredCourse(url))
            {
                return 1;
            }
            var idCourse = GetIdCourse(url);
            var coupon = GetCounpon(url);
            AddToCart(idCourse);
            var checkOut = CheckOut(idCourse, coupon);
            if (checkOut)
            {
                Get(url, true);
                return 0;
            }
            return -1;
        }

        public string GetIdCourse(string url)
        {
            Location = null;
            var content = Get(url);
            var regex = new Regex("data-clp-course-id=\"(?<key>\\d+)");
            return regex.Match(content).Groups["key"]?.Value;
        }

        public bool CheckRegisteredCourse(string url)
        {
            Location = null;
            var content = Get(url);
            if (string.IsNullOrWhiteSpace(content) || !string.IsNullOrWhiteSpace(Location))
            {
                return true;
            }
            return false;
        }

        public bool AddToCart(string courseId)
        {
            var data = $"{{\"buyables\":[{{\"buyable_object_type\":\"course\",\"id\":{courseId},\"buyable_context\":{{}}}}]}}";

            string url = $"https://www.udemy.com/api-2.0/shopping-carts/me/cart/";
            WebHeaderCollection.Add("X-CSRFToken", CookieCollection["csrftoken"]?.Value);
            WebHeaderCollection.Add("X-Udemy-Authorization", $"Bearer {CookieCollection["access_token"]?.Value}");
            WebHeaderCollection.Add("Authorization", $"Bearer {CookieCollection["access_token"]?.Value}");
            WebHeaderCollection.Add("If-Match", $"W/\"bf1154452ff224164ef0875fc6c29661\"");
            Referer = "https://www.udemy.com/cart/checkout/";
            var post = Post(url, data, RequestType.Json);
            if (!string.IsNullOrWhiteSpace(post))
            {
                return true;
            }
            return false;
        }

        public bool CheckOut(string courseId, string coupon)
        {
            var data =
                $"{{\"shopping_cart\":{{\"items\":[{{\"discountInfo\":{{\"code\":\"{coupon}\"}},\"purchasePrice\":{{\"amount\":0,\"currency\":\"USD\",\"price_string\":\"Free\",\"currency_symbol\":\"$\"}},\"buyableType\":\"course\",\"buyableId\":{courseId},\"buyableContext\":{{}}}}],\"is_cart\":true}},\"is_tax_enabled\":true,\"payment_info\":{{\"payment_vendor\":\"Free\",\"payment_method\":\"free-method\"}},\"tax_info\":{{\"tax_rate\":\"0.000\",\"billing_location\":{{\"country_code\":\"VN\",\"secondary_location_info\":null}},\"currency_code\":\"usd\",\"transaction_items\":[{{\"tax_amount\":\"0.00\",\"udemy_txn_item_reference\":\"course-{courseId}\",\"tax_excluded_amount\":\"0.00\",\"tax_included_amount\":\"0\"}}]}}}}";

            string url = $"https://www.udemy.com/payment/checkout-submit/";
            Referer = "https://www.udemy.com/cart/checkout/";
            var post = Post(url, data, RequestType.Json);
            var response = JsonConvert.DeserializeObject<ResponseModel>(post);
            if (response != null && response.Status == "succeeded")
            {
                return true;
            }
            return false;
        }

        public string GetCounpon(string url)
        {
            var regex = new Regex("couponCode=(?<key>[A-Za-z\\-\\d]+)");
            return regex.Match(url).Groups["key"]?.Value;
        }
    }
}