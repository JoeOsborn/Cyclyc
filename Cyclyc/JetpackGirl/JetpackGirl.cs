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

namespace Cyclyc.JetpackGirl
{
    public class JetpackGirl : CycSprite, JetpackOwner
    {
        protected CycGame CycGame { get; set; }
        KeyboardState kb, oldKB;
        GamePadState gp, oldGP;
        protected bool jumpReleased;
        public bool Attacking { get; set; }
        protected double attackCounter;
        protected double attackCooldown;
        protected Jetpack jetpack;
        protected double respawnTimer;
        public CycSprite Wrench { get; set; }

        protected JetpackGirlPS particles;

        public bool Dying
        {
            get;
            set;
        }

        public JetpackGirl(Game1 game, CycGame cg)
            : base(game)
        {
            CycGame = cg;
            Dying = false;
            AttackRadius = 10;
            particles = new JetpackGirlPS(Game);
            Game.Components.Add(particles);
            ScaleFactor = 2.0f;
            assetName = "rockGirl";
            collisionStyle = CollisionStyle.Box;
            jetpack = new Jetpack(this);
            spriteWidth = 14;
            jumpReleased = true;
            attackCounter = 0;
            attackCooldown = 0;
            Attacking = false;
            bounds = new Rectangle(0, 0, 14, 16);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Die()
        {
            if (Dying) { return; }
            Dying = true;
            Play("death", false);
            Attacking = false;
            attackCooldown = 0;
            attackCounter = 0;
            Wrench.Visible = false;
            Wrench.Alive = false;
            FlipImage = false;
            jumpReleased = true;
            respawnTimer = RespawnDelay;
            jetpack.JPFuel = jetpack.MaxJPFuel;
            velocity.Y = -jetpack.Gravity*4;
        }

        public override void LoadContent()
        {
            int[] timings = TimingSequence(5, 2);
            animations["default"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["death"] =
                new Animation(FrameSequence(8, 1), TimingSequence(1,1), false);
            animations["run"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["run-attacking"] = 
                new Animation(FrameSequence(4, 2), timings, true);
            animations["jet"] = 
                new Animation(FrameSequence(2, 2), timings, true);
            animations["jet-attacking"] = 
                new Animation(FrameSequence(6, 2), timings, true);
            animations["begin-jet"] = 
                new Animation(FrameSequence(2, 2), timings, true);
            animations["begin-jet-attacking"] = 
                new Animation(FrameSequence(6, 2), timings, true);
            animations["stop-jet"] = 
                new Animation(FrameSequence(2, 2), timings, true);
            animations["stop-jet-attacking"] = 
                new Animation(FrameSequence(6, 2), timings, true);
            animations["jump"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["jump-attacking"] = 
                new Animation(FrameSequence(4, 2), timings, true);
            animations["fall"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["fall-attacking"] = 
                new Animation(FrameSequence(4, 2), timings, true);
            animations["land"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["land-attacking"] = 
                new Animation(FrameSequence(4, 2), timings, true);
            animations["run"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["run-attacking"] = 
                new Animation(FrameSequence(4, 2), timings, true);
            Play("default");

            Wrench = new CycSprite(Game);
            Wrench.Initialize();
            Wrench.AddAnimation("default", new int[] { 0, 1 }, new int[] { 5, 5 }, true);
            Wrench.Play("default");
            Wrench.AssetName = "wrench";
            Wrench.Visible = false;
            Wrench.Alive = false;
            Wrench.CollisionStyle = CollisionStyle.Circle;
            Wrench.SpriteWidth = 14;
            Wrench.ScaleFactor = 2.0f;
            Wrench.Radius = 14;
            Wrench.VisualRadius = 14;
            Wrench.LoadContent();
            base.LoadContent();
        }

        public Vector2 StartPosition { get; set; }

        public void BeginJet()
        {
            //later, might have 'begin jet' anims
            if (Dying) { Play("death", false); return;  }
            else if (Attacking)
            {
                Play("begin-jet-attacking", true);
            }
            else
            {
                Play("begin-jet", true);
            }
            //HACK ALERT
        }
        public void MaintainJet()
        {
            if (Dying) { Play("death", false); }
            else if (Attacking)
            {
                Play("jet-attacking", false);
            }
            else
            {
                Play("jet", false);
            }
            particles.SetFuelRatio(jetpack.JPFuel / jetpack.MaxJPFuel);
            particles.AddParticles(new Vector2((position.X + View.X) * ScaleFactor, (position.Y + 8 + View.Y) * ScaleFactor));
        }
        public void FizzleJet()
        {
            //later, have a fizzled jet animation
            if (Dying) { Play("death", false); }
            else if (Attacking)
            {
                Play("fizzle-jet-attacking", false);
            }
            else
            {
                Play("fizzle-jet", false);
            }
        }
        public void StopJet()
        {
            if (Dying) { Play("death", false); }
            else if (Attacking)
            {
                Play("stop-jet-attacking", true);
            }
            else
            {
                Play("stop-jet", true);
            }
        }
        public void Jump()
        {
            jumpReleased = false;
            if (Dying) { Play("death", false); }
            else if (Attacking)
            {
                Play("jump-attacking", false);
            }
            else
            {
                Play("jump", false);
            }
        }
        public void Fall()
        {
            if (Dying) { Play("death", false); }
            else if (Attacking)
            {
                Play("fall-attacking", false);
            }
            else
            {
                Play("fall", false);
            }
        }
        public void Land()
        {
            if (Dying) { Play("death", false); }
            else if (Attacking)
            {
                Play("land-attacking", true);
            }
            else
            {
                Play("land", true);
            }
        }
        public void Run()
        {
            if (Dying) { Play("death", false); }
            else if (Attacking)
            {
                Play("run-attacking", false);
            }
            else
            {
                Play("run", false);
            }
        }
        protected double RespawnDelay
        {
            get { return 1.0; }
        }
        protected double AttackCooldownDuration
        {
            get { return 0.1; }
        }
        protected double AttackDuration
        {
            get { return 0.4; }
        }
        protected double AttackRatio
        {
            get { return attackCounter / AttackDuration; }
        }
        protected double AttackRadius
        {
            get;
            set;
        }
        protected double AttackRadiusDefault
        {
            get { return 10.0; }
        }
        protected double AttackRadiusBonus
        {
            get { return 20.0; }
        }
        public bool IsInAir
        {
            get { return BottomEdge < FloorY; }
        }
        public bool FallingThroughGround
        {
            get { return Dying ? false : (OnGround && velocity.Y > 0); }
        }
        public bool OnGround
        {
            get { return BottomEdge == FloorY; }
        }
        public bool PlayerWantRight
        {
            get { return kb.IsKeyDown(Keys.D) || (gp.ThumbSticks.Left.X > 0); }
        }
        public bool oldPlayerWantRight
        {
            get { return oldKB.IsKeyDown(Keys.D) || (oldGP.ThumbSticks.Left.X > 0); }
        }
        public bool PlayerWantLeft
        {
            get { return kb.IsKeyDown(Keys.A) || (gp.ThumbSticks.Left.X < 0); }
        }
        public bool oldPlayerWantLeft
        {
            get { return oldKB.IsKeyDown(Keys.A) || (oldGP.ThumbSticks.Left.X < 0); }
        }
        public bool PlayerWantUp
        {
            get { return kb.IsKeyDown(Keys.W) || (gp.Buttons.A == ButtonState.Pressed); }
        }
        public bool oldPlayerWantUp
        {
            get { return oldKB.IsKeyDown(Keys.W) || (oldGP.Buttons.A == ButtonState.Pressed); }
        }
        public bool PlayerWantAttack
        {
            get { return kb.IsKeyDown(Keys.Q) || (gp.Buttons.X == ButtonState.Pressed); }
        }
        public bool oldPlayerWantAttack
        {
            get { return oldKB.IsKeyDown(Keys.Q) || (oldGP.Buttons.X == ButtonState.Pressed); }
        }
        public bool ShouldMoveRight
        {
            get { return !Dying && PlayerWantRight; }
        }
        public bool ShouldMoveLeft
        {
            get { return !Dying && PlayerWantLeft; }
        }
        public bool ShouldJet
        {
            get { return !Dying && jumpReleased && PlayerWantUp; }
        }
        public bool ShouldJump
        {
            get { return !Dying && !oldPlayerWantUp && PlayerWantUp; }
        }
        protected override bool StopAtBottomEdge(GameTime gt)
        {
            return Dying ? false : true;
        }
        protected override void HitBottomEdge(GameTime gt)
        {
            if (Dying)
            {
                //nop
            }
            else
            {
                BottomEdge = FloorY;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Dying)
            {
                respawnTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (respawnTimer >= 0)
                {
                    jetpack.Update(gameTime);
                    base.Update(gameTime);
                    return;
                }
                Dying = false;
                position = StartPosition;
            }
            //SUPER SAIYAN Jetpack Girl
            if (CycGame.Grade >= 2)
            {
                AttackRadius = AttackRadiusBonus;
            }
            else
            {
                AttackRadius = AttackRadiusDefault;
            }
            
            //INPUT
            oldKB = kb;
            oldGP = gp;
            kb = Keyboard.GetState();
            gp = GamePad.GetState(PlayerIndex.Two);
            if (!jumpReleased && oldPlayerWantUp && !PlayerWantUp)
            {
                jumpReleased = true;
            }
            if (!Attacking && attackCooldown > 0)
            {
                attackCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
                if (attackCooldown <= 0)
                {
                    attackCooldown = 0;
                }
            }
            if (attackCooldown == 0 && attackCounter <= 0 && !Attacking)
            {
                if (PlayerWantAttack && !oldPlayerWantAttack)
                {
                    attackCounter = AttackDuration;
                    Attacking = true;
                }
            } 

            if (Attacking && attackCounter > 0)
            {
                Wrench.Visible = true;
                Wrench.Alive = true;
                double ratio = 1-AttackRatio;
                double angle = ratio * 2*Math.PI - Math.PI/2.0;
                //ratio of rotation from circle starting at 0 = 0 degrees, .25 = 90 degrees (flip), .5 = 180 degrees, .75 = 270 degrees (flip back), 1.0 = 0 degrees
                double r = AttackRadius;
                Wrench.Position = new Vector2((float)(Center.X-2 + r * Math.Cos(angle)), (float)(Center.Y + r * Math.Sin(angle)));
                if (angle > (Math.PI/2.0) && angle < (3.0*Math.PI/2.0)) { FlipImage = true; }
                else { FlipImage = false; }
                attackCounter -= gameTime.ElapsedGameTime.TotalSeconds;
                Attacking = true;
            }
            //attack cooldown?
            if (Attacking && attackCounter <= 0)
            {
                attackCooldown = AttackCooldownDuration;
                Attacking = false;
                Wrench.Visible = false;
                Wrench.Alive = false;
                FlipImage = false;
                attackCounter = 0;
                attackCooldown = AttackCooldownDuration;
            }
            Wrench.Update(gameTime);
            jetpack.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Wrench.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}