using System;
using MazeNetClient.Game;
using MazeNetClient.XSDGenerated;

namespace MazeNetClient.AI
{
    class UserInputMazePlayer : IMazePlayer
    {
        string GetInput()
        {
            return Console.ReadLine();
        }

        void Write(string value)
        {
            Logger.Write(value);
        }

        public MoveMessageType PlayNextMove(Board currentField)
        {
            //TODO: Frage: Was soll in diese nextMove.shiftCard variable rein?
            MoveMessageType nextMove = new MoveMessageType();
            nextMove.newPinPos = new positionType();
            nextMove.shiftCard = new cardType();
            nextMove.shiftPosition = new positionType();

            Write("Enter new shiftPosition (row,col): ");
            string input = GetInput();
            int row = int.Parse(input[0].ToString());
            int col = int.Parse(input[2].ToString());
            nextMove.shiftPosition.row = row;
            nextMove.shiftPosition.col = col;

            var field = currentField.ShiftCard;
            nextMove.shiftCard.openings = new cardTypeOpenings
            {
                bottom = field.IsBottomOpen,
                left = field.IsLeftOpen,
                top = field.IsTopOpen,
                right = field.IsRightOpen
            };
            nextMove.shiftCard.pin = field.ContainingPlayers;
            nextMove.shiftCard.treasure = field.Treasure;
            nextMove.shiftCard.treasureSpecified = field.ContainsTreasure;


            Write("Enter new pinposition (row,col): ");
            input = GetInput();
            row = int.Parse(input[0].ToString());
            col = int.Parse(input[2].ToString());
            nextMove.newPinPos.row = row;
            nextMove.newPinPos.col = col;

            return nextMove;
        }
    }
}
