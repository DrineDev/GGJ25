using Godot;

public partial class PlatformManager : Node2D
{
    [Export] private Node2D _scene; // Parent node for instantiated groups
    [Export] public float _levelSpeed { get; set; } = 200f; // Speed at which groups move
    [Export] private float _groupLength = 250f; // Spacing between groups

    // Platform groups for each level
    [Export] private PackedScene[] _platformGroupsLevel1;
    [Export] private PackedScene[] _platformGroupsLevel2;
    [Export] private PackedScene[] _platformGroupsLevel3;

    private float _currentGroupProgress = 0; // Tracks movement progress
    private int _currentLevel = 0; // Current level index
    private int _score = 0; // Current score
    private float _timeElapsed = 0; // Time elapsed since the game started

    // Signal to notify when the level changes
    [Signal]
    public delegate void LevelChangedEventHandler(int newLevel);

    public override void _Ready()
    {
        _currentGroupProgress = 0;

        // Initialize groups to fill the screen and make sure it scrolls infinitely
        int initialGroups = Mathf.CeilToInt(GetViewportRect().Size.Y / _groupLength) + 2; // Dynamically calculate based on screen height
        for (int i = 0; i < initialGroups; i++)
        {
            _InstantiateGroup(-(i + 1)); // Instantiate groups at negative y values
        }

        // Find the player node and connect to its PlayerDied signal
        Player player = GetNode<Player>("../Player"); // Adjust the path to your player node
        if (player != null)
        {
            player.PlayerDied += OnPlayerDied;
        }
        else
        {
            GD.PrintErr("Player node not found!");
        }
    }

    public override void _Process(double delta)
    {
        float progress = _levelSpeed * (float)delta;

        // Move all children of _scene downward
        foreach (Node child in _scene.GetChildren())
        {
            if (child is Node2D node2D)
            {
                node2D.GlobalPosition += new Vector2(0, progress);
            }
        }

        // Track progress for instantiating and removing groups
        _currentGroupProgress += progress;

        if (_currentGroupProgress >= _groupLength)
        {
            // Spawn a new group above the top of the screen
            Node2D lastGroup = _scene.GetChild(_scene.GetChildCount() - 1) as Node2D;
            if (lastGroup != null)
            {
                // Calculate the y-position for the new group
                float newY = lastGroup.GlobalPosition.Y - _groupLength;
                _InstantiateGroupAtY(newY);
            }

            // Remove the first group only if it is completely out of view
            Node2D firstGroup = _scene.GetChild(0) as Node2D;
            if (firstGroup != null && firstGroup.GlobalPosition.Y >= GetViewportRect().Size.Y)
            {
                firstGroup.QueueFree();
            }

            // Reset progress
            _currentGroupProgress -= _groupLength;
        }

        // Update score based on time elapsed (slower progression)
        _timeElapsed += (float)delta;
        _score = (int)(_timeElapsed / 2); // Adjust the divisor to make points harder to earn

        // Check for level transitions based on score
        CheckLevelTransition();
    }

    private void _InstantiateGroup(int offset = 0, int type = -1)
    {
        PackedScene[] currentPlatformGroup = GetCurrentPlatformGroup();
        int index = (type == -1) ? (int)(GD.Randi() % currentPlatformGroup.Length) : type;
        PackedScene selectedScene = currentPlatformGroup[index];

        if (selectedScene == null)
        {
            GD.PrintErr("Invalid PackedScene in platform group array!");
            return;
        }

        Node2D group = selectedScene.Instantiate<Node2D>();
        _scene.AddChild(group);

        // Position the group at the correct y-position
        group.GlobalPosition = new Vector2(0, offset * _groupLength);

        // Debug print
        GD.Print($"Instantiated group at offset {offset}, y = {offset * _groupLength}");
    }

    private void _InstantiateGroupAtY(float y, int type = -1)
    {
        PackedScene[] currentPlatformGroup = GetCurrentPlatformGroup();
        int index = (type == -1) ? (int)(GD.Randi() % currentPlatformGroup.Length) : type;
        PackedScene selectedScene = currentPlatformGroup[index];

        if (selectedScene == null)
        {
            GD.PrintErr("Invalid PackedScene in platform group array!");
            return;
        }

        Node2D group = selectedScene.Instantiate<Node2D>();
        _scene.AddChild(group);

        // Position the group at the specified y-position
        group.GlobalPosition = new Vector2(0, y);

        // Debug print
        GD.Print($"Instantiated group at y = {y}");
    }

    private void OnPlayerDied()
    {
        GD.Print("Player died! Stopping platform movement...");
        _levelSpeed = 0;
    }

    private void CheckLevelTransition()
    {
        int[] levelThresholds = { 10, 20, 30 }; // thresholds

        if (_currentLevel < levelThresholds.Length && _score >= levelThresholds[_currentLevel])
        {
            _currentLevel++;
            GD.Print($"LEVEL TRANSITION TRIGGERED! Emitting signal for level {_currentLevel}");

            // Emit the LevelChanged signal with extensive logging
            GD.Print("Attempting to emit LevelChanged signal...");
            try 
            {
                EmitSignal(SignalName.LevelChanged, _currentLevel);
                GD.Print($"Successfully emitted LevelChanged signal for level {_currentLevel}");
            }
            catch (System.Exception e)
            {
                GD.PrintErr($"Error emitting LevelChanged signal: {e.Message}");
            }
        }
    }

    private PackedScene[] GetCurrentPlatformGroup()
    {
        switch (_currentLevel)
        {
            case 0: return _platformGroupsLevel1;
            case 1: return _platformGroupsLevel2;
            case 2: return _platformGroupsLevel3;
            default: return _platformGroupsLevel1; // Default to level 1 if out of bounds
        }
    }
}