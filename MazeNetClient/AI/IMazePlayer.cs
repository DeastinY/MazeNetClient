using MazeNetClient.Game;
using MazeNetClient.XSDGenerated;

namespace MazeNetClient.AI
{
    /// <summary>
    /// Defines a method for every class, that provides a solution for the game: "Das verrückte Labyrinth".
    /// </summary>
    interface IMazePlayer
    {
        /// <summary>
        /// The artificial player receives the specified board, applies his algorithm
        /// and returns his next move.
        /// </summary>
        /// <param name="currentBoard">The board, where the algorithm will be applied to.</param>
        /// <param name="playerRowIndex">The index of the row, where the player stands.</param>
        /// <param name="playerColumnIndex">The index of the column, where the player stands.</param>
        /// <param name="nextTreasure">Describes the treasure, that the player needs to get.</param>
        /// <returns>The result of the IMazePlayers next move.</returns>
        Move PlayNextMove(Board currentBoard, int playerRowIndex, int playerColumnIndex, treasureType nextTreasure);
    }
}