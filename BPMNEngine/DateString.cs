using BPMNEngine.Interfaces.Variables;
using System;
using System.Text.RegularExpressions;

namespace BPMNEngine
{
    /// <summary>
    /// This class is used to convert a date string into a datetime value, uses the similar 
    /// concepts as the strtotime found in php
    /// </summary>
    public class DateString
    {
        private const string ValidFirstRegexString = "now|today|tomorrow|yesterday";
        private const string ValidUnits = "year|month|week|day|hour|minute|second";
        private const string VariableRegexString = @"\$\{([^}]+)\}";
        private const string UnitShiftRegexString = @"last +|next +|[+-]?\d+ *";
        private const string AgoRegexString = @" +ago";
        private static readonly Regex fullRegex = new($"(({ValidFirstRegexString})|(({UnitShiftRegexString}|{VariableRegexString} *)(({ValidUnits})s?|{VariableRegexString})({AgoRegexString})?))", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMinutes(5));
        private static readonly Regex firstRegex = new($"^({ValidFirstRegexString})$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMinutes(5));
        private static readonly Regex relativeRegex = new($"^({UnitShiftRegexString})({ValidUnits})s?({AgoRegexString})?$", RegexOptions.Compiled | RegexOptions.IgnoreCase,TimeSpan.FromMinutes(5));
        private static readonly Regex variableRegex = new(VariableRegexString, RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMinutes(5));

        private readonly string[] data;

        /// <summary>
        /// creates an instance
        /// </summary>
        /// <param name="value">the date string that is meant to be converted</param>
        public DateString(string value)
        {
            var idx = 0;
            var tmp = new List<string>();
            fullRegex.Matches(value)
                .ForEach(match =>
                {
                    if (idx<match.Index)
                        tmp.Add(value[idx..match.Index]);
                    tmp.Add(match.Value);
                    idx = match.Index + match.Length;
                });
            if (idx+1<value.Length)
                tmp.Add(value[idx..]);
            data=tmp
                .Select(v=>v.Trim())
                .Where(v=>!string.IsNullOrEmpty(v))
                .ToArray();
        }

        /// <summary>
        /// Converts the date string into an actual datetime object
        /// </summary>
        /// <param name="variables">The process variables currently avaialbe (this is used when variables exist in the string e.g. ${variablename})</param>
        /// <returns>A DateTime build from the string</returns>
        /// <exception cref="Exception">Occurs when the string is determined unparsable</exception>
        public DateTime GetTime(IReadonlyVariables variables)
        {
            DateTime ret = DateTime.Now;
            bool isFirst = true;
            data.ForEach(line =>
            {
                var current = variableRegex.Replace(line,m=> $"{variables[m.Groups[1].Value]}");
                if (isFirst && firstRegex.IsMatch(current))
                {
                    isFirst=false;
                    ret = line.ToLower() switch
                    {
                        "now" => DateTime.Now,
                        "today" => DateTime.Today,
                        "tomorrow" => DateTime.Now.AddDays(1),
                        "yesterday" => DateTime.Now.AddDays(-1),
                        _ => throw new NotImplementedException()
                    };
                }
                else if (relativeRegex.IsMatch(current))
                {
                    var m = relativeRegex.Match(current);
                    ret = AddOffset(m.Groups[2].Value, m.Groups[1].Value.Trim().ToLower() switch
                    {
                        "next" => 1,
                        "last" => -1,
                        _ => int.Parse(m.Groups[1].Value)
                    } * (m.Groups.Count==3||string.IsNullOrEmpty(m.Groups[3].Value) ? 1 : -1), ret);
                }
                else if (isFirst && DateTime.TryParse(current, out var tmp))
                    ret=tmp;
                else
                    throw new FormatException($"Invalid Date String Specified [{current}]");
            });
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
