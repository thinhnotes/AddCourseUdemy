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
            var csrfToken = GetCsrfToken();

            string urlData = $"csrfmiddlewaretoken={csrfToken}&locale=en_US&email={WebUtility.UrlEncode(user)}&password={WebUtility.UrlEncode(pass)}";
            Location = null;
            CookieCollection.Add(new Cookie("csrftoken", csrfToken));
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
            if (CheckRegistedCourse(url))
            {
                return 1;
            }
            var idCourse = GetIdCourse(url);
            var counpon = GetCounpon(url);
            var checkOut = CheckOut(idCourse, counpon);
            if (checkOut)
            {
                Get(url, true);
                return 0;
            }
            return  -1;
        }

        public string GetIdCourse(string url)
        {
            Location = null;
            var content = Get(url);
            var regex = new Regex("data-clp-course-id=\"(?<key>\\d+)");
            return regex.Match(content).Groups["key"]?.Value;
        }

        public bool CheckRegistedCourse(string url)
        {
            Location = null;
            var content = Get(url);
            if (string.IsNullOrWhiteSpace(content) || !string.IsNullOrWhiteSpace(Location))
            {
                return true;
            }
            return false;
        }

        public bool CheckOut(string courseId, string coupon)
        {
            string url = $"https://www.udemy.com/payment/checkout/?boType=course&boId={courseId}&couponCode={coupon}";
            Get("https://www.udemy.com");
            Location = null;
            Get(url);
            if (Location == null) return false;
            Get(Location);
            if (Location == null) return false;
            if (Location.Contains("https://www.udemy.com/cart/success"))
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