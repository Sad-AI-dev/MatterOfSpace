using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

public class Player : Sprite
{
    private int borderOffset, fireRate;
    private float speed, moveSmoothing;
    private Vector2 velocity;

    private Timer bulletTimer;
    private bool cooling = false;

    readonly private HealthManager health;

    public Player() : base("player.png") {
        LoadSettings();
        health = new HealthManager(GameSettings.PlayerSettings.LIVES, GameOver, this);
        SetOrigin(width / 2, height / 2);
        SetScaleXY(5, 5);
        Spawn();
    }

    void LoadSettings()
    {
        borderOffset = GameSettings.PlayerSettings.BORDER_OFFSET;
        fireRate = GameSettings.PlayerSettings.FIRERATE;
        speed = GameSettings.PlayerSettings.SPEED;
        moveSmoothing = GameSettings.PlayerSettings.MOVE_SMOOTHING;
    }

    void Update() {
        KeyMoveSmooth();
        ShootInput();
    }

    private void ShootInput() {
        if (bulletTimer == null && Input.GetMouseButton(0)) { //start loop
            Shoot(); //instant response
            bulletTimer = new Timer(Shoot, fireRate, true);
        }
        if (Input.GetMouseButtonUp(0) && !cooling) { //destroy and reset
            bulletTimer?.Destroy();
            Timer timer = new Timer(EndShoot, fireRate / 2);
            cooling = true;
        }
    }

    private void Shoot() {
        Bullet bullet = new Bullet(new Vector2(x, y - height / 2), new Vector2(0, -1), "Player", "bullet.png");
        //play shoot sfx
        MyGame.PlaySFX("Sounds/playerShoot.wav", 0.3f);
    }

    private void EndShoot() {
        bulletTimer?.Destroy();
        bulletTimer = null;
        cooling = false;
    }

    private void KeyMoveSmooth()
    {
        if (velocity.GetLength() > 0) {
            velocity.SetLength(velocity.GetLength() * moveSmoothing);
        }
        Vector2 newMove = GetMoveInput();
        if (Math.Abs(velocity.x) < Math.Abs(newMove.x)) { velocity.x = newMove.x; }
        if (Math.Abs(velocity.y) < Math.Abs(newMove.y)) { velocity.y = newMove.y; }
        Move(velocity.x, velocity.y);
    }

    private Vector2 GetMoveInput()
    {
        Vector2 toMove = new Vector2(0, 0);
        if (Input.GetKey(Key.A) && x > borderOffset) { //left
            toMove.x -= speed;
        }
        if (Input.GetKey(Key.D) && x < game.width - borderOffset) { //right
            toMove.x += speed;
        }
        if (Input.GetKey(Key.W) && y > borderOffset) { //up
            toMove.y -= speed;
        }
        if (Input.GetKey(Key.S) && y < game.height - borderOffset) { //down
            toMove.y += speed;
        }
        if (toMove.x != 0 && toMove.y != 0) {
            toMove.SetLength(speed);
        }
        return toMove;
    }

    public void Spawn()
    {
        SetXY((game.width - width) / 2, (game.height - height) * 0.8f);
        velocity = new Vector2(0, 0);
    }

    private void GameOver()
    {
        MyGame.scenes.LoadScene(2);
    }

    void OnCollision (GameObject other)
    {
        if (other is Enemy) //collide with enemy
        {
            Enemy e = other as Enemy;
            if (e.healthManage.health > 0) //make sure enemy is alive
                HitTest();
        }
    }

    public void HitTest()
    {
        if (!health.invincible)
        {
            health.Damage(1);
            //play player got hit sfx
            MyGame.PlaySFX("Sounds/playerHit.wav", 2f);
        }
    }

    public int GetHealth()
    {
        return health.health;
    }

    public void GainHealth(int toGain)
    {
        health.health += toGain;
    }
}