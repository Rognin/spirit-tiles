using Godot;

namespace Spirittiles.Scripts;

public partial class TileVisual : Area2D
{
    [Export] private Sprite2D _sprite;
    private int _id;

    // dragging
    private bool _dragging = false;
    private Vector2 _dragStartOffset = Vector2.Zero;
    
    // signals
    [Signal] public delegate void TileVisualDroppedEventHandler(int id, Vector2 worldPos);

    public override void _Ready()
    {
        InputEvent += OnInputEvent;
    }

    public void Initialize(int id, SharedTileData data)
    {
        if (data == null)
        {
            GD.PrintErr("Tile trying to initialize with a null data");
            return;
        }
        _sprite.Texture = data.Texture;
        _sprite.Modulate = data.Color;
        _id = id;
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
                ZIndex = 1;
                _dragStartOffset = GlobalPosition - GetGlobalMousePosition();
            }
            else
            {
                _dragging = false;
                ZIndex = 0;
                EmitSignal(SignalName.TileVisualDropped, _id, GlobalPosition);
            }
        }
    }
}