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
            SongName = "jet";
            enemyPools = new JetpackEnemyPool[] { 
                new RobotEnemyPool(this), 
                new SpiderEnemyPool(this), 
                new HoverEnemyPool(this), 
                new FrogEnemyPool(this) 
            };
        }

        protected override void AddBackground(string n, float spd)
        {
            base.AddBackground(n, spd);
            Backgrounds.Last().ScaleFactor = 2.0f;
        }

        protected float[] ParallaxSpeeds = new float[] { 0.1f, 0.1f, 0.2f, 0.4f, 0.6f, 0.6f };

        public override void Initialize()
        {
            AddBackground("pixel city sky", ParallaxSpeeds[0]);
            AddBackground("pixel city sunset", ParallaxSpeeds[1]);
            AddBackground("pixel city skyline", ParallaxSpeeds[2]);
            AddBackground("pixel city middleground", ParallaxSpeeds[3]);
            AddBackground("pixel city foreground", ParallaxSpeeds[4]);
            AddBackground("pixel city road", ParallaxSpeeds[5]);

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

        #region enemyTypes
        //   DEFINE SPECIFIC ENEMY TYPES








        #endregion

        protected override void SetupChallenges()
        {
            Challenge testChallenge = new Challenge(this, Game, 4);
            testChallenge.AddBeat(new ChallengeBeat(0, new EnemyMaker[] { MakeEnemy(false, 1), MakeEnemy(false, 1) }));
            testChallenge.AddBeat(new ChallengeBeat(2, new EnemyMaker[] { MakeEnemy(true, 1), MakeEnemy(true, 1) }));
            TriggerChallenge(0, testChallenge);
        }

        public override void LoadContent()
        {
            jg.StartPosition = StartPosition;
            jg.Position = StartPosition;
            base.LoadContent();
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