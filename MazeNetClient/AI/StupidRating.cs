using System;
using MazeNetClient.Game;
using System.Linq;
using System.Collections.Generic;

namespace MazeNetClient.AI
{
    class StupidRating : IRatingStrategy
    {
        public Tuple<Move, float> GetBestMove(SimulatedBoard board, List<Field> reachableFields)
        {
            float result = 0.0f;
            Move move = null;

            if (reachableFields.Count == 0)
            {
                move = new Move(board.PlayerPositionRowIndex, board.PlayerPositionColumnIndex,
                    board.ShiftPositionRowIndex, board.ShiftPositionColumnIndex, board.ShiftCardRotation);
            }
            else
            {
                Field newField = null;

                var foundTreasure = reachableFields.FirstOrDefault(fi => fi.HasTreasure(board.TreasureTarget));
                if (foundTreasure != null)
                {
                    newField = foundTreasure;
                    result = 1.0f;
                }
                else
                {
                    float maxDist = -1;
                    foreach (var aField in reachableFields)
                    {
                        var dist = Distance(aField, board.PlayerPositionRowIndex, board.PlayerPositionColumnIndex);
                        if (dist > maxDist)
                        {
                            maxDist = dist;
                            newField = aField;
                        }
                    }

                    result = 1.0f - 1.0f / reachableFields.Count;
                }

                move = new Move(newField.RowIndex, newField.ColumnIndex, board.ShiftPositionRowIndex, board.ShiftPositionColumnIndex, board.ShiftCardRotation);
            }
#warning HIER IST DER FEHLER; DISTANCE IST FALSCH.
            return Tuple.Create(move, result);
        }

        float Distance(Field field, int toRowIndex, int toColumnIndex)
        {
            return (float)
                Math.Sqrt(Math.Pow(Math.Abs(field.RowIndex - toRowIndex), 2) +
                Math.Pow(Math.Abs(field.ColumnIndex - toColumnIndex), 2));
        }
    }
}
