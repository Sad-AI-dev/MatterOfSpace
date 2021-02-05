using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

public class Bullet : Sprite
{
    private float speed = 15;
    readonly private int damage = 1;
    public string tag;
    public Bullet(Vector2 startPos, Vector2 direction, string type, string img) : base(img)
    {
        SetOrigin(width / 2, height / 2);
        //set rotation
        rotation = startPos.GetLookAtAngle(new Vector2(startPos.x + direction.x, startPos.y + direction.y));

        SetXY(startPos.x + width/2, startPos.y);
        SetScaleXY(2, 2);
        //set tag
        tag = type;
        Game.main.LateAddChild(this);
    }

    void Update()
    {
        Move(0, -speed);
        BoundsCheck();
    }

    public void SetSpeed(int s)
    {
        speed = s;
    }

    private void BoundsCheck()
    { //destroy bullet if offscreen
        if (x + width/2 < MyGame.bounds.x || x - width/2 > MyGame.bounds.x + MyGame.bounds.width)
            LateDestroy();
        if (y + height / 2 < 0 || y - height / 2 > game.height)
            LateDestroy();
    }

    void OnCollision(GameObject other)
    {
        if (other is Enemy && tag == "Player")
            EnemyHit(other as Enemy);
        if (other is Player && tag == "Enemy")
            PlayerHit(other as Player);
    }

    private void EnemyHit(Enemy en)
    {
        HealthManager health = en.healthManage;
        if (health.health > 0)
        {
            if (!health.invincible)
            {
                health.Damage(damage);
            }
            LateDestroy();
        }
    }
    private void PlayerHit(Player p)
    {
        p.HitTest();
        LateDestroy();
    }
}