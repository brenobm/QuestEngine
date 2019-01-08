using QuestEngine.Business.Services;
using System.Linq;
using System.Threading.Tasks;

namespace QuestEngine.Controllers
{
    public class ProgressControllerImpl : IProgressController
    {
        private readonly IQuestEngineService _questEngineService;

        public ProgressControllerImpl(IQuestEngineService questEngineService)
        {
            _questEngineService = questEngineService;
        }

        public async Task<ProgressOutput> PostProgressAsync(ProgressInput request)
        {
            var playerProgress = await _questEngineService.CalculateQuestProgress(request.PlayerId, (int)request.PlayerLevel, request.ChipAmountBet);

            return new ProgressOutput
            {
                QuestPointsEarned = playerProgress.QuestPointsEarned,
                TotalQuestPercentCompleted = playerProgress.TotalQuestPercentCompleted,
                MilestonesCompleted = playerProgress
                                            .MilestonesCompleted
                                            .Select(milestone =>
                                                new Milestone
                                                {
                                                    ChipsAwarded = milestone.ChipsAwarded,
                                                    MilestoneIndex = milestone.MilestoneIndex
                                                })
                                            .ToList()
            };
        }
    }
}
