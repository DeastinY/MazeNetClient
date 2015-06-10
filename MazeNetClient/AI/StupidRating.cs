using System;
using MazeNetClient.Game;
using System.Linq;
using System.Collections.Generic;

namespace MazeNetClient.AI
{
    class StupidRating : IRatingStrategy
    {
        public Tuple<Move, float> GetBestMove(SimulatedBoard board)
        {
            float result = 0.0f;
            Move move = null;
            var reachableFields = board.GetReachableFields(board.PlayerPositionRowIndex, board.PlayerPositionColumnIndex);

            if (reachableFields.Count == 0)
            {
                move = Move.Create(board, board.PlayerPositionRowIndex, board.PlayerPositionColumnIndex);
            }
            else
            {
                Field newField = null;

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

                result = 1.0f - 1.0f / maxDist;

                move = new Move(newField.RowIndex, newField.ColumnIndex, board.ShiftPositionRowIndex, board.ShiftPositionColumnIndex, board.ShiftCardRotation);
            }

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