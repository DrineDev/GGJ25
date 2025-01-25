using Godot;

public partial class BaseEnemy : CharacterBody2D, IEnemy
{
    [Export] public float Speed { get; set; } = 100.0f;
    [Export] public float WanderSpeed { get; set; } = 50.0f;
    [Export] public float WaitTime { get; set; } = 3.0f;

    public bool HasEnteredBorder { get; set; } = false;

    protected Player _player;
    protected Vector2 _wanderDirection;
    protected float _waitTimer = 0;
    protected AnimatedSprite2D _sprite;

    public override void _Ready()
    {
        _player = GetNode<Player>("/root/Game/Player");
        if (_player == null)
        {
            GD.PrintErr("Player node not found!");
            return;
        }

        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        SetWanderDirection();

        var killzone = GetNode<Area2D>("Killzone");
        if (killzone != null)
        {
            killzone.BodyEntered += OnBodyEntered;
        }
        else
        {
            GD.PrintErr("Killzone not found!");
        }
    }

    public override void _Process(double delta)
    {
        if (_player == null) return;

        if (_player.IsInStealthMode)
        {
            if (_waitTimer > 0)
            {
                _waitTimer -= (float)delta;
                GD.Print("Enemy is waiting...");
            }
            else
            {
                Wander((float)delta);
            }
        }
        else
        {
            ChasePlayer((float)delta);
        }

        UpdateSpriteOrientation();
    }

    protected virtual void ChasePlayer(float delta)
    {
        Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
    }

    protected virtual void Wander(float delta)
    {
        Velocity = _wanderDirection * WanderSpeed;
        MoveAndSlide();

        if (GD.Randf() < 0.01f)
        {
            SetWanderDirection();
        }
    }

    protected void SetWanderDirection()
    {
        _wanderDirection = new Vector2(GD.Randf() - 0.5f, GD.Randf() - 0.5f).Normalized();
    }

    public void OnPlayerHidden()
    {
        _waitTimer = WaitTime;
        GD.Print("Enemy lost track of the player!");
    }

    protected virtual void OnBodyEntered(Node body)
    {
        GD.Print("Body entered the enemy's hitbox!");

        if (body is Player player)
        {
            GD.Print("Enemy hit the player!");
            player.TakeDamage(1);
            QueueFree();
        }
    }

    public virtual void OnBorderEntered()
    {
        GD.Print("Enemy entered the border!");

        if (HasEnteredBorder)
        {
            GD.Print("Enemy has entered the border before. Despawning...");
            QueueFree();
        }
        else
        {
            HasEnteredBorder = true;
            GD.Print("Enemy entered the border for the first time.");
        }
    }

    protected virtual void UpdateSpriteOrientation()
    {
        if (_sprite == null) return;

        if (Velocity.X > 0)
        {
            _sprite.FlipH = false;
        }
        else if (Velocity.X < 0)
        {
            _sprite.FlipH = true;
        }

        _sprite.Play(Velocity != Vector2.Zero ? "walk" : "idle");
    }
}
