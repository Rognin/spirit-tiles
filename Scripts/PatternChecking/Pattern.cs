using System.Linq;
using Godot;
using Godot.Collections;

namespace Spirittiles.Scripts.PatternChecker;

// class responsible for storing a pattern of tiles
// pattern is stored in axial coordinates
// the first tile in the pattern is the "main" one that the pattern rotates around
// gets all rotations of the pattern on initialization
// allows to get an array that contains all valid permutations of the pattern
[GlobalClass]
public partial class Pattern : Resource
{
	[Export] public string Name { get; set; }
	[Export] public Texture2D Texture { get; set; }
	[Export] public int Score { get; set; }
	[Export] private Array<Vector2I> _basePattern;
	[Export] public Array<TileType> PatternColors { get; set; }
	public Vector2I[][] PatternRotations { get; set; }
	
	public void Initialize()
	{
		FillRotations();
	}

	private void FillRotations()
	{
		PatternRotations = new Vector2I[6][];
		for (int i = 0; i < 6; i++)
		{
			PatternRotations[i] = new Vector2I[_basePattern.Count];
		}
		
		
		PatternRotations[0] = _basePattern.ToArray();
		for (int i = 1; i < 6; i++)
		{
			Vector2I[] previousRotation = PatternRotations[i - 1];
			PatternRotations[i] = RotatePatternBy60Clockwise(previousRotation);
		}
	}

	private Vector2I[] RotatePatternBy60Clockwise(Vector2I[] pattern)
	{
		Vector3I[] toRotate = new Vector3I[pattern.Length];
		// convert pattern to cube coords
		for (int i = 0; i < toRotate.Length; i++)
		{
			int x = pattern[i].X;
			int y = pattern[i].Y;
			toRotate[i] = new Vector3I(x, y, - x - y);
		}
		for (int i = 1; i < pattern.Length; i++)
		{
			Vector3I hex = toRotate[i];
			Vector3I center = toRotate[0];
			toRotate[i] = RotateHex60Clockwise(hex, center);
		}
		
		return CubeToAxial(toRotate);
	}

	private Vector3I RotateHex60Clockwise(Vector3I hex, Vector3I center)
	{
		Vector3I vector = new Vector3I(hex.X - center.X, hex.Y - center.Y, hex.Z - center.Z);
		Vector3I rotated = new Vector3I(-1 * vector.Y, -1 * vector.Z, -1 * vector.X);
		Vector3I answer = new Vector3I(rotated.X + center.X, rotated.Y + center.Y, rotated.Z + center.Z);
		return answer;
	}

	private Vector2I[] CubeToAxial(Vector3I[] cube)
	{
		Vector2I[] axial = new Vector2I[cube.Length];
		for (int i = 0; i < cube.Length; i++)
		{
			axial[i] = new Vector2I(cube[i].X, cube[i].Y);
		}

		return axial;
	}
	
}