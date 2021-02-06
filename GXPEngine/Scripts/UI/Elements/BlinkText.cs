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

    readonly private string text;
    readonly private bool infRepeat = false;

    readonly private int repeatCount = Setting.BLINK_COUNT, blinkTime;
    private int blinkCount = 0;
    private bool invis = false;
    private Color c;

    readonly public Timer timer;
    public BlinkText(string text, int fontSize, Vector2 pos, Canvas displayTarget)
    {
        canvas = displayTarget;
        displayPos = pos;
        this.text = text;
        format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        //font & color
        font = new Font(MyGame.fonts.Families[0], fontSize, FontStyle.Bold, GraphicsUnit.Point);
        c = Color.FromArgb(175, 255, 255, 255);
        //set Timer
        blinkTime = Setting.BLINK_TIME;
        timer = new Timer(ToggleBlink, blinkTime);
    }

    public BlinkText(string text, int fontSize, Vector2 pos, Canvas displayTarget, bool infRepeat, int blinkTime)
    {
        canvas = displayTarget;
        displayPos = pos;
        this.text = text;
        this.infRepeat = infRepeat;
        format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        //font & color
        font = new Font(MyGame.fonts.Families[0], fontSize, FontStyle.Bold, GraphicsUnit.Point);
        c = Color.FromArgb(175, 255, 255, 255);
        //set timer
        this.blinkTime = blinkTime;
        timer = new Timer(ToggleBlink, blinkTime);
    }

    void Update()
    {
        DrawText();
    }
    public void DrawText()
    {
        canvas.graphics.DrawString(text, font, new SolidBrush(c), new Point((int)displayPos.x, (int)displayPos.y), format);
    }

    private void ToggleBlink()
    {
        if (!infRepeat)
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
            new Timer(ToggleBlink, blinkTime);
        else
            LateDestroy();
    }
}
