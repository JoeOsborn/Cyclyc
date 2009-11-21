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
    public class HoverEnemyPool : JetpackEnemyPool
    {
        public HoverEnemyPool(CycGame game)
            : base(game)
        {

        }

        public override CycEnemy MakeEnemy()
        {
            return new HoverEnemy(CycGame.Game, this);
        }
    }
}
