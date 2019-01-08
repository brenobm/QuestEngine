using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using QuestEngine.Data;

namespace QuestEngine
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<QuestEngineContext>
    {
        public QuestEngineContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var contextFactory = new QuestEngineContextFactory(new Configuration.ContextConfiguration
            {
                ConnectionString = configuration.GetConnectionString("QuestEngineDb"),
                InMemoryDb = false
            });

            return contextFactory.Create();
        }
    }
}
