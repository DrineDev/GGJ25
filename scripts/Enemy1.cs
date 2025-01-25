using Godot;

public partial class Enemy1 : BaseEnemy
{
    [Export] public new float Speed = 250.0f;
    [Export] public new float WanderSpeed = 500.0f;
    [Export] public new float WaitTime = 2.0f;
}