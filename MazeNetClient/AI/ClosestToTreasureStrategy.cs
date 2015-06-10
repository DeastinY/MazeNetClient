using MazeNetClient.Game;
using System;
using System.Linq;

namespace MazeNetClient.AI
{
    /// <summary>
    /// This class implements the IRatingStrategy-interface.
    /// Its tactic it is, to get as close to the treasure as possible.
    /// </summary>
    class ClosestToTreasureStrategy : IRatingStrategy
    {
        public Tuple<Move, float> GetBestMove(SimulatedBoard board)
        {
            /* Try to get to one of the diagonal fields around the treasure.
             *  d1      d2
             *      t   
             *  d3      d4
             */
            var reachableFields = board.GetReachableFields(board.PlayerPositionRowIndex, board.PlayerPositionColumnIndex);
            var targetField = board.FirstOrDefault(f => f.HasTreasure(Board.Current.TreasureTarget));
            if (reachableFields.Count == 0 || targetField == null)
            {
                var move = Move.Create(board, board.PlayerPositionRowIndex, board.PlayerPositionColumnIndex);
                return Tuple.Create(move, 0.0f);
            }
            else
            {
                Field closestField = null;
                float smallestDistance = float.MaxValue;

                foreach (var aReachableField in reachableFields)
                {
                    float currentDistance = Distance(aReachableField, targetField);
                    if (currentDistance < smallestDistance)
                    {
                        smallestDistance = currentDistance;
                        closestField = aReachableField;
                    }
                }

                var move = Move.Create(board, closestField.RowIndex, closestField.ColumnIndex);
                return Tuple.Create(move, 1.0f / smallestDistance);
            }
        }

        float Distance(Field first, Field second)
        {
            return (float)
                Math.Sqrt(Math.Pow(Math.Abs(first.RowIndex - second.RowIndex), 2) +
                Math.Pow(Math.Abs(first.ColumnIndex - second.ColumnIndex), 2));
        }
    }
}