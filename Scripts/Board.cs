using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts;

public partial class Board : Node2D
{
    [Export] TileIdAllocator TileIdAllocator { get; set; }
    [Export] Array<HexGridManager> HexGridManagers { get; set; }
    [Export] Array<TileHolder.TileHolder> TileHolders { get; set; }

    public override void _Ready()
    {
        foreach (HexGridManager manager in HexGridManagers)
        {
            manager.TileIdAllocator = TileIdAllocator;
            manager.Initialize();
        }
        foreach (TileHolder.TileHolder tileHolder in TileHolders)
        {
            tileHolder.TileIdAllocator = TileIdAllocator;
            tileHolder.Initialize();
        }
    }
}