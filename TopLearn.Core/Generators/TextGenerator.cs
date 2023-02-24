using System;

namespace TopLearn.Core.Generator
{
    public static class TextGenerator
    {
        public static string GenerateUniqCode()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
