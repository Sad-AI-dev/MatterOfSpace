using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;
using Setting = GameSettings.HUDSettings.BlinkSettings;
public class BlinkText : GameObject
{
    readonly private Canvas canvas;

    readonly private StringFormat format;
    readonly private Font font;
    readonly private Vector2 displayPos;

    readonly private int repeatCount = Setting.BLINK_COUNT;
    private int blinkCount = 0;
    private bool invis = false;
    private Color c;
    public BlinkText(int fontSize, Vector2 pos, Canvas displayTarget)
    {
        canvas = displayTarget;
        displayPos = pos;
        format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        font = new Font(MyGame.fonts.Families[0], fontSize, FontStyle.Bold, GraphicsUnit.Point);
        c = Color.FromArgb(175, 255, 255, 255);
        new Timer(ToggleBlink, Setting.BLINK_TIME);
    }

    void Update()
    {
        canvas.graphics.DrawString("Wave Incoming", font, new SolidBrush(c), new Point((int)displayPos.x, (int)displayPos.y), format);
    }

    private void ToggleBlink()
    {
        blinkCount++;
        invis = !invis;
        SetColor();
        BlinkUpdate();
    }
    private void SetColor()
    {
        int alpha = 175;
        if (invis)
            alpha = 0;
        c = Color.FromArgb(alpha, 255, 255, 255);
    }
    private void BlinkUpdate()
    {
        if (blinkCount < repeatCount)
            new Timer(ToggleBlink, Setting.BLINK_TIME);
        else
            LateDestroy();
    }
}
