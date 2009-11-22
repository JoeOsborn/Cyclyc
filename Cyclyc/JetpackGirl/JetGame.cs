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


namespace Cyclyc.JetpackGirl
{
    public class JetGame : Cyclyc.Framework.CycGame
    {
        JetpackGirl jg;

        //robots and spiders are the same right now, but laters robots will hop occasionally
        //and maybe jetpack?
        JetpackEnemyPool[] enemyPools;

        protected EndingSprite emptyShip;

        public JetGame(Game1 game)
            : base(game)
        {
            Grade1Expectation = 5;
            Grade2Expectation = 15;


            SongName = "jet";
            enemyPools = new JetpackEnemyPool[] { 
                new RobotEnemyPool(this), 
                new SpiderEnemyPool(this), 
                new HoverEnemyPool(this), 
                new FrogEnemyPool(this) 
            };
        }

        protected override CycBackground AddBackground(string n, float spd)
        {
            CycBackground bg = base.AddBackground(n, spd);
            bg.ScaleFactor = 2.0f;
            return bg;
        }

        protected float[] ParallaxSpeeds = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 0.2f, 0.4f, 0.6f, 0.6f };
        protected CycBackground lowSunset;
        protected CycBackground midSunset;
        protected CycBackground highSunset;

        public override void Initialize()
        {
            AddBackground("pixel city sky", ParallaxSpeeds[0]);
            lowSunset = AddBackground("pixel city sunset low saturation", ParallaxSpeeds[1]);
            midSunset = AddBackground("pixel city sunset med saturation", ParallaxSpeeds[2]);
            midSunset.TargetAlpha = 0.0f;
            midSunset.BlendDuration = 0.0f;
            highSunset = AddBackground("pixel city sunset high saturation", ParallaxSpeeds[3]);
            highSunset.TargetAlpha = 0.0f;
            highSunset.BlendDuration = 0.0f;
            AddBackground("pixel city skyline", ParallaxSpeeds[4]);
            AddBackground("pixel city middleground", ParallaxSpeeds[5]);
            AddBackground("pixel city foreground", ParallaxSpeeds[6]);
            AddBackground("pixel city road", ParallaxSpeeds[7]);

            jg = new JetpackGirl((Game1)Game, this);
            AddSprite(jg);

            base.Initialize();
        }

        protected override ChallengeBeat[] CoalesceChallengeBeatEnemies(bool left, int difficulty)
        {
            //ship game needs to halve the number of enemies
            EnemyMaker[] es = new EnemyMaker[difficulty / 2];
            for (int i = 0; i < difficulty / 2; i++)
            {
                es[i] = MakeEnemy(left, 2);
                if (left) { ConsumeLeft(2); }
                else { ConsumeRight(2); }
            }
            return new ChallengeBeat[] { new ChallengeBeat(0, es) };
        }

        protected override void CalculateGrade()
        {
            base.CalculateGrade();
            jg.Superize(Grade >= 2);
            if (Grade >= 2)
            {
                lowSunset.TargetAlpha = 0.0f;
                lowSunset.BlendDuration = 1.0f;
                midSunset.TargetAlpha = 0.0f;
                midSunset.BlendDuration = 1.0f;
                highSunset.TargetAlpha = 1.0f;
                highSunset.BlendDuration = 1.0f;
            }
            else if (Grade >= 1)
            {
                lowSunset.TargetAlpha = 0.0f;
                lowSunset.BlendDuration = 1.0f;
                midSunset.TargetAlpha = 1.0f;
                midSunset.BlendDuration = 1.0f;
                highSunset.TargetAlpha = 0.0f;
                highSunset.BlendDuration = 1.0f;
            }
            else
            {
                lowSunset.TargetAlpha = 1.0f;
                lowSunset.BlendDuration = 1.0f;
                midSunset.TargetAlpha = 0.0f;
                midSunset.BlendDuration = 1.0f;
                highSunset.TargetAlpha = 0.0f;
                highSunset.BlendDuration = 1.0f;
            }
        }

        public override int Score
        {
            get { return base.Score * 2; }
        }


        #region Random Enemy Maker

        public override EnemyMaker MakeEnemy(bool leftToRight, int difficulty)
        {
            double r = rgen.NextDouble();
            return (c) =>
                {
                    JetpackEnemy en;
                    //scale up or down based on difficulty, etc
                    float sizeMultiplier = (float)(rgen.NextDouble() * 1.0 + 0.5);
                    if (r < 0.35)
                    {
                        en = enemyPools[0].Create(c, "robot", 2, leftToRight, 0, (int)(16 * sizeMultiplier), (int)(21 * sizeMultiplier), (float)((rgen.NextDouble() * 1.0) + 0.25), (int)(3 * sizeMultiplier), (int)(3 * sizeMultiplier), (int)(10 * sizeMultiplier), (int)(18 * sizeMultiplier), difficulty);
                    }
                    else if (r < 0.5)
                    {
                        en = enemyPools[1].Create(c, "spider", 3, leftToRight, (int)(View.Height / 2 - 34 * sizeMultiplier), (int)(sizeMultiplier * 102 / 3), (int)(sizeMultiplier * 17), (float)(rgen.NextDouble() * 1.0) + 0.25f, (int)(sizeMultiplier * 5), (int)(sizeMultiplier * 4), (int)(sizeMultiplier * ((102 / 3) - 10)), (int)(sizeMultiplier * 11), difficulty);
                    }
                    else if (r < 0.75)
                    {
                        en = enemyPools[2].Create(c, "hover", 3, leftToRight, (int)(rgen.NextDouble() * 100 + 10), (int)(16 * sizeMultiplier), (int)(21 * sizeMultiplier), (float)((rgen.NextDouble() * 1.0) + 0.25), (int)(3 * sizeMultiplier), (int)(3 * sizeMultiplier), (int)(10 * sizeMultiplier), (int)(18 * sizeMultiplier), difficulty);
                    }
                    else
                    {
                        en = enemyPools[3].Create(c, "frog", 2, leftToRight, 0, (int)(16 * sizeMultiplier), (int)(25 * sizeMultiplier), (float)((rgen.NextDouble() * 1.0) + 0.25), (int)(0 * sizeMultiplier), (int)(5 * sizeMultiplier), (int)(16 * sizeMultiplier), (int)(25 * sizeMultiplier), difficulty);
                    }
                    en.Target = jg;
                    return en;
                };
        }
        #endregion


        #region enemyTypes
        // -----------------DEFINE SPECIFIC ENEMY TYPES------------------------------------------------

     
        protected EnemyMaker MakeRobotEnemy(float sizeMultiplier, float speedMultiplier)
        {
            return (c) =>
                {
                    JetpackEnemy en;
                    en = enemyPools[0].Create(c, "robot", 2, false, 0, (int)(16 * sizeMultiplier), (int)(21 * sizeMultiplier), (float)((speedMultiplier * 1.0) + 0.25), (int)(3 * sizeMultiplier), (int)(3 * sizeMultiplier), (int)(10 * sizeMultiplier), (int)(18 * sizeMultiplier), 1);
                    en.Target = jg;
                    return en;
                };
        }

        protected EnemyMaker MakeSpiderEnemy(float sizeMultiplier, float speedMultiplier)
        {
            return (c) =>
                enemyPools[1].Create(c, "spider", 3, false, 0, (int)(sizeMultiplier * 102 / 3), (int)(sizeMultiplier * 17), (float)(speedMultiplier * 1.0) + 0.25f, (int)(sizeMultiplier * 5), (int)(sizeMultiplier * 4), (int)(sizeMultiplier * ((102 / 3) - 10)), (int)(sizeMultiplier * 11), 1);
        }

        protected EnemyMaker MakeHoverEnemy(int y, float sizeMultiplier, float speedMultiplier)
        {
            return (c) =>
                enemyPools[2].Create(c, "hover", 3, false, y, (int)(16 * sizeMultiplier), (int)(21 * sizeMultiplier), (float)((sizeMultiplier * 1.0) + 0.25), (int)(3 * sizeMultiplier), (int)(3 * sizeMultiplier), (int)(10 * sizeMultiplier), (int)(18 * sizeMultiplier), 1);
        }

        protected EnemyMaker MakeFrogEnemy(float sizeMultiplier, float speedMultiplier)
        {
            return (c) =>
                enemyPools[3].Create(c, "frog", 2, false, 0, (int)(16 * sizeMultiplier), (int)(25 * sizeMultiplier), (float)((speedMultiplier * 1.0) + 0.25), (int)(0 * sizeMultiplier), (int)(5 * sizeMultiplier), (int)(16 * sizeMultiplier), (int)(25 * sizeMultiplier), 1);
        }
        
// --------------------------------------------------------------------------------------
        #endregion


        #region Level Design

        protected override void SetupChallenges()
        {
            #region Demo Level
            //Challenge testChallenge = new Challenge(this, Game, 4);
            //testChallenge.AddBeat(new ChallengeBeat(0, new EnemyMaker[] { MakeEnemy(false, 1), MakeEnemy(false, 1) }));
            //testChallenge.AddBeat(new ChallengeBeat(2, new EnemyMaker[] { MakeEnemy(true, 1), MakeEnemy(true, 1) }));
            //TriggerChallenge(0, testChallenge);
            #endregion

            #region Intro

            Challenge wave1_0 = new Challenge(this, Game, 3);
            wave1_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeSpiderEnemy(1,1)
            }));
            wave1_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeRobotEnemy(1,1)
            }));
            wave1_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeSpiderEnemy(1,1), MakeSpiderEnemy(1,1)
            }));
            TriggerChallenge(0, wave1_0);

            Challenge wave2_0 = new Challenge(this, Game, 9);
            wave2_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeFrogEnemy(1,2)
            }));
            wave2_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeFrogEnemy(1,2)
            }));
            wave2_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeSpiderEnemy(1,2)
            }));
            wave2_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeFrogEnemy(1,2)
            }));
            TriggerChallenge(0, wave2_0);

            Challenge wave3_0 = new Challenge(this, Game, 13);
            wave3_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeFrogEnemy(1,1)
            }));
            wave3_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeFrogEnemy(1,1)
            }));
            TriggerChallenge(0, wave3_0);

            Challenge wave4_0 = new Challenge(this, Game, 19);
            wave4_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeRobotEnemy(1,1), MakeHoverEnemy(100, 1, 2)
            }));
            wave4_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeFrogEnemy(1,1)
            }));
            wave4_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeSpiderEnemy(1,1)
            }));
            wave4_0.AddBeat(new ChallengeBeat(10, new EnemyMaker[] {
                MakeSpiderEnemy(1,1)
            }));
            wave4_0.AddBeat(new ChallengeBeat(12, new EnemyMaker[] {
                MakeSpiderEnemy(1,1)
            }));
            TriggerChallenge(0, wave4_0);

            Challenge wave5_0 = new Challenge(this, Game, 25);
            wave5_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeHoverEnemy(30, 1, 1), MakeHoverEnemy(80, 1, 1), MakeHoverEnemy(130,                1, 1), MakeHoverEnemy(180, 1, 1), MakeHoverEnemy(230, 1, 1)
            }));
            TriggerChallenge(0, wave5_0);

            Challenge wave6_0 = new Challenge(this, Game, 29);
            wave6_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeRobotEnemy(1,1)
            }));
            wave6_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeRobotEnemy(1,1)
            }));
            wave6_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeRobotEnemy(1,1)
            }));
            wave6_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeRobotEnemy(1,1)
            }));
            TriggerChallenge(0, wave6_0);

            #endregion

            #region verse1

            Challenge wave7_0 = new Challenge(this, Game, 34);
            wave7_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeHoverEnemy(0,1,2), MakeHoverEnemy(30,1,2), MakeHoverEnemy(60,1,2),                 MakeHoverEnemy(90,1,2), MakeHoverEnemy(120,1,2), MakeHoverEnemy(150,1,                 2), MakeHoverEnemy(180,1,2), MakeHoverEnemy(210,1,2), MakeHoverEnemy                   (240,1,2)
            }));
            wave7_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeRobotEnemy(2,1)
            }));
            TriggerChallenge(0, wave7_0);

            Challenge wave8_0 = new Challenge(this, Game, 38);
            wave8_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeHoverEnemy(260,1,2)
            }));
            wave8_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeHoverEnemy(230,1,2)
            }));
            wave8_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeHoverEnemy(200,1,2)
            }));
            wave8_0.AddBeat(new ChallengeBeat(12, new EnemyMaker[] {
                MakeSpiderEnemy(1,1)
            }));
            wave8_0.AddBeat(new ChallengeBeat(16, new EnemyMaker[] {
                MakeHoverEnemy(170,1,2)
            }));
            wave8_0.AddBeat(new ChallengeBeat(20, new EnemyMaker[] {
                MakeHoverEnemy(140,1,2)
            }));
            TriggerChallenge(0, wave8_0);

            Challenge wave9_0 = new Challenge(this, Game, 42);
            wave9_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeHoverEnemy(0,1,1), MakeHoverEnemy(30,1,1), MakeFrogEnemy(1,1)
            }));
            wave9_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeHoverEnemy(0,1,1), MakeHoverEnemy(30,1,1), MakeRobotEnemy(1,1)
            }));
            TriggerChallenge(0, wave9_0);

            Challenge wave10_0 = new Challenge(this, Game, 46);
            wave10_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeSpiderEnemy(1,1), MakeHoverEnemy(50, 1, 1)
            }));
            wave10_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeSpiderEnemy(1,1), MakeHoverEnemy(50, 1, 1)
            }));
            wave10_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeSpiderEnemy(1,1), MakeHoverEnemy(50, 1, 1)
            }));
            wave10_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeSpiderEnemy(1,1), MakeHoverEnemy(50, 1, 1)
            }));
            TriggerChallenge(0, wave10_0);

            #endregion

            #region verse2

            Challenge wave11_0 = new Challenge(this, Game, 50);
            wave11_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeHoverEnemy(140,1,1)
            }));
            wave11_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeHoverEnemy(170,1,1), MakeHoverEnemy(110,1,1)
            }));
            wave11_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeHoverEnemy(200,1,1), MakeHoverEnemy(80,1,1)
            }));
            wave11_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeHoverEnemy(230,1,1), MakeHoverEnemy(50,1,1)
            }));
            wave11_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeHoverEnemy(260,1,1), MakeHoverEnemy(20,1,1)
            }));
            TriggerChallenge(0, wave11_0);

            Challenge wave12_0 = new Challenge(this, Game, 54);
            wave12_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeSpiderEnemy(1,2)
            }));
            TriggerChallenge(0, wave12_0);

            Challenge wave13_0 = new Challenge(this, Game, 58);
            wave13_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeRobotEnemy(2,1)
            }));
            TriggerChallenge(0, wave13_0);

            Challenge wave14_0 = new Challenge(this, Game, 62);
            wave14_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeFrogEnemy(1,1)
            }));
            wave14_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeFrogEnemy(1,1)
            }));
            TriggerChallenge(0, wave14_0);

            #endregion

            #region bridge

            Challenge wave15_0 = new Challenge(this, Game, 66);
            wave15_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeHoverEnemy(180,1,1)
            }));
            wave15_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeHoverEnemy(220,1,1)
            }));
            wave15_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeHoverEnemy(180, 1,1)
            }));
            wave15_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeHoverEnemy(220, 1,1)
            }));
            wave15_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeHoverEnemy(180, 1,1)
            }));
            wave15_0.AddBeat(new ChallengeBeat(10, new EnemyMaker[] {
                MakeHoverEnemy(220, 1,1)
            }));
            TriggerChallenge(0, wave15_0);

            Challenge wave16_0 = new Challenge(this, Game, 70);
            wave16_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeRobotEnemy(1,1), MakeHoverEnemy(80, 1,1)
            }));
            wave16_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeRobotEnemy(1,1), MakeHoverEnemy(80, 1,1)
            }));
            wave16_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeRobotEnemy(1,1), MakeHoverEnemy(80, 1,1)
            }));
            TriggerChallenge(0, wave16_0);

            Challenge wave17_0 = new Challenge(this, Game, 73);
            wave17_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeHoverEnemy(0,1,1), MakeHoverEnemy(30,1,1), MakeHoverEnemy(60,1,1), MakeHoverEnemy(90,1,1),MakeHoverEnemy(120,1,1),MakeHoverEnemy(150,1,1), MakeHoverEnemy(180,1,1), MakeHoverEnemy(210,1,1), MakeHoverEnemy(240,1,1), MakeHoverEnemy(270,1,1), 
            }));
            wave17_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeHoverEnemy(0,1,1), MakeHoverEnemy(30,1,1), MakeHoverEnemy(60,1,1), MakeHoverEnemy(90,1,1),MakeHoverEnemy(120,1,1),MakeHoverEnemy(150,1,1), MakeHoverEnemy(180,1,1), MakeHoverEnemy(210,1,1), MakeHoverEnemy(240,1,1), MakeHoverEnemy(270,1,1), 
            }));
            TriggerChallenge(0, wave17_0);

            Challenge wave18_0 = new Challenge(this, Game, 77);
            wave18_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeSpiderEnemy(1,1), MakeFrogEnemy(1,1)
            }));
            wave18_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeSpiderEnemy(1,1),
            }));
            wave18_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeSpiderEnemy(1,1),
            }));
            wave18_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeSpiderEnemy(1,1),
            }));
            wave18_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeSpiderEnemy(1,1),
            }));
            wave18_0.AddBeat(new ChallengeBeat(10, new EnemyMaker[] {
                MakeSpiderEnemy(1,1),
            }));
            TriggerChallenge(0, wave18_0);

            #endregion

            #region verse3

            Challenge wave19_0 = new Challenge(this, Game, 79);
            wave19_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeHoverEnemy(0,1,1), MakeHoverEnemy(60,1,1), MakeHoverEnemy(120,1,1), MakeHoverEnemy(180,1,1), MakeHoverEnemy(240,1,1)
            }));
            wave19_0.AddBeat(new ChallengeBeat(2, new EnemyMaker[] {
                MakeHoverEnemy(30,1,1), MakeHoverEnemy(90,1,1), MakeHoverEnemy(150,1,1), MakeHoverEnemy(210,1,1), MakeHoverEnemy(270,1,1)
            }));
            wave19_0.AddBeat(new ChallengeBeat(4, new EnemyMaker[] {
                MakeHoverEnemy(0,1,1), MakeHoverEnemy(60,1,1), MakeHoverEnemy(120,1,1), MakeHoverEnemy(180,1,1), MakeHoverEnemy(240,1,1)
            }));
            wave19_0.AddBeat(new ChallengeBeat(6, new EnemyMaker[] {
                MakeHoverEnemy(30,1,1), MakeHoverEnemy(90,1,1), MakeHoverEnemy(150,1,1), MakeHoverEnemy(210,1,1), MakeHoverEnemy(270,1,1)
            }));
            wave19_0.AddBeat(new ChallengeBeat(8, new EnemyMaker[] {
                MakeHoverEnemy(0,1,1), MakeHoverEnemy(60,1,1), MakeHoverEnemy(120,1,1), MakeHoverEnemy(180,1,1), MakeHoverEnemy(240,1,1)
            }));
            wave19_0.AddBeat(new ChallengeBeat(12, new EnemyMaker[] {
                MakeHoverEnemy(30,1,1), MakeHoverEnemy(90,1,1), MakeHoverEnemy(150,1,1), MakeHoverEnemy(210,1,1), MakeHoverEnemy(270,1,1)
            }));
            TriggerChallenge(0, wave19_0);

            Challenge wave20_0 = new Challenge(this, Game, 82);
            wave20_0.AddBeat(new ChallengeBeat(0, new EnemyMaker[] {
                MakeRobotEnemy(5,1), MakeRobotEnemy(3,2)
            }));
            TriggerChallenge(0, wave20_0);

            #endregion

        }
        #endregion



        public override void LoadContent()
        {
            jg.StartPosition = StartPosition;
            jg.Position = StartPosition;
            base.LoadContent();
            jg.ForceFeedback = ff;
            foreach (JetpackEnemyPool ep in enemyPools)
            {
                ep.ForceFeedback = ff;
            }
        }

        protected Vector2 StartPosition
        {
            get
            {
                //a random free range on the left upper part of the screen
                Vector2 oldPos = jg.Position;
                do
                {
                    jg.Position = new Vector2((float)(rgen.NextDouble() * 170) + 30, 0);
                } while (enemyPools.Any((ep) => ep.Collide(jg).Count != 0));
                Vector2 ret = jg.Position;
                jg.Position = oldPos;
                return ret;
            }
        }

        public void KillPlayer()
        {
            Combo = 0;
            jg.Die();
            Console.WriteLine("jet killed player");
        }
        protected float EmptyShipSpeed
        {
            get { return -0.6f; }
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
                jg.GroundLoss = jg.DefaultGroundLoss * (1 - r);
                if (emptyShip == null)
                {
                    emptyShip = new EndingSprite(Game, "emptyShip", new Vector2(View.Width + 125, View.Height - 79), 125);
                    emptyShip.Initialize();
                    emptyShip.LoadContent();
                    AddSprite(emptyShip);
                }
                emptyShip.Velocity = new Vector2(EmptyShipSpeed * (1 - r), emptyShip.Velocity.Y);
            }
            foreach (EnemyPool ep in enemyPools)
            {
                ep.Update(gameTime);
            }
            base.Update(gameTime);
            if (jg.Attacking)
            {
                foreach (EnemyPool ep in enemyPools)
                {
                    foreach (CycEnemy hit in ep.Collide(jg.Wrench))
                    {
                        ((JetpackEnemy)(hit)).Hit(jg.Position.X);
                    }
                }
            }
            foreach (EnemyPool ep in enemyPools)
            {
                foreach (CycEnemy en in ep.Collide(jg))
                {
                    if (!((JetpackEnemy)en).IsHit && !jg.Dying)
                    {
                        KillPlayer();
                    }
                }
                foreach (CycEnemy en in ep.Enemies)
                {
                    if (en.Alive && ((JetpackEnemy)en).KnockedOffScreen)
                    {
                        Combo += en.Difficulty;
                        NextGame.DeliverEnemy(en.Position.X < 0 ? true : false, en.Difficulty);
                        en.Die();
                    }
                }
            }
            if (jg.Dying)
            {
                jg.StartPosition = StartPosition;
            }
        }

        protected override void DrawInnards(GameTime gt)
        {
            base.DrawInnards(gt);
            foreach (EnemyPool ep in enemyPools)
            {
                ep.Draw(gt);
            }
        }
    }
}