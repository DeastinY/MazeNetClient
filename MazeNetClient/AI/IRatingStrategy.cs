using System;

namespace MazeNetClient.AI
{
    interface IRatingStrategy
    {
        Tuple<Move, float> GetBestMove(SimulatedBoard board);
    }
}