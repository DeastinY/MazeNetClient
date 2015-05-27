using MazeNetClient.XSDGenerated;
using System;

namespace MazeNetClient.Game
{
    /// <summary>
    /// Defines a single field into a board.
    /// </summary>
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
        public Field(cardType xsdCardType, int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;

            var openings = xsdCardType.openings;
            IsLeftOpen = openings.left;
            IsTopOpen = openings.top;
            IsRightOpen = openings.right;
            IsBottomOpen = openings.bottom;

            var pin = xsdCardType.pin;
            ContainingPlayers = new int[pin.Length];
            Array.Copy(pin, ContainingPlayers, pin.Length);

            Treasure = xsdCardType.treasure;
            ContainsTreasure = xsdCardType.treasureSpecified;
        }

        //TODO: Frage: was ist das optional attribute in der boardtype->row[0]->
    }
}