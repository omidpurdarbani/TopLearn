using System;
using System.Collections.Generic;
using System.Text;

namespace TopLearn.Core.Convertors
{
    public static class Fixedtext
    {
        public static string FixEmail(this string email)
        {
            return email.Trim().ToLower();
        }
    }
}
