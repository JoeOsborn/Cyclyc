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

        List<CycSprite> sprites;

        protected float grade;
        public float Grade
        {
            get { return grade; }
        }

        List<CycBackground> backgrounds;
        List<Challenge>[] challenges;
        List<Challenge> otherPlayerChallenges;

        public CycGame(Game1 g)
        {
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
        public virtual EnemyMaker MakeRandomEnemy(bool leftToRight, int difficulty)
        {
            return null;
        }
        public void DeliverRandomEnemy(bool leftSide, int difficulty)
        {
            EnemyMaker enemy = MakeRandomEnemy(leftSide, difficulty);
            //is the next beat in a new measure?
            Challenge c=null;
            int nextBeat = (int)(Math.Floor(Game.CurrentBeat) + 1);
            //later, consider setting up at 4-measure bounadries
            int measure = nextBeat / 4;
            int beatInMeasure = nextBeat - (measure * 4);
            if (nextBeat % 4 == 0 || otherPlayerChallenges.Count == 0)
            {
                c = new Challenge(measure);
                if (otherPlayerChallenges.Count > 0)
                {
                    //TODO: not sure this is right
                    otherPlayerChallenges.Last().State = ChallengeState.Deployed;
                } 
                otherPlayerChallenges.Add(c);
            }
            else
            {
                c = otherPlayerChallenges.Last();
            }
            c.AddBeat(new ChallengeBeat(beatInMeasure, new EnemyMaker[] { enemy }));
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

        protected virtual void ProcessChallenges(int gradeLevel, GameTime gt)
        {
            List<Challenge> challengeList = challenges[gradeLevel];
            foreach (Challenge c in challengeList)
            {
                c.Process(gradeLevel, Grade, true);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
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