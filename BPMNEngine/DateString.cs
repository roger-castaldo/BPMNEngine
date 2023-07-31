using BPMNEngine.Interfaces.Variables;
using System.Text.RegularExpressions;

namespace BPMNEngine
{
    internal class DateString
    {
        private const string ValidUnits = "year|month|week|day|hour|minute|second";
        private static readonly Regex basicRelativeRegex = new(@"^(last|next) +(" + ValidUnits + ")$", RegexOptions.Compiled | RegexOptions.IgnoreCase,TimeSpan.FromMinutes(500));
        private static readonly Regex simpleRelativeRegex = new(@"^([+-]?\d+) *(" + ValidUnits + ")s?$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMinutes(500));
        private static readonly Regex completeRelativeRegex = new(@"^(\d+) *(" + ValidUnits + ")s?( +ago)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMinutes(500));
        private static readonly Regex variableRegex = new("^[+-]?\\$\\{([^}]+)\\}$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMinutes(500));

        private readonly string value;

        public DateString(string value)
        {
            this.value = value.Trim() + " ";
        }

        public DateTime GetTime(IReadonlyVariables variables)
        {
            DateTime ret = DateTime.Now;
            bool isFirst = true;
            string buffer = "";
            Match m;
            value.ForEach(c =>
            {
                switch (c)
                {
                    case ' ':
                        if (variableRegex.IsMatch(buffer))
                        {
                            m = variableRegex.Match(buffer);
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
                            if (basicRelativeRegex.IsMatch(buffer))
                            {
                                m = basicRelativeRegex.Match(buffer);
                                ret = DateString.AddOffset(m.Groups[2].Value, (m.Groups[1].Value.ToLower() == "next" ? 1 : -1), ret);
                                buffer = "";
                            }
                            else if (simpleRelativeRegex.IsMatch(buffer))
                            {
                                m = simpleRelativeRegex.Match(buffer);
                                ret = DateString.AddOffset(m.Groups[2].Value, int.Parse(m.Groups[1].Value), ret);
                                buffer = "";
                            }
                            else if (completeRelativeRegex.IsMatch(buffer))
                            {
                                m = completeRelativeRegex.Match(buffer);
                                ret = DateString.AddOffset(m.Groups[2].Value, int.Parse(m.Groups[1].Value) * (m.Groups[3].Value != "" ? -1 : 1), ret);
                                buffer = "";
                            }
                            else if (isFirst)
                            {
                                if (DateTime.TryParse(buffer, out DateTime tmp))
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

        private static DateTime AddOffset(string unit, int value, DateTime dateTime)
        {
            return unit.ToLower() switch
            {
                "year"or"years" => dateTime.AddYears(value),
                "month"or"months" => dateTime.AddMonths(value),
                "week"or"weeks" => dateTime.AddDays(value * 7),
                "day"or"days" => dateTime.AddDays(value),
                "hour"or"hours" => dateTime.AddHours(value),
                "minute"or"minutes" => dateTime.AddMinutes(value),
                "second"or"seconds" => dateTime.AddSeconds(value),
                _ => throw new Exception("Internal error: Unhandled relative date/time case."),
            };
        }
    }
}
