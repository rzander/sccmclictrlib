//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2018 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Globalization;
using System.Linq;

namespace sccmclictr.automation.schedule
{
    /// <summary>
    /// Class ScheduleDecoding.
    /// </summary>
    public static class ScheduleDecoding
    {
        //Schedule ID decoding was possible because of the reverse engineering work from Jeff Huston 
        //<http://myitforum.com/cs2/blogs/jhuston/archive/2007/07/30/sms-schedule-token-strings.aspx>


        /// <summary>
        /// Chech if ScheduleID is a NonRecuring Schedule
        /// </summary>
        /// <param name="ScheduleID"></param>
        /// <returns></returns>
        internal static Boolean isNonRecurring(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Check if bit signature is 001 on Position 19
            if ((lSchedID >> 19 & 7) == 1)
                return true;
            return false;
        }

        internal static Boolean isRecurInterval(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Check if bit signature is 010 on Position 19
            if ((lSchedID >> 19 & 7) == 2)
                return true;
            return false;
        }

        internal static Boolean isRecurWeekly(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Check if bit signature is 011 on Position 19
            if ((lSchedID >> 19 & 7) == 3)
                return true;
            return false;
        }

        internal static Boolean isRecurMonthlyByWeekday(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Check if bit signature is 100 on Position 19
            if ((lSchedID >> 19 & 7) == 4)
                return true;
            return false;
        }

        internal static Boolean isRecurMonthlyByDate(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Check if bit signature is 101 on Position 19
            if ((lSchedID >> 19 & 7) == 5)
                return true;
            return false;
        }

        internal static Boolean isgmt(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 1Bit from position 1
            long lstart = (lSchedID & 1);
            return System.Convert.ToBoolean(lstart);
        }

        internal static int dayspan(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 3
            long lstart = (lSchedID >> 3 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int hourpan(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 8
            long lstart = (lSchedID >> 8 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int minutespan(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 6Bit's from position 13
            long lstart = (lSchedID >> 13 & 63);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int weekorder(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 3Bit's from position 9
            long lstart = (lSchedID >> 9 & 7);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int fornumberofweeks(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 3Bit's from position 13
            long lstart = (lSchedID >> 13 & 7);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int fornumberofmonths(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 4Bit's from position 12
            long lstart = (lSchedID >> 12 & 15);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int fornumberofmonths2(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 4Bit's from position 10
            long lstart = (lSchedID >> 10 & 15);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int iDay(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 3Bit's from position 16
            long lstart = (lSchedID >> 16 & 7);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int monthday(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 14
            long lstart = (lSchedID >> 14 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int dayduration(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 22
            long lstart = (lSchedID >> 22 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int hourduration(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 27
            long lstart = (lSchedID >> 27 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int minuteduration(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 6Bit's from position 32
            long lstart = (lSchedID >> 32 & 63);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int startyear(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 6Bit's from position 38
            long lstart = (lSchedID >> 38 & 63);
            int iRes = System.Convert.ToInt16(lstart + 1970);
            return iRes;
        }

        internal static int startmonth(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 4Bit's from position 44
            long lstart = (lSchedID >> 44 & 15);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int startday(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 48
            long lstart = (lSchedID >> 48 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int starthour(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 53
            long lstart = (lSchedID >> 53 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int startminute(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 6Bit's from position 58
            long lstart = (lSchedID >> 58 & 63);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static string encodeID(object Schedule)
        {
            long lSchedID = new long();
            int lo = 0;
            int hi = 0;

            SMS_ST_NonRecurring oSched = Schedule as SMS_ST_NonRecurring;

            #region BaseSchedule
            //IsGMT Flag
            if (oSched.IsGMT)
            {
                lo |= 1;
            }

            //StartTime Year (check if in a valid range)
            if ((oSched.StartTime.Year > 1970) & (oSched.StartTime.Year < 2033))
            {
                hi |= (oSched.StartTime.Year - 1970) << 6;
            }
            else
            {
                hi |= 63 << 6;
            }

            //StartTime Month
            hi |= oSched.StartTime.Month << 12;

            //StartTime Day
            hi |= oSched.StartTime.Day << 16;

            //StartTime Hour
            hi |= oSched.StartTime.Hour << 21;

            //StartTime Minute
            hi |= oSched.StartTime.Minute << 26;

            //Day duration
            lo |= oSched.DayDuration << 22;

            //hourduration duration
            lo |= oSched.HourDuration << 27;

            //Minute duration
            hi |= oSched.MinuteDuration;
            #endregion

            switch (Schedule.GetType().Name)
            {

                case ("SMS_ST_NonRecurring"):
                    //Set type to SMS_ST_NonRecurring
                    lo |= 1 << 19;
                    break;

                case ("SMS_ST_RecurInterval"):
                    SMS_ST_RecurInterval oSchedRI = oSched as SMS_ST_RecurInterval;

                    //DaySpan
                    lo |= oSchedRI.DaySpan << 3;

                    //HourSpan
                    lo |= oSchedRI.HourSpan << 8;

                    //MinuteSpan
                    lo |= oSchedRI.MinuteSpan << 13;

                    //Set type to SMS_ST_RecurInterval
                    lo |= 2 << 19;
                    break;
            }

            //Convert to HEX
            lSchedID = (((long)hi) << 32) | ((uint)lo);
            string sResult = lSchedID.ToString("X");

            //Result must always have 16 Characters, otherwise ConfigMgr does not understand the Format ! 
            while (sResult.Length < 16)
            {
                sResult = '0' + sResult;
            }
            return sResult;
        }

        /// <summary>
        /// Decode an SMS ScheduleID string
        /// </summary>
        /// <param name="ScheduleID">SMS encoded 64bit ScheduleID string</param>
        /// <returns>object of type: SMS_ST_NonRecurring, SMS_ST_RecurInterval, SMS_ST_RecurWeekly, SMS_ST_RecurMonthlyByWeekday or SMS_ST_RecurMonthlyByDate</returns>
        public static object DecodeScheduleID(string ScheduleID)
        {
            try
            {
                int year = startyear(ScheduleID);
                int month = startmonth(ScheduleID);
                int day = startday(ScheduleID);
                int hour = starthour(ScheduleID);
                int minute = startminute(ScheduleID);

                if (isNonRecurring(ScheduleID))
                {
                    SMS_ST_NonRecurring oRes = new SMS_ST_NonRecurring();
                    oRes.IsGMT = isgmt(ScheduleID);
                    oRes.StartTime = new DateTime(year, month, day, hour, minute, 0);
                    oRes.DayDuration = dayduration(ScheduleID);
                    oRes.HourDuration = hourduration(ScheduleID);
                    oRes.MinuteDuration = minuteduration(ScheduleID);
                    return oRes;
                }

                if (isRecurInterval(ScheduleID))
                {
                    SMS_ST_RecurInterval oRes = new SMS_ST_RecurInterval();
                    oRes.IsGMT = isgmt(ScheduleID);
                    oRes.StartTime = new DateTime(year, month, day, hour, minute, 0);
                    oRes.DayDuration = dayduration(ScheduleID);
                    oRes.DaySpan = dayspan(ScheduleID);
                    oRes.HourDuration = hourduration(ScheduleID);
                    oRes.HourSpan = hourpan(ScheduleID);
                    oRes.MinuteDuration = minuteduration(ScheduleID);
                    oRes.MinuteSpan = minutespan(ScheduleID);
                    return oRes;
                }

                if (isRecurWeekly(ScheduleID))
                {
                    SMS_ST_RecurWeekly oRes = new SMS_ST_RecurWeekly();
                    oRes.IsGMT = isgmt(ScheduleID);
                    oRes.StartTime = new DateTime(year, month, day, hour, minute, 0);
                    oRes.Day = iDay(ScheduleID);
                    oRes.ForNumberOfWeeks = fornumberofweeks(ScheduleID);
                    oRes.DayDuration = dayduration(ScheduleID);
                    oRes.HourDuration = hourduration(ScheduleID);
                    oRes.MinuteDuration = minuteduration(ScheduleID);
                    return oRes;
                }

                if (isRecurMonthlyByWeekday(ScheduleID))
                {
                    SMS_ST_RecurMonthlyByWeekday oRes = new SMS_ST_RecurMonthlyByWeekday();
                    oRes.IsGMT = isgmt(ScheduleID);
                    oRes.StartTime = new DateTime(year, month, day, hour, minute, 0);
                    oRes.WeekOrder = weekorder(ScheduleID);
                    oRes.Day = iDay(ScheduleID);
                    oRes.ForNumberOfMonths = fornumberofmonths(ScheduleID);
                    oRes.DayDuration = dayduration(ScheduleID);
                    oRes.HourDuration = hourduration(ScheduleID);
                    oRes.MinuteDuration = minuteduration(ScheduleID);
                    return oRes;
                }

                if (isRecurMonthlyByDate(ScheduleID))
                {
                    SMS_ST_RecurMonthlyByDate oRes = new SMS_ST_RecurMonthlyByDate();
                    oRes.IsGMT = isgmt(ScheduleID);
                    oRes.StartTime = new DateTime(year, month, day, hour, minute, 0);
                    oRes.ForNumberOfMonths = fornumberofmonths2(ScheduleID);
                    oRes.MonthDay = monthday(ScheduleID);
                    oRes.DayDuration = dayduration(ScheduleID);
                    oRes.HourDuration = hourduration(ScheduleID);
                    oRes.MinuteDuration = minuteduration(ScheduleID);
                    return oRes;
                }
            }
            catch { }
            return null;
        }

        public static DateTime GetNthWeekdayOfMonth(DateTime dt, int n, DayOfWeek weekday)
        {
            var days = Enumerable.Range(1, DateTime.DaysInMonth(dt.Year, dt.Month)).Select(day => new DateTime(dt.Year, dt.Month, day));

            var weekdays = from day in days
                           where day.DayOfWeek == weekday
                           orderby day.Day ascending
                           select day;

            int index = n - 1;

            if (index >= 0 && index < weekdays.Count())
                return weekdays.ElementAt(index);

            else
            {
                // if the month doesn't has the 5th day than return the 4th day because in SCCM the 5th spells "the last"
                if(n == 5)
                {
                    index = 3;
                    if (index >= 0 && index < weekdays.Count())
                        return weekdays.ElementAt(index);
                }

                throw new InvalidOperationException("The specified day does not exist in this month!");
            }
                
        }

        /// <summary>
        /// split the scheduleID string into 16char substrings
        /// </summary>
        /// <param name="ScheduleID"></param>
        /// <returns>16char ScheduleIDs</returns>
        public static string[] GetScheduleIDs(string ScheduleID)
        {
            string[] aSchedIds;
            if (ScheduleID.Length < 16)
            {
                aSchedIds = new string[1];
                aSchedIds[0] = ScheduleID;
                return aSchedIds;
            }
            else
            {
                aSchedIds = new string[(ScheduleID.Length) / 16];
            }
            int i = 0;
            while ((i + 1) * 16 <= ScheduleID.Length)
            {
                aSchedIds[i] = ScheduleID.Substring(i * 16, 16);
                i++;
            }
            return aSchedIds;
        }

        /// <summary>
        /// Non recuring schedule
        /// </summary>
        public class SMS_ST_NonRecurring
        {
            //public SMS_ST_NonRecurring() { }

            /// <summary>
            /// duration in Days
            /// </summary>
            public int DayDuration { get; set; }

            /// <summary>
            /// duration in hours
            /// </summary>
            public int HourDuration { get; set; }

            /// <summary>
            /// Time is GMT Time
            /// </summary>
            public Boolean IsGMT { get; set; }

            /// <summary>
            /// duration in minutes
            /// </summary>
            public int MinuteDuration { get; set; }

            /// <summary>
            /// Get or set the start time
            /// </summary>
            public DateTime StartTime { get; set; }

            /// <summary>
            /// Get the next start time
            /// </summary>
            public DateTime NextStartTime
            {
                get
                {
                    return StartTime;
                }
            }

            /// <summary>
            /// get the ScheduleID
            /// </summary>
            public string ScheduleID
            {
                get
                {
                    return encodeID(this);
                }
            }
        }

        /// <summary>
        /// Interval Schedule (day, hour, minute)
        /// </summary>
        public class SMS_ST_RecurInterval : SMS_ST_NonRecurring
        {
            //public SMS_ST_RecurInterval() { }

            /// <summary>
            /// Interval span in days
            /// </summary>
            public int DaySpan { get; set; }

            /// <summary>
            /// Interval span in hours
            /// </summary>
            public int HourSpan { get; set; }

            /// <summary>
            /// Interval span in minutes
            /// </summary>
            public int MinuteSpan { get; set; }

            /// <summary>
            /// get the next start time
            /// </summary>
            public new DateTime NextStartTime
            {
                get
                {
                    DateTime dEndTime = new DateTime();

                    //determine the new start date-time
                    DateTime oNextStartTime = base.StartTime.Subtract(new TimeSpan(DaySpan, HourSpan, MinuteSpan, 0));
                    dEndTime = oNextStartTime + new TimeSpan(this.DayDuration, this.HourDuration, this.MinuteDuration, 0);
                    while (dEndTime < StartTime)
                    {
                        dEndTime = dEndTime + new TimeSpan(DaySpan, HourSpan, MinuteSpan, 0);
                        oNextStartTime = oNextStartTime + new TimeSpan(DaySpan, HourSpan, MinuteSpan, 0);
                    }
                    return oNextStartTime;

                }
            }

            /// <summary>
            /// The last Start Time in the past...
            /// </summary>
            public DateTime PreviousStartTime
            {
                get
                {
                    //determine the new start date-time
                    DateTime oNextStartTime = base.StartTime.Subtract(new TimeSpan(DaySpan, HourSpan, MinuteSpan, 0));
                    DateTime oPrevDate = oNextStartTime;
                    while (oNextStartTime < DateTime.Now)
                    {
                        oPrevDate = oNextStartTime;
                        oNextStartTime = oNextStartTime + new TimeSpan(DaySpan, HourSpan, MinuteSpan, 0);
                    }
                    if (oNextStartTime > DateTime.Now)
                        return oPrevDate;
                    else
                        return oNextStartTime;

                }
            }
        }

        /// <summary>
        /// Weekly Interval
        /// </summary>
        public class SMS_ST_RecurWeekly : SMS_ST_NonRecurring
        {
            //public SMS_ST_RecurWeekly() { }

            /// <summary>
            /// Day of the Week
            /// </summary>
            public int Day { get; set; }

            /// <summary>
            /// interval in weeks
            /// </summary>
            public int ForNumberOfWeeks { get; set; }

            /// <summary>
            /// Get the next start time
            /// </summary>
            public new DateTime NextStartTime
            {
                get
                {
                    //determine the new start date-time
                    DateTime oNextStartTime = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, StartTime.Hour, StartTime.Minute, 0);

                    while((int)oNextStartTime.DayOfWeek + 1 != Day)
                    {
                        oNextStartTime = oNextStartTime + new TimeSpan(1, 0, 0, 0);
                    }

                    return oNextStartTime;
                }
            }

            /// <summary>
            /// The last Start Time in the past...
            /// </summary>
            public DateTime PreviousStartTime
            {
                get
                {
                    if (base.StartTime > DateTime.Now)
                        return base.StartTime;
                    else
                    {
                        //determine the new start date-time
                        DateTime oNextStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, StartTime.Hour, StartTime.Minute, 0);
                        while (((int)oNextStartTime.DayOfWeek + 1 != Day) | (oNextStartTime < DateTime.Now))
                        {
                            oNextStartTime = oNextStartTime + new TimeSpan(1, 0, 0, 0);
                            
                        }
                        DateTime oPrevStartTime = oNextStartTime.Subtract(new TimeSpan(ForNumberOfWeeks * 7, 0, 0, 0));
                        if(oPrevStartTime < base.StartTime)
                            return oNextStartTime;
                        else
                            return oPrevStartTime;
                    }
                }
            }
        }

        /// <summary>
        /// Monthly interval (by date)
        /// </summary>
        public class SMS_ST_RecurMonthlyByDate : SMS_ST_NonRecurring
        {
            //public SMS_ST_RecurMonthlyByDate() { }

            /// <summary>
            /// interval in months
            /// </summary>
            public int ForNumberOfMonths { get; set; }

            /// <summary>
            /// Day of the month
            /// </summary>
            public int MonthDay { get; set; }

            /// <summary>
            /// get next start time
            /// </summary>
            public new DateTime NextStartTime
            {
                get
                {
                    //determine the new start date-time
                    if (MonthDay == 0)
                    {
                        DateTime oNextStartTime = new DateTime(StartTime.Year, StartTime.Month, DateTime.DaysInMonth(StartTime.Year, StartTime.Month), StartTime.Hour, StartTime.Minute, 0);

                        //Last Day of Month...
                        while (oNextStartTime < StartTime)
                        {
                            oNextStartTime = oNextStartTime.AddMonths(ForNumberOfMonths);
                            oNextStartTime = new DateTime(oNextStartTime.Year, oNextStartTime.Month, DateTime.DaysInMonth(oNextStartTime.Year, oNextStartTime.Month), StartTime.Hour, StartTime.Minute, 0);
                        }

                        return oNextStartTime;
                    }
                    else
                    {
                        DateTime oNextStartTime = new DateTime(StartTime.Year, StartTime.Month, MonthDay, StartTime.Hour, StartTime.Minute, 0);

                        // Skip month if month has less than MonthDay days
                        while (oNextStartTime < StartTime || DateTime.DaysInMonth(oNextStartTime.Year, oNextStartTime.Month) < MonthDay)
                        {
                            oNextStartTime = oNextStartTime.AddMonths(ForNumberOfMonths);
                        }
                        return oNextStartTime;
                    }
                }
            }
        }

        /// <summary>
        /// Monthly interval (by weekday)
        /// </summary>
        public class SMS_ST_RecurMonthlyByWeekday : SMS_ST_NonRecurring
        {
            //public SMS_ST_RecurMonthlyByWeekday() { }

            /// <summary>
            /// Week day
            /// </summary>
            public int Day { get; set; }

            /// <summary>
            /// interval in months
            /// </summary>
            public int ForNumberOfMonths { get; set; }

            /// <summary>
            /// WeekOrder
            /// </summary>
            public int WeekOrder { get; set; }

            /// <summary>
            /// Get next start time
            /// </summary>
            public new DateTime NextStartTime
            {
                get
                {
                    //determine the new start date-time
                    DateTime oNextStartTime = new DateTime(StartTime.Year, StartTime.Month, 1, StartTime.Hour, StartTime.Minute, 0);
                    oNextStartTime = ScheduleDecoding.GetNthWeekdayOfMonth(StartTime, WeekOrder, ((DayOfWeek)((Day - 1) % 7)));

                    oNextStartTime = new DateTime(oNextStartTime.Year, oNextStartTime.Month, oNextStartTime.Day, StartTime.Hour, StartTime.Minute, 0);

                    while (oNextStartTime < StartTime)
                    {
                        oNextStartTime = oNextStartTime.AddMonths(ForNumberOfMonths);
                        oNextStartTime = ScheduleDecoding.GetNthWeekdayOfMonth(oNextStartTime, WeekOrder, ((DayOfWeek)((Day - 1) % 7)));
                        oNextStartTime = oNextStartTime.AddHours(StartTime.Hour);
                        oNextStartTime = oNextStartTime.AddMinutes(StartTime.Minute);
                    }

                    return oNextStartTime;
                }
            }
        }
    }
}
