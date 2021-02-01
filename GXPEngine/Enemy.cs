using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

public class Enemy : AnimationSprite
{
    //stats struct
    public struct Stats
    {
        public int reward;
        public float moveSpeed;
        public bool animated;
        //chance to drop extra life
        public int dropChance;
    }

    public HealthManager healthManage;
    public Stats stats;

    private readonly Action<Enemy> deathEvent;

    public Enemy(string fileName, Vector2 spriteSize, Action<Enemy> deathEvent) : base(fileName, (int)spriteSize.x, (int)spriteSize.y)
    {
        stats.dropChance = GameSettings.EnemySettings.LIFE_DROP_CHANCE;
        this.deathEvent = deathEvent;
    }

    public void Spawn(Vector2 pos)
    {
        SetXY(pos.x, pos.y);
    }

    public void BoundCheck()
    {
        if (y > game.height + height) //vertical loop
        {
            y = 0 - height;
            x = Utils.Random(0, game.width);
            //reward player for quickly defeating enemy
            LosePoints();
        }
        x = Mathf.Clamp(x, width, game.width - width);
    }

    public void DieGeneric()
    {
        deathEvent?.Invoke(this);
        MyGame.scenes.GainScore(stats.reward);
        //chance to spawn health pickup
        if (Utils.Random(0, stats.dropChance) < 1)
        {
            Timer timer = new Timer(SpawnPickUp, GameSettings.EnemySettings.INVINCE_TIME);
        }
    }

    private void SpawnPickUp()
    {
        parent.LateAddChild(new ExtraLife("extraLife.png", new Vector2(x, y)));
    }

    public void GotHurt()
    {
        if (healthManage.health > 0)
        {
            SetFrame(1);
            Timer timer = new Timer(HurtEnd, 20);
            //play enemy got hit sound
            MyGame.PlaySFX("Sounds/enemyHit.wav", 1);
            
        }
        else
        {
            SetFrame(2);
            //play enemy died
            MyGame.PlaySFX("Sounds/enemyDie.wav", 0.7f);
        }
    }

    private void HurtEnd()
    {
        if (!stats.animated)
            SetFrame(0);
    }

    public void LosePoints()
    {
        stats.reward = Mathf.Floor(stats.reward * GameSettings.EnemySettings.SCORE_FALLOFF);
        stats.reward -= stats.reward % 10; //prevent numbers below 10
    }
}
