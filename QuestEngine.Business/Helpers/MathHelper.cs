using System;

namespace QuestEngine.Business.Helpers
{
    public static class MathHelper
    {
        public static decimal CalculatePercentage(long newValue, long originalValue)
        {
            if (originalValue == 0)
                return 0;

            var percentage = (decimal)newValue / originalValue;

            return Math.Truncate(percentage * 100 * 100) / 100;
        }
    }
}
