using Godot;
using System.Linq;

public partial class Game : Node2D
{
    private Sprite2D[] _backgrounds;
    private int _currentLevel = 0;
    private float _levelChangedTime = 0;
    private const float LEVEL_CHANGE_DELAY = 7.5f;
    private bool _delayedChangeTriggered = false;

    public override void _Ready()
    {
        // Fetch all Sprite2D children that are backgrounds
        _backgrounds = GetChildren()
            .OfType<Sprite2D>()
            .ToArray();

        // Initially hide all backgrounds
        foreach (var bg in _backgrounds)
        {
            bg.Visible = false;
        }

        // Show first background
        if (_backgrounds.Length > 0)
        {
            _backgrounds[0].Visible = true;
        }

        // Find the PlatformManager node
        PlatformManager platformManager = GetNode<PlatformManager>("GameManager");
        if (platformManager != null)
        {
            platformManager.LevelChanged += OnLevelChanged;
        }
        else
        {
            GD.PrintErr("PlatformManager node not found!");
        }
    }

    public override void _Process(double delta)
    {
        // Check for delayed background change
        if (_delayedChangeTriggered)
        {
            _levelChangedTime += (float)delta;

            if (_levelChangedTime >= LEVEL_CHANGE_DELAY)
            {
                // Perform the actual background change
                PerformBackgroundChange();
                _delayedChangeTriggered = false;
                _levelChangedTime = 0;
            }
        }
    }

    private void OnLevelChanged(int newLevel)
    {
        // Set up for delayed change
        _currentLevel = newLevel;
        _delayedChangeTriggered = true;
        _levelChangedTime = 0;
        
        GD.Print($"Level change initiated. Will change background in {LEVEL_CHANGE_DELAY} seconds.");
    }

    private void PerformBackgroundChange()
    {
        // Ensure the level is within the backgrounds array bounds
        if (_currentLevel >= 0 && _currentLevel < _backgrounds.Length)
        {
            // Hide all backgrounds
            foreach (var bg in _backgrounds)
            {
                bg.Visible = false;
            }

            // Show the new background
            _backgrounds[_currentLevel].Visible = true;
            
            GD.Print($"Background changed to level {_currentLevel}");
        }
        else
        {
            GD.PrintErr($"Invalid level {_currentLevel}. No corresponding background found.");
        }
    }
}