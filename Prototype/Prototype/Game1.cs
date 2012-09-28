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

namespace Prototype
{
     
    /// <summary>
    /// This is the main type for your game
    /// </summary>

    public class Card
    {
        int imageIndex;
        bool hidden;
        
        public Card()
        {
            imageIndex = 0;
            hidden = true;
        }

        public Card(int imageNumber)
        {
            imageIndex = imageNumber;
            hidden = true;
        }
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        Cards[] card;  //Iimage for cards

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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


                card = new Cards[10];  //creats the cards
                for (int i = 0; i < 10; i++)
                {
                    card[i] = new Cards();
                }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // ;Load the sprit resources
            for (int i = 0; i < 10; i++)
            {
                int imagePos = (i * 10 + 10);
                Vector2 imgPosition = new Vector2(imagePos, 10);
                card[i].Initialize(Content.Load<Texture2D>("card1"), imgPosition);
            }

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            
            
            // TODO: Add your drawing code here

            // Start drawing
            spriteBatch.Begin();

            // Draw the Player

            for (int i = 0; i < 10; i++)
            {
                card[i].Draw(spriteBatch);
            }

            // Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
