using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ant0nRocket.Lib.Extensions;

using NUnit.Framework;

namespace Ant0nRocket.Lib.Tests.Extensions
{
    [TestFixture]
    public class ExceptionExtensionsTests
    {
        [Test]
        public void GetFullExceptionErrorMessage_NullException_ReturnsEmptyString()
        {
            // Arrange
            Exception? nullException = null;

            // Act
            var result = nullException.GetFullExceptionErrorMessage();

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetFullExceptionErrorMessage_SingleException_ReturnsSingleMessage()
        {
            // Arrange
            var exception = new InvalidOperationException("Test error message");

            // Act
            var result = exception.GetFullExceptionErrorMessage();

            // Assert
            Assert.That(result, Is.EqualTo("Test error message"));
        }

        [Test]
        public void GetFullExceptionErrorMessage_ExceptionWithInner_ReturnsChainedMessages()
        {
            // Arrange
            var innerException = new ArgumentNullException("param", "Parameter cannot be null");
            var outerException = new InvalidOperationException("Operation failed", innerException);

            // Act
            var result = outerException.GetFullExceptionErrorMessage();

            // Assert
            Assert.That(result, Is.EqualTo("Operation failed -> Parameter cannot be null (Parameter 'param')"));
        }

        [Test]
        public void GetFullExceptionErrorMessage_ThreeLevelExceptionChain_ReturnsAllMessages()
        {
            // Arrange
            var innerMost = new DivideByZeroException("Division by zero");
            var inner = new ArgumentException("Invalid argument", innerMost);
            var outer = new ApplicationException("Application error", inner);

            // Act
            var result = outer.GetFullExceptionErrorMessage();

            // Assert
            Assert.That(result, Is.EqualTo("Application error -> Invalid argument -> Division by zero"));
        }

        [Test]
        public void GetFullExceptionErrorMessage_CustomSeparator_UsesCustomSeparator()
        {
            // Arrange
            var innerException = new Exception("Inner error");
            var outerException = new Exception("Outer error", innerException);

            // Act
            var result = outerException.GetFullExceptionErrorMessage(" | ");

            // Assert
            Assert.That(result, Is.EqualTo("Outer error | Inner error"));
        }

        [Test]
        public void GetFullExceptionErrorMessage_IncludeExceptionTypeTrue_IncludesExceptionTypes()
        {
            // Arrange
            var innerException = new ArgumentNullException("param");
            var outerException = new InvalidOperationException("Operation failed", innerException);

            // Act
            var result = outerException.GetFullExceptionErrorMessage(includeExceptionType: true);

            // Assert
            Assert.That(result, Is.EqualTo("[InvalidOperationException] Operation failed -> [ArgumentNullException] Value cannot be null. (Parameter 'param')"));
        }

        [Test]
        public void GetFullExceptionErrorMessage_IncludeExceptionTypeFalse_ExcludesExceptionTypes()
        {
            // Arrange
            var innerException = new ArgumentNullException("param");
            var outerException = new InvalidOperationException("Operation failed", innerException);

            // Act
            var result = outerException.GetFullExceptionErrorMessage(includeExceptionType: false);

            // Assert
            Assert.That(result, Is.EqualTo("Operation failed -> Value cannot be null. (Parameter 'param')"));
        }

        [Test]
        public void GetFullExceptionErrorMessage_CustomSeparatorAndType_UsesBothParameters()
        {
            // Arrange
            var innerException = new DivideByZeroException("Math error");
            var outerException = new ApplicationException("App error", innerException);

            // Act
            var result = outerException.GetFullExceptionErrorMessage(" ==> ", true);

            // Assert
            Assert.That(result, Is.EqualTo("[ApplicationException] App error ==> [DivideByZeroException] Math error"));
        }

        [Test]
        public void GetFullExceptionErrorMessage_ExceptionWithEmptyMessage_HandlesEmptyMessages()
        {
            // Arrange
            var innerException = new Exception("");
            var outerException = new Exception("Outer message", innerException);

            // Act
            var result = outerException.GetFullExceptionErrorMessage();

            // Assert
            Assert.That(result, Is.EqualTo("Outer message -> "));
        }

        [Test]
        public void GetFullExceptionErrorMessage_AggregateException_HandlesAggregateException()
        {
            // Arrange
            var inner1 = new Exception("First inner");
            var inner2 = new Exception("Second inner");
            var aggregateException = new AggregateException("Aggregate error", inner1, inner2);

            // Act
            var result = aggregateException.GetFullExceptionErrorMessage();

            // Assert
            Assert.That(result, Is.EqualTo("Aggregate error (First inner) (Second inner) -> First inner"));
        }
    }
}
