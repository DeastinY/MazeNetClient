namespace MazeNetClient.Game
{
    interface IFieldCollection : System.Collections.Generic.IEnumerable<Field>
    {
        Field this[int rowIndex, int columnIndex] { get; }
    }
}