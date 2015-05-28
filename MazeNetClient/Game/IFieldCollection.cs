namespace MazeNetClient.Game
{
    interface IFieldCollection
    {
        Field this[int rowIndex, int columnIndex] { get; }
    }
}