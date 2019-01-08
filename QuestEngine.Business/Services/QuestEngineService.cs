using QuestEngine.Business.Calculators;
using QuestEngine.Business.Helpers;
using QuestEngine.Configuration;
using QuestEngine.Data.Models;
using QuestEngine.Data.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace QuestEngine.Business.Services
{
    public class QuestEngineService : IQuestEngineService
    {
        private readonly IPlayerProgressRepository _playerProgressRepository;
        private readonly IQuestPointCalculator _questPointCalculator;
        private readonly IMilestoneCalculator _milestoneCalculator;
        private readonly GameConfiguration _gameConfiguration;

        public QuestEngineService(
            IPlayerProgressRepository playerProgressRepository,
            IQuestPointCalculator questPointCalculator,
            IMilestoneCalculator milestoneCalculator,
            GameConfiguration gameConfiguration)
        {
            _playerProgressRepository = playerProgressRepository;
            _questPointCalculator = questPointCalculator;
            _milestoneCalculator = milestoneCalculator;
            _gameConfiguration = gameConfiguration;
        }

        public async Task<PlayerProgressResult> CalculateQuestProgress(string playerId, int playerLevel, long chipAmountBet)
        {
            if (playerId.Length > 50)
            {
                return new PlayerProgressResult
                {
                    QuestPointsEarned = 0,
                    TotalQuestPercentCompleted = 0,
                    MilestonesCompleted = new Data.Models.Milestone[0]
                };
            }

            var playerProgress = await _playerProgressRepository.GetPlayerProgress(playerId, _gameConfiguration.QuestId);

            if (playerLevel < 0 || chipAmountBet < 0)
            {
                return new PlayerProgressResult
                {
                    QuestPointsEarned = 0,
                    TotalQuestPercentCompleted = MathHelper.CalculatePercentage(playerProgress.QuestPointsEarned, _gameConfiguration.TotalQuestPoints),
                    MilestonesCompleted = new Data.Models.Milestone[0]
                };
            }

            var questPointsEarned = _questPointCalculator.CalculateQuestPoints(chipAmountBet, playerLevel);

            if (questPointsEarned + playerProgress.QuestPointsEarned > _gameConfiguration.TotalQuestPoints)
            {
                questPointsEarned = _gameConfiguration.TotalQuestPoints - playerProgress.QuestPointsEarned;
            }

            var milestonesArchived = _milestoneCalculator.GetMilestonesArchived(playerProgress.QuestPointsEarned, questPointsEarned);

            var questPercentCompleted = MathHelper.CalculatePercentage(playerProgress.QuestPointsEarned + questPointsEarned, _gameConfiguration.TotalQuestPoints);

            await _playerProgressRepository.SavePlayerProgress(
                playerId, 
                _gameConfiguration.QuestId, 
                questPointsEarned, 
                milestonesArchived.LastOrDefault()?.MilestoneId);

            return new PlayerProgressResult
            {
                QuestPointsEarned = questPointsEarned,
                TotalQuestPercentCompleted = questPercentCompleted,
                MilestonesCompleted = milestonesArchived
                    .Select(milestone =>
                        new Data.Models.Milestone
                        {
                            MilestoneIndex = milestone.MilestoneId,
                            ChipsAwarded = milestone.ChipsAwarded
                        })
                    .ToList()
            };
        }

        public async Task<PlayerProgresState> GetPlayerQuestState(string playerId)
        {
            if (playerId.Length > 50)
            {
                return new PlayerProgresState
                {
                    LastMilestoneIndexCompleted = null,
                    TotalQuestPercentCompleted = 0
                };
            }
            
            var playerProgress = await _playerProgressRepository.GetPlayerProgress(playerId, _gameConfiguration.QuestId);

            return new PlayerProgresState
            {
                LastMilestoneIndexCompleted = playerProgress.LastMilestoneCompletedId,
                TotalQuestPercentCompleted = MathHelper.CalculatePercentage(playerProgress.QuestPointsEarned, _gameConfiguration.TotalQuestPoints)
            };
        }
    }
}
