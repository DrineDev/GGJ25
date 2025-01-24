using Godot;
using System;
using System.Collections.Generic;

public partial class EndlessPlatformGenerator : Node2D
{
    [Export] private TileMap _tileMap;
    [Export] private CharacterBody2D _player;
    [Export] private PackedScene _enemyScene;

    private const int PlatformWidth = 20;
    private const int VisibleSections = 10;
    private const int SectionHeight = 16; // Tile units
    private const float ScrollSpeed = 100f;

    private Vector2 _lastPlatformPosition = Vector2.Zero;
    private Random _random = new Random();

    public override void _Ready()
    {
        InitializePlatforms();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Move platforms down as player ascends
        _tileMap.Position -= new Vector2(0, ScrollSpeed * (float)delta);

        // Check if we need to generate new platforms
        if (_player.GlobalPosition.Y < _lastPlatformPosition.Y - (VisibleSections * SectionHeight * 16))
        {
            GeneratePlatformSection();
            RemoveOldPlatforms();
            SpawnEnemies();
        }
    }

    private void InitializePlatforms()
    {
        for (int i = 0; i < VisibleSections; i++)
        {
            GeneratePlatformSection();
        }
    }

    private void GeneratePlatformSection()
    {
        // Create a random platform section
        int platformX = _random.Next(2, PlatformWidth - 2);
        
        for (int x = 0; x < PlatformWidth; x++)
        {
            if (x >= platformX && x < platformX + 3)
            {
                // Generate platform tiles
                _tileMap.SetCell(0, new Vector2I(x, (int)_lastPlatformPosition.Y / 16), 0);
            }
        }

        _lastPlatformPosition = new Vector2(0, _lastPlatformPosition.Y - (SectionHeight * 16));
    }

    private void RemoveOldPlatforms()
    {
        // Remove platforms that are far below the player
        // Implement logic to clear tiles that are no longer visible
    }

    private void SpawnEnemies()
    {
        // Randomly spawn enemies on platforms
        if (_random.NextDouble() < 0.5)
        {
            Enemy enemy = _enemyScene.Instantiate<Enemy>();
            enemy.GlobalPosition = _lastPlatformPosition + new Vector2(_random.Next(PlatformWidth) * 16, 0);
            AddChild(enemy);
        }
    }
}

public partial class Enemy : CharacterBody2D
{
    private const float Speed = 100f;

    public override void _PhysicsProcess(double delta)
    {
        // Simple enemy AI to chase player
        // You'll need to implement actual chasing logic
        Velocity = new Vector2(0, -Speed);
        MoveAndSlide();
    }
}