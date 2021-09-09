using System;

namespace Common
{
    public static class Guard
    {
        public static void ThrowIf(bool expression, string msg)
        {
            if (expression)
            {
                throw new ArgumentException(msg);
            }
        }
    }
}