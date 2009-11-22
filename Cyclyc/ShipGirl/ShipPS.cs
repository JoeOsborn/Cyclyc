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
    public class ShipPS : ParticleSystem
    {
        public ShipPS(Game1 game)
            : base(game, 16)
        {
            PoweredColor = new Color(1.0f, 0.7f, 0.7f);
        }
        public float Rotation { get; set; }
        protected override float PickRandomAngle()
        {
            return Rotation;
        }

        protected override void InitializeConstants()
        {
            textureFilename = "shipMove particle";

            // less initial speed than the explosion itself
            minInitialSpeed = 0.0f;
            maxInitialSpeed = 0.0f;

            // acceleration is negative, so particles will accelerate away from the
            // initial velocity.  this will make them slow down, as if from wind
            // resistance. we want the smoke to linger a bit and feel wispy, though,
            // so we don't stop them completely like we do ExplosionParticleSystem
            // particles.
            minAcceleration = 0;
            maxAcceleration = 0;

            // explosion smoke lasts for longer than the explosion itself, but not
            // as long as the plumes do.
            minLifetime = 0.6f;
            maxLifetime = 0.6f;

            minScale = 0.6f;
            maxScale = 0.8f;

            minNumParticles = 0;
            maxNumParticles = 4;

            minRotationSpeed = 0;
            maxRotationSpeed = 0;

            spriteBlendMode = SpriteBlendMode.AlphaBlend;

            SetPowerRatio(1.0f);

            DrawOrder = AlphaBlendDrawOrder;
        }
        public Color PoweredColor { get; set; }
        public void SetPowerRatio(float ratio)
        {
            Color white = Color.White;

            Color current = Color.Lerp(white, PoweredColor, ratio);

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
