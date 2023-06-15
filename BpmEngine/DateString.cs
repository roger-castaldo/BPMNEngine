using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BPMNEngine
{
    internal class DateString
    {
        private const string ValidUnits = "year|month|week|day|hour|minute|second";
        private static readonly Regex _basicRelativeRegex = new Regex(@"^(last|next) +(" + ValidUnits + ")$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _simpleRelativeRegex = new Regex(@"^([+-]?\d+) *(" + ValidUnits + ")s?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _completeRelativeRegex = new Regex(@"^(\d+) *(" + ValidUnits + ")s?( +ago)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _variableRegex = new Regex("^[+-]?\\$\\{([^}]+)\\}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string _value;

        public DateString(string value)
        {
            _value = value.Trim() + " ";
        }

        public DateTime GetTime(IReadonlyVariables variables)
        {
            DateTime ret = DateTime.Now;
            bool isFirst = true;
            string buffer = "";
            Match m;
            _value.ForEach(c =>
            {
                switch (c)
                {
                    case ' ':
                        if (_variableRegex.IsMatch(buffer))
                        {
                            m = _variableRegex.Match(buffer);
                            buffer = buffer.Replace("${" + m.Groups[1].Value + "}", string.Format("{0}", variables[m.Groups[1].Value]));
                        }
                        if (isFirst)
                        {
                            switch (buffer.ToLower())
                            {
                                case "now":
                                case "today":
                                    buffer = "";
                                    ret = DateTime.Now;
                                    isFirst = false;
                                    break;
                                case "tomorrow":
                                    buffer = "";
                                    ret = DateTime.Now.AddDays(1);
                                    isFirst = false;
                                    break;
                                case "yesterday":
                                    buffer = "";
                                    ret = DateTime.Now.AddDays(-1);
                                    isFirst = false;
                                    break;
                            }
                        }
                        if (buffer != "")
                        {
                            if (_basicRelativeRegex.IsMatch(buffer))
                            {
                                m = _basicRelativeRegex.Match(buffer);
                                ret = _AddOffset(variables, m.Groups[2].Value, (m.Groups[1].Value.ToLower() == "next" ? 1 : -1), ret);
                                buffer = "";
                            }
                            else if (_simpleRelativeRegex.IsMatch(buffer))
                            {
                                m = _simpleRelativeRegex.Match(buffer);
                                ret = _AddOffset(variables, m.Groups[2].Value, int.Parse(m.Groups[1].Value), ret);
                                buffer = "";
                            }
                            else if (_completeRelativeRegex.IsMatch(buffer))
                            {
                                m = _completeRelativeRegex.Match(buffer);
                                ret = _AddOffset(variables, m.Groups[2].Value, int.Parse(m.Groups[1].Value) * (m.Groups[3].Value != "" ? -1 : 1), ret);
                                buffer = "";
                            }
                            else if (isFirst)
                            {
                                DateTime tmp;
                                if (DateTime.TryParse(buffer, out tmp))
                                {
                                    isFirst = false;
                                    buffer = "";
                                    ret = tmp;
                                }
                                else
                                    buffer += c;
                            }
                            else
                                buffer += c;
                        }
                        break;
                    default:
                        buffer += c;
                        break;
                }
            });
            if (buffer != "")
            {
                throw new Exception(string.Format("Invalid Date String Specified [{0}]", buffer));
            }
            return ret;
        }

        private DateTime _AddOffset(IReadonlyVariables variables,string unit, int value, DateTime dateTime)
        {
            switch (unit.ToLower())
            {
                case "year":
                case "years":
                    return dateTime.AddYears(value);
                case "month":
                case "months":
                    return dateTime.AddMonths(value);
                case "week":
                case "weeks":
                    return dateTime.AddDays(value * 7);
                case "day":
                case "days":
                    return dateTime.AddDays(value);
                case "hour":
                case "hours":
                    return dateTime.AddHours(value);
                case "minute":
                case "minutes":
                    return dateTime.AddMinutes(value);
                case "second":
                case "seconds":
                    return dateTime.AddSeconds(value);
                default:
                    throw new Exception("Internal error: Unhandled relative date/time case.");
            }
        }
    }
}
