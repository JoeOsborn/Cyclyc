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

namespace Cyclyc.JetpackGirl
{
    public class Jetpack
    {
        JetpackOwner owner;
        public bool jetting;
        protected float jpFuel;
        public Jetpack(JetpackOwner o)
        {
            MaxSpeedX = 1.0f;
            DefaultSpeedX = -0.25f;
            MaxSpeedFallY = 1.5f;
            MaxSpeedRiseY = -1.5f;
            MaxJPFuel = 180.0f;
            Gravity = 0.1f;
            JetWipesVelocity = true;
            JetThrust = 0.15f;
            JumpThrust = 3.0f;
            DefuelRate = 1.0f;
            RefuelRate = 0.8f;

            owner = o;
            jetting = false;
            jpFuel = MaxJPFuel;
        }
        public float MaxSpeedX { get; set; }
        public float DefaultSpeedX { get; set; }
        public float MaxSpeedFallY { get; set; }
        public float MaxSpeedRiseY { get; set; }
        public float MaxJPFuel { get; set; }
        public float Gravity { get; set; }
        public bool JetWipesVelocity { get; set; }
        public float JetThrust { get; set; }
        public float JumpThrust { get; set; }
        public float DefuelRate { get; set; }
        public float RefuelRate { get; set; }

        protected void BeginJet()
        {
            jetting = true;
            owner.BeginJet();
        }
        protected void MaintainJet()
        {
            jetting = true;
            owner.MaintainJet();
        }
        protected void FizzleJet()
        {
            jetting = false;
            owner.FizzleJet();
        }
        protected void StopJet()
        {
            jetting = false;
            owner.StopJet();
        }
        protected void Jump()
        {
            owner.Jump();
        }
        protected void Fall()
        {
            owner.Fall();
        }
        protected void Land()
        {
            owner.Land();
        }
        protected void Run()
        {
            owner.Run();
        }

        public void Update(GameTime gt)
        {
            Vector2 newVelocity = owner.Velocity;
            if (owner.ShouldMoveRight)
            {
                newVelocity.X = MaxSpeedX;
            }
            else if (owner.ShouldMoveLeft)
            {
                newVelocity.X = -MaxSpeedX;
            }
            else
            {
                newVelocity.X = DefaultSpeedX;
            }
            if (owner.IsInAir)
            {
                newVelocity.Y = Math.Min(newVelocity.Y + Gravity, MaxSpeedFallY);
                //we're in the air.  do we jetpack?
                if (owner.ShouldJet)
                {
                    if (jpFuel <= 0)
                    {
                        Console.WriteLine("fizzle");
                        FizzleJet();
                    }
                    else
                    {
                        if (JetWipesVelocity && newVelocity.Y > 0) { newVelocity.Y = 0; }
                        newVelocity.Y = Math.Max(newVelocity.Y - JetThrust, MaxSpeedRiseY);
                        jpFuel -= DefuelRate;
                        if (!jetting)
                        {
                            Console.WriteLine("start jet");
                            BeginJet();
                        }
                        else
                        {
                            Console.WriteLine("jet");
                            MaintainJet();
                        }
                    }
                }
                else
                {
                    if (jetting)
                    {
                        Console.WriteLine("stop jet");
                        StopJet();
                    }
                    if (newVelocity.Y > 0)
                    {
                        Console.WriteLine("falling");
                        Fall();
                    }
                }
            }
            else if (owner.FallingThroughGround)
            {
                owner.BottomEdge = owner.FloorY;
                newVelocity.Y = 0;
                Console.WriteLine("landing");
                Land();
            }
            if (owner.OnGround)
            {
                jpFuel = Math.Min(jpFuel + RefuelRate, MaxJPFuel);
                //do we jump?
                if (owner.ShouldJump)
                {
                    Console.WriteLine("jump");
                    if (JetWipesVelocity)
                    {
                        newVelocity.Y = -JumpThrust;
                    }
                    else
                    {
                        newVelocity.Y -= JumpThrust;
                    }
                    Jump();
                }
                else
                {
                    Console.WriteLine("jog");
                    Run();
                }
            }
            owner.Velocity = newVelocity;
        }
    }
}
