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
            textureFilename = "jetParticleMulti";

            // less initial speed than the explosion itself
            minInitialSpeed = 0.5f;
            maxInitialSpeed = 1.5f;

            // acceleration is negative, so particles will accelerate away from the
            // initial velocity.  this will make them slow down, as if from wind
            // resistance. we want the smoke to linger a bit and feel wispy, though,
            // so we don't stop them completely like we do ExplosionParticleSystem
            // particles.
            minAcceleration = 0;
            maxAcceleration = 0;

            // explosion smoke lasts for longer than the explosion itself, but not
            // as long as the plumes do.
            minLifetime = 0.15f;
            maxLifetime = 0.2f;

            minScale = 3.5f;
            maxScale = 6.5f;

            minNumParticles = 0;
            maxNumParticles = 2;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            spriteBlendMode = SpriteBlendMode.AlphaBlend;

            SetFuelRatio(1.0f);

            DrawOrder = AlphaBlendDrawOrder;
        }
        public void SetFuelRatio(float ratio)
        {
            Color red = new Color(0.9f, 0.1f, 0.5f);
            Color smoke = Color.White;

            Color current = Color.Lerp(smoke, red, ratio);

            Vector3 floatCol = current.ToVector3();

            minRed = floatCol.X;
            maxRed = minRed;

            minGreen = floatCol.Y;
            maxGreen = minGreen;

            minBlue = floatCol.Z;
            maxBlue = minBlue;
        }
    }
}
