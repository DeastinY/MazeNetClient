using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeNetClient.AI
{
    static class Evaluator
    {
        internal static Move GetBestMove(List<SimulatedBoard> container, IEvaluationStrategy strategy)
        {
            var moves = new List<Tuple<Move, float>>(container.Count);

            foreach (SimulatedBoard sb in container)
            {
                Tuple<Move, float> temp = strategy.GetBestMove(sb);
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
