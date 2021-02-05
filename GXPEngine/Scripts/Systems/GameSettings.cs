using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
public static class GameSettings
{
    //general settings
    public const float SFX_VOLUME = 0.3f;

    //UI settings
    readonly public struct HUDSettings
    {
        public const int FADE_TIME = 20;
        public const int FADE_DELAY = 30;
    }

    //settings for the player
    readonly public struct PlayerSettings
    {
        public const int LIVES = 3;
        public const int BORDER_OFFSET = 80;
        public const float SPEED = 9f, MOVE_SMOOTHING = 0.85f;
        public const int FIRERATE = 10;
        public const int INVINCE_TIME = 60;
    }

    readonly public struct WaveSpawnerSettings 
    {
        public enum Costs //cost of spawning enemy type
        {
            red = 10,
            green = 20,
            blue = 40,
            green_cluster = 100,
            red_cluster = 140,
            blue_cluster = 320
        }
        //starting budget, increase of budget per round
        public const int START_BUDGET = 20, BUDGET_SCALING = 10;
        public const float SCALING_INCREASE = 1.5f;
        public const int SCALING_INCREASE_DELAY = 5; //how many waves before scaling increase?
        public const int WAVE_DELAY = 100;
        //how many most expensive should spawner choose from
        public const int CHOICE_POOL = 3;
        //chace to replace enemy with green monster (1 in x)
        public const int GREEN_CHANCE = 3;
    }

    //hold all enemy info
    public class EnemySettings
    {
        public const int INVINCE_TIME = 30;
        //represented in a 1 in x chance
        public const int LIFE_DROP_CHANCE = 10;
        //score reduction at loop
        public const float SCORE_FALLOFF = 0.9f;
        readonly public struct Red //settings for 'red' type
        {
            public const int LIVES = 3, SCORE = 100;
            public const float MOVE_SPEED = 1.6f;
            public const int ACT_TIME = 80, SHOTS_PER_MOVE = 2;
        }
        readonly public struct Blue //settings for 'blue' type
        {
            public const int LIVES = 5, SCORE = 350;
            public const float MOVE_SPEED = 1.5f;
            public const int ANIM_SPEED = 10;
            public const float MIN_MOVE = 300;
            public const int CHARGE_TIME = 80, BEAM_TIME = 230;
            public const int START_REST = 40, END_REST = 100;
        }
        readonly public struct Green //settings for 'green' type
        {
            public const int LIVES = 2, SCORE = 50;
            public const float MOVE_SPEED = 3.0f;
            public const int ACT_TIME = 20;
            public const float MAX_SWAY = 0.8f, SWAY_SIZE = 0.4f; //how much the direction can change per update
            public const float CHACE_RANGE = 400;
            //decrease drop chance
            public const int LIFE_DROP_CHANCE = 25;
        }
    }

    public struct PickUpSettings
    {
        public const float SPEED = 2.5f;
    }

    public struct BeamSettings
    {
        public const float GROW_SPEED = 0.4f;
    }
}
