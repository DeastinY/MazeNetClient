using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeNetClient.AI
{
    class StupidEvaluation :IEvaluationStrategy
    {
        public Tuple<Move, float> GetBestMove(SimulatedBoard board)
        {
            Move move = new Move(board.PlayerPositionRowIndex, board.PlayerPositionColumnIndex,
                board.ShiftPositionRowIndex, board.ShiftPositionColumnIndex, board.ShiftCardRotation);
            float result = 0.5f;

            return Tuple.Create(move, result);
        }
    }
}
