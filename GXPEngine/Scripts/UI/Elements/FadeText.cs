using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;
using Setting = GameSettings.HUDSettings.FadeSettings;
class FadeText : GameObject
{
    readonly private int fadeTime = Setting.FADE_TIME; 
    private int timer = 0;
    private bool started = false;

    readonly private Canvas canvas;
    readonly private string text;
    readonly private Font font;
    readonly private StringFormat format;
    private Color c;
    private Vector2 drawPos;

    public FadeText(Vector2 drawPos, string text, int size, Canvas displayTarget)
    {
        this.text = text;
        this.drawPos = drawPos;
        canvas = displayTarget;
        font = new Font(MyGame.fonts.Families[0], size, FontStyle.Regular);
        format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        //initialize color
        c = Color.FromArgb(255, 255, 255, 255);
        //start timer
        new Timer(StartFade, Setting.FADE_DELAY);
    }
    private void StartFade()
    {
        started = true;
    }

    void Update()
    {
        if (started)
        {
            UpdateColor();
            drawPos.y -= 1.2f;
            timer++;
        }
        canvas.graphics.DrawString(text, font, new SolidBrush(c), new Point((int)drawPos.x, (int)drawPos.y), format);
    }

    void UpdateColor()
    { //update color to create fade effect
        if (timer >= fadeTime)
            LateDestroy();
        else
            c = Color.FromArgb(Mathf.Floor(255f * ((fadeTime - timer)) / fadeTime), 255, 255, 255);
    }
}