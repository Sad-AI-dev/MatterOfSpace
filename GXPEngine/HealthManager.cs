using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
public class HealthManager
{
    readonly private Sprite sprite;
    public int health;
    public bool invincible = false;
    readonly private Action deathEvent, hurtEvent;

    private Timer flashTimer = null;
    private bool flash = false;
    private int invinceTime = GameSettings.EnemySettings.INVINCE_TIME;

    public HealthManager(int maxHealth, Action deathFunction, Sprite owner)
    {
        health = maxHealth;
        deathEvent = deathFunction;
        sprite = owner;
        SetSettings();
    }

    public HealthManager(int maxHealth, Action deathFunction, Action hurtFunction, Sprite owner)
    {
        health = maxHealth;
        deathEvent = deathFunction;
        sprite = owner;
        hurtEvent = hurtFunction;
        SetSettings();
    }

    private void SetSettings()
    {
        if (sprite is Player)
        {
            invinceTime = GameSettings.PlayerSettings.INVINCE_TIME;
        }
    }

    public void Damage(int damage)
    {
        health -= damage;
        invincible = true;
        Timer invincibleTimer = new Timer(EndFlash, invinceTime);
        flashTimer = new Timer(ToggleFlash, 3, true);
        EventCheck();
    }

    private void EventCheck()
    {
        hurtEvent?.Invoke();
        if (health <= 0)
        {
            deathEvent?.Invoke();
        }
    }

    private void InvincibleEnd()
    {
        invincible = false;
        if (health <= 0) {
            sprite.LateDestroy();
        }
    }

    private void ToggleFlash()
    {
        flash = !flash;
        if (flash) { sprite.SetColor(1, 1, 1); }
        else { sprite.SetColor(0.3f, 0.3f, 0.3f); }
    }

    private void EndFlash()
    {
        flash = false;
        flashTimer.LateDestroy();
        sprite.SetColor(1, 1, 1);
        Timer tailEndTimer = new Timer(InvincibleEnd, 5);
    }
}
