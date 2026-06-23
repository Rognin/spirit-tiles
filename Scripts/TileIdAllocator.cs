using Godot;

namespace Spirittiles.Scripts;

public partial class TileIdAllocator : Node
{
    private int _nextId = 0;
    public int GetNextId()
    {
        return _nextId++;
    }
}