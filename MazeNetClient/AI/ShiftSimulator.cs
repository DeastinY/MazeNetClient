using MazeNetClient.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MazeNetClient.AI
{
    /// <summary>
    /// A static class that contains a method for generating a list of SimulatedBoards.
    /// One SimulatedBoard represents a possible shift applied to the current game board.
    /// </summary>
    static class ShiftSimulator
    {
        /// <summary>
        /// Describes the maximum number of positions where a shift card can be inserted.
        /// You can only insert the shift card in rows or columns with an odd index.
        /// And you have four possible rotations for each position.
        /// </summary>
        private const int MAX_NUMBER_OF_SHIFTS = (((Board.ROW_COUNT / 2) * 2) + ((Board.COLUMN_COUNT / 2) * 2)) * 4;

        /// <summary>
        /// Creates a list of SimulatedBoard-objects so that every element in that list is the result of a possible shift operation.
        /// </summary>
        /// <param name="currentBoard">The state of the board where the shift operations are based on.</param>
        /// <param name="shiftCard">The shift card that wil be used for the shift operations.</param>
        /// <param name="forbiddenShiftRowIndex">The index of the row where the shift card is not allowed to be inserted.</param>
        /// <param name="forbiddenShiftColumnIndex">The index of the column where the shift card is not allwoed to be inserted.</param>
        /// <returns>A List with SimulatedBoards.</returns>
        internal static List<SimulatedBoard> GeneratePossibleBoards(IFieldCollection currentBoard, Field shiftCard, int forbiddenShiftRowIndex, int forbiddenShiftColumnIndex)
        {
            var possibleBoards = new List<SimulatedBoard>(MAX_NUMBER_OF_SHIFTS);

            //Iterate over the rows.
            for (int i = 1; i < Board.ROW_COUNT; i += 2)
            {
                //The left column for insertion
                if (IsValidShiftPosition(forbiddenShiftRowIndex, forbiddenShiftColumnIndex, i, 0))
                    AddAllShiftRotations(possibleBoards, currentBoard, shiftCard, i, 0);

                //The right column for insertion
                if (IsValidShiftPosition(forbiddenShiftRowIndex, forbiddenShiftColumnIndex, i, Board.COLUMN_COUNT - 1))
                    AddAllShiftRotations(possibleBoards, currentBoard, shiftCard, i, Board.COLUMN_COUNT - 1);
            }

            //Iterate over the columns.
            for (int j = 1; j < Board.COLUMN_COUNT; j += 2)
            {
                //The upper row for insertion
                if (IsValidShiftPosition(forbiddenShiftRowIndex, forbiddenShiftColumnIndex, 0, j))
                    AddAllShiftRotations(possibleBoards, currentBoard, shiftCard, 0, j);

                //The lower row for insertion
                if (IsValidShiftPosition(forbiddenShiftRowIndex, forbiddenShiftColumnIndex, Board.ROW_COUNT - 1, j))
                    AddAllShiftRotations(possibleBoards, currentBoard, shiftCard, Board.ROW_COUNT - 1, j);
            }

            Debug.Assert(possibleBoards.Capacity == MAX_NUMBER_OF_SHIFTS && (possibleBoards.Count == MAX_NUMBER_OF_SHIFTS || possibleBoards.Count + 4 == MAX_NUMBER_OF_SHIFTS));
            return possibleBoards;
        }

        static bool IsValidShiftPosition(int forbiddenShiftRowIndex, int forbiddenShiftColumnIndex, int shiftPositionRowIndex, int shiftPositionColumnIndex)
        {
            return forbiddenShiftRowIndex != shiftPositionRowIndex
                || forbiddenShiftColumnIndex != shiftPositionColumnIndex;
        }

        static void AddAllShiftRotations(List<SimulatedBoard> container, IFieldCollection currentBoard, Field shiftCard, int shiftCardPositionRowIndex, int shiftCardPositionColumnIndex)
        {
            foreach (Rotation aRotation in Enum.GetValues(typeof(Rotation)))
            {
                var aSimulatedBoard = new SimulatedBoard(currentBoard, shiftCard, shiftCardPositionRowIndex, shiftCardPositionColumnIndex, aRotation);
                container.Add(aSimulatedBoard);
            }
        }
    }
}