
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

        internal readonly object Rotationtype;

        internal Move(int newPinPosRowIndex, int newPinPosColumnIndex, int shiftPositionRowIndex, int shiftPositionColumnIndex, object rotationType)
        {
            NewPinPosRowIndex = newPinPosRowIndex;
            NewPinPosColumnIndex = newPinPosColumnIndex;
            ShiftPositionRowIndex = shiftPositionRowIndex;
            ShiftPositionColumnIndex = shiftPositionColumnIndex;
            Rotationtype = rotationType;
        }

        //TODO: Ich muss die erhaltene shiftcard genauso zurückschicken, nur mit der rotation eingebaut.
    }
}