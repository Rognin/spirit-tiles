using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts;

public partial class HexGridData : Node
{
    [Export] private Dictionary<Vector2I, GridCellData> _cells = new();
    public int NumberOfRows { get; set; }
    public int NumberOfColumns { get; set; }
    
    [Signal] public delegate void CellCreatedEventHandler(int row, int column, MyTileData newTileData);

    public void Initialize(int numberOfRows, int numberOfColumns)
    {
        NumberOfRows = numberOfRows;
        NumberOfColumns = numberOfColumns;
        // FillWithEmptyCells();
    }

    /*public void FillWithEmptyCells()
    {
        for (int i = 0; i < NumberOfRows; i++)
        {
            for (int j = 0; j < NumberOfColumns; j++)
            {
                _cells.Add(new Vector2I(i, j), new GridCellData());
            }
        }
    }*/

    public void CreateTile(int row, int col, MyTileData newTileData)
    {
        if (newTileData == null)
        {
            GD.PrintErr("AddTile called with null MyTileData");
            return;
        }
        if (_cells.ContainsKey(new Vector2I(row, col)))
        {
            GD.PrintErr($"There's already a cell at {row}, {col}");
            return;
        }
        _cells.Add(new Vector2I(row, col), new GridCellData(newTileData));
        EmitSignal(SignalName.CellCreated, row, col, newTileData);
    }

    public void RemoveTile(int row, int col)
    {
        _cells.Remove(new Vector2I(row, col));
    }

    public void AddTileAfterMove(int row, int col, MyTileData tileData)
    {
        if (_cells.ContainsKey(new Vector2I(row, col)))
        {
            // print error, TODO: change so some tiles can be added on top of each other
            GD.PrintErr("HexGridData trying to add a tile after move where there's already a tile");
        }
        else
        {
            _cells.Add(new Vector2I(row, col), new GridCellData(tileData));
            
        }
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