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
    public class ShipEnemy : CycEnemy
    {
        protected string curveSet;

        protected Curve xCurve;
        protected Curve yCurve;
        protected double tick;
        protected double timeScale;
        public double TimeScale
        {
            get { return timeScale; }
            set { timeScale = value; }
        }

        public ShipEnemy(Game1 game, EnemyPool p)
            : base(game, p)
        {
            timeScale = 1.0;
            collisionStyle = CollisionStyle.Circle;
        }

        public override void LoadContent()
        {
            xCurve = Game.Content.Load<Curve>(curveSet + "-X");
            yCurve = Game.Content.Load<Curve>(curveSet + "-Y");
            base.LoadContent();
        }

        public void Reset(Challenge c, string img, int fc, CollisionStyle col, string curves, bool left, int xp, int yp, int w, int h, double ts)
        {
            timeScale = ts;
            tick = 0;
            curveSet = curves;
            Reset(c, img, fc, col, left, xp, yp, w, h);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }


        protected override void UpdatePosition(GameTime gameTime)
        {
            tick += gameTime.ElapsedGameTime.TotalSeconds * timeScale;
            position.X = startPosition.X + (leftToRight ? xCurve.Evaluate((float)tick) : -xCurve.Evaluate((float)tick));
            position.Y = startPosition.Y + yCurve.Evaluate((float)tick);
        }

    }
}