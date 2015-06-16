using MazeNetClient.Game;
using MazeNetClient.XSDGenerated;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MazeNetClient.AI
{
    static class Evaluator
    {
        internal static Move GetBestMove(Board actualBoard)
        {
            Debug.Assert(actualBoard == Board.Current);

            var ourPlayer = actualBoard.PlayerId;

            //First generate all possible shift operations that we can make within our draw.
            var possibleBoards = ShiftSimulator.GeneratePossibleBoards(actualBoard, actualBoard.ShiftCard, actualBoard.ForbiddenShiftRow, actualBoard.ForbiddenShiftColumn);

            //Second ensure that we filter all possibleBoards after that our next enemy player can reach his last treasure.
            //We filter only possibleBoards when we know that our enemy needs just one more treasure (and therefore his home-position).
            //And we filter only when we can't reach our last treasure within this draw.

            int nextPlayer = ourPlayer.FollowingPlayer();
            var nextPlayersMissingTreasuresCount = actualBoard.TreasuresToGo.First(ttg => ttg.player == nextPlayer).treasures;
            if (nextPlayersMissingTreasuresCount == 1)
            {
                BestShiftStrategy anyStrategy = new BestShiftStrategy(i => i.First());
                var ourPlayersMissingTreasuresCount = actualBoard.TreasuresToGo.First(ttg => ttg.player == ourPlayer).treasures;
                if ((ourPlayersMissingTreasuresCount != 1) || (TryGetTreasureFindingMove(possibleBoards, anyStrategy) == null))
                {
                    FilterPossibleShiftedBoardsSuchThatGivenEnemyCantReachHisLastTreasure(possibleBoards, nextPlayer);

                    //When we reach this line of code and possibleBoards is empty than we know, that we have lost and our following player won the game!
                    //Just regenerate the possible boards. Maybe we will find one more treasure before we lose.
                    possibleBoards = ShiftSimulator.GeneratePossibleBoards(actualBoard, actualBoard.ShiftCard, actualBoard.ForbiddenShiftRow, actualBoard.ForbiddenShiftColumn);
                }
            }

            Debug.Assert(possibleBoards.Count != 0, "The number of possible shift operations should never become zero!");

            //When we find a move that leads us to our treasure, we will return that move.
            var aFindingMove = TryGetTreasureFindingMove(possibleBoards, BestShiftStrategies.MinimizeTotalNumberOfReachableTreasures);
            if (aFindingMove != null)
            {
                Logger.WriteLine("Found our next treasure!");
                return aFindingMove;
            }
            else Logger.WriteLine("Didn't find our next treasure!");


            //TODO: What do we want to do now?

            //Let the OneStepAheadStrategy return a best move for each possible shift operation
            var ratedMoves = new List<Tuple<Move, float>>(possibleBoards.Count);
            var aStrategy = new OneStepAheadStrategy();
            foreach (var aPossibleBoard in possibleBoards)
            {
                var moveTuple = aStrategy.GetBestMove(aPossibleBoard);
                ratedMoves.Add(moveTuple);
            }

            //The OneStepAheadStrategies can return tuples with Moves equal to null. Remove those!
            for (int i = 0; i != ratedMoves.Count; )
            {
                if (ratedMoves[i].Item1 == null) ratedMoves.RemoveAt(i);
                else ++i;
            }

            if (ratedMoves.Count == 0) //The OneStepAheadStrategy did only return tuples with Moves equal to null.
            {
                var shiftedBoard = BestShiftStrategies.MinimizeTotalNumberOfReachableTreasuresForBestPlayers(possibleBoards);
                var tuple = new ClosestToTreasureStrategy().GetBestMove(shiftedBoard);
                return tuple.Item1;
            }
            else //Iterate over the returned moves of the OneStepAheadStrategy and pick the best rated.
            {
                Tuple<Move, float> bestRatedMove = ratedMoves[0];

                for (int i = 1; i < ratedMoves.Count; ++i)
                {
                    if (ratedMoves[i].Item2 > bestRatedMove.Item2)
                        bestRatedMove = ratedMoves[i];
                }

                return bestRatedMove.Item1;
            }
        }

        /// <summary>
        /// Filters those ShiftedBoards in the given list of possibleShiftedBoards on that our player can reach his treasure.
        /// When there are any filtered ShiftedBoards on that our player can reach his treasure, we let the given bestShiftFinder
        /// choose the one that we will return.
        /// </summary>
        /// <param name="possibleShiftedBoards">The given list of possible shifted boards.</param>
        /// <param name="bestShiftFinder">The BestShiftStrategy that chooses a ShiftedBoard of the list of those on which we can find our treasure.</param>
        /// <returns>A move with that we will find our next treasure or null if there is no possibleShiftedBoard on that we can find our treasure.</returns>
        static Move TryGetTreasureFindingMove(IEnumerable<ShiftedBoard> possibleShiftedBoards, BestShiftStrategy bestShiftFinder)
        {
            var findingMoves = new List<ShiftedBoard>();
            var treasureTarget = Board.Current.TreasureTarget;

            foreach (var aBoard in possibleShiftedBoards)
            {
                //Get all fields that our player can reach on that shifted board.
                var reachableFields = aBoard.GetReachableFields(aBoard.PlayerPositionRowIndex, aBoard.PlayerPositionColumnIndex);
                //When any of those reachable fields contains our treasure, add it to the findingMoves.
                if (reachableFields.Any(fi => fi.HasTreasure(treasureTarget)))
                {
                    findingMoves.Add(aBoard);
                }
            }

            if (findingMoves.Count == 0)
            {
                return null;
            }

            var findingMove = bestShiftFinder(findingMoves);
            Debug.Assert(findingMove.GetReachableFields(findingMove.PlayerPositionRowIndex, findingMove.PlayerPositionColumnIndex).Count(f =>
                f.RowIndex == findingMove.TreasureTargetRowIndex && f.ColumnIndex == findingMove.TreasureTargetColumnIndex) == 1);
            return Move.Create(findingMove, findingMove.TreasureTargetRowIndex, findingMove.TreasureTargetColumnIndex);
        }

        /// <summary>
        /// Removes each entry from the given list of possible shifted boards that would, if we applied one of them,
        /// left the given enemy player reach his treasure after our move.
        /// </summary>
        /// <param name="possibleShiftedBoards">The list of possible shifted boards. After this function the list should contain less elements.</param>
        /// <param name="enemyPlayerId">The id of the enemy player.</param>
        static void FilterPossibleShiftedBoardsSuchThatGivenEnemyCantReachHisLastTreasure(List<ShiftedBoard> possibleShiftedBoards, int enemyPlayerId)
        {
            Debug.Assert(enemyPlayerId != Board.Current.PlayerId, "You try to harm yourself!");
            Debug.Assert(enemyPlayerId == 1 || enemyPlayerId == 2 || enemyPlayerId == 3 || enemyPlayerId == 4, "The enemyPlayerId has an unexpected value: " + enemyPlayerId);

            Debug.Assert((Board.Current.TreasuresToGo.First(ttg => ttg.player == Board.Current.PlayerId).treasures > 1) //Either we still need more than one treasure,
                || (TryGetTreasureFindingMove(possibleShiftedBoards, new BestShiftStrategy(i => i.First())) == null), //or we can't find our treasure within this draw.
                "Do not call this function when our player only needs one treasure and you could find that within this move!");

            Debug.Assert(Board.Current.TreasuresToGo.First(t => t.player == enemyPlayerId).treasures == 1,
                "The specified enemy with id " + enemyPlayerId + " needs more than 1 treasure! This is an invalid state for this function!");

            //When the nextPlayer misses only one treasure, it must be his home position
            treasureType nextPlayersTreasureTarget = (treasureType)Enum.Parse(typeof(treasureType), "Start0" + enemyPlayerId, false);
            Debug.Assert(nextPlayersTreasureTarget == treasureType.Start01 || nextPlayersTreasureTarget == treasureType.Start02
                || nextPlayersTreasureTarget == treasureType.Start03 || nextPlayersTreasureTarget == treasureType.Start04);


            var possibleMovesForOurPlayer = new List<SimulatedMoveResult>(possibleShiftedBoards.Count);
            //A mapping from a possible shift operation to a bool whether we can use that shift operation such that the next player wont find his treasure.
            Dictionary<ShiftedBoard, bool> canUseThisBoard = new Dictionary<ShiftedBoard, bool>(possibleShiftedBoards.Count);

            foreach (var aPossibleShiftedBoard in possibleShiftedBoards)
            {
                //Create a SimulatedMoveResult for each possible shifted board. We do not move our player because it does not make any difference for the enemy where our player stands.
                possibleMovesForOurPlayer.Add(new SimulatedMoveResult(aPossibleShiftedBoard, Board.Current.PlayerId, aPossibleShiftedBoard.PlayerPositionRowIndex, aPossibleShiftedBoard.PlayerPositionColumnIndex, null));
                //Add each possible shift operation and set the bool to true because later we will set it to false if the next player can reach his treasure.
                canUseThisBoard.Add(aPossibleShiftedBoard, true);
            }

            foreach (SimulatedMoveResult aPossibleMoveForOurPlayer in possibleMovesForOurPlayer)
            {
                //Generate the possible shift operations that the next player can do.
                var possibleShiftOperationsForTheNextPlayer = ShiftSimulator.GeneratePossibleBoards(aPossibleMoveForOurPlayer, aPossibleMoveForOurPlayer.KickedField, aPossibleMoveForOurPlayer.ForbiddenShiftPositionRowIndex, aPossibleMoveForOurPlayer.ForbiddenShiftPositionColumnIndex);

                bool elminiatedThatPossibleMoveForOurPlayer = false;

                for (int i = 0; i != possibleShiftOperationsForTheNextPlayer.Count && !elminiatedThatPossibleMoveForOurPlayer; ++i)
                {
                    //Generate the possible moves that the next player can do on the current possible shift operation.
                    var possibleMovesForTheNextPlayer = SimulatedMoveResult.CreateAllPossibleMoves(possibleShiftOperationsForTheNextPlayer[i], enemyPlayerId, aPossibleMoveForOurPlayer);

                    foreach (var aPossibleMoveForTheNextPlayer in possibleMovesForTheNextPlayer)
                    {
                        if (aPossibleMoveForTheNextPlayer.FoundTreasure(nextPlayersTreasureTarget))
                        {
                            //When the next player can reach his treasure after we did that possibleMoveForOurPlayer, mark that possibleMoveForOurPlayer as unusable.

                            Debug.Assert(canUseThisBoard[aPossibleMoveForOurPlayer.UnderlyingShiftedBoard]);
                            canUseThisBoard[aPossibleMoveForOurPlayer.UnderlyingShiftedBoard] = false;
                            elminiatedThatPossibleMoveForOurPlayer = true;
                            break;
                        }
                    }
                }
            }

            possibleShiftedBoards.Clear();
            possibleShiftedBoards.AddRange(canUseThisBoard.Where(pair => pair.Value).Select(pair => pair.Key));
        }
    }
}