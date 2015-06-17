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
    /// It is an singleton that is accessible from all around the program.
    /// </summary>
    sealed class TreasureTracker
    {
        /// <summary>
        /// Creates and initializes a new instance of the type TreasureTracker.
        /// The constructor is marked private because this class is a singleton.
        /// </summary>
        private TreasureTracker()
        {
            m_allTreasureTypes = (treasureType[])Enum.GetValues(typeof(treasureType));
            m_foundTreasures = new Dictionary<int, List<treasureType>>(4);
            m_numberOfFoundTreasures = 0;
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
        private readonly treasureType[] m_allTreasureTypes;

        /// <summary>
        /// Describes a mapping from a player to a list of treasures that the player has already found.
        /// </summary>
        private Dictionary<int, List<treasureType>> m_foundTreasures;

        /// <summary>
        /// Holds the number of treasures that where already found.
        /// It does not describe the number of found treasures that we did identify.
        /// Perhaps we recognized that an enemy found a treasure but where not able to identify that found treasure.
        /// Then we would anyway increase this field.
        /// </summary>
        private int m_numberOfFoundTreasures;

        void Initialize()
        {
            var players = Board.Current.TreasuresToGo.Select(t => t.player);
            int treasuresPerPlayer = m_allTreasureTypes.Length / players.Count();
            foreach (var aPlayer in players)
            {
                m_foundTreasures.Add(aPlayer, new List<treasureType>(treasuresPerPlayer));
            }

            Logger.WriteLine("TreasureTracker detected " + m_foundTreasures.Count + " players.");
        }

        void AddFoundTreasureForPlayer(int playerId, treasureType foundTreasure)
        {
            Debug.Assert(!m_foundTreasures[playerId].Contains(foundTreasure));
            m_foundTreasures[playerId].Add(foundTreasure);
        }

        bool TryGetFoundTreasure(int playerId, out treasureType foundTreasure)
        {
            var board = Board.Current;

            if (Board.HistoryLength == 1) //We are not the first player and some players, before ours, found treasures.
            {
                Debug.Assert(Board.Current.PlayerId != 1, "We should not be the first player");

                if (Board.Current.PlayerId == 2)
                {

                }

            }
            else
            {
                Board previousBoard = Board.GetBoard(Board.HistoryLength - 2);


            }

            foundTreasure = (treasureType)(-1);
            return false;
        }

        internal void UpdateStatus()
        {
            //TODO: Einbauen dass wenn ein spieler disconnected das auch berücksichtigt wird.

            if (m_foundTreasures.Count == 0)
            {
                Initialize();
            }

            treasureType[] foundTreasures = Board.Current.FoundTreasures;

            if (foundTreasures.Length != m_numberOfFoundTreasures) //When since our last board the number of found treasures increased.
            {
                foreach (var aTreasureToGoType in Board.Current.TreasuresToGo)
                {
                    var aPlayer = aTreasureToGoType.player;

                    if (aPlayer != Board.Current.PlayerId) //When aPlayer is our player then we have handled that already in the function AddFoundTreasureForOurPlayer.
                    {
                        var lastFoundTreasures = m_foundTreasures[aPlayer];

                        //The number of found treasures changed for this player
                        if (lastFoundTreasures.Count != aTreasureToGoType.treasures)
                        {
                            ++m_numberOfFoundTreasures;

                            treasureType treasureFoundByPlayer;
                            if (TryGetFoundTreasure(aPlayer, out treasureFoundByPlayer))
                            {
                                AddFoundTreasureForPlayer(aPlayer, treasureFoundByPlayer);
                            }
                            else
                            {
                                Logger.WriteLine("Player " + aPlayer + " found a treasure but we do not know which treasure it was.");
                                Debug.Assert(false);
                            }
                        }
                    }
                }

                Debug.Assert(m_numberOfFoundTreasures == foundTreasures.Length);
            }

            Debug.Assert(AllFoundTreasures().Count() + AllMissingTreasures().Count() == Enum.GetValues(typeof(treasureType)).Length);
        }

        /// <summary>
        /// Adds the TreasureTarget of the current board to the list of treasures that our player has found.
        /// </summary>
        internal void AddFoundTreasureForOurPlayer()
        {
            AddFoundTreasureForPlayer(Board.Current.PlayerId, Board.Current.TreasureTarget);
            ++m_numberOfFoundTreasures;
        }

        internal IEnumerable<treasureType> AllFoundTreasures()
        {
            foreach (List<treasureType> aFoundTreasureListPerPlayer in m_foundTreasures.Values)
            {
                foreach (treasureType aFoundTreasure in aFoundTreasureListPerPlayer)
                {
                    yield return aFoundTreasure;
                }
            }
        }

        internal IEnumerable<treasureType> AllMissingTreasures()
        {
            var allFoundTreasures = AllFoundTreasures();
            foreach (var aTreasure in m_allTreasureTypes)
            {
                if (!allFoundTreasures.Contains(aTreasure))
                {
                    yield return aTreasure;
                }
            }
        }

        internal List<treasureType> FoundTreasures(int playerId)
        {
            return m_foundTreasures[playerId];
        }

        internal int NumberOfFoundTreasuresThatWeCouldNotIdentify()
        {
            int numberOfActualyFoundTreasures = m_foundTreasures.Values.Sum(foundTreasures => foundTreasures.Count);
            return (m_numberOfFoundTreasures - numberOfActualyFoundTreasures);
        }
    }
}