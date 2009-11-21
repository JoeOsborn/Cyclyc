
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
    public class ShipEnemyPool : EnemyPool
    {
        public ShipEnemyPool(CycGame g) : base(g)
        {
        }
        public override CycSprite MakeEnemy()
        {
            return new ShipEnemy(CycGame.Game, this);
        }
        public CycSprite Create(Challenge c, string img, int fc, CollisionStyle col, string curveSet, bool left, int y, int w, int h, double timeScale, int diff)
        {
            ShipEnemy enemy = (ShipEnemy)FindOrMakeEnemy();
            enemy.Reset(c, img, fc, col, curveSet, left, left ? 0 : CycGame.View.Width, y, w, h, timeScale, diff);
            return enemy;
        }
    }
}
