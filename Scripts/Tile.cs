using Godot;

namespace Spirittiles.Scripts;

public partial class Tile : Area2D
{
    [Export] private Sprite2D _sprite;
    public MyTileData Data;
    public Vector2 SnapBackPosition { get; set; }
    public int Id { get; set; }
    
    public ISnapAreaForTiles CurrentSnapArea { get; set; }

    public Vector2I CurrentCoordinates { get; set; }

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
        Data = data;
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
                TryToSnap();
            }
        }
    }

    // method that gets called when the tile is dropped after dragging
    private void TryToSnap()
    {
        foreach (Area2D area in GetOverlappingAreas())
        {
            if (area.GetParent() is ISnapAreaForTiles snapArea &&
                snapArea.TryGetSnapPosition(GlobalPosition, out Vector2 SnapPos, out Vector2I snapCoords))
            {
                GlobalPosition = SnapPos;
                snapArea.OnTileDropped(this, snapCoords);
                return;
            }
        }
    }
}