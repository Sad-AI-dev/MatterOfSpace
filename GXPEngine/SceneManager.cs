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
                rootObj.LateAddChild(new HUDMainMenu());
                break;

            case 1: //load gameplay screen
		        //load player
		        player = new Player();
		        rootObj.LateAddChild(player);
                //wave spawner
                rootObj.LateAddChild(new WaveSpawner());
		        //add UI overlay
		        rootObj.LateAddChild(new HUDGamePlay());
                break;

            case 2:
                //load game over screen
                rootObj.LateAddChild(new HUDGameOver());
                break;
        }
    }

    public void GainScore(int toGain, Vector2 pos)
    {
        score += toGain;
        scoreUpdate?.Invoke(toGain, pos);
    }
}
