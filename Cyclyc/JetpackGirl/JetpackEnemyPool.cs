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
    public class JetpackEnemyPool : EnemyPool
    {
        Random rgen;
        public JetpackEnemyPool(CycGame g)
            : base(g)
        {
            rgen = new Random();
        }

        public override CycEnemy MakeEnemy()
        {
            return new JetpackEnemy(CycGame.Game, this);
        }

        public JetpackEnemy Create(Challenge c, string img, int fc, CollisionStyle col, bool left, int y, int w, int h, float speed)
        {
            JetpackEnemy enemy = (JetpackEnemy)FindOrMakeEnemy();
            enemy.Reset(c, img, fc, col, left, (int)(left ? (0 - (rgen.NextDouble() * 32)) : (400 + (rgen.NextDouble()*32))), y, w, h, speed);
            return enemy;
        }
    }
}
