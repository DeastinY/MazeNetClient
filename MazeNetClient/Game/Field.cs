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
        /// Describes which sides of the field are open.
        /// </summary>
        internal readonly Openings OpenSides;

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
            : this(rowIndex, columnIndex, CreateOpeningsFromBool(xsdCardType.openings.left, xsdCardType.openings.top, xsdCardType.openings.right,
            xsdCardType.openings.bottom), xsdCardType.pin, xsdCardType.treasure, xsdCardType.treasureSpecified)
        {
        }

        /// <summary>
        /// Creates and initializes a new instance of the type Field.
        /// </summary>
        /// <param name="rowIndex">The index of the row where the field appears.</param>
        /// <param name="columnIndex">The index of the column where the field appears.</param>
        /// <param name="openSides">The open sides of the field.</param>
        /// <param name="containingPlayers">An array of player-IDs that the field contains.</param>
        /// <param name="treasure">The treasure that the field contains.</param>
        /// <param name="isTreasureSpecified">Indicates whether the field contains a treasure.</param>
        internal Field(int rowIndex, int columnIndex, Openings openSides,
            int[] containingPlayers, treasureType treasure, bool isTreasureSpecified)
        {
            System.Diagnostics.Debug.Assert(openSides.HasFlag(Openings.Left) || openSides.HasFlag(Openings.Top) || openSides.HasFlag(Openings.Right) || openSides.HasFlag(Openings.Bottom));

            RowIndex = rowIndex;
            ColumnIndex = columnIndex;

            OpenSides = openSides;

            ContainingPlayers = (int[])containingPlayers.Clone();

            Treasure = treasure;
            ContainsTreasure = isTreasureSpecified;
        }

        private static Openings CreateOpeningsFromBool(bool isLeftOpen, bool isTopOpen, bool isRightOpen, bool isBottomOpen)
        {
            Openings ret = 0;

            if (isLeftOpen) ret = Openings.Left;
            if (isTopOpen) ret |= Openings.Top;
            if (isRightOpen) ret |= Openings.Right;
            if (isBottomOpen) ret |= Openings.Bottom;

            return ret;
        }
    }
}