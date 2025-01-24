using Godot;
using System;

public partial class EnemyManager : Node2D
{
    [Export] private Node2D _scene; // Parent node for instantiated enemies
    [Export] private float _levelSpeed = 100f; // Speed at which enemies move
    [Export] private PackedScene[] _enemies; // Array of enemy scenes to instantiate
    [Export] private float _spawnInterval = 250f; // Spacing between enemy spawns

    private float _currentSpawnProgress = 0; // Tracks spawn progress

    public override void _Ready()
    {
        _currentSpawnProgress = 0;

        // Initialize enemies to fill the screen and make sure it scrolls infinitely
        int initialEnemies = Mathf.CeilToInt(GetViewportRect().Size.Y / _spawnInterval) + 2; // Dynamically calculate based on screen height
        for (int i = 0; i < initialEnemies; i++)
        {
            _InstantiateEnemy(-(i + 1)); // Instantiate enemies at negative y values
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
    }

    private void _InstantiateEnemy(int offset = 0, int type = -1)
    {
        int index = (type == -1) ? (int)(GD.Randi() % _enemies.Length) : type;
        PackedScene selectedScene = _enemies[index];

        if (selectedScene == null)
        {
            GD.PrintErr("Invalid PackedScene in _enemies array!");
            return;
        }

        Node2D enemy = selectedScene.Instantiate<Node2D>();
        _scene.AddChild(enemy);

        // Position the enemy at the correct y-position
        enemy.GlobalPosition = new Vector2(0, offset * _spawnInterval);

        // Debug print
        GD.Print($"Instantiated enemy at offset {offset}, y = {offset * _spawnInterval}");
    }

    private void _InstantiateEnemyAtY(float y, int type = -1)
    {
        int index = (type == -1) ? (int)(GD.Randi() % _enemies.Length) : type;
        PackedScene selectedScene = _enemies[index];

        if (selectedScene == null)
        {
            GD.PrintErr("Invalid PackedScene in _enemies array!");
            return;
        }

        Node2D enemy = selectedScene.Instantiate<Node2D>();
        _scene.AddChild(enemy);

        // Position the enemy at the specified y-position
        enemy.GlobalPosition = new Vector2(0, y);

        // Debug print
        GD.Print($"Instantiated enemy at y = {y}");
    }
}
