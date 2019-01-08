using QuestEngine.Data.Models;
using System.Threading.Tasks;

namespace QuestEngine.Business.Services
{
    public interface IQuestEngineService
    {
        Task<PlayerProgressResult> CalculateQuestProgress(string playerId, int playerLevel, long chipAmountBet);
        Task<PlayerProgresState> GetPlayerQuestState(string playerId);
    }
}
