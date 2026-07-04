using Godot;
using Godot.Collections;
using Spirittiles.Scripts.HexGrid;
using Spirittiles.Scripts.PatternChecker;

namespace Spirittiles.Scripts;

public partial class Board : Node2D
{
    [Export] TileIdAllocator TileIdAllocator { get; set; }
    
    [Export] private HexGridManager ScoringGrid { get; set; }
    [Export] Array<HexGridManager> HexGridManagers { get; set; }
    [Export] Array<TileHolder.TileHolder> TileHolders { get; set; }
    [Export] TileMoveCoordinator TileMoveCoordinator { get; set; }
    [Export] PatternChecking.PatternChecker PatternChecker { get; set; }
    [Export] Array<Pattern> Patterns { get; set; }
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
        
        PatternChecker.Initialize(ScoringGrid);
        // add patterns to PatternChecker, manually for testing
        foreach (Pattern pattern in Patterns)
        {
            PatternChecker.AddPattern(pattern);
        }
    }
}