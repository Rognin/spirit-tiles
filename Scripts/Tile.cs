using Godot;

namespace Spirittiles.Scripts;

public partial class Tile : Area2D
{
    [Export] private Sprite2D _sprite;
    private MyTileData _data;
    
    // dragging
    private bool _dragging = false;
    private Vector2 _dragStartOffset = Vector2.Zero;

    public override void _Ready()
    {
        InputEvent += OnInputEvent;
    }

    public void Initialize(MyTileData data)
    {
        if (data == null)
        {
            GD.PrintErr("Tile trying to initialize with a null data");
            return;
        }
        _data = data;
        _sprite.Texture = data.Texture;
    }

    public override void _Process(double delta)
    {
        if (_dragging)
        {
            GlobalPosition = GetGlobalMousePosition() + _dragStartOffset;
        }
    }

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if (mouseEvent.Pressed)
            {
                _dragging = true;
                _dragStartOffset = GlobalPosition - GetGlobalMousePosition();
            }
            else
            {
                _dragging = false;
            }
        }
    }
}