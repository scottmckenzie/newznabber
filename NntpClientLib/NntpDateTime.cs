using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NntpClientLib
{
    // http://www.informit.com/guides/content.asp?g=dotnet&seqNum=172&rl=1
    public struct NntpDateTime : IEquatable<NntpDateTime>
    {
        private static readonly string[] MonthNames = 
        {
            "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec"
        };

        private readonly DateTime _dtime;
        private TimeSpan _utcOffset;

        public NntpDateTime(DateTime dt)
        {
            _dtime = dt;
            _utcOffset = TimeZoneInfo.Local.GetUtcOffset(dt);
        }

        public NntpDateTime(DateTime dt, TimeSpan utcOffset)
        {
            _dtime = dt;
            _utcOffset = utcOffset;
        }

        public DateTime UtcTime
        {
            get { return _dtime; }
        }

        public DateTime DateTime
        {
            get { return _dtime + _utcOffset; }
        }

        public TimeSpan UtcOffset
        {
            get { return _utcOffset; }
        }

        public bool Equals(NntpDateTime other)
        {
            return _dtime == other._dtime && _utcOffset == other._utcOffset;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NntpDateTime))
            {
                return false;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(NntpDateTime d1, NntpDateTime d2)
        {
            return d1.Equals(d2);
        }

        public static bool operator !=(NntpDateTime d1, NntpDateTime d2)
        {
            return !d1.Equals(d2);
        }

        public static NntpDateTime ParseBasic(string date)
        {
            if (date == null)
            {
                throw new ArgumentNullException("date");
            }

            var i = date.IndexOf(',') + 1;
            if (i == 0)
            {
                throw new ArgumentException();
            }
            var j = date.LastIndexOf(':') + 3;
            if (j == 2)
            {
                throw new ArgumentException();
            }

            return new NntpDateTime(DateTime.Parse(date.Substring(i, j - i)), TimeSpan.Zero);
        }

        public static NntpDateTime Parse(string date)
        {
            if (date == null)
            {
                throw new ArgumentNullException("date");
            }

            const string parseDateExpression = @"
                (?<rfc822>
                    ^((mon|tue|wed|thu|fri|sat|sun),\s*)?
                    (?<day>\d{2}?)\s+
                    (?<month>jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)\s+
                    (?<year>(\d{2,4}))\s+
                    (?<hour>\d{2}):(?<min>\d{2})(:(?<sec>\d{2}))?\s+
                    (?<ofs>([+\-]?\d{4})|ut|gmt|est|edt|cst|cdt|mst|mdt|pst|pdt)(\s\(utc\))?$
                )
            ";

            var regexDate = new Regex(parseDateExpression, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            var m = regexDate.Match(date);
            if (!m.Success)
            {
                throw new FormatException(Resource.ErrorMessage47);
            }
            try
            {
                var year = ParseInt32(m.Groups["year"].Value);
                if (year < 1000)
                {
                    year = (year < 50) ? year + 2000 : year + 1999;
                }

                var month = Array.IndexOf(MonthNames, m.Groups["month"].Value.ToLowerInvariant()) + 1;

                var day = m.Groups["day"].Success ? ParseInt32(m.Groups["day"].Value) : 1;
                var hour = m.Groups["hour"].Success ? ParseInt32(m.Groups["hour"].Value) : 0;
                var minutes = m.Groups["min"].Success ? ParseInt32(m.Groups["min"].Value) : 0;
                var seconds = m.Groups["sec"].Success ? ParseInt32(m.Groups["sec"].Value) : 0;
                var milliSeconds = m.Groups["ms"].Success ? (int) Math.Round((1000*double.Parse(m.Groups["ms"].Value, Rfc977NntpClient.FormatProvider))) : 0;

                var ofs = TimeSpan.Zero;
                if (m.Groups["ofs"].Success)
                {
                    ofs = ParseOffset(m.Groups["ofs"].Value);
                }
                return new NntpDateTime(new DateTime(year, month, day, hour, minutes, seconds, milliSeconds) - ofs, ofs);
            }
            catch (Exception ex)
            {
                throw new FormatException(Resource.ErrorMessage47, ex);
            }
        }

        private static int ParseInt32(string value)
        {
            return int.Parse(value, Rfc977NntpClient.FormatProvider);
        }

        private static TimeSpan ParseOffset(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return TimeSpan.Zero;
            }
            var hours = 0;
            switch (s.ToLowerInvariant())
            {
                case "ut":
                case "utc":
                case "gmt":
                    break;
                case "edt":
                    hours = -4;
                    break;
                case "est":
                case "cdt":
                    hours = -5;
                    break;
                case "cst":
                case "mdt":
                    hours = -6;
                    break;
                case "mst":
                case "pdt":
                    hours = -7;
                    break;
                case "pst":
                    hours = -8;
                    break;
                default:
                    if (s[0] == '+')
                    {
                        var sfmt = s.Substring(1, 2) + ":" + s.Substring(3, 2);
                        return TimeSpan.Parse(sfmt);
                    }
                    return TimeSpan.Parse(s.Insert(s.Length - 2, ":"));
            }
            return TimeSpan.FromHours(hours);
        }

        public override string ToString()
        {
            return ToString("R");
        }

        public string ToString(string format)
        {
            if (format == null || format == "R")
            {
                var f = (_dtime + _utcOffset).ToString("ddd, dd MMM yyyy HH:mm:ss ", Rfc977NntpClient.FormatProvider);
                if (_utcOffset >= TimeSpan.Zero)
                {
                    f += "+";
                }
                f += _utcOffset.Hours.ToString("00", Rfc977NntpClient.FormatProvider) + _utcOffset.Minutes.ToString("00", Rfc977NntpClient.FormatProvider);
                f += " (UTC)";
                return f;
            }
            throw new ArgumentException(Resource.ErrorMessage46);
        }
    }
}