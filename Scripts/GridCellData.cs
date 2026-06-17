using Godot;

namespace Spirittiles.Scripts;

[GlobalClass]
public partial class GridCellData() : Resource
{
	public TileTypes CurrentTileType { get; set; } = TileTypes.Empty; // TODO: probably remove this in favor of TileData
	
	public MyTileData CurrentMyTileData { get; set; }

	public bool IsEmpty() => CurrentTileType == TileTypes.Empty;
}