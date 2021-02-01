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
        blue = Setting.Costs.blue,
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
            SpawnEnemy(GetMostExpensive());
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
            new Timer(SpawnWave, Setting.WAVE_DELAY);
            new Timer(GiveWaveReward, 5);
        }
    }

    private void GiveWaveReward()
    { //give score bonus for clearing wave
        if (round > 0)
        {
            MyGame.scenes.GainScore(100 * round);
        }
    }

    private Vector2 GetRandomStartPos()
    {
        Vector2 pos;
        pos.x = Utils.Random(0, game.width);
        pos.y = Utils.Random(-game.height / 2, -50);
        return pos;
    }

    private string GetMostExpensive()
    {
        string expensive = "";
        int max = 0;
        foreach (int cost in Enum.GetValues(typeof(Cost)))
        {
            if (cost > max && cost <= budget)
            {
                max = cost;
                expensive = Enum.GetName(typeof(Cost), cost);
            }
        }
        return expensive;
    }

    private void SpawnEnemy(string type)
    {
        switch(type)
        {
            case "red":
                if (Utils.Random(0, Setting.GREEN_CHANCE) < 1)
                {
                    for (int i = 0; i < 2; i++) { SpawnGreen(); }
                }
                else
                    SpawnRed();
                break;
            case "blue":
                if (Utils.Random(0, Setting.GREEN_CHANCE) < 1)
                {
                    for (int i = 0; i < 8; i++) { SpawnGreen(); }
                }
                else
                    SpawnBlue();
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
