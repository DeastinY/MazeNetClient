using MazeNetClient.XSDGenerated;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MazeNetClient.Game
{
    /// <summary>
    /// Defines the board of the game.
    /// A board consist of 7 times 7 fields, a shift card, a forbidden shifting place
    /// and the information about the next treasures position.
    /// </summary>
    class Board : IFieldCollection
    {
        /// <summary>
        /// This is the number of rows, that the playing board has.
        /// </summary>
        internal const int ROW_COUNT = 7;

        /// <summary>
        /// This is the number of columns, that the playing board has.
        /// </summary>
        internal const int COLUMN_COUNT = 7;

        /// <summary>
        /// Describes a history of all boards during the game.
        /// </summary>
        private static List<Board> boardHistory = new List<Board>();

        /// <summary>
        /// Holds an array with the length ROW_COUNT * COLUMN_COUNT.
        /// The array contains a field of the board at each index.
        /// </summary>
        private readonly Field[] m_fields;

        /// <summary>
        /// Describes the row, where you are not allowed to insert the shift card.
        /// This member will be -1 at the first round of the game as there is no row,
        /// where you are not allowed to insert the shift card.
        /// </summary>
        internal readonly int ForbiddenShiftRow;

        /// <summary>
        /// Describes the column, where you are not allowed to insert the shift card.
        /// This member will be -1 at the first round of the game as there is no column,
        /// where you are not allowed to insert the shift card.
        /// </summary>
        internal readonly int ForbiddenShiftColumn;

        /// <summary>
        /// Describes the card, that will be used for the shift.
        /// </summary>
        internal readonly Field ShiftCard;

        /// <summary>
        /// Describes the treasure, that the player needs to collect next.
        /// </summary>
        internal readonly treasureType TreasureTarget;

        /// <summary>
        /// Describes an array of all treasures that were found yet.
        /// </summary>
        internal readonly treasureType[] FoundTreasures;

        /// <summary>
        /// Describes the id of the player that we represent.
        /// </summary>
        internal readonly int PlayerId;

        /// <summary>
        /// Describes an array of pairs, where each element describes a pair of a player id and the number of treasures that he still needs to find.
        /// </summary>
        internal readonly TreasuresToGoType[] TreasuresToGo;

        /// <summary>
        /// Creates and initializes a new instance of the Board type, depending on the specified AwaitMoveMessageType.
        /// </summary>
        /// <param name="currentGameStatus">The specified AwaitMoveMessageType, that contains data bout the board and the game status.</param>
        /// <param name="playerId">The id that we received from the server and that represents us.</param>
        internal Board(AwaitMoveMessageType currentGameStatus, int playerId)
        {
            var currentBoard = currentGameStatus.board;

            var forbiddenShiftPos = currentBoard.forbidden;
            if (forbiddenShiftPos == null)
            {
                ForbiddenShiftRow = ForbiddenShiftColumn = -1;
            }
            else
            {
                ForbiddenShiftRow = forbiddenShiftPos.row;
                ForbiddenShiftColumn = forbiddenShiftPos.col;
            }

            ShiftCard = new Field(currentBoard.shiftCard, -1, -1);

            TreasureTarget = currentGameStatus.treasure;

            m_fields = new Field[ROW_COUNT * COLUMN_COUNT];
            var boardRows = currentBoard.row;
            for (int i = 0; i < boardRows.Length; ++i)
            {
                var boardColumns = boardRows[i].col;
                for (int j = 0; j < boardColumns.Length; ++j)
                {
                    int index = i * ROW_COUNT + j;
                    m_fields[index] = new Field(boardColumns[j], i, j);
                }
            }

            var foundTreasures = currentGameStatus.foundTreasures ?? new treasureType[0];
            FoundTreasures = (treasureType[])foundTreasures.Clone();

            PlayerId = playerId;

            var treasuresToGo = currentGameStatus.treasuresToGo;
            TreasuresToGo = new TreasuresToGoType[treasuresToGo.Length];
            for (int i = 0; i < treasuresToGo.Length; ++i)
            {
                TreasuresToGo[i] = new TreasuresToGoType
                {
                    player = treasuresToGo[i].player,
                    treasures = treasuresToGo[i].treasures
                };
            }

            boardHistory.Add(this);
            Board.Current = this;
            this.AssertValidIFieldCollection();
        }

        public Field this[int row, int column]
        {
            get
            {
                Debug.Assert(row >= 0 && row < ROW_COUNT);
                Debug.Assert(column >= 0 && column < COLUMN_COUNT);

                int index = row * ROW_COUNT + column;
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

        internal IEnumerable<int> GetEnemyPlayers()
        {
            return TreasuresToGo.Select(ttg => ttg.player).Where(id => id != PlayerId);
        }

        /// <summary>
        /// Resets the history of boards by removing all boards from the history.
        /// </summary>
        internal static void ResetBoardHistory()
        {
            boardHistory.Clear();
        }

        /// <summary>
        /// Gets the number of boards in the history.
        /// </summary>
        internal static int HistoryLength
        {
            get { return boardHistory.Count; }
        }

        /// <summary>
        /// Returns the board in the history at the specified index.
        /// </summary>
        /// <param name="index">The specified index of the board in the history.</param>
        /// <returns></returns>
        internal static Board GetBoard(int index)
        {
            Debug.Assert((uint)index < (uint)HistoryLength);
            return boardHistory[index];
        }

        /// <summary>
        /// Gets (or private sets) the board of the current round, which is always the last board in the history.
        /// </summary>
        internal static Board Current { get; private set; }
    }
}