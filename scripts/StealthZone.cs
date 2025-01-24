using Godot;

public partial class StealthZone : Area2D
{
    private Player _player; // Reference to the player

    public override void _Ready()
    {
        // Connect the body_entered and body_exited signals
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        // Check if the body that entered the zone is the player
        if (body is Player player)
        {
            GD.Print("Player entered stealth zone!");
            _player = player;
            _player.SetStealthMode(true); // Make the player invisible and untargetable
        }
    }

    private void OnBodyExited(Node2D body)
    {
        // Check if the body that exited the zone is the player
        if (body is Player player)
        {
            GD.Print("Player exited stealth zone!");
            _player.SetStealthMode(false); // Make the player visible and targetable again
            _player = null;
        }
    }
}
