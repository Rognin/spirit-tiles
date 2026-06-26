using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts.TileHolder;

public partial class TileHolder : Node2D
{
	public TileIdAllocator TileIdAllocator { get; set; } // set by board
	[Export] private HexGridManager _hexGridManager;
	[Export] private Array<SharedTileData> _tilesToSpawn;
	private TileSpawner _tileSpawner;

	public void Initialize()
	{
		_tileSpawner = new TileSpawner();
		_tileSpawner.Initialize(TileIdAllocator, _tilesToSpawn);
		_hexGridManager.Initialize();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debug_button_1"))
		{
			// int newTilesAmount = _hexGridManager.GetEmptyCellCount();
			int newTilesAmount = 2;
			Array<TileLogic> newTiles = _tileSpawner.GetNewTiles(newTilesAmount);
			for (int i = 0; i < newTilesAmount; i++)
			{
				TileLogic tileToPlace = newTiles[i];
				_hexGridManager.FillNextEmptyHexWithTile(tileToPlace);
			}
		}
	}
}