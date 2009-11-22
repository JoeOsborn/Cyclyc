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
    public class JetpackGirlPS : ParticleSystem
    {
        public JetpackGirlPS(Game1 game)
            : base(game, 8)
        {

        }
        protected override void InitializeConstants()
        {
            textureFilename = "wrench";

            // less initial speed than the explosion itself
            minInitialSpeed = 0.5f;
            maxInitialSpeed = 2.0f;

            // acceleration is negative, so particles will accelerate away from the
            // initial velocity.  this will make them slow down, as if from wind
            // resistance. we want the smoke to linger a bit and feel wispy, though,
            // so we don't stop them completely like we do ExplosionParticleSystem
            // particles.
            minAcceleration = 0;
            maxAcceleration = 0;

            // explosion smoke lasts for longer than the explosion itself, but not
            // as long as the plumes do.
            minLifetime = 0.25f;
            maxLifetime = 0.35f;

            minScale = 0.5f;
            maxScale = 1.5f;

            minNumParticles = 0;
            maxNumParticles = 2;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            spriteBlendMode = SpriteBlendMode.AlphaBlend;

            ColorBlend(0.0f);

            DrawOrder = AlphaBlendDrawOrder;
        }
        public void ColorBlend(float ratio)
        {
            Color red = Color.Red;
            Color smoke = Color.Gray;

            Color current = Color.Lerp(red, smoke, ratio);

            minRed = current.R;
            maxRed = minRed;

            minGreen = current.G;
            maxGreen = minGreen;

            minBlue = current.B;
            maxBlue = minBlue;
        }
    }
}
