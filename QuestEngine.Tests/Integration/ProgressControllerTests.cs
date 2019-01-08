using System.Linq;
using System.Threading;
using QuestEngine.Client;
using System.Threading.Tasks;
using FluentAssertions;
using QuestEngine.Business.Helpers;
using Xunit;

namespace QuestEngine.Tests.Integration
{
    public class ProgressControllerTests: IntegrationTestsBase
    {
        [Fact]
        public async Task PostProgress_NoPreviousProgressNotEnoughtToCompleteMilestone_ShouldCalculateCorrectPoints()
        {
            var questEngineProgressClient = new ProgressClient(TestHttpClient);

            var result = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress1",
                    PlayerLevel = 2,
                    ChipAmountBet = 50
                });

            var pointsToBeEarned = (50 * GameConfiguration.RateFromBet) + (2 * GameConfiguration.LevelBonusRate);

            result.Should().NotBeNull();
            result.QuestPointsEarned.Should().Be(pointsToBeEarned);
            result.TotalQuestPercentCompleted.Should().Be(MathHelper.CalculatePercentage(pointsToBeEarned, GameConfiguration.TotalQuestPoints));
            result.MilestonesCompleted.Should().BeEmpty();
        }

        [Fact]
        public async Task PostProgress_NoPreviousProgressWithEnoughtToCompleteMilestone_ShouldCalculateCorrectPoints()
        {
            var questEngineProgressClient = new ProgressClient(TestHttpClient);

            var result = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress2",
                    PlayerLevel = 2,
                    ChipAmountBet = 260
                });

            var pointsToBeEarned = (260 * GameConfiguration.RateFromBet) + (2 * GameConfiguration.LevelBonusRate);

            result.Should().NotBeNull();
            result.QuestPointsEarned.Should().Be(pointsToBeEarned);
            result.TotalQuestPercentCompleted.Should().Be(MathHelper.CalculatePercentage(pointsToBeEarned, GameConfiguration.TotalQuestPoints));
            result.MilestonesCompleted.First().MilestoneIndex.Should().Be(MilestoneConfigurations.First().MilestoneId);
            result.MilestonesCompleted.First().ChipsAwarded.Should().Be(MilestoneConfigurations.First().ChipsAwarded);
        }

        [Fact]
        public async Task PostProgress_WithProgressWithNotEnoughtToCompleteMilestone_ShouldSaveAndCalculateCorrectPoints()
        {
            var questEngineProgressClient = new ProgressClient(TestHttpClient);

            // Including previous progress
            var previousResult = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress3",
                    PlayerLevel = 2,
                    ChipAmountBet = 260
                });

            var result = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress3",
                    PlayerLevel = 3,
                    ChipAmountBet = 50
                });

            var pointsToBeEarned = (50 * GameConfiguration.RateFromBet) + (3 * GameConfiguration.LevelBonusRate);

            result.Should().NotBeNull();
            result.QuestPointsEarned.Should().Be(pointsToBeEarned);
            result.TotalQuestPercentCompleted.Should().Be(MathHelper.CalculatePercentage(previousResult.QuestPointsEarned + pointsToBeEarned, GameConfiguration.TotalQuestPoints));
            result.MilestonesCompleted.Should().BeEmpty();
        }

        [Fact]
        public async Task PostProgress_WithProgressWithEnoughtToCompleteMilestone_ShouldSaveAndCalculateCorrectPoints()
        {
            Context.PlayerProgresses.Add(new QuestEngine.Data.Models.PlayerProgress
            {
                QuestPointsEarned = 1304,
                LastMilestoneCompletedId = 1,
                PlayerId = "playerProgress4",
                QuestId = 1
            });

            Context.SaveChanges();

            var questEngineProgressClient = new ProgressClient(TestHttpClient);

            var result = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress4",
                    PlayerLevel = 3,
                    ChipAmountBet = 250
                });

            var pointsToBeEarned = (250 * GameConfiguration.RateFromBet) + (3 * GameConfiguration.LevelBonusRate);

            result.Should().NotBeNull();
            result.QuestPointsEarned.Should().Be(pointsToBeEarned);
            result.TotalQuestPercentCompleted.Should().Be(MathHelper.CalculatePercentage(1304  + pointsToBeEarned, GameConfiguration.TotalQuestPoints));
            result.MilestonesCompleted.First().MilestoneIndex.Should().Be(MilestoneConfigurations.First(m => m.MilestoneId == 2).MilestoneId);
            result.MilestonesCompleted.First().ChipsAwarded.Should().Be(MilestoneConfigurations.First(m => m.MilestoneId == 2).ChipsAwarded);
        }

        [Theory]
        [InlineData(-1, 50)]
        [InlineData(1, -50)]
        [InlineData(-1, -50)]
        public async Task PostProgress_NoPreviousProgressSendingNegativeValues_ShouldReturnEmptyValues(int playerLevel, long chipAmountBet)
        {
            var questEngineProgressClient = new ProgressClient(TestHttpClient);

            var result = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress5",
                    PlayerLevel = playerLevel,
                    ChipAmountBet = chipAmountBet
                });


            result.Should().NotBeNull();
            result.MilestonesCompleted.Should().BeEmpty();
            result.QuestPointsEarned.Should().Be(0);
            result.TotalQuestPercentCompleted.Should().Be(0);
        }

        [Theory]
        [InlineData(-1, 50)]
        [InlineData(1, -50)]
        [InlineData(-1, -50)]
        public async Task PostProgress_WithProgressSendingNegativeValues_ShouldReturnEmptyPointsButWithPreviousProgress(int playerLevel, long chipAmountBet)
        {
            var questEngineProgressClient = new ProgressClient(TestHttpClient);

            // Including previous progress
            var previousResult = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress6",
                    PlayerLevel = 2,
                    ChipAmountBet = 260
                });

            var questPercentCompleted =
                MathHelper.CalculatePercentage(previousResult.QuestPointsEarned, GameConfiguration.TotalQuestPoints);

            var result = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress6",
                    PlayerLevel = playerLevel,
                    ChipAmountBet = chipAmountBet
                });


            result.Should().NotBeNull();
            result.MilestonesCompleted.Should().BeEmpty();
            result.QuestPointsEarned.Should().Be(0);
            result.TotalQuestPercentCompleted.Should().Be(questPercentCompleted);
        }

        [Fact]
        public async Task PostProgress_NoPreviousProgressBetMoreThanQuestPoints_ShouldReturnOnlyMaxQuestPoints()
        {
            var questEngineProgressClient = new ProgressClient(TestHttpClient);

            var result = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress7",
                    PlayerLevel = 2,
                    ChipAmountBet = 1500
                });

            result.Should().NotBeNull();
            result.QuestPointsEarned.Should().Be(GameConfiguration.TotalQuestPoints);
            result.TotalQuestPercentCompleted.Should().Be(100);
            result.MilestonesCompleted.Count.Should().Be(3);
            result.MilestonesCompleted.Should().BeEquivalentTo(
                MilestoneConfigurations.Select(m => new Client.Milestone
                    {
                        ChipsAwarded = m.ChipsAwarded,
                        MilestoneIndex = m.MilestoneId
                    }));
        }
        
        [Fact]
        public async Task PostProgress_WithProgressBetMoreThanQuestPoints_ShouldReturnOnlyNeededPointsToCompletQuest()
        {
            var questEngineProgressClient = new ProgressClient(TestHttpClient);

            // Including previous progress
            var previousResult = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress8",
                    PlayerLevel = 2,
                    ChipAmountBet = 510
                });

            var maxAllowedPoints = GameConfiguration.TotalQuestPoints - previousResult.QuestPointsEarned;

            var result = await questEngineProgressClient.PostProgressAsync(
                new ProgressInput
                {
                    PlayerId = "playerProgress8",
                    PlayerLevel = 2,
                    ChipAmountBet = 1500
                });

            result.Should().NotBeNull();
            result.QuestPointsEarned.Should().Be(maxAllowedPoints);
            result.TotalQuestPercentCompleted.Should().Be(100);
            result.MilestonesCompleted.Count.Should().Be(1);
            result.MilestonesCompleted.Should().BeEquivalentTo(
                MilestoneConfigurations
                    .Where(m => m.MilestoneId == 3)
                    .Select(m => new Client.Milestone
                    {
                        ChipsAwarded = m.ChipsAwarded,
                        MilestoneIndex = m.MilestoneId
                    }));
        }
    }
}
