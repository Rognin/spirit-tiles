using Godot;

namespace Spirittiles.Scripts;

[GlobalClass]
public partial class GridCellData(MyTileData newData) : Node
{

	[Export] public MyTileData CurrentTileData { get; set; } = newData;

	public bool IsEmpty() => CurrentTileData == null; //TODO: change since there's empty tiles or remove the empty tiles
}