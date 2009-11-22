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
        protected Game1 Game;

        #region internal drawing requirements
        protected GraphicsDevice GraphicsDevice
        {
            get { return Game.GraphicsDevice; }
        }
        protected SpriteBatch SpriteBatch
        {
            get { return Game.SpriteBatch; }
        }
        #endregion
        #region boundary conveniences
        public float TopEdge
        {
            get 
            { 
                return collisionStyle == CollisionStyle.Circle ? position.Y - radius : position.Y; 
            }
            set
            {
                if (collisionStyle == CollisionStyle.Circle) { position.Y = value + radius; }
                else { position.Y = value; }
            }
        }
        public float BottomEdge
        {
            get { return TopEdge + bounds.Y + bounds.Height; }
            set { TopEdge = (value - bounds.Y - bounds.Height); }
        }
        public float LeftEdge
        {
            get
            {
                return collisionStyle == CollisionStyle.Circle ? position.X - radius : position.X;
            }
            set
            {
                if (collisionStyle == CollisionStyle.Circle) { position.X = value + radius; }
                else { position.X = value; }
            }
        }
        public float RightEdge
        {
            get { return LeftEdge + bounds.X + bounds.Width; }
            set { LeftEdge = (value - bounds.X - bounds.Width); }
        }
        public Vector2 Center
        {
            get
            {
                return collisionStyle == CollisionStyle.Circle ? position :
                    new Vector2(position.X + bounds.X + bounds.Width / 2.0f, position.Y + bounds.Y + bounds.Height / 2.0f);
            }
        }
        #endregion

        //set the ones in these groups as needed
        #region liveness
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
        #endregion
        #region viewport voodoo
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

        public float FloorY
        {
            get { return view.Height; }
        }
        public float CeilY
        {
            get { return 0; }
        }
        public float LeftX
        {
            get { return 0; }
        }
        public float RightX
        {
            get { return view.Width; }
        }
        #endregion
        //scaleFactor is not just for visuals; it also scales the logical coordinate system of this sprite.
        #region logical placement & sizing
        public float ScaleFactor { get; set; }

        protected Rectangle bounds;
        public Rectangle Bounds
        {
            get { return bounds; }
        }
        protected float radius;
        public float Radius
        {
            get { return radius; }
            set { radius = value; bounds.Width = (int)value * 2; bounds.Height = (int)value * 2; }
        }
        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        protected CollisionStyle collisionStyle;
        public CollisionStyle CollisionStyle
        {
            get { return collisionStyle; }
            set { collisionStyle = value; }
        }
        #endregion
        //assetName should be set before LoadContent is called.
        #region visual assets
        protected Texture2D spriteSheet;
        protected Animation currentAnimation;
        protected Dictionary<string, Animation> animations;
        protected string assetName;
        public virtual string AssetName
        {
            get { return assetName; }
            set { assetName = value; }
        }
        protected int spriteWidth;
        public virtual int SpriteWidth
        {
            get { return spriteWidth; }
            set { spriteWidth = value; }
        }
        protected virtual int XForSprite(int i)
        {
            return SpriteWidth * i;
        }
        protected int[] FrameSequence(int start, int length)
        {
            int[] ret = new int[length];
            for (int i = start; i < start + length; i++)
            {
                ret[i-start] = i;
            }
            return ret;
        }
        protected int[] TimingSequence(int fc, int length)
        {
            int[] ret = new int[length];
            for (int i = 0; i < length; i++)
            {
                ret[i] = fc;
            }
            return ret;
        }
        #endregion
        #region purely visual scaling
        protected float visualWidth;
        public virtual float VisualWidth
        {
            get { return visualWidth; }
            set { visualWidth = value; }
        }
        protected float visualHeight;
        public virtual float VisualHeight
        {
            get { return visualHeight; }
            set { visualHeight = value; }
        }
        public virtual float VisualRadius
        {
            get { return visualWidth/2; }
            set { visualWidth = value*2; visualHeight = value*2; }
        }
        protected virtual bool FlipImage { get; set; }
        public float Rotation { get; set; }
        #endregion

        public CycSprite(Game1 game)
        {
            Game = game;
            Rotation = 0.0f;
            ScaleFactor = 1.0f;
            assetName = "placeholder";
            collisionStyle = CollisionStyle.Box;
            alive = true;
            visible = true;
            bounds = new Rectangle(0, 0, 8, 8);
            position = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            currentAnimation = null;
            animations = new Dictionary<string, Animation>();
            AddAnimation("default", new int[]{0}, new int[]{0}, false);
            Play("default");
        }
        public virtual void Initialize()
        {

        }

        public virtual void LoadContent()
        {
            spriteSheet = Game.Content.Load<Texture2D>(AssetName);
            visualWidth = SpriteWidth;
            visualHeight = spriteSheet.Height;
        }

        public void AddAnimation(string name, int[] frames, int[] timings, bool loop)
        {
            animations[name] = new Animation(frames, timings, loop);
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

        #region collision
        protected float CircleLineDistance(Vector2 p3, Vector2 p1, Vector2 p2)
        {
            if (p1 == p2) { return float.NaN; } //infinitely small line
            float u = ((p3.X - p1.X) * (p2.X - p1.X) + (p3.Y - p1.Y) * (p2.Y - p1.Y)) / (p2 - p1).LengthSquared();
            Vector2 intersection = new Vector2(p1.X + u * (p2.X - p1.X), p1.Y + u * (p2.Y - p1.Y));
            return (intersection - p3).Length();
        }

        protected bool CollideCircleBox(Vector2 cp, float rad, Vector2 bp, Vector2 bsz)
        {
            float left = bp.X;
            float right = bp.X + bsz.X;
            float top = bp.Y;
            float bottom = bp.Y + bsz.Y;

            if ((cp.X + rad > left) && (cp.X - rad < right) && (cp.Y + rad > top) && (cp.Y - rad < bottom))
            {
                return true;
            }

            //have to do this four times!
            float upperDist = CircleLineDistance(cp, new Vector2(left, top), new Vector2(right, top));
            float rightDist = CircleLineDistance(cp, new Vector2(right, top), new Vector2(right, bottom));
            float lowerDist = CircleLineDistance(cp, new Vector2(left, bottom), new Vector2(right, bottom));
            float leftDist = CircleLineDistance(cp, new Vector2(left, top), new Vector2(left, bottom));
            if (Math.Abs(upperDist) < rad && Math.Abs(rightDist) < rad && Math.Abs(lowerDist) < rad && Math.Abs(upperDist) < rad) { return true; }
            return upperDist < 0 && rightDist < 0 && lowerDist < 0 && leftDist < 0;
        }

        public bool Collide(CycSprite other)
        {
            if (!Alive) { return false; }
            Vector2 myBoxPos = new Vector2(bounds.X+position.X, bounds.Y+position.Y);
            Vector2 myBoxSz = new Vector2(bounds.Width, bounds.Height);
            Vector2 otherBoxPos = new Vector2(other.bounds.X + other.position.X, other.bounds.Y + other.position.Y);
            Vector2 otherBoxSz = new Vector2(other.bounds.Width, other.bounds.Height);

            if (collisionStyle == CollisionStyle.Circle && other.collisionStyle == CollisionStyle.Circle)
            {
                float delta = (other.position - position).Length();
                if (delta < other.Radius || delta < this.Radius)
                {
                    return true;
                }
            }
            else if (collisionStyle == CollisionStyle.Box && other.collisionStyle == CollisionStyle.Box)
            {
              //box-box
                if (myBoxPos.X > (otherBoxPos.X+otherBoxSz.X)) { return false; }
                if ((myBoxPos.X+myBoxSz.X) < otherBoxPos.X) { return false; }
                if ((myBoxPos.Y+myBoxSz.Y) < otherBoxPos.Y) { return false; }
                if (myBoxPos.Y > (otherBoxPos.Y + otherBoxSz.Y)) { return false; }
                return true;
            }
            else if (collisionStyle == CollisionStyle.Box)
            {
                return CollideCircleBox(other.position, other.Radius, myBoxPos, myBoxSz);
            }
            else
            {
                return CollideCircleBox(position, Radius, otherBoxPos, otherBoxSz);
            }
            return false;
        }
        #endregion

        #region movement
        protected virtual void MoveInX(GameTime gt)
        {
            position.X += velocity.X;
        }
        protected virtual void MoveInY(GameTime gt)
        {
            position.Y += velocity.Y;
        }
        #endregion
        #region edge checks and responses
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
        #endregion
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
        protected virtual Color DrawColor(GameTime gt)
        {
            return Color.White;
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (!Visible) { return; }
            Rectangle srcRect = new Rectangle(XForSprite(currentAnimation.CurrentFrame), 0, SpriteWidth, spriteSheet.Height);
            //modify srcRect.X for animation frame
            Rectangle dstRect = new Rectangle();
            //modify dstRect.X, .Y for position
            Vector2 rotOrigin;
            if (collisionStyle == CollisionStyle.Box)
            {
                dstRect.X = (int)(position.X * ScaleFactor);
                dstRect.Y = (int)(position.Y * ScaleFactor);
                rotOrigin = new Vector2(0,0);
            }
            else
            {
                dstRect.X = (int)((position.X * ScaleFactor));
                dstRect.Y = (int)((position.Y * ScaleFactor));
                rotOrigin = new Vector2(SpriteWidth / 2.0f, srcRect.Height / 2.0f);
            }
            dstRect.Width = (int)(VisualWidth * ScaleFactor);
            dstRect.Height = (int)(VisualHeight * ScaleFactor);
            SpriteBatch.Draw(spriteSheet, dstRect, srcRect, DrawColor(gameTime), Rotation, rotOrigin, FlipImage ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}