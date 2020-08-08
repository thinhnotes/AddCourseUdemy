using System;
using System.Linq;
using THttpWebRequest.Utility;

namespace THttpWebRequest.Helper
{
    public class StringHelper
    {
        /// <summary>
        ///     lấy username dạng viêt tắt của tên họ
        /// </summary>
        /// <param name="fullname">tên đầy đủ</param>
        /// <returns>username theo chuẩn viết tắt</returns>
        public static string GetUsername(string fullname)
        {
            fullname = fullname.RemoveVnChar();
            string[] sp = fullname.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string last = sp.Last();
            string mid = sp.Skip(1).FirstOrDefault();

            return (mid + last).ToLower();
        }
    }
}
