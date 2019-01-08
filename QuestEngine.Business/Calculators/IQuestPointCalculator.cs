namespace QuestEngine.Business.Calculators
{
    public interface IQuestPointCalculator
    {
        long CalculateQuestPoints(long chipAmountBet, int playerLevel);
    }
}
