

using Godot;

namespace Spirittiles.Scripts;

public interface ISnapAreaForTiles
{
    public bool TryGetSnapPosition(Vector2 worlPos, out Vector2 snapPos, out Vector2I snapCoords);
    
    public void OnTileDropped(TileVisual tileVisual, Vector2I snapCoords);

    public void OnTileMovedAway(TileVisual tileVisual);
}