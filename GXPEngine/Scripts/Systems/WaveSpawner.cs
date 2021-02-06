using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using Setting = GameSettings.WaveSpawnerSettings;
class WaveSpawner : GameObject
{
    private List<Enemy> actives = new List<Enemy>();

    private int budget, nextBudget, budgetIncrement;
    private int round = 0;
    private enum Cost : int
    {
        red = Setting.Costs.red,
        green = Setting.Costs.green,
        blue = Setting.Costs.blue,
        green_cluster = Setting.Costs.green_cluster,
        red_cluster = Setting.Costs.red_cluster,
        blue_cluster = Setting.Costs.blue_cluster
    }

    public WaveSpawner()
    {
        SetSettings();
        ResetCheck();
    }

    private void SetSettings()
    {
        nextBudget = Setting.START_BUDGET;
        budgetIncrement = Setting.BUDGET_SCALING;
    }

    private void SpawnWave()
    {
        round++;
        HandleBudget();
        //spawn enemies with new budget
        while (budget >= (int)Cost.red)
        {
            SpawnEnemy(GetRandomEntry(GetMostExpensives()));
        }
    }

    private void HandleBudget()
    {
        budget += nextBudget;
        nextBudget += budgetIncrement;
        IncreaseScalingCheck();
    }

    private void IncreaseScalingCheck()
    {
        if (round % Setting.SCALING_INCREASE_DELAY == 0)
            budgetIncrement = Mathf.Floor(budgetIncrement * Setting.SCALING_INCREASE);
    }

    private void EnemyDeath(Enemy e)
    {
        actives.Remove(e);
        ResetCheck();
    }

    private void ResetCheck()
    {
        if (actives.Count <= 0) //no enemies left
        {
            if (round != 0)
            {
                new Timer(SpawnWave, Setting.WAVE_DELAY);
                new Timer(GiveWaveReward, 5);
                //add banner
                new Timer(DrawBanner, 100);
            }
            else
                new Timer(SpawnWave, 200);
        }
    }
    private void DrawBanner()
    {
        MyGame.scenes.currentHUD.LateAddChild(new BlinkText("Wave Incoming", 40, new Vector2(game.width / 2, game.height * 0.2f), MyGame.scenes.currentHUD));
        MyGame.PlaySFX("Sounds/waveIncoming.wav", 1.2f);
    }

    private void GiveWaveReward()
    { //give score bonus for clearing wave
        MyGame.scenes.GainScore(100 * round, new Vector2(game.width / 2, game.height * 0.4f));
    }

    private Vector2 GetRandomStartPos()
    {
        Vector2 pos;
        pos.x = Utils.Random(MyGame.bounds.x, MyGame.bounds.x + MyGame.bounds.width);
        pos.y = Utils.Random(-game.height / 2, -50);
        return pos;
    }

    private List<string> GetMostExpensives()
    {
        List<int> expensives = new List<int>();
        foreach (int cost in Enum.GetValues(typeof(Cost)))
        {
            if (cost <= budget)
            {
                expensives.Add(cost);
            }
        }
        //sort List
        expensives.Sort();
        expensives.Reverse();
        //choice pool
        List<string> pool = new List<string>();
        for (int i = 0; i < Setting.CHOICE_POOL; i++)
        {
            if (i < expensives.Count)
                pool.Add(Enum.GetName(typeof(Cost), expensives[i]));
        }
        return pool;
    }
    private T GetRandomEntry<T>(List<T> list)
    {
        return list[Utils.Random(0, list.Count)];
    }

    private void SpawnEnemy(string type)
    {
        switch(type)
        {
            case "red":
                SpawnRed();
                break;
            case "green":
                for (int i = 0; i < 3; i++) { SpawnGreen(); }
                break;
            case "blue":
                SpawnBlue();
                break;
            case "green_cluster":
                for (int i = 0; i < 12; i++) { SpawnGreen(); }
                break;
            case "red_cluster":
                //spawn cluster of 'red' type enemy
                for (int i = 0; i < 5; i++) { SpawnRed(); }
                break;
            case "blue_cluster":
                //spawn cluster of 'blue' type enemy
                for (int i = 0; i < 5; i++) { SpawnBlue(); }
                break;
        }
        //pay for enemy
        Pay(type);
    }
    private void Pay(string type)
    {
        if (Enum.TryParse(type, out Cost spawnCost))
        {
            budget -= (int)spawnCost;
        }
    }

    private void SpawnRed()
    {
        Enemy e = new EnemyRed("monster-red.png", GetRandomStartPos(), EnemyDeath);
        AddEnemy(e);
    }
    private void SpawnBlue()
    {
        Enemy e = new EnemyBlue("monster-blue.png", GetRandomStartPos(), EnemyDeath);
        AddEnemy(e);
    }
    private void SpawnGreen()
    {
        Enemy e = new EnemyGreen("monster-green.png", GetRandomStartPos(), EnemyDeath);
        AddEnemy(e);
    }

    private void AddEnemy(Enemy e)
    {
        actives.Add(e);
        LateAddChild(e);
    }
}
