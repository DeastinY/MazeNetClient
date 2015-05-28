using MazeNetClient.Game;
using System;
using System.Collections.Generic;

namespace MazeNetClient.AI
{
    interface IRatingStrategy
    {
        Tuple<Move, float> GetBestMove(SimulatedBoard board, List<Field> reachableFields);
    }
}