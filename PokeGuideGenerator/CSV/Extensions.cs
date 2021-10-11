using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeGuideGenerator
{
    public static class Extensions
    {
        public static string ToHumanFormat(this bool value)
        {
            return value ? "yes" : "no";
        }
    }
}
