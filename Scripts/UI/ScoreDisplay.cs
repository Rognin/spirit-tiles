using Godot;

namespace Spirittiles.Scripts.UI;

public partial class ScoreDisplay : Label
{
    private int _score = 0;

    public void AddScore(int score)
    {
        _score += score;
        this.Text = _score.ToString();
    }
}