// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging.Testing;
using Moq;
using Xunit;

namespace Microsoft.Extensions.Logging.Test
{
    public class LoggerFactoryExtensionsTest
    {
        [Fact]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/34091", TestRuntimes.Mono)]
        public void LoggerFactoryCreateOfT_CallsCreateWithCorrectName()
        {
            // Arrange
            var expected = typeof(TestType).FullName;

            var factory = new Mock<ILoggerFactory>();
            factory.Setup(f => f.CreateLogger(
                It.IsAny<string>()))
            .Returns(new Mock<ILogger>().Object);

            // Act
            factory.Object.CreateLogger<TestType>();

            // Assert
            factory.Verify(f => f.CreateLogger(expected));
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/34091", TestRuntimes.Mono)]
        public void LoggerFactoryCreateOfT_SingleGeneric_CallsCreateWithCorrectName()
        {
            // Arrange
            var factory = new Mock<ILoggerFactory>();
            factory.Setup(f => f.CreateLogger(It.Is<string>(
                x => x.Equals("Microsoft.Extensions.Logging.Test.GenericClass<Microsoft.Extensions.Logging.Test.TestType>"))))
            .Returns(new Mock<ILogger>().Object);

            var logger = factory.Object.CreateLogger<GenericClass<TestType>>();

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/34091", TestRuntimes.Mono)]
        public void LoggerFactoryCreateOfT_TwoGenerics_CallsCreateWithCorrectName()
        {
            // Arrange
            var factory = new Mock<ILoggerFactory>();
            factory.Setup(f => f.CreateLogger(It.Is<string>(
                x => x.Equals("Microsoft.Extensions.Logging.Test.GenericClass<Microsoft.Extensions.Logging.Test.TestType, Microsoft.Extensions.Logging.Test.SecondTestType>"))))
            .Returns(new Mock<ILogger>().Object);

            var logger = factory.Object.CreateLogger<GenericClass<TestType, SecondTestType>>();

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        public void CreatesLoggerName_WithoutGenericTypeArgumentsInformation()
        {
            // Arrange
            var fullName = typeof(GenericClass<string>).GetGenericTypeDefinition().FullName;
            var fullNameWithoutBacktick = fullName.Substring(0, fullName.IndexOf('`'));
            var testSink = new TestSink();
            var factory = new TestLoggerFactory(testSink, enabled: true);

            // Act
            var logger = factory.CreateLogger<GenericClass<string>>();
            logger.LogInformation("test message");

            // Assert
            var sinkWrite = Assert.Single(testSink.Writes);
            Assert.Equal(fullNameWithoutBacktick, sinkWrite.LoggerName);
        }

        [Fact]
        public void CreatesLoggerName_OnNestedGenericType_CreatesWithoutGenericTypeArgumentsInformation()
        {
            // Arrange
            var fullName = typeof(GenericClass<GenericClass<string>>).GetGenericTypeDefinition().FullName;
            var fullNameWithoutBacktick = fullName.Substring(0, fullName.IndexOf('`'));
            var testSink = new TestSink();
            var factory = new TestLoggerFactory(testSink, enabled: true);

            // Act
            var logger = factory.CreateLogger<GenericClass<GenericClass<string>>>();
            logger.LogInformation("test message");

            // Assert
            var sinkWrite = Assert.Single(testSink.Writes);
            Assert.Equal(fullNameWithoutBacktick, sinkWrite.LoggerName);
        }

        [Fact]
        public void CreatesLoggerName_OnMultipleTypeArgumentGenericType_CreatesWithoutGenericTypeArgumentsInformation()
        {
            // Arrange
            var fullName = typeof(GenericClass<string, string>).GetGenericTypeDefinition().FullName;
            var fullNameWithoutBacktick = fullName.Substring(0, fullName.IndexOf('`'));
            var testSink = new TestSink();
            var factory = new TestLoggerFactory(testSink, enabled: true);

            // Act
            var logger = factory.CreateLogger<GenericClass<string, string>>();
            logger.LogInformation("test message");

            // Assert
            var sinkWrite = Assert.Single(testSink.Writes);
            Assert.Equal(fullNameWithoutBacktick, sinkWrite.LoggerName);
        }


        [Fact]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/34091", TestRuntimes.Mono)]
        public void LoggerFactoryCreate_CallsCreateWithCorrectName()
        {
            // Arrange
            var expected = typeof(TestType).FullName;

            var factory = new Mock<ILoggerFactory>();
            factory.Setup(f => f.CreateLogger(
                It.IsAny<string>()))
            .Returns(new Mock<ILogger>().Object);

            // Act
            factory.Object.CreateLogger(typeof(TestType));

            // Assert
            factory.Verify(f => f.CreateLogger(expected));
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/34091", TestRuntimes.Mono)]
        public void LoggerFactoryCreate_SingleGeneric_CallsCreateWithCorrectName()
        {
            // Arrange
            var factory = new Mock<ILoggerFactory>();
            factory.Setup(f => f.CreateLogger(It.Is<string>(
                x => x.Equals("Microsoft.Extensions.Logging.Test.GenericClass"))))
            .Returns(new Mock<ILogger>().Object);

            var logger = factory.Object.CreateLogger(typeof(GenericClass<TestType>));

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/34091", TestRuntimes.Mono)]
        public void LoggerFactoryCreate_TwoGenerics_CallsCreateWithCorrectName()
        {
            // Arrange
            var factory = new Mock<ILoggerFactory>();
            factory.Setup(f => f.CreateLogger(It.Is<string>(
                x => x.Equals("Microsoft.Extensions.Logging.Test.GenericClass"))))
            .Returns(new Mock<ILogger>().Object);

            var logger = factory.Object.CreateLogger(typeof(GenericClass<TestType, SecondTestType>));

            // Assert
            Assert.NotNull(logger);
        }


        /// <summary>
        /// Checks that the <see cref="LoggerFactory.CreateLogger(string)"/> method
        /// returns the same instance when called multiple times with a simple type full name as argument.
        /// </summary>
        [Fact]
        public void LoggerFactoryCreate_CategoryName_SimpleType_ReturnsSameInstance()
        {
            // Arrange
            var factory = new LoggerFactory();
            var logger1 = factory.CreateLogger(typeof(TestType).FullName);
            var logger2 = factory.CreateLogger(typeof(TestType).FullName);

            // Assert
            Assert.Equal(logger1, logger2);
        }

        /// <summary>
        /// Checks that the <see cref="LoggerFactoryExtensions.CreateLogger(ILoggerFactory, System.Type)"/> method
        /// returns the same instance when called multiple times with a simple type as argument.
        /// </summary>
        [Fact]
        public void LoggerFactoryCreate_Type_SimpleType_ReturnsSameInstance()
        {
            var factory = new LoggerFactory();
            var logger1 = factory.CreateLogger(typeof(TestType));
            var logger2 = factory.CreateLogger(typeof(TestType));

            Assert.Equal(logger1, logger2);
        }

        /// <summary>
        /// Checks that the <see cref="LoggerFactoryExtensions.CreateLogger{T}(ILoggerFactory)"/> method
        /// returns the same instance when called multiple times with a simple type as generic type argument.
        /// </summary>
        [Fact]
        public void LoggerFactoryCreateOfT_SimpleType_ReturnsSameInstance()
        {
            var factory = new LoggerFactory();
            var logger1 = factory.CreateLogger<TestType>();
            var logger2 = factory.CreateLogger<TestType>();

            Assert.IsType<Logger<TestType>>(logger1);
            Assert.IsType<Logger<TestType>>(logger2);
            Assert.Equal(logger1, logger2);
        }

        /// <summary>
        /// Checks that the <see cref="LoggerFactory.CreateLogger(string)"/> method
        /// returns the same instance when called multiple times with a single generic typed type full name as argument.
        /// </summary>
        [Fact]
        public void LoggerFactoryCreate_CategoryName_SingleGeneric_ReturnsSameInstance()
        {
            var factory = new LoggerFactory();
            var logger1 = factory.CreateLogger(typeof(GenericClass<TestType>).FullName);
            var logger2 = factory.CreateLogger(typeof(GenericClass<TestType>).FullName);

            Assert.Equal(logger1, logger2);
        }

        /// <summary>
        /// Checks that the <see cref="LoggerFactoryExtensions.CreateLogger(ILoggerFactory, System.Type)"/> method
        /// returns the same instance when called multiple times with a single generic typed type as argument.
        /// </summary>
        [Fact]
        public void LoggerFactoryCreate_Type_SingleGeneric_ReturnsSameInstance()
        {
            var factory = new LoggerFactory();
            var logger1 = factory.CreateLogger(typeof(GenericClass<TestType>));
            var logger2 = factory.CreateLogger(typeof(GenericClass<TestType>));

            Assert.Equal(logger1, logger2);
        }

        /// <summary>
        /// Checks that the <see cref="LoggerFactoryExtensions.CreateLogger{T}(ILoggerFactory)"/> method
        /// returns the same instance when called multiple times with a single generic typed type as generic type argument.
        /// </summary>
        [Fact]
        public void LoggerFactoryCreateOfT_SingleGeneric_ReturnsSameInstance()
        {
            var factory = new LoggerFactory();
            var logger1 = factory.CreateLogger<GenericClass<TestType>>();
            var logger2 = factory.CreateLogger<GenericClass<TestType>>();

            Assert.IsType<Logger<GenericClass<TestType>>>(logger1);
            Assert.IsType<Logger<GenericClass<TestType>>>(logger2);
            Assert.Equal(logger1, logger2);
        }

        /// <summary>
        /// Checks that the <see cref="LoggerFactory.CreateLogger(string)"/> method
        /// returns the same instance when called multiple times with a multiple generic typed type full name as argument.
        /// </summary>
        [Fact]
        public void LoggerFactoryCreate_CategoryName_DoubleGeneric_ReturnsSameInstance()
        {
            var factory = new LoggerFactory();
            var logger1 = factory.CreateLogger(typeof(GenericClass<TestType, SecondTestType>).FullName);
            var logger2 = factory.CreateLogger(typeof(GenericClass<TestType, SecondTestType>).FullName);

            Assert.Equal(logger1, logger2);
        }

        /// <summary>
        /// Checks that the <see cref="LoggerFactoryExtensions.CreateLogger(ILoggerFactory, System.Type)"/> method
        /// returns the same instance when called multiple times with a multiple generic typed type as argument.
        /// </summary>
        [Fact]
        public void LoggerFactoryCreate_Type_DoubleGeneric_ReturnsSameInstance()
        {
            var factory = new LoggerFactory();
            var logger1 = factory.CreateLogger(typeof(GenericClass<TestType, SecondTestType>));
            var logger2 = factory.CreateLogger(typeof(GenericClass<TestType, SecondTestType>));

            Assert.Equal(logger1, logger2);
        }

        /// <summary>
        /// Checks that the <see cref="LoggerFactoryExtensions.CreateLogger{T}(ILoggerFactory)"/> method
        /// returns the same instance when called multiple times with a multiple generic typed type as generic type argument.
        /// </summary>
        [Fact]
        public void LoggerFactoryCreateOfT_DoubleGeneric_ReturnsSameInstance()
        {
            var factory = new LoggerFactory();
            var logger1 = factory.CreateLogger<GenericClass<TestType, SecondTestType>>();
            var logger2 = factory.CreateLogger<GenericClass<TestType, SecondTestType>>();

            Assert.IsType<Logger<GenericClass<TestType, SecondTestType>>>(logger1);
            Assert.IsType<Logger<GenericClass<TestType, SecondTestType>>>(logger2);
            Assert.Equal(logger1, logger2);
        }

        /// <summary>
        /// Checks that the <see cref="LoggerFactory.CreateLogger"/> and <see cref="LoggerFactoryExtensions.CreateLogger"/> methods
        /// return the same instance when called multiple times with a simple type as argument.
        /// </summary>
        [Fact]
        public void LoggerFactoryCreate_SimpleType_ReturnsSameInstance()
        {
            // Arrange
            var factory = new LoggerFactory();
            var logger1 = factory.CreateLogger(typeof(TestType).FullName);
            var logger2 = factory.CreateLogger(typeof(TestType));

            // Assert
            Assert.Equal(logger1, logger2);
        }
    }

    internal class TestType
    {
        // intentionally holds nothing
    }

    internal class SecondTestType
    {
        // intentionally holds nothing
    }

    internal class GenericClass<X, Y> where X : class where Y : class
    {
        // intentionally holds nothing
    }

    internal class GenericClass<X> where X : class
    {
        // intentionally holds nothing
    }
}
