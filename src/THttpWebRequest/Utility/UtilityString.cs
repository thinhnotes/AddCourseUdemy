using System;
using System.Linq;
using System.Text;
using System.Web;

namespace THttpWebRequest.Utility
{
    public static class UtilityString
    {
        public static string CombineUrl(this string s, string url)
        {
            var baseUri = new Uri(s);
            return new Uri(baseUri, url).OriginalString;
        }

        //xxx
        public static bool TestWildcard(this string s, string name)
        {
            return false;
        }

        public static string HtmlDecode(this string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        public static string SubStringSpaceLengh(this string str, int lenght)
        {
            string[] strings = str.Split(' ');
            string resuft = "";
            foreach (string s in strings)
            {
                if (resuft.Length > lenght)
                    break;
                resuft += s + " ";
            }
            return resuft;
        }

        /// <summary>
        ///     xóa dấu tính việt
        /// </summary>
        /// <param name="str">chuỗi cần xóa dấu</param>
        /// <returns>chuỗi đã xóa dấu</returns>
        public static string RemoveVnChar(this string str)
        {
            string engs = "aAeEoOuUiIdDyY";
            string[] vns =
            {
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ"
            };
            var sb = new StringBuilder();
            foreach (char ch in str)
            {
                int i;
                for (i = 0; i < vns.Length; i++)
                    if (vns[i].Contains(ch)) break;
                sb.Append(i < vns.Length ? engs[i] : ch);
            }
            return sb.ToString();
        }

        
    }
}