using MazeNetClient.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MazeNetClient.AI
{
    class OneStepAheadStrategy : IRatingStrategy
    {
        public Tuple<Move, float> GetBestMove(ShiftedBoard board)
        {
            int playerId = Board.Current.PlayerId;
            var possibleFirstLevelMoves = SimulatedMoveResult.CreateAllPossibleMoves(board, playerId, null);
            Debug.Assert(possibleFirstLevelMoves.All(smr => !smr.FoundTreasure(Board.Current.TreasureTarget)));

            List<SimulatedMoveResult> allNextLevelRecursionMoves = new List<SimulatedMoveResult>();
            foreach (var aPossibleFirstLevelMove in possibleFirstLevelMoves)
            {
                var nextMoves = GenerateNextMoves(aPossibleFirstLevelMove, playerId);
                allNextLevelRecursionMoves.AddRange(nextMoves);
            }

            var nextLevelFindingMoves = allNextLevelRecursionMoves.Where(m => m.FoundTreasure(Board.Current.TreasureTarget));
            var bestRatedMove = new Tuple<Move, float>(null, 0.0f);

            foreach (var aNextLevelFindingMove in nextLevelFindingMoves)
            {
                var ratedMove = GetRatedMove(aNextLevelFindingMove, playerId);
                if (ratedMove.Item2 > bestRatedMove.Item2)
                    bestRatedMove = ratedMove;
            }

            return bestRatedMove;
        }

        List<SimulatedMoveResult> GenerateNextMoves(SimulatedMoveResult from, int playerId)
        {
            var allShiftOperations = ShiftSimulator.GeneratePossibleBoards(from, from.KickedField, from.ForbiddenShiftPositionRowIndex, from.ForbiddenShiftPositionColumnIndex);

            List<SimulatedMoveResult> nextMoves = new List<SimulatedMoveResult>();
            foreach (var aShiftOperation in allShiftOperations)
            {
                nextMoves.AddRange(SimulatedMoveResult.CreateAllPossibleMoves(aShiftOperation, playerId, from));
            }

            return nextMoves;
        }

        Tuple<Move, float> GetRatedMove(SimulatedMoveResult nextLevelFindingMove, int playerId)
        {
            //The position of our player after we applied our next level move.
            var playerPositionAfterThisMove = nextLevelFindingMove.First(f => f.ContainsPlayer(playerId));

            //The position of our player after we applied our next level shift operation but haven't moved our player yet.
            int playerPositionBeforeThisMoveRowIndex = nextLevelFindingMove.UnderlyingShiftedBoard.PlayerPositionRowIndex;
            int playerPositionBeforeThisMoveColumnIndex = nextLevelFindingMove.UnderlyingShiftedBoard.PlayerPositionColumnIndex;

            var playerPositionBeforeThisMove = nextLevelFindingMove[playerPositionBeforeThisMoveRowIndex, playerPositionBeforeThisMoveColumnIndex];


#warning TODO: Entweder rowDiff und columnDiff nehmen
            //Make the path between these two different positions as short as possible!
            //var pathLength = nextLevelFindingMove.CalculatePathLength(playerPositionAfterThisMove, playerPositionBeforeThisMove);
            var rowDiff = Math.Abs(playerPositionAfterThisMove.RowIndex - playerPositionBeforeThisMoveRowIndex);
            var columnDiff = Math.Abs(playerPositionAfterThisMove.ColumnIndex - playerPositionBeforeThisMoveColumnIndex);
            var pathLength = rowDiff + columnDiff;


#warning TODO: Oder schauen, dass man auf einer zeile und spalte steht, die einen geraden index hat, sodass man sie nicht verschieben kann
            var firstLevelMove = nextLevelFindingMove.GetFirstMove();
            bool unshiftablePosition = firstLevelMove.NewPinPosRowIndex % 2 == 0 && firstLevelMove.NewPinPosColumnIndex % 2 == 0;


#warning TODO: Man kann den move entweder anhand der pathlength bewerten oder anhand der wahrscheinlichkeit, dass ein gegner da dazwischenfunkt


            return Tuple.Create(nextLevelFindingMove.GetFirstMove(), 1.0f / pathLength);
        }
    }
}