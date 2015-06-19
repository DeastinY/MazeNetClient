using MazeNetClient.AI;
using MazeNetClient.Game;
using MazeNetClient.Network;
using MazeNetClient.XSDGenerated;
using System.Diagnostics;
using System.Linq;

namespace MazeNetClient
{
    /// <summary>
    /// This class starts a communication with a MazeNetServer. It sends and retrieves messages that are
    /// interpreted and delegated by and to other components into this class.
    /// </summary>
    class Communicator
    {
        /// <summary>
        /// Holds the connection to the MazeNetServer.
        /// Via that member messages will be sent to / received from the MazeNetServer.
        /// </summary>
        private readonly ServerConnection m_connection;

        /// <summary>
        /// Holds a name of the client that will be shown to the MazeNetServer.
        /// </summary>
        private readonly string m_name;

        /// <summary>
        /// Describes the id of the client. The id will be distributed by the MazeNetServer.
        /// The id has to be contained within each message that we send to the MazeNetServer so he can
        /// identify us.
        /// </summary>
        private int m_clientId;

        /// <summary>
        /// Creates and initializes a new instance of the class Communicator.
        /// </summary>
        /// <param name="connection">The connection to the MazeNetServer.</param>
        /// <param name="name">The name of the client. This name will be shown to the MazeNetServer.</param>
        internal Communicator(ServerConnection connection, string name)
        {
            m_connection = connection;
            m_name = name;
            m_clientId = -1;

            //Reset the board-history, because maybe there are still boards left from a previous game.
            Board.ResetBoardHistory();

            //(Re-)Initialize the singleton of the treasure tracker.
            TreasureTracker.StartNewTracker();
        }

        /// <summary>
        /// Starts the communication to the MazeNetServer.
        /// This will also start the game and will lead into a loop that ends,
        /// when either a winner of the game is found of when we disconnect from the MazeNetServer.
        /// </summary>
        internal void StartCommunication()
        {
            //Send the LoginMessage
            var mazeComLogin = new MazeCom
            {
                Item = new LoginMessageType { name = m_name },
                mcType = MazeComType.LOGIN
            };
            SendMazeCom(mazeComLogin);

            //Receive the LoginReplyMessage
            var mazeComReplyMessage = ReceiveMazeCom();
            Debug.Assert(mazeComReplyMessage.mcType == MazeComType.LOGINREPLY);

            var loginMessageReply = (LoginReplyMessageType)mazeComReplyMessage.Item;
            Debug.Assert(loginMessageReply.newID >= 0);

            m_clientId = loginMessageReply.newID;

            Logger.WriteLine("Received Id: " + m_clientId + " from server.");
            StartGame();
        }

        /// <summary>
        /// Start the game. Receive messages, generate answers and go on
        /// until either a winner was found or we disconnected from the server.
        /// </summary>
        private void StartGame()
        {
            var nextMessage = ReceiveMazeCom();

            while (nextMessage.mcType == MazeComType.AWAITMOVE)
            {
                Debug.Assert(nextMessage.id == m_clientId);

                var timeStamp = -Stopwatch.GetTimestamp();

                var awaitMoveMessage = (AwaitMoveMessageType)nextMessage.Item;
                var currentBoard = new Board(awaitMoveMessage, m_clientId);
                var nextMove = GenerateNextMove(currentBoard, awaitMoveMessage.board.shiftCard);

                timeStamp += Stopwatch.GetTimestamp();
                Logger.WriteLine("Calculation time: " + (timeStamp / Stopwatch.Frequency) + "s");

                SendMoveMessage(nextMove);

                nextMessage = ReceiveMazeCom();
            }

            InterpretFinalMessage(nextMessage);
        }

        /// <summary>
        /// Convert the given mazeComObject to a string and send it via the connection to the MazeNetServer.
        /// </summary>
        /// <param name="mazeComObject">The given mazeComObject that will be send.</param>
        private void SendMazeCom(MazeCom mazeComObject)
        {
            var stringMessage = mazeComObject.ConvertToString();
            m_connection.SendMessage(stringMessage);
        }

        /// <summary>
        /// Receive a message from the MazeNetServer and convert it to a MazeComObject.
        /// </summary>
        /// <returns>The next MazeComObject sent by the server.</returns>
        private MazeCom ReceiveMazeCom()
        {
            var stringMessage = m_connection.ReceiveMessage();
            var mazeComObject = stringMessage.ConvertToMazeCom();
            return mazeComObject;
        }

        private MoveMessageType GenerateNextMove(Board currentBoard, cardType shiftCard)
        {
            Debug.Assert(currentBoard.Count(f => f.ContainsPlayer(m_clientId)) == 1);

            var nextMove = Evaluator.GetBestMove(currentBoard);

            ApplyRotation(shiftCard, nextMove.ShiftCardRotation);

            MoveMessageType moveMessage = new MoveMessageType
            {
                newPinPos = new positionType { row = nextMove.NewPinPosRowIndex, col = nextMove.NewPinPosColumnIndex },
                shiftCard = shiftCard,
                shiftPosition = new positionType { row = nextMove.ShiftPositionRowIndex, col = nextMove.ShiftPositionColumnIndex }
            };
            return moveMessage;
        }

        /// <summary>
        /// Wraps the given moveMessage in a MazeComObject and sends it to the server.
        /// Waits until the MazeNetServer answers with an accept-message.
        /// </summary>
        /// <param name="moveMessage">The given moveMessage that wil be send.</param>
        private void SendMoveMessage(MoveMessageType moveMessage)
        {
            //Create a MazeCom-object with the moveMessage as member
            var mazeComMoveMessage = new MazeCom
            {
                id = m_clientId,
                Item = moveMessage,
                mcType = MazeComType.MOVE
            };

            //Send the MazeCom-object
            SendMazeCom(mazeComMoveMessage);

            //Receive the AcceptMessage as answer to the MoveMessage
            var mazeComAcceptMessage = ReceiveMazeCom();
            Debug.Assert(mazeComAcceptMessage.mcType == MazeComType.ACCEPT);

            var acceptMessage = (AcceptMessageType)mazeComAcceptMessage.Item;
            Debug.Assert(acceptMessage.accept && acceptMessage.errorCode == ErrorType.NOERROR);

            if (!acceptMessage.accept || acceptMessage.errorCode != ErrorType.NOERROR)
            {
                Logger.WriteLine("AcceptMessage with error: " + acceptMessage.errorCode);
            }
        }

        /// <summary>
        /// Applies the given rotation to the given shift-card.
        /// This will affect the openings of the shift-card only.
        /// </summary>
        /// <param name="shiftCard">The given shift-card whose openings will be modified.</param>
        /// <param name="rotation">The rotation by that the shift-card will be rotated.</param>
        private void ApplyRotation(cardType shiftCard, AI.Rotation rotation)
        {
            var openings = shiftCard.openings;
            var oldLeft = openings.left;
            var oldTop = openings.top;
            var oldRight = openings.right;
            var oldBottom = openings.bottom;

            switch (rotation)
            {
                case Rotation.DEGREE_0:
                    //No rotation, nothing to do here.
                    break;
                case Rotation.DEGREE_90:
                    openings.left = oldBottom;
                    openings.top = oldLeft;
                    openings.right = oldTop;
                    openings.bottom = oldRight;
                    break;
                case Rotation.DEGREE_180:
                    openings.left = oldRight;
                    openings.top = oldBottom;
                    openings.right = oldLeft;
                    openings.bottom = oldTop;
                    break;
                case Rotation.DEGREE_270:
                    openings.left = oldTop;
                    openings.top = oldRight;
                    openings.right = oldBottom;
                    openings.bottom = oldLeft;
                    break;
                default:
                    Debug.Assert(false, "Invalid value of enum rotation: " + rotation);
                    break;
            }
        }

        /// <summary>
        /// Interprets the given MazeCom-object that can be either a win- or a disconnect-message.
        /// </summary>
        /// <param name="finalMazeComMessage">The given MazeCom-object that describes the last message from the server before the game finishes.</param>
        private void InterpretFinalMessage(MazeCom finalMazeComMessage)
        {
            if (finalMazeComMessage.mcType == MazeComType.WIN)
            {
                //Handle the WinMessage
                var winMessage = (WinMessageType)finalMazeComMessage.Item;
                var winnerId = winMessage.winner.id;
                var winnerValue = winMessage.winner.Value;
                Logger.WriteLine("WinMessage arrived! ID: " + winnerId + " Value: " + winnerValue);
                if (winnerId == m_clientId)
                {
                    Logger.WriteLine("SAUFEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEN!");
                }
            }
            else
            {
                Debug.Assert(finalMazeComMessage.mcType == MazeComType.DISCONNECT);
                var disconnectMessage = (DisconnectMessageType)finalMazeComMessage.Item;
                Logger.WriteLine("Disconnected from server with error code: " + disconnectMessage.errorCode + " and name: " + disconnectMessage.name);
            }
        }
    }
}