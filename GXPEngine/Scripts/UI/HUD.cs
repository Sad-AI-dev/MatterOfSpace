using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;
using GXPEngine.Core;
public class HUD : Canvas
{
    readonly public StringFormat centerFormat, leftFormat;
    public HUD() : base(Game.main.width, Game.main.height)
    {
        centerFormat = new StringFormat()
        { //create text format
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        leftFormat = new StringFormat()
        { //create left text format
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        };
    }

    //generic draw functions
    public void DrawButton(Action clickEvent, Vector2 pos, string text)
    { //button template
        AddChild(new Button(clickEvent, pos, new Vector2(10, 8), "Sounds/buttonConfirm.wav"));
        DrawText(text, pos, 20);
    }
    public void DrawButton(Action clickEvent, Vector2 pos, string text, string sfxFile)
    { //button template
        AddChild(new Button(clickEvent, pos, new Vector2(10, 8), sfxFile));
        DrawText(text, pos, 20);
    }
    public void DrawText(string Text, Vector2 pos, int fontSize)
    {
        Font f = new Font(MyGame.fonts.Families[0], fontSize, FontStyle.Bold, GraphicsUnit.Point);
        graphics.DrawString(Text, f, Brushes.White, new Point((int)pos.x, (int)pos.y), centerFormat);
    }
    public void DrawText(string Text, Vector2 pos, int fontSize, StringFormat format)
    {
        Font f = new Font(MyGame.fonts.Families[0], fontSize, FontStyle.Bold, GraphicsUnit.Point);
        graphics.DrawString(Text, f, Brushes.White, new Point((int)pos.x, (int)pos.y), format);
    }
    public void SetSprite(string file, Vector2 pos, Vector2 size)
    {
        Sprite s = new Sprite(file, false, false);
        s.SetOrigin(s.width / 2, s.height / 2); //center origin
        s.SetScaleXY(size.x, size.y); //set size
        s.SetXY(pos.x, pos.y); //set position
        AddChild(s);
    }
}

public class HUDMainMenu : HUD
{
    public HUDMainMenu() : base()
    {
        DrawMain();
    }

    void DrawMain()
    {
        DrawText("Matter of Space", new Vector2(game.width / 2, height * 0.1f), 50);
        //start game button
        DrawButton(Start, new Vector2(game.width / 2, game.height * 0.65f), "Start game");
        //quit game button
        DrawButton(Quit, new Vector2(game.width / 2, game.height * 0.8f), "Quit game");
        DrawTutorial();
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

    //button functions
    void Start()
    {
        MyGame.scenes.LoadScene(1);
    }
    void Quit()
    {
        Environment.Exit(1);
    }
}
public class HUDGameOver : HUD
{
    public HUDGameOver() : base()
    {
        new Timer(DrawGameOver, 60);
    }

    void DrawGameOver()
    {
        DrawText("Game Over", new Vector2(game.width / 2, game.height * 0.1f), 50);
        //score banner
        DrawText("your score: " + MyGame.scenes.score, new Vector2(game.width / 2, game.height * 0.2f), 50);
        //replay game button
        DrawButton(ReturnToMain, new Vector2(game.width / 2, game.height / 2 - 50), "Replay", "Sounds/buttonConfirm2.wav");
        //quit game button
        DrawButton(Quit, new Vector2(game.width / 2, game.height / 2 + 100), "Quit game");
    }
    //button functions
    void ReturnToMain()
    {
        MyGame.scenes.LoadScene(0);
    }
    void Quit()
    {
        Environment.Exit(1);
    }
}

public class HUDGamePlay : HUD
{
    private List<Sprite> liveSprites;

    public HUDGamePlay() : base()
    {
        MyGame.scenes.score = 0;
        MyGame.scenes.scoreUpdate += ScoreIncreaseEvent;

        liveSprites = new List<Sprite>();
    }

    void Update()
    {
        graphics.Clear(Color.Empty);
        //draw score text
        DrawText("Score:", new Vector2(game.width * 0.01f, 80), 40, leftFormat);
        DrawText(MyGame.scenes.score.ToString(), new Vector2(game.width * 0.01f, 140), 30, leftFormat);
        //extra lives text
        DrawText("Extra Lives", new Vector2(game.width * 0.01f, game.height * 0.40f), 25, leftFormat);

        if (MyGame.scenes.player.GetHealth() != liveSprites.Count + 1)
            UpdateHealth(); //draw appropiate amount of sprites for player lives

        //pause
        if (Input.GetKeyDown(Key.ESCAPE))
            new HUDPause();
    }

    private void ScoreIncreaseEvent(int gained, Vector2 pos)
    {
        FadeText fader = new FadeText(pos, "+" + gained, 25, this);
        LateAddChild(fader);
    }

    private void UpdateHealth()
    {
        ResetHealth();
        //health images (should be one less then lives, represents EXTRA lives)
        for (int i = 0; i < MyGame.scenes.player.GetHealth() - 1; i++)
        {
            Sprite sprite = new Sprite("extraLife.png", false, false);
            sprite.SetScaleXY(5.5f, 5.5f);
            sprite.SetXY(40 + ((i%3) * 110), game.height * 0.45f + (90 * Mathf.Floor(i/3)));
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
}

public class HUDPause : HUD
{
    readonly private Button unPause, toMain;
    readonly private Color bgColor;
    public HUDPause() : base()
    {
        Game.main.AddChild(this);
        //instantiate elements
        unPause = new Button(UnPause, new Vector2(game.width / 2, game.height * 0.6f), new Vector2(10, 8), "Sounds/buttonConfirm2.wav");
        LateAddChild(unPause);
        toMain = new Button(ToMain, new Vector2(game.width / 2, game.height * 0.75f), new Vector2(10, 8), "Sounds/buttonConfirm.wav");
        LateAddChild(toMain);
        //set bg color
        bgColor = Color.FromArgb(120, 0, 0, 0);
        //pause game
        Pause();
    }

    private void Show()
    {
        DisplayUI();
        GetInput();
    }
    private void DisplayUI()
    {
        graphics.Clear(bgColor);
        DrawText("PAUSED", new Vector2(game.width / 2, game.height * 0.4f), 100);
        unPause.ButtonUpdate();
        DrawText("Resume", new Vector2(game.width / 2, game.height * 0.6f), 20);
        toMain.ButtonUpdate();
        DrawText("Main Menu", new Vector2(game.width / 2, game.height * 0.75f), 20);
    }
    private void GetInput()
    {
        if (Input.GetKeyDown(Key.ESCAPE))
            UnPause();
    }

    private void Pause()
    {
        //pause game + update loop
        Game.main.OnBeforeStep += Show;
    }
    private void UnPause()
    {
        Game.main.OnBeforeStep -= Show;
        LateDestroy();
    }

    private void ToMain()
    {
        UnPause();
        MyGame.scenes.LoadScene(0);
    }
}