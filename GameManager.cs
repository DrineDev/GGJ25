using Godot;
using System;
public partial class GameManager : Node2D
{
    [Export] private Node2D _scene;
    [Export] private float _levelSpeed = 100f;

    public override void _Process(double delta)
    {
        foreach (Node n in _scene.GetChildren())
        {
            if (n is Node2D node2D)
            {
                node2D.Translate(Vector2.Up * _levelSpeed * (float)delta);
            }
        }
    }
}