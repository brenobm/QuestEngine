using FluentAssertions;
using QuestEngine.Business.Helpers;
using QuestEngine.Client;
using QuestEngine.Configuration;
using QuestEngine.Data.Models;
using System.Threading.Tasks;
using Xunit;

namespace QuestEngine.Tests.Integration
{
    public class StateControllerTests: IntegrationTestsBase
    {
        [Fact]
        public async Task GetStatePlayer_NoExistingData_ShouldReturnEmptyState()
        {
            var questEngineStateClient = new StateClient(TestHttpClient);

            var result = await questEngineStateClient.GetStatePlayerAsync("playerGetDataNoExisting");

            result.Should().NotBeNull();
            result.LastMilestoneIndexCompleted.Should().BeNull();
            result.TotalQuestPercentCompleted.Should().Be(0);
        }

        [Fact]
        public async Task GetStatePlayer_ExistingData_ShouldReturnCorrectState()
        {
            Context.PlayerProgresses.Add(
                new PlayerProgress
                {
                    QuestId = 1,
                    PlayerId = "playerGetData1",
                    LastMilestoneCompletedId = 1,
                    QuestPointsEarned = 100
                });

            Context.SaveChanges();

            var questEngineStateClient = new StateClient(TestHttpClient);

            var result = await questEngineStateClient.GetStatePlayerAsync("playerGetData1");

            result.Should().NotBeNull();
            result.LastMilestoneIndexCompleted.Should().Be(1);
            result.TotalQuestPercentCompleted.Should().Be(
                MathHelper.CalculatePercentage(100, GameConfiguration.TotalQuestPoints));
        }
    }
}
