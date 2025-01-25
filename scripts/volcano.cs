using Godot;

public partial class volcano : Node2D
{
    [Export] private float eruptionInterval = 2.0f; // Time between eruptions
    [Export] private float eruptionDuration = 1.0f; // How long the eruption lasts
    [Export] private int damage = 1; // Damage dealt to the player

    private Area2D airEruption;
    private Timer eruptionTimer;
    private bool isErupting = false;

    public override void _Ready()
    {

		GD.Print("SCRIPT IS RUNNING......");
        // Get references to nodes
        airEruption = GetNode<Area2D>("AirEruption");
        eruptionTimer = GetNode<Timer>("EruptionTimer");

        // Debug: Check if nodes are found
        if (airEruption == null)
            GD.PrintErr("AirEruption Area2D not found!");
        if (eruptionTimer == null)
            GD.PrintErr("EruptionTimer not found!");

        // Set up the timer
        eruptionTimer.WaitTime = eruptionInterval;
        eruptionTimer.Timeout += OnEruptionTimerTimeout;
        eruptionTimer.Start();

        // Connect the Area2D signals
        airEruption.BodyEntered += OnBodyEnteredEruption;

        // Debug: Confirm signal connection
        GD.Print("Volcano initialized. Eruption interval: ", eruptionInterval);
    }

    private void OnEruptionTimerTimeout()
    {
        // Debug: Confirm timer is working
        GD.Print("EruptionTimer timeout. Starting eruption.");
        StartEruption();
    }

    private void StartEruption()
    {
        isErupting = true;
        GD.Print("Volcano is erupting!");

        // Enable the eruption area
        airEruption.Monitoring = true;

        // Debug: Confirm monitoring is enabled
        GD.Print("AirEruption monitoring: ", airEruption.Monitoring);

        // Schedule the end of the eruption
        GetTree().CreateTimer(eruptionDuration).Timeout += EndEruption;
    }

    private void EndEruption()
    {
        isErupting = false;
        GD.Print("Volcano eruption ended.");

        // Disable the eruption area
        airEruption.Monitoring = false;

        // Debug: Confirm monitoring is disabled
        GD.Print("AirEruption monitoring: ", airEruption.Monitoring);
    }

    private void OnBodyEnteredEruption(Node2D body)
    {
        // Debug: Confirm the signal is triggered
        GD.Print("Body entered eruption area: ", body.Name);

        if (isErupting && body is Player player)
        {
            // Debug: Confirm player is detected
            GD.Print("Player detected in eruption area.");

            // Damage the player
            player.TakeDamage(damage);
            GD.Print("Player took damage from the volcano eruption!");
        }
    }
}