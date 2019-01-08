using QuestEngine.Configuration;

namespace QuestEngine.Business.Calculators
{
    public class QuestPointCalculator : IQuestPointCalculator
    {
        private readonly GameConfiguration _gameConfiguration;

        public QuestPointCalculator(GameConfiguration gameConfiguration)
        {
            _gameConfiguration = gameConfiguration;
        }

        public long CalculateQuestPoints(long chipAmountBet, int playerLevel)
        {
            if (chipAmountBet < 0 || playerLevel < 0)
                return 0;

            return (chipAmountBet * _gameConfiguration.RateFromBet) +
                (playerLevel * _gameConfiguration.LevelBonusRate);
        }
    }
}
