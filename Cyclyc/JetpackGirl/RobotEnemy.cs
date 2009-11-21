using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    class RobotEnemy : JetpackEnemy
    {
        float jetTime;
        public RobotEnemy(Game1 game, EnemyPool p)
            : base(game, p)
        {
            jetTime = 0;
            jetpack.MaxJPFuel = 2f;
        }
        public override bool ShouldJet
        {
            get 
            {
                return (jetTime > 0);
            }
        }
        public override bool ShouldJump
        {
            get 
            { 
                return !IsHit && (rgen.NextDouble() < 0.001 || (Target.Position.Y < position.Y && rgen.NextDouble() < 0.005)); 
            }
        }
        protected double MaxJetPeriod
        {
            get { return 1.0; }
        }
        public override void Update(GameTime gameTime)
        {
            if (jetTime > 0)
            {
                jetTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (!IsHit && (Target.Position.Y < position.Y && rgen.NextDouble() < 0.05))
            {
                jetTime = (float)(rgen.NextDouble() * MaxJetPeriod);
            }
            if (IsHit)
            {
                jetTime = 0;
            }
            base.Update(gameTime);
        }
    }
}
