using Godot;
using System;

public partial class EnemyManager : Node2D {
    [Export] private Node2D _scene;
    [Export] private float _levelSpeed = 100f;
    [Export] private PackedScene[] _enemies;
    [Export] private float _spawnInterval = 250f;
    [Export] private float _minX = -300f;
    [Export] private float _maxX = 300f;

    private float _currentSpawnProgress = 0;

    public override void _Ready() {
        _currentSpawnProgress = 0;
        int initialEnemies = Mathf.CeilToInt(GetViewportRect().Size.Y / _spawnInterval) + 2;
        for (int i = 0; i < initialEnemies; i++) {
            _InstantiateEnemy(-(i + 1));
        }
    }

    public override void _Process(double delta) {
        float progress = _levelSpeed * (float)delta;

        foreach (Node child in _scene.GetChildren()) {
            if (child is Node2D node2D) {
                node2D.GlobalPosition += new Vector2(0, progress);
            }
        }

        _currentSpawnProgress += progress;

        if (_currentSpawnProgress >= _spawnInterval) {
            Node2D lastEnemy = _scene.GetChild(_scene.GetChildCount() - 1) as Node2D;
            if (lastEnemy != null) {
                float newY = lastEnemy.GlobalPosition.Y - _spawnInterval;
                _InstantiateEnemyAtY(newY);
            }

            Node2D firstEnemy = _scene.GetChild(0) as Node2D;
            if (firstEnemy != null && firstEnemy.GlobalPosition.Y >= GetViewportRect().Size.Y) {
                firstEnemy.QueueFree();
            }

            _currentSpawnProgress -= _spawnInterval;
        }
    }

    private void _InstantiateEnemy(int offset = 0, int type = -1) {
        int index = (type == -1) ? (int)(GD.Randi() % _enemies.Length) : type;
        PackedScene selectedScene = _enemies[index];

        if (selectedScene == null) {
            GD.PrintErr("Invalid PackedScene in _enemies array!");
            return;
        }

        BaseEnemy enemy = selectedScene.Instantiate<BaseEnemy>();
        _scene.AddChild(enemy);
        
        // Random x coordinate, fixed y at negative spawn interval
        float randomX = GD.Randf() * (_maxX - _minX) + _minX;
        enemy.GlobalPosition = new Vector2(randomX, offset * _spawnInterval);
    }

    private void _InstantiateEnemyAtY(float y, int type = -1) {
        int index = (type == -1) ? (int)(GD.Randi() % _enemies.Length) : type;
        PackedScene selectedScene = _enemies[index];

        if (selectedScene == null) {
            GD.PrintErr("Invalid PackedScene in _enemies array!");
            return;
        }

        BaseEnemy enemy = selectedScene.Instantiate<BaseEnemy>();
        _scene.AddChild(enemy);
        
        // Random x coordinate at specified y
        float randomX = GD.Randf() * (_maxX - _minX) + _minX;
        enemy.GlobalPosition = new Vector2(randomX, y);
    }
}