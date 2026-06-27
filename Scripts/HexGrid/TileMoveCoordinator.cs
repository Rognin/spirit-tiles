using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts.HexGrid;

public partial class TileMoveCoordinator : Node2D
{
	Array<HexGridManager> _hexGridManagers = new();
	private Dictionary<int, HexGridManager> _gridByTileId = new();
	public void Initialize()
	{
		foreach (Node gridManager in GetTree().GetNodesInGroup("HexGridManagers"))
		{
			_hexGridManagers.Add(gridManager as HexGridManager);
		}

		foreach (HexGridManager gridManager in _hexGridManagers)
		{
			GD.Print("Registering new grid");
			gridManager.TileCreated += (id) => RegisterTile(id, gridManager);
			gridManager.TileDestroyed += (id) => UnregisterTile(id, gridManager);
			gridManager.ForwardTileDrop += (id, globalPos) => HandleTileDrop(id, globalPos, gridManager);
		}
	}

	private void RegisterTile(int id, HexGridManager gridManager)
	{
		if (_gridByTileId.ContainsKey(id))
		{
			GD.PrintErr("Duplicate tile id " + id);
		}
		else
		{
			GD.Print($"Registering tile with id {id}");
			_gridByTileId.Add(id, gridManager);
		}
	}

	private void UnregisterTile(int id, HexGridManager gridManager)
	{
		_gridByTileId.Remove(id);
	}

	private void UpdateTileOwner(int id, HexGridManager newManager)
	{
		if (!_gridByTileId.ContainsKey(id))
		{
			RegisterTile(id, newManager);
			return;
		}
		_gridByTileId[id] = newManager;
	}

	private void HandleTileDrop(int id, Vector2 globalPos, HexGridManager gridManager)
	{
		if (!_gridByTileId.ContainsKey(id))
		{
			GD.PrintErr("Tile with id {id} not registered");
			return;
		}

		if (_gridByTileId[id] == gridManager)
		{
			gridManager.MoveTileWithinThisGrid(id, globalPos);
		}
		else
		{
			// GD.Print("Step 3: coordinator starts movement between grids");
			HexGridManager from = _gridByTileId[id];
			HexGridManager to = gridManager;

			if (to.CheckIfTileMoveToWorldPosValid(globalPos, out Vector2I newTileCoords))
			{
				// move valid. Remove tile from first grid, add it to second
				// GD.Print("Step 4: move valid, removing tile from old grid");
				from.MoveTileToDifferentGrid(id, out TileLogic tile, out TileVisual visual);
				
				// GD.Print("Step 5: move tile to different grid");
				to.AddTileAfterMoveFromDifferentGrid(id, newTileCoords, tile, visual);
				
				// update the id-hexGridManager link for the coordinator
				UpdateTileOwner(id, to);
			}
			else
			{
				// move invalid. Snap cell back to where it came from
				from.CancelMove(id);
			}
		}
	}

	public override void _Process(double delta)
	{
	}
}