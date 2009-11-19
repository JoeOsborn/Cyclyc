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
using Cyclyc.JetpackGirl;
using Cyclyc.ShipGirl;

namespace Cyclyc.Framework
{
    public class ChallengeBeat
    {
        protected double beat;
        public double Beat
        {
            get { return beat; }
        }

        protected bool unsent;
        public bool Unsent
        {
            get { return unsent; }
            set { unsent = value; }
        }

        protected EnemyMaker[] enemies;
        public EnemyMaker[] Enemies
        {
            get { return enemies; }
        }

        public ChallengeBeat(double b, EnemyMaker[] es)
        {
            unsent = true;
            beat = b;
            enemies = es;
        }

    }
}
