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
        protected CycGame CycGame { get; set; }
        protected KeyboardState kb;
        protected GamePadState gp;

        protected Vector2 LastInputVelocity { get; set; }

        public bool Dying { get; set; }
        protected double respawnTimer;
        protected double RespawnDelay
        {
            get { return 2.0; }
        }

        protected float ShotCooldown { get; set; }

        Random rgen;

        public BeamPool CrushPool { get; set; }

        protected ShipPS particles;

        SoundEffectInstance skimSnd, deathSnd;

        bool super;

        Texture2D normalTexture, superTexture;

        public Ship(Game1 game, CycGame cg)
            : base(game)
        {
            super = false;
            CycGame = cg;
            particles = new ShipPS(Game);
            Game.Components.Add(particles);
            rgen = new Random();
            assetName = "shipGirl";
            respawnTimer = 0;
            Dying = false;
            collisionStyle = CollisionStyle.Circle;
            AddAnimation("death", new int[] { 1, 2 }, new int[] { 5, 5 }, true);
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
            SpriteWidth = 438 / 3;
            VisualWidth = SpriteWidth / 2;
            VisualHeight = spriteSheet.Height/2;
            Radius = 15;
            CrushPower = 0;
            deathSnd = Game.SoundInstance("space-die");
            skimSnd = Game.SoundInstance("space-skim");
            normalTexture = spriteSheet;
            superTexture = Game.Content.Load<Texture2D>("shipGirlSuper");
        }

        protected float MaxSpeedX
        {
            get { return 3f; }
        }
        protected float MaxSpeedY
        {
            get { return 3f; }
        }

        public float CrushPower { get; set; }
        public float CrushMaxPower { get { return 30.0f; } }
        public float CrushPowerUpRate
        {
            get { return 0.25f; }
        }
        public float CrushPowerDownRate
        {
            get { return 0.5f; }
        }

        public void Skim(int enemyCount)
        {
            CrushPower = (float)Math.Min(CrushMaxPower, CrushPower+CrushPowerUpRate * enemyCount);
            Game.PlayIfNotPlaying(skimSnd);
        }

        protected float ShotCooldownMax
        {
            get
            {
                if (kb.IsKeyDown(Keys.Space))
                {
                    return (float)0.8 * (CrushMaxPower / (CrushMaxPower + 10 * (CrushPower)));
                }
                else
                {
                    return (float)0.8 * (CrushMaxPower / (CrushMaxPower + 10 * (CrushPower * gp.Triggers.Right)));
                }
            }
        }
        //        if ((CrushPower * gp.Triggers.Right) > (7 * CrushMaxPower / 8.0))
        //        {
        //            return 0.05f;
        //        }
        //        else if ((CrushPower * gp.Triggers.Right) > (6 * CrushMaxPower / 8.0))
        //        {
        //            return 0.1f;
        //        }
        //        else if ((CrushPower * gp.Triggers.Right) > (5 * CrushMaxPower / 8.0))
        //        {
        //            return 0.2f;
        //        }
        //        else if ((CrushPower * gp.Triggers.Right) > (4 * CrushMaxPower / 8.0))
        //        {
        //            return 0.35f;
        //        }
        //        else if ((CrushPower * gp.Triggers.Right) > (3 * CrushMaxPower / 8.0))
        //        {
        //            return 0.5f;
        //        }
        //        else if ((CrushPower * gp.Triggers.Right) > (2 * CrushMaxPower / 8.0))
        //        {
        //            return 0.6f;
        //        }
        //        else if ((CrushPower * gp.Triggers.Right) > (1 * CrushMaxPower / 8.0))
        //        {
        //            return 0.7f;
        //        }
        //        else
        //        {
        //            return 0.8f;
        //        }
        //    }
        //}

        protected float ShotMaxSpeed
        {
            get { return super ? 10.0f : 6.0f; }
        }
        protected float ShotMinSpeed
        {
            get { return super ? 7.0f : 5.0f; }
        }
        protected float MaxRotationVariance
        {
            get { return (float)(Math.PI / (super ? 32 : 64)); }
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
            CrushPool.Create(super ? "shipBulletSuper" : "shipBullet", pos.X, pos.Y, vel.X, vel.Y, super ? 20 : 14);
        }

        protected void Crush(GameTime gt)
        {
            if (ShotCooldown <= 0)
            {
                FireShot();
                ShotCooldown = super ? ShotCooldownMax / 2 : ShotCooldownMax;
            }
            //CrushPower = Math.Max(CrushPower - (gp.Triggers.Right * CrushPowerDownRate), 0);
        }

        public void Superize(bool sup)
        {
            super = sup;
            if (super)
            {
                spriteSheet = superTexture;
            }
            else
            {
                spriteSheet = normalTexture;
            }
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
            Game.PlayIfNotPlaying(deathSnd);
        }

        protected float ManualSpeedStep
        {
            get { return 0.25f; }
        }
        protected float AnalogSpeedMultiplier
        {
            get { return 3f; }
        }
        protected float InertiaSpeedStep
        {
            get { return 0.125f; }
        }

        public bool PlayerWantFire
        {
            get { return kb.IsKeyDown(Keys.Space) || (gp.Triggers.Right > 0); }
        }

        public override void Update(GameTime gameTime)
        {
            kb = Keyboard.GetState();
            gp = GamePad.GetState(PlayerIndex.One);
            Console.WriteLine("Crush power: " + CrushPower);
            particles.SetPowerRatio(PowerRatio);

            if (ShotCooldown > ShotCooldownMax)
            {
                ShotCooldown = ShotCooldownMax;
            }
            if (ShotCooldown > 0)
            {
                ShotCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (!Dying && PlayerWantFire && ShotCooldown <= 0)
            {
                Crush(gameTime);
            }
            if (!Dying && PlayerWantFire)
            {
                if (kb.IsKeyDown(Keys.Space))
                {
                    CrushPower = Math.Max(CrushPower - CrushPowerDownRate, 0);
                }
                else
                {
                    CrushPower = Math.Max(CrushPower - (gp.Triggers.Right * CrushPowerDownRate), 0);
                }
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
                Play("default");
                Flicker(3.0f);
                //check to make sure no enemies are here
                position = StartPosition;
                Dying = false;
            }
            // TODO: Add your update code here
            // VERTICAL VELOCITY
            if (kb.IsKeyDown(Keys.Up) && kb.IsKeyUp(Keys.Down) && TopEdge > CeilY)
            {
                velocity.Y = -MaxSpeedY;
                LastInputVelocity = Velocity;
            }
            else if (kb.IsKeyDown(Keys.Down) && kb.IsKeyUp(Keys.Up) && BottomEdge < FloorY)
            {
                velocity.Y = MaxSpeedY;
                LastInputVelocity = Velocity;
            }
            else if ((gp.ThumbSticks.Left.Y > 0 && TopEdge > CeilY) || (gp.ThumbSticks.Left.Y < 0 && BottomEdge < FloorY))
            {
                velocity.Y = -(gp.ThumbSticks.Left.Y * AnalogSpeedMultiplier);
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
            //HORIZONTAL VELOCITY
            if (kb.IsKeyDown(Keys.Right) && kb.IsKeyUp(Keys.Left) && RightEdge < RightX)
            {
                velocity.X = MaxSpeedX;
                LastInputVelocity = Velocity;
            }
            else if (kb.IsKeyDown(Keys.Left) && kb.IsKeyUp(Keys.Right) && LeftEdge > LeftX)
            {
                velocity.X = -MaxSpeedX;
                LastInputVelocity = Velocity;
            }
            else if ((gp.ThumbSticks.Left.X > 0 && RightEdge < RightX) || (gp.ThumbSticks.Left.X < 0 && LeftEdge > LeftX))
            {
                velocity.X = (gp.ThumbSticks.Left.X * AnalogSpeedMultiplier);
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

            //ROTATION
            if (velocity.X != 0 || velocity.Y != 0)
            {
                Vector2 pos = Position - new Vector2((float)(VisualWidth/2 * Math.Cos(Rotation)), (float)(VisualHeight/2 * Math.Sin(Rotation)));
                particles.Rotation = Rotation;
                particles.AddParticles(pos);
            }
            if ((LastInputVelocity.X != 0 && gp.ThumbSticks.Left.X == 0)|| (LastInputVelocity.Y != 0 && gp.ThumbSticks.Left.Y == 0))
            {
                TargetRotation = (float)Math.Atan2(LastInputVelocity.Y, LastInputVelocity.X);
            }
            else if ((LastInputVelocity.X != 0 && gp.ThumbSticks.Left.X != 0) || (LastInputVelocity.Y != 0 && gp.ThumbSticks.Left.Y != 0))
            {
                TargetRotation = (float)Math.Atan2(-(gp.ThumbSticks.Left.Y), gp.ThumbSticks.Left.X);
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