using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using QuestEngine.Business.Calculators;
using QuestEngine.Business.Services;
using QuestEngine.Configuration.Services;
using QuestEngine.Controllers;
using QuestEngine.Data;
using QuestEngine.Data.Repositories;

namespace QuestEngine
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IQuestEngineConfigurationService, QuestEngineConfigurationService>();
            services.AddSingleton(sp =>
                sp.GetService<IQuestEngineConfigurationService>().GetGameConfiguration()
            );
            services.AddSingleton(sp =>
                sp.GetService<IQuestEngineConfigurationService>().GetContextConfiguration()
            );
            services.AddSingleton(sp =>
                sp.GetService<IQuestEngineConfigurationService>().GetMilestonesConfiguration()
            );

            services.AddSingleton<IQuestEngineContextFactory, QuestEngineContextFactory>();
            services.AddScoped(sp =>
                sp.GetService<IQuestEngineContextFactory>().Create()
            );
            services.AddTransient<IPlayerProgressRepository, PlayerProgressRepository>();

            services.AddScoped<IProgressController, ProgressControllerImpl>();
            services.AddScoped<IStateController, StateControllerImpl>();

            services.AddSingleton<IQuestPointCalculator, QuestPointCalculator>();
            services.AddSingleton<IMilestoneCalculator, MilestoneCalculator>();

            services.AddTransient<IQuestEngineService, QuestEngineService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerDocument(settings =>
            {
                settings.Title = "Quest Engine";
                settings.Version = "1.0.0";
                settings.Description = "Quest Engine for Slot Games";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUi3();
        }
    }
}
