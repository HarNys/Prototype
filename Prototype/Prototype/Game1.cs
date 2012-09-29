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

    public class Game1 : Microsoft.Xna.Framework.Game
    {


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int numCards = 6;

        Cards card;  //Iimage for cards

        int[] beatSequence;

        Random random = new Random();


        /// <summary>
        /// This controls the transition time for the resize animation.
        /// </summary>
        private const double TransitionDuration = 1.0;

        /// <summary>
        /// This control selects a sensor, and displays a notice if one is
        /// not connected.
        /// </summary>
        private readonly KinectChooser chooser;

        /// <summary>
        /// This manages the rendering of the color stream.
        /// </summary>
        private readonly ColorStreamRenderer colorStream;

        /// <summary>
        /// This manages the rendering of the depth stream.
        /// </summary>
        private readonly DepthStreamRenderer depthStream;

        /// <summary>
        /// This is the location of the color stream when minimized.
        /// </summary>
        private readonly Vector2 colorSmallPosition;

        /// <summary>
        /// This is the location of the depth stream when minimized;
        /// </summary>
        private readonly Vector2 depthSmallPosition;

        /// <summary>
        /// This is the minimized size for both streams.
        /// </summary>
        private readonly Vector2 minSize;

        /// <summary>
        /// This is the viewport of the streams.
        /// </summary>
        private readonly Microsoft.Xna.Framework.Rectangle viewPortRectangle;

        /// <summary>
        /// This tracks the state to indicate which stream has focus.
        /// </summary>
        private bool colorHasFocus = true;

        /// <summary>
        /// This tracks the previous keyboard state.
        /// </summary>
        private KeyboardState previousKeyboard;

        /// <summary>
        /// This tracks the current transition time.
        /// 0                   = Color Stream Full Focus
        /// TransitionDuration  = Depth Stream Full Focus
        /// </summary>
        private double transition;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
           
            /*Rezising the window*/
            int Width = 400;
            int Hight = 600;
            graphics.PreferredBackBufferHeight = Hight;
            graphics.PreferredBackBufferWidth = Width;

            // The Kinect sensor will use 640x480 for both streams
            // To make your app handle multiple Kinects and other scenarios,
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            this.chooser = new KinectChooser(this, ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution640x480Fps30);
            this.Services.AddService(typeof(KinectChooser), this.chooser);

            // Default size is the full viewport
            this.colorStream = new ColorStreamRenderer(this);

            // Calculate the minimized size and location
            this.depthStream = new DepthStreamRenderer(this);
            this.depthStream.Size = new Vector2(this.viewPortRectangle.Width / 4, this.viewPortRectangle.Height / 4);
            this.depthStream.Position = new Vector2(Width - this.depthStream.Size.X - 15, 85);

            // Store the values so we can animate them later
            this.minSize = this.depthStream.Size;
            this.depthSmallPosition = this.depthStream.Position;
            this.colorSmallPosition = new Vector2(15, 85);

            this.Components.Add(this.chooser);

            this.previousKeyboard = Keyboard.GetState();

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
            this.Components.Add(this.depthStream);
            this.Components.Add(this.colorStream);

            card = new Cards();  //creats the cards

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

                this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
                this.Services.AddService(typeof(SpriteBatch), this.spriteBatch);

                Vector2 imgPosition = new Vector2(imageXPos, imageYPos);
                card.Initialize(Content.Load<Texture2D>("hitBox"), imgPosition);
            

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

            int[] tempRandomArray = { 1, 2, 3, 4, 5, 0, 3 };


            if (gameTime.TotalGameTime.Seconds < tempRandomArray.Length)
            {
                card.Update(tempRandomArray[gameTime.TotalGameTime.Seconds]);
            }

            else
            {
                card.Show = false;
            }


            // Animate the transition value
            if (this.colorHasFocus)
            {
                this.transition -= gameTime.ElapsedGameTime.TotalSeconds;
                if (this.transition < 0)
                {
                    this.transition = 0;
                }
            }
            else
            {
                this.transition += gameTime.ElapsedGameTime.TotalSeconds;
                if (this.transition > TransitionDuration)
                {
                    this.transition = TransitionDuration;
                }
            }

            // Animate the stream positions and sizes
            this.colorStream.Position = Vector2.SmoothStep(
                new Vector2(this.viewPortRectangle.X, this.viewPortRectangle.Y),
                this.colorSmallPosition,
                (float)(this.transition / TransitionDuration));
            this.colorStream.Size = Vector2.SmoothStep(
                new Vector2(this.viewPortRectangle.Width, this.viewPortRectangle.Height),
                this.minSize,
                (float)(this.transition / TransitionDuration));

            this.depthStream.Position = Vector2.SmoothStep(
                this.depthSmallPosition,
                new Vector2(this.viewPortRectangle.X, this.viewPortRectangle.Y),
                (float)(this.transition / TransitionDuration));
            this.depthStream.Size = Vector2.SmoothStep(
                this.minSize,
                new Vector2(this.viewPortRectangle.Width, this.viewPortRectangle.Height),
                (float)(this.transition / TransitionDuration));


            base.Update(gameTime);
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            
            
            // TODO: Add your drawing code here

            // Start drawing
            spriteBatch.Begin();

            // Draw the cards

            for (int i = 0; i < numCards; i++)
            {
                card.Draw(spriteBatch);
            }

            // Stop drawing
            spriteBatch.End();

            // Render the streams with respect to focus
            if (this.colorHasFocus)
            {
                this.colorStream.DrawOrder = 1;
                this.depthStream.DrawOrder = 2;
            }
            else
            {
                this.colorStream.DrawOrder = 2;
                this.depthStream.DrawOrder = 1;
            }


            base.Draw(gameTime);
        }

        /// <summary>
        /// This method ensures that we can render to the back buffer without
        /// losing the data we already had in our previous back buffer.  This
        /// is necessary for the SkeletonStreamRenderer.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event args.</param>
        private void GraphicsDevicePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // This is necessary because we are rendering to back buffer/render targets and we need to preserve the data
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }
    }
}
