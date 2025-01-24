using Godot;

public partial class Enemy : Node2D
{
    public const float Speed = 300.0f;

    private Player _player; // Reference to the player
    private Area2D _hitbox; // Hitbox for detecting collisions with the player
    public bool HasEnteredBorder { get; set; } = false; // Flag to track border entry

    public override void _Ready()
    {
        // Find the player node in the scene
        _player = GetNode<Player>("/root/Game/Player"); // Adjust the path to your player node

        // Get the hitbox area (ensure the enemy has an Area2D child for collision detection)
        _hitbox = GetNode<Area2D>("Hitbox");

        // Connect the Area2D's body_entered signal to handle collisions
        _hitbox.BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        if (_player != null)
        {
            // Calculate the direction to the player
            Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

            // Move the enemy toward the player
            Position += direction * Speed * (float)delta;
        }
    }

    private void OnBodyEntered(Node body)
    {
        // Check if the body that entered the hitbox is the player
        if (body is Player player)
        {
            // Reduce the player's HP by 1
            player.TakeDamage(1);

            // Despawn the enemy
            GD.Print("Enemy hit the player. Despawning...");
            QueueFree(); // Destroy the enemy
        }
    }
}