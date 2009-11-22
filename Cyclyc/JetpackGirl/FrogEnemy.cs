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
    class FrogEnemy : JetpackEnemy
    {
        public FrogEnemy(Game1 game, EnemyPool p)
            : base(game, p)
        {
        }
        public override bool ShouldJet
        {
            get
            {
                return false;
            }
        }
        public override bool ShouldJump
        {
            get
            {
                return !IsHit;
            }
        }
        protected override void LoadAnimations()
        {
            //these are wrong, figure out a nice parameterizable way to avoid a lot of anim duplication?
            int[] timings = TimingSequence(8, frameCount);
            animations["default"] =
                new Animation(FrameSequence(0, 1), timings, false);
            animations["run"] =
                new Animation(FrameSequence(0, 1), timings, false);
            animations["jet"] =
                new Animation(FrameSequence(1, 1), timings, false);
            animations["begin-jet"] =
                new Animation(FrameSequence(1, 1), timings, false);
            animations["stop-jet"] =
                new Animation(FrameSequence(1, 1), timings, false);
            animations["jump"] =
                new Animation(FrameSequence(0, 2), timings, false);
            animations["fall"] =
                new Animation(FrameSequence(1, 1), timings, false);
            animations["land"] =
                new Animation(FrameSequence(0, 1), timings, false);
            animations["run"] =
                new Animation(FrameSequence(0, 1), timings, false);
        }
    }
}
