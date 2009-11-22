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
        SkimPS skimParticles;
        ShipCircle skim;
        ShipEnemyPool enemyBatch;
        protected float crushRecovery;
        protected CycSprite earth;

        protected CycBackground whiteZodiac;
        protected CycBackground superZodiac;

        public BeamPool CrushBeams { get; set; }

        public ShipGame(Game1 game)
            : base(game)
        {
            Grade1Expectation = 15;
            Grade2Expectation = 30;


            SongName = "ship";
            CrushBeams = new BeamPool(this);
            enemyBatch = new ShipEnemyPool(this);
        }

        protected float[] ParallaxSpeeds = new float[] { -0.05f, -0.05f, -0.05f, -0.1f, -0.1f, -0.3f };

        protected int MID = 150;

        public override void Initialize()
        {
            AddBackground("space background", ParallaxSpeeds[0]);
            whiteZodiac = AddBackground("zodiac", ParallaxSpeeds[1]);
            superZodiac = AddBackground("zodiacSuper", ParallaxSpeeds[2]);
            superZodiac.TargetAlpha = 0.0f;
            superZodiac.BlendDuration = 0.0f;
            AddBackground("galaxy", ParallaxSpeeds[3]);
            AddBackground("nebula", ParallaxSpeeds[4]);
            AddBackground("stars", ParallaxSpeeds[5]);


            ship = new Ship(Game, this);
            ship.CrushPool = CrushBeams;
            skim = new ShipCircle(Game, ship, "skimRing");
            AddSprite(skim);
            crushRecovery = 0;
            skim.ResizeTo(DefaultSkimRadius, 0);
            AddSprite(ship);
            skimParticles = new SkimPS(Game);
            Game.Components.Add(skimParticles);
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

        protected void AddEssBeats(Challenge c, int y, int length, int beat)
        {
            int templength = length;

            while (length > 0)
            {
                c.AddBeat(new ChallengeBeat(beat+(templength - length), new EnemyMaker[] {
                MakeEssEnemy(y)
                }));
                length--;
            }
        }

        protected void AddStaticBlock(Challenge c, int y, int w, int h, int beat)
        {
            int tempy = y;
            int temph = h;
            int tempw = w;

            while (w > 0)
            {
                while (h > 0)
                {
                    c.AddBeat(new ChallengeBeat(beat+((2 * tempw) - (2 * w)), new EnemyMaker[] {
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

        protected void AddStaticWall(Challenge c, int y, int h, int gap, int beat)
        {
            int tempy = y;
            int temph = h;

            while (h > 0)
            {
                if (h != gap)
                {
                    c.AddBeat(new ChallengeBeat(beat, new EnemyMaker[] {
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

        protected void AddSpread(Challenge c, int number, int interval, int gap, int beat)
        {
            int tempy = 0;
            int temph = 0;

            if (number % 2 == 1)
            {
                tempy = MID - (((number - 1) / 2)*interval);
            }
            else
            {
                tempy = (MID + interval / 2) - (interval*number/2);
            }
            
            while (temph < number)
            {
                if (temph != gap)
                {
                    c.AddBeat(new ChallengeBeat(beat, new EnemyMaker[] {
                        MakeStaticEnemy(tempy)
                    }));
                    tempy += interval;
                    temph++;
                }
                else
                {
                    tempy += interval;
                    temph ++;
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
            
            const int INT5 = 68;
            const int MID = INT5*2;

            #region Demo Level
            /*
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
            */
            #endregion



            #region Intro


            Challenge wave1_0 = new Challenge(this, Game, 3);
            AddSpread(wave1_0, 1, 0, 2, 0);
            AddSpread(wave1_0, 1, INT5/2, 3, 4);
            AddSpread(wave1_0, 1, INT5, 3, 8);
            AddSpread(wave1_0, 1, 0, 3, 12);
            TriggerChallenge(0, wave1_0);

            Challenge wave2_0 = new Challenge(this, Game, 9);
            AddSpread(wave2_0, 1, 0, 2, 2);
            AddSpread(wave2_0, 2, INT5, 3, 4);
            AddSpread(wave2_0, 2, INT5*2, 3, 6);
            AddSpread(wave2_0, 1, 0, 3, 8);
            TriggerChallenge(0, wave2_0);

            Challenge wave3_0 = new Challenge(this, Game, 13);
            wave3_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeStaticEnemy(MID)
            }));
            wave3_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeJerkEnemy(MID+INT5), MakeJerkEnemy(MID-INT5)
            }));
             
            TriggerChallenge(0, wave3_0);
            
            
            Challenge wave4_0 = new Challenge(this, Game, 19);
            AddSpread(wave4_0, 3, INT5, 9, 0); 
            wave4_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakePongEnemy(0), MakeStaticEnemy(MID+INT5/2), MakeStaticEnemy(MID-INT5/2)
            }));
            AddSpread(wave4_0, 3, INT5, 9, 8); 
            //AddEssBeats(wave4_0, MID + 100, 8, 12);
            AddSpread(wave4_0, 2, INT5, 9, 12); 
            //AddEssBeats(wave4_0, MID - 100, 8, 12);
            TriggerChallenge(0, wave4_0);

            Challenge wave5_0 = new Challenge(this, Game, 25);
            wave5_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeLoopEnemy(MID)
            }));
            wave5_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeLoopEnemy(MID+80), MakeLoopEnemy(MID-80)
            }));
            TriggerChallenge(0, wave5_0);
            
            Challenge wave6_0 = new Challenge(this, Game, 29); 
            wave6_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeStaticEnemy(MID-90), MakeStaticEnemy(MID+90)
            }));
            wave6_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeStaticEnemy(MID-30), MakeStaticEnemy(MID+30)
            }));
            wave6_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeStaticEnemy(MID+30), MakeStaticEnemy(MID-30)
            }));
            wave6_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeStaticEnemy(MID-90), MakeStaticEnemy(MID+90)
            }));
            AddSpread(wave6_0, 3, 60, 4, 16);
            AddSpread(wave6_0, 4, 60, 5, 24);
            AddEssBeats(wave6_0, MID - 90, 5, 32);
            AddEssBeats(wave6_0, MID + 90, 5, 32);
            TriggerChallenge(0, wave6_0);
            
            #endregion

            #region verse1

            Challenge wave7_0 = new Challenge(this, Game, 34);
            wave7_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakePongEnemy(0), MakeStaticEnemy(MID-30), MakeStaticEnemy(MID+30)
            }));
            wave7_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeStaticEnemy(MID-60), MakeStaticEnemy(MID+60)
            }));
            TriggerChallenge(0, wave7_0);

            Challenge wave8_0 = new Challenge(this, Game, 38);
            AddSpread(wave8_0, 3, 90, 4, 0);
            AddSpread(wave8_0, 3, 60, 4, 4);
            AddSpread(wave8_0, 3, 30, 4, 8);
            AddSpread(wave8_0, 1, 60, 4, 12);
            //AddEssBeats(wave6_0, MID - 90, 5, 32);
            TriggerChallenge(0, wave8_0);

            Challenge wave9_0 = new Challenge(this, Game, 42);
            wave9_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeJerkEnemy(MID-60), MakeJerkEnemy(MID+60)
            }));
            AddEssBeats(wave9_0, MID, 5, 8);
            TriggerChallenge(0, wave9_0);

            Challenge wave10_0 = new Challenge(this, Game, 46);
            AddStaticBlock(wave10_0, 60, 5, 5, 0);
            wave10_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakePongEnemy(0)
            }));
            TriggerChallenge(0, wave10_0);

            #endregion

            #region verse2

            Challenge wave11_0 = new Challenge(this, Game, 50);
            wave11_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeLoopEnemy(MID+60), MakeLoopEnemy(MID-60)
            }));
            AddSpread(wave11_0, 6, 30, 7, 8);
            wave11_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeLoopEnemy(MID+60), MakeLoopEnemy(MID-60)
            }));
            TriggerChallenge(0, wave11_0);

            Challenge wave12_0 = new Challenge(this, Game, 54);
            wave12_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeStaticEnemy(MID-90), MakeJerkEnemy(MID+90)
            }));
            wave12_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeStaticEnemy(MID-30), MakeJerkEnemy(MID+30)
            }));
            wave12_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeStaticEnemy(MID+30), MakeJerkEnemy(MID-30)
            }));
            wave12_0.AddBeat(new ChallengeBeat(12, new EnemyMaker[] {
                MakeJerkEnemy(MID-90), MakeStaticEnemy(MID+90)
            }));
            TriggerChallenge(0, wave12_0);

            Challenge wave13_0 = new Challenge(this, Game, 58);
            wave13_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeStaticEnemy(MID-105), MakePongEnemy(0)
            }));
            wave13_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeStaticEnemy(MID-75)
            }));
            wave13_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeStaticEnemy(MID-45), MakeStaticEnemy(MID+60)
            }));
            wave13_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeStaticEnemy(MID-15)
            }));
            wave13_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeStaticEnemy(MID+15)
            }));
            wave13_0.AddBeat(new ChallengeBeat(10, new EnemyMaker[] {
                MakeStaticEnemy(MID+45), MakeStaticEnemy(MID-60)
            }));
            wave13_0.AddBeat(new ChallengeBeat(12, new EnemyMaker[] {
                MakeStaticEnemy(MID+75)
            }));
            wave13_0.AddBeat(new ChallengeBeat(14, new EnemyMaker[] {
                MakeStaticEnemy(MID+105)
            }));
            TriggerChallenge(0, wave13_0);

            Challenge wave14_0 = new Challenge(this, Game, 62);
            AddEssBeats(wave14_0, MID - 60, 5, 0);
            AddEssBeats(wave14_0, MID + 60, 5, 0);
            AddEssBeats(wave14_0, MID, 5, 4);
            AddEssBeats(wave14_0, MID - 90, 5, 8);
            AddEssBeats(wave14_0, MID + 90, 5, 8);
            AddEssBeats(wave14_0, MID, 5, 12);
            
            TriggerChallenge(0, wave14_0);

            #endregion

            #region bridge

            Challenge wave15_0 = new Challenge(this, Game, 66);
            AddStaticWall(wave15_0, 15, 9, 0, 0);
            AddStaticWall(wave15_0, 15, 9, 1, 4);
            AddStaticWall(wave15_0, 15, 9, 2, 8);
            AddStaticWall(wave15_0, 15, 9, 3, 12);
            TriggerChallenge(0, wave15_0);

            Challenge wave16_0 = new Challenge(this, Game, 70);
            AddStaticWall(wave16_0, 15, 9, 4, 0);
            AddStaticWall(wave16_0, 15, 9, 5, 4);
            AddStaticWall(wave16_0, 15, 9, 4, 8);
            AddStaticWall(wave16_0, 15, 9, 5, 9);
            AddStaticWall(wave16_0, 15, 9, 6, 10);
            AddStaticWall(wave16_0, 15, 9, 7, 11);
            TriggerChallenge(0, wave16_0);

            Challenge wave17_0 = new Challenge(this, Game, 73);
            AddSpread(wave17_0, 5, 45, 2, 0);
            wave17_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeJerkEnemy(MID-15)
            }));
            AddSpread(wave17_0, 5, 60, 2, 2);
            wave17_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeJerkEnemy(MID+15)
            }));
            AddSpread(wave17_0, 5, 45, 2, 4);
            AddSpread(wave17_0, 5, 60, 2, 6);
            
            wave17_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeLoopEnemy(MID+60), MakeLoopEnemy(MID-60), MakeJerkEnemy(MID+60), MakeJerkEnemy(MID-60), MakeStaticEnemy(15), MakeStaticEnemy(285)
            }));
            wave17_0.AddBeat(new ChallengeBeat(10, new EnemyMaker[] {
                MakeLoopEnemy(MID+45), MakeLoopEnemy(MID-45)
            }));
            wave17_0.AddBeat(new ChallengeBeat(12, new EnemyMaker[] {
                MakeLoopEnemy(MID+60), MakeLoopEnemy(MID-60), MakeJerkEnemy(MID), MakeStaticEnemy(15), MakeStaticEnemy(285)
            }));
            wave17_0.AddBeat(new ChallengeBeat(14, new EnemyMaker[] {
                MakeLoopEnemy(MID+45), MakeLoopEnemy(MID-45)
            }));
            TriggerChallenge(0, wave17_0);

            Challenge wave18_0 = new Challenge(this, Game, 77);
            AddSpread(wave18_0, 1, 0, 9, 0);
            wave18_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeLoopEnemy(MID-100), MakeLoopEnemy(MID+100)
            }));
            AddSpread(wave18_0, 2, 30, 9, 1);
            AddSpread(wave18_0, 2, 60, 9, 2);
            AddSpread(wave18_0, 2, 90, 9, 3);
            wave18_0.AddBeat(new ChallengeBeat(3, new EnemyMaker[] {
                MakeLoopEnemy(MID-100), MakeLoopEnemy(MID+100), MakeJerkEnemy(MID), MakeJerkEnemy(MID+15), MakeJerkEnemy(MID-15)
            }));
            AddSpread(wave18_0, 2, 60, 9, 4);
            AddSpread(wave18_0, 2, 30, 9, 5);
            AddSpread(wave18_0, 1, 0, 9, 6);
            AddStaticBlock(wave18_0, 15, 3, 10, 7);
            AddEssBeats(wave18_0, 30, 8, 8);
            AddEssBeats(wave18_0, 270, 8, 8);
            AddEssBeats(wave18_0, MID, 8, 8);
            TriggerChallenge(0, wave18_0);

            #endregion

            #region verse3

            Challenge wave19_0 = new Challenge(this, Game, 81);
            wave19_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeLoopEnemy(MID-15), MakeLoopEnemy(MID+75), MakeLoopEnemy(MID-105), MakePongEnemy(0)
            }));
            wave19_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeLoopEnemy(MID+15), MakeLoopEnemy(MID+105), MakeLoopEnemy(MID-75), MakePongEnemy(0)
            }));
            wave19_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeJerkEnemy(MID+15), MakeJerkEnemy(MID+105), MakeJerkEnemy(MID-75), MakePongEnemy(0)
            })); 
            wave19_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeJerkEnemy(MID+15), MakeJerkEnemy(MID+105), MakeJerkEnemy(MID-75), MakePongEnemy(0)
            }));
            AddStaticWall(wave19_0, 15, 10, 10, 8);
            AddSpread(wave19_0, 9, 30, 4, 9);
            AddStaticWall(wave19_0, 15, 10, 10, 10);
            AddSpread(wave19_0, 9, 30, 4, 11);
            TriggerChallenge(0, wave19_0);

            Challenge wave20_0 = new Challenge(this, Game, 85);
            wave20_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeLoopEnemy(MID), MakeLoopEnemy(MID+90), MakeLoopEnemy(MID-90)
            }));
            AddStaticWall(wave20_0, 15, 10, 3, 0);
            AddStaticWall(wave20_0, 15, 10, 4, 1);
            AddStaticWall(wave20_0, 15, 10, 5, 2);
            AddStaticWall(wave20_0, 15, 10, 6, 3);
            wave20_0.AddBeat(new ChallengeBeat(3, new EnemyMaker[] {
                MakePongEnemy(0)
            }));
            AddStaticWall(wave20_0, 15, 10, 7, 4);
            wave20_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeJerkEnemy(30), MakeJerkEnemy(60), MakeJerkEnemy(90), MakeJerkEnemy(120), MakeJerkEnemy(150), MakeJerkEnemy(180), MakeJerkEnemy(210), MakeJerkEnemy(240), MakeJerkEnemy(270)
            }));

            wave20_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakePongEnemy(0)
            }));
            AddStaticWall(wave20_0, 15, 10, 6, 5);
            wave20_0.AddBeat(new ChallengeBeat(5, new EnemyMaker[] {
                MakePongEnemy(0)
            }));
            AddStaticWall(wave20_0, 15, 10, 5, 6);
            wave20_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakePongEnemy(0)
            }));
            AddStaticWall(wave20_0, 15, 10, 4, 7);
            wave20_0.AddBeat(new ChallengeBeat(7, new EnemyMaker[] {
                MakePongEnemy(0)
            }));
            AddStaticWall(wave20_0, 15, 10, 3, 8);
            wave20_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakePongEnemy(0)
            }));
            AddStaticWall(wave20_0, 15, 10, 4, 9);
            wave20_0.AddBeat(new ChallengeBeat(9, new EnemyMaker[] {
                MakePongEnemy(0)
            }));
            AddStaticWall(wave20_0, 15, 10, 5, 10);
            wave20_0.AddBeat(new ChallengeBeat(10, new EnemyMaker[] {
                MakePongEnemy(0)
            }));
            AddStaticWall(wave20_0, 15, 10, 6, 11);
            wave20_0.AddBeat(new ChallengeBeat(11, new EnemyMaker[] {
                MakePongEnemy(0)
            }));

            wave20_0.AddBeat(new ChallengeBeat(12, new EnemyMaker[] {
                MakeJerkEnemy(30), MakeJerkEnemy(60), MakeJerkEnemy(90), MakeJerkEnemy(120), MakeJerkEnemy(150), MakeJerkEnemy(180), MakeJerkEnemy(210), MakeJerkEnemy(240), MakeJerkEnemy(270)
            }));
            //AddStaticWall(wave20_0, 15, 10, 7, 12);
            AddStaticWall(wave20_0, 15, 10, 8, 13);

            AddEssBeats(wave20_0, 15, 16, 16);
            AddEssBeats(wave20_0, 45, 16, 16);
            AddEssBeats(wave20_0, 75, 16, 16);
            AddEssBeats(wave20_0, 105, 16, 16);
            AddEssBeats(wave20_0, 135, 16, 16);
            AddEssBeats(wave20_0, 165, 16, 16);
            AddEssBeats(wave20_0, 195, 16, 16);
            AddEssBeats(wave20_0, 225, 16, 16);
            AddEssBeats(wave20_0, 255, 16, 16);
            AddEssBeats(wave20_0, 285, 16, 16);

            TriggerChallenge(0, wave20_0);

            #endregion

        }

        //--------------------------------------------------------------------------------
        #endregion

        protected float EarthSpeed
        {
            get { return 0.6f; }
        }

        public override void LoadContent()
        {
            ship.StartPosition = StartPosition;
            ship.Position = StartPosition;
            base.LoadContent();
            ship.ForceFeedback = ff;
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
            Combo = 0;
            ship.Die();
        }
        public void Skim(int enemyCount)
        {
//            Console.WriteLine("skimmed");
            ship.Skim(enemyCount);
        }
        protected override void CalculateGrade()
        {
            base.CalculateGrade();
            if (Grade >= 2)
            {
                whiteZodiac.TargetAlpha = 0.0f;
                whiteZodiac.BlendDuration = 1.0f;
                superZodiac.TargetAlpha = 1.0f;
                superZodiac.BlendDuration = 1.0f;
            }
            else
            {
                whiteZodiac.TargetAlpha = 1.0f;
                whiteZodiac.BlendDuration = 1.0f;
                superZodiac.TargetAlpha = 0.0f;
                superZodiac.BlendDuration = 1.0f;
            }
            ship.Superize(Grade >= 2);
        }
        public override void Update(GameTime gameTime)
        {
            if (Game.SongIsEnding)
            {
                float r = (float)Game.OutroRatio;
                for (int i = 0; i < backgrounds.Count; i++)
                {
                    CycBackground bg = backgrounds[i];
                    bg.ScrollSpeed = ParallaxSpeeds[i] * (1 - r);
                }
                if (earth == null)
                {
                    earth = new EndingSprite(Game, "earth", new Vector2(-400, 90), 301);
                    earth.Initialize();
                    earth.LoadContent();
                    AddSprite(earth);
                }
                earth.Velocity = new Vector2(EarthSpeed * (1 - r), earth.Velocity.Y);
            }
            CrushBeams.Update(gameTime);
            List<CollisionGroup> crushCollided = enemyBatch.CollidePool(CrushBeams);
            foreach (CollisionGroup crushed in crushCollided)
            {
                CycEnemy deadEnemy = (CycEnemy)crushed.collider;
                if (!deadEnemy.Alive) { continue; }
                bool leftOfCenter = (ship.Position.X >= deadEnemy.Position.X);
                Combo += deadEnemy.Difficulty;
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
                    foreach (CycSprite s in skimCollided)
                    {
                        skimParticles.AddParticles(s.Center);
                    }
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