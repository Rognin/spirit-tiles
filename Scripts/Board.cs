using Godot;
using Godot.Collections;
using Spirittiles.Scripts.HexGrid;

namespace Spirittiles.Scripts;

public partial class Board : Node2D
{
    [Export] TileIdAllocator TileIdAllocator { get; set; }
    [Export] Array<HexGridManager> HexGridManagers { get; set; }
    [Export] Array<TileHolder.TileHolder> TileHolders { get; set; }
    
    [Export] TileMoveCoordinator TileMoveCoordinator { get; set; }

    public override void _Ready()
    {
        foreach (TileHolder.TileHolder tileHolder in TileHolders)
        {
            tileHolder.TileIdAllocator = TileIdAllocator;
            tileHolder.Initialize();
        }
        
        foreach (HexGridManager manager in HexGridManagers)
        {
            manager.Initialize();
        }
        
        TileMoveCoordinator.Initialize();
    }
}