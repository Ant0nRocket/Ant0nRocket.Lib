using Ant0nRocket.Lib.Extensions;
using NUnit.Framework;
using System;

namespace Ant0nRocket.Lib.Tests.Extensions
{
    [TestFixture]
    public class DateTimeExtensionTests
    {
        [Test]
        public void StartOfTheDay_WithTimePart_ReturnsStartOfDay()
        {
            var originalDate = new DateTime(2021, 1, 20, 12, 45, 30, 500);
            var expected = new DateTime(2021, 1, 20, 0, 0, 0, 0);

            var result = originalDate.StartOfTheDay();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void StartOfTheDay_WithDateTimeKind_ReturnsWithSpecifiedKind()
        {
            var originalDate = new DateTime(2021, 1, 20, 12, 45, 30);

            var resultUtc = originalDate.StartOfTheDay(DateTimeKind.Utc);
            var resultLocal = originalDate.StartOfTheDay(DateTimeKind.Local);
            var resultUnspecified = originalDate.StartOfTheDay(DateTimeKind.Unspecified);

            Assert.That(resultUtc.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(resultLocal.Kind, Is.EqualTo(DateTimeKind.Local));
            Assert.That(resultUnspecified.Kind, Is.EqualTo(DateTimeKind.Unspecified));
        }

        [Test]
        public void StartOfTheDay_AlreadyStartOfDay_ReturnsSameValue()
        {
            var originalDate = new DateTime(2021, 1, 20, 0, 0, 0, 0);

            var result = originalDate.StartOfTheDay();

            Assert.That(result, Is.EqualTo(originalDate));
        }

        [Test]
        public void EndOfTheDay_WithTimePart_ReturnsEndOfDay()
        {
            var originalDate = new DateTime(2021, 1, 20, 12, 45, 30, 500);
            var expected = new DateTime(2021, 1, 20, 23, 59, 59, 999);

            var result = originalDate.EndOfTheDay();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void EndOfTheDay_WithDateTimeKind_ReturnsWithSpecifiedKind()
        {
            var originalDate = new DateTime(2021, 1, 20, 12, 45, 30);

            var resultUtc = originalDate.EndOfTheDay(DateTimeKind.Utc);
            var resultLocal = originalDate.EndOfTheDay(DateTimeKind.Local);
            var resultUnspecified = originalDate.EndOfTheDay(DateTimeKind.Unspecified);

            Assert.That(resultUtc.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(resultLocal.Kind, Is.EqualTo(DateTimeKind.Local));
            Assert.That(resultUnspecified.Kind, Is.EqualTo(DateTimeKind.Unspecified));
        }

        [Test]
        public void EndOfTheDay_AlreadyEndOfDay_ReturnsSameValue()
        {
            var originalDate = new DateTime(2021, 1, 20, 23, 59, 59, 999);

            var result = originalDate.EndOfTheDay();

            Assert.That(result, Is.EqualTo(originalDate));
        }

        [Test]
        public void StartOfTheDay_EndOfTheDay_Combined_CoverFullDay()
        {
            var originalDate = new DateTime(2021, 1, 20, 12, 45, 30, 500);

            var start = originalDate.StartOfTheDay();
            var end = originalDate.EndOfTheDay();

            Assert.That(start, Is.LessThan(end));
            Assert.That(end - start, Is.EqualTo(TimeSpan.FromDays(1) - TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void StartOfTheDay_LeapDay_HandlesCorrectly()
        {
            var leapDate = new DateTime(2020, 2, 29, 15, 30, 45);
            var expected = new DateTime(2020, 2, 29, 0, 0, 0, 0);

            var result = leapDate.StartOfTheDay();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void EndOfTheDay_LeapDay_HandlesCorrectly()
        {
            var leapDate = new DateTime(2020, 2, 29, 15, 30, 45);
            var expected = new DateTime(2020, 2, 29, 23, 59, 59, 999);

            var result = leapDate.EndOfTheDay();

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}