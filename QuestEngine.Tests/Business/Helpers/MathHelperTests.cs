using System;
using FluentAssertions;
using QuestEngine.Business.Helpers;
using Xunit;

namespace QuestEngine.Tests.Business.Helpers
{
    public class MathHelperTests
    {
        [Fact]
        public void CalculatePercentage_ZeroOriginalValue_ShouldReturnZero()
        {
            var percent = MathHelper.CalculatePercentage(1, 0);

            percent.Should().Be(0);
        }

        [Theory]
        [InlineData(1, 100, 1)]
        [InlineData(1, 1000, 0.1)]
        [InlineData(12, 100000, 0.01)]
        [InlineData(12612, 100000, 12.61)]
        [InlineData(12612, 10000, 126.12)]
        public void CalculatePercentage_GivenValues_ShouldCalculatePercentWithTwoDecimalPrecision(int newValue,
            int originalValue, decimal correctPercent)
        {
            var percent = MathHelper.CalculatePercentage(newValue, originalValue);

            percent.Should().Be(correctPercent);
            CheckTwoDecimalPrecision(percent).Should().BeTrue();
        }

        private bool CheckTwoDecimalPrecision(decimal decimalNumber)
        {
            var value = decimalNumber * 100;
            return value == Math.Floor(value);
        }
    }
}
