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
    class SimulatedBoard : IEnumerable<Field>, IFieldCollection
    {
        /// <summary>
        /// Holds the fields of the simulated board.
        /// </summary>
        private readonly Field[] m_fields;

        /// <summary>
        /// The index of the row where we inserted the shift card to get from the actual board to this SimulatedBoard.
        /// </summary>
        internal readonly int ShiftPositionRowIndex;

        /// <summary>
        /// The index of the column where we inserted the shift card to get from the actual board to this SimulatedBoard.
        /// </summary>
        internal readonly int ShiftPositionColumnIndex;

        /// <summary>
        /// The rotation that we applied to the shift card before we inserted it in this SimulatedBoard.
        /// </summary>
        internal readonly Rotation ShiftCardRotation;

        /// <summary>
        /// Describes the id of the player that we represent.
        /// </summary>
        internal readonly int PlayerId;

        /// <summary>
        /// Holds the index of the row where the player appears in the simulated board.
        /// </summary>
        internal readonly int PlayerPositionRowIndex;

        /// <summary>
        /// Hold the index of the column where the player appears in the simulated board.
        /// </summary>
        internal readonly int PlayerPositionColumnIndex;

        /// <summary>
        /// Holds all treasures that are already found.
        /// </summary>
        internal readonly treasureType[] FoundTreasures;

        /// <summary>
        /// Describes an array of pairs, where each element describes a pair of a player id and the number of treasures that he still needs to find.
        /// </summary>
        internal readonly TreasuresToGoType[] TreasuresToGo;

        /// <summary>
        /// Describes the treasure, that the player needs to collect next.
        /// </summary>
        internal readonly treasureType TreasureTarget;

        /// <summary>
        /// Describes the index of the row where the treasure appears that our player needs to find next.
        /// </summary>
        internal readonly int TreasureTargetRowIndex;

        /// <summary>
        /// Describes the index of the column where the treasure appears that our player needs to find next.
        /// </summary>
        internal readonly int TreasureTargetColumnIndex;

        /// <summary>
        /// Creates and initializes a new instance of the type SimulatedBoard.
        /// </summary>
        /// <param name="actualBoard">The initial board where the shift will be simulated on.</param>
        /// <param name="shiftPositionRowIndex">The index of the row where the shift card will be inserted.</param>
        /// <param name="shiftPositionColumnIndex">The index of the column where the shift card will be inserted.</param>
        /// <param name="shiftCardRotation">The rotation that will be applied to the shift card.</param>
        internal SimulatedBoard(Board actualBoard, int shiftPositionRowIndex, int shiftPositionColumnIndex, Rotation shiftCardRotation)
        {
            m_fields = new Field[Board.ROW_COUNT * Board.COLUMN_COUNT];
            ShiftPositionRowIndex = shiftPositionRowIndex;
            ShiftPositionColumnIndex = shiftPositionColumnIndex;
            ShiftCardRotation = shiftCardRotation;
            PlayerId = actualBoard.PlayerId;
            FoundTreasures = (treasureType[])actualBoard.FoundTreasures.Clone();
            TreasuresToGo = new TreasuresToGoType[actualBoard.TreasuresToGo.Length];
            for (int i = 0; i < TreasuresToGo.Length; ++i)
            {
                var aTreasureToGo = actualBoard.TreasuresToGo[i];
                TreasuresToGo[i] = new TreasuresToGoType { player = aTreasureToGo.player, treasures = aTreasureToGo.treasures };
            }
            TreasureTarget = actualBoard.TreasureTarget;

            //This variable describes the player ids, that were kicked out of the game by inserting the shift card.
            //They have to be inserted in the shift card.
            int[] kickedPlayers = null;

            if (InsertInRowFromLeft(shiftPositionRowIndex, shiftPositionColumnIndex))
            {
                kickedPlayers = actualBoard[shiftPositionRowIndex, Board.COLUMN_COUNT - 1].ContainingPlayers;

                for (int j = 1; j < Board.COLUMN_COUNT; ++j)
                {
                    int index = shiftPositionRowIndex * Board.COLUMN_COUNT + j;
                    var boardField = actualBoard[shiftPositionRowIndex, j - 1];
                    m_fields[index] = CreateCopy(boardField, shiftPositionRowIndex, j);
                }
            }
            else if (InsertInRowFromRight(shiftPositionRowIndex, shiftPositionColumnIndex))
            {
                kickedPlayers = actualBoard[shiftPositionRowIndex, 0].ContainingPlayers;

                for (int j = 0; j < Board.COLUMN_COUNT - 1; ++j)
                {
                    int index = shiftPositionRowIndex * Board.COLUMN_COUNT + j;
                    var boardField = actualBoard[shiftPositionRowIndex, j + 1];
                    m_fields[index] = CreateCopy(boardField, shiftPositionRowIndex, j);
                }
            }
            else if (InsertInColumnFromTop(shiftPositionRowIndex, shiftPositionColumnIndex))
            {
                kickedPlayers = actualBoard[Board.ROW_COUNT - 1, shiftPositionColumnIndex].ContainingPlayers;

                for (int i = 1; i < Board.ROW_COUNT; ++i)
                {
                    int index = i * Board.COLUMN_COUNT + shiftPositionColumnIndex;
                    var boardField = actualBoard[i - 1, shiftPositionColumnIndex];
                    m_fields[index] = CreateCopy(boardField, i, shiftPositionColumnIndex);
                }

            }
            else if (InsertInColumnFromBottom(shiftPositionRowIndex, shiftPositionColumnIndex))
            {
                kickedPlayers = actualBoard[0, shiftPositionColumnIndex].ContainingPlayers;

                for (int i = 0; i < Board.ROW_COUNT - 1; ++i)
                {
                    int index = i * Board.COLUMN_COUNT + shiftPositionColumnIndex;
                    var boardField = actualBoard[i + 1, shiftPositionColumnIndex];
                    m_fields[index] = CreateCopy(boardField, i, shiftPositionColumnIndex);
                }
            }
            else
            {
                Debug.Assert(false, "This code should not be reached!");
            }

            InsertShiftCard(actualBoard.ShiftCard, shiftCardRotation, shiftPositionRowIndex, shiftPositionColumnIndex, kickedPlayers);

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
                    }
                    else
                    {
                        Debug.Assert(m_fields[index].RowIndex == shiftPositionRowIndex || m_fields[index].ColumnIndex == shiftPositionColumnIndex);
                    }
                }
            }


            Debug.Assert(m_fields.Count(f => f.ContainsPlayer(PlayerId)) == 1);
            var playerField = m_fields.First(f => f.ContainsPlayer(PlayerId));
            PlayerPositionRowIndex = playerField.RowIndex;
            PlayerPositionColumnIndex = playerField.ColumnIndex;

            Debug.Assert(m_fields.Count(f => f.HasTreasure(TreasureTarget)) == 1);
            var treasureTargetField = m_fields.First(f => f.HasTreasure(TreasureTarget));
            TreasureTargetRowIndex = treasureTargetField.RowIndex;
            TreasureTargetColumnIndex = treasureTargetField.ColumnIndex;
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

        private void InsertShiftCard(Field from, Rotation rotation, int shiftPositionRowIndex, int shiftPositionColumnIndex, int[] containingPlayers)
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
                (int[])containingPlayers.Clone(), from.Treasure, from.ContainsTreasure);

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