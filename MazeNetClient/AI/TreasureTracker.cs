using MazeNetClient.Game;
using MazeNetClient.XSDGenerated;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MazeNetClient.AI
{
    /// <summary>
    /// This class tracks the evolution of the treasures.
    /// It is an singleton that is accessible from all around the program but thats life time is limited to one game.
    /// Each time you start a new game you have to start a new treasure tracker.
    /// </summary>
    sealed class TreasureTracker
    {
        /// <summary>
        /// Creates and initializes a new instance of the type TreasureTracker.
        /// The constructor is marked private because this class is a singleton.
        /// </summary>
        private TreasureTracker()
        {
            m_foundTreasuresMap = new Dictionary<int, List<treasureType>>(4);
            m_foundTreasures = new List<treasureType>();
        }

        /// <summary>
        /// Gets the one and only instance of the type TreasureTracker.
        /// </summary>
        internal static TreasureTracker Instance { get; private set; }

        /// <summary>
        /// Start a new instance of the TreasureTracker.
        /// </summary>
        internal static void StartNewTracker()
        {
            Instance = new TreasureTracker();
        }

        /// <summary>
        /// Holds each value of the treasureType enum.
        /// </summary>
        private static readonly treasureType[] m_allTreasureTypes = (treasureType[])Enum.GetValues(typeof(treasureType));

        /// <summary>
        /// Describes a mapping from a player to a list of treasures that the player has already found.
        /// </summary>
        private Dictionary<int, List<treasureType>> m_foundTreasuresMap;

        /// <summary>
        /// Describes a list of all treasures that where found yet during this game.
        /// This member should be equal to the field TreasuresToGo in the class Board.
        /// </summary>
        private List<treasureType> m_foundTreasures;

        /// <summary>
        /// Describes the number of treasures that a player needs to find in the current game.
        /// This member will be initialized once at the beginning of a game. It depends on the number of players in the game.
        /// Even if a player disconnects during a game, the number of treasures that a player needs to find will not change.
        /// </summary>
        private int m_treasuresPerPlayer;

        /// <summary>
        /// Initialize once for each game the found treasures map depending on the initial number of players.
        /// </summary>
        void Initialize()
        {
            Debug.Assert(Board.HistoryLength == 1, "Ensure that you initialize the TreasureTracker with the first Board!");

            var players = Board.Current.TreasuresToGo.Select(t => t.player);
            m_treasuresPerPlayer = m_allTreasureTypes.Length / players.Count();
            foreach (var aPlayer in players)
            {
                m_foundTreasuresMap.Add(aPlayer, new List<treasureType>(m_treasuresPerPlayer));
            }

            Logger.WriteLine("TreasureTracker detected " + m_foundTreasuresMap.Count + " players.");
            Logger.WriteLine("TreasureTracker detected " + m_treasuresPerPlayer + " treasures per player");
        }

        /// <summary>
        /// Safes the information that the player with the given id found the given treasure.
        /// </summary>
        /// <param name="playerId">The id of the player that found a treasure.</param>
        /// <param name="foundTreasure">The type of the treasure.</param>
        void AddFoundTreasureForPlayer(int playerId, treasureType foundTreasure)
        {
            Debug.Assert(!m_foundTreasuresMap[playerId].Contains(foundTreasure));
            m_foundTreasuresMap[playerId].Add(foundTreasure);
            Debug.Assert(!m_foundTreasures.Contains(foundTreasure));
            m_foundTreasures.Add(foundTreasure);
            Logger.WriteLine("Player " + playerId + " found a treasure.");
        }

        /// <summary>
        /// Identifies which of the given enemy players found which of the given treasures.
        /// After the identification each pair of player and treasure will be added to our data structures calling AddFoundTreasureForPlayer.
        /// </summary>
        /// <param name="newlyFoundTreasures">The list of treasures that were found.</param>
        /// <param name="playersThatFoundATreasure">The list of enemy players that found a treasure since we have played our last move.</param>
        void MapFoundTreasuresToPlayers(List<treasureType> newlyFoundTreasures, List<int> playersThatFoundATreasure)
        {
            Debug.Assert(!playersThatFoundATreasure.Contains(Board.Current.PlayerId), "When our player finds a treasures this should be handled by calling the method AddFoundTreasureForOurPlayer.");
            Debug.Assert(newlyFoundTreasures.Count == playersThatFoundATreasure.Count, "For each treasure that was found, there must be exactly one player that has found it.");

            Action<treasureType, int> removeFromListsDelegate = (t, p) =>
                {
                    bool treasureRemoveResult = newlyFoundTreasures.Remove(t);
                    Debug.Assert(treasureRemoveResult);
                    bool playerRemoveResult = playersThatFoundATreasure.Remove(p);
                    Debug.Assert(playerRemoveResult);
                };

            switch (newlyFoundTreasures.Count)
            {
                case 1: //There is one treasure that was found, so the mapping is very easy.
                    AddFoundTreasureForPlayer(playersThatFoundATreasure[0], newlyFoundTreasures[0]);
                    break;
                case 2:
                    {
                        treasureType aFoundTreasure;
                        int aPlayerThatFoundATreasure;

                        var previousPlayer = Board.Current.PlayerId.PreviousPlayer();
                        if (playersThatFoundATreasure.Contains(previousPlayer)) //One player that found a treasure is our previous player.
                        {
                            //Since there did not happen any shift operations between our previous players move and this function call,
                            //the treasure that the previous player found must be on the field that he currently stands on.
                            var fieldThatContainsPreviousPlayer = Board.Current.First(f => f.ContainsPlayer(previousPlayer));
                            Debug.Assert(fieldThatContainsPreviousPlayer.ContainsTreasure && newlyFoundTreasures.Contains(fieldThatContainsPreviousPlayer.Treasure));

                            aFoundTreasure = fieldThatContainsPreviousPlayer.Treasure;
                            aPlayerThatFoundATreasure = previousPlayer;
                        }
                        else //We have to players that both are not our previous player.
                        {
                            Debug.Assert(Board.Current.TreasuresToGo.Length == 4);

                            //Therefor my shift card must be the card, that the unknown previous player kicked out by his shift operation.

                            int previousEnemyShiftPositionRowIndex = -1;
                            int previousEnemyShiftPositionColumnIndex = -1;

                            if ((Board.Current.ForbiddenShiftRow == 0) || (Board.Current.ForbiddenShiftRow == (Board.ROW_COUNT - 1)))
                            {
                                previousEnemyShiftPositionRowIndex = Board.ROW_COUNT - 1 - Board.Current.ForbiddenShiftRow;
                                previousEnemyShiftPositionColumnIndex = Board.Current.ForbiddenShiftColumn;
                            }
                            else if ((Board.Current.ForbiddenShiftColumn == 0) || (Board.Current.ForbiddenShiftColumn == (Board.COLUMN_COUNT - 1)))
                            {
                                previousEnemyShiftPositionRowIndex = Board.Current.ForbiddenShiftRow;
                                previousEnemyShiftPositionColumnIndex = Board.COLUMN_COUNT - 1 - Board.Current.ForbiddenShiftColumn;
                            }
                            else Debug.Assert(false);

                            var twoAfterPlayer = Board.Current.PlayerId.FollowingPlayer().FollowingPlayer();
                            Debug.Assert(playersThatFoundATreasure.Contains(Board.Current.PlayerId.FollowingPlayer()) && playersThatFoundATreasure.Contains(twoAfterPlayer) && Board.Current.PlayerId.FollowingPlayer() != twoAfterPlayer);
                            var twoAfterPlayerField = Board.Current.First(f => f.ContainsPlayer(twoAfterPlayer));

                            if (twoAfterPlayerField.RowIndex == previousEnemyShiftPositionRowIndex && twoAfterPlayerField.ColumnIndex == previousEnemyShiftPositionColumnIndex)
                            {
                                //This means that the player was inserted on the other side of the board after the shift operation.
                                //Following the field with the treasure where he stood on, also got shifted out and now becomes our shift card!
                                Debug.Assert(Board.Current.ShiftCard.ContainsTreasure);
                                Debug.Assert(newlyFoundTreasures.Contains(Board.Current.ShiftCard.Treasure));

                                aFoundTreasure = Board.Current.ShiftCard.Treasure;
                            }
                            else
                            {
                                //The previous enemy shift operation did not kick out the players' field on the board.
                                //Therefor he still must stand on the field where he found his treasure.
                                Debug.Assert(twoAfterPlayerField.ContainsTreasure);
                                Debug.Assert(newlyFoundTreasures.Contains(twoAfterPlayerField.Treasure));

                                aFoundTreasure = twoAfterPlayerField.Treasure;
                            }

                            aPlayerThatFoundATreasure = twoAfterPlayer;
                        }

                        AddFoundTreasureForPlayer(aPlayerThatFoundATreasure, aFoundTreasure);
                        removeFromListsDelegate(aFoundTreasure, aPlayerThatFoundATreasure);
                        goto case 1;
                    }
                case 3:
                    {
                        //Three enemy players found their treasures. Because this is a game with at most four players,
                        //we can assume that one of them must be our previous player.
                        //Since there did not happen any shift operations between our previous players move and this function call,
                        //the treasure that the previous player found must be on the field that he currently stands on.

                        Debug.Assert(Board.Current.TreasuresToGo.Length == 4);

                        var previousPlayer = Board.Current.PlayerId.PreviousPlayer();
                        var previousPlayersField = Board.Current.First(f => f.ContainsPlayer(previousPlayer));

                        Debug.Assert(previousPlayersField.ContainsTreasure && newlyFoundTreasures.Contains(previousPlayersField.Treasure));

                        //Add the mapping of our previous player and his found treasure to our data, remove them from the lists and go to case 2.
                        AddFoundTreasureForPlayer(previousPlayer, previousPlayersField.Treasure);
                        removeFromListsDelegate(previousPlayersField.Treasure, previousPlayer);
                        goto case 2;
                    }
                default:
                    Debug.Assert(false, "Unexpected value for the number of treasures that were found this round. Number: " + newlyFoundTreasures.Count);
                    break;
            }
        }

        /// <summary>
        /// Updates the data of the treasure tracker. Call this function each time when you receive a new board.
        /// </summary>
        internal void UpdateStatus()
        {
            if (m_foundTreasuresMap.Count == 0)
            {
                Initialize();
            }

            treasureType[] foundTreasures = Board.Current.FoundTreasures;

            Debug.Assert(foundTreasures.Length >= m_foundTreasures.Count);

            if (foundTreasures.Length != m_foundTreasures.Count) //Any enemies found a treasure since our last draw.
            {
                List<treasureType> newlyFoundTreasures = foundTreasures.Where(t => !m_foundTreasures.Contains(t)).ToList();

                List<int> playersThatFound = new List<int>(newlyFoundTreasures.Count);
                foreach (var aPlayerAndMissingTreasures in Board.Current.TreasuresToGo)
                {
                    var aPlayer = aPlayerAndMissingTreasures.player;
                    Debug.Assert((m_treasuresPerPlayer - aPlayerAndMissingTreasures.treasures) >= m_foundTreasuresMap[aPlayer].Count);
                    Debug.Assert(aPlayer != Board.Current.PlayerId || (m_treasuresPerPlayer - aPlayerAndMissingTreasures.treasures) == m_foundTreasuresMap[aPlayer].Count);

                    if ((m_treasuresPerPlayer - aPlayerAndMissingTreasures.treasures) != m_foundTreasuresMap[aPlayer].Count)
                    {
                        playersThatFound.Add(aPlayer);
                    }
                }

                //We added that if closure, because it can happen that an enemy player that found a treasure disconnected after that.
                //And this scenario would lead us to a newlyFoundTreasures list and a playersThatFound list with different numbers of elements. 
                //This case is extremely rare, because there is no reason for a player to disconnect after he found a treasure (finding a treasure means that you sent a valid move to the server).
                if (newlyFoundTreasures.Count == playersThatFound.Count)
                    MapFoundTreasuresToPlayers(newlyFoundTreasures, playersThatFound);
            }

            Debug.Assert(m_foundTreasures.Count == foundTreasures.Length);
            Debug.Assert(m_foundTreasuresMap.Values.Sum(l => l.Count) == m_foundTreasures.Count);
            Debug.Assert(m_foundTreasuresMap.Values.All(l => l.Count < m_treasuresPerPlayer));
        }

        /// <summary>
        /// Adds the TreasureTarget for our player in the current board to the list of treasures that our player has found.
        /// </summary>
        internal void AddFoundTreasureForOurPlayer()
        {
            AddFoundTreasureForPlayer(Board.Current.PlayerId, Board.Current.TreasureTarget);
        }

        /// <summary>
        /// Returns a list with the treasures that the given player could still miss.
        /// Till the end of a game it is not identifiable for us which treasure which player needs to find.
        /// So the missing treasure for a player are all treasures except the other players start positions and the treasures that other player
        /// </summary>
        /// <param name="playerId">The id of the player we want the missing treasures for.</param>
        /// <returns>All treasures that the specified player might be missing.</returns>
        internal List<treasureType> MissingTreasures(int playerId)
        {
            int numberOfMissingTreasures = Board.Current.TreasuresToGo.First(ttg => ttg.player == playerId).treasures;
            if (numberOfMissingTreasures == 1) //When the given player only needs to find one more treasure it must be his start position.
            {
                var playersStartPosition = (treasureType)Enum.Parse(typeof(treasureType), "Start0" + playerId);
                Debug.Assert(playersStartPosition == treasureType.Start01 || playersStartPosition == treasureType.Start02 || playersStartPosition == treasureType.Start03 || playersStartPosition == treasureType.Start04);
                return new List<treasureType> { playersStartPosition };
            }

            //We initialize the missing treasures for the given player with all treasures.
            var missingTreasures = new List<treasureType>(m_allTreasureTypes);

            //Iterate over all possible players.
            for (int p = 1; p <= 4; ++p)
            {
                if (playerId != p) //When p is not the specified playerId remove the concerning start position of p.
                {
                    var playersStartPosition = (treasureType)Enum.Parse(typeof(treasureType), "Start0" + p);
                    Debug.Assert(playersStartPosition == treasureType.Start01 || playersStartPosition == treasureType.Start02 || playersStartPosition == treasureType.Start03 || playersStartPosition == treasureType.Start04);
                    var removeResult = missingTreasures.Remove(playersStartPosition);
                    Debug.Assert(removeResult);
                }

                //Remove all treasures that any player has found yet.
                if (m_foundTreasuresMap.ContainsKey(p))
                {
                    foreach (var aPlayersFoundTreasures in m_foundTreasuresMap[p])
                    {
                        var removeResult = missingTreasures.Remove(aPlayersFoundTreasures);
                        Debug.Assert(removeResult);
                    }
                }
            }

            if (playerId != Board.Current.PlayerId)
            {
                //Also remove the treasure that our player needs to find next
                var removeResult = missingTreasures.Remove(Board.Current.TreasureTarget);
                Debug.Assert(removeResult);
            }

            return missingTreasures;
        }
    }
}