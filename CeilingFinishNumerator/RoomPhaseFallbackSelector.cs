using System.Collections.Generic;

namespace CeilingFinishNumerator
{
    public sealed class RoomPhaseFallbackSelector
    {
        public long? SelectPhaseWithRooms(
            IReadOnlyList<long> phaseIds,
            long? selectedPhaseIdValue,
            IReadOnlyDictionary<long, int> roomCountsByPhase)
        {
            if (!selectedPhaseIdValue.HasValue)
            {
                return null;
            }

            int selectedPhaseIndex = -1;
            for (int i = 0; i < phaseIds.Count; i++)
            {
                if (phaseIds[i] == selectedPhaseIdValue.Value)
                {
                    selectedPhaseIndex = i;
                    break;
                }
            }

            if (selectedPhaseIndex < 0)
            {
                return selectedPhaseIdValue;
            }

            for (int i = selectedPhaseIndex; i >= 0; i--)
            {
                long phaseId = phaseIds[i];
                if (roomCountsByPhase.TryGetValue(phaseId, out int roomCount) && roomCount > 0)
                {
                    return phaseId;
                }
            }

            return selectedPhaseIdValue;
        }
    }
}
