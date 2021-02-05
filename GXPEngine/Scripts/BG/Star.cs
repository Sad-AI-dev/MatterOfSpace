using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
public class Star : Sprite
{
    public int speed;

    readonly private Action<Star, Rectangle> resetEvent;
    readonly private Rectangle startZone;

    public Star(Action<Star, Rectangle> reset, Rectangle normalStart) : base("Star.png", false, false)
    {
        SetColor(0.8f, 0.8f, 0.8f);

        SetOrigin(width / 2, height / 2);
        startZone = normalStart;
        rotation = Utils.Random(0, 360);

        resetEvent += reset;
        resetEvent?.Invoke(this, new Rectangle(MyGame.bounds.x, 0, MyGame.bounds.width, game.height));
    }

    void Update()
    {
        if (y > game.height) { resetEvent?.Invoke(this, startZone); }
        y += speed;
        Turn(1);
    }
}
