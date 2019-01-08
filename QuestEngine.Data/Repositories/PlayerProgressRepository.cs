using QuestEngine.Data.Models;
using System;
using System.Threading.Tasks;

namespace QuestEngine.Data.Repositories
{
    public class PlayerProgressRepository : IPlayerProgressRepository, IDisposable
    {
        private readonly QuestEngineContext _context;
        private bool _disposedValue = false;

        public PlayerProgressRepository(QuestEngineContext context)
        {
            _context = context;
        }

        public async Task<PlayerProgress> GetPlayerProgress(string playerId, int questId)
        {
            return await _context.PlayerProgresses.FindAsync(playerId, questId) ?? 
                new PlayerProgress
                {
                    PlayerId = playerId,
                    QuestId = questId,
                    QuestPointsEarned = 0,
                    LastMilestoneCompletedId = null
                };
        }

        public async Task SavePlayerProgress(string playerId, int questId, long questPointsEarned, int? lastMilestoneCompleted)
        {
            var playerProgress = await _context.PlayerProgresses.FindAsync(playerId, questId);

            if (playerProgress == null)
            {
                InsertPlayerProgress(playerId, questId, questPointsEarned, lastMilestoneCompleted);
            }
            else
            {
                UpdatePlayerProgress(playerProgress, questPointsEarned, lastMilestoneCompleted);
            }

            await _context.SaveChangesAsync();
        }

        private void UpdatePlayerProgress(PlayerProgress playerProgress, long questPointsEarned, int? lastMilestoneCompleted)
        {
            playerProgress.QuestPointsEarned += questPointsEarned;

            if (lastMilestoneCompleted != null)
                playerProgress.LastMilestoneCompletedId = lastMilestoneCompleted;
        }

        private void InsertPlayerProgress(string playerId, int questId, long questPointsEarned, int? lastMilestoneCompleted)
        {
            PlayerProgress playerProgress = new PlayerProgress
            {
                PlayerId = playerId,
                QuestId = questId,
                QuestPointsEarned = questPointsEarned,
                LastMilestoneCompletedId = lastMilestoneCompleted
            };

            _context.PlayerProgresses.Add(playerProgress);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                
                _disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
