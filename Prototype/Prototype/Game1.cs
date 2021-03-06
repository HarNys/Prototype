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
        int score;
        int lvl;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        bool corectHit;
        int wrongHit;
        bool newLevel;
        bool playing;
        int hits;
        int roundTime;
        int drawTime;
        int newLevelTimer;
        float updateTime;
        int hitTimer;
        bool nexthit;
        bool watching;
        bool gameOver;
        bool play;

        SoundEffect soundEffect;
        SoundEffect wrongHitSound;
        SoundEffect gameOverSound;

        Vector2 LHandPos;
        Vector2 RHandPos;
        Vector2 LFootPos;
        Vector2 RFootPos;

        private Texture2D playtext;
        private Texture2D watchtext;


        Cards card;  //Iimage for cards
        Cards[] hitBox;
        SpriteFont scoreFont;


        int[] beatSequence;

        SoundEffect[] sounds;
        Random random = new Random();

        float textshow = 0;





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
            gameOver = false;
            corectHit = false;
            newLevel = false;
            play = false;
            watching = true;
            roundTime = 0;
            drawTime = 0;
            updateTime = 0;
            newLevelTimer = 0;
            wrongHit = 0;

            hitTimer = 0;
            nexthit = true;
            // TODO: Add your initialization logic here
            hits = 0;
            lvl = 0;
            
            this.Components.Add(this.depthStream);
            this.Components.Add(this.colorStream);


            card = new Cards();  //creats the hitboxes
            hitBox = new Cards[6];

            for (int i = 0; i<6;i++)
            {
                hitBox[i] = new Cards();
                hitBox[i].Show = true;
                hitBox[i].Update(i);
            }

            card.Show = true;
            playing = false;
         /*   do
            {
                newLevel = false;
            } while (this.skeletonStream.getSkeletonDrawn());*/

           // newLevel = true; //games starts at new level

            score = 0;

            base.Initialize();
        }
        
       

        //Random generator for the placing of boxes
        protected void RandomSequence(int beats)
        {

            /*
			
			int[] TempBeatSequence = new int[beats-1]
			
			for (int i = 0; i < (beats-1); i++)
            {
				TempBeatSequence[i] = beatSequence[i]; 
			}
			
			beatSequence = new int[beats];
			
			for (int i = 0; i < (beats-1); i++)
            {
				 beatSequence[i] = TempBeatSequence[i]; 
			}
			
			if(i>0)
				{
			        while (beatSequence[beats] == beatSequence[beats - 1])
                    {
                        beatSequence[beats] = random.Next(6);
                    }
				}
			
			*/
			
            beatSequence = new int[beats];
            for (int i = 0; i < beats; i++)
            {
                beatSequence[i] = random.Next(6);
                
                if(i>0)
                {           //prvents from two circles in same spot
                    while (beatSequence[i] == beatSequence[i - 1])
                    {
                        beatSequence[i] = random.Next(6);
                    }
                }
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
            // Load the sprit resources
            int imageXPos = 0;
            int imageYPos = 0;

            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), this.spriteBatch);

            Vector2 imgPosition = new Vector2(imageXPos, imageYPos);
            card.Initialize(Content.Load<Texture2D>("hitCircleHit"), imgPosition);
            card.Initialize(Content.Load<Texture2D>("hitBoxHit"),imgPosition);
            scoreFont = Content.Load<SpriteFont>("SpriteFont1");

            sounds = new SoundEffect[6];

            sounds[0] = Content.Load<SoundEffect>("hitRedSound");
            sounds[1] = Content.Load<SoundEffect>("hitGreenSound");
            sounds[2] = Content.Load<SoundEffect>("hitYellowSound");
            sounds[3] = Content.Load<SoundEffect>("hitBlueSound");
            sounds[4] = Content.Load<SoundEffect>("hitPurpleSound");
            sounds[5] = Content.Load<SoundEffect>("hitOrangeSound");

            wrongHitSound =  Content.Load<SoundEffect>("wronghit");
            gameOverSound =  Content.Load<SoundEffect>("Gameover");

            playtext = Content.Load<Texture2D>("play");
            watchtext = Content.Load<Texture2D>("watch");


            for (int i = 0; i < 6; i++)
            {
                hitBox[0].Initialize(Content.Load<Texture2D>("hitCircleRed"), imgPosition);
                hitBox[1].Initialize(Content.Load<Texture2D>("hitCircleGreen"), imgPosition);
                hitBox[2].Initialize(Content.Load<Texture2D>("hitCircleBlue"), imgPosition);
                hitBox[3].Initialize(Content.Load<Texture2D>("hitCircleYellow"), imgPosition);
                hitBox[4].Initialize(Content.Load<Texture2D>("hitCircleOrange"), imgPosition);
                hitBox[5].Initialize(Content.Load<Texture2D>("hitCirclePurple"), imgPosition);
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

            
            //trying to make the game wait for some time before next level starts           
            if ((gameTime.TotalGameTime.Seconds - newLevelTimer) > 2 && !newLevel)
            {
                gameOver = false;
                play = false;
                watching = true;
                spriteBatch.Begin();
                newLevel = true;
                roundTime = gameTime.TotalGameTime.Seconds;
                card.Show = true;
                spriteBatch.End();
            }
            else if (!newLevel)
            {
                card.Show = false;
            }

                LHandPos = this.depthStream.skeletonStream.jointPosLHand;
                RHandPos = this.depthStream.skeletonStream.jointPosRHand;
                LFootPos = this.depthStream.skeletonStream.jointPosLFoot;
                RFootPos = this.depthStream.skeletonStream.jointPosRFoot;


                for (int i = 0; i < 6; i++)
                {
                    hitBox[i].Update(i);
                }

                int imageXPos;
                int imageYPos;

                if (!playing)
                {
                    RandomSequence(2 + lvl);
                    watching = true;
                    playing = true;
                }

                //draw shapes again if player have not finish in 15 sec
                if (newLevel && (gameTime.TotalGameTime.Seconds - roundTime) > (10 + lvl))
                {
                    watching = true;
                    card.Show = true;
                    drawTime = 0;
                    score = score-2;
                    hits = 0;
                    roundTime = gameTime.TotalGameTime.Seconds;
                }

                if (newLevel && playing)
                {
                    //gameTime.TotalGameTime is reseting it self, so we have to make up for it
                   if (gameTime.TotalGameTime.Seconds ==0)
                    {
                        drawTime = 0;
                        roundTime = 0;
                        hitTimer = 0;
                    }


                    if (drawTime < beatSequence.Length)
                    {
                        play = false;
                        card.Update(beatSequence[drawTime]);
                        drawTime = (gameTime.TotalGameTime.Seconds - roundTime);
                        updateTime = 0;
                    }

                    else
                    {
                        card.Show = false;
                    }

                    if (!card.Show && playing)
                    {
                        play = true;
                        watching = false;

                        imageXPos = 0;
                        imageYPos = ((beatSequence[hits] / 2) * 250);

                        card.Update(beatSequence[hits]);
                        

                        if (beatSequence[hits] % 2 == 0)
                        {
                            imageXPos = 0;
                            imageYPos = ((beatSequence[hits] / 2) * 200);
                            

                            if (LHandPos.X > imageXPos && LHandPos.X < (imageXPos + 150) && LHandPos.Y > imageYPos && LHandPos.Y < (imageYPos + 100) ||
                                RHandPos.X > imageXPos && RHandPos.X < (imageXPos + 150) && RHandPos.Y > imageYPos && RHandPos.Y < (imageYPos + 100) ||
                                LFootPos.X > imageXPos && LFootPos.X < (imageXPos + 150) && LFootPos.Y > imageYPos && LFootPos.Y < (imageYPos + 100) ||
                                RFootPos.X > imageXPos && RFootPos.X < (imageXPos + 150) && RFootPos.Y > imageYPos && RFootPos.Y < (imageYPos + 100))
                            {

                                sounds[beatSequence[hits]].Play();
                                score++;
                                hits++;
                                nexthit = false;

                                
                            }
                        }
                        else
                        {

                            imageXPos = 500; //Kinect sucks balls, thats why
                            imageYPos = ((beatSequence[hits] / 2) * 200); 
                           




                            if (LHandPos.X > imageXPos && LHandPos.X < (imageXPos + 150) && LHandPos.Y > imageYPos && LHandPos.Y < (imageYPos + 100) ||
                                RHandPos.X > imageXPos && RHandPos.X < (imageXPos + 150) && RHandPos.Y > imageYPos && RHandPos.Y < (imageYPos + 100) ||
                                LFootPos.X > imageXPos && LFootPos.X < (imageXPos + 150) && LFootPos.Y > imageYPos && LFootPos.Y < (imageYPos + 100) ||
                                RFootPos.X > imageXPos && RFootPos.X < (imageXPos + 150) && RFootPos.Y > imageYPos && RFootPos.Y < (imageYPos + 100))
                            {
                                sounds[beatSequence[hits]].Play();
                                score++;
                                hits++;
                                nexthit = false;

                            }
                        }

  ////////////////////////////if you hit wrong//////////////////////////////////////////////////////////
                       
                        for (int i = 0; i < 6; i++)
                        {
                            if (!corectHit && nexthit)
                            {
                                if (i % 2 == 0)
                                {
                                    imageXPos = 0;
                                    imageYPos = ((i / 2) * 200);

                                    //Console.WriteLine("Left: Top: " + imageXPos +"," + imageYPos + " Bot: " + (imageXPos + 100) + "," + (imageYPos+100));

                                    if (LHandPos.X > imageXPos && LHandPos.X < (imageXPos + 150) && LHandPos.Y > imageYPos && LHandPos.Y < (imageYPos + 100) ||
                                         RHandPos.X > imageXPos && RHandPos.X < (imageXPos + 150) && RHandPos.Y > imageYPos && RHandPos.Y < (imageYPos + 100) ||
                                         LFootPos.X > imageXPos && LFootPos.X < (imageXPos + 150) && LFootPos.Y > imageYPos && LFootPos.Y < (imageYPos + 100) ||
                                         RFootPos.X > imageXPos && RFootPos.X < (imageXPos + 150) && RFootPos.Y > imageYPos && RFootPos.Y < (imageYPos + 100))
                                    {
                                        wrongHit++;
                                        nexthit = false;
                                        hitTimer = gameTime.TotalGameTime.Seconds;
                                        wrongHitSound.Play();
                                    }
                                }
                                else
                                {

                                    imageXPos = 500; //Kinect sucks balls, thats why
                                    imageYPos = ((i / 2) * 200);

                                    if (LHandPos.X > imageXPos && LHandPos.X < (imageXPos + 150) && LHandPos.Y > imageYPos && LHandPos.Y < (imageYPos + 100) ||
                                    RHandPos.X > imageXPos && RHandPos.X < (imageXPos + 150) && RHandPos.Y > imageYPos && RHandPos.Y < (imageYPos + 100) ||
                                    LFootPos.X > imageXPos && LFootPos.X < (imageXPos + 150) && LFootPos.Y > imageYPos && LFootPos.Y < (imageYPos + 100) ||
                                    RFootPos.X > imageXPos && RFootPos.X < (imageXPos + 150) && RFootPos.Y > imageYPos && RFootPos.Y < (imageYPos + 100))
                                    {
                                        wrongHit++;
                                        nexthit = false;
                                        hitTimer = gameTime.TotalGameTime.Seconds;
                                        wrongHitSound.Play();
                                    }

                                }
                                if (wrongHit > 2)
                                {
                                    gameOver = true;
                                    gameOverSound.Play();
                                    spriteBatch.Begin();
                                    spriteBatch.DrawString(scoreFont, "Game Over", new Vector2(150, 200), Microsoft.Xna.Framework.Color.Red);
                                    spriteBatch.End();
                                    lvl = 0;
                                    playing = false;
                                    card.Show = true;
                                    play = false;
                                    watching = false;
                                    hits = 0;
                                    drawTime = 0;
                                    roundTime = gameTime.TotalGameTime.Seconds;
                                    newLevel = false; //yes its backwards, just dael with it 
                                    newLevelTimer = gameTime.TotalGameTime.Seconds;
                                    wrongHit = 0;
                                    score = 0;
                                    hitTimer = gameTime.TotalGameTime.Seconds;
                                    
                                    
                                }


                             }
                        }

                        if ((gameTime.TotalGameTime.Seconds - hitTimer) > 1 && !nexthit)
                        {
                            nexthit = true;
                            hitTimer = gameTime.TotalGameTime.Seconds;
                            corectHit = false;
                        }
///////////////////////////////////////////////////////////////////////////////////////////////
                    }

                    //new level starts
                    if (beatSequence.Length == hits)
                    {
                        gameOver = false;
                        watching = true;
                        play = false;
                        lvl++;
                        playing = false;
                        card.Show = true;
                        hits = 0;
                        drawTime = 0;
                        roundTime = gameTime.TotalGameTime.Seconds;
                        newLevel = false; //yes its backwards, just dael with it 
                        newLevelTimer = gameTime.TotalGameTime.Seconds;
                        wrongHit = 0;
                    }

                }

                if (gameOver == true)
                {
                    watching = false;
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

            
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

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

            // Draw
            for (int i = 0; i < 6; i++)
            {
                hitBox[i].Draw(spriteBatch);
            }



            card.Draw(spriteBatch);
            if (gameOver) spriteBatch.DrawString(scoreFont, "Game Over", new Vector2(151, 200), Microsoft.Xna.Framework.Color.White);
            if (gameOver) spriteBatch.DrawString(scoreFont, "Game Over", new Vector2(150, 200), Microsoft.Xna.Framework.Color.Red);
            if (watching) spriteBatch.Draw(watchtext, new Vector2(125, 100), Microsoft.Xna.Framework.Color.White);
            if (play) spriteBatch.Draw(playtext, new Vector2(130, 100), Microsoft.Xna.Framework.Color.White);
            spriteBatch.DrawString(scoreFont, "score: " + score, new Vector2(151, 11), Microsoft.Xna.Framework.Color.Black);
            spriteBatch.DrawString(scoreFont, "Level: " + (lvl + 1), new Vector2(151, 31), Microsoft.Xna.Framework.Color.Black);
            spriteBatch.DrawString(scoreFont, "score: " + score, new  Vector2 (150,10), Microsoft.Xna.Framework.Color.White);
            spriteBatch.DrawString(scoreFont, "Level: " + (lvl+1), new Vector2(150, 30), Microsoft.Xna.Framework.Color.White);
            spriteBatch.DrawString(scoreFont, "Lenght:" + (hits), new Vector2(151, 50), Microsoft.Xna.Framework.Color.Black);
            spriteBatch.DrawString(scoreFont, "Lenght:" + (hits) , new Vector2(150, 50), Microsoft.Xna.Framework.Color.White);
            

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
