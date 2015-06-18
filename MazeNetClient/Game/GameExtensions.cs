using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MazeNetClient.Game
{
    static class GameExtensions
    {
        enum Direction
        {
            Left,
            Top,
            Right,
            Bottom,
            Unset
        }

        /// <summary>
        /// Get a list of fields, within the given board, that you can reach, starting from the specified position.
        /// </summary>
        /// <param name="board">The given board.</param>
        /// <param name="rowIndex">The index of the row, from where you start.</param>
        /// <param name="columnIndex">The index of the column, from where you start.</param>
        /// <returns>A list of reachable fields, excluding the field at the position from where you start.</returns>
        internal static List<Field> GetReachableFields(this IFieldCollection board, int rowIndex, int columnIndex)
        {
            List<Field> reachableFields = new List<Field>();

            GetReachableFieldsRecursive(reachableFields, board, rowIndex, columnIndex, Direction.Unset);

            return reachableFields;
        }

        static void GetReachableFieldsRecursive(List<Field> basket, IFieldCollection board, int rowIndex, int columnIndex, Direction fromDirection)
        {
            if (fromDirection != Direction.Left)
            {
                bool canGoLeft = CanGoLeft(board, rowIndex, columnIndex);
                if (canGoLeft)
                {
                    var leftNeighbour = board[rowIndex, columnIndex - 1];
                    if (!basket.Contains(leftNeighbour))
                    {
                        basket.Add(leftNeighbour);
                        GetReachableFieldsRecursive(basket, board, leftNeighbour.RowIndex, leftNeighbour.ColumnIndex, Direction.Right);
                    }
                }
            }

            if (fromDirection != Direction.Top)
            {
                bool canGoUp = CanGoUp(board, rowIndex, columnIndex);
                if (canGoUp)
                {
                    var upperNeighbour = board[rowIndex - 1, columnIndex];
                    if (!basket.Contains(upperNeighbour))
                    {
                        basket.Add(upperNeighbour);
                        GetReachableFieldsRecursive(basket, board, upperNeighbour.RowIndex, upperNeighbour.ColumnIndex, Direction.Bottom);
                    }
                }
            }

            if (fromDirection != Direction.Right)
            {
                bool canGoRight = CanGoRight(board, rowIndex, columnIndex);
                if (canGoRight)
                {
                    var rightNeighbour = board[rowIndex, columnIndex + 1];
                    if (!basket.Contains(rightNeighbour))
                    {
                        basket.Add(rightNeighbour);
                        GetReachableFieldsRecursive(basket, board, rightNeighbour.RowIndex, rightNeighbour.ColumnIndex, Direction.Left);
                    }
                }
            }

            if (fromDirection != Direction.Bottom)
            {
                bool canGoDown = CanGoDown(board, rowIndex, columnIndex);
                if (canGoDown)
                {
                    var lowerNeighbour = board[rowIndex + 1, columnIndex];
                    if (!basket.Contains(lowerNeighbour))
                    {
                        basket.Add(lowerNeighbour);
                        GetReachableFieldsRecursive(basket, board, lowerNeighbour.RowIndex, lowerNeighbour.ColumnIndex, Direction.Top);
                    }
                }
            }
        }

        /// <summary>
        /// Test, if you can go to the field, that is left from you.
        /// </summary>
        /// <param name="board">The board, for that we test.</param>
        /// <param name="rowIndex">The index of the row, where you are currently.</param>
        /// <param name="columnIndex">The index of the column, where you are currently.</param>
        /// <returns>True, if you can go left, false otherwise.</returns>
        internal static bool CanGoLeft(this IFieldCollection board, int rowIndex, int columnIndex)
        {
            if (columnIndex > 0)
            {
                return board[rowIndex, columnIndex].IsLeftOpen
                    && board[rowIndex, columnIndex - 1].IsRightOpen;
            }
            return false;
        }

        /// <summary>
        /// Test, if you can go to the field, that is above you.
        /// </summary>
        /// <param name="board">The board, for that we test.</param>
        /// <param name="rowIndex">The index of the row, where you are currently.</param>
        /// <param name="columnIndex">The index of the column, where you are currently.</param>
        /// <returns>True, if you can go up, false otherwise.</returns>
        internal static bool CanGoUp(this IFieldCollection board, int rowIndex, int columnIndex)
        {
            if (rowIndex > 0)
            {
                return board[rowIndex, columnIndex].IsTopOpen
                    && board[rowIndex - 1, columnIndex].IsBottomOpen;
            }
            return false;
        }

        /// <summary>
        /// Test, if you can go to the field, that is right from you.
        /// </summary>
        /// <param name="board">The board, for that we test.</param>
        /// <param name="rowIndex">The index of the row, where you are currently.</param>
        /// <param name="columnIndex">The index of the column, where you are currently.</param>
        /// <returns>True, if you can go right, false otherwise.</returns>
        internal static bool CanGoRight(this IFieldCollection board, int rowIndex, int columnIndex)
        {
            if (columnIndex + 1 < Board.COLUMN_COUNT)
            {
                return board[rowIndex, columnIndex].IsRightOpen
                    && board[rowIndex, columnIndex + 1].IsLeftOpen;
            }
            return false;
        }

        /// <summary>
        /// Test, if you can go to the field, that is under you.
        /// </summary>
        /// <param name="board">The board, for that we test.</param>
        /// <param name="rowIndex">The index of the row, where you are currently.</param>
        /// <param name="columnIndex">The index of the column, where you are currently.</param>
        /// <returns>True, if you can go down, false otherwise.</returns>
        internal static bool CanGoDown(this IFieldCollection board, int rowIndex, int columnIndex)
        {
            if (rowIndex + 1 < Board.ROW_COUNT)
            {
                return board[rowIndex, columnIndex].IsBottomOpen
                    && board[rowIndex + 1, columnIndex].IsTopOpen;
            }
            return false;
        }

        /// <summary>
        /// Get the number of treasures, that you can reach from the field that contains the player with the specified id.
        /// </summary>
        /// <param name="board">The board on that you search.</param>
        /// <param name="playerId">The id of the player from where we start.</param>
        /// <returns>The number of reachable treasures.</returns>
        internal static int GetNumberOfReachableTreasures(this IFieldCollection board, int playerId)
        {
            Debug.Assert(board.Count(f => f.ContainsPlayer(playerId)) == 1);

            var playerField = board.First(f => f.ContainsPlayer(playerId));
            var reachableFields = board.GetReachableFields(playerField.RowIndex, playerField.ColumnIndex);
            int numberOfFieldsWithATreasure = reachableFields.Count(rf => rf.ContainsTreasure);
            return numberOfFieldsWithATreasure;
        }

        /// <summary>
        /// Test, if the given field contains a player with the specified id.
        /// </summary>
        /// <param name="field">The given field, where we search the player in.</param>
        /// <param name="playerId">The id of the player.</param>
        /// <returns>True, if the field contains the player, false otherwise.</returns>
        internal static bool ContainsPlayer(this Field field, int playerId)
        {
            return field.ContainingPlayers.Any(p => p == playerId);
        }

        /// <summary>
        /// Test if the given field contains a treasure with the specified treasure type.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="treasure"></param>
        /// <returns>True if the field contains the treasure type, false otherwise.</returns>
        internal static bool HasTreasure(this Field field, XSDGenerated.treasureType treasure)
        {
            return field.ContainsTreasure && field.Treasure == treasure;
        }

        /// <summary>
        /// Returns the number of open paths on a board.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        internal static int GetNumberOfOpenPaths(this IFieldCollection board)
        {
            int numberOfOpenPaths = 0;

            for (int i = 0; i < Board.ROW_COUNT; ++i)
            {
                for (int j = 0; j < Board.COLUMN_COUNT; ++j)
                {
                    if (board.CanGoLeft(i, j)) ++numberOfOpenPaths;
                    if (board.CanGoUp(i, j)) ++numberOfOpenPaths;
                    if (board.CanGoRight(i, j)) ++numberOfOpenPaths;
                    if (board.CanGoDown(i, j)) ++numberOfOpenPaths;
                }
            }

            Debug.Assert(numberOfOpenPaths % 2 == 0);
            numberOfOpenPaths /= 2; //we counted each path twice
            return numberOfOpenPaths;
        }

        [Conditional("DEBUG")]
        internal static void AssertValidIFieldCollection(this IFieldCollection fieldCollection)
        {
            //The fieldCollection must contain our playerId exactly one time.
            Debug.Assert(fieldCollection.Count(f => f.ContainsPlayer(Board.Current.PlayerId)) == 1);

            foreach (var anEnemy in Board.Current.GetEnemyPlayers())
            {
                Debug.Assert(fieldCollection.Count(f => f.ContainsPlayer(anEnemy)) == 1);
            }

            //The fieldCollection must contain each treasure except of when one is in the shift card.
            var count = fieldCollection.Where(f => System.Enum.GetValues(typeof(XSDGenerated.treasureType)).Cast<XSDGenerated.treasureType>().Any(t => f.HasTreasure(t))).Count();
            var length = System.Enum.GetValues(typeof(XSDGenerated.treasureType)).Length;
            Debug.Assert((count == length) || (count == length - 1));
        }

        /// <summary>
        /// Returns the id of the player that comes after the specified current player.
        /// </summary>
        /// <param name="currentPlayerId">The id of the current player. It does not have to be the id of our player.</param>
        /// <returns>The id of the player that comes after the specified player.</returns>
        internal static int FollowingPlayer(this int currentPlayerId)
        {
            Debug.Assert(Board.Current.TreasuresToGo.Count(t => t.player == currentPlayerId) == 1);
            int nextPlayerId = -1;
            var followingTreasureToGoType = Board.Current.TreasuresToGo.FirstOrDefault(t => t.player > currentPlayerId);
            if (followingTreasureToGoType == null) //There is no player with an higher id
            {
                Debug.Assert(currentPlayerId != 1);
                Debug.Assert(Board.Current.TreasuresToGo.Min(t => t.player) < currentPlayerId);
                nextPlayerId = Board.Current.TreasuresToGo.Min(t => t.player);
            }
            else
            {
                nextPlayerId = followingTreasureToGoType.player;
            }

            Debug.Assert(Board.Current.TreasuresToGo.Count(t => t.player == nextPlayerId) == 1);
            return nextPlayerId;
        }

        /// <summary>
        /// Returns the id of the player that comes before the specified current player.
        /// </summary>
        /// <param name="currentPlayerId">The id of the current player. It does not have to be the id of our player.</param>
        /// <returns>The id of the player that comes before the specified player.</returns>
        internal static int PreviousPlayer(this int currentPlayerId)
        {
            int previousPlayer;
            for (previousPlayer = currentPlayerId.FollowingPlayer(); previousPlayer.FollowingPlayer() != currentPlayerId; previousPlayer = previousPlayer.FollowingPlayer()) ;
            return previousPlayer;
        }

        /// <summary>
        /// Test if the given field is symmetric.
        /// That means if it is only at the left and right side open, or only at the upper and lower side open.
        /// </summary>
        /// <param name="field">The given field to proof the symmetry for.</param>
        /// <returns>True if the given field is symmetric, false otherwise.</returns>
        internal static bool IsSymmetric(this Field field)
        {
            bool isVertical = field.IsLeftOpen && field.IsRightOpen && !field.IsTopOpen && !field.IsBottomOpen;
            bool isHorizontal = !field.IsLeftOpen && !field.IsRightOpen && field.IsTopOpen && field.IsBottomOpen;
            return isVertical || isHorizontal;
        }
    }
}