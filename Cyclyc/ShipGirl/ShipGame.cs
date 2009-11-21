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
        ShipEnemyPool enemyBatch;
        protected float crushRecovery;

        public ShipGame(Game1 game)
            : base(game)
        {
            SongName = "ship";
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
            ship.Position = new Vector2(30, 30);
            crush = new ShipCircle(Game, ship, "crushRing");
            AddSprite(crush);
            skim = new ShipCircle(Game, ship, "skimRing");
            AddSprite(skim);
            crushRecovery = 0;
            skim.ResizeTo(DefaultSkimRadius, 0);
            crush.ResizeTo(DefaultCrushRadius, 0);
            AddSprite(ship);
            ship.Position = StartPosition;
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
                    ship.Position = new Vector2(800 - (float)(rgen.NextDouble() * 370)+30, (float)(rgen.NextDouble() * 100)+10);
                } while(enemyBatch.Collide(ship).Count != 0);
                Vector2 ret = ship.Position;
                ship.Position = oldPos;
                return ret;
            }
        }

        protected override void CoalesceChallengeBeats(Challenge c)
        {
            base.CoalesceChallengeBeats(c);
        }

        //MAKE AN ENEMY. USED WHEN AN ENEMY IS SENT FROM OTHER PLAYER
        public override EnemyMaker MakeEnemy(bool leftToRight, int difficulty)
        {
            return (c) => {
                double r = rgen.NextDouble();
                //make a different monster based on difficulty
                if(r < 0.2)
                {

                }
                else if (r < 0.4)
                {

                }
                else if(r < 0)
                enemyBatch.Create(c, "wrench", 2, CollisionStyle.Circle, "wave", 
                    leftToRight, 0, (int)(rgen.NextDouble()*(View.Height)), 22, 22, 
                    1.0, difficulty);
            }
        }




        #region enemyTypes
        //--------------------------------------------------------------------------------
        //   DEFINE SPECIFIC ENEMY TYPES
        //--------------------------------------------------------------------------------

        protected EnemyMaker MakeJerkEnemy(int x, int y)
        {
            return (c) => 
                enemyBatch.Create(c, "spider robot space creepy", 1, CollisionStyle.Circle, "jerk", true,
                    x, y, 14, 14, 1.0, 1);
        }

        protected EnemyMaker MakeLoopEnemy(int x, int y)
        {
            return (c) =>
                enemyBatch.Create(c, "walking robot space creepy", 1, CollisionStyle.Circle, "loop", true,
                    x, y, 28, 28, 1.0, 1);
        }

        protected EnemyMaker MakeZigzagEnemy(int x, int y)
        {
            return (c) =>
                enemyBatch.Create(c, "walking robot space creepy", 1, CollisionStyle.Circle, "zigzag", true,
                    x, y, 28, 28, 1.0, 1);
        }

        protected EnemyMaker MakeEssEnemy(int x, int y)
        {
            return (c) =>
                enemyBatch.Create(c, "walking robot space creepy", 1, CollisionStyle.Circle, "ess", true,
                    x, y, 28, 28, 1.0, 1);
        }

        protected EnemyMaker MakeWaveEnemy(int x, int y)
        {
            return (c) =>
                enemyBatch.Create(c, "spider robot space creepy", 1, CollisionStyle.Circle, "wave", true,
                    x, y, 14, 14, 1.0, 1);
        }
        //--------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------
        #endregion

        #region enemyGroups
        //--------------------------------------------------------------------------------
        //   DEFINE ENEMY GROUPS
        //--------------------------------------------------------------------------------

        protected ChallengeBeat MakeEssString(int y)
        {
            EnemyMaker[] makers = new EnemyMaker[] { MakeEssEnemy(0, y), MakeEssEnemy(16, y) };
            EnemyMaker[] moreMakers = new EnemyMaker[] { MakeEssEnemy(32, y) };
            makers.Concat(moreMakers);
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
            Challenge wave0_0 = new Challenge(this, Game, 4);
            wave0_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeJerkEnemy(0, 40), MakeJerkEnemy(0, 100), MakeJerkEnemy(0, 250)
            }));
            TriggerChallenge(0, wave0_0);

            Challenge wave1_0 = new Challenge(this, Game, 8);
            wave1_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeLoopEnemy(0, 50), MakeLoopEnemy(0, 150)
            }));
            wave1_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeLoopEnemy(0, 100)
            }));
            TriggerChallenge(0, wave1_0);

            Challenge wave2_0 = new Challenge(this, Game, 12);
            wave2_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeZigzagEnemy(0, 20), MakeZigzagEnemy(0, 70)
            }));
<<<<<<< HEAD
            wave2_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeZigzagEnemy(250), MakeZigzagEnemy(280)
            }));
            wave2_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeZigzagEnemy(100), MakeZigzagEnemy(140), MakeZigzagEnemy(170)
=======
            wave2_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeZigzagEnemy(0, 250), MakeZigzagEnemy(0, 280)
            }));
            wave2_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeZigzagEnemy(0, 100), MakeZigzagEnemy(0, 140), MakeZigzagEnemy(0, 170)
>>>>>>> 4b1bd71393dd4804c690b2a0608d5e418fb65ddc
            }));
            TriggerChallenge(0, wave2_0);

            Challenge wave3_0 = new Challenge(this, Game, 16);
            wave3_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeEssEnemy(100)
            }));
            wave3_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeEssEnemy(100)
            }));
            wave3_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeEssEnemy(100)
            }));
            wave3_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeEssEnemy(100)
            }));
            wave3_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeEssEnemy(100)
            }));
            wave3_0.AddBeat(new ChallengeBeat(10, new EnemyMaker[] {
                MakeEssEnemy(100)
            }));
            TriggerChallenge(0, wave3_0);

            Challenge wave5_0 = new Challenge(this, Game, 20);
            wave5_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeWaveEnemy(40)
            }));
            TriggerChallenge(0, wave5_0);
        }

        //--------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------
        #endregion

        public override void LoadContent()
        {
            //replace this with a Challenge and an EnemyMaker. Also provide a random enemy maker.  Then implement
            //killing and parametrize circle vs box collision?
            base.LoadContent();
//            debugRadius.ResizeTo(ship.Radius, 0);
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
            get { return 16; }
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
            skim.ResizeTo(DefaultSkimRadius, 0.0);
            crush.ResizeTo(DefaultCrushRadius, 0.0);
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
                        bool leftOfCenter = (ship.Position.X >= crushed.Position.X);
                        NextGame.DeliverEnemy(leftOfCenter, crushed.Difficulty);
                        crushed.Die();
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