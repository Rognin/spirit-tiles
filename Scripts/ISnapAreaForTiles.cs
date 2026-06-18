

using Godot;

namespace Spirittiles.Scripts;

public interface ISnapAreaForTiles
{
    public bool TryGetSnapPosition(Vector2 worlPos, out Vector2 snapPos);
    
    public void OnTilePlaced(Tile tile);
}