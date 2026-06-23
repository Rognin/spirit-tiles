using System;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;

namespace Spirittiles.Scripts;

public partial class HexGridManager : Node, IHexGrid
{
	[Export] private int _numberOfRows = 20;
	[Export] private int _numberOfColumns = 20;

	// hex grid components
	[Export] private HexGridData _hexGridData;
	[Export] private HexGridDisplay _hexGridDisplay;
	
	[Export] private SharedTileData _waterSharedTile;
	[Export] private SharedTileData _mountainSharedTile;
	
	//
	public TileIdAllocator TileIdAllocator { get; set; } // set by board
	
	public override void _Ready()
	{
		
	}

	public void Initialize()
	{
		_hexGridData.Initialize(_numberOfRows, _numberOfColumns);
		_hexGridDisplay.Initialize(_hexGridData);
		
		_hexGridData.CellCreated += OnCellCreated;
		_hexGridDisplay.TileDropped += OnTileDropped;
		
		// configure snapAreaCollider
		_hexGridDisplay.UpdateSnapAreaColliderShape();
		
		// TEMP: manually adding tiles for testing
		_hexGridData.CreateBasicTile(0, 0, _waterSharedTile, TileIdAllocator.GetNextId());
		_hexGridData.CreateBasicTile(2, 2, _mountainSharedTile, TileIdAllocator.GetNextId());
		_hexGridData.PrintCurrentCells();
	}

	private void OnCellCreated(int row, int column, TileLogic tile)
	{
		_hexGridDisplay.CreateTileVisual(new Vector2I(row, column), tile);
	}

	private void OnTileDropped(int id, bool valid, Vector2I newCoords)
	{
		// step 1: validate move

		if (!_hexGridData.ValidateMove(id, valid, newCoords, out Vector2I correctCoords))
		{
			_hexGridDisplay.CancelMove(id, correctCoords);
			return;
		}
		
		// step 2: if move valid, update HexGridData
		
		_hexGridData.MoveTile(id, newCoords.X, newCoords.Y);
		
		// step 3: if move valid, snap the tileVisual

		_hexGridDisplay.PlaceTileVisualAtCoords(id, correctCoords);

	}

	public override void _Process(double delta)
	{
	}
}