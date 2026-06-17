using System;
using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts;

public partial class HexGridDisplay : Node2D
{
    
    [Export] private float _tileScale = 1;
    public const float TILE_EDGE_SIZE = 44.325f;
    public static readonly float TILE_WIDTH = TILE_EDGE_SIZE * MathF.Sqrt(3);
    public const float TILE_HEIGHT = TILE_EDGE_SIZE * 2;
    
    private int _numberOfRows;
    private int _numberOfColumns;
    
    [Export] private PackedScene _tileScene;

    private Dictionary<Vector2I, Tile> _placedTiles = new Dictionary<Vector2I, Tile>();
    public void Initialize(HexGridData hexGridData)
    {
        _numberOfRows = hexGridData.NumberOfRows;
        _numberOfColumns = hexGridData.NumberOfColumns;
        foreach (var kvp in hexGridData.GetCurrentCells())
        {
            GridCellData cell = kvp.Value;
            Vector2I coords = kvp.Key;
            TileTypes tileType = cell.CurrentTileType;
            if (!cell.IsEmpty())
            {
                PlaceAndDrawTile(coords, cell.CurrentMyTileData);
            }
        }
    }

    public void PlaceAndDrawTile(Vector2I coords, MyTileData data)
    {
        // place tile
        Tile tile = _tileScene.Instantiate<Tile>();
        tile.Position = CenterFromRowColumn(coords.X, coords.Y);
        AddChild(tile);
        tile.Initialize(data);
        _placedTiles[coords] = tile;
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, 0), 5f, Colors.White);
        for (int i = 0; i < _numberOfRows; i++)
        {
            for (int j = 0; j < _numberOfColumns; j++)
            {
                DrawPolyline(VerticesFromCenter(CenterFromRowColumn(i, j)), Colors.LightGray, 5);
            }
        }
    }

    private Vector2[] VerticesFromCenter(Vector2 center)
    {
        Vector2[] vertices = new Vector2[7];
        vertices[0] = new Vector2(center.X - TILE_WIDTH / 2, center.Y - TILE_EDGE_SIZE / 2);
        vertices[1] = new Vector2(center.X, center.Y - TILE_EDGE_SIZE);
        vertices[2] = new Vector2(center.X + TILE_WIDTH / 2, center.Y - TILE_EDGE_SIZE / 2);
        vertices[3] = new Vector2(center.X + TILE_WIDTH / 2, center.Y + TILE_EDGE_SIZE / 2);
        vertices[4] = new Vector2(center.X, center.Y + TILE_EDGE_SIZE);
        vertices[5] = new Vector2(center.X - TILE_WIDTH / 2, center.Y + TILE_EDGE_SIZE / 2);
        vertices[6] = new Vector2(center.X - TILE_WIDTH / 2, center.Y - TILE_EDGE_SIZE / 2);
		
        return vertices;
    }

    private Vector2 CenterFromRowColumn(int row, int col)
    {
        // odd rows are shorter, even rows are longer 
        float x = (row % 2) * TILE_WIDTH / 2 + (col * 2 + 1) * TILE_WIDTH / 2;
        float y = TILE_HEIGHT / 2 * (row * 1.5f + 1);
		
        return new Vector2(x, y);
    }
}