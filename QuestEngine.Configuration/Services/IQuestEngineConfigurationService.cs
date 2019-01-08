using System.Collections.Generic;

namespace QuestEngine.Configuration.Services
{
    public interface IQuestEngineConfigurationService
    {
        GameConfiguration GetGameConfiguration();

        IReadOnlyCollection<Milestone> GetMilestonesConfiguration();

        ContextConfiguration GetContextConfiguration();
    }
}
