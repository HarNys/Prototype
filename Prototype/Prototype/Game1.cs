using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SwipeGestureRecognizer;


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
        int numCards = 6;
        int[] beatSequence;
        Cards[] card;  //Iimage for cards
        Random random = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
           
            /*Rezising the window*/
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 400;
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


            card = new Cards[numCards];  //creats the cards
            for (int i = 0; i < numCards; i++)
                {
                    card[i] = new Cards();
                }

            base.Initialize();
        }

        protected void RandomSequence(int beats)
        {

            beatSequence = new int[beats];
            for (int i = 0; i < beats; i++)
            {
                beatSequence[i] = random.Next(6);
            }
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
                int imageXPos = 0;
                int imageYPos = 0;
            
            for (int i = 0; i < numCards; i++)
            {
                if (i%2 != 0)
                {
                    imageXPos = 0;
                    imageYPos = ((i/2)*250) ;
                }
                else
                {
                    imageXPos = 300;
                    imageYPos = ((i / 2) * 250);
                }

                Vector2 imgPosition = new Vector2(imageXPos, imageYPos);
                card[i].Initialize(Content.Load<Texture2D>("hitBox"), imgPosition);
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

            // Draw the cards

            for (int i = 0; i < numCards; i++)
            {
                card[i].Draw(spriteBatch);
            }

            // Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
