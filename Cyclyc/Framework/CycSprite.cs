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


namespace Cyclyc.Framework
{
    /// <summary>
    /// This is a game component that implements IUpdateable/Drawable.
    /// </summary>
    public class CycSprite : Object
    {
        protected bool alive;
        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }
        protected bool visible;
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        protected Viewport view;
        public virtual Viewport View
        {
            get { return view; }
            set 
            { 
                view.X = (int)(value.X / ScaleFactor);
                view.Y = (int)(value.Y / ScaleFactor);
                view.Width = (int)(value.Width / ScaleFactor);
                view.Height = (int)(value.Height / ScaleFactor);
            }
        }

        protected Game1 Game;

        protected GraphicsDevice GraphicsDevice
        {
            get { return Game.GraphicsDevice; }
        }

        protected float FloorY
        {
            get { return view.Height; }
        }
        protected float CeilY
        {
            get { return 0; }
        }
        protected float LeftX
        {
            get { return 0; }
        }
        protected float RightX
        {
            get { return view.Width; }
        }
        protected float TopEdge
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        protected float BottomEdge
        {
            get { return position.Y + bounds.Y + bounds.Height; }
            set { position.Y = (value - bounds.Y - bounds.Height); }
        }
        protected float LeftEdge
        {
            get { return position.X; }
            set { position.X = value; }
        }
        protected float RightEdge
        {
            get { return position.X + bounds.X + bounds.Width; }
            set { position.X = (value - bounds.X - bounds.Width); }
        }

        protected virtual float ScaleFactor
        {
            get { return 1.0f; }
        }

        protected Texture2D spriteSheet;
        public Rectangle bounds;
        public Vector2 position;
        public Vector2 velocity;

        public Animation currentAnimation;
        public Dictionary<string, Animation> animations;
        public Vector2 Center
        {
            get { return new Vector2(position.X + SpriteWidth/2.0f, position.Y + spriteSheet.Height/2.0f); }
        }
        public virtual string AssetName
        {
            get { return "placeholder"; }
        }

        protected SpriteBatch SpriteBatch
        {
            get { return Game.SpriteBatch; }
        }

        protected virtual int SpriteWidth
        {
            get { return 8; }
        }
        protected virtual int XForSprite(int i)
        {
            return SpriteWidth * i;
        }
        public void Play(string anim) { Play(anim, true); }
        public void Play(string anim, bool retrigger)
        {
            if (animations.ContainsKey(anim))
            {
                if (currentAnimation == animations[anim] && !retrigger)
                {
                    return;
                }
                currentAnimation = animations[anim];
                currentAnimation.Play();
            }
        }
        
        public CycSprite(Game1 game)
        {
            Game = game;
            alive = true;
            visible = true;
            bounds = new Rectangle(0, 0, 8, 8);
            position = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            currentAnimation = null;
            animations = new Dictionary<string, Animation>();
            animations["default"] = new Animation(new int[]{0}, new int[]{0}, false);
            Play("default");
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public virtual void Initialize()
        {

        }

        public virtual void LoadContent()
        {
            spriteSheet = Game.Content.Load<Texture2D>(AssetName);
        }

        protected virtual void MoveInX(GameTime gt)
        {
            position.X += velocity.X;
        }
        protected virtual void MoveInY(GameTime gt)
        {
            position.Y += velocity.Y;
        }
        protected virtual bool IsOnRightEdge(GameTime gt)
        {
            return (RightEdge + velocity.X) > RightX;
        }
        protected virtual bool IsPastRightEdge(GameTime gt)
        {
            return (LeftEdge + velocity.X) > RightX;
        }
        protected virtual bool StopAtRightEdge(GameTime gt)
        {
            return true;
        }
        protected virtual void HitRightEdge(GameTime gt)
        {
            RightEdge = RightX;
        }
        protected virtual void OffRightEdge(GameTime gt)
        {
            //destroy self
        }
        protected virtual bool IsOnLeftEdge(GameTime gt)
        {
            return (LeftEdge + velocity.X) < LeftX;
        }
        protected virtual bool IsPastLeftEdge(GameTime gt)
        {
            return (RightEdge + velocity.X) < LeftX;
        }
        protected virtual bool StopAtLeftEdge(GameTime gt)
        {
            return true;
        }
        protected virtual void HitLeftEdge(GameTime gt)
        {
            LeftEdge = LeftX;
        }
        protected virtual void OffLeftEdge(GameTime gt)
        {
            //destroy self
        }
        protected virtual bool IsOnBottomEdge(GameTime gt)
        {
            return (BottomEdge + velocity.Y) > FloorY;
        }
        protected virtual bool IsPastBottomEdge(GameTime gt)
        {
            return (TopEdge + velocity.Y) > FloorY;
        }
        protected virtual bool StopAtBottomEdge(GameTime gt)
        {
            return true;
        }
        protected virtual void HitBottomEdge(GameTime gt)
        {
            BottomEdge = FloorY;
        }
        protected virtual void OffBottomEdge(GameTime gt)
        {
            //destroy self
        }
        protected virtual bool IsOnTopEdge(GameTime gt)
        {
            return (TopEdge + velocity.Y) < CeilY;
        }
        protected virtual bool IsPastTopEdge(GameTime gt)
        {
            return (BottomEdge + velocity.Y) < CeilY;
        }
        protected virtual bool StopAtTopEdge(GameTime gt)
        {
            return true;
        }
        protected virtual void HitTopEdge(GameTime gt)
        {
            TopEdge = CeilY;
        }
        protected virtual void OffTopEdge(GameTime gt)
        {
            //destroy self
        }
        public virtual void Update(GameTime gameTime)
        {
            if (!Alive) { return; }
            currentAnimation.Tick();
            bool onLeft = IsOnLeftEdge(gameTime);
            bool onRight = IsOnRightEdge(gameTime);
            bool onTop = IsOnTopEdge(gameTime);
            bool onBottom = IsOnBottomEdge(gameTime);
            if (velocity.X != 0)
            {
                if (!onLeft && !onRight)
                {
                    MoveInX(gameTime);
                }
                else if (onRight)
                {
                    if (!StopAtRightEdge(gameTime))
                    {
                        MoveInX(gameTime);
                    }
                    HitRightEdge(gameTime);
                    if (IsPastRightEdge(gameTime))
                    {
                        OffRightEdge(gameTime);
                    }
                }
                else if (onLeft)
                {
                    if (!StopAtLeftEdge(gameTime))
                    {
                        MoveInX(gameTime);
                    }
                    HitLeftEdge(gameTime);
                    if (IsPastLeftEdge(gameTime))
                    {
                        OffLeftEdge(gameTime);
                    }
                }
            }
            if (velocity.Y != 0)
            {
                if (!onBottom && !onTop)
                {
                    MoveInY(gameTime);
                }
                else if (onBottom)
                {
                    if (!StopAtBottomEdge(gameTime))
                    {
                        MoveInY(gameTime);
                    }
                    HitBottomEdge(gameTime);
                    if (IsPastBottomEdge(gameTime))
                    {
                        OffBottomEdge(gameTime);
                    }
                }
                else if (onTop)
                {
                    if (!StopAtTopEdge(gameTime))
                    {
                        MoveInY(gameTime);
                    }
                    HitTopEdge(gameTime);
                    if (IsPastTopEdge(gameTime))
                    {
                        OffTopEdge(gameTime);
                    }
                }
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (!Visible) { return; }
            Rectangle srcRect = new Rectangle(XForSprite(currentAnimation.CurrentFrame), 0, SpriteWidth, spriteSheet.Height);
            //modify srcRect.X for animation frame
            Rectangle dstRect = new Rectangle();
            //modify dstRect.X, .Y for position, viewport
            dstRect.X = (int)(position.X*ScaleFactor);
            dstRect.Y = (int)(position.Y*ScaleFactor);
            dstRect.Width = (int)(srcRect.Width*ScaleFactor);
            dstRect.Height = (int)(srcRect.Height*ScaleFactor);
            SpriteBatch.Draw(spriteSheet, dstRect, srcRect, Color.White);
        }
    }
}