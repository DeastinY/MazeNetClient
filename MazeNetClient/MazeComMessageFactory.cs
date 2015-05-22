using MazeNetClient.XSDGenerated;

namespace MazeNetClient
{
    static class MazeComMessageFactory
    {
        internal static MazeCom CreateLoginMessage(string name)
        {
            var mazeCom = new MazeCom
            {
                Item = new LoginMessageType { name = name }
            };
            return mazeCom;
        }

        internal static MazeCom CreateMoveMessage(int playerId)
        {
            var moveMessage = new MoveMessageType { };

            var mazeCom = new MazeCom
            {
                Item = moveMessage,
                mcType = MazeComType.MOVE,
                id = playerId
            };
            return mazeCom;
        }
    }
}