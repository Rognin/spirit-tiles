using System;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using Spirittiles.Scripts.HexGrid;

namespace Spirittiles.Scripts;

public partial class HexGridManager : Node, IHexGrid
{
	[Export] private int _numberOfRows = 20;
	[Export] private int _numberOfColumns = 20;

	// hex grid components
	[Export] private HexGridData _hexGridData;
	[Export] private HexGridDisplay _hexGridDisplay;
	
	[Signal] public delegate void TileCreatedEventHandler(int id);
	[Signal] public delegate void TileDestroyedEventHandler(int id);
	[Signal] public delegate void ForwardTileDropEventHandler(int id, Vector2 globalPos);
	
	public override void _Ready()
	{
		
	}

	public void Initialize()
	{
		_hexGridDisplay.Initialize(_numberOfRows, _numberOfColumns);
		_hexGridData.Initialize(_numberOfRows, _numberOfColumns);
		
		_hexGridData.CellCreated += OnTileCreated;
		_hexGridData.CellDestroyed += OnCellDestroyed;
		_hexGridDisplay.TileDropped += OnTileDropped;
		_hexGridDisplay.ForwardTileDrop += OnForwardTileDrop;
		
		// configure snapAreaCollider
		_hexGridDisplay.UpdateSnapAreaColliderShape();
		
		this.AddToGroup("HexGridManagers");
		
		// TEMP: manually adding tiles for testing
		// _hexGridData.CreateBasicTile(0, 0, _waterSharedTile, TileIdAllocator.GetNextId());
		// _hexGridData.CreateBasicTile(2, 2, _mountainSharedTile, TileIdAllocator.GetNextId());
		// _hexGridData.PrintCurrentCells();
	}

	public bool FillNextEmptyHexWithTile(TileLogic tile) // returns false if grid is full
	{
		return _hexGridData.FillNextEmptyHexWithTile(tile);
	}

	public int GetEmptyCellCount()
	{
		return _hexGridData.GetEmptyCellCount();
	}

	private void OnTileCreated(int row, int column, TileLogic tile)
	{
		_hexGridDisplay.CreateTileVisual(new Vector2I(row, column), tile);
		EmitSignal(SignalName.TileCreated, tile.Id);
	}

	private void OnCellDestroyed(int id)
	{
		_hexGridDisplay.DestroyTileVisual(id);
		EmitSignal(SignalName.TileDestroyed, id);
	}

	private void OnTileDropped(int id, bool valid, Vector2I newCoords)
	{
		// step 1: validate move

		if (!_hexGridData.ValidateSameGridMove(id, valid, newCoords, out Vector2I correctCoords))
		{
			_hexGridDisplay.CancelMove(id, correctCoords);
			return;
		}
		
		// step 2: if move valid, update HexGridData
		
		_hexGridData.MoveTileSameGrid(id, newCoords.X, newCoords.Y);
		
		// step 3: if move valid, snap the tileVisual

		_hexGridDisplay.PlaceTileVisualAtCoords(id, correctCoords);

	}

	private void OnForwardTileDrop(int id, Vector2 globalPos)
	{
		// GD.Print("Step 2: manager forwards to coordinator");
		EmitSignal(SignalName.ForwardTileDrop, id, globalPos);
	}

	public void MoveTileWithinThisGrid(int id, Vector2 newPos) // tile moved from this grid to this grid
	{
		// call the chain of methods for one-grid move
		_hexGridDisplay.MoveTileWithinThisGrid(id, newPos);
	}

	public bool CheckIfTileMoveToWorldPosValid(Vector2 worldPos, out Vector2I newTileCoords)
	{
		bool valid = false;
		newTileCoords = Vector2I.Zero;
		// check if display has the position inside it (should always be true but just in case)
		if (_hexGridDisplay.TryGetSnapPosition(worldPos, out Vector2I coords))
		{
			// check if data says it's all good
			if (_hexGridData.ValidateDifferentGridMove(coords))
			{
				newTileCoords = coords;
				valid = true;
			}
		}
		return valid;
	}

	public void CancelMove(int id)
	{
		Vector2I coords = _hexGridData.GetTileCoordinatesById(id);
		_hexGridDisplay.CancelMove(id, coords);
	}

	public void MoveTileToDifferentGrid(int id, out TileLogic tile, out TileVisual visual)
	{
		tile = _hexGridData.SendOffTileToDifferentGrid(id);
		visual = _hexGridDisplay.SendOffVisual(id);
	}

	public void AddTileAfterMoveFromDifferentGrid(int id, Vector2I coords, TileLogic tile, TileVisual visual)
	{
		_hexGridData.ReceiveTileAfterMoveFromDifferentGrid(coords, tile);
		
		_hexGridDisplay.ReceiveNewTileVisual(id, visual, coords);
	}

	public Dictionary<Vector2I, GridCellData> GetGridCells()
	{
		return _hexGridData.GetGridCells();
	}

	public override void _Process(double delta)
	{
	}
}