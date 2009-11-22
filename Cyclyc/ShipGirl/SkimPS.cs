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
    public class SkimPS : ParticleSystem
    {
        public SkimPS(Game1 game)
            : base(game, 32)
        {

        }
        protected override void InitializeConstants()
        {
            textureFilename = "skimRing particle";

            minInitialSpeed = 0.5f;
            maxInitialSpeed = 1.5f;

            minAcceleration = -0.05f;
            maxAcceleration = 0.05f;

            minLifetime = 0.1f;
            maxLifetime = 0.25f;

            minScale = 0.4f;
            maxScale = 1.0f;

            minNumParticles = 0;
            maxNumParticles = 2;

            minRotationSpeed = 0;
            maxRotationSpeed = 0.05f;

            spriteBlendMode = SpriteBlendMode.AlphaBlend;

            minRed = 1; maxRed = 1;
            minGreen = 1; maxGreen = 1;
            minBlue = 1; maxBlue = 1;

            DrawOrder = AlphaBlendDrawOrder;
        }
    }
}
