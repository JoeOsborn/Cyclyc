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
    public class CycBackground : CycSprite
    {
        protected float scrollSpeed;
        public float ScrollSpeed
        {
            get { return scrollSpeed; }
            set { scrollSpeed = value; }
        }

        float targetAlpha, oldAlpha;
        public float TargetAlpha 
        { 
            get 
            { 
                return targetAlpha; 
            } 
            set
            {
                oldAlpha = targetAlpha; 
                targetAlpha = value;
            } 
        }
        float blendDuration;
        public float BlendDuration 
        {
            get
            {
                return blendDuration;
            }
            set
            {
                blendDuration = value;
            }
        }
        float blendTimer;

        public CycBackground(Game1 game, string img)
            : base(game)
        {
            ScaleFactor = 1.0f;
            assetName = img;
            scrollSpeed = 1.0f;
            TargetAlpha = 1.0f;
            blendTimer = 0.0f;
            BlendDuration = 0.0f;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            spriteWidth = spriteSheet.Width;
            visualWidth = SpriteWidth;
        }

        public override void Update(GameTime gameTime)
        {
            position.X -= scrollSpeed;
            if(ScrollSpeed > 0 && position.X < -VisualWidth)
            {
                position.X += VisualWidth;
            }
            if (ScrollSpeed < 0 && position.X > VisualWidth)
            {
                position.X -= VisualWidth;
            }
            if (blendTimer > 0)
            {
                blendTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
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
            float alpha = TargetAlpha;
            if (blendTimer > 0)
            {
                alpha = MathHelper.Lerp(TargetAlpha, oldAlpha, blendTimer / BlendDuration);
            }
            SpriteBatch.Draw(spriteSheet, dstRect, srcRect, new Color(1.0f, 1.0f, 1.0f, alpha));
            dstRect.X = (int)(dstRect.X + ((position.X <= 0) ? dstRect.Width : -dstRect.Width));
            SpriteBatch.Draw(spriteSheet, dstRect, srcRect, new Color(1.0f, 1.0f, 1.0f, alpha));
        }
    }
}
