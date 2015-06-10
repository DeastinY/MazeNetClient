using MazeNetClient.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeNetClient.AI
{
    static class Evaluator
    {
        internal static Move GetBestMove(List<SimulatedBoard> possibleBoards)
        {
            var aFindingMove = TryGetTreasureFindingMove(possibleBoards, BestShiftStrategies.MinimizeTotalNumberOfReachableTreasures);
            if (aFindingMove != null)
            {
                return aFindingMove;
            }

            var strategyList = new List<IRatingStrategy>();
            strategyList.Add(new ClosestToTreasureStrategy());

            var ratedMoves = new List<Tuple<Move, float>>(possibleBoards.Count * strategyList.Count);

            foreach (var aStrategy in strategyList)
            {
                foreach (SimulatedBoard sb in possibleBoards)
                {
                    Tuple<Move, float> temp = aStrategy.GetBestMove(sb);
                    ratedMoves.Add(temp);
                }
            }


            Tuple<Move, float> bestRatedMove = ratedMoves[0];

            for (int i = 1; i < ratedMoves.Count; ++i)
            {
                if (ratedMoves[i].Item2 > bestRatedMove.Item2)
                    bestRatedMove = ratedMoves[i];
            }

            return bestRatedMove.Item1;
        }

        static Move TryGetTreasureFindingMove(IEnumerable<SimulatedBoard> simulatedBoards, BestShiftStrategy bestShiftFinder)
        {
            var findingMoves = new List<SimulatedBoard>();

            foreach (var aBoard in simulatedBoards)
            {
                //Get all fields that our player can reach on that simulated board.
                var reachableFields = aBoard.GetReachableFields(aBoard.PlayerPositionRowIndex, aBoard.PlayerPositionColumnIndex);
                //When any of those reachable fields contains our treasure, add it to the findingMoves.
                if (reachableFields.Any(fi => fi.HasTreasure(Board.Current.TreasureTarget)))
                {
                    findingMoves.Add(aBoard);
                }
            }

            if (findingMoves.Count == 0)
            {
                return null;
            }

            var findingMove = bestShiftFinder(findingMoves);
            System.Diagnostics.Debug.Assert(findingMove.GetReachableFields(findingMove.PlayerPositionRowIndex, findingMove.PlayerPositionColumnIndex).Count(f =>
                f.RowIndex == findingMove.TreasureTargetRowIndex && f.ColumnIndex == findingMove.TreasureTargetColumnIndex) == 1);
            return Move.Create(findingMove, findingMove.TreasureTargetRowIndex, findingMove.TreasureTargetColumnIndex);
        }
    }
}