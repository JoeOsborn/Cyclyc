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
    class SpiderEnemy : JetpackEnemy
    {
        public SpiderEnemy(Game1 game, EnemyPool p)
            : base(game, p)
        {

        }
        public override bool ShouldMoveLeft
        {
            get
            {
                if (CloseToTarget)
                {
                    if (Math.Abs(TargetDistance) < 8)
                    {
                        //are we close and already moving left?  let's keep going
                        return Velocity.X < 0 ? false : true;
                    }
                    return !TargetIsLeft;
                }
                else
                {
                    return leftToRight;
                }
            }
        }
        public override bool ShouldMoveRight
        {
            get
            {
                if (CloseToTarget)
                {
                    if (Math.Abs(TargetDistance) < 8)
                    {
                        //are we close and already moving right?  let's keep going
                        return Velocity.X < 0 ? true : false;
                    }
                    return TargetIsLeft;
                }
                else
                {
                    return !leftToRight;
                }
            }
        }
    }
}
