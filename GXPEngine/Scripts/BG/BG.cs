using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
public class BG : GameObject
{
    public Rectangle spawnZone;
    readonly private int starCount = 40;

    public BG()
    {
        spawnZone = new Rectangle(MyGame.bounds.x, -Game.main.height * 0.3f, MyGame.bounds.width, Game.main.height * 0.2f);
        for (int i = 0; i < starCount; i++)
        { //spawn background star
            Star star = new Star(ResetStar, spawnZone);
            AddChild(star);
        }
    }

    private void ResetStar(Star star, Rectangle zone)
    { //reset start to top of screen, bigger stars go faster for paralex effect
        int newScale = Utils.Random(2, 6);
        star.SetScaleXY(newScale, newScale);
        star.SetXY(Utils.Random(zone.x, zone.x + zone.width), Utils.Random(zone.y, zone.y + zone.height));
        star.speed = 1 + newScale;
    }
}
