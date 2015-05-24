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
        /// and returns a MoveMessageType, containing his next move.
        /// </summary>
        /// <param name="currentField">The board, where the algorithm will be applied to.</param>
        /// <returns>The result of the IMazePlayers next move.</returns>
        MoveMessageType PlayNextMove(Game.Board currentField);
    }
}