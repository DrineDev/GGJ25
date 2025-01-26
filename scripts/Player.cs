using Godot;

public partial class Player : CharacterBody2D {
    [Signal]
    public delegate void PlayerDiedEventHandler();
    public const float Speed = 500.0f;
    public const float DashSpeed = 1500.0f; // Speed during dash
    public const float DashDistance = 300.0f; // Distance to dash
    public const float DownSpeedMultiplier = 1.5f; // Multiplier for downward speed
    public int HP { get; private set; } = 10; // Player's health

    public bool IsInStealthMode { get; private set; } = false;
    public bool IsAlive = true;
    private bool IsDashing = false; // Track if the player is dashing

    private CpuParticles2D deathParticles;
    private AnimatedSprite2D sprite; 
    private AudioStreamPlayer2D hurtMusic;
    private AudioStreamPlayer2D failMusic;

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

        // Play the default animation initially
        sprite.Play("default");

        // Initialize HP display
        UpdateHPDisplay();
    }

    public override void _PhysicsProcess(double delta) {
        if (!IsAlive) return;

        Vector2 velocity = Velocity;

        if (!IsDashing) {
            // Move towards the mouse cursor
            Vector2 mousePosition = GetGlobalMousePosition();
            Vector2 direction = (mousePosition - GlobalPosition).Normalized();

            // Increase downward speed
            if (direction.Y > 0) {
                direction.Y *= DownSpeedMultiplier;
            }

            velocity = direction * Speed;

            // Play the default animation if not dashing
            if (sprite.Animation != "default") {
                sprite.Play("default");
            }
        }

        // Move the player
        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed) {
            // Trigger dash when left mouse button is pressed
            Dash();
        }
    }

    private void Dash() {
        if (IsDashing) return; // Prevent multiple dashes

        IsDashing = true;
        IsInStealthMode = true;

        // Calculate dash direction based on mouse position
        Vector2 mousePosition = GetGlobalMousePosition();
        Vector2 dashDirection = (mousePosition - GlobalPosition).Normalized();

        // Increase downward speed during dash
        if (dashDirection.Y > 0) {
            dashDirection.Y *= DownSpeedMultiplier;
        }

        // Set dash velocity
        Velocity = dashDirection * DashSpeed;

        // Play the dash animation
        sprite.Play("dash");

        // Use a timer to control the dash duration
        Timer dashTimer = new Timer();
        AddChild(dashTimer);
        dashTimer.WaitTime = DashDistance / DashSpeed; // Time = Distance / Speed
        dashTimer.OneShot = true;
        dashTimer.Timeout += () => {
            IsDashing = false;
            IsInStealthMode = false;
            Velocity = Vector2.Zero; // Stop movement after dash
            dashTimer.QueueFree(); // Clean up the timer

            // Reset to the default animation after dash
            sprite.Play("default");
        };
        dashTimer.Start();
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
            hurtMusic = GetNode<AudioStreamPlayer2D>("HurtMusic");
            hurtMusic.Play();
            UpdateHPDisplay();
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
            hurtMusic = GetNode<AudioStreamPlayer2D>("HurtMusic");
            hurtMusic.Play();
            failMusic = GetNode<AudioStreamPlayer2D>("DeathMusic");
            failMusic.Play();
        } else {
            GD.PrintErr("Death particles node is missing!");
        }

        // Emit the PlayerDied signal
        GD.Print("Emitting PlayerDied signal...");
        EmitSignal(SignalName.PlayerDied);

        // Notify the scoring system to stop scoring
        var scoringSystem = GetNode<Node>("/root/ScoringSystem");
        if (scoringSystem != null) {
            ((GodotObject)scoringSystem).Call("stop_scoring");
        }

        // Restart the game after a delay
        GetTree().CreateTimer(4.0f).Timeout += RestartGame;
    }

    public void RestartGame() {
        // Reset the score before reloading the scene
        var scoringSystem = GetNode<Node>("/root/ScoringSystem");
        if (scoringSystem != null) {
            ((GodotObject)scoringSystem).Call("reset_score");
        }
        GetTree().ReloadCurrentScene();
    }

    public void UpdateHPDisplay() {
        try {
            // Try different potential paths
            var hpContainer = GetNode<HBoxContainer>($"../Camera2D/CanvasLayer/heartDisplay");
            // Or try
            // var hpContainer = GetNode<HBoxContainer>("Camera2D/CanvasLayer/heartDisplay");
            // Or try absolute path
            // var hpContainer = GetNode<HBoxContainer>("/root/YourSceneName/Camera2D/CanvasLayer/heartDisplay");

            GD.Print($"Found HP Container. Child count: {hpContainer.GetChildCount()}");

            for (int i = 0; i < hpContainer.GetChildCount(); i++) {
                var heartIcon = hpContainer.GetChild<TextureRect>(i);

                GD.Print($"Heart {i}: Visible = {i < HP}");

                // Show/hide hearts based on current HP
                if (i < HP) {
                    heartIcon.Visible = true;
                } else {
                    heartIcon.Visible = false;
                }
            }
        }
        catch (System.Exception e) {
            GD.PrintErr($"Error updating HP display: {e.Message}");
        }
    }
}
