using System.Collections.Generic;
using Xunit;

namespace CeilingFinishNumerator.Tests
{
    public class RoomPhaseFallbackSelectorTests
    {
        [Fact]
        public void SelectPhaseWithRoomsKeepsSelectedPhaseWhenItHasRooms()
        {
            var selector = new RoomPhaseFallbackSelector();
            var phaseIds = new List<long> { 1, 2, 3 };
            var roomCountsByPhase = new Dictionary<long, int>
            {
                [1] = 4,
                [2] = 2,
                [3] = 7
            };

            long? selectedPhaseId = selector.SelectPhaseWithRooms(phaseIds, 3, roomCountsByPhase);

            Assert.Equal(3, selectedPhaseId);
        }

        [Fact]
        public void SelectPhaseWithRoomsFallsBackToNearestPreviousPhaseWithRooms()
        {
            var selector = new RoomPhaseFallbackSelector();
            var phaseIds = new List<long> { 1, 2, 3, 4 };
            var roomCountsByPhase = new Dictionary<long, int>
            {
                [1] = 3,
                [2] = 0,
                [3] = 5,
                [4] = 0
            };

            long? selectedPhaseId = selector.SelectPhaseWithRooms(phaseIds, 4, roomCountsByPhase);

            Assert.Equal(3, selectedPhaseId);
        }

        [Fact]
        public void SelectPhaseWithRoomsKeepsSelectedPhaseWhenNoEarlierPhaseHasRooms()
        {
            var selector = new RoomPhaseFallbackSelector();
            var phaseIds = new List<long> { 1, 2, 3 };
            var roomCountsByPhase = new Dictionary<long, int>
            {
                [1] = 0,
                [2] = 0,
                [3] = 0
            };

            long? selectedPhaseId = selector.SelectPhaseWithRooms(phaseIds, 3, roomCountsByPhase);

            Assert.Equal(3, selectedPhaseId);
        }
    }
}
