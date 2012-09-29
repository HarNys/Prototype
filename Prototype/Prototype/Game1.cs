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
        int Score;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        bool newLevel;
        bool playing;
        int hits;
        bool boxDipslaying;
        SoundEffect soundEffect;
        //int[] testarray={0,1,2,3,4,5};

        Vector2 LHandPos;
        Vector2 RHandPos;
        Vector2 LFootPos;
        Vector2 RFootPos;

        Cards card;  //Iimage for cards

        int[] beatSequence = {0,1,2,3,4,5};

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
        /// This manages the rendering of the depth stream.
        /// </summary>
        private readonly SkeletonStreamRenderer skeletonStream;

        
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
        private bool colorHasFocus = false;

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

           // this.skeletonStream = new SkeletonStreamRenderer(this, this.depthStream.SkeletonToDepthMap);

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
            hits = 0;
            
            this.Components.Add(this.depthStream);
            this.Components.Add(this.colorStream);


            card = new Cards();  //creats the hitboxes



            playing = false;
         /*   do
            {
                newLevel = false;
            } while (this.skeletonStream.getSkeletonDrawn());*/

           // newLevel = true; //games starts at new level

            Score = 0;

            base.Initialize();
        }

        
        //Random generator for the placing of boxes
        protected void RandomSequence(int beats)
        {

           /* beatSequence = new int[beats];
            for (int i = 0; i < beats; i++)
            {
 ///////////////////////////////////////////////////////////               beatSequence[i] = random.Next(6);
            }*/
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

                soundEffect = Content.Load<SoundEffect>("lydting");
            

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

            LHandPos = this.depthStream.skeletonStream.jointPosLHand;
            RHandPos = this.depthStream.skeletonStream.jointPosRHand;
            LFootPos = this.depthStream.skeletonStream.jointPosLFoot;
            RFootPos = this.depthStream.skeletonStream.jointPosRFoot;



            if (playing)
            {
                // Console.WriteLine("Hit: " + hits);
                Console.WriteLine("Score: " + Score);
                Console.WriteLine("Left hand: " + LHandPos);
                Console.WriteLine("Rigth Hand: " + RHandPos);
            }

           /* if (LHandPos.X < 100 && LHandPos.Y < 100 || RHandPos.X < 100 && RHandPos.Y < 100)
            {
                playing = false;
            }*/

            int imageXPos;
            int imageYPos;
            


            if (!playing)
            {
              //  RandomSequence(6);

                playing = true;
            }

            if (playing)
            {
                if (gameTime.TotalGameTime.Seconds < beatSequence.Length)
                {
                    card.Update(beatSequence[gameTime.TotalGameTime.Seconds]);
                }
                
                else
                {
                    card.Show = false;
                }

                imageXPos = 0;
                imageYPos = ((beatSequence[hits] / 2) * 250);

                if (!card.Show)
                {
                    if (beatSequence[hits] % 2 == 0)
                    {
                        imageXPos = 0;
                        imageYPos = ((beatSequence[hits] / 2) * 250);

                        Console.WriteLine("Left: Top: " + imageXPos +"," + imageYPos + " Bot: " + (imageXPos + 100) + "," + (imageYPos+100));

                        if (LHandPos.X > imageXPos && LHandPos.X < (imageXPos + 100) && LHandPos.Y > imageYPos && LHandPos.Y < (imageYPos + 100) || RHandPos.X > imageXPos && RHandPos.X < (imageXPos + 100) && RHandPos.Y > imageYPos && RHandPos.Y < (imageYPos + 100) || LFootPos.X > imageXPos && LFootPos.X < (imageXPos + 100) && LFootPos.Y > imageYPos && LFootPos.Y < (imageYPos + 100) || RFootPos.X > imageXPos && RFootPos.X < (imageXPos + 100) && RFootPos.Y > imageYPos && RFootPos.Y < (imageYPos + 100))
                        {
                            soundEffect.Play();
                            Score++;
                            hits++;
                        }


                    }
                    else
                    {
                        

                        imageXPos = 300;
                        imageYPos = ((beatSequence[hits] / 2) * 250);

                        Console.WriteLine("Right: Top: " + imageXPos + "," + imageYPos + " Bot: " + (imageXPos + 100) + "," + (imageYPos + 100));

                        if (LHandPos.X > imageXPos && LHandPos.X < (imageXPos + 100) && LHandPos.Y > imageYPos && LHandPos.Y < (imageYPos + 100) || RHandPos.X > imageXPos && RHandPos.X < (imageXPos + 100) && RHandPos.Y > imageYPos && RHandPos.Y < (imageYPos + 100) || LFootPos.X > imageXPos && LFootPos.X < (imageXPos + 100) && LFootPos.Y > imageYPos && LFootPos.Y < (imageYPos + 100) || RFootPos.X > imageXPos && RFootPos.X < (imageXPos + 100) && RFootPos.Y > imageYPos && RFootPos.Y < (imageYPos + 100))
                        {
                            soundEffect.Play();
                            Score++;
                            hits++;
                        }
                    }
                }

                if (hits == beatSequence.Length)
                {
                    playing = false;
                }
            
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

           

            /*float skeletonPosX //= this.skeletonStream.getSkeleton().X;
            float skeletonPosY //= this.skeletonStream.getSkeleton().Y;
            

            if (skeletonPosX < 101 && skeletonPosY < 101)
            {
                newLevel = true;
            }*/

            base.Update(gameTime);
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            
            // TODO: Add your drawing code here




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

            // Start drawing
            spriteBatch.Begin();

            // Draw the cards
            card.Draw(spriteBatch);

            // Stop drawing
            spriteBatch.End();
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
