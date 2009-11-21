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
    public class RobotEnemyPool : JetpackEnemyPool
    {
        public RobotEnemyPool(CycGame game)
            : base(game)
        {

        }

        public override CycSprite MakeEnemy()
        {
            return new RobotEnemy(CycGame.Game, this);
        }
    }
}
