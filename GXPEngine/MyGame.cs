using System;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine;
using GXPEngine.Core;

public class MyGame : Game
{
	public static SceneManager scenes;
	public MyGame() : base(1000, 1000, false, true, -1, -1, true)
	{
		targetFps = 60;
		scenes = new SceneManager();
    }

	static void Main()
	{
		new MyGame().Start();
	}

	public static void PlaySFX(string sound, float volumeMod)
    {
		Sound s = new Sound(sound);
        s.Play(false, 0, GameSettings.SFX_VOLUME * volumeMod);
    }
}