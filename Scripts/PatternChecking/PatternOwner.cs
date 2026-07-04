using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts.PatternChecker;

public partial class PatternOwner : Node2D
{
    [Export] private Pattern _pattern;

    public override void _Ready()
    {
        _pattern.Initialize();
    }

    private void PrintAllPatternPermutations()
    {
        GD.Print($"All Pattern Permutations for PatternOwner {this}");
        Vector2I[][] rotations = _pattern.PatternRotations;
        Array<TileType> colors = _pattern.PatternColors;
        for (int i = 0; i < rotations.Length; i++)
        {
            string line = $"Rotation {i}: ";
            for (int j = 0; j < rotations[i].Length; j++)
            {
                line += $"{rotations[i][j]}, ";
            }
            GD.Print(line);
        }
    }

    public Pattern GetPattern()
    {
        return _pattern;
    }
    
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("debug_button_3"))
        {
            PrintAllPatternPermutations();
        }
    }
}