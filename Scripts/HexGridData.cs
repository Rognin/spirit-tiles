using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts;

// [Tool]
// [GlobalClass]
public partial class HexGridData : Node
{
    // [Export] private Resource _testMyTileData;
    // [ExportToolButton("My Button Label")]
    // public Callable MyButton => Callable.From(() => AddTile(0, 0, _testMyTileData as MyTileData));
    
    [Export] private Dictionary<Vector2I, GridCellData> _cells = new();
    public int NumberOfRows { get; set; }
    public int NumberOfColumns { get; set; }
    
    [Signal] public delegate void CellChangedEventHandler(int row, int column, MyTileData oldTileData, MyTileData newTileData);

    public void Initialize(int numberOfRows, int numberOfColumns)
    {
        NumberOfRows = numberOfRows;
        NumberOfColumns = numberOfColumns;
        FillWithEmptyCells();
    }

    public void FillWithEmptyCells()
    {
        for (int i = 0; i < NumberOfRows; i++)
        {
            for (int j = 0; j < NumberOfColumns; j++)
            {
                _cells.Add(new Vector2I(i, j), new GridCellData());
            }
        }
    }

    public void AddTile(int row, int col, MyTileData newMyTileData)
    {
        if (newMyTileData == null)
        {
            GD.PrintErr("AddTile called with null MyTileData");
            return;
        }
        if (!_cells.ContainsKey(new Vector2I(row, col)))
        {
            GD.PrintErr($"No cell at {row}, {col}");
            return;
        }
        MyTileData oldTileData = _cells[new Vector2I(row, col)].CurrentMyTileData;
        _cells[new Vector2I(row, col)].CurrentTileType = newMyTileData.Type;
        EmitSignal(SignalName.CellChanged, row, col, oldTileData, newMyTileData);
    }

    public Dictionary<Vector2I, GridCellData> GetCurrentCells()
    {
        return _cells;
    }

    public void PrintCurrentCells()
    {
        foreach (var kvp in _cells)
        {
            GD.Print(kvp.Value.CurrentTileType);
        }
    }
}