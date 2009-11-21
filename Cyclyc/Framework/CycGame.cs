using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Cyclyc.JetpackGirl;
using Cyclyc.ShipGirl;


namespace Cyclyc.Framework
{
    public delegate CycEnemy EnemyMaker(Challenge c);

    public class CycGame : Object
    {
        Game1 game;
        public Game1 Game
        {
            get { return game; }
        }
        public GraphicsDevice GraphicsDevice
        {
            get { return Game.GraphicsDevice; }
        }
        Viewport view;
        public Viewport View
        {
            get { return view; }
            set 
            { 
                view = value;
                foreach (CycBackground bg in backgrounds)
                {
                    bg.View = View;
                }
                foreach (CycSprite sprite in sprites)
                {
                    sprite.View = view;
                }
            }
        }

        public CycGame NextGame { get; set; }

        List<CycSprite> sprites;

        protected float grade;
        public float Grade
        {
            get { return grade; }
        }

        List<CycBackground> backgrounds;
        List<Challenge>[] challenges;
        List<Challenge> otherPlayerChallenges;

        double[] GradeWeights { get; set; }
        double GradeModifier { get; set; }

        protected Random rgen;

        public int NextMeasureLeftDifficulty { get; set; }
        public int NextMeasureRightDifficulty { get; set; }

        int lastMeasure;
        public CycGame(Game1 g)
        {
            lastMeasure = -1;
            GradeWeights = new double[] { 1.0, 1.0, 1.0,     0.3 };
            GradeModifier = 1.0;
            rgen = new Random();
            game = g;
            backgrounds = new List<CycBackground>();
            grade = 0.0f;
            sprites = new List<CycSprite>();
            challenges = new List<Challenge>[3] { 
                new List<Challenge>(), 
                new List<Challenge>(), 
                new List<Challenge>() 
            };
            otherPlayerChallenges = new List<Challenge>();
        }

        protected void AddBackground(string bgName, float bgSpeed)
        {
            CycBackground bg = new CycBackground(Game, bgName);
            bg.ScrollSpeed = bgSpeed;
            bg.View = View;
            backgrounds.Add(bg);
        }

        public void TriggerChallenge(int gradeLevel, Challenge c)
        {
            challenges[gradeLevel].Add(c);
            c.CycGame = this;
            c.Game = Game;
        }
        public virtual EnemyMaker MakeEnemy(bool leftToRight, int difficulty)
        {
            return null;
        }
        public void DeliverEnemy(bool leftSide, int difficulty)
        {
            if (leftSide)
            {
                NextMeasureLeftDifficulty += difficulty;
            }
            else
            {
                NextMeasureRightDifficulty += difficulty;
            }
        }

        public void AddSprite(CycSprite cs)
        {
            sprites.Add(cs);
            cs.View = view;
            //sprites added after LoadContent is called won't get their content loaded.  sucks!
        }

        public virtual void Initialize()
        {
            foreach (CycBackground bg in backgrounds)
            {
                bg.Initialize();
            }
            foreach (CycSprite sprite in sprites)
            {
                sprite.Initialize();
            }
        }

        protected virtual void SetupChallenges()
        {

        }

        public virtual void LoadContent()
        {
            foreach (CycBackground bg in backgrounds)
            {
                bg.LoadContent();
            }
            foreach (CycSprite sprite in sprites)
            {
                sprite.LoadContent();
            }
            SetupChallenges();
        }

        protected virtual void CoalesceChallengeBeats(Challenge c)
        {
            EnemyMaker[] enemies = new EnemyMaker[NextMeasureLeftDifficulty + NextMeasureRightDifficulty];
            for (int i = 0; i < NextMeasureLeftDifficulty; i++)
            {
                enemies[i] = MakeEnemy(true, 1);
            }
            for (int j = 0; j < NextMeasureRightDifficulty; j++)
            {
                enemies[j + NextMeasureLeftDifficulty] = MakeEnemy(false, 1);
            }
            c.AddBeat(new ChallengeBeat(0, enemies));
        }

        protected void TriggerOtherPlayerChallenge()
        {
            Challenge c = new Challenge(this, Game, (int)Math.Floor(Game.CurrentMeasure));
            CoalesceChallengeBeats(c);
            otherPlayerChallenges.Add(c);
            NextMeasureLeftDifficulty = 0;
            NextMeasureRightDifficulty = 0;
        }

        protected virtual void ProcessChallenges(int gradeLevel, GameTime gt)
        {
            List<Challenge> challengeList = challenges[gradeLevel];
            foreach (Challenge c in challengeList)
            {
                c.Process(gradeLevel, Grade, true);
            }
        }

        protected virtual void CalculateGrade()
        {
            double prospectiveGrade = 0.0;
            double avgGrade = 0.0;
            if (otherPlayerChallenges.Count > 0)
            {
                //average per-challenge grade of otherPlayerChallenges
                foreach (Challenge c in otherPlayerChallenges)
                {
                    if (c.EnemyCount > 0)
                    {
                        avgGrade += ((double)c.EnemiesKilled / (double)c.EnemyCount);
                    }
                }
                avgGrade /= otherPlayerChallenges.Count;
                prospectiveGrade += avgGrade * GradeWeights[3];
            }
            for (int i = 0; i < 3; i++)
            {
                if (challenges[i].Count == 0) { continue; }
                avgGrade = 0.0;
                //average per-challenge grade of each challenge
                foreach (Challenge c in challenges[i])
                {
                    if (c.EnemyCount > 0)
                    {
                        avgGrade += ((double)c.EnemiesKilled / (double)c.EnemyCount);
                    }
                }
                avgGrade /= challenges[i].Count;
                prospectiveGrade += avgGrade * GradeWeights[i];
            }
            grade = (float)(prospectiveGrade * GradeModifier);
            Console.WriteLine("new grade: " + grade);
        }

        public virtual void Update(GameTime gameTime)
        {
            if ((int)(Game.CurrentMeasure) != lastMeasure)
            {
                CalculateGrade();
                TriggerOtherPlayerChallenge();
            }
            lastMeasure = (int)(Game.CurrentMeasure);
            ProcessChallenges(0, gameTime);
            ProcessChallenges(1, gameTime);
            ProcessChallenges(2, gameTime);
            foreach (Challenge c in otherPlayerChallenges)
            {
                c.Process(Grade, Grade, false);
            }
            foreach (CycBackground bg in backgrounds)
            {
                bg.Update(gameTime);
            }
            foreach (CycSprite sprite in sprites)
            {
                sprite.Update(gameTime);
            }
        }
        protected virtual void SetupFilters()
        {
            GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.None;
            GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.None;
            GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.None;
        }
        protected virtual Color ClearColor()
        {
            return Color.Gray;
        }
        protected virtual void DrawInnards(GameTime gt)
        {
//            GraphicsDevice.Clear(ClearColor());
        }
        public virtual void Draw(GameTime gameTime)
        {
            Viewport defaultVP = GraphicsDevice.Viewport;
            GraphicsDevice.Viewport = view;
            ((Game1)Game).SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            SetupFilters();
            foreach (CycBackground bg in backgrounds)
            {
                bg.Draw(gameTime);
            }
            DrawInnards(gameTime);
            foreach (CycSprite sprite in sprites)
            {
                sprite.Draw(gameTime);
            }
            ((Game1)Game).SpriteBatch.End();
            GraphicsDevice.Viewport = defaultVP;
        }
    }
}