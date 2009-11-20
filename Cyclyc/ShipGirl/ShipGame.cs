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

        public ShipGame(Game1 game)
            : base(game)
        {
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
            crush = new ShipCircle(Game, ship, "crushRing");
            AddSprite(crush);
            skim = new ShipCircle(Game, ship, "crushRing");
            AddSprite(skim);
            crushRecovery = 0;
            skim.ResizeTo(DefaultSkimRadius, 0);
            crush.ResizeTo(DefaultCrushRadius, 0);
            AddSprite(ship);
            ship.Position = StartPosition;
            debugRadius = new ShipCircle(Game, ship, "crushRing");
            AddSprite(debugRadius);
            base.Initialize();
        }

        protected Vector2 StartPosition
        {
            get
            {
                //a random free radius on the left half of the screen
                Vector2 oldPos = ship.Position;
                do {
                    ship.Position = new Vector2((float)(rgen.NextDouble() * 370)+30, (float)(rgen.NextDouble() * 100)+10);
                } while(enemyBatch.Collide(ship).Count != 0);
                Vector2 ret = ship.Position;
                ship.Position = oldPos;
                return ret;
            }
        }

        public override EnemyMaker MakeRandomEnemy(bool leftToRight, int difficulty)
        {
            return (c) => 
                enemyBatch.Create(c, "wrench", 2, CollisionStyle.Circle, "wave", 
                    leftToRight, (int)(rgen.NextDouble()*(View.Height)), 22, 22, 
                    1.0);
        }

        protected EnemyMaker MakeJerkEnemy(int y)
        {
            return (c) => 
                enemyBatch.Create(c, "spider robot space", 1, CollisionStyle.Circle, "jerk", true,
                    y, 14, 14, 1.0);
        }

        protected EnemyMaker MakeLoopEnemy(int y)
        {
            return (c) =>
                enemyBatch.Create(c, "walking robot space", 1, CollisionStyle.Circle, "loop", true,
                    y, 28, 28, 1.0);
        }

        protected EnemyMaker MakeZigzagEnemy(int y)
        {
            return (c) =>
                enemyBatch.Create(c, "walking robot space creepy", 1, CollisionStyle.Circle, "zigzag", true,
                    y, 28, 28, 1.0);
        }

        protected override void SetupChallenges()
        {
            Challenge testChallenge = new Challenge(this, Game, 0);
            testChallenge.AddBeat(new ChallengeBeat(0, new EnemyMaker[] { MakeRandomEnemy(true, 0), MakeRandomEnemy(true, 0) }));
            testChallenge.AddBeat(new ChallengeBeat(2, new EnemyMaker[] { MakeRandomEnemy(true, 0), MakeRandomEnemy(true, 0) }));
            TriggerChallenge(0, testChallenge);

            Challenge wave0_0 = new Challenge(this, Game, 2);
            wave0_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeJerkEnemy(40), MakeJerkEnemy(100), MakeJerkEnemy(250)
            }));
            TriggerChallenge(0, wave0_0);

            Challenge wave1_0 = new Challenge(this, Game, 4);
            wave1_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeLoopEnemy(50), MakeLoopEnemy(150)
            }));
            wave1_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeLoopEnemy(100)
            }));
            TriggerChallenge(0, wave1_0);

            Challenge wave2_0 = new Challenge(this, Game, 7);
            wave2_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeZigzagEnemy(20), MakeZigzagEnemy(70)
            }));
            wave2_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeZigzagEnemy(250), MakeZigzagEnemy(280)
            }));
            wave2_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeZigzagEnemy(100), MakeZigzagEnemy(140), MakeZigzagEnemy(170)
            }));
            TriggerChallenge(0, wave2_0);
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
            get { return 0.3f; }
        }
        public float CrushGrowRate
        {
            get { return 0.3f; }
        }
        public float DefaultCrushRadius
        {
            get { return 32; }
        }
        public float DefaultSkimRadius
        {
            get { return 96; }
        }
        public float MaxCrushRadius
        {
            get { return 128; }
        }
        public float MinSkimRadius
        {
            get { return 32; }
        }
        public float SkimResizeDuration
        {
            get { return 0.25f; }
        }
        public float CrushCooldown
        {
            get { return 1.0f; }
        }
        public void KillPlayer()
        {
            skim.ResizeTo(DefaultSkimRadius, CrushCooldown);
            crush.ResizeTo(DefaultCrushRadius, CrushCooldown);
            ship.Die();
        }
        public void Skim(int enemyCount)
        {
//            Console.WriteLine("skimmed");
            skim.ResizeTo(Math.Max(skim.DestRadius - SkimShrinkRate * enemyCount, MinSkimRadius), SkimResizeDuration);
            crush.ResizeTo(Math.Min(crush.DestRadius + CrushGrowRate * enemyCount, MaxCrushRadius), SkimResizeDuration);
        }
        public void Crush()
        {
//            Console.WriteLine("crush");
            skim.ResizeTo(DefaultSkimRadius, CrushCooldown);
            crush.ResizeTo(DefaultCrushRadius, CrushCooldown);
            crushRecovery = CrushCooldown;
        }

        public override void Update(GameTime gameTime)
        {
            if (!ship.Dying)
            {
                if (crushRecovery > 0)
                {
                    List<CycEnemy> crushCollided = enemyBatch.Collide(crush);
                    foreach (CycEnemy crushed in crushCollided)
                    {
                        crushed.Die();
                        bool leftOfCenter = (ship.Position.X >= crushed.Position.X);
                        NextGame.DeliverRandomEnemy(leftOfCenter, 0);
                    }
                    crushRecovery -= (float)(gameTime.ElapsedGameTime.TotalSeconds);
                }
            }
            enemyBatch.Update(gameTime);
            base.Update(gameTime);
            if (!ship.Dying)
            {
                List<CycEnemy> shipCollided = enemyBatch.Collide(ship);
                if (shipCollided.Count() != 0)
                {
                    KillPlayer();
                }
                if (crushRecovery <= 0)
                {
                    List<CycEnemy> skimCollided = enemyBatch.Collide(skim);
                    if (skimCollided.Count() != 0)
                    {
                        Skim(skimCollided.Count());
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        Crush();
                    }
                }
            }
            else
            {
                ship.StartPosition = StartPosition;
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