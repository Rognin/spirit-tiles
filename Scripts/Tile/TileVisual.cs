using Godot;
using Godot.Collections;
using Spirittiles.Scripts.HexGrid;

namespace Spirittiles.Scripts;

public partial class TileVisual : Area2D
{
    [Export] private Sprite2D _sprite;
    private int _id;

    // dragging
    private bool _dragging = false;
    private Vector2 _dragStartOffset = Vector2.Zero;
    private Tween _scaleTween;
    private Tween _tiltTween;
    private float _lastX;

    
    // signals
    [Signal] public delegate void TileVisualDroppedOutsideOfAnyGridEventHandler(int id, Vector2 globalPos);

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
        
        // debug label with id
        Label label = new Label
        {
            Text = id.ToString(),
            Position = new Vector2(-5, -10),
            ZIndex = 10
        };
        AddChild(label);
    }

    public override void _Process(double delta)
    {
        if (_dragging)
        {
            GlobalPosition = GetGlobalMousePosition() + _dragStartOffset;
            float deltaX = GlobalPosition.X - _lastX;
            _lastX = GlobalPosition.X;

            float targetRotation = Mathf.Clamp(deltaX 
                                               * 0.05f, -0.3f, 0.3f);

            _tiltTween?.Kill();
            _tiltTween = CreateTween();
            _tiltTween.TweenProperty(this, "rotation",
                targetRotation, 0.1f);

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
                _scaleTween?.Kill();
                _scaleTween = CreateTween();
                _scaleTween.TweenProperty(this, "scale",  
                        new Vector2(1.1f, 1.1f), 0.15f)
                    .SetEase(Tween.EaseType.Out);
                _dragStartOffset = GlobalPosition - GetGlobalMousePosition();
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (_dragging)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (!mouseEvent.Pressed)
                {
                    // debug message
                    int listenerCount = GetSignalConnectionList(SignalName.TileVisualDroppedOutsideOfAnyGrid).Count;
                    // GD.Print($"Listeners on drop signal: {listenerCount}");
                    
                    _dragging = false;
                    ZIndex = 0;
                    _scaleTween?.Kill();
                    _scaleTween = CreateTween();
                    _scaleTween.TweenProperty(this, "scale",  
                            Vector2.One, 0.15f)
                        .SetEase(Tween.EaseType.Out);
                    _tiltTween?.Kill();
                    _tiltTween = CreateTween();
                    _tiltTween.TweenProperty(this, "rotation",
                            0f, 0.2f)
                        .SetEase(Tween.EaseType.Out);

                    
                    foreach (Area2D area in GetOverlappingAreas())
                    {
                        if (area.GetParent() is ISnapAreaForTiles snapArea)
                        {
                            // GD.Print("TileVisual started sequence after drop");
                            snapArea.NotifyAboutDropFromTileVisual(_id, GlobalPosition);
                            return;
                        }
                    }
                    // if we're here, player let go while not above a snap area
                    // Notify whoever is responsible for this visual about the fact
                    EmitSignal(SignalName.TileVisualDroppedOutsideOfAnyGrid, _id, GlobalPosition);
                }
            }
        }
    }
}