using MazeNetClient.Game;
using MazeNetClient.XSDGenerated;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MazeNetClient.AI
{
    /// <summary>
    /// A SimulatedMoveResult applies a shift operation and a movement of a players pin on an existing
    /// Board and simulates the result.
    /// This class is used as a linked list of SimulatedMoveResult. It provides functions to generate new
    /// SimulatedMoveResults as long as the treasure was not found yet.
    /// </summary>
    class SimulatedMoveResult : IFieldCollection
    {
        /// <summary>
        /// Holds the fields of the simulated move result.
        /// </summary>
        private readonly Field[] m_fields;

        /// <summary>
        /// Describes the board, already including a shift operation, that this
        /// simulated move result is based on.
        /// </summary>
        private readonly ShiftedBoard m_underlyingShiftedBoard;

        internal ShiftedBoard UnderlyingShiftedBoard
        {
            get { return m_underlyingShiftedBoard; }
        }

        /// <summary>
        /// Describes the previous simulated move result.
        /// This member is important to generate the linked list.
        /// </summary>
        private readonly SimulatedMoveResult m_previousSimulatedMoveResult;

        internal SimulatedMoveResult PreviousSimulatedMoveResult
        {
            get { return m_previousSimulatedMoveResult; }
        }

        /// <summary>
        /// The index of the row where this simulated move leads the player to.
        /// </summary>
        private readonly int m_newPlayerPosRowIndex;

        /// <summary>
        /// The index of the column where this simulated move leads the player to.
        /// </summary>
        private readonly int m_newPlayerPosColumnIndex;

        /// <summary>
        /// Gets the index of the row where the next player is not allowed to insert his shift card.
        /// </summary>
        internal int ForbiddenShiftPositionRowIndex
        {
            get { return KickedField.RowIndex; }
        }

        /// <summary>
        /// Gets the index of the column where the next player is not allowed to insert his shift card.
        /// </summary>
        internal int ForbiddenShiftPositionColumnIndex
        {
            get { return KickedField.ColumnIndex; }
        }

        /// <summary>
        /// The field that was kicked out by the shift operation that was applied by this simulated move result.
        /// The kicked field is the shift card for the following draw.
        /// </summary>
        internal readonly Field KickedField;

        /// <summary>
        /// The id of the player that did this simulated move.
        /// </summary>
        internal readonly int PlayerId;

        /// <summary>
        /// Creates and initializes a new instance of the type SimulatedMoveResult.
        /// </summary>
        /// <param name="underlyingBoard"></param>
        /// <param name="playerId">The id of the player that we simulate the move for.</param>
        /// <param name="newPlayerPosRowIndex">The new index of the row where the given player now stands on.</param>
        /// <param name="newPlayerPosColumnIndex">The new index of the column where the given player now stands on.</param>
        /// <param name="previousSimulatedMoveResult">You can create a linked list of SimulatedMoveResults. This parameter is the previous simulated move result in the linked list. It may be null.</param>
        internal SimulatedMoveResult(ShiftedBoard underlyingBoard, int playerId, int newPlayerPosRowIndex, int newPlayerPosColumnIndex, SimulatedMoveResult previousSimulatedMoveResult)
        {
            m_fields = new Field[Board.ROW_COUNT * Board.COLUMN_COUNT];
            m_underlyingShiftedBoard = underlyingBoard;
            m_previousSimulatedMoveResult = previousSimulatedMoveResult;
            m_newPlayerPosRowIndex = newPlayerPosRowIndex;
            m_newPlayerPosColumnIndex = newPlayerPosColumnIndex;
            KickedField = underlyingBoard.KickedField;
            PlayerId = playerId;

            for (int i = 0; i != Board.ROW_COUNT; ++i)
            {
                for (int j = 0; j != Board.COLUMN_COUNT; ++j)
                {
                    int index = i * Board.COLUMN_COUNT + j;
                    var srcField = underlyingBoard[i, j];

                    bool srcContainsPlayerId = srcField.ContainsPlayer(playerId);
                    bool srcIsMoveTarget = (srcField.RowIndex == newPlayerPosRowIndex) && (srcField.ColumnIndex == newPlayerPosColumnIndex);

                    if (srcContainsPlayerId && !srcIsMoveTarget)
                    {
                        var modifiedContainingPlayers = srcField.ContainingPlayers.Where(p => p != playerId).ToArray();
                        m_fields[index] = new Field(i, j, srcField.IsLeftOpen, srcField.IsTopOpen, srcField.IsRightOpen, srcField.IsBottomOpen, modifiedContainingPlayers, srcField.Treasure, srcField.ContainsTreasure);
                    }
                    else if (srcIsMoveTarget && !srcContainsPlayerId)
                    {
                        var containingPlayersAsList = srcField.ContainingPlayers.ToList();
                        containingPlayersAsList.Add(playerId);
                        var modifiedContainingPlayers = containingPlayersAsList.ToArray();
                        m_fields[index] = new Field(i, j, srcField.IsLeftOpen, srcField.IsTopOpen, srcField.IsRightOpen, srcField.IsBottomOpen, modifiedContainingPlayers, srcField.Treasure, srcField.ContainsTreasure);
                    }
                    else
                    {
                        m_fields[index] = srcField;
                    }
                }
            }

            this.AssertValidIFieldCollection();
        }

        public Field this[int row, int column]
        {
            get
            {
                Debug.Assert(row >= 0 && row < Board.ROW_COUNT);
                Debug.Assert(column >= 0 && column < Board.COLUMN_COUNT);

                int index = row * Board.COLUMN_COUNT + column;
                var aField = m_fields[index];
                Debug.Assert(aField.RowIndex == row && aField.ColumnIndex == column);
                return aField;
            }
        }

        public IEnumerator<Field> GetEnumerator()
        {
            foreach (var aField in m_fields)
            {
                yield return aField;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Creates a list of SimulatedMoveResults.
        /// </summary>
        /// <param name="shiftedBoard">The underlying board containing the shift operation.</param>
        /// <param name="playerId">The id of the player that accomplishes the moves.</param>
        /// <param name="previousSimulatedMoveResult">The previous simulated move result. This member is important for the linked list structure. It may be null.</param>
        /// <returns>A list with all simulated move results, that the player can go to.</returns>
        internal static IEnumerable<SimulatedMoveResult> CreateAllPossibleMoves(ShiftedBoard shiftedBoard, int playerId, SimulatedMoveResult previousSimulatedMoveResult)
        {
            Debug.Assert(shiftedBoard.Count(f => f.ContainsPlayer(playerId)) == 1);

            var playerField = shiftedBoard.First(f => f.ContainsPlayer(playerId));
            var possiblePlayerMoves = shiftedBoard.GetReachableFields(playerField.RowIndex, playerField.ColumnIndex);
            foreach (var aPossiblePlayerMove in possiblePlayerMoves)
            {
                yield return new SimulatedMoveResult(shiftedBoard, playerId, aPossiblePlayerMove.RowIndex, aPossiblePlayerMove.ColumnIndex, previousSimulatedMoveResult);
            }
        }

        /// <summary>
        /// Indicates whether the SimulatedMoves' Player stands on a field where
        /// also the given treasure appears.
        /// </summary>
        /// <param name="playersTreasureTarget">The given treasure that we are looking for.</param>
        /// <returns>True when the player stands on a field containing the specified treasure, false otherwise.</returns>
        internal bool FoundTreasure(treasureType playersTreasureTarget)
        {
            Debug.Assert(m_fields.Count(f => f.HasTreasure(playersTreasureTarget)) <= 1);
            var fieldWithTreasure = m_fields.FirstOrDefault(f => f.HasTreasure(playersTreasureTarget));

            var playerStandsOnTreasure = (fieldWithTreasure != null)
                && (m_newPlayerPosRowIndex == fieldWithTreasure.RowIndex)
                && (m_newPlayerPosColumnIndex == fieldWithTreasure.ColumnIndex);

            Debug.Assert(!playerStandsOnTreasure || fieldWithTreasure.ContainsPlayer(PlayerId));
            Debug.Assert(!playerStandsOnTreasure || m_previousSimulatedMoveResult != null);

            return playerStandsOnTreasure;
        }

        /// <summary>
        /// The SimulatedMoveResult contains a linked list of SimulatedMoveResult-objects.
        /// This method returns the Move that would lead the player to the first SimulatedMoveResult of this linked list.
        /// </summary>
        /// <returns></returns>
        internal Move GetFirstMove()
        {
            if (m_previousSimulatedMoveResult == null)
                return Move.Create(m_underlyingShiftedBoard, m_newPlayerPosRowIndex, m_newPlayerPosColumnIndex);
            else
                return m_previousSimulatedMoveResult.GetFirstMove();
        }
    }
}