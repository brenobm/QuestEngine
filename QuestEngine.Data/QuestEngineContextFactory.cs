using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QuestEngine.Configuration;

namespace QuestEngine.Data
{
    public class QuestEngineContextFactory : IQuestEngineContextFactory
    {
        private static DbContextOptionsBuilder<QuestEngineContext> _contextOptionsBuilder;

        public QuestEngineContextFactory(ContextConfiguration contextConfiguration)
        {
            _contextOptionsBuilder = new DbContextOptionsBuilder<QuestEngineContext>();

            if (contextConfiguration.InMemoryDb)
            {
                _contextOptionsBuilder.UseInMemoryDatabase("questEngine");
                _contextOptionsBuilder.ConfigureWarnings(x =>
                    x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            }
            else
            {
                _contextOptionsBuilder.UseSqlServer(contextConfiguration.ConnectionString);
            }
        }

        public QuestEngineContext Create()
        {
            return new QuestEngineContext(_contextOptionsBuilder.Options);
        }
    }
}
