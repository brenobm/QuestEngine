using System.Linq;
using FluentAssertions;
using QuestEngine.Business.Calculators;
using QuestEngine.Configuration;
using Xunit;

namespace QuestEngine.Tests.Business.Calculators
{
    public class MilestoneCalculatorTests
    {
        private static Milestone[] _milestoneConfigurations;

        public MilestoneCalculatorTests()
        {
            _milestoneConfigurations = new[]
            {
                new Milestone
                {
                    MilestoneId = 1,
                    ChipsAwarded = 10,
                    MilestoneQuestPoints = 10
                },
                new Milestone
                {
                    MilestoneId = 2,
                    ChipsAwarded = 15,
                    MilestoneQuestPoints = 25
                },
                new Milestone
                {
                    MilestoneId = 3,
                    ChipsAwarded = 25,
                    MilestoneQuestPoints = 50
                }
            };
        }

        [Fact]
        public void GetMilestonesArchived_NoMilestoneConfiguration_ShouldReturnEmptyList()
        {
            var milestoneConfigurations = new Milestone[0];

            var milestoneCalculator = new MilestoneCalculator(milestoneConfigurations);

            var milestonesArchived = milestoneCalculator.GetMilestonesArchived(0, 10);

            milestonesArchived.Should().NotBeNull();
            milestonesArchived.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0, 5, new int[] { })]
        [InlineData(0, 10, new int[] { 1 })]
        [InlineData(0, 30, new int[] { 1, 2 })]
        [InlineData(0, 500, new int[] { 1, 2, 3 })]
        [InlineData(10, 30, new int[] { 2 })]
        [InlineData(10, 55, new int[] { 2, 3 })]
        public void GetMilestonesArchived_PassingPointsEarned_ShouldGetCorrectMilestones(
            int initialQuestPoints,
            int questPointsEarned,
            int[] milestones)
        {
            var milestoneCalculator = new MilestoneCalculator(_milestoneConfigurations);

            var milestonesArchived = milestoneCalculator.GetMilestonesArchived(initialQuestPoints, questPointsEarned);

            milestonesArchived.Should().NotBeNull();
            milestonesArchived.Count.Should().Be(milestones.Length);
            milestonesArchived.Select(m => m.MilestoneId).Should().BeEquivalentTo(milestones);
        }

        [Fact]
        public void GetMilestonesArchived_PassingNegativeValues_ShouldNotThrowErrorAndReturnEmptyList()
        {
            var milestoneCalculator = new MilestoneCalculator(_milestoneConfigurations);

            var milestonesAchived = milestoneCalculator.GetMilestonesArchived(-1, -50);

            milestonesAchived.Should().NotBeNull();
            milestonesAchived.Should().BeEmpty();
        }

    }
}
