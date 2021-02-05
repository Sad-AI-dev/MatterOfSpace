using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using Setting = GameSettings.PickUpSettings;
public class PickUp : Sprite
{
    public Action pickUpEvent;
    private readonly float speed = Setting.SPEED;
    public PickUp(string file, Vector2 startPos) : base(file)
    {
        SetOrigin(width / 2, height / 2);
        SetXY(startPos.x, startPos.y);
    }

    void Update()
    {
        y += speed;
        if (y > game.height + height)
        { //if offscreen, destroy
            LateDestroy();
        }
    }
}

public class ExtraLife : PickUp
{
    public ExtraLife(string file, Vector2 startPos) : base(file, startPos)
    {
        SetScaleXY(4, 4);
        pickUpEvent = GainLife;
    }

    private void GainLife()
    {
        MyGame.PlaySFX("Sounds/GetPickUp.wav", 1f);
        MyGame.scenes.player.GainHealth(1);
        LateDestroy();
    }

    void Update()
    {
        y += Setting.SPEED;
    }

    void OnCollision(GameObject other)
    {
        if (other is Player)
        {
            pickUpEvent?.Invoke();
        }
    }
}