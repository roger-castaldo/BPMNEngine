using BPMNEngine.Interfaces.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using StrToTime = BPMNEngine.DateString;

namespace UnitTest
{
    [TestClass]
    public class DateString
    {
        private class ReadonlyVariablesImplementation : IReadonlyVariables
        {
            private readonly Dictionary<string, object> data;

            public ReadonlyVariablesImplementation(Dictionary<string, object> data)
            {
                this.data=data;
            }

            public object this[string name] => data[name];

            public IEnumerable<string> Keys => data.Keys;

            public IEnumerable<string> FullKeys => data.Keys;

            public Exception Error => null;
        }

        private const long MAX_TICK_DELTA = 1000_000;
        private static bool DatesAreEqual(DateTime d1,DateTime d2)
        {
            System.Diagnostics.Trace.WriteLine(Math.Abs(d1.Ticks-d2.Ticks));
            return Math.Max(Math.Abs(d1.Ticks-d2.Ticks), MAX_TICK_DELTA)==MAX_TICK_DELTA;
        }

        [TestMethod]
        public void TestDateStrings()
        {
            Assert.IsTrue(DatesAreEqual(DateTime.Now, new StrToTime("now").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Today, new StrToTime("today").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(1), new StrToTime("tomorrow").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-1), new StrToTime("yesterday").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddYears(1), new StrToTime("next year").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddYears(-1), new StrToTime("last year").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddMonths(1), new StrToTime("next month").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddMonths(-1), new StrToTime("last month").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(7), new StrToTime("next week").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-7), new StrToTime("last week").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(1), new StrToTime("next day").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-1), new StrToTime("last day").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddHours(1), new StrToTime("next hour").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddHours(-1), new StrToTime("last hour").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddMinutes(1), new StrToTime("next minute").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddMinutes(-1), new StrToTime("last minute").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddSeconds(1), new StrToTime("next second").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddSeconds(-1), new StrToTime("last second").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(2), new StrToTime("2 days").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-2), new StrToTime("-2 days").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(1), new StrToTime("1 day").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-1), new StrToTime("-1day").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-5), new StrToTime("5 days ago").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-6), new StrToTime("6days ago").GetTime(null)));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(1), new StrToTime("1day").GetTime(null)));

            Assert.IsTrue(DatesAreEqual(DateTime.Today, new StrToTime(DateTime.Today.ToString()).GetTime(null)));


            Assert.IsTrue(DatesAreEqual(DateTime.Today.AddDays(5), new StrToTime("today ${five} ${days}").GetTime(new ReadonlyVariablesImplementation(new()
            {
                {"five",5 },
                {"days","days" }
            }))));

            var current = "not valid";
            var exception = Assert.ThrowsException<FormatException>(() =>
            {
                new StrToTime(current).GetTime(null);
            });
            Assert.AreEqual($"Invalid Date String Specified [{current}]", exception.Message);
        }
    }
}
