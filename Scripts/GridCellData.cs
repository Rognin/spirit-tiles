using Godot;

namespace Spirittiles.Scripts;

[GlobalClass]
public partial class GridCellData() : Resource
{
	[Export] public MyTileData CurrentTileData { get; set; }

	public bool IsEmpty() => CurrentTileData == null;
}