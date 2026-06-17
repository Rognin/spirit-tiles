using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts;

public partial class HexGridData : Node
{
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

    public void AddTile(int row, int col, MyTileData newTileData)
    {
        if (newTileData == null)
        {
            GD.PrintErr("AddTile called with null MyTileData");
            return;
        }
        if (!_cells.ContainsKey(new Vector2I(row, col)))
        {
            GD.PrintErr($"No cell at {row}, {col}");
            return;
        }
        MyTileData oldTileData = _cells[new Vector2I(row, col)].CurrentTileData;
        _cells[new Vector2I(row, col)].CurrentTileData = newTileData;
        EmitSignal(SignalName.CellChanged, row, col, oldTileData, newTileData);
    }

    public Dictionary<Vector2I, GridCellData> GetCurrentCells()
    {
        return _cells;
    }

    public void PrintCurrentCells()
    {
        foreach (var kvp in _cells)
        {
            if (kvp.Value.CurrentTileData != null)
            {
                GD.Print(kvp.Value.CurrentTileData.Type);
            }
            else
            {
                GD.Print($"No cell at {kvp.Key}");
            }
        }
    }
}