using Godot;

public partial class Player : CharacterBody2D {
    [Signal]
    public delegate void PlayerDiedEventHandler();
    public const float Speed = 500.0f;
    public int HP { get; private set; } = 10; // Player's health

    public bool IsInStealthMode { get; private set; } = false;
    public bool IsAlive = true;

    private CpuParticles2D deathParticles;
    private AnimatedSprite2D sprite; // Reference to the AnimatedSprite2D node

    public override void _Ready() {
        // Get the CPUParticles2D node
        deathParticles = GetNode<CpuParticles2D>("DeathParticles");
        if (deathParticles != null) {
            GD.Print("Death particles node found.");
        } else {
            GD.PrintErr("Death particles node is missing!");
        }

        // Get the AnimatedSprite2D node
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        if (sprite == null) {
            GD.PrintErr("AnimatedSprite2D node not found!");
        }
    }

    public override void _PhysicsProcess(double delta) {
        Vector2 velocity = Velocity;

        // Get input direction
        if (IsAlive) {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (direction != Vector2.Zero) {
                velocity = direction * Speed;
                UpdateSpriteOrientation(direction); // Update sprite based on direction
            } else {
                velocity = Vector2.Zero;
                sprite.Play("idle"); // Play idle animation when not moving
            }

            // Move the player
            Velocity = velocity;
            MoveAndSlide();
        }
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
            GD.Print($"Player took {damage} damage! HP: {HP}");
        }

        // Check if the player is dead
        if (HP <= 0) {
            GD.Print("Player died!");
            Die();
        }
    }

    public void Die() {
        GD.Print("Player is dying...");
        IsAlive = false;

        // Disable the player's collision and visibility
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        Modulate = new Color(1, 1, 1, 0); // Make the player invisible

        // Check if the deathParticles node is valid
        if (deathParticles != null) {
            GD.Print("Playing death particles...");
            deathParticles.GlobalPosition = this.GlobalPosition;
            deathParticles.Restart(); // Restart the particle system
            deathParticles.Emitting = true; // Ensure particles are emitting
        } else {
            GD.PrintErr("Death particles node is missing!");
        }

        // Emit the PlayerDied signal
        GD.Print("Emitting PlayerDied signal...");
        EmitSignal(SignalName.PlayerDied);

        // Restart the game after a delay
        GetTree().CreateTimer(3.0f).Timeout += RestartGame;
    }

    public void RestartGame() {
        GetTree().ReloadCurrentScene();
    }

    private void UpdateSpriteOrientation(Vector2 direction) {
        if (sprite == null) return;

        // Normalize the direction vector
        direction = direction.Normalized();

        // Determine the animation based on the direction
        if (direction.Y < -0.7f) {
            // Up
            sprite.Play("walk_up");
        } else if (direction.Y > 0.7f) {
            // Down
            sprite.Play("walk_down");
        } else if (direction.X < -0.7f) {
            // Left
            sprite.Play("walk_left");
        } else if (direction.X > 0.7f) {
            // Right
            sprite.Play("walk_right");
        } else if (direction.X < -0.5f && direction.Y < -0.5f) {
            // Up-Left
            sprite.Play("walk_up_left");
        } else if (direction.X > 0.5f && direction.Y < -0.5f) {
            // Up-Right
            sprite.Play("walk_up_right");
        } else if (direction.X < -0.5f && direction.Y > 0.5f) {
            // Down-Left
            sprite.Play("walk_down_left");
        } else if (direction.X > 0.5f && direction.Y > 0.5f) {
            // Down-Right
            sprite.Play("walk_down_right");
        }
    }
}