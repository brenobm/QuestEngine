using System.Collections.Generic;

namespace QuestEngine.Data.Models
{
    public class PlayerProgressResult
    {
        public long QuestPointsEarned { get; set; }
        public decimal TotalQuestPercentCompleted { get; set; }
        public ICollection<Milestone> MilestonesCompleted { get; set; }
    }
}
