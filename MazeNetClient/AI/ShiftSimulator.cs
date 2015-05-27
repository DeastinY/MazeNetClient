using MazeNetClient.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MazeNetClient.AI
{
    static class ShiftSimulator
    {
        /// <summary>
        /// Describes the maximum number of positions where a shift card can be inserted.
        /// You can only insert the shift card in rows or columns with an odd index.
        /// And you have four possible rotations for each position.
        /// </summary>
        private const int MAX_NUMBER_OF_SHIFTS = (((Board.ROW_COUNT / 2) * 2) + ((Board.COLUMN_COUNT / 2) * 2)) * 4;

        /// <summary>
        /// Creates a list of SimulateBoar-objects so that every element in that list is the result of a possible shift operation.
        /// </summary>
        /// <param name="currentBoard">The state of the Board where the shift operations are based on.</param>
        /// <returns>A List with SimulatedBoards.</returns>
        internal static List<SimulatedBoard> GeneratePossibleBoards(Board currentBoard)
        {
            var possibleBoards = new List<SimulatedBoard>(MAX_NUMBER_OF_SHIFTS);

            //Iterate over the rows.
            for (int i = 1; i < Board.ROW_COUNT; i += 2)
            {
                //The left column for insertion
                if (IsValidShiftPosition(currentBoard, i, 0))
                    AddAllShiftRotations(possibleBoards, currentBoard, i, 0);

                //The right column for insertion
                if (IsValidShiftPosition(currentBoard, i, Board.COLUMN_COUNT - 1))
                    AddAllShiftRotations(possibleBoards, currentBoard, i, Board.COLUMN_COUNT - 1);
            }

            //Iterate over the columns.
            for (int j = 1; j < Board.COLUMN_COUNT; j += 2)
            {
                //The upper row for insertion
                if (IsValidShiftPosition(currentBoard, 0, j))
                    AddAllShiftRotations(possibleBoards, currentBoard, 0, j);

                //The lower row for insertion
                if (IsValidShiftPosition(currentBoard, Board.ROW_COUNT - 1, j))
                    AddAllShiftRotations(possibleBoards, currentBoard, Board.ROW_COUNT - 1, j);
            }

            Debug.Assert(possibleBoards.Capacity == MAX_NUMBER_OF_SHIFTS && (possibleBoards.Count == MAX_NUMBER_OF_SHIFTS || possibleBoards.Count + 4 == MAX_NUMBER_OF_SHIFTS));
            return possibleBoards;
        }

        static bool IsValidShiftPosition(Board currentBoard, int shiftPositionRowIndex, int shiftPositionColumnIndex)
        {
            return currentBoard.ShiftCard.RowIndex != shiftPositionRowIndex
                && currentBoard.ShiftCard.ColumnIndex != shiftPositionColumnIndex;
        }

        static void AddAllShiftRotations(List<SimulatedBoard> container, Board currentBoard, int shiftCardPositionRowIndex, int shiftCardPositionColumnIndex)
        {
            foreach (Rotation aRotation in Enum.GetValues(typeof(Rotation)))
            {
                var aSimulatedBoard = new SimulatedBoard(currentBoard, shiftCardPositionRowIndex, shiftCardPositionColumnIndex, aRotation);
                container.Add(aSimulatedBoard);
            }
        }
    }
}