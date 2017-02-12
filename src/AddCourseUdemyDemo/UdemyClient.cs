using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using THttpClient.Utility;

namespace AddCourseUdemyDemo
{
    public class UdemyClient : TWebRequest
    {
        public bool Login(string user, string pass)
        {
            string url = "https://www.udemy.com/join/login-popup/";
            Get(url);
            var cookieCollection = this.CookieContainer.GetCookies(new Uri(url));
            var cookie = cookieCollection["csrftoken"];
            Referer = url;

            string urlData = $"csrfmiddlewaretoken={cookie.Value}&locale=en_US&email={WebUtility.UrlEncode(user)}&password={WebUtility.UrlEncode(pass)}";
            Location = null;
            Post("https://www.udemy.com/join/login-popup/", urlData);
            if (Location != null)
            {
                return true;
            }
            return false;
        }

        public bool AddLink(string url)
        {
            var idCourse = GetIdCourse(url);
            var counpon = GetCounpon(url);
            var checkOut = CheckOut(idCourse, counpon);
            if (checkOut)
            {
                Get(url, true);
            }
            return checkOut;
        }

        public string GetIdCourse(string url)
        {
            var content = Get(url);
            var regex = new Regex("data-clp-course-id=\"(?<key>\\d+)");
            return regex.Match(content).Groups["key"]?.Value;
        }

        public bool CheckOut(string courseId, string coupon)
        {
            string url = $"https://www.udemy.com/payment/checkout/?boType=course&boId={courseId}&couponCode={coupon}";
            var content = Get(url);
            var regex = new Regex("data-url=\"(?<key>[a-z/\\dA-Z]+)\"");
            return string.IsNullOrWhiteSpace(regex.Match(content).Groups["key"]?.Value);
        }

        public string GetCounpon(string url)
        {
            var regex = new Regex("couponCode=(?<key>[A-Za-z\\-\\d]+)&");
            return regex.Match(url).Groups["key"]?.Value;
        }
    }
}