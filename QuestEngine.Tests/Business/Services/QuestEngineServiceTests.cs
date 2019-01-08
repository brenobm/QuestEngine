using FluentAssertions;
using Moq;
using QuestEngine.Business.Calculators;
using QuestEngine.Business.Services;
using QuestEngine.Configuration;
using QuestEngine.Data.Models;
using QuestEngine.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Milestone = QuestEngine.Configuration.Milestone;

namespace QuestEngine.Tests.Business.Services
{
    public class QuestEngineServiceTests
    {
        private readonly Mock<IPlayerProgressRepository> _playerProgressRepository;
        private readonly Mock<IQuestPointCalculator> _questPointCalculator;
        private readonly Mock<IMilestoneCalculator> _milestoneCalculator;
        private readonly GameConfiguration _gameConfiguration;

        public QuestEngineServiceTests()
        {
            _playerProgressRepository = new Mock<IPlayerProgressRepository>();

            _questPointCalculator = new Mock<IQuestPointCalculator>();

            _milestoneCalculator = new Mock<IMilestoneCalculator>();

            _gameConfiguration = new GameConfiguration
            {
                QuestId = 1,
                TotalQuestPoints = 1000
            };
        }

        [Fact]
        public async Task GetPlayerQuestState_PlayerDoesntHaveProgress_ReturnZeroProgress()
        {
            _playerProgressRepository
                .Setup(ppr =>
                    ppr.GetPlayerProgress(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => new PlayerProgress());


            var questEngineService = new QuestEngineService(
                _playerProgressRepository.Object, 
                _questPointCalculator.Object,
                _milestoneCalculator.Object, 
                _gameConfiguration);

            var result = await questEngineService.GetPlayerQuestState("playerOne");

            result.Should().NotBeNull();
            result.TotalQuestPercentCompleted.Should().Be(0);
            result.LastMilestoneIndexCompleted.Should().BeNull();
        }

        [Fact]
        public async Task GetPlayerQuestState_PlayerIdGreaterThan50_ReturnZeroProgress()
        {
            _playerProgressRepository
                .Setup(ppr =>
                    ppr.GetPlayerProgress(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => new PlayerProgress());


            var questEngineService = new QuestEngineService(
                _playerProgressRepository.Object, 
                _questPointCalculator.Object,
                _milestoneCalculator.Object, 
                _gameConfiguration);

            var result = await questEngineService.GetPlayerQuestState("player123456789012345678901234567890123456789012345678901234567890");

            result.Should().NotBeNull();
            result.TotalQuestPercentCompleted.Should().Be(0);
            result.LastMilestoneIndexCompleted.Should().BeNull();
        }

        [Fact]
        public async Task CalculateQuestProgress_PlayerIdGreaterThan50_ShouldReturnEmptyProgress()
        {
            var questEngineService = new QuestEngineService(
                _playerProgressRepository.Object, 
                _questPointCalculator.Object,
                _milestoneCalculator.Object, 
                _gameConfiguration);

            var result = await questEngineService.CalculateQuestProgress("player123456789012345678901234567890123456789012345678901234567890", 1, 50);

            result.Should().NotBeNull();
            result.MilestonesCompleted.Should().BeEmpty();
            result.QuestPointsEarned.Should().Be(0);
            result.TotalQuestPercentCompleted.Should().Be(0);

            _playerProgressRepository.Verify(ppr =>
                ppr.SavePlayerProgress(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int?>()), Times.Never);
        }

        [Theory]
        [InlineData(1, 135, 13.5)]
        [InlineData(null, 135, 13.5)]
        [InlineData(null, 0, 0)]
        [InlineData(2, 520, 52)]
        [InlineData(3, 1000, 100)]
        public async Task GetPlayerQuestState_PlayerHaveProgress_ReturnCurrentProgress(
            int? lastMilestoneCompletedId,
            long questPointsEarned,
            decimal totalQuestPercentCompleted)
        {
            _playerProgressRepository
                .Setup(ppr =>
                    ppr.GetPlayerProgress(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => 
                    new PlayerProgress
                    {
                        LastMilestoneCompletedId = lastMilestoneCompletedId,
                        QuestPointsEarned = questPointsEarned
                    });


            var questEngineService = new QuestEngineService(
                _playerProgressRepository.Object, 
                _questPointCalculator.Object,
                _milestoneCalculator.Object, 
                _gameConfiguration);

            var result = await questEngineService.GetPlayerQuestState("playerOne");

            result.Should().NotBeNull();
            result.TotalQuestPercentCompleted.Should().Be(totalQuestPercentCompleted);
            result.LastMilestoneIndexCompleted.Should().Be(lastMilestoneCompletedId);
        }

        [Fact]
        public async Task CalculateQuestProgress_PlayerWithPreviousProgressBetPositiveAmountNotCompleteMilestone_ShouldSaveAndReturnTheProgress()
        {
            _playerProgressRepository
                .Setup(ppr =>
                    ppr.GetPlayerProgress(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => 
                    new PlayerProgress
                    {
                        LastMilestoneCompletedId = null,
                        QuestPointsEarned = 100
                    });

            _questPointCalculator
                .Setup(qpc =>
                    qpc.CalculateQuestPoints(It.IsAny<long>(), It.IsAny<int>()))
                .Returns(100);

            _milestoneCalculator
                .Setup(mc =>
                    mc.GetMilestonesArchived(It.IsAny<long>(), It.IsAny<long>()))
                .Returns((ICollection<Milestone>)new Milestone[0]);

            var questEngineService = new QuestEngineService(
                _playerProgressRepository.Object, 
                _questPointCalculator.Object,
                _milestoneCalculator.Object, 
                _gameConfiguration);

            var result = await questEngineService.CalculateQuestProgress("playerOne", 1, 50);

            result.Should().NotBeNull();
            result.MilestonesCompleted.Should().BeEmpty();
            result.QuestPointsEarned.Should().Be(100);
            result.TotalQuestPercentCompleted.Should().Be(20m);

            _playerProgressRepository.Verify(ppr =>
                ppr.SavePlayerProgress("playerOne", 1, 100, null), Times.Once);
        }

        [Fact]
        public async Task CalculateQuestProgress_PlayerWithPreviousProgressBetPositiveAmountCompleteMilestone_ShouldSaveAndReturnTheProgress()
        {
            _playerProgressRepository
                .Setup(ppr =>
                    ppr.GetPlayerProgress(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => 
                    new PlayerProgress
                    {
                        LastMilestoneCompletedId = null,
                        QuestPointsEarned = 200
                    });

            _questPointCalculator
                .Setup(qpc =>
                    qpc.CalculateQuestPoints(It.IsAny<long>(), It.IsAny<int>()))
                .Returns(300);

            _milestoneCalculator
                .Setup(mc =>
                    mc.GetMilestonesArchived(It.IsAny<long>(), It.IsAny<long>()))
                .Returns((ICollection<Milestone>)new []
                        {
                            new Milestone
                            {
                                ChipsAwarded = 10,
                                MilestoneId = 1
                            }
                        });

            var questEngineService = new QuestEngineService(
                _playerProgressRepository.Object, 
                _questPointCalculator.Object,
                _milestoneCalculator.Object, 
                _gameConfiguration);

            var result = await questEngineService.CalculateQuestProgress("playerOne", 1, 50);

            result.Should().NotBeNull();
            result.MilestonesCompleted.Count.Should().Be(1);
            result.MilestonesCompleted.First().ChipsAwarded.Should().Be(10);
            result.MilestonesCompleted.First().MilestoneIndex.Should().Be(1);

            result.QuestPointsEarned.Should().Be(300);
            result.TotalQuestPercentCompleted.Should().Be(50m);

            _playerProgressRepository.Verify(ppr =>
                ppr.SavePlayerProgress("playerOne", 1, 300, 1), Times.Once);
        }

        [Fact]
        public async Task CalculateQuestProgress_PlayerWithPreviousProgressBetPositiveAmountCompleteTwoMilestone_ShouldSaveAndReturnTheProgress()
        {
            _playerProgressRepository
                .Setup(ppr =>
                    ppr.GetPlayerProgress(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => 
                    new PlayerProgress
                    {
                        LastMilestoneCompletedId = null,
                        QuestPointsEarned = 500
                    });

            _questPointCalculator
                .Setup(qpc =>
                    qpc.CalculateQuestPoints(It.IsAny<long>(), It.IsAny<int>()))
                .Returns(150);

            _milestoneCalculator
                .Setup(mc =>
                    mc.GetMilestonesArchived(It.IsAny<long>(), It.IsAny<long>()))
                .Returns((ICollection<Milestone>)new []
                        {
                            new Milestone
                            {
                                ChipsAwarded = 10,
                                MilestoneId = 1
                            },
                            new Milestone
                            {
                                ChipsAwarded = 20,
                                MilestoneId = 2
                            }
                        });

            var questEngineService = new QuestEngineService(
                _playerProgressRepository.Object, 
                _questPointCalculator.Object,
                _milestoneCalculator.Object, 
                _gameConfiguration);

            var result = await questEngineService.CalculateQuestProgress("playerOne", 1, 50);

            result.Should().NotBeNull();
            result.MilestonesCompleted.Count.Should().Be(2);
            result.MilestonesCompleted.First().ChipsAwarded.Should().Be(10);
            result.MilestonesCompleted.First().MilestoneIndex.Should().Be(1);

            result.MilestonesCompleted.Last().ChipsAwarded.Should().Be(20);
            result.MilestonesCompleted.Last().MilestoneIndex.Should().Be(2);

            result.QuestPointsEarned.Should().Be(150);
            result.TotalQuestPercentCompleted.Should().Be(65m);

            _playerProgressRepository.Verify(ppr =>
                ppr.SavePlayerProgress("playerOne", 1, 150, 2), Times.Once);
        }

        [Fact]
        public async Task CalculateQuestProgress_PlayerWithPreviousProgressBetPositiveAmountAndShouldReceiveMorePointsThanQuest_ShouldReceiveMaxQuestPointsAndSaveAndReturnTheProgress()
        {
            _playerProgressRepository
                .Setup(ppr =>
                    ppr.GetPlayerProgress(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => 
                    new PlayerProgress
                    {
                        LastMilestoneCompletedId = 3,
                        QuestPointsEarned = 800
                    });

            _questPointCalculator
                .Setup(qpc =>
                    qpc.CalculateQuestPoints(It.IsAny<long>(), It.IsAny<int>()))
                .Returns(350);

            _milestoneCalculator
                .Setup(mc =>
                    mc.GetMilestonesArchived(It.IsAny<long>(), It.IsAny<long>()))
                .Returns((ICollection<Milestone>)new Milestone[0]);

            var questEngineService = new QuestEngineService(
                _playerProgressRepository.Object, 
                _questPointCalculator.Object,
                _milestoneCalculator.Object, 
                _gameConfiguration);

            var result = await questEngineService.CalculateQuestProgress("playerOne", 1, 50);

            result.Should().NotBeNull();
            result.MilestonesCompleted.Should().BeEmpty();


            result.QuestPointsEarned.Should().Be(200);
            result.TotalQuestPercentCompleted.Should().Be(100m);

            _playerProgressRepository.Verify(ppr =>
                ppr.SavePlayerProgress("playerOne", 1, 200, null), Times.Once);
        }

        [Fact]
        public async Task CalculateQuestProgress_PlayerAlreadyGetAllQuestPointsBetPositiveAmount_ShouldNotReceiveQuestPointsAndSaveAndReturnTheProgress()
        {
            _playerProgressRepository
                .Setup(ppr =>
                    ppr.GetPlayerProgress(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => 
                    new PlayerProgress
                    {
                        LastMilestoneCompletedId = 5,
                        QuestPointsEarned = 1000
                    });

            _questPointCalculator
                .Setup(qpc =>
                    qpc.CalculateQuestPoints(It.IsAny<long>(), It.IsAny<int>()))
                .Returns(50);

            _milestoneCalculator
                .Setup(mc =>
                    mc.GetMilestonesArchived(It.IsAny<long>(), It.IsAny<long>()))
                .Returns((ICollection<Milestone>)new Milestone[0]);

            var questEngineService = new QuestEngineService(
                _playerProgressRepository.Object, 
                _questPointCalculator.Object,
                _milestoneCalculator.Object, 
                _gameConfiguration);

            var result = await questEngineService.CalculateQuestProgress("playerOne", 1, 50);

            result.Should().NotBeNull();
            result.MilestonesCompleted.Should().BeEmpty();


            result.QuestPointsEarned.Should().Be(0);
            result.TotalQuestPercentCompleted.Should().Be(100m);

            _playerProgressRepository.Verify(ppr =>
                ppr.SavePlayerProgress("playerOne", 1, 0, null), Times.Once);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(-1, -1)]
        public async Task CalculateQuestProgress_PlayerWithPreviousProgressBetNegativeAmountOrNegativeLevel_ShouldNotReceiveQuestPointsAndSaveAndReturnTheProgress(
            int playerLevel,
            long chipAmountBet)
        {
            _playerProgressRepository
                .Setup(ppr =>
                    ppr.GetPlayerProgress(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => 
                    new PlayerProgress
                    {
                        LastMilestoneCompletedId = null,
                        QuestPointsEarned = 100
                    });

            _questPointCalculator
                .Setup(qpc =>
                    qpc.CalculateQuestPoints(It.IsAny<long>(), It.IsAny<int>()))
                .Returns(100);

            _milestoneCalculator
                .Setup(mc =>
                    mc.GetMilestonesArchived(It.IsAny<long>(), It.IsAny<long>()))
                .Returns((ICollection<Milestone>)new Milestone[0]);

            var questEngineService = new QuestEngineService(
                _playerProgressRepository.Object, 
                _questPointCalculator.Object,
                _milestoneCalculator.Object, 
                _gameConfiguration);

            var result = await questEngineService.CalculateQuestProgress("playerOne", playerLevel, chipAmountBet);

            result.Should().NotBeNull();
            result.MilestonesCompleted.Should().BeEmpty();


            result.QuestPointsEarned.Should().Be(0);
            result.TotalQuestPercentCompleted.Should().Be(10m);

            _playerProgressRepository
                .Verify(ppr =>
                    ppr.SavePlayerProgress(
                        It.IsAny<string>(), 
                        It.IsAny<int>(), 
                        It.IsAny<long>(), 
                        It.IsAny<int?>()), 
                    Times.Never);
        }
    }
}
