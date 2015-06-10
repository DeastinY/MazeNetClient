﻿using MazeNetClient.Game;
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
    internal delegate SimulatedBoard BestShiftStrategy(IEnumerable<SimulatedBoard> possibleShiftOperations);

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
        internal static SimulatedBoard MinimizeTotalNumberOfReachableTreasures(IEnumerable<SimulatedBoard> possibleShiftOperations)
        {
            SimulatedBoard bestShiftOperation = null;
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
        internal static SimulatedBoard MinimizeTotalNumberOfReachableTreasuresForBestPlayers(IEnumerable<SimulatedBoard> possibleShiftOperations)
        {
            var otherPlayersTreasuresToGo = Board.Current.TreasuresToGo.Where(t => t.player != Board.Current.PlayerId).ToArray();
            if (otherPlayersTreasuresToGo.Length == 0)
            {
                Logger.WriteLine("You call MinimizeTotalNumberOfReachableTreasuresForBestPlayers with a list of TreasuresToGo with only the treasures of our player!");
                Logger.WriteLine("Because there is no way to work correct for MinimizeTotalNumberOfReachableTreasuresForBestPlayers, we just return the last index of the possibleMoves!");
                return possibleShiftOperations.Last();
            }

            int minimalNumberOfMissingTreasures = otherPlayersTreasuresToGo.Min(t => t.treasures);
            var bestEnemyPlayers = otherPlayersTreasuresToGo.Where(t => t.treasures == minimalNumberOfMissingTreasures).Select(t => t.player);

            SimulatedBoard bestShiftOperation = null;
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

        internal static SimulatedBoard MinimizeOpenPaths(IEnumerable<SimulatedBoard> possibleShiftOperations)
        {
#warning TODO: Auch hier könnte man die anzahl der open paths im Board vergleichen mit denen des SimulatedBoards, die diese Methode zurückgibt und anhand der differenz diese Methode bewerten!

            SimulatedBoard minNumOfOpenPathsBoard = null;
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
    }
}