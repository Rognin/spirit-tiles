using Godot;

namespace Spirittiles.Scripts;

[GlobalClass]
public partial class GridCellData(TileLogic newTile) : Resource
{

	public TileLogic Tile { get; set; }

	public bool IsEmpty() => Tile == null;
}