using Godot;
using System;

public partial class Game : Node2D
{
	private AudioStreamPlayer audioPlayer;

	public override void _Ready()
	{
    	audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    	audioPlayer.Stream = GD.Load<AudioStream>("res://path/to/sound.mp3");
	}

	// To trigger specific audio
	private void PlayEruptionSound()
	{
    	audioPlayer.Play();
	}
}
