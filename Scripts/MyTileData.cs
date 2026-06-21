using Godot;

namespace Spirittiles.Scripts;

[GlobalClass]
public partial class MyTileData : Resource
{
    [Export] public Texture2D Texture { get; set; }
    [Export] public TileTypes Type { get; set; }
}