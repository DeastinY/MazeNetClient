using System;

namespace MazeNetClient.Game
{
    /// <summary>
    /// Describes the sides of a field that are open.
    /// </summary>
    [Flags]
    enum Openings : byte
    {
        /// <summary>
        /// Left side of the field is open.
        /// </summary>
        Left = 1,

        /// <summary>
        /// Upper side of the field is open.
        /// </summary>
        Top = 2,

        /// <summary>
        /// Right side of the field is open.
        /// </summary>
        Right = 4,

        /// <summary>
        /// Lower side of the field is open.
        /// </summary>
        Bottom = 8
    }
}