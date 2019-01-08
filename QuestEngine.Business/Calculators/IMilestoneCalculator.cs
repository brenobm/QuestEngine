using QuestEngine.Configuration;
using System.Collections.Generic;

namespace QuestEngine.Business.Calculators
{
    public interface IMilestoneCalculator
    {
        ICollection<Milestone> GetMilestonesArchived(long initialQuestPoints, long questPointEarned); 
    }
}