

using Godot;

namespace Spirittiles.Scripts.HexGrid;

public interface ISnapAreaForTiles
{
    public bool TryGetSnapPosition(Vector2 worldPos, out Vector2I snapCoords);
    
    public void OnTileDropped(int id, Vector2 globalPos);

    // public void OnTileMovedAway(TileVisual tileVisual);
}