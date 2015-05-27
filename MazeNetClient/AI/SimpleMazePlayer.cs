using MazeNetClient.Game;
using MazeNetClient.XSDGenerated;
using System.Diagnostics;
using System.Linq;

namespace MazeNetClient.AI
{
    class SimpleMazePlayer : IMazePlayer
    {
        public Move PlayNextMove(Board currentBoard, int playerRowIndex, int playerColumnIndex, treasureType nextTreasure)
        {
            var reachableFields = currentBoard.GetReachableFields(playerRowIndex, playerColumnIndex);
            var fieldWithNextTreasure = reachableFields.FirstOrDefault(f => f.ContainsTreasure && f.Treasure == nextTreasure);

            if (fieldWithNextTreasure != null)
            {
                int shiftRow, shiftColumn;
                GetShiftPosition(out shiftRow, out shiftColumn, currentBoard, fieldWithNextTreasure.RowIndex, fieldWithNextTreasure.ColumnIndex);
                Logger.WriteLine("FOUND TREASURE: ");
                return new Move(fieldWithNextTreasure.RowIndex, fieldWithNextTreasure.ColumnIndex, shiftRow, shiftColumn, "//TODO: Hier muss eine rotation rein?");
            }
            else
            {
                int PRI = playerRowIndex;
                int PCI = playerColumnIndex;
                if (reachableFields.Count != 0)
                {
                    var f = reachableFields[reachableFields.Count - 1];
                    foreach (var aField in reachableFields)
                    {
                        break;
                        var dist = System.Math.Abs(f.RowIndex - PRI) + System.Math.Abs(f.ColumnIndex - PCI);
                        var d2 = System.Math.Abs(aField.RowIndex - PRI) + System.Math.Abs(aField.ColumnIndex - PCI);
                        if (d2 > dist)
                            f = aField;
                    }
                    PRI = f.RowIndex;
                    PCI = f.ColumnIndex;
                }

                int shiftRow, shiftColumn;
                //GetShiftPosition(out shiftRow, out shiftColumn, currentBoard, PRI, PCI);
                shiftColumn = 0;
                shiftRow = GetShiftRowIndex(currentBoard, playerRowIndex, PRI);
                Logger.WriteLine(shiftRow.ToString());

                return new Move(PRI, PCI, shiftRow, shiftColumn, "//TODO: Hier muss eine rotation rein?");
            }
        }


        private void GetShiftPosition(out int row, out int column, Board currentBoard, int prohibitedRow, int prohibitedColumn)
        {
            for (int i = 1; i < Board.ROW_COUNT; i += 2)
            {
                if (currentBoard.ForbiddenShiftRow != i
                    && currentBoard.ForbiddenShiftColumn != 1)
                {
                    if (prohibitedRow != i
                        && prohibitedColumn != 1)
                    {
                        row = i;
                        column = 0;
                        return;
                    }
                }

                if (currentBoard.ForbiddenShiftRow != i
                    && currentBoard.ForbiddenShiftColumn != Board.COLUMN_COUNT - 1)
                {
                    if (prohibitedRow != i
                        && prohibitedColumn != Board.COLUMN_COUNT - 1)
                    {
                        row = i;
                        column = Board.COLUMN_COUNT - 1;
                        return;
                    }
                }
            }


            Debug.Assert(false, "This code shouldn't be reached, because we should always find a valid position where to place the shift card.");
            row = column = -1;
        }


        private int GetShiftRowIndex(Board currentBoard, int prohibitedRowBegin, int prohibitedRowEnd)
        {
            for (int r = 1; r < Board.ROW_COUNT; r += 2)
            {
                if (r != currentBoard.ShiftCard.RowIndex && (r < prohibitedRowBegin || r > prohibitedRowEnd))
                    return r;
            }

            Debug.Assert(false);
            return -1;
        }
    }
}