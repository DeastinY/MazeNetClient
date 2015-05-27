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
                var nextTreasure = awaitMoveMessage.treasure;
                var currentBoard = new Board(awaitMoveMessage);

                Debug.Assert(currentBoard.Count(f => f.ContainsPlayer(m_clientId)) == 1);
                var fieldWithPlayer = currentBoard.First(f => f.ContainsPlayer(m_clientId));

                var nextMove = m_mazePlayer.PlayNextMove(currentBoard, fieldWithPlayer.RowIndex, fieldWithPlayer.ColumnIndex, nextTreasure);
                MoveMessageType moveMessage = new MoveMessageType
                {
                    newPinPos = new positionType { row = nextMove.NewPinPosRowIndex, col = nextMove.NewPinPosColumnIndex },
                    shiftCard = awaitMoveMessage.board.shiftCard, /*//TODO: Frage Was soll ich hier hin tun?*/ // kann noch rotieren
                    shiftPosition = new positionType { row = nextMove.ShiftPositionRowIndex, col = nextMove.ShiftPositionColumnIndex }
                };

                SendMoveMessage(moveMessage);

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

            //TODO: Frage: Aren't these information redundant? accept and errorCode == NOERROR?
            if (!acceptMessage.accept || acceptMessage.errorCode != ErrorType.NOERROR)
            {
                Logger.WriteLine("AcceptMessage with error: " + acceptMessage.errorCode);
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