namespace QuestEngine.Data.Models
{
    public class PlayerProgress
    {
        public string PlayerId { get; set; }
        public int QuestId { get; set; }
        public long QuestPointsEarned { get; set; }
        public int? LastMilestoneCompletedId { get; set; }
    }
}
