using MazeNetClient.Game;
using System.Diagnostics;

namespace MazeNetClient.AI
{
    /// <summary>
    /// This class describes the result of an IMazePlayers move.
    /// </summary>
    class Move
    {
        /// <summary>
        /// Holds the index of the row where the player will move to.
        /// </summary>
        internal readonly int NewPinPosRowIndex;

        /// <summary>
        /// Holds the index of the column where the player will move to.
        /// </summary>
        internal readonly int NewPinPosColumnIndex;

        /// <summary>
        /// Holds the index of the row where the shift card will be inserted.
        /// </summary>
        internal readonly int ShiftPositionRowIndex;

        /// <summary>
        /// Holds the index of the column where the shift card will be inserted.
        /// </summary>
        internal readonly int ShiftPositionColumnIndex;

        /// <summary>
        /// Holds the clockwise rotation in degrees that will be applied to the shift card before insertion.
        /// </summary>
        internal readonly Rotation ShiftCardRotation;

        /// <summary>
        /// Creates and initializes a new instance of the type Move.
        /// </summary>
        /// <param name="newPinPosRowIndex">The value for NewPinPosRowIndex.</param>
        /// <param name="newPinPosColumnIndex">The value for NewPinPosColumnIndex.</param>
        /// <param name="shiftPositionRowIndex">The value for ShiftPositionRowIndex.</param>
        /// <param name="shiftPositionColumnIndex">The value for ShiftPositionColumnIndex.</param>
        /// <param name="shiftCardRotation">The value for ShiftCardRotation.</param>
        internal Move(int newPinPosRowIndex, int newPinPosColumnIndex, int shiftPositionRowIndex, int shiftPositionColumnIndex, Rotation shiftCardRotation = Rotation.DEGREE_0)
        {
            Debug.Assert(newPinPosRowIndex >= 0 && newPinPosRowIndex < Board.ROW_COUNT);
            Debug.Assert(newPinPosColumnIndex >= 0 && newPinPosColumnIndex < Board.COLUMN_COUNT);
            Debug.Assert(shiftPositionRowIndex >= 0 && shiftPositionRowIndex < Board.ROW_COUNT);
            Debug.Assert(shiftPositionColumnIndex >= 0 && shiftPositionColumnIndex < Board.COLUMN_COUNT);
            Debug.Assert(
                ((shiftPositionRowIndex % 2 == 1) && (shiftPositionColumnIndex == 0))
                || ((shiftPositionRowIndex % 2 == 1) && (shiftPositionColumnIndex == Board.COLUMN_COUNT - 1))
                || ((shiftPositionRowIndex == 0) && (shiftPositionColumnIndex % 2 == 1))
                || ((shiftPositionRowIndex == Board.ROW_COUNT - 1) && (shiftPositionColumnIndex % 2 == 1)));

            Debug.Assert(System.Enum.IsDefined(typeof(Rotation), shiftCardRotation));

            NewPinPosRowIndex = newPinPosRowIndex;
            NewPinPosColumnIndex = newPinPosColumnIndex;
            ShiftPositionRowIndex = shiftPositionRowIndex;
            ShiftPositionColumnIndex = shiftPositionColumnIndex;
            ShiftCardRotation = shiftCardRotation;
        }

        internal static Move Create(ShiftedBoard newBoard, int newPinPosRowIndex, int newPinPosColumnIndex)
        {
            return new Move(newPinPosRowIndex, newPinPosColumnIndex, newBoard.ShiftPositionRowIndex, newBoard.ShiftPositionColumnIndex, newBoard.ShiftCardRotation);
        }
    }
}