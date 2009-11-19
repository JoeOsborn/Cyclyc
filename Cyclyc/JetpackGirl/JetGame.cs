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
                    if (rgen.NextDouble() < 0.5)
                    {
                        return robots.Create(c, "robot", 2, CollisionStyle.Box, leftToRight, 0, 16, 21);
                    }
                    else
                    {
                        return spiders.Create(c, "spider", 3, CollisionStyle.Box, leftToRight, View.Height/2 - 34, 102 / 3, 17);
                    }
                };
        }

        protected override void SetupChallenges()
        {
            Challenge testChallenge = new Challenge(4);
            testChallenge.AddBeat(new ChallengeBeat(0, new EnemyMaker[] { MakeRandomEnemy(false, 0), MakeRandomEnemy(false, 0) }));
            testChallenge.AddBeat(new ChallengeBeat(2, new EnemyMaker[] { MakeRandomEnemy(true, 0), MakeRandomEnemy(true, 0) }));
            TriggerChallenge(0, testChallenge);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }
        
        public override void Update(GameTime gameTime)
        {
            robots.Update(gameTime);
            spiders.Update(gameTime);
            base.Update(gameTime);
            //robots.Collide(jetpackGirl), spiders.Collide(jetpackGirl)
        }

        protected override void DrawInnards(GameTime gt)
        {
            base.DrawInnards(gt);
            robots.Draw(gt);
            spiders.Draw(gt);
        }
    }
}