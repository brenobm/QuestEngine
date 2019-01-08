using QuestEngine.Data.Models;
using System.Threading.Tasks;

namespace QuestEngine.Data.Repositories
{
    public interface IPlayerProgressRepository
    {
        Task<PlayerProgress> GetPlayerProgress(string playerId, int questId);
        Task SavePlayerProgress(string playerId, int questId, long questPointsEarned, int? lastMilestoneCompleted);
    }
}
