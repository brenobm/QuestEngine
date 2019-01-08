using Microsoft.EntityFrameworkCore;
using QuestEngine.Data.Models;

namespace QuestEngine.Data.Mappings
{
    public class PlayerProgressConfiguration : IEntityTypeConfiguration<PlayerProgress>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<PlayerProgress> builder)
        {
            builder
                .Property(pp => pp.PlayerId)
                .HasMaxLength(50)
                .ValueGeneratedNever();

            builder
                .Property(pp => pp.QuestId)
                .ValueGeneratedNever();

            builder
                .Property(pp => pp.LastMilestoneCompletedId);

            builder
                .Property(pp => pp.QuestPointsEarned);

            builder
                .HasKey(pp => new { pp.PlayerId, pp.QuestId });
        }
    }
}
