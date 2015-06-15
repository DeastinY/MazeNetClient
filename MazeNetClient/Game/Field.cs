using MazeNetClient.XSDGenerated;
using System;

namespace MazeNetClient.Game
{
    /// <summary>
    /// Defines a single field into a board.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Row: {RowIndex}, Column: {ColumnIndex}")]
    class Field
    {
        /// <summary>
        /// Describes the index of the row, at that the field appears in a board.
        /// </summary>
        internal readonly int RowIndex;

        /// <summary>
        /// Describes the index of the column, at that the field appears in a board.
        /// </summary>
        internal readonly int ColumnIndex;

        /// <summary>
        /// Contains the identifier of the player, that stand on this field.
        /// </summary>
        internal readonly int[] ContainingPlayers;

        /// <summary>
        /// Describes the type of the treasure, that this field contains.
        /// First query the value of ContainsTreasure, to test whether this field contains a Treasure.
        /// </summary>
        internal readonly treasureType Treasure;

        /// <summary>
        /// Indicates, whether the field is open on the left side.
        /// </summary>
        internal readonly bool IsLeftOpen;

        /// <summary>
        /// Indicates, whether the field is open on the upper side.
        /// </summary>
        internal readonly bool IsTopOpen;

        /// <summary>
        /// Indicates, whether the field is open on the right side.
        /// </summary>
        internal readonly bool IsRightOpen;

        /// <summary>
        /// Indicates, whether the field is open on the lower side.
        /// </summary>
        internal readonly bool IsBottomOpen;

        /// <summary>
        /// Indicates, whether this field contains a treasure.
        /// </summary>
        internal readonly bool ContainsTreasure;

        /// <summary>
        /// Creates and initializes a new instance of the type Field.
        /// </summary>
        /// <param name="xsdCardType">The cardType, from which the Field will be initialized.</param>
        /// <param name="rowIndex">The index of the row in a board.</param>
        /// <param name="columnIndex">The index of the column in a board</param>
        internal Field(cardType xsdCardType, int rowIndex, int columnIndex)
            : this(rowIndex, columnIndex, xsdCardType.openings.left, xsdCardType.openings.top, xsdCardType.openings.right,
            xsdCardType.openings.bottom, xsdCardType.pin, xsdCardType.treasure, xsdCardType.treasureSpecified)
        {
        }

        /// <summary>
        /// Creates and initializes a new instance of the type Field.
        /// </summary>
        /// <param name="rowIndex">The index of the row where the field appears.</param>
        /// <param name="columnIndex">The index of the column where the field appears.</param>
        /// <param name="isLeftOpen">Indicates whether the field will be open on the left side.</param>
        /// <param name="isTopOpen">Indicates whether the field will be open on the upper side.</param>
        /// <param name="isRightOpen">Indicates whether the field will be open on the right side.</param>
        /// <param name="isBottomOpen">Indicates whether the field will be open on the lower side.</param>
        /// <param name="containingPlayers">An array of player-IDs that the field contains.</param>
        /// <param name="treasure">The treasure that the field contains.</param>
        /// <param name="isTreasureSpecified">Indicates whether the field contains a treasure.</param>
        internal Field(int rowIndex, int columnIndex, bool isLeftOpen, bool isTopOpen, bool isRightOpen, bool isBottomOpen,
            int[] containingPlayers, treasureType treasure, bool isTreasureSpecified)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;

            IsLeftOpen = isLeftOpen;
            IsTopOpen = isTopOpen;
            IsRightOpen = isRightOpen;
            IsBottomOpen = isBottomOpen;

            ContainingPlayers = (int[])containingPlayers.Clone();

            Treasure = treasure;
            ContainsTreasure = isTreasureSpecified;
        }
    }
}