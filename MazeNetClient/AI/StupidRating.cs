using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeNetClient.Game;

namespace MazeNetClient.AI
{
    class StupidRating :IRatingStrategy
    {
        private SimulatedBoard board;
        public Tuple<Move, float> GetBestMove(SimulatedBoard board, List<Field> moves)
        {
            this.board = board;
            Field bestField = null;
            float bestRating = 0f;

            foreach(Field f in moves)
            {
                float rating = this.RateMove(f);

                if(rating>bestRating)
                {
                    bestField = f;
                    bestRating = rating;
                }
            }

            Move move = new Move(bestField.RowIndex, bestField.ColumnIndex,
                board.ShiftPositionRowIndex, board.ShiftPositionColumnIndex, board.ShiftCardRotation);
            return Tuple.Create(move, bestRating);
        }

        private float RateMove(Field move)
        {
            Random r = new Random();
            return (float) r.NextDouble();
        }
    }
}
