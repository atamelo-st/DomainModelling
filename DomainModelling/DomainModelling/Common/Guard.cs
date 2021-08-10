using System;


namespace DomainModelling.Common
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