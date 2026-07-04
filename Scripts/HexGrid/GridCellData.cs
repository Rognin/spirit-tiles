using Godot;

namespace Spirittiles.Scripts;

[GlobalClass]
public partial class GridCellData(TileLogic newTile) : Resource
{

	public TileLogic Tile { get; set; }

	public bool IsEmpty() => Tile == null;

	public TileType GetTileType()
	{
		if (IsEmpty())
		{
			GD.PrintErr("Tried to access an empty cell's tile. Returning fallback");
			return 0;
		}
		return Tile.SharedTileData.Type;
	}
}