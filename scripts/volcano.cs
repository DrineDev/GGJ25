using Godot;

public partial class volcano : Node2D
{
    [Export] private float eruptionInterval = 2.0f; // Time between eruptions
    [Export] private float eruptionDuration = 1.0f; // How long the eruption lasts
    [Export] private int damage = 1; // Damage dealt to the player

    private Area2D airEruption;
    private Timer eruptionTimer;
    private CpuParticles2D eruptionParticles;
    private AnimatedSprite2D animatedSprite;
    private bool isErupting = false;

    public override void _Ready()
    {
        // Get references to nodes
        airEruption = GetNode<Area2D>("AirEruption");
        eruptionTimer = GetNode<Timer>("EruptionTimer");
        eruptionParticles = GetNode<CpuParticles2D>("AnimatedSprite2D/CPUParticles2D");
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        // Debug: Check if nodes are found
        if (animatedSprite == null)
            GD.PrintErr("AnimatedSprite2D not found!");

        // Set up the timer
        eruptionTimer.WaitTime = eruptionInterval;
        eruptionTimer.Timeout += OnEruptionTimerTimeout;
        eruptionTimer.Start();

        // Connect the Area2D signals
        airEruption.BodyEntered += OnBodyEnteredEruption;
    }

    public override void _Process(double delta)
    {
        // Update the particles' position to match the volcano's position
        if (eruptionParticles != null)
        {
            eruptionParticles.GlobalPosition = animatedSprite.GlobalPosition;
        }
    }

    private void OnEruptionTimerTimeout()
    {
        StartEruption();
    }

    private void StartEruption()
    {
        isErupting = true;
        GD.Print("Volcano is erupting!");

        // Play the Explode animation with a delay
        GetTree().CreateTimer(0.0f).Timeout += () => 
        {
            animatedSprite.Play("Explode");
        };
        animatedSprite.Play("Default");

        // Start particles with a slight delay
        GetTree().CreateTimer(0.7f).Timeout += () => 
        {
            airEruption.Monitoring = true;

            if (eruptionParticles != null)
            {
                eruptionParticles.Restart();
                eruptionParticles.Emitting = true;
            }
        };

        // Schedule the end of the eruption
        GetTree().CreateTimer(eruptionDuration).Timeout += EndEruption;
    }

    private void EndEruption()
    {
        isErupting = false;
        GD.Print("Volcano eruption ended.");

        // Disable the eruption area
        airEruption.Monitoring = false;

        // Stop the particle effect
        if (eruptionParticles != null)
        {
            eruptionParticles.Emitting = false;
        }

        // Return to default animation
        animatedSprite.Play("default");
    }

    private void OnBodyEnteredEruption(Node2D body)
    {
        if (isErupting && body is Player player)
        {
            // Damage the player
            player.TakeDamage(damage);
            GD.Print("Player took damage from the volcano eruption!");
        }
    }
}