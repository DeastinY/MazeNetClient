using MazeNetClient.Game;
using MazeNetClient.XSDGenerated;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MazeNetClient.AI
{
    /// <summary>
    /// This class creates a new board, based on an existing board and applies a shift operation on that new board.
    /// This class is used for simulating all possible shift operations on the current board and analyze the results.
    /// </summary>
    class SimulatedBoard : IEnumerable<Field>
    {
        /// <summary>
        /// Holds the fields of the simulated board.
        /// </summary>
        private readonly Field[] m_fields;

        /// <summary>
        /// Holds the index of the row where the player appears in the simulated board.
        /// </summary>
        internal readonly int PlayerPositionRowIndex;

        /// <summary>
        /// Hold the index of the column where the player appears in the simulated board.
        /// </summary>
        internal readonly int PlayerPositionColumnIndex;

        /// <summary>
        /// Creates and initializes a new instance of the type SimulatedBoard.
        /// </summary>
        /// <param name="actualBoard"></param>
        /// <param name="shiftPositionRowIndex"></param>
        /// <param name="shiftPositionColumnIndex"></param>
        /// <param name="shiftCardRotation"></param>
        internal SimulatedBoard(Board actualBoard, int shiftPositionRowIndex, int shiftPositionColumnIndex, Rotation shiftCardRotation)
        {
            m_fields = new Field[Board.ROW_COUNT * Board.COLUMN_COUNT];

            InsertShiftCard(actualBoard.ShiftCard, shiftCardRotation, shiftPositionRowIndex, shiftPositionColumnIndex);

            Field fieldWithPlayer = actualBoard.First(f => f.ContainsPlayer(actualBoard.PlayerId));

            if (InsertInRowFromLeft(shiftPositionRowIndex, shiftPositionColumnIndex))
            {
                Debug.Assert(this[shiftPositionRowIndex, 0] != null);

                for (int j = 1; j < Board.COLUMN_COUNT; ++j)
                {
                    int index = shiftPositionRowIndex * Board.COLUMN_COUNT + j;
                    var boardField = actualBoard[shiftPositionRowIndex, j - 1];
                    m_fields[index] = CreateCopy(boardField, shiftPositionRowIndex, j);
                    if (boardField == fieldWithPlayer)
                    {
                        this.PlayerPositionRowIndex = shiftPositionRowIndex;
                        this.PlayerPositionColumnIndex = j;
                    }
                }
            }
            else if (InsertInRowFromRight(shiftPositionRowIndex, shiftPositionColumnIndex))
            {
                Debug.Assert(this[shiftPositionRowIndex, Board.COLUMN_COUNT - 1] != null);

                for (int j = 0; j < Board.COLUMN_COUNT - 1; ++j)
                {
                    int index = shiftPositionRowIndex * Board.COLUMN_COUNT + j;
                    var boardField = actualBoard[shiftPositionRowIndex, j + 1];
                    m_fields[index] = CreateCopy(boardField, shiftPositionRowIndex, j);
                    if (boardField == fieldWithPlayer)
                    {
                        this.PlayerPositionRowIndex = shiftPositionRowIndex;
                        this.PlayerPositionColumnIndex = j;
                    }
                }
            }
            else if (InsertInColumnFromTop(shiftPositionRowIndex, shiftPositionColumnIndex))
            {
                Debug.Assert(this[0, shiftPositionColumnIndex] != null);

                for (int i = 1; i < Board.ROW_COUNT; ++i)
                {
                    int index = i * Board.COLUMN_COUNT + shiftPositionColumnIndex;
                    var boardField = actualBoard[i - 1, shiftPositionColumnIndex];
                    m_fields[index] = CreateCopy(boardField, i, shiftPositionColumnIndex);
                    if (boardField == fieldWithPlayer)
                    {
                        this.PlayerPositionRowIndex = i;
                        this.PlayerPositionColumnIndex = shiftPositionColumnIndex;
                    }
                }

            }
            else if (InsertInColumnFromBottom(shiftPositionRowIndex, shiftPositionColumnIndex))
            {
                Debug.Assert(this[Board.ROW_COUNT - 1, shiftPositionColumnIndex] != null);

                for (int i = 0; i < Board.ROW_COUNT - 1; ++i)
                {
                    int index = i * Board.COLUMN_COUNT + shiftPositionColumnIndex;
                    var boardField = actualBoard[i + 1, shiftPositionColumnIndex];
                    m_fields[index] = CreateCopy(boardField, i, shiftPositionColumnIndex);
                    if (boardField == fieldWithPlayer)
                    {
                        this.PlayerPositionRowIndex = i;
                        this.PlayerPositionColumnIndex = shiftPositionColumnIndex;
                    }
                }
            }
            else
            {
                Debug.Assert(false, "This code should not be reached!");
            }

            //Fill up the m_fields with all fields, that are not infected by the shift operation (and therefore still null).
            for (int i = 0; i < Board.ROW_COUNT; ++i)
            {
                for (int j = 0; j < Board.COLUMN_COUNT; ++j)
                {
                    int index = i * Board.ROW_COUNT + j;
                    if (m_fields[index] == null)
                    {
                        var boardField = actualBoard[i, j];
                        Debug.Assert(boardField.RowIndex == i && boardField.ColumnIndex == j);
                        m_fields[index] = CreateCopy(boardField, i, j);
                        if (boardField == fieldWithPlayer)
                        {
                            this.PlayerPositionRowIndex = i;
                            this.PlayerPositionColumnIndex = j;
                        }
                    }
                    else
                    {
                        Debug.Assert(m_fields[index].RowIndex == shiftPositionRowIndex || m_fields[index].ColumnIndex == shiftPositionColumnIndex);
                    }
                }
            }


            var count = this.m_fields.Count(f => f.ContainsPlayer(actualBoard.PlayerId));
            Debug.Assert(count == 1);
            var first = this.m_fields.First(f => f.ContainsPlayer(actualBoard.PlayerId));
            Debug.Assert(first.RowIndex == PlayerPositionRowIndex && first.ColumnIndex == PlayerPositionColumnIndex);
            Debug.Assert(this[PlayerPositionRowIndex, PlayerPositionColumnIndex] == first);
        }

        internal Field this[int row, int column]
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

        private void InsertShiftCard(Field from, Rotation rotation, int shiftPositionRowIndex, int shiftPositionColumnIndex)
        {
            bool isLeftOpen = false,
                 isTopOpen = false,
                 isRightOpen = false,
                 isBottomOpen = false;

            switch (rotation)
            {
                case Rotation.DEGREE_0:
                    isLeftOpen = from.IsLeftOpen;
                    isTopOpen = from.IsTopOpen;
                    isRightOpen = from.IsRightOpen;
                    isBottomOpen = from.IsBottomOpen;
                    break;
                case Rotation.DEGREE_90:
                    isLeftOpen = from.IsBottomOpen;
                    isTopOpen = from.IsLeftOpen;
                    isRightOpen = from.IsTopOpen;
                    isBottomOpen = from.IsRightOpen;
                    break;
                case Rotation.DEGREE_180:
                    isLeftOpen = from.IsRightOpen;
                    isTopOpen = from.IsBottomOpen;
                    isRightOpen = from.IsLeftOpen;
                    isBottomOpen = from.IsTopOpen;
                    break;
                case Rotation.DEGREE_270:
                    isLeftOpen = from.IsTopOpen;
                    isTopOpen = from.IsRightOpen;
                    isRightOpen = from.IsBottomOpen;
                    isBottomOpen = from.IsLeftOpen;
                    break;
                default:
                    Debug.Assert(false, "Invalid value of enum rotation: " + rotation);
                    break;
            }

            var shiftCard = new Field(shiftPositionRowIndex, shiftPositionColumnIndex, isLeftOpen, isTopOpen, isRightOpen, isBottomOpen,
                from.ContainingPlayers, from.Treasure, from.ContainsTreasure);

            int shiftCard1DIndex = shiftPositionRowIndex * Board.ROW_COUNT + shiftPositionColumnIndex;
            m_fields[shiftCard1DIndex] = shiftCard;
        }

        private bool InsertInRowFromLeft(int shiftPositionRowIndex, int shiftPositionColumnIndex)
        {
            return (shiftPositionRowIndex != 0 && shiftPositionColumnIndex == 0);
        }

        private bool InsertInRowFromRight(int shiftPositionRowIndex, int shiftPositionColumnIndex)
        {
            return (shiftPositionRowIndex != 0 && shiftPositionColumnIndex == Board.COLUMN_COUNT - 1);
        }

        private bool InsertInColumnFromTop(int shiftPositionRowIndex, int shiftPositionColumnIndex)
        {
            return (shiftPositionRowIndex == 0 && shiftPositionColumnIndex != 0);
        }

        private bool InsertInColumnFromBottom(int shiftPositionRowIndex, int shiftPositionColumnIndex)
        {
            return (shiftPositionRowIndex == Board.ROW_COUNT - 1 && shiftPositionColumnIndex != 0);
        }

        private Field CreateCopy(Field source, int newRowIndex, int newColumnIndex)
        {
            return new Field(newRowIndex, newColumnIndex, source.IsLeftOpen, source.IsTopOpen, source.IsRightOpen, source.IsBottomOpen,
               source.ContainingPlayers, source.Treasure, source.ContainsTreasure);
        }
    }
}