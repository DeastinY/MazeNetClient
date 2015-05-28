using MazeNetClient.AI;
using MazeNetClient.Game;
using MazeNetClient.Network;
using MazeNetClient.XSDGenerated;
using System.Diagnostics;
using System.Linq;

namespace MazeNetClient
{
    class Communicator
    {
        private readonly ServerConnection m_connection;

        private readonly string m_name;

        private readonly IMazePlayer m_mazePlayer;

        private int m_clientId;

        internal Communicator(ServerConnection connection, string name, IMazePlayer mazePlayer)
        {
            m_connection = connection;
            m_name = name;
            m_mazePlayer = mazePlayer;
            m_clientId = -1;
        }

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

        private void StartGame()
        {
            var nextMessage = ReceiveMazeCom();

            while (nextMessage.mcType == MazeComType.AWAITMOVE)
            {
                Debug.Assert(nextMessage.id == m_clientId);

                var awaitMoveMessage = (AwaitMoveMessageType)nextMessage.Item;
                var currentBoard = new Board(awaitMoveMessage, m_clientId);
                var nextMove = GenerateNextMove(currentBoard, awaitMoveMessage.board.shiftCard);

                SendMoveMessage(nextMove);

                nextMessage = ReceiveMazeCom();
            }

            FinishGame(nextMessage);
        }

        private void SendMazeCom(MazeCom mazeComObject)
        {
            var stringMessage = mazeComObject.ConvertToString();
            m_connection.SendMessage(stringMessage);
        }

        private MazeCom ReceiveMazeCom()
        {
            var stringMessage = m_connection.ReceiveMessage();
            var mazeComObject = stringMessage.ConvertToMazeCom();
            return mazeComObject;
        }

        private MoveMessageType GenerateNextMove(Board currentBoard, cardType shiftCard)
        {
            Debug.Assert(currentBoard.Count(f => f.ContainsPlayer(m_clientId)) == 1);
            var fieldWithPlayer = currentBoard.First(f => f.ContainsPlayer(m_clientId));

            var nextMove = m_mazePlayer.PlayNextMove(currentBoard, fieldWithPlayer.RowIndex, fieldWithPlayer.ColumnIndex, currentBoard.TreasureTarget);

            ApplyRotation(shiftCard, nextMove.ShiftCardRotation);

            MoveMessageType moveMessage = new MoveMessageType
            {
                newPinPos = new positionType { row = nextMove.NewPinPosRowIndex, col = nextMove.NewPinPosColumnIndex },
                shiftCard = shiftCard,
                shiftPosition = new positionType { row = nextMove.ShiftPositionRowIndex, col = nextMove.ShiftPositionColumnIndex }
            };
            return moveMessage;
        }

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

        private void FinishGame(MazeCom finalMazeComMessage)
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
                Logger.WriteLine("Disconnected from server.");
            }
        }
    }
}