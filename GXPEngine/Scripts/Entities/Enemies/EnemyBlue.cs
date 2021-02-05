using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using Setting = GameSettings.EnemySettings.Blue;
public class EnemyBlue : Enemy
{
    private enum State
    {
        move,
        charge,
        beam,
        die
    }
    private State state;

    readonly private Rectangle moveRect;
    private Vector2 target, moveDir;

    public EnemyBlue(string fileName, Vector2 startPos, Action<Enemy> deathEvent) : base(fileName, new Vector2(3, 3), deathEvent)
    {
        SetOrigin(width / 2, height / 2);
        SetScaleXY(10, 10);
        SetXY(startPos.x, startPos.y);

        //set health
        healthManage = new HealthManager(Setting.LIVES, CreateDeathEvent(), CreateHurtEvent(), this);

        stats.reward = Setting.SCORE;
        stats.moveSpeed = Setting.MOVE_SPEED;
        stats.animated = true;

        moveRect = new Rectangle(MyGame.bounds.x + width, game.height * 0.05f, MyGame.bounds.width - (width * 2), game.height * 0.3f);

        //start AI
        SetAnimation();
        StartMove();
    }

    void Update()
    {
        Animate();
        switch(state)
        {
            case State.move: //move towards chosen spot
                MoveUpdate();
                break;
        }
        if (healthManage.health <= 0)
        {
            Move(0, -1f);
        }
    }
    //move into position
    private void StartMove()
    {
        state = State.move;
        SetTarget();
        moveDir.x = target.x - x;
        moveDir.y = target.y - y;
        moveDir.SetLength(Setting.MOVE_SPEED);
    }
    private void SetTarget()
    {
        target.x = Utils.Random(moveRect.x, moveRect.x + moveRect.width);
        target.y = Utils.Random(moveRect.y, moveRect.y + moveRect.height);
        if (target.GetDistance(new Vector2(x, y)) <= Setting.MIN_MOVE)
        { //random new location is too close
            SetTarget();
        }
    }

    private void MoveUpdate()
    {
        float dist = target.GetDistance(new Vector2(x + moveDir.x, y + moveDir.y));
        if (dist > 5)
        {
            if (dist < moveDir.GetLength())
            {
                moveDir.SetLength(dist);
            }
            Move(moveDir.x, moveDir.y);
        }
        else //start beam charge
        {
            state = State.charge;
            Timer timer = new Timer(StartCharge, Setting.START_REST);
        }
    }

    //start beam charge
    private void StartCharge()
    {
        SetAnimation();
        Timer timer = new Timer(StartBeam, Setting.CHARGE_TIME);
        //sfx
        if (healthManage.health > 0)
        {
            MyGame.PlaySFX("Sounds/enemyChargeBeam.wav", 0.8f);
        }
    }

    //shoot beam
    private void StartBeam()
    {
        state = State.beam;
        SetAnimation();
        //shoot beam
        Beam beam = new Beam("beam.png", this);
        Timer timer = new Timer(EndBeam, Setting.BEAM_TIME);
        //sfx
        if (healthManage.health > 0)
        {
            MyGame.PlaySFX("Sounds/enemyShootBeam.wav", 1.0f);
        }
    }
    private void EndBeam()
    {
        state = State.move;
        SetAnimation();
        state = State.charge;
        Timer timer = new Timer(StartMove, Setting.END_REST);
        //reward fast players
        LosePoints();
    }

    private void SetAnimation()
    {
        switch(state)
        {
            case State.move:
                SetCycle(0, 1, 255, false);
                break;
            case State.charge:
                SetCycle(3, 3, Setting.ANIM_SPEED);
                break;
            case State.beam:
                SetCycle(6, 2, Setting.ANIM_SPEED);
                break;
            case State.die:
                SetCycle(2, 1, 255, false);
                break;
        }
    }

    private void Die()
    {
        state = State.die;
        SetAnimation();
    }
    private void Hurt()
    {
        SetCycle(1, 1, 255, false);
        Timer timer = new Timer(SetAnimation, GameSettings.EnemySettings.INVINCE_TIME + 5);
    }

    private Action CreateDeathEvent()
    {
        Action deathEvent = DieGeneric;
        deathEvent += Die;
        return deathEvent;
    }
    private Action CreateHurtEvent()
    {
        Action hurtEvent = GotHurt;
        hurtEvent += Hurt;
        return hurtEvent;
    }
}
