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
	
	[Export] private MyTileData _waterMyTile;
	[Export] private MyTileData _mountainMyTile;
	
	//
	public TileIdAllocator TileIdAllocator { get; set; } // set by board
	
	public override void _Ready()
	{
		_hexGridData.Initialize(_numberOfRows, _numberOfColumns);
		_hexGridDisplay.Initialize(_hexGridData);
		
		_hexGridData.CellCreated += OnCellCreated;
		_hexGridDisplay.TileDropped += OnTileDropped;
		_hexGridDisplay.TileMoved += OnTileMovedAway;
		
		// configure snapAreaCollider
		_hexGridDisplay.UpdateSnapAreaColliderShape();
		
		// TEMP: manually adding tiles for testing
		_hexGridData.CreateTile(0, 0, _waterMyTile);
		_hexGridData.CreateTile(2, 2, _mountainMyTile);
		_hexGridData.PrintCurrentCells();
	}

	private void OnCellCreated(int row, int column, MyTileData newTileData)
	{
		_hexGridDisplay.PlaceAndDrawNewTile(new Vector2I(row, column), newTileData);
	}

	private void OnTileDropped(TileVisual tileVisual, Vector2I snapCoords)
	{
		// step 1: clear where the cell used to be

		if (tileVisual.CurrentSnapArea != null)
		{
			tileVisual.CurrentSnapArea.OnTileMovedAway(tileVisual);
		}
		
		// step 2: add cell to the new position
		
		_hexGridData.AddTileAfterMove(snapCoords.X, snapCoords.Y, tileVisual.Data);
		
		// step 3: update internal cell coords and curSnapArea
		
		tileVisual.SnapBackCoordinates = snapCoords;
		tileVisual.CurrentSnapArea = _hexGridDisplay;

	}

	private void OnTileMovedAway(TileVisual tileVisual)
	{
		// for now only clear previous position of the tile
		ClearTileAfterMove(tileVisual);
	}

	private void ClearTileAfterMove(TileVisual tileVisual)
	{
		// clear tile from HexGridData
		_hexGridData.RemoveTile(tileVisual.SnapBackCoordinates.X, tileVisual.SnapBackCoordinates.Y);
	}

	public override void _Process(double delta)
	{
	}
}