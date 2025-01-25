using Godot;

public partial class Player : CharacterBody2D
{
    public const float Speed = 500.0f;
    public int HP { get; private set; } = 10; // Player's health

    public bool IsInStealthMode { get; private set; } = false;

    private Area2D _area2D;

    // References to the GpuParticles2D orbs
    private GpuParticles2D _orb1;
    private GpuParticles2D _orb2;
    private GpuParticles2D _orb3;

    public override void _Ready()
    {
        // Get references to the GpuParticles2D orbs
        _orb1 = GetNode<GpuParticles2D>("Area2D/Orb1");
        _orb2 = GetNode<GpuParticles2D>("Area2D/Orb2");
        _orb3 = GetNode<GpuParticles2D>("Area2D/Orb3");

        // Ensure the orbs are not emitting at the start
        _orb1.Emitting = false;
        _orb2.Emitting = false;
        _orb3.Emitting = false;

        // Get reference to Area2D
        _area2D = GetNode<Area2D>("Area2D");
        // Connect the signal
        _area2D.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        // Check if the body that entered is the player
        if (body == this)
        {
            GD.Print("Player collided with Area2D!");
            ActivateOrbs(); // Activate the orbs
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Get input direction
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (direction != Vector2.Zero)
        {
            velocity = direction * Speed;
        }
        else
        {
            velocity = Vector2.Zero;
        }

        // Move the player
        Velocity = velocity;
        MoveAndSlide();
    }

    public void SetStealthMode(bool stealth)
    {
        IsInStealthMode = stealth;

        if (stealth)
        {
            GD.Print("Player is now invisible and untargetable!");
            Modulate = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            GD.Print("Player is now visible and targetable!");
            Modulate = new Color(1, 1, 1, 1);
        }
    }

    public void TakeDamage(int damage)
    {
        GD.Print("TakeDamage called!");

        // Check if the player is in stealth mode
        if (IsInStealthMode)
        {
            GD.Print("Player takes no damage.");
        }
        else
        { // Deal damage
            HP -= damage;
            GD.Print($"Player took {damage} damage! HP: {HP}");
        }

        // Check if the player is dead
        if (HP <= 0)
        {
            GD.Print("Player died!");
            RestartGame();
        }
    }

    private void RestartGame()
    {
        GetTree().ReloadCurrentScene();
    }

    // Method to activate the orbs
    public void ActivateOrbs()
    {
        GD.Print("Activating orbs!");
        _orb1.Emitting = true;
        _orb2.Emitting = true;
        _orb3.Emitting = true;
    }
}