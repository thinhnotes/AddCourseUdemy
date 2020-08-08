using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THttpWebRequest.Helper
{
    internal class RandomHelper
    {
        private static readonly Random r = new Random();

        public static string GetRandomStringFrom(IEnumerable<string> s, int maxLength)
        {
            int n = s.Count();
            string[] arr = s.ToArray();
            var sb = new StringBuilder();

            while (sb.ToString().Length < maxLength)
            {
                sb.Append(arr[r.Next(n - 1)]);
            }
            return sb.ToString();
        }

        public static string GetRandomStringFromWithLoop(IEnumerable<string> s, int loopTimes = 1)
        {
            int n = s.Count();
            string[] arr = s.ToArray();
            var sb = new StringBuilder();
            for (int i = 0; i < loopTimes; i++)
            {
                sb.Append(arr[r.Next(n - 1)]);
            }

            return sb.ToString();
        }


        public static string GetRandomStringFrom(string s, int maxLength)
        {
            return GetRandomStringFrom(s.ToCharArray().Select(c => c.ToString()), maxLength);
        }
    }
}