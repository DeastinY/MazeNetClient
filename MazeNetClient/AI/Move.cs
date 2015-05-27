
namespace MazeNetClient.AI
{
    /// <summary>
    /// This class describes the result of an IMazePlayers move.
    /// </summary>
    class Move
    {
        internal readonly int NewPinPosRowIndex;

        internal readonly int NewPinPosColumnIndex;

        internal readonly int ShiftPositionRowIndex;

        internal readonly int ShiftPositionColumnIndex;

        internal readonly Rotation ShiftCardRotation;

        internal Move(int newPinPosRowIndex, int newPinPosColumnIndex, int shiftPositionRowIndex, int shiftPositionColumnIndex, Rotation shiftCardRotation = Rotation.DEGREE_0)
        {
            NewPinPosRowIndex = newPinPosRowIndex;
            NewPinPosColumnIndex = newPinPosColumnIndex;
            ShiftPositionRowIndex = shiftPositionRowIndex;
            ShiftPositionColumnIndex = shiftPositionColumnIndex;
            ShiftCardRotation = shiftCardRotation;
        }
    }
}