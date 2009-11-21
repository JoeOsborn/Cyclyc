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
        ShipCircle skim;
        ShipEnemyPool enemyBatch;
        protected float crushRecovery;
        public BeamPool CrushBeams { get; set; }

        public ShipGame(Game1 game)
            : base(game)
        {
            SongName = "ship";
            CrushBeams = new BeamPool(this);
            enemyBatch = new ShipEnemyPool(this);
        }

        public override void Initialize()
        {
            AddBackground("space background", -0.05f);
            AddBackground("galaxy", -0.1f);
            AddBackground("nebula", -0.1f);
            AddBackground("zodiac", -0.2f);
            AddBackground("stars", -0.3f);

            ship = new Ship(Game);
            ship.CrushPool = CrushBeams;
            skim = new ShipCircle(Game, ship, "skimRing");
            AddSprite(skim);
            crushRecovery = 0;
            skim.ResizeTo(DefaultSkimRadius, 0);
            AddSprite(ship);
//            debugRadius = new ShipCircle(Game, ship, "crushRing");
//            AddSprite(debugRadius);
            base.Initialize();
        }

        protected Vector2 StartPosition
        {
            get
            {
                //a random free radius on the left half of the screen
                Vector2 oldPos = ship.Position;
                do {
                    ship.Position = new Vector2(View.Width - (float)(rgen.NextDouble() * View.Width/4.0)-30, (float)(rgen.NextDouble() * 100)+10);
                } while(enemyBatch.Collide(ship).Count != 0);
                Vector2 ret = ship.Position;
                ship.Position = oldPos;
                return ret;
            }
        }

        protected override ChallengeBeat[] CoalesceChallengeBeatEnemies(bool left, int difficulty)
        {
            //ship game needs to double the number of enemies
            EnemyMaker[] es = new EnemyMaker[difficulty * 2];
            for (int i = 0; i < difficulty*2; i+=2)
            {
                es[i] = MakeEnemy(left, 1);
                es[i+1] = MakeEnemy(left, 1);
                if (left) { ConsumeLeft(1); }
                else { ConsumeRight(1); }
            }
            return new ChallengeBeat[] { new ChallengeBeat(0, es) };
        }



        //MAKE AN ENEMY. USED WHEN AN ENEMY IS SENT FROM OTHER PLAYER
        public override EnemyMaker MakeEnemy(bool leftToRight, int difficulty)
        {
            return (c) =>
            {
                double r = rgen.NextDouble();
                //make a different monster based on difficulty
                if (r < 0.2)
                {
                    return enemyBatch.Create(c, "spider robot space creepy", 1, CollisionStyle.Circle, "jerk", leftToRight, 
                        (int)(rgen.NextDouble() * ((View.Height) - 21)), 21, 21, 1.0, difficulty);
                }
                else if (r < 0.4)
                {
                    return enemyBatch.Create(c, "walking robot space creepy", 1, CollisionStyle.Circle, "loop", leftToRight, 
                        (int)((25 + rgen.NextDouble() * ((View.Height) - 53))), 28, 28, 1.0, difficulty);
                }
                else if (r < 0.6)
                {
                    return enemyBatch.Create(c, "hover space", 1, CollisionStyle.Circle, "static", leftToRight,
                        (int)(rgen.NextDouble() * ((View.Height)-28)), 28, 28, 1.0, difficulty);
                }
                else if (r < 0.8)
                {
                    return enemyBatch.Create(c, "spider robot space creepy", 1, CollisionStyle.Circle, "ess", leftToRight,
                        (int)(30 + rgen.NextDouble() * ((View.Height) - 44)), 14, 14, 1.0, difficulty);
                }
                else
                {
                    return enemyBatch.Create(c, "walking robot space creepy", 1, CollisionStyle.Circle, "pong", leftToRight,
                        0, 21, 21, 1.0, difficulty);
                }
            };
        }


        #region enemyTypes
        //--------------------------------------------------------------------------------
        //   DEFINE SPECIFIC ENEMY TYPES
        //--------------------------------------------------------------------------------

        protected EnemyMaker MakeJerkEnemy(int y)
        {
            return (c) =>
                enemyBatch.Create(c, "spider robot space creepy", 1, CollisionStyle.Circle, "jerk", true,
                    y, 21, 21, 1.0, 1);
        }

        protected EnemyMaker MakeLoopEnemy(int y)
        {
            return (c) =>
                enemyBatch.Create(c, "walking robot space creepy", 1, CollisionStyle.Circle, "loop", true,
                    y, 28, 28, 1.0, 1);
        }

        protected EnemyMaker MakeStaticEnemy(int y)
        {
            return (c) =>
                enemyBatch.Create(c, "hover space", 1, CollisionStyle.Circle, "static", true,
                    y, 28, 28, 1.0, 1);
        }

        protected EnemyMaker MakeEssEnemy(int y)
        {
            return (c) =>
                enemyBatch.Create(c, "spider robot space creepy", 1, CollisionStyle.Circle, "ess", true,
                    y, 14, 14, 1.0, 1);
        }

        protected EnemyMaker MakePongEnemy(int y)
        {
            return (c) =>
                enemyBatch.Create(c, "frog space", 1, CollisionStyle.Circle, "pong", true,
                    y, 21, 21, 1.0, 1);
        }
        #endregion

        #region enemyGroups
        //--------------------------------------------------------------------------------
        //   DEFINE ENEMY GROUPS
        //--------------------------------------------------------------------------------

        //protected ChallengeBeat MakeEssString(int y)
        //{
        //    EnemyMaker[] makers = new EnemyMaker[] { MakeEssEnemy(0, y), MakeEssEnemy(16, y) };
        //    EnemyMaker[] moreMakers = new EnemyMaker[] { MakeEssEnemy(32, y) };
        //    makers.Concat(moreMakers);
        //}

        protected void AddEssBeats(Challenge c, int y, int length)
        {
            int templength = length;

            while (length > 0)
            {
                c.AddBeat(new ChallengeBeat((templength - length), new EnemyMaker[] {
                MakeEssEnemy(y)
                }));
                length--;
            }
        }

        protected void AddStaticBlock(Challenge c, int y, int w, int h)
        {
            int tempy = y;
            int temph = h;
            int tempw = w;

            while (w > 0)
            {
                while (h > 0)
                {
                    c.AddBeat(new ChallengeBeat(((2 * tempw) - (2 * w)), new EnemyMaker[] {
                    MakeStaticEnemy(y)
                    }));
                    y += 30;
                    h--;
                }
                y = tempy;
                h = temph;
                w--;
            }
        }

        protected void AddStaticWall(Challenge c, int y, int h, int gap)
        {
            int tempy = y;
            int temph = h;

            while (h > 0)
            {
                if (h != gap)
                {
                    c.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                    MakeStaticEnemy(y)
                    }));
                    y += 30;
                    h--;
                }
                else
                {
                    y += 40;
                    h--;
                }
            }
        }

        //--------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------
        #endregion





        #region levelDesign
        //--------------------------------------------------------------------------------
        //   CODE FOR WHAT THE GAME ACTUALLY SENDS TO THE PLAYER
        //--------------------------------------------------------------------------------

        protected override void SetupChallenges()
        {

            #region Demo Level
            Challenge wave0_0 = new Challenge(this, Game, 4);
            wave0_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeJerkEnemy(40), MakeJerkEnemy(100), MakeJerkEnemy(250)
            }));
            TriggerChallenge(0, wave0_0);

            Challenge wave1_0 = new Challenge(this, Game, 8);
            wave1_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeLoopEnemy(50), MakeLoopEnemy(150)
            }));
            wave1_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeLoopEnemy(100)
            }));
            TriggerChallenge(0, wave1_0);

            Challenge wave2_0 = new Challenge(this, Game, 12);
            AddStaticBlock(wave2_0, 20, 1, 9);
            TriggerChallenge(0, wave2_0);

            Challenge wave3_0 = new Challenge(this, Game, 16);
            AddEssBeats(wave3_0, 100, 12);
            TriggerChallenge(0, wave3_0);

            Challenge wave4_0 = new Challenge(this, Game, 20);
            wave4_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakePongEnemy(0)
            }));
            TriggerChallenge(0, wave4_0);

            Challenge wave5_0 = new Challenge(this, Game, 30);
            AddStaticWall(wave5_0, 20, 9, 3); ;
            TriggerChallenge(0, wave5_0);
            Challenge wave6_0 = new Challenge(this, Game, 31);
            AddStaticWall(wave6_0, 20, 9, 4); ;
            TriggerChallenge(0, wave6_0);
            Challenge wave7_0 = new Challenge(this, Game, 32);
            AddStaticWall(wave7_0, 20, 9, 5); ;
            TriggerChallenge(0, wave7_0);
            Challenge wave8_0 = new Challenge(this, Game, 33);
            AddStaticWall(wave8_0, 20, 9, 6); ;
            TriggerChallenge(0, wave8_0);
            Challenge wave9_0 = new Challenge(this, Game, 34);
            AddStaticWall(wave9_0, 20, 9, 6); ;
            TriggerChallenge(0, wave9_0);

            #endregion


        }

        //--------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------
        #endregion

        public override void LoadContent()
        {
            ship.StartPosition = StartPosition;
            ship.Position = StartPosition;
            base.LoadContent();
//            debugRadius.ResizeTo(ship.Radius, 0);
        }
        public float SkimShrinkRate
        {
            get { return 0.15f; }
        }
        public float DefaultSkimRadius
        {
            get { return 64; }
        }
        public float MinSkimRadius
        {
            get { return 32; }
        }
        public float SkimResizeDuration
        {
            get { return 0.25f; }
        }
        public void KillPlayer()
        {
            ship.Die();
        }
        public void Skim(int enemyCount)
        {
//            Console.WriteLine("skimmed");
            ship.Skim(enemyCount);
        }
        public override void Update(GameTime gameTime)
        {
            CrushBeams.Update(gameTime);
            List<CollisionGroup> crushCollided = enemyBatch.CollidePool(CrushBeams);
            foreach (CollisionGroup crushed in crushCollided)
            {
                CycEnemy deadEnemy = (CycEnemy)crushed.collider;
                if (!deadEnemy.Alive) { continue; }
                bool leftOfCenter = (ship.Position.X >= deadEnemy.Position.X);
                NextGame.DeliverEnemy(leftOfCenter, deadEnemy.Difficulty);
                deadEnemy.Die();
                foreach (BeamBit beam in crushed.collided)
                {
                    beam.Die();
                }
            }
            enemyBatch.Update(gameTime);
            base.Update(gameTime);
            if (!ship.Dying)
            {
                List<CycSprite> shipCollided = enemyBatch.Collide(ship);
                if (shipCollided.Count() != 0)
                {
                    KillPlayer();
                }
                List<CycSprite> skimCollided = enemyBatch.Collide(skim);
                if (skimCollided.Count() != 0)
                {
                    Skim(skimCollided.Count());
                }
            }
            else
            {
                ship.StartPosition = StartPosition;
            }
            skim.ResizeTo(MinSkimRadius + (DefaultSkimRadius-MinSkimRadius) * (1-ship.PowerRatio), SkimResizeDuration);
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
            CrushBeams.Draw(gt);
        }
    }
}