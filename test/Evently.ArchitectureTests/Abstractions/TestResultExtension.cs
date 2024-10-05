using FluentAssertions;
using NetArchTest.Rules;

namespace Evently.ArchitectureTests.Abstractions;

internal static class TestResultExtension
{
    internal static void ShouldBeSuccessful(this TestResult testResult)
    {
        testResult.FailingTypes?.Should().BeEmpty();
    }
}
