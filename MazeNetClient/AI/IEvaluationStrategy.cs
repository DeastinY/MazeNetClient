using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeNetClient.AI
{
    interface IEvaluationStrategy
    {
        Tuple<Move, float> GetBestMove(SimulatedBoard board);
    }
}
