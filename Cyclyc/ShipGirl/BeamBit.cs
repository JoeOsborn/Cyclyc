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

namespace Cyclyc.ShipGirl
{
    public class BeamBit : CycSprite
    {
        SoundEffectInstance shotSnd;
        protected BeamPool Pool { get; set; }
        public BeamBit(Game1 game, BeamPool pool)
            : base(game)
        {
            Pool = pool;
            AssetName = "shipBullet";
            AddAnimation("shot", FrameSequence(0, 1), TimingSequence(5, 1), true);
        }

        public override void LoadContent()
        {
            SpriteWidth = 12;
            if (shotSnd == null)
            {
                shotSnd = Game.SoundInstance("space-shot");
            }
            base.LoadContent();
        }

        protected override bool StopAtRightEdge(GameTime gt)
        {
            return false;
        }
        protected override bool StopAtLeftEdge(GameTime gt)
        {
            return false;
        }

        protected override void HitLeftEdge(GameTime gt)
        {
            //nop
        }
        protected override void HitRightEdge(GameTime gt)
        {
            //nop
        }

        protected override bool StopAtTopEdge(GameTime gt)
        {
            return false;
        }
        protected override bool StopAtBottomEdge(GameTime gt)
        {
            return false;
        }

        protected override void HitTopEdge(GameTime gt)
        {
            //nop
        }
        protected override void HitBottomEdge(GameTime gt)
        {
            //nop
        }

        public override void Update(GameTime gt)
        {
            if (Alive && (IsPastBottomEdge(gt) || IsPastTopEdge(gt) || IsPastLeftEdge(gt) || IsPastRightEdge(gt)))
            {
                alive = false;
                visible = false;
            }
            base.Update(gt);
        }

        public void Die()
        {
            alive = false;
            visible = false;
            shotSnd.Stop();
        }

        public void Reset(float x, float y, float vx, float vy)
        {
            Game.PlayIfNotPlaying(shotSnd);
            //orient to the direction of motion
            Rotation = (float)(Math.Atan2(vy, vx));
            Velocity = new Vector2(vx, vy);
            Position = new Vector2(x, y);
            CollisionStyle = CollisionStyle.Circle;
            SpriteWidth = spriteSheet.Width;
            Play("shot");
            VisualRadius = 14;
            VisualWidth = 14;
            VisualHeight = 12;
            Radius = 14;
            alive = true;
            visible = true;
        }


    }
}
