using System;
using Godot;
using Godot.Collections;
using Spirittiles.Scripts.HexGrid;

namespace Spirittiles.Scripts;

public partial class HexGridDisplay : Node2D, ISnapAreaForTiles
{
    
    [Export] private float _tileScale = 1;
    public const float TILE_EDGE_SIZE = 44.325f;
    public static readonly float TILE_WIDTH = TILE_EDGE_SIZE * MathF.Sqrt(3);
    public const float TILE_HEIGHT = TILE_EDGE_SIZE * 2;
    [Export] float maxSnapDistance = TILE_HEIGHT/2;
    
    private int _numberOfRows;
    private int _numberOfColumns;
    
    [Export] private PackedScene _tileScene;
    
    [Export] private Area2D _snapArea;
    [Export] private CollisionShape2D _snapAreaCollider;

    private Dictionary<int, TileVisual> _placedTiles = new Dictionary<int, TileVisual>();
    
    private Dictionary<Vector2I, Vector2> _cellCenters = new Dictionary<Vector2I, Vector2>();
    
    [Signal] public delegate void TileDroppedEventHandler(int id, bool valid, Vector2I coords);
    [Signal] public delegate void ForwardTileDropEventHandler(int id, Vector2 globalPos);
    
    // [Signal] public delegate void TileMovedAwayEventHandler(int id);
    public void Initialize(int numberOfRows, int numberOfColumns)
    {
        _numberOfRows = numberOfRows;
        _numberOfColumns = numberOfColumns;
        
        // fill the array of hex center coords
        BuildCellCentersDict();
    }

    public void CreateTileVisual(Vector2I coords, TileLogic tile)
    {
        // place tile
        TileVisual tileVisual = _tileScene.Instantiate<TileVisual>();
        tileVisual.Position = CenterFromRowColumn(coords.X, coords.Y);
        AddChild(tileVisual);
        tileVisual.Initialize(tile.Id, tile.SharedTileData);
        _placedTiles[tile.Id] = tileVisual;
    }

    public void DestroyTileVisual(int id)
    {
        TileVisual toRemove = _placedTiles[id];
        _placedTiles.Remove(id);
        toRemove.QueueFree();
    }

    private void BuildCellCentersDict()
    {
        _cellCenters.Clear();
        for (int i = 0; i < _numberOfRows; i++)
        {
            for (int j = 0; j < _numberOfColumns; j++)
            {
                _cellCenters.Add(new Vector2I(i, j), CenterFromRowColumn(i, j));
            }
        }
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
    
    // tile placement related methods

    public void NotifyAboutDropFromTileVisual(int id, Vector2 globalPos)
    {
        GD.Print("step 1: display forwards to manager");
        ForwardTileDropped(id, globalPos);
    }
    
    private void ForwardTileDropped(int id, Vector2 globalPos)
    {
        EmitSignal(SignalName.ForwardTileDrop, id, globalPos);
    }

    public void MoveTileWithinThisGrid(int id, Vector2 globalPos)
    {
        bool validDrop = TryGetSnapPosition(globalPos, out Vector2I snapCoords);
        
        EmitSignal(SignalName.TileDropped, id, validDrop, snapCoords);
    }

    public void CancelMove(int id, Vector2I correctionCoords)
    {
        PlaceTileVisualAtCoords(id, correctionCoords);
    }

    public TileVisual SendOffVisual(int id)
    {
        TileVisual toSend = _placedTiles[id];
        _placedTiles.Remove(id);
        RemoveChild(toSend);
        return toSend;
    }

    public void ReceiveNewTileVisual(int id, TileVisual visual, Vector2I coords)
    {
        _placedTiles.Add(id, visual);
        AddChild(visual);
        PlaceTileVisualAtCoords(id, coords);
    }

    public void PlaceTileVisualAtCoords(int id, Vector2I newCoords)
    {
        _placedTiles[id].Position = CenterFromRowColumn(newCoords.X, newCoords.Y);
    }
    
    // grid coordinates related methods

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
    
    

    private Vector2 CalculateGridSizeInPixels()
    {
        Vector2 result = new Vector2();
        result.X = (_numberOfColumns + 0.5f) * HexGridDisplay.TILE_WIDTH;
        result.Y = _numberOfRows * (HexGridDisplay.TILE_HEIGHT * 0.75f) + 0.25f * HexGridDisplay.TILE_HEIGHT;
        return result;
    }
    
    public void UpdateSnapAreaColliderShape()
    {
        Vector2 gridSize = CalculateGridSizeInPixels();
        RectangleShape2D colliderShape = new RectangleShape2D();
        colliderShape.Size = gridSize;
        _snapAreaCollider.Shape = colliderShape;
        _snapAreaCollider.Position = gridSize / 2f;
    }
    
    public bool TryGetSnapPosition(Vector2 worldPos, out Vector2I snapCoords)
    {
        Vector2 localPos = worldPos - GlobalPosition;
        float minDist = float.MaxValue;
        Vector2I closestCellCoord = default;
        foreach (var (coord, center) in _cellCenters)
        {
            float distSqured = localPos.DistanceSquaredTo(center);
            if (distSqured < minDist)
            {
                minDist = distSqured;
                closestCellCoord = coord;
            }
        }

        if (minDist == float.MaxValue) // empty grid
        {
            snapCoords = Vector2I.Zero;
            return false;
        }
        
        if (minDist > maxSnapDistance * maxSnapDistance) // dropped out of grid
        {
            snapCoords = Vector2I.Zero;
            return false;
        }

        snapCoords = new Vector2I(closestCellCoord.X, closestCellCoord.Y);
        return true;
    }
    // called when a tile is moved from one cell to another
    // including to a different snap area
    /*public void OnTileMovedAway(int id)
    {
        EmitSignal(SignalName.TileMoved, tileVisual);
    }*/
}
