using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
public class SceneManager : GameObject
{
    private class Empty : GameObject { };
    public static GameObject rootObj;
    public Player player;
    public HUD currentHUD;

    //score handling
    public int score = 0;
    public Action<int, Vector2> scoreUpdate;

    public SceneManager()
    {
        rootObj = new Empty();
        Game.main.AddChild(rootObj);
        LoadScene(0);
    }

    public void LoadScene(int id)
    {
        //unload all objects
        if (rootObj != null)
        {
            foreach (GameObject obj in rootObj.GetChildren())
            {
                obj.LateDestroy();
            }
        }
        //background
		rootObj.LateAddChild(new BG());
        switch(id)
        {
            case 0:
                //load main menu
                currentHUD = new HUDMainMenu();
                rootObj.LateAddChild(currentHUD);
                break;

            case 1: //load gameplay screen
		        //load player
		        player = new Player();
		        rootObj.LateAddChild(player);
                //wave spawner
                rootObj.LateAddChild(new WaveSpawner());
                //add UI overlay
                currentHUD = new HUDGamePlay();
		        rootObj.LateAddChild(currentHUD);
                break;

            case 2:
                //load game over screen
                currentHUD = new HUDGameOver();
                rootObj.LateAddChild(currentHUD);
                break;
        }
    }

    public void GainScore(int toGain, Vector2 pos)
    {
        score += toGain;
        scoreUpdate?.Invoke(toGain, pos);
    }
}
