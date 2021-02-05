using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using Setting = GameSettings.EnemySettings.Green;
public class EnemyGreen : Enemy
{
    readonly private Timer AIUpdate;
    private Vector2 moveDir;
    public EnemyGreen(string file, Vector2 startPos, Action<Enemy> deathEvent) : base(file, new Vector2(3, 1), deathEvent)
    {
        SetOrigin(width / 2, height / 2);
        SetScaleXY(6, 6);
        Spawn(startPos);

        //set health
        healthManage = new HealthManager(Setting.LIVES, CreateDeathAction(), GotHurt, this);

        //set stats
        stats.reward = Setting.SCORE;
        stats.moveSpeed = Setting.MOVE_SPEED;
        stats.dropChance = Setting.LIFE_DROP_CHANCE;

        //AI setup
        moveDir = new Vector2(0, 1f); //straight down
        AIUpdate = new Timer(UpdateMoveDir, Setting.ACT_TIME, true);
    }

    void Update()
    {
        if (healthManage.health > 0)
        {
            Move(moveDir.x, moveDir.y);
            BoundCheck();
        }
        else
        {
            Move(0, 1f);
        }
    }

    private void UpdateMoveDir()
    {
        if (new Vector2(x, y).GetDistance(new Vector2(MyGame.scenes.player.x, MyGame.scenes.player.y)) < Setting.CHACE_RANGE)
            ChasePlayer();
        else
            RandomMove();
    }
    private void ChasePlayer()
    {
        if (y + height < MyGame.scenes.player.y)
        { //chase player
            moveDir.x = MyGame.scenes.player.x - x;
            moveDir.y = MyGame.scenes.player.y - y;
            moveDir.SetLength(stats.moveSpeed);
        }
        else
            RandomMove();
    }
    private void RandomMove()
    {
        moveDir.Normalize();
        moveDir.x += Utils.Random(-Setting.SWAY_SIZE, Setting.SWAY_SIZE);
        moveDir.x = Mathf.Clamp(moveDir.x, -Setting.MAX_SWAY, Setting.MAX_SWAY);
        moveDir.SetLength(stats.moveSpeed);
    }

    private Action CreateDeathAction()
    {
        Action deathAction = Die;
        deathAction += DieGeneric;
        return deathAction;
    }
    private void Die()
    {
        AIUpdate.LateDestroy();
    }
}
