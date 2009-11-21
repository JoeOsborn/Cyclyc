using System;
using System.Collections.Generic;
using System.Linq;
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
using Cyclyc.JetpackGirl;
using Cyclyc.ShipGirl;

namespace Cyclyc
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        ShipGame shipGame;
        JetGame jetGame;
        public bool playing;

        public double timePlayed;

        public Game1()
        {
            playing = false;
            timePlayed = 0;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            shipGame = new ShipGame(this);
            jetGame = new JetGame(this);
            shipGame.NextGame = jetGame;
            jetGame.NextGame = shipGame;
        }

        public int Tempo
        {
            get { return 180; }
        }
        public double CurrentBeat
        {
            get { return (timePlayed / 60.0) * Tempo; }
        }
        public double CurrentMeasure
        {
            get { return (int)(CurrentBeat / 4.0); }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            shipGame.Initialize();
            jetGame.Initialize();
            base.Initialize();
        }

        protected int PipeWidth
        {
            get { return 24; }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            shipGame.LoadContent();
            jetGame.LoadContent();

            Viewport upperView = GraphicsDevice.Viewport;
            upperView.Height /= 2;
            upperView.X = PipeWidth;
            upperView.Width -= PipeWidth*2;
            Viewport lowerView = upperView;
            lowerView.Y += lowerView.Height;

            shipGame.View = upperView;
            jetGame.View = lowerView;

            foreach (SongTrack s in shipGame.Songs)
            {
                s.Play();
            }
            foreach (SongTrack s in jetGame.Songs)
            {
                s.Play();
            }
            playing = true;

            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            //update beat and measure counters
            if (playing)
            {
                timePlayed += gameTime.ElapsedGameTime.TotalSeconds;
            }
            shipGame.Update(gameTime);
            jetGame.Update(gameTime);
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Viewport defaultVP = GraphicsDevice.Viewport;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            shipGame.Draw(gameTime);
            jetGame.Draw(gameTime);
            base.Draw(gameTime);
            GraphicsDevice.Viewport = defaultVP;
        }
    }
}
