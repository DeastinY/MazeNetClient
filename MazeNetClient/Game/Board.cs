using MazeNetClient.XSDGenerated;
using System.Diagnostics;

namespace MazeNetClient.Game
{
    /// <summary>
    /// Defines the board of the game.
    /// A board consist of 7 times 7 fields, a shift card, a forbidden shifting place
    /// and the information about the next treasures position.
    /// </summary>
    class Board
    {
        /// <summary>
        /// This is the number of rows, that the playing board has.
        /// </summary>
        private const int ROW_COUNT = 7;

        /// <summary>
        /// This is the number of columns, that the playing board has.
        /// </summary>
        private const int COLUMN_COUNT = 7;

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
        /// Creates and initializes a new instance of the Board type, depending on the specified AwaitMoveMessageType.
        /// </summary>
        /// <param name="currentGameStatus">The specified AwaitMoveMessageType, that contains data bout the board and the game status.</param>
        internal Board(AwaitMoveMessageType currentGameStatus)
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

            ShiftCard = new Field(currentBoard.shiftCard);

            TreasureTarget = currentGameStatus.treasure;

            m_fields = new Field[ROW_COUNT * COLUMN_COUNT];
            var boardRows = currentBoard.row;
            for (int i = 0; i < boardRows.Length; ++i)
            {
                System.Diagnostics.Debug.Assert(boardRows[i].OptionalAttirbute == null, "//TODO: Frage: Was soll dieses OptionalAttribute machen");

                var boardColumns = boardRows[i].col;
                for (int j = 0; j < boardColumns.Length; ++j)
                {
                    int index = i * ROW_COUNT + j;
                    m_fields[index] = new Field(boardColumns[j]);
                }
            }

            //TODO: Frage: currentGameStatus.treasuresToGo, sind das ein array von paaren, wobei das erste item angibt, welcher spieler gemeint ist, und im zweiten item, wie viele schätze er noch bruacht?!
            //TODO: currentGameStatus.treasuresToGo berücksichtigen
        }

        internal Field this[int row, int column]
        {
            get
            {
                Debug.Assert(row >= 0 && row < ROW_COUNT);
                Debug.Assert(column >= 0 && column < COLUMN_COUNT);

                int index = row * ROW_COUNT + column;
                return m_fields[index];
            }
        }
    }
}