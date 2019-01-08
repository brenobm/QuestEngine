using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace QuestEngine.Configuration.Services
{
    public class QuestEngineConfigurationService : IQuestEngineConfigurationService
    {
        private readonly IConfiguration _configuration;

        public QuestEngineConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GameConfiguration GetGameConfiguration() =>
                _configuration.GetSection("GameConfiguration").Get<GameConfiguration>();

        public IReadOnlyCollection<Milestone> GetMilestonesConfiguration() =>
                _configuration.GetSection("MilestoneConfiguration").Get<IReadOnlyCollection<Milestone>>();

        public ContextConfiguration GetContextConfiguration()
        {
            return new ContextConfiguration
            {
                ConnectionString = _configuration.GetConnectionString("QuestEngineDb"),
                InMemoryDb = _configuration.GetValue<bool>("UseInMemoryDb")
            };
        }

    }
}
