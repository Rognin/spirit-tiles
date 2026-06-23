using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts;

public partial class HexGridData : Node
{
    [Export] private Dictionary<Vector2I, GridCellData> _cells = new();
    public int NumberOfRows { get; set; }
    public int NumberOfColumns { get; set; }
    
    [Signal] public delegate void CellCreatedEventHandler(int row, int column, TileLogic newTile);
    [Signal] public delegate void CellMovedEventHandler(int rowFrom, int colFrom, int rowTo, int columnTo, TileLogic Tile);
    [Signal] public delegate void CellDestroyedEventHandler(int row, int column);
    
    public void Initialize(int numberOfRows, int numberOfColumns)
    {
        NumberOfRows = numberOfRows;
        NumberOfColumns = numberOfColumns;
        FillWithEmptyCells();
    }

    private void FillWithEmptyCells()
    {
        for (int i = 0; i < NumberOfRows; i++)
        {
            for (int j = 0; j < NumberOfColumns; j++)
            {
                _cells.Add(new Vector2I(i, j), new GridCellData(null));
            }
        }
    }

    private Vector2I GetTileCoordinatesById(int id)
    {
        foreach (var kvp in _cells)
        {
            if (!kvp.Value.IsEmpty())
            {
                if (kvp.Value.Tile.Id == id)
                {
                    return kvp.Key;
                }
            }
        }
        GD.PrintErr("Couldn't find tile with id: " + id);
        return Vector2I.Zero;
    }

    public bool ValidateMove(int id, bool valid, Vector2I newCoords, out Vector2I correctCoords)
    {
        // check that there's no tile already at new position
        if (!_cells[newCoords].IsEmpty())
        {
            valid = false;
        }
        
        if (valid)
        {
            correctCoords = newCoords;
            // GD.Print($"HexGridData validates coords: {correctCoords}");
            return true;
        }
        else
        {
            // correct the position to where tile is before move
            correctCoords = GetTileCoordinatesById(id);
            return false;
        }
    }

    public void CreateBasicTile(int row, int col, SharedTileData newTileData, int id)
    {
        if (newTileData == null)
        {
            GD.PrintErr("HexGridData.AddTile called with null MyTileData");
            return;
        }
        if (!_cells[new Vector2I(row, col)].IsEmpty())
        {
            GD.PrintErr($"There's already a cell at {row}, {col}");
            return;
        }

        TileLogic newTile = new TileLogic(newTileData, id);
        _cells[new Vector2I(row, col)].Tile = newTile;
        EmitSignal(SignalName.CellCreated, row, col, newTile);
    }

    public void DestroyTile(int row, int col)
    {
        _cells[new Vector2I(row, col)].Tile = null;
        EmitSignal(SignalName.CellDestroyed, row, col);
    }

    public void MoveTile(int id, int rowTo, int colTo)
    {
        Vector2I from = GetTileCoordinatesById(id);
        Vector2I to = new Vector2I(rowTo, colTo);
        // remove tile at starting cell
        if (_cells[from].IsEmpty())
        {
            GD.PrintErr("Trying to move a tile from a spot where there's no tile");
            return;
        }
        TileLogic tileToMove = _cells[from].Tile;
        _cells[from].Tile = null;
        
        // add the tile to the new position
        if (!_cells[to].IsEmpty())
        {
            // print error, TODO: add behavior for cell merging
            GD.PrintErr("Trying to move a tile to an occupied cell");
            return;
        }
        _cells[to].Tile = tileToMove;
        EmitSignal(SignalName.CellMoved, from.X, from.Y, to.X, to.Y, tileToMove);
    }

    public Dictionary<Vector2I, GridCellData> GetCurrentCells()
    {
        return _cells;
    }

    public void PrintCurrentCells()
    {
        foreach (var kvp in _cells)
        {
            if (!kvp.Value.IsEmpty())
            {
                GD.Print(kvp.Value.Tile.SharedTileData.Type);
            }
            else
            {
                GD.Print($"No cell at {kvp.Key}");
            }
        }
    }
}