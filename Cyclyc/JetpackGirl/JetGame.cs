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

        RobotEnemyPool robots;
        SpiderEnemyPool spiders;

        public JetGame(Game1 game)
            : base(game)
        {
            robots = new RobotEnemyPool(this);
            spiders = new SpiderEnemyPool(this);
        }

        public override void Initialize()
        {
            AddBackground("pixel city sky", 0.1f);
            AddBackground("pixel city skyline", 0.3f);
            AddBackground("pixel city foreground", 0.5f);
            
            jg = new JetpackGirl((Game1)Game);
            AddSprite(jg);

            base.Initialize();
        }

        public override EnemyMaker MakeRandomEnemy(bool leftToRight, int difficulty)
        {
            return (c) =>
                {
                    JetpackEnemy en;
                    if (rgen.NextDouble() < 0.5)
                    {
                        en = robots.Create(c, "robot", 2, leftToRight, 0, 16, 21, (float)((rgen.NextDouble() * 1.0)+0.25), 2, 2, 12, 17);
                    }
                    else
                    {
                        en = spiders.Create(c, "spider", 3, leftToRight, View.Height / 2 - 34, 102 / 3, 17, (float)(rgen.NextDouble() * 1.0) + 0.25f, 6, 2, (102/3)-12, 13);
                    }
                    en.Target = jg;
                    return en;
                };
        }

        protected override void SetupChallenges()
        {
            Challenge testChallenge = new Challenge(this, Game, 4);
            testChallenge.AddBeat(new ChallengeBeat(0, new EnemyMaker[] { MakeRandomEnemy(false, 0), MakeRandomEnemy(false, 0) }));
            testChallenge.AddBeat(new ChallengeBeat(2, new EnemyMaker[] { MakeRandomEnemy(true, 0), MakeRandomEnemy(true, 0) }));
            TriggerChallenge(0, testChallenge);
        }

        public override void LoadContent()
        {
            base.LoadContent();
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
            base.Update(gameTime);
            if (jg.Attacking)
            {
                List<CycEnemy> hitRobots = robots.Collide(jg.Wrench);
                List<CycEnemy> hitSpiders = spiders.Collide(jg.Wrench);
                foreach (CycEnemy hit in hitRobots)
                {
                    ((JetpackEnemy)(hit)).Hit(jg.Position.X);
                }
                foreach (CycEnemy hit in hitSpiders)
                {
                    ((JetpackEnemy)(hit)).Hit(jg.Position.X);
                }
            }
            if(robots.Collide(jg).Count != 0 || spiders.Collide(jg).Count != 0)
            {
                if (!jg.Dying)
                {
                    KillPlayer();
                }
            }
            foreach (CycEnemy en in robots.Enemies)
            {
                if (en.Alive && ((JetpackEnemy)en).KnockedOffScreen)
                {
                    en.Die();
                    NextGame.DeliverRandomEnemy(en.Position.X < 0 ? true : false, 0);
                }
            }
            foreach (CycEnemy en in spiders.Enemies)
            {
                if (en.Alive && ((JetpackEnemy)en).KnockedOffScreen)
                {
                    en.Die();
                    NextGame.DeliverRandomEnemy(en.Position.X < 0 ? true : false, 0);
                }
            }
        }

        protected override void DrawInnards(GameTime gt)
        {
            base.DrawInnards(gt);
            robots.Draw(gt);
            spiders.Draw(gt);
        }
    }
}