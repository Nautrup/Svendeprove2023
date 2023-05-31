using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Common
{
    public static class UnixConversion
    {
        public static long ToUnixTime(DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            return dto.ToUnixTimeSeconds();
        }

        public static long ToUnixTimeMilliSeconds(DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            return dto.ToUnixTimeMilliseconds();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTime;
        }
    }
}
