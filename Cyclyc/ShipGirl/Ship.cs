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
    public class Ship : CycSprite
    {
        protected KeyboardState kb;

        protected Vector2 LastInputVelocity { get; set; }

        public bool Dying { get; set; }
        protected double respawnTimer;
        protected double RespawnDelay
        {
            get { return 1.0; }
        }

        protected float ShotCooldown { get; set; }

        Random rgen;

        public BeamPool CrushPool { get; set; }

        public Ship(Game1 game)
            : base(game)
        {
            rgen = new Random();
            assetName = "shipGirl";
            respawnTimer = 0;
            Dying = false;
            collisionStyle = CollisionStyle.Circle;
            AddAnimation("death", new int[] { 0 }, new int[] { 5 }, true);
            LastInputVelocity = new Vector2(-1, 0);
            Rotation = (float)Math.PI;
            TargetRotation = Rotation;
        }

        protected float TargetRotation { get; set; }

        public Vector2 StartPosition { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            spriteWidth = spriteSheet.Width;
            VisualWidth = spriteWidth/2;
            VisualHeight = spriteSheet.Height/2;
            Radius = 15;
            CrushPower = 0;
        }

        protected float MaxSpeedX
        {
            get { return 2.5f; }
        }
        protected float MaxSpeedY
        {
            get { return 2.5f; }
        }

        public float CrushPower { get; set; }
        public float CrushMaxPower { get { return 30.0f; } }
        public float CrushPowerUpRate
        {
            get { return 0.5f; }
        }
        public float CrushPowerDownRate
        {
            get { return 1.5f; }
        }

        public void Skim(int enemyCount)
        {
            CrushPower = (float)Math.Min(CrushMaxPower, CrushPower+CrushPowerUpRate * enemyCount);
        }

        protected float ShotCooldownMax
        {
            get
            {
                if (CrushPower > (3 * CrushMaxPower / 4.0))
                {
                    return 0.1f;
                }
                else if (CrushPower > (CrushMaxPower / 2.0))
                {
                    return 0.5f;
                }
                else
                {
                    return 0.8f;
                }
            }
        }
        protected float ShotMaxSpeed
        {
            get { return 6.0f; }
        }
        protected float ShotMinSpeed
        {
            get { return 5.0f; }
        }
        protected float MaxRotationVariance
        {
            get { return (float)(Math.PI / 64); }
        }
        public float PowerRatio
        {
            get { return ((float)Math.Max(CrushPower, 0.05) / CrushMaxPower); }
        }

        protected void FireShot()
        {
            //magnitude
            float ratio = PowerRatio;
            float mag = (float)(ratio * (ShotMaxSpeed-ShotMinSpeed) * rgen.NextDouble() + ShotMinSpeed);
            float rotVar = (MaxRotationVariance * ratio);
            float dir = (float)(Rotation + ((rotVar * 2 * rgen.NextDouble()) - rotVar));
            Vector2 vel = new Vector2((float)(Math.Cos(dir) * mag), (float)(Math.Sin(dir) * mag));
            Vector2 pos = Position + new Vector2((float)(Radius * Math.Cos(dir)), (float)(Radius * Math.Sin(dir)));
            CrushPool.Create(pos.X, pos.Y, vel.X, vel.Y);
        }

        protected void Crush(GameTime gt)
        {
            if (ShotCooldown <= 0)
            {
                FireShot();
                ShotCooldown = ShotCooldownMax;
            }
            CrushPower = Math.Max(CrushPower - CrushPowerDownRate, 0);
        }

        public void Die()
        {
            Dying = true;
            respawnTimer = RespawnDelay;
            CrushPower = 0;
            ShotCooldown = 0;
            Rotation = (float)Math.PI;
            TargetRotation = Rotation; 
            Play("death", false);
        }

        protected float ManualSpeedStep
        {
            get { return 0.25f; }
        }
        protected float InertiaSpeedStep
        {
            get { return 0.125f; }
        }

        public override void Update(GameTime gameTime)
        {
            Console.WriteLine("Crush power: " + CrushPower);
            if (ShotCooldown > ShotCooldownMax)
            {
                ShotCooldown = ShotCooldownMax;
            }
            if (ShotCooldown > 0)
            {
                ShotCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (!Dying && Keyboard.GetState().IsKeyDown(Keys.Space) && ShotCooldown <= 0)
            {
                Crush(gameTime);
            }
            if (Dying)
            {
                respawnTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if(respawnTimer > 0)
                {
                    velocity.X = 0;
                    velocity.Y = 0;
                    base.Update(gameTime);
                    return;
                }
                respawnTimer = 0;
                Dying = false;
                //check to make sure no enemies are here
                position = StartPosition;
            }
            // TODO: Add your update code here
            kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Up) && kb.IsKeyUp(Keys.Down) && TopEdge > CeilY)
            {
                velocity.Y = MathHelper.Clamp(velocity.Y - ManualSpeedStep, -MaxSpeedY, MaxSpeedY);
                LastInputVelocity = Velocity;
            }
            else if (kb.IsKeyDown(Keys.Down) && kb.IsKeyUp(Keys.Up) && BottomEdge < FloorY)
            {
                velocity.Y = MathHelper.Clamp(velocity.Y + ManualSpeedStep, -MaxSpeedY, MaxSpeedY);
                LastInputVelocity = Velocity;
            }
            else
            {
                if (velocity.Y > 0) 
                {
                    velocity.Y = MathHelper.Max(velocity.Y - InertiaSpeedStep, 0);
                }
                else if (velocity.Y < 0) 
                {
                    velocity.Y = MathHelper.Min(velocity.Y + InertiaSpeedStep, 0);
                }
            }
            if (kb.IsKeyDown(Keys.Right) && kb.IsKeyUp(Keys.Left) && RightEdge < RightX)
            {
                velocity.X = MathHelper.Clamp(velocity.X + ManualSpeedStep, -MaxSpeedX, MaxSpeedX);
                LastInputVelocity = Velocity;
            }
            else if (kb.IsKeyDown(Keys.Left) && kb.IsKeyUp(Keys.Right) && LeftEdge > LeftX)
            {
                velocity.X = MathHelper.Clamp(velocity.X - ManualSpeedStep, -MaxSpeedX, MaxSpeedX);
                LastInputVelocity = Velocity;
            }
            else
            {
                if (velocity.X > 0)
                {
                    velocity.X = MathHelper.Max(velocity.X - InertiaSpeedStep, 0);
                }
                else if (velocity.X < 0)
                {
                    velocity.X = MathHelper.Min(velocity.X + InertiaSpeedStep, 0);
                }
            }
            if (LastInputVelocity.X != 0 || LastInputVelocity.Y != 0)
            {
                TargetRotation = (float)Math.Atan2(LastInputVelocity.Y, LastInputVelocity.X);
            }
            if (Math.Abs(Rotation - TargetRotation) > float.Epsilon)
            {
                double turnAmount = Math.Atan2(Math.Sin(TargetRotation - Rotation), Math.Cos(TargetRotation - Rotation));
                Rotation += (float)(turnAmount * 0.1);
            }
            base.Update(gameTime);
        }

        protected override Color DrawColor(GameTime gt)
        {
            return Color.Lerp(Color.White, Color.Red, PowerRatio);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}