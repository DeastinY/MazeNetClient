using MazeNetClient.Game;
using System;
using System.Collections.Generic;

namespace MazeNetClient.AI
{
    static class Evaluator
    {
        internal static Move GetBestMove(List<SimulatedBoard> container, IRatingStrategy strategy)
        {
            var moves = new List<Tuple<Move, float>>(container.Count);

            foreach (SimulatedBoard sb in container)
            {
                List<Field> posFields = sb.GetReachableFields(sb.PlayerPositionRowIndex, sb.PlayerPositionColumnIndex);
                Tuple<Move, float> temp = strategy.GetBestMove(sb, posFields);
                moves.Add(temp);
            }

            Tuple<Move, float> bestMove = moves[0];

            for (int i = 1; i < moves.Count; ++i)
            {
                if (moves[i].Item2 > bestMove.Item2)
                    bestMove = moves[i];
            }

            return bestMove.Item1;
        }
    }
}