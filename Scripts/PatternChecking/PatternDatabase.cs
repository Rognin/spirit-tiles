using Godot;
using Godot.Collections;
using Spirittiles.Scripts.PatternChecker;

namespace Spirittiles.Scripts.PatternChecking;

public partial class PatternDatabase : Node
{
	[Export] Array<Pattern> Patterns { get; set; }
	private Array<Pattern> _patternBag;
	private int _bagIndex;

	public void Initialize()
	{
		foreach (Pattern pattern in Patterns)
		{
			pattern.Initialize();
		}
		_bagIndex = 0;
		_patternBag = new Array<Pattern>(Patterns);
		_patternBag.Shuffle();
	}

	public Pattern GetRandomPattern()
	{
		if (_bagIndex >= _patternBag.Count)
		{
			_patternBag = new Array<Pattern>(Patterns);
			_patternBag.Shuffle();
			_bagIndex = 0;
		}
		return _patternBag[_bagIndex++];
	}
}