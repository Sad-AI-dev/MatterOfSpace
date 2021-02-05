using System;
using System.Collections.Generic;
using System.Drawing.Text;
using GXPEngine;
using GXPEngine.Core;

public class MyGame : Game
{
	public static SceneManager scenes;
	public static Rectangle bounds;
	public static PrivateFontCollection fonts;
	public MyGame() : base(1920, 1800, true, true, -1, -1, true)
	{
		ShowMouse(true);
		targetFps = 60;
		//include custom font
		BuildFont("../Files/Fonts/joystix.ttf");
		//define gamebounds
		bounds = new Rectangle(game.width * 0.2f, 0, game.width * 0.6f, game.height);
		//build scene
		scenes = new SceneManager();
    }

	static void Main()
	{
		new MyGame().Start();
	}

	private void BuildFont(string font)
    {
		fonts = new PrivateFontCollection();
		fonts.AddFontFile(font);
    }

	public static void PlaySFX(string sound, float volumeMod)
    {
		Sound s = new Sound(sound);
        s.Play(false, 0, GameSettings.SFX_VOLUME * volumeMod);
    }
}