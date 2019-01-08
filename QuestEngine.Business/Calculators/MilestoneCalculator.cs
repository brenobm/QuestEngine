using QuestEngine.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace QuestEngine.Business.Calculators
{
    public class MilestoneCalculator : IMilestoneCalculator
    {
        private readonly IReadOnlyCollection<Milestone> _milestoneConfigurations;

        public MilestoneCalculator(IReadOnlyCollection<Milestone> milestoneConfigurations)
        {
            _milestoneConfigurations = milestoneConfigurations;
        }

        public ICollection<Milestone> GetMilestonesArchived(long initialQuestPoints, long questPointEarned)
        {
            return _milestoneConfigurations
                .Where(milestone =>
                    milestone.MilestoneQuestPoints > initialQuestPoints && 
                    milestone.MilestoneQuestPoints <= initialQuestPoints + questPointEarned)
                .ToList();
        }
    }
}
