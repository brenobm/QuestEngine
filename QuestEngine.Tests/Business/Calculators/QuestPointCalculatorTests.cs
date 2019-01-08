using FluentAssertions;
using QuestEngine.Business.Calculators;
using QuestEngine.Configuration;
using Xunit;

namespace QuestEngine.Tests.Business.Calculators
{
    public class QuestPointCalculatorTests
    {
        private static GameConfiguration _gameConfiguration;

        public QuestPointCalculatorTests()
        {
            _gameConfiguration = new GameConfiguration
            {
                RateFromBet = 5,
                LevelBonusRate = 2
            };
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(0, 1, 2)]
        [InlineData(1, 0, 5)]
        [InlineData(1, 1, 7)]
        [InlineData(2, 1, 12)]
        [InlineData(1, 2, 9)]
        public void CalculateQuestPoints_PassingCorrectData_ShouldCalcultePointsEarned(int chipsAmountBet, int playerLevel, int questPoints)
        {
            var questPointCalculator = new QuestPointCalculator(_gameConfiguration);

            var questPointsEarned = questPointCalculator.CalculateQuestPoints(chipsAmountBet, playerLevel);

            questPointsEarned.Should().Be(questPoints);
        }

        [Fact]
        public void CalculateQuestPoints_NoConfigurationSet_ShouldReturnZero()
        {
            var questPointCalculator = new QuestPointCalculator(new GameConfiguration());

            var questPointsEarned = questPointCalculator.CalculateQuestPoints(1, 1);

            questPointsEarned.Should().Be(0);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(-1, -1)]
        public void CalculateQuestPoints_PassingAnyNegativeValue_ShouldReturnZero(int chipsAmountBet, int playerLevel)
        {
            var questPointCalculator = new QuestPointCalculator(_gameConfiguration);

            var questPointsEarned = questPointCalculator.CalculateQuestPoints(chipsAmountBet, playerLevel);

            questPointsEarned.Should().Be(0);
        }
    }
}
