using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using Setting = GameSettings.BeamSettings;
public class Beam : Sprite
{
    public Beam(string file, Sprite owner) : base(file)
    {
        owner.AddChild(this);
        SetOrigin(width / 2, 0); //origin at mid top, allows for natural growth
        SetXY(0, 8);

        Timer timer = new Timer(LateDestroy, GameSettings.EnemySettings.Blue.BEAM_TIME);
    }

    void Update()
    {
        if (height < game.height)
        {
            scaleY += Setting.GROW_SPEED;
        }
    }

    void OnCollision(GameObject other)
    {
        if (other is Player)
        { //damage player on contact
            Player p = other as Player;
            p.HitTest();
        }
    }
}
