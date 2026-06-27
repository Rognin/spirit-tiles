using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts.TileHolder;

public partial class TileSpawner () : Node
{
	private Array<TileLogic> _tileArray = new Array<TileLogic>();
	TileIdAllocator _tileIdAllocator;

	private int _currentSpawnIndex = 0;
	
	// tiles
	private Array<SharedTileData> _tilesToSpawn;
	
	// call initialize before _Ready (so before addChild)

	public void Initialize(TileIdAllocator tileIdAllocator, Array<SharedTileData> tilesToSpawn)
	{
		_tileIdAllocator = tileIdAllocator;
		_tilesToSpawn = tilesToSpawn;
		
		// temp: manually fill up the tileArray
		foreach(SharedTileData tileType in _tilesToSpawn)
		{
			for (int i = 0; i < 5; i++)
			{
				int id = _tileIdAllocator.GetNextId();
				_tileArray.Add(new TileLogic(tileType, id));
			}
		}
		
		// shuffle the order of tiles in the array
		_tileArray.Shuffle();
	}

	public Array<TileLogic> GetNewTiles(int amount)
	{
		Array<TileLogic> newTiles = new Array<TileLogic>();
		for (int i = 0; i < amount; i++)
		{
			if (_currentSpawnIndex >= _tileArray.Count)
			{
				GD.Print("NoMoreTilesInTheBag");
				break;
			}
			newTiles.Add(_tileArray[_currentSpawnIndex]);
			_currentSpawnIndex++;
		}
		return newTiles;
	}
}