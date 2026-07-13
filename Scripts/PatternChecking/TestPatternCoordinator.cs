using Godot;
using Spirittiles.Scripts.PatternChecker;

namespace Spirittiles.Scripts.PatternChecking;

public partial class TestPatternCoordinator : Node
{
	[Export] PatternDatabase _patternDatabase;
	[Export] VBoxContainer _patternsDisplay;
	[Export] PatternChecker _patternChecker;
	
	[Signal] public delegate void PatternAddedEventHandler(Pattern pattern);
	
	public override void _Ready()
	{
		_patternDatabase.Initialize();
		PatternAdded += _patternChecker.AddPattern;
	}

	private void AddRandomPattern()
	{
		Pattern next = _patternDatabase.GetRandomPattern();
		// add image to display
		TextureRect image = new TextureRect();
		image.Texture = next.Texture;
		image.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;
		image.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		image.CustomMinimumSize = new Vector2(100, 100);
		_patternsDisplay.AddChild(image);
		GD.Print("Added random pattern");
		EmitSignal(SignalName.PatternAdded, next);
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debug_button_5"))
		{
			AddRandomPattern();
		}
	}
}