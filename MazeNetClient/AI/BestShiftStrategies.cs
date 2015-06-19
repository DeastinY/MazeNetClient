using MazeNetClient.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeNetClient.AI
{
    /// <summary>
    /// Describes an interface for finding the best shift operation in a given list of possible shift operations.
    /// This interface does not focus on finding the players treasure!
    /// It assumes that either each of the given shifts leads to the treasure target or none of them does.
    /// </summary>
    /// <param name="possibleShiftOperations">A sequence of possible shift operations.</param>
    /// <returns>An element from the list, describing the best shift operation.</returns>
    internal delegate ShiftedBoard BestShiftStrategy(IEnumerable<ShiftedBoard> possibleShiftOperations);

    /// <summary>
    /// A static class, containing a set of BestShiftStrategy.
    /// </summary>
    static class BestShiftStrategies
    {
        /// <summary>
        /// Describes a BestShiftStrategy with the goal, to minimize the number of treasures that the other players can reach.
        /// </summary>
        /// <param name="possibleShiftOperations">A sequence of possible shift operations.</param>
        /// <returns>The possible shift operation that minimizes the number of reachable treasures for the other players.</returns>
        internal static ShiftedBoard MinimizeTotalNumberOfReachableTreasures(IEnumerable<ShiftedBoard> possibleShiftOperations)
        {
            ShiftedBoard bestShiftOperation = null;
            int minNumberOfReachableTreasures = int.MaxValue;
            var enemyPlayers = Board.Current.GetEnemyPlayers();

            foreach (var aBoard in possibleShiftOperations)
            {
                int currentNumberOfReachableTreasures = 0;
                foreach (var anEnemy in enemyPlayers)
                {
                    currentNumberOfReachableTreasures += aBoard.GetNumberOfReachableTreasures(anEnemy);
                }

                if (currentNumberOfReachableTreasures < minNumberOfReachableTreasures)
                {
                    minNumberOfReachableTreasures = currentNumberOfReachableTreasures;
                    bestShiftOperation = aBoard;
                }
            }

#warning TODO: Anhand von Informationen, zum Beispiel daran, wie sehr sich die neue Anzahl an reachable treasures von der alten unterscheidet, könnte man die Qualität dieser Methode beurteilen.
            return bestShiftOperation;
        }

        /// <summary>
        /// Describes a BestShiftStrategy with the goal to minimize the number of treasures that the other players can reach.
        /// But this algorithm just takes care about the players thats amount of missing treasures is minimum.
        /// </summary>
        /// <param name="possibleShiftOperations"></param>
        /// <returns></returns>
        internal static ShiftedBoard MinimizeTotalNumberOfReachableTreasuresForBestPlayers(IEnumerable<ShiftedBoard> possibleShiftOperations)
        {
            var otherPlayersTreasuresToGo = Board.Current.TreasuresToGo.Where(t => t.player != Board.Current.PlayerId).ToArray();
            if (otherPlayersTreasuresToGo.Length == 0)
            {
                //TODO: During a test game with one enemy player I reached this code! But that shouldn't have happened, because we were two player!
                Logger.WriteLine("You call MinimizeTotalNumberOfReachableTreasuresForBestPlayers with a list of TreasuresToGo with only the treasures of our player!");
                Logger.WriteLine("Because there is no way to work correct for MinimizeTotalNumberOfReachableTreasuresForBestPlayers, we just return a random possibleMove!");
                int randomIndex = new Random().Next(possibleShiftOperations.Count());
                return possibleShiftOperations.ElementAt(randomIndex);
            }

            int minimalNumberOfMissingTreasures = otherPlayersTreasuresToGo.Min(t => t.treasures);
            var bestEnemyPlayers = otherPlayersTreasuresToGo.Where(t => t.treasures == minimalNumberOfMissingTreasures).Select(t => t.player);

            ShiftedBoard bestShiftOperation = null;
            int minNumberOfReachableTreasures = int.MaxValue;

            foreach (var aBoard in possibleShiftOperations)
            {
                int currentNumberOfReachableTreasures = 0;
                foreach (var aPlayer in bestEnemyPlayers)
                    currentNumberOfReachableTreasures += aBoard.GetNumberOfReachableTreasures(aPlayer);

                if (currentNumberOfReachableTreasures < minNumberOfReachableTreasures)
                {
                    minNumberOfReachableTreasures = currentNumberOfReachableTreasures;
                    bestShiftOperation = aBoard;
                }
            }

#warning TODO: AUCH HIER: Anhand von Informationen, zum Beispiel daran, wie sehr sich die neue Anzahl an reachable treasures von der alten unterscheidet, könnte man die Qualität dieser Methode beurteilen.
            return bestShiftOperation;
        }

        internal static ShiftedBoard MinimizeOpenPaths(IEnumerable<ShiftedBoard> possibleShiftOperations)
        {
#warning TODO: Auch hier könnte man die anzahl der open paths im Board vergleichen mit denen des SimulatedBoards, die diese Methode zurückgibt und anhand der differenz diese Methode bewerten!

            ShiftedBoard minNumOfOpenPathsBoard = null;
            int minNumOfOpenPaths = int.MaxValue;

            foreach (var aPossibleShiftOperation in possibleShiftOperations)
            {
                var numOpenPaths = aPossibleShiftOperation.GetNumberOfOpenPaths();
                if (numOpenPaths < minNumOfOpenPaths)
                {
                    minNumOfOpenPaths = numOpenPaths;
                    minNumOfOpenPathsBoard = aPossibleShiftOperation;
                }
            }

            return minNumOfOpenPathsBoard;
        }

        /// <summary>
        /// Returns the ShiftedBoard with the maximum number of fields that our player can reach on.
        /// </summary>
        /// <param name="possibleShiftOperations"></param>
        /// <returns></returns>
        internal static ShiftedBoard MaximizeNumberOfReachableFields(IEnumerable<ShiftedBoard> possibleShiftOperations)
        {
            ShiftedBoard maxNumOfReachableFieldsBoard = null;
            int maxNumOfReachableFields = 0;

            foreach (var aPossibleShiftOperation in possibleShiftOperations)
            {
                var numOfReachableFields = aPossibleShiftOperation.GetReachableFields(aPossibleShiftOperation.PlayerPositionRowIndex, aPossibleShiftOperation.PlayerPositionColumnIndex).Count;
                if (numOfReachableFields > maxNumOfReachableFields)
                {
                    maxNumOfReachableFields = numOfReachableFields;
                    maxNumOfReachableFieldsBoard = aPossibleShiftOperation;
                }
            }

            return maxNumOfReachableFieldsBoard;
        }
    }
}