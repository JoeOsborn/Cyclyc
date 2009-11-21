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
        RobotEnemyPool robots;
        SpiderEnemyPool spiders;
        HoverEnemyPool hovers;

        public JetGame(Game1 game)
            : base(game)
        {
            SongName = "jet";
            robots = new RobotEnemyPool(this);
            spiders = new SpiderEnemyPool(this);
            hovers = new HoverEnemyPool(this);
        }

        protected override void AddBackground(string n, float spd)
        {
            base.AddBackground(n, spd);
            Backgrounds.Last().ScaleFactor = 2.0f;
        }

        public override void Initialize()
        {
            AddBackground("pixel city sky", 0.1f);
            AddBackground("pixel city sunset", 0.1f);
            AddBackground("pixel city skyline", 0.2f);
            AddBackground("pixel city middleground", 0.4f);
            AddBackground("pixel city foreground", 0.6f);
            AddBackground("pixel city road", 0.6f);
            
            jg = new JetpackGirl((Game1)Game);
            AddSprite(jg);
            jg.StartPosition = StartPosition;

            base.Initialize();
        }

        protected override void CoalesceChallengeBeats(Challenge c)
        {
            base.CoalesceChallengeBeats(c);
        }

        public override EnemyMaker MakeEnemy(bool leftToRight, int difficulty)
        {
            double r = rgen.NextDouble();
            return (c) =>
                {
                    JetpackEnemy en;
                    //scale up or down based on difficulty, etc
                    float sizeMultiplier = (float)(rgen.NextDouble() * 1.0 + 0.5);
                    if (r < 0.3)
                    {
                        en = robots.Create(c, "robot", 2, leftToRight, 0, (int)(16 * sizeMultiplier), (int)(21 * sizeMultiplier), (float)((rgen.NextDouble() * 1.0) + 0.25), (int)(3 * sizeMultiplier), (int)(3 * sizeMultiplier), (int)(10 * sizeMultiplier), (int)(18 * sizeMultiplier), difficulty);
                    }
                    else if (r < 0.6)
                    {
                        en = spiders.Create(c, "spider", 3, leftToRight, (int)(View.Height / 2 - 34 * sizeMultiplier), (int)(sizeMultiplier * 102 / 3), (int)(sizeMultiplier * 17), (float)(rgen.NextDouble() * 1.0) + 0.25f, (int)(sizeMultiplier * 5), (int)(sizeMultiplier * 4), (int)(sizeMultiplier * ((102 / 3) - 10)), (int)(sizeMultiplier * 11), difficulty);
                    }
                    else
                    {
                        en = hovers.Create(c, "hover", 3, leftToRight, (int)(rgen.NextDouble()*100 + 10), (int)(16 * sizeMultiplier), (int)(21 * sizeMultiplier), (float)((rgen.NextDouble() * 1.0) + 0.25), (int)(3 * sizeMultiplier), (int)(3 * sizeMultiplier), (int)(10 * sizeMultiplier), (int)(18 * sizeMultiplier), difficulty);
                    }

                    en.Target = jg;
                };
        }

        protected override void SetupChallenges()
        {
            Challenge testChallenge = new Challenge(this, Game, 4);
            testChallenge.AddBeat(new ChallengeBeat(0, new EnemyMaker[] { MakeEnemy(false, 1), MakeEnemy(false, 1) }));
            testChallenge.AddBeat(new ChallengeBeat(2, new EnemyMaker[] { MakeEnemy(true, 1), MakeEnemy(true, 1) }));
            TriggerChallenge(0, testChallenge);
        }

        public override void LoadContent()
        {
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
                    jg.Position = new Vector2((float)(rgen.NextDouble() * 170)+30, 0);
                } while (spiders.Collide(jg).Count != 0 || robots.Collide(jg).Count != 0 || hovers.Collide(jg).Count != 0);
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

        public override void Update(GameTime gameTime)
        {
            robots.Update(gameTime);
            spiders.Update(gameTime);
            hovers.Update(gameTime);
            base.Update(gameTime);
            if (jg.Attacking)
            {
                List<CycEnemy> hitRobots = robots.Collide(jg.Wrench);
                List<CycEnemy> hitSpiders = spiders.Collide(jg.Wrench);
                List<CycEnemy> hitHovers = hovers.Collide(jg.Wrench);
                foreach (CycEnemy hit in hitRobots)
                {
                    ((JetpackEnemy)(hit)).Hit(jg.Position.X);
                }
                foreach (CycEnemy hit in hitSpiders)
                {
                    ((JetpackEnemy)(hit)).Hit(jg.Position.X);
                }
                foreach (CycEnemy hit in hitHovers)
                {
                    ((JetpackEnemy)(hit)).Hit(jg.Position.X);
                }
            }
            List<CycEnemy> killerRobots = robots.Collide(jg);
            List<CycEnemy> killerSpiders = spiders.Collide(jg);
            List<CycEnemy> killerHovers = hovers.Collide(jg);
            foreach (CycEnemy en in killerRobots)
            {
                if (!((JetpackEnemy)en).IsHit && !jg.Dying)
                {
                    KillPlayer();
                }
            }
            foreach (CycEnemy en in killerSpiders)
            {
                if (!((JetpackEnemy)en).IsHit && !jg.Dying)
                {
                    KillPlayer();
                }
            }
            foreach (CycEnemy en in killerHovers)
            {
                if (!((JetpackEnemy)en).IsHit && !jg.Dying)
                {
                    KillPlayer();
                }
            }
            foreach (CycEnemy en in robots.Enemies)
            {
                if (en.Alive && ((JetpackEnemy)en).KnockedOffScreen)
                {
                    NextGame.DeliverEnemy(en.Position.X < 0 ? true : false, en.Difficulty);
                    en.Die();
                }
            }
            foreach (CycEnemy en in spiders.Enemies)
            {
                if (en.Alive && ((JetpackEnemy)en).KnockedOffScreen)
                {
                    NextGame.DeliverEnemy(en.Position.X < 0 ? true : false, en.Difficulty);
                    en.Die();
                }
            }
            foreach (CycEnemy en in hovers.Enemies)
            {
                if (en.Alive && ((JetpackEnemy)en).KnockedOffScreen)
                {
                    NextGame.DeliverEnemy(en.Position.X < 0 ? true : false, en.Difficulty);
                    en.Die();
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
            robots.Draw(gt);
            spiders.Draw(gt);
            hovers.Draw(gt);
        }
    }
}