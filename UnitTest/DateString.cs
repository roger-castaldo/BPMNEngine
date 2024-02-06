using BPMNEngine.Interfaces.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using StrToTime = BPMNEngine.DateString;

namespace UnitTest
{
    [TestClass]
    public class DateString
    {
        private class ReadonlyVariablesImplementation : IReadonlyVariables
        {
            private readonly IImmutableDictionary<string, object> data;

            public ReadonlyVariablesImplementation(Dictionary<string, object> data)
            {
                this.data=data.ToImmutableDictionary();
            }

            public object this[string name] => data[name];

            public ImmutableArray<string> Keys => data.Keys.ToImmutableArray();

            public ImmutableArray<string> FullKeys => data.Keys.ToImmutableArray();

            public Exception Error => null;
        }

        private const long MAX_TICK_DELTA = 1_000_000;
        private static bool DatesAreEqual(DateTime time,string timeToConvert,IReadonlyVariables? variables=null)
        {
            var start = DateTime.Now;
            var d2 = new StrToTime(timeToConvert).GetTime(variables);
            var conversionTime = DateTime.Now.Subtract(start);
            var delta = Math.Abs(time.Subtract(d2).Ticks)-conversionTime.Ticks;
            System.Diagnostics.Trace.WriteLine($"Conversion Time: {conversionTime}, Delta: {delta}");
            return Math.Max(delta, MAX_TICK_DELTA)==MAX_TICK_DELTA;
        }

        [TestMethod]
        public void TestDateStrings()
        {
            Assert.IsTrue(DatesAreEqual(DateTime.Now, "now",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Today, "today",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(1), "tomorrow",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-1), "yesterday",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddYears(1), "next year",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddYears(-1), "last year",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddMonths(1), "next month",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddMonths(-1), "last month",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(7), "next week",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-7), "last week",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(1), "next day",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-1), "last day",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddHours(1), "next hour",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddHours(-1), "last hour",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddMinutes(1), "next minute",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddMinutes(-1), "last minute",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddSeconds(1), "next second",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddSeconds(-1), "last second",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(2), "2 days",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-2), "-2 days",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(1), "1 day",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-1), "-1day",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-5), "5 days ago",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(-6), "6days ago",null));
            Assert.IsTrue(DatesAreEqual(DateTime.Now.AddDays(1), "1day",null));

            Assert.IsTrue(DatesAreEqual(DateTime.Today, DateTime.Today.ToString(),null));


            Assert.IsTrue(DatesAreEqual(DateTime.Today.AddDays(5), "today ${five} ${days}",new ReadonlyVariablesImplementation(new()
            {
                {"five",5 },
                {"days","days" }
            })));

            var current = "not valid";
            var exception = Assert.ThrowsException<FormatException>(() =>
            {
                new StrToTime(current).GetTime(null);
            });
            Assert.AreEqual($"Invalid Date String Specified [{current}]", exception.Message);
        }
    }
}
