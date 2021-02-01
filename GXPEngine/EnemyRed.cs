using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using Setting = GameSettings.EnemySettings;
public class EnemyRed : Enemy
{
    readonly Timer AIUpdate;
    Vector2 moveDir;
    private int moveCounter = 0;
    private readonly int flipDelay = Setting.Red.SHOTS_PER_MOVE;
    public EnemyRed(string fileName, Vector2 startPos, Action<Enemy> deathEvent) : base(fileName, new Vector2(3, 1), deathEvent)
    {
        SetOrigin(width / 2, height / 2);
        SetScaleXY(6, 6);
        Spawn(startPos);

        //set health
        healthManage = new HealthManager(Setting.Red.LIVES, CreateDeathEvent(), GotHurt, this);

        //set stats
        stats.reward = Setting.Red.SCORE;
        stats.moveSpeed = Setting.Red.MOVE_SPEED;

        //AI behaviour
        if (Utils.Random(0, 2) < 1)
            moveDir = new Vector2(1f, 1f);
        else
            moveDir = new Vector2(-1f, 1f);
        moveDir.SetLength(stats.moveSpeed);
        //AI timer
        AIUpdate = new Timer(TakeAction, Setting.Red.ACT_TIME, true);
        AIUpdate.currentTime = Utils.Random(0, Setting.Red.ACT_TIME);
    }

    void Update()
    {
        if (healthManage.health > 0)
        {
            Move(moveDir.x * stats.moveSpeed, moveDir.y * stats.moveSpeed);
            BoundCheck();
        }
        else
        {
            Move(0, -moveDir.y / 2);
        }
    }

    private void TakeAction()
    {
        Shoot();
        MoveUpdate();
    }

    private void Shoot()
    {
        Vector2 shootDir = new Vector2(MyGame.scenes.player.x - x, MyGame.scenes.player.y - y);
        Bullet bul = new Bullet(new Vector2(x, y + (height / 2)), shootDir, "Enemy", "enemyBullet.png");
        bul.SetSpeed(10);
        //play shoot sound
        MyGame.PlaySFX("Sounds/enemyShoot.wav", 0.3f);
    }

    private void MoveUpdate()
    {
        moveCounter++;
        if (moveCounter >= flipDelay)
        {
            //flip direction to create zigzag pattern
            moveDir.x = -moveDir.x;
            moveCounter = 0;
        }
    }

    private void Die()
    {
        AIUpdate.LateDestroy();
    }

    private Action CreateDeathEvent()
    {
        Action deathEvent = Die;
        deathEvent += DieGeneric;
        return deathEvent;
    }
}
