using System;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;

namespace Spirittiles.Scripts;

public partial class HexGridManager : Node
{
	[Export] private int _numberOfRows = 20;
	[Export] private int _numberOfColumns = 20;

	[Export] private HexGridData _hexGridData;
	[Export] private HexGridDisplay _hexGridDisplay;
	
	[Export] private MyTileData _waterMyTile;
	[Export] private MyTileData _mountainMyTile;
	
	public override void _Ready()
	{
		_hexGridData.Initialize(_numberOfRows, _numberOfColumns);
		_hexGridDisplay.Initialize(_hexGridData);
		
		_hexGridData.CellChanged += OnCellChanged;
		
		// TEMP: manually adding tiles for testing
		_hexGridData.AddTile(0, 0, _waterMyTile);
		_hexGridData.AddTile(2, 2, _mountainMyTile);
		_hexGridData.PrintCurrentCells();
	}

	private void OnCellChanged(int row, int column, MyTileData oldTileData, MyTileData newTileData)
	{
		_hexGridDisplay.PlaceAndDrawTile(new Vector2I(row, column), newTileData);
	}


	public override void _Process(double delta)
	{
	}
}