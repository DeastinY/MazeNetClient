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

        public Move PlayNextMove(Board currentBoard, int playerRowIndex, int playerColumnIndex, treasureType nextTreasure)
        {
            Write("Enter new shiftPosition (row,col): ");
            string input = GetInput();
            int rowShift = int.Parse(input[0].ToString());
            int colShift = int.Parse(input[2].ToString());


            Write("Enter new pinposition (row,col): ");
            input = GetInput();
            int rowPin = int.Parse(input[0].ToString());
            int colPin = int.Parse(input[2].ToString());

            return new Move(rowPin, colPin, rowShift, colShift, "TODO: Hier kann eine rotation rein");
        }
    }
}