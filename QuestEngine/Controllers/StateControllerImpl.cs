using QuestEngine.Business.Services;
using System.Threading.Tasks;

namespace QuestEngine.Controllers
{
    public class StateControllerImpl : IStateController
    {
        private readonly IQuestEngineService _questEngineService;

        public StateControllerImpl(IQuestEngineService questEngineService)
        {
            _questEngineService = questEngineService;
        }

        public async Task<StateOutput> GetStatePlayerAsync(string playerId)
        {
            var questState = await _questEngineService.GetPlayerQuestState(playerId);

            return new StateOutput
            {
                TotalQuestPercentCompleted = questState.TotalQuestPercentCompleted,
                LastMilestoneIndexCompleted = questState.LastMilestoneIndexCompleted
            };
        }
    }
}
