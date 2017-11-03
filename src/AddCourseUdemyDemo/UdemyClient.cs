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
            Get("https://www.udemy.com");
            Location = null;
            Get(url);
            if (Location == null) return false;
            Get(Location);
            if (Location == null) return false;
            var successContent = Get(Location);
            if (successContent.Contains("Congratulations! You've successfully enrolled in"))
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