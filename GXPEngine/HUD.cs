using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;
using GXPEngine.Core;
public class HUD : Canvas
{
    readonly private StringFormat centerFormat;
    public HUD(int sWidth, int sHeight, string type) : base(sWidth, sHeight)
    {
        centerFormat = new StringFormat();
        centerFormat.Alignment = StringAlignment.Center;
        centerFormat.LineAlignment = StringAlignment.Center;
        //draw screen
        CreateScreen(type);
    }

    void CreateScreen(string type)
    {
        switch(type)
        {
            case "mainMenu":
                DrawMainMenu();
                break;

            case "gameOver":
                Timer timer = new Timer(DrawGameOverMenu, 50);
                break;
        }
    }
    void DrawMainMenu()
    {
        DrawText("a matter of space", new Vector2(game.width / 2, height * 0.1f), 50);
        //start game button
        DrawButton(Start, new Vector2(game.width / 2, game.height * 0.65f), "Start game");
        //quit game button
        DrawButton(Quit, new Vector2(game.width / 2, game.height * 0.8f), "Quit game");
        DrawTutorial();
    }
    void DrawGameOverMenu()
    {
        DrawText("Game Over", new Vector2(game.width / 2, game.height * 0.1f), 50);
        //score banner
        DrawText("your score: " + MyGame.scenes.score, new Vector2(game.width / 2, game.height * 0.2f), 50);
        //replay game button
        DrawButton(ReturnToMain, new Vector2(game.width / 2, game.height / 2 - 50), "Replay", "Sounds/buttonConfirm2.wav");
        //quit game button
        DrawButton(Quit, new Vector2(game.width / 2, game.height / 2 + 100), "Quit game");
    }
    void DrawTutorial()
    {
        //banner
        DrawText("How to play", new Vector2(game.width / 2, game.height * 0.25f), 35);
        //keyboard to move
        DrawText("to move", new Vector2(game.width * 0.35f, game.height * 0.5f), 20);
        SetSprite("moveButtons.png", new Vector2(game.width * 0.35f, game.height * 0.38f), new Vector2(8, 8));
        //mouse to shoot
        DrawText("to shoot", new Vector2(game.width * 0.65f, game.height * 0.5f), 20);
        SetSprite("mouseClick.png", new Vector2(game.width * 0.66f, game.height * 0.38f), new Vector2(8, 8));
    }

    //generic draw functions
    void DrawButton(Action clickEvent, Vector2 pos, string text)
    { //button template
        AddChild(new Button(clickEvent, pos, new Vector2(10, 8), "Sounds/buttonConfirm.wav"));
        DrawText(text, pos, 20);
    }
    void DrawButton(Action clickEvent, Vector2 pos, string text, string sfxFile)
    { //button template
        AddChild(new Button(clickEvent, pos, new Vector2(10, 8), sfxFile));
        DrawText(text, pos, 20);

    }
    void DrawText(string Text, Vector2 pos, int fontSize)
    {
        Font f = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point);
        graphics.DrawString(Text, f, Brushes.White, new Point((int)pos.x, (int)pos.y), centerFormat);
    }
    void SetSprite(string file, Vector2 pos, Vector2 size)
    {
        Sprite s = new Sprite(file, false, false);
        s.SetOrigin(s.width / 2, s.height / 2); //center origin
        s.SetScaleXY(size.x, size.y); //set size
        s.SetXY(pos.x, pos.y); //set position
        AddChild(s);
    }
    //button functions
    void Start()
    {
        MyGame.scenes.LoadScene(1);
    }
    void ReturnToMain()
    {
        MyGame.scenes.LoadScene(0);
    }
    void Quit()
    {
        Environment.Exit(1);
    }
}

public class GamePlayHUD : Canvas
{
    readonly private Font scoreFont;
    readonly private StringFormat format;

    private List<Sprite> liveSprites;

    private List<FadeText> faders = new List<FadeText>();

    public GamePlayHUD(int sWidth, int sHeight) : base(sWidth, sHeight)
    {
        scoreFont = new Font("Arial", 30, FontStyle.Bold, GraphicsUnit.Point);
        format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;

        MyGame.scenes.score = 0;
        MyGame.scenes.scoreUpdate += ScoreIncreaseEvent;

        liveSprites = new List<Sprite>();
    }

    void Update()
    {
        //score text
        graphics.Clear(Color.Empty);
        graphics.DrawString("Score: " + MyGame.scenes.score, scoreFont, Brushes.White, new Point(game.width/2, 50), format);

        if (MyGame.scenes.player.GetHealth() != liveSprites.Count + 1)
        { //draw appropiate amount of sprites for player lives
            UpdateHealth();
        }

        //draw fade texts
        for (int i = 0; i < faders.Count; i++)
        {
            faders[i].DrawText(this, new Vector2(game.width / 2, game.height * 0.07f + (i * 40)));
        }
    }

    private void ScoreIncreaseEvent(int gained)
    {
        FadeText fader = new FadeText("+" + gained, EndFader);
        faders.Add(fader);
        LateAddChild(fader);
    }

    private void EndFader(FadeText fader)
    {
        faders.Remove(fader);
        fader.LateDestroy();
    }

    private void UpdateHealth()
    {
        ResetHealth();
        //health images (should be one less then lives, represents EXTRA lives)
        for (int i = 0; i < MyGame.scenes.player.GetHealth() - 1; i++)
        {
            Sprite sprite = new Sprite("extraLife.png", false, false);
            sprite.SetScaleXY(4, 4);
            sprite.SetXY(75 * i + 10, game.height - 80);
            liveSprites.Add(sprite);
            AddChild(sprite);
        }
    }

    private void ResetHealth()
    {
        foreach (Sprite sprit in liveSprites)
        {
            sprit.Destroy();
        }
        liveSprites = new List<Sprite>();
    }

    private class FadeText : GameObject
    {
        readonly private int fadeTime = GameSettings.HUDSettings.FADE_TIME; 
        private int timer = 0;
        readonly private Action<FadeText> endEvent;

        readonly private string text;
        readonly private Font font;
        readonly private StringFormat format;
        private Color c;

        public FadeText(string text, Action<FadeText> endEvent)
        {
            this.text = text;
            this.endEvent = endEvent;
            font = new Font("Arial", 25, FontStyle.Regular);
            format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            //initialize color
            c = Color.FromArgb(255, 255, 255, 255);
        }

        void Update()
        {
            UpdateColor();
            timer++;
        }

        public void DrawText(Canvas can, Vector2 pos)
        {
            can.graphics.DrawString(text, font, new SolidBrush(c), new Point((int)pos.x, (int)pos.y), format);
        }

        void UpdateColor()
        { //update color to create fade effect
            if (timer >= fadeTime)
            {
                endEvent?.Invoke(this);
            }
            else
            {
                c = Color.FromArgb(Mathf.Floor(255f * ((fadeTime - timer)) / fadeTime), 255, 255, 255);
            }
        }
    }
}
