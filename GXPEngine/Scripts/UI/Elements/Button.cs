using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
public class Button : AnimationSprite
{
    readonly private Action onClick;
    private bool played = false;
    readonly private string sfxFile;
    public Button(Action clickEvent, Vector2 pos, Vector2 size, string sfx) : base("button.png", 3, 1)
    {
        onClick = clickEvent;
        //set position and size
        SetOrigin(width / 2, height / 2);
        SetScaleXY(size.x, size.y);
        SetXY(pos.x, pos.y);
        //sfx to play
        sfxFile = sfx;
    }

    void Update()
    {
        ButtonUpdate();
    }
    public void ButtonUpdate()
    {
        if (HitTestPoint(Input.mouseX, Input.mouseY))
            MouseCheck();
        else if (played)
        { //reset
            SetFrame(0);
            played = false;
        }
    }

    void MouseCheck()
    {
        SetFrame(1);
        if (Input.GetMouseButton(0))
            SetFrame(2);
        if (Input.GetMouseButtonUp(0))
        {
            MyGame.PlaySFX(sfxFile, 1f);
            onClick?.Invoke();
        }
        else if (!played)
        {
            played = true;
            MyGame.PlaySFX("Sounds/buttonHover.wav", 1f);
        }
    }
}

