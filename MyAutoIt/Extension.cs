using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAutoIt
{
    public static class Extension
    {
        public static String ToString<T>(this T[] _self)
        {
            return JsonConvert.SerializeObject(_self);
        }

    }
}
