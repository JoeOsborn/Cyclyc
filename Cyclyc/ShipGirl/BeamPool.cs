
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
    public class BeamPool : EnemyPool
    {
        public BeamPool(CycGame g)
            : base(g)
        {

        }

        public override CycSprite MakeEnemy()
        {
            return new BeamBit(CycGame.Game, this);
        }
        public BeamBit Create(string img, float x, float y, float vx, float vy, float sz)
        {
            BeamBit bit = (BeamBit)FindOrMakeEnemy();
            bit.Initialize();
            bit.LoadContent();
            bit.Reset(img, x, y, vx, vy, sz);
            return bit;
        }
    }
}
