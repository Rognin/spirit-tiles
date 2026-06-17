using Godot;

namespace Spirittiles.Scripts;

public partial class Tile : Area2D
{
    [Export] private Sprite2D _sprite;
    private MyTileData _data;

    public void Initialize(MyTileData data)
    {
        if (data == null)
        {
            GD.PrintErr("Tile data is null");
            return;
        }
        _data = data;
        _sprite.Texture = data.Texture;
    }
}