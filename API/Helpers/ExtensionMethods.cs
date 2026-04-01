using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Helpers
{
    public class ExtensionMethods
    {
        public class TurkeyDateTime
        {
            public static DateTime Now
            {
                get
                {
                    return Turkey();
                }
            }

            public static DateTime Today
            {
                get
                {
                    return Turkey().Date;
                }
            }

            private static DateTime Turkey()
            {
                DateTime result = DateTime.Now;

                try
                {
                    var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

                    result = TimeZoneInfo.ConvertTime(DateTime.Now, turkeyTimeZone);
                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }

    }
}
