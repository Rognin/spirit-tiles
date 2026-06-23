using Godot;

namespace Spirittiles.Scripts;

public partial class TileLogic(SharedTileData newData, int id) : Node
{
    public SharedTileData SharedTileData { get; set; } = newData;
    public int Id { get; set; } = id;
}