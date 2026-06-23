using Godot;

namespace Spirittiles.Scripts;

[GlobalClass]
public partial class SharedTileData : Resource
{
    [Export] public Texture2D Texture { get; set; }
    [Export] public TileType Type { get; set; }
    
    [Export] public Color Color { get; set; }
}