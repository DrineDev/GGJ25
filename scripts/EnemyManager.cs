using Godot;
using System;

public partial class EnemyManager : Node2D 
{
    [Export] private Node2D _scene;
    [Export] public float _levelSpeed { get; set; } = 200f;
    [Export] private float _spawnInterval = 250f;
    [Export] private float _minX = -300f;
    [Export] private float _maxX = 300f;

    // Enemy groups for each level
    [Export] private PackedScene[] _enemiesLevel1;
    [Export] private PackedScene[] _enemiesLevel2;
    [Export] private PackedScene[] _enemiesLevel3;

    private float _currentSpawnProgress = 0;
    private int _currentLevel = 0;
    private float _timeElapsed = 0;
    private int _score = 0;

    public override void _Ready() 
    {
        _currentSpawnProgress = 0;

        // Initialize enemies to fill the screen
        int initialEnemies = Mathf.CeilToInt(GetViewportRect().Size.Y / _spawnInterval) + 2;
        for (int i = 0; i < initialEnemies; i++) 
        {
            _InstantiateEnemy(-(i + 1));
        }

        // Find the player node and connect to its PlayerDied signal
        Player player = GetNode<Player>("../Player");
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

        // Track progress for instantiating and removing enemies
        _currentSpawnProgress += progress;

        if (_currentSpawnProgress >= _spawnInterval) 
        {
            // Spawn a new enemy above the top of the screen
            Node2D lastEnemy = _scene.GetChild(_scene.GetChildCount() - 1) as Node2D;
            if (lastEnemy != null) 
            {
                // Calculate the y-position for the new enemy
                float newY = lastEnemy.GlobalPosition.Y - _spawnInterval;
                _InstantiateEnemyAtY(newY);
            }

            // Remove the first enemy only if it is completely out of view
            Node2D firstEnemy = _scene.GetChild(0) as Node2D;
            if (firstEnemy != null && firstEnemy.GlobalPosition.Y >= GetViewportRect().Size.Y) 
            {
                firstEnemy.QueueFree();
            }

            // Reset progress
            _currentSpawnProgress -= _spawnInterval;
        }

        // Update score based on time elapsed
        _timeElapsed += (float)delta;
        _score = (int)(_timeElapsed / 2);

        // Check for level transitions based on score
        CheckLevelTransition();
    }

    private void _InstantiateEnemy(int offset = 0, int type = -1) 
    {
        PackedScene[] currentEnemyGroup = GetCurrentEnemyGroup();
        int index = (type == -1) ? (int)(GD.Randi() % currentEnemyGroup.Length) : type;
        PackedScene selectedScene = currentEnemyGroup[index];

        if (selectedScene == null) 
        {
            GD.PrintErr("Invalid PackedScene in enemy group array!");
            return;
        }

        BaseEnemy enemy = selectedScene.Instantiate<BaseEnemy>();
        _scene.AddChild(enemy);
        
        // Random x coordinate, fixed y at negative spawn interval
        float randomX = GD.Randf() * (_maxX - _minX) + _minX;
        enemy.GlobalPosition = new Vector2(randomX, offset * _spawnInterval);
    }

    private void _InstantiateEnemyAtY(float y, int type = -1) 
    {
        PackedScene[] currentEnemyGroup = GetCurrentEnemyGroup();
        int index = (type == -1) ? (int)(GD.Randi() % currentEnemyGroup.Length) : type;
        PackedScene selectedScene = currentEnemyGroup[index];

        if (selectedScene == null) 
        {
            GD.PrintErr("Invalid PackedScene in enemy group array!");
            return;
        }

        BaseEnemy enemy = selectedScene.Instantiate<BaseEnemy>();
        _scene.AddChild(enemy);
        
        // Random x coordinate at specified y
        float randomX = GD.Randf() * (_maxX - _minX) + _minX;
        enemy.GlobalPosition = new Vector2(randomX, y);
    }

    private void OnPlayerDied()
    {
        GD.Print("Player died! Stopping enemy movement...");
        _levelSpeed = 0;
    }

    private void CheckLevelTransition()
    {
        int[] levelThresholds = { 10, 20, 30 }; // thresholds

        if (_currentLevel < levelThresholds.Length && _score >= levelThresholds[_currentLevel])
        {
            _currentLevel++;
            GD.Print($"ENEMY LEVEL TRANSITION TRIGGERED! New level: {_currentLevel}");
        }
    }

    private PackedScene[] GetCurrentEnemyGroup()
    {
        switch (_currentLevel)
        {
            case 0: return _enemiesLevel1;
            case 1: return _enemiesLevel2;
            case 2: return _enemiesLevel3;
            default: return _enemiesLevel1; // Default to level 1 if out of bounds
        }
    }
}