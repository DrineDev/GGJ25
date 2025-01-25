using Godot;

public partial class Enemy2 : BaseEnemy
{
    [Export] public new float Speed = 300.0f;
    [Export] public new float WanderSpeed = 500.0f;
    [Export] public new float WaitTime = 2.0f;

    protected override void OnBodyEntered(Node body)
    {
        GD.Print("Body entered the enemy's hitbox!");

        if (body is Player player)
        {
            GD.Print("Enemy hit the player!");
            player.TakeDamage(3);
            QueueFree();
        }
    }
}