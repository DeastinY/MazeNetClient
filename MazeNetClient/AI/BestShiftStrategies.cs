using MazeNetClient.Game;
using System.Collections.Generic;
using System.Linq;

namespace MazeNetClient.AI
{
    /// <summary>
    /// Describes an interface for finding the best shift operations in a given list of possible shift operations.
    /// This interface does not focus on finding the players treasure!
    /// It assumes that either each of the given shifts leads to the treasure target or none of them does.
    /// </summary>
    /// <param name="possibleShiftOperations">A sequence of possible shift operations.</param>
    /// <returns>A subset from the list, describing the shift operations that all satisfy the given criteria the most.</returns>
    internal delegate List<ShiftedBoard> BestShiftStrategy(IEnumerable<ShiftedBoard> possibleShiftOperations);

    /// <summary>
    /// A static class, containing a set of BestShiftStrategy.
    /// </summary>
    static class BestShiftStrategies
    {
        /// <summary>
        /// Describes a BestShiftStrategy with the goal to minimize the number of treasures that the other players can reach.
        /// But this algorithm just takes care about the players thats amount of missing treasures is minimum.
        /// </summary>
        /// <param name="possibleShiftOperations"></param>
        /// <returns></returns>
        internal static List<ShiftedBoard> MinimizeTotalNumberOfReachableTreasuresForBestPlayers(IEnumerable<ShiftedBoard> possibleShiftOperations)
        {
            var otherPlayersTreasuresToGo = Board.Current.TreasuresToGo.Where(t => t.player != Board.Current.PlayerId).ToArray();
            if (otherPlayersTreasuresToGo.Length == 0)
            {
                //TODO: During a test game with one enemy player I reached this code! But that shouldn't have happened, because we were two player!
                Logger.WriteLine("You call MinimizeTotalNumberOfReachableTreasuresForBestPlayers with a list of TreasuresToGo with only the treasures of our player!");
                Logger.WriteLine("Because there is no way to work correct for MinimizeTotalNumberOfReachableTreasuresForBestPlayers, we just return the complete list!");

                return new List<ShiftedBoard>(possibleShiftOperations);
            }

            int minimalNumberOfMissingTreasures = otherPlayersTreasuresToGo.Min(t => t.treasures);
            var bestEnemyPlayers = otherPlayersTreasuresToGo.Where(t => t.treasures == minimalNumberOfMissingTreasures).Select(t => t.player);


            int minNumberOfReachableTreasures = int.MaxValue;
            var shiftedBoardsWithMinimumNumberOfReachableTreasures = new List<ShiftedBoard>();

            foreach (var aBoard in possibleShiftOperations)
            {
                int currentNumberOfReachableTreasures = bestEnemyPlayers.Sum(p => aBoard.GetNumberOfReachableTreasures(p));

                if (currentNumberOfReachableTreasures < minNumberOfReachableTreasures)
                {
                    minNumberOfReachableTreasures = currentNumberOfReachableTreasures;
                    shiftedBoardsWithMinimumNumberOfReachableTreasures.Clear();
                }
                if (currentNumberOfReachableTreasures == minNumberOfReachableTreasures)
                {
                    shiftedBoardsWithMinimumNumberOfReachableTreasures.Add(aBoard);
                }
            }

            return shiftedBoardsWithMinimumNumberOfReachableTreasures;
        }

        /// <summary>
        /// Returns a list such that each element has the maximum number of reachable treasures that our player stil might need.
        /// </summary>
        /// <param name="possibleShiftBoards"></param>
        /// <returns></returns>
        internal static List<ShiftedBoard> MaximizeNumberOfReachableTreasuresForOurPlayer(IEnumerable<ShiftedBoard> possibleShiftBoards)
        {
            var ourPlayer = Board.Current.PlayerId;
            int maxNumberOfReachableTreasures = 0;
            var shiftedBoardsWithMaximumNumberOfReachableTreasures = new List<ShiftedBoard>();

            foreach (var aBoard in possibleShiftBoards)
            {
                int currentNumberOfReachableTreasures = aBoard.GetNumberOfReachableTreasures(ourPlayer);
                if (currentNumberOfReachableTreasures > maxNumberOfReachableTreasures)
                {
                    maxNumberOfReachableTreasures = currentNumberOfReachableTreasures;
                    shiftedBoardsWithMaximumNumberOfReachableTreasures.Clear();
                }
                if (currentNumberOfReachableTreasures == maxNumberOfReachableTreasures)
                {
                    shiftedBoardsWithMaximumNumberOfReachableTreasures.Add(aBoard);
                }
            }

            return shiftedBoardsWithMaximumNumberOfReachableTreasures;
        }

        internal static List<ShiftedBoard> GetShiftedBoardsThatAlsoReachGivenTreasure(IEnumerable<ShiftedBoard> possibleShiftedBoards, XSDGenerated.treasureType givenTreasure)
        {
            var reachingShiftedBoards = new List<ShiftedBoard>();
            foreach (var aShiftedBoard in possibleShiftedBoards)
            {
                var reachableFields = aShiftedBoard.GetReachableFields(aShiftedBoard.PlayerPositionRowIndex, aShiftedBoard.PlayerPositionColumnIndex);
                if (reachableFields.Any(f => f.HasTreasure(givenTreasure)))
                {
                    reachingShiftedBoards.Add(aShiftedBoard);
                }
            }
            return reachingShiftedBoards;
        }
    }
}