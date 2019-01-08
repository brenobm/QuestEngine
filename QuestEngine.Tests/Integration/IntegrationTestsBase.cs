using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestEngine.Configuration;
using QuestEngine.Data;
using System.Net.Http;

namespace QuestEngine.Tests.Integration
{
    public class IntegrationTestsBase: IDisposable
    {
        protected readonly HttpClient TestHttpClient;
        protected readonly QuestEngineContext Context;
        protected readonly GameConfiguration GameConfiguration;
        protected readonly TestServer QuestEngineServer;
        protected readonly IReadOnlyCollection<Milestone> MilestoneConfigurations;

        public IntegrationTestsBase()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            QuestEngineServer = new TestServer(new WebHostBuilder()
                .UseEnvironment("Develpment")
                .UseConfiguration(configuration)
                .UseStartup<Startup>());

            TestHttpClient = QuestEngineServer.CreateClient();

            Context = QuestEngineServer.Host.Services.GetService<QuestEngineContext>();
            GameConfiguration = QuestEngineServer.Host.Services.GetService<GameConfiguration>();
            MilestoneConfigurations = QuestEngineServer.Host.Services.GetService<IReadOnlyCollection<Milestone>>();
        }


        public void Dispose()
        {
            Context?.Database?.EnsureDeleted();
        }
    }
}
