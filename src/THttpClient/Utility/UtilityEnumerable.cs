﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace THttpClient
{
    public static class UtilityEnumerable
    {
        public static string ToJsonString<T>(this IEnumerable<T> enumrable)
        {
            return JsonConvert.SerializeObject(enumrable);
        }
    }
}