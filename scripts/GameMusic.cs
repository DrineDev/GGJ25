using Godot;
using System;

public partial class GameMusic : AudioStreamPlayer2D
{
	private AudioStreamPlayer audioPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioPlayer.Stream = GD.Load<AudioStream>("res://Audio/main game real.wav");
		audioPlayer.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
