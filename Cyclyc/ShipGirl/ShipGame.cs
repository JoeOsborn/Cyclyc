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
using Cyclyc.Framework;

namespace Cyclyc.ShipGirl
{
    public class ShipGame : Cyclyc.Framework.CycGame
    {
        Ship ship;
        ShipCircle crush;
        ShipCircle skim;
        ShipCircle debugRadius;
        ShipEnemyPool enemyBatch;
        protected float crushRecovery;

        protected Random rgen;
        public ShipGame(Game1 game)
            : base(game)
        {
            rgen = new Random();
            enemyBatch = new ShipEnemyPool(this);
        }

        public override void Initialize()
        {
            AddBackground("space background", 0.05f);
            AddBackground("stars", 0.06f);
            AddBackground("zodiac", 0.07f);
            AddBackground("nebula", 0.075f);
            AddBackground("galaxy", 0.08f);
            ship = new Ship(Game);
            ship.Position = new Vector2(30, 30);
            AddSprite(ship);
            debugRadius = new ShipCircle(Game, ship, "crushRing");
            AddSprite(debugRadius);
            crush = new ShipCircle(Game, ship, "crushRing");
            AddSprite(crush);
            skim = new ShipCircle(Game, ship, "crushRing");
            AddSprite(skim);
            crushRecovery = 0;
            skim.ResizeTo(DefaultSkimRadius, 0);
            crush.ResizeTo(DefaultCrushRadius, 0);
            base.Initialize();
        }

        public override EnemyMaker MakeRandomEnemy(bool leftToRight, int difficulty)
        {
            return (c) => 
                enemyBatch.Create(c, "wrench", 2, CollisionStyle.Circle, "wave", 
                    leftToRight, (int)(rgen.NextDouble()*(View.Height)), 14, 14, 
                    1.0);
        }

        protected override void SetupChallenges()
        {
            Challenge testChallenge = new Challenge(4);
            testChallenge.AddBeat(new ChallengeBeat(0, new EnemyMaker[] { MakeRandomEnemy(true, 0), MakeRandomEnemy(true, 0) }));
            testChallenge.AddBeat(new ChallengeBeat(2, new EnemyMaker[] { MakeRandomEnemy(true, 0), MakeRandomEnemy(true, 0) }));
            TriggerChallenge(0, testChallenge);
        }

        public override void LoadContent()
        {
            //replace this with a Challenge and an EnemyMaker. Also provide a random enemy maker.  Then implement
            //killing and parametrize circle vs box collision?
            base.LoadContent();
            debugRadius.ResizeTo(ship.Radius, 0);
        }
        public float SkimShrinkRate
        {
            get { return 1.0f; }
        }
        public float CrushGrowRate
        {
            get { return 1.0f; }
        }
        public float DefaultCrushRadius
        {
            get { return 32; }
        }
        public float DefaultSkimRadius
        {
            get { return 128; }
        }
        public float MaxCrushRadius
        {
            get { return 128; }
        }
        public float MinSkimRadius
        {
            get { return 24; }
        }
        public float SkimResizeDuration
        {
            get { return 0.25f; }
        }
        public float CrushCooldown
        {
            get { return 1.0f; }
        }
        public void KillShip()
        {
            Console.WriteLine("killed player");
        }
        public void Skim()
        {
            Console.WriteLine("skimmed");
            skim.ResizeTo(Math.Max(skim.Radius - SkimShrinkRate, MinSkimRadius), SkimResizeDuration);
            crush.ResizeTo(Math.Min(crush.Radius + CrushGrowRate, MaxCrushRadius), SkimResizeDuration);
        }
        public void Crush()
        {
            Console.WriteLine("crush");
            skim.ResizeTo(DefaultSkimRadius, CrushCooldown);
            crush.ResizeTo(DefaultCrushRadius, CrushCooldown);
            crushRecovery = CrushCooldown;
        }

        public override void Update(GameTime gameTime)
        {
            if (crushRecovery > 0)
            {
                crushRecovery -= (float)(gameTime.ElapsedGameTime.TotalSeconds);
                //ask batch to check collision
                //kill all dead things
                //kill stuff
                //--hemispherical crushing
            }
            enemyBatch.Update(gameTime);
            base.Update(gameTime);
            //check circle overlapping, ship collision
            //we'll just treat them all as circles
            List<CycEnemy> shipCollided = enemyBatch.Collide(ship);
            if (shipCollided.Count() != 0)
            {
                KillShip();
            }
            if (crushRecovery <= 0)
            {
                List<CycEnemy> skimCollided = enemyBatch.Collide(skim);
                if (skimCollided.Count() != 0)
                {
                    Skim();
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Crush();
                }
            }
        }

        protected override void SetupFilters()
        {
            GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.GaussianQuad;
            GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.GaussianQuad;
            GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.GaussianQuad;
        }

        protected override void DrawInnards(GameTime gt)
        {
            base.DrawInnards(gt);
            enemyBatch.Draw(gt);
        }
    }
}