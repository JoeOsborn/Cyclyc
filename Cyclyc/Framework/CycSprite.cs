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
        protected Rectangle bounds;
        public Vector2 position;
        public Vector2 velocity;
        

        public virtual string AssetName
        {
            get { return "placeholder"; }
        }

        protected SpriteBatch SpriteBatch
        {
            get { return Game.SpriteBatch; }
        }
        
        public CycSprite(Game1 game)
        {
            Game = game;
            bounds = new Rectangle(0, 0, 8, 8);
            position = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
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
            if ((RightEdge + velocity.X) < RightX && (LeftEdge + velocity.X) > LeftX)
            {
                MoveInX(gameTime);
            }
            else if ((RightEdge + velocity.X) > RightX)
            {
                if (!StopAtRightEdge(gameTime))
                {
                    MoveInX(gameTime);
                }
                HitRightEdge(gameTime);
                if (LeftEdge > RightX)
                {
                    OffRightEdge(gameTime);
                }
            }
            else if ((LeftEdge + velocity.X) < LeftX)
            {
                if (!StopAtLeftEdge(gameTime))
                {
                    MoveInX(gameTime);
                }
                HitLeftEdge(gameTime);
                if (RightEdge < LeftX)
                {
                    OffLeftEdge(gameTime);
                }
            }
            if ((TopEdge + velocity.Y) > CeilY && (BottomEdge + velocity.Y) < FloorY)
            {
                MoveInY(gameTime);
            }
            else if ((BottomEdge + velocity.Y) > FloorY)
            {
                if (!StopAtBottomEdge(gameTime))
                {
                    MoveInY(gameTime);
                }
                HitBottomEdge(gameTime);
                if (TopEdge > FloorY)
                {
                    OffBottomEdge(gameTime);
                }
            }
            else if ((TopEdge + velocity.Y) < CeilY)
            {
                if (!StopAtTopEdge(gameTime))
                {
                    MoveInY(gameTime);
                }
                HitTopEdge(gameTime);
                if (BottomEdge < CeilY)
                {
                    OffTopEdge(gameTime);
                }
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            Viewport defaultView = GraphicsDevice.Viewport;
            Viewport newView = new Viewport();
            newView.X = (int)(view.X * ScaleFactor);
            newView.Y = (int)(view.Y * ScaleFactor);
            newView.Width = (int)(view.Width * ScaleFactor);
            newView.Height = (int)(view.Height * ScaleFactor);
            GraphicsDevice.Viewport = newView;
            Rectangle srcRect = bounds;
            //modify srcRect.X for animation frame
            Rectangle dstRect = bounds;
            //modify dstRect.X, .Y for position, viewport
            dstRect.X = (int)(position.X*ScaleFactor) + newView.X;
            dstRect.Y = (int)(position.Y*ScaleFactor) + newView.Y;
            dstRect.Width = (int)(dstRect.Width*ScaleFactor);
            dstRect.Height = (int)(dstRect.Height*ScaleFactor);
            SpriteBatch.Draw(spriteSheet, dstRect, srcRect, Color.White);
            GraphicsDevice.Viewport = defaultView;
        }
    }
}