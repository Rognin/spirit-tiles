using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Spirittiles.Scripts.PatternChecker;
using Spirittiles.Scripts.UI;

namespace Spirittiles.Scripts.PatternChecking;

public partial class PatternChecker : Node
{
	[Export] private Array<Pattern> _patterns;
	private HexGridManager _grid;
	private Godot.Collections.Dictionary<Vector2I, GridCellData> _cellsAxial = new ();
	[Export] private ScoreDisplay _scoreDisplay;
	
	
	public void Initialize(HexGridManager grid)
	{
		_patterns = new Array<Pattern>();
		_grid = grid;
	}

	public void AddPattern(Pattern pattern)
	{
		_patterns.Add(pattern);
	}

	// use removal carefully - needs a reference to the exact instance of a Pattern
	public void RemovePattern(Pattern pattern)
	{
		_patterns.Remove(pattern);
	}

	public void ClearPatterns()
	{
		_patterns.Clear();
	}

	public void UpdateScore()
	{
		int scoreToAdd = CalculateTotalScore();
		_scoreDisplay.AddScore(scoreToAdd);
	}
	
	private int CalculateTotalScore()
	{
		SynchronizeGridData();
		int totalScore = 0;
		string debugScoreString = "";
		foreach (Pattern pattern in _patterns)
		{
			int score = CalculatePatternScore(pattern);
			debugScoreString += $"{score} + ";
			totalScore += score;
		}

		if (debugScoreString != "")
		{
			GD.Print(debugScoreString.Substring(0, debugScoreString.Length - 1) + $"= {totalScore}");
		}

		return totalScore;
	}

	private int CalculatePatternScore(Pattern pattern)
	{
		Vector2I[][] patternRotations = pattern.PatternRotations;
		Array<TileType> colors = pattern.PatternColors;
		int patternScore = 0;
		int baseScore = pattern.Score;
		HashSet<HashSet<Vector2I>> occurrences = new HashSet<HashSet<Vector2I>>(HashSet<Vector2I>.CreateSetComparer());
		
		// add every occurrence of this rotation to the set
		foreach (Vector2I[] rotation in patternRotations) // for each rotation, check
		{
			foreach (Vector2I coords in _cellsAxial.Keys) // every cell in the grid and see
			{
				bool patternValid = true;
				HashSet<Vector2I> currentOccurrence = new HashSet<Vector2I>();
				// if this cell is a "starting point" of this particular rotation.
				// to do this, go sequentially through the current pattern rotation and see if it matches.
				// first, check if the cell itself could be the starting point
				if (_cellsAxial[coords].IsEmpty())
				{
					continue;
				}

				if (_cellsAxial[coords].GetTileType() != colors[0])
				{
					continue;
				}
				// if it can be the starting point, add it to the current occurrence and
				// for each other cell check if it fits the pattern
				currentOccurrence.Add(coords);
				for (int i = 1; i < rotation.Length; i++)
				{
					Vector2I curCoords = coords + rotation[i];
					// first, see if the coordinates are in the grid
					if (!_cellsAxial.ContainsKey(curCoords))
					{
						patternValid = false;
						break;
					}
					// if they are, we see if the corresponding cell has the right tile type
					if (_cellsAxial[curCoords].IsEmpty())
					{
						patternValid = false;
						break;
					}
					if (_cellsAxial[curCoords].GetTileType() != colors[i])
					{
						patternValid = false;
						break;
					}
						// if the tile type matches the pattern, add it to the current occurrence
						currentOccurrence.Add(curCoords);
				}

				// if it turnes out the pattern is fully valid starting from this cell, add it to the set
				if (patternValid)
				{
					occurrences.Add(currentOccurrence);
				}
			}
		}
		
		GD.Print($"Pattern {pattern.Name} occurs {occurrences.Count} times");
		patternScore += occurrences.Count * baseScore;
		return patternScore;
	}

	private void SynchronizeGridData()
	{
		Godot.Collections.Dictionary<Vector2I, GridCellData> gridDataRowColCoords = _grid.GetGridCells();
		_cellsAxial.Clear();
		foreach (Vector2I coords in gridDataRowColCoords.Keys)
		{
			Vector2I axial = RowColToAxial(coords);
			_cellsAxial.Add(axial, gridDataRowColCoords[coords]);
		}
	}

	private Vector2I RowColToAxial(Vector2I rowCol)
	{
		int parity = rowCol.X & 1;
		int xAxial = rowCol.Y - (rowCol.X - parity) / 2;
		int diagAxial = rowCol.X;
		return new Vector2I(xAxial, diagAxial);
	}

	public override void _Ready()
	{
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debug_button_2"))
		{
			// PrintCurrentCells();
			// PrintCurrentPatterns();
			CalculateTotalScore();
		}

		if (Input.IsActionJustPressed("debug_button_4"))
		{
			UpdateScore();
		}
	}

	private void PrintCurrentCells()
	{
		SynchronizeGridData();
		foreach (var kvp in _cellsAxial)
		{
			if (!kvp.Value.IsEmpty())
			{
				GD.Print($"PatternChecker {this}: tile with id {kvp.Value.Tile.Id} at {kvp.Key}");
			}
		}
	}

	private void PrintCurrentPatterns()
	{
		foreach (Pattern pattern in _patterns)
		{
			GD.Print(pattern.Name);
		}
	}
	
}