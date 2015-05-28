using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeNetClient.Game;

namespace MazeNetClient.AI
{
    interface IRatingStrategy
    {
        Tuple<Move, float> GetBestMove(SimulatedBoard board, List<Field> moves);
    }
}
