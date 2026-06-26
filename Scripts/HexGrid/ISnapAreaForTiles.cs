

using Godot;

namespace Spirittiles.Scripts.HexGrid;

public interface ISnapAreaForTiles
{
    public void NotifyAboutDropFromTileVisual(int id, Vector2 globalPos);
}