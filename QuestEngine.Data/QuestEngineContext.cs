using Microsoft.EntityFrameworkCore;
using QuestEngine.Data.Mappings;
using QuestEngine.Data.Models;

namespace QuestEngine.Data
{
    public class QuestEngineContext: DbContext
    {
        public QuestEngineContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<PlayerProgress> PlayerProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new PlayerProgressConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
