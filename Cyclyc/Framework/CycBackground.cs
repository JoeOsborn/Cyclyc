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

namespace Cyclyc.Framework
{
    class CycBackground : CycSprite
    {
        protected string assetName;
        public override string AssetName
        {
            get
            {
                return assetName;
            }
        }

        protected override float ScaleFactor
        {
            get { return 2.0f; }
        }

        protected float scrollSpeed;
        public float ScrollSpeed
        {
            get { return scrollSpeed; }
            set { scrollSpeed = value; }
        }

        protected override int SpriteWidth
        {
            get { return spriteSheet.Width; }
        }

        public CycBackground(Game1 game, string img)
            : base(game)
        {
            assetName = img;
            scrollSpeed = 1.0f;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            position.X -= scrollSpeed;
            if(position.X < -SpriteWidth)
            {
                position.X += SpriteWidth;
            }
            //base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (!Visible) { return; }
            Rectangle srcRect = new Rectangle(XForSprite(currentAnimation.CurrentFrame), 0, SpriteWidth, spriteSheet.Height);
            //modify srcRect.X for animation frame
            Rectangle dstRect = new Rectangle();
            //modify dstRect.X, .Y for position, viewport
            dstRect.X = (int)(position.X * ScaleFactor);
            dstRect.Y = (int)(view.Height*ScaleFactor - (VisualHeight*ScaleFactor));
            dstRect.Width = (int)(VisualWidth * ScaleFactor);
            dstRect.Height = (int)(VisualHeight * ScaleFactor);
            SpriteBatch.Draw(spriteSheet, dstRect, srcRect, Color.White);
            dstRect.X += SpriteWidth;
            SpriteBatch.Draw(spriteSheet, dstRect, srcRect, Color.White);
        }
    }
}
