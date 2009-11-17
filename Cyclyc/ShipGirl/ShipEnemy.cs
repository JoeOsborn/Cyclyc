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
    public class ShipEnemy : CycSprite
    {
        protected int FrameCount
        {
            get { return 2; }
        }

        protected string assetName;
        public override string AssetName
        {
            get { return assetName; }
        }
        protected int spriteWidth;
        protected override int SpriteWidth
        {
            get { return spriteWidth; }
        }

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

        protected bool leftToRight;

        protected Vector2 startPosition;

        public ShipEnemy(Game1 game)
            : base(game)
        {
            animations["default"] = new Animation(new int[] { 0, 1 }, new int[] { 10, 10 }, true);
            timeScale = 1.0;
        }

        public override void LoadContent()
        {
            xCurve = Game.Content.Load<Curve>(curveSet + "-X");
            yCurve = Game.Content.Load<Curve>(curveSet + "-Y");
            base.LoadContent();
            spriteWidth = spriteSheet.Width / FrameCount;
        }

        public void Reset(string img, string curves, bool left, int y, int w, int h, double ts)
        {
            timeScale = ts;
            tick = 0;
            assetName = img;
            curveSet = curves;
            LoadContent();
            Play("default");
            bounds = new Rectangle(0, 0, w, h);
            int x = 0;
            if (left)
            {
                x = -SpriteWidth;
                leftToRight = true;
            }
            else
            {
                x = 800;
                leftToRight = false;
            }
            startPosition = new Vector2(x, y);
            position = startPosition;
            velocity = new Vector2(0, 0);
            leftToRight = false;
            alive = true;
            visible = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (alive)
            {
                tick += gameTime.ElapsedGameTime.TotalSeconds*timeScale;
                position.X = startPosition.X + (leftToRight ? xCurve.Evaluate((float)tick) : -xCurve.Evaluate((float)tick));
                position.Y = startPosition.Y + yCurve.Evaluate((float)tick);
                //if (IsPastRightEdge(gameTime))
                //{
                //    alive = false;
                //    visible = false;
                //}
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}