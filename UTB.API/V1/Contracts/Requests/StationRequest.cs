using System;
using Microsoft.AspNetCore.Mvc;
using NodaTime;


namespace UTB.API.V1.Contracts.Requests
{
    public class StationRequest
    {
        [FromRoute(Name = "id")] 
        public Guid Id { get; set; }
        [FromQuery(Name = "include_measurements")]
        public bool IncludeMeasurements { get; set; }
        
        /// <summary>
        /// In ISO-8601 format
        /// </summary>
        /// <remarks>
        /// If the date is not in the correct format and you are not in the same timezone as the server you might end
        /// up with weird results. The ISO format is:
        ///
        ///  YYYY-MM-DDThh:mm:ss[.mmm]TZD (eg 2021-06-01T12:00:00-04:00)
        /// 
        ///  YYYY = four-digit year
        ///  MM = two-digit month (eg 03=March)
        ///  DD = two-digit day of the month (01 through 31)
        ///  T = a set character indicating the start of the time element
        ///  hh = two digits of an hour (00 through 23, AM/PM not included)
        ///  mm = two digits of a minute (00 through 59)
        ///  ss = two digits of a second (00 through 59)
        ///  mmm = three digits of a millisecond (000 through 999)
        ///  TZD = time zone designator (Z or +hh:mm or -hh:mm), the + or - values indicate how far ahead or behind a time zone is from the UTC (Coordinated Universal Time) zone.
        /// 
        ///      US time zone values are as follows:
        /// 
        ///  EDT = -4:00
        ///  EST/CDT = -5:00
        ///  CST/MDT = -6:00
        ///  MST/PDT = -7:00
        ///  PST = -8:00
        /// </remarks>
        /// <example>2021-06-01T12:00:00-04:00</example>
        [FromQuery(Name = "start_date")]
        public DateTimeOffset? StartDate { get; set; }
        
        /// <summary>
        /// In ISO-8601 format
        /// </summary>
        /// <remarks>
        /// See start_date for reference
        /// </remarks>
        /// <example>2021-06-01T20:00:00-04:00</example>
        [FromQuery(Name = "end_date")]
        public DateTimeOffset? EndDate { get; set; }
    }
}