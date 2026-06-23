using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts;

public partial class Board : Node2D
{
    [Export] TileIdAllocator TileIdAllocator { get; set; }
    [Export] Array<HexGridManager> HexGridManagers { get; set; }

    public override void _Ready()
    {
        foreach (HexGridManager manager in HexGridManagers)
        {
            manager.TileIdAllocator = TileIdAllocator;
        }
    }
}