using Godot;

public partial class Killzone : Area2D
{
    public override void _Ready()
    {
        // Connect the body_entered signal to handle collisions
        this.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        // Check if the body that entered the killzone is the player
        if (body is Player player)
        {
            GD.Print("Player entered the killzone!");
            // player.TakeDamage(1); // Reduce the player's HP by 1
        }
        // Check if the body that entered the killzone is an enemy
        else if (body is IEnemy enemy)
        {
            GD.Print("Enemy entered the killzone!");
            // Cast the enemy to a Node and call QueueFree
            if (enemy is Node enemyNode)
            {
                enemyNode.QueueFree(); // Despawn the enemy
            }
            else
            {
                GD.PrintErr("Enemy does not inherit from Node!");
            }
        }
    }
}