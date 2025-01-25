using Godot;

public partial class Player : CharacterBody2D {
    public const float Speed = 500.0f;
    public int HP { get; private set; } = 10; // Player's health

    public bool IsInStealthMode { get; private set; } = false;
    public override void _PhysicsProcess(double delta) {
        Vector2 velocity = Velocity;

        // Get input direction
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (direction != Vector2.Zero) {
            velocity = direction * Speed;
        } else {
            velocity = Vector2.Zero;
        }

        // Move the player
        Velocity = velocity;
        MoveAndSlide();
    }

    public void SetStealthMode(bool stealth) {
        IsInStealthMode = stealth;

        if (stealth) {
            GD.Print("Player is now invisible and untargetable!");
            Modulate = new Color(1, 1, 1, 0.5f);
        } else {
            GD.Print("Player is now visible and targetable!");
            Modulate = new Color(1, 1, 1, 1);
        }
    }

    public void TakeDamage(int damage) {
        // Check if the player is in stealth mode
        if (IsInStealthMode) {
            GD.Print("Player takes no damage.");
        } else { // Deal damage
            HP -= damage;
        }

        // Print the player's current HP (for debugging)
        GD.Print($"Player took {damage} damage! HP: {HP}");

        // Check if the player is dead
        if (HP <= 0) {
            GD.Print("Player died!");
            RestartGame();
            // Handle player death (e.g., restart the level or show a game over screen)
        }
    }

    private void RestartGame() {
        GetTree().ReloadCurrentScene();
    }
}