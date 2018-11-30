using FitMiAndroid;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


// NOTES: Want to be able to adjust paddle width and sensetivity; Collect data (save exactly what the user is doing, and any events (like score, levels, bonuses etc.),
//        and output to a text file). Also want to keep track of how many days or time the user is playing the game.
//        Work on puck controls, fix content loading issues (recreated by increasing the difficulty parameter, most likely caused by maps)



namespace blockBreakerAndroid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Paddle paddle;

        SoundEffect blockHitSFX,
                    paddleHitSFX,
                    wallHitSFX,
                    fireBallSFX,
                    powerUpSFX;

        int ballWithPaddle;

        int score = 0;
        int level = 0;
        public static bool startOfLevel = true;
        float newLevelCounter = 0f; // controls will be disabled and level string displayed for a moment when a new level first loads
        float powerUpChance; // % chance of dropping a powerup, set in CreateLevel
        float powerUpTimer = 0; // used to prevent power-ups from dropping near-simultaneously
        float dataTimer = 0;  // used as timer to print various puck values on the screen slower than fps
        
          public enum LevelParameter
            {
                Background = 0,    // 0 == Sky, 1 == underwater, 2 == space
                BallSpeed,         // 0 == slow, 1 == medium, 2 == fast
                PaddleWidth,       // 0 == largest paddle, 1 == smaller paddle, 2 == smallest paddle
                PowerUpFrequency,  // power-up frequency is a percentage => 40 would be 40% chance of a block dropping a powerup
                MapSize,           // 0 == least amount of blocks, 1 == medium amount, 2 = largest amount
                BlockDurability,   //  0 == 1 hit to destroy blocks, 1 == 2 hits to destroy block, 2 == 3 hits to destroy block
                ProgressDifficulty // progressDifficulty => the game will slightly increase the speed of the ball, decrease paddlewidth, increase power-up frequency, 
                                   // and make blocks more durable (up to two hits) if progressDifficulty is set to true
            }

       // [background, ballSpeed, paddleWidth, availability of power-ups, blockLevel, durabilityOfBlocks, progressDifficulty 0 == false, 1 == true]
        int[] levelParams = { 0, 1, 0, 50, 0, 0, 1 }; // for now just setting the values here, but going to read them from a file or maybe command line

        List<Block> blocks = new List<Block>();
        List<Ball> balls = new List<Ball>();
        List<PowerUp> powerUps = new List<PowerUp>();
        Texture2D background;
        Random rand = new Random();
        
        SpriteFont font;

        HIDPuckDongle puckDongle = new HIDPuckDongle(Game.Activity);

        int screenWidth = 1300;
        int screenWidthDivisor = 16; // 28 used for repsonsive resolution code
        int screenHeight = 760;//800;
        int screenHeightDivisor = 8;

        int gameDuration; // how long the game will run for (in seconds) before exiting
        int gameDifficulty;

        // 11/30/18 difficulty causes a content load error when increased, likely due to a map not loading properly/not being in the pipeline
        public Game1(string contentDir = "CONTENT_DIR", int duration = 9999, int difficulty = 0)
        {
            // contentDir = "blockBreakerAndroid"
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; //contentDir;
            gameDuration = duration;
            gameDifficulty = difficulty;
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            // SetDifficulty();
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

            puckDongle.Open();


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

            //GraphicsAdapter graphicsAdapter = graphics.GraphicsDevice.Adapter;
            //screenWidth = graphicsAdapter.CurrentDisplayMode.Width;
            //screenHeight = graphicsAdapter.CurrentDisplayMode.Height;
            paddle = new Paddle(this);
            paddle.LoadContent();
            paddle.position = new Vector2(screenWidth / 2, screenHeight - paddle.Height * 2);

            SpawnBall();

            blockHitSFX = Content.Load<SoundEffect>("high_beep");
            paddleHitSFX = Content.Load<SoundEffect>("low_beep");
            wallHitSFX = Content.Load<SoundEffect>("mid_beep");
            fireBallSFX = Content.Load<SoundEffect>("fireball_sound");
            powerUpSFX = Content.Load<SoundEffect>("powerup");

            font = Content.Load<SpriteFont>("Score");

            // TODO: use this.Content to load your game content here
        }



        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            spriteBatch.Dispose();
            
            // TODO: Unload any non ContentManager content here
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            powerUpTimer += 0.05f;
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || gameTime.ElapsedGameTime.TotalSeconds >= gameDuration)
            {
                //   Exit();
                Game.Activity.Finish();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !startOfLevel)
              {
                  foreach (Ball b in balls)
                      b.IsPaddleBall = false;
              }

            

            // keep track of how long the "Level X" string is on the screen, disable paddle until it's gone
            newLevelCounter += 0.05f;

            if (newLevelCounter > 5f)
                startOfLevel = false;
                
            if (!startOfLevel)
                paddle.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            // if ball is not launched, the position will be the same as the paddle
            foreach (Ball b in balls)
            {
                if (b.IsActive && b.IsPaddleBall)
                {
                    b.position = new Vector2(paddle.position.X, paddle.position.Y - b.Radius * 2f);
                    b.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                    b.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                CheckCollisions(b);
            }


            // remove balls that have been lost
            for (int i = 0; i < balls.Count; i++)
            {
                if (!balls[i].IsActive)
                {
                    balls.RemoveAt(i);

                    if (score > 50)
                    {
                        score -= 5;
                    }

                    if (balls.Count == 0)
                        SpawnBall();
                }
            }


            // drop powerup and check if it collides with the player
            foreach (PowerUp p in powerUps)
            {
                Rectangle paddlePos = paddle.BoundingRect;

                if (!p.shouldRemove)
                    p.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                if (paddlePos.Intersects(p.BoundingRect) && (!p.isActive))
                {
                    ActivatePowerUp(p);
                    score += 15;
                }
            }

            // remove powerups that have been collected or off the screen
            for (int i = powerUps.Count - 1; i >= 0; i--)
            {
                if (powerUps[i].shouldRemove)
                    powerUps.RemoveAt(i);
            }

            if (blocks.Count == 0)
            {
                ClearLevel();
                CreateLevel();
            }
            dataTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (dataTimer > 0.5f)
            {
                try
                {
                    puckDongle.CheckForNewPuckData();
                    //  full_packet = puck_dongle.full_packet;
                }
                catch (Exception)
                {
                    //      full_packet = "UNABLE TO READ FULL PACKET!";
                }

                dataTimer = 0f;
            }

            base.Update(gameTime);
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(background, /*new Rectangle(0, 0,6803, 1052)*/GraphicsDevice.Viewport.Bounds, Color.White);

            foreach (Block b in blocks)
                spriteBatch.Draw(b.Texture, new Rectangle((int)b.position.X,(int)b.position.Y, (int)b.BlockWidth, (int)b.BlockHeight), Color.White);


            paddle.Draw(spriteBatch);

            foreach (Ball b in balls)
            {
                if (b.IsActive)
                    spriteBatch.Draw(b.Texture, b.position, Color.White);
                
                // print ball x and y directions
                //spriteBatch.DrawString(font, String.Format("Ball Direction.X: {0:#,###0}", b.direction.X.ToString()),
                //                   new Vector2(100, 200), Color.White);
                //spriteBatch.DrawString(font, String.Format("Ball Direction.Y: {0:#,###0}", b.direction.Y.ToString()),
                //                   new Vector2(100, 250), Color.White);
            }
            foreach (PowerUp p in powerUps)
            {
                if (!p.shouldRemove)
                    p.Draw(spriteBatch);
            }

            spriteBatch.DrawString(font, String.Format("Score: {0:#,###0}", score),
                                   new Vector2(40, 50), Color.White);

            if (startOfLevel)
                spriteBatch.DrawString(font, String.Format("Level {0:#0}", level),
                                       new Vector2(screenWidth / 2, screenHeight / 14), Color.White);

            // print blue puck accelerometer values
            //spriteBatch.DrawString(font, puckDongle.PuckPack0.Accelerometer[0].ToString(), new Vector2(100, 150), Color.White);
            //spriteBatch.DrawString(font, puckDongle.PuckPack0.Accelerometer[1].ToString(), new Vector2(100, 200), Color.White);
            //spriteBatch.DrawString(font, puckDongle.PuckPack0.Accelerometer[2].ToString(), new Vector2(100, 250), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }



        // called to check for collision between the ball and blocks 
        private bool intersects(double circle_x, double circle_y, double circle_r, double rect_x, double rect_y, double rect_width, double rect_height)
        {
            double circleDistance_x = Math.Abs(circle_x - rect_x);
            double circleDistance_y = Math.Abs(circle_y - rect_y);

            if (circleDistance_x > (rect_width/2 + circle_r)) { return false; }
            if (circleDistance_y > (rect_height/2 + circle_r)) { return false; }

            if (circleDistance_x <= (rect_width/2)) { return true; } 
            if (circleDistance_y <= (rect_height/2)) { return true; }

            double cornerDistance_sq = Math.Pow((circleDistance_x - rect_width/2), 2) + Math.Pow((circleDistance_y - rect_height/2), 2);

            return (cornerDistance_sq <= Math.Pow(circle_r, 2));
        }




        private void CheckCollisions(Ball ball)
        {
            // Check for paddle
            if ((ballWithPaddle == 0 &&
                (ball.position.X > (paddle.position.X - ball.Radius - paddle.Width / 2)) && // Left, Right, and top half of paddle
                (ball.position.X < (paddle.position.X + ball.Radius + paddle.Width / 2)) &&
                (ball.position.Y < paddle.position.Y) &&
                (ball.position.Y > (paddle.position.Y - ball.Radius - paddle.Height / 2))))
            {
                if (!ball.IsPaddleBall)
                {   
                    paddleHitSFX.Play();
                    score += 3;
                }
                // Reflect based on which part of the paddle is hit

                // By default, set the normal to up
                    Vector2 normal = -1.0f * Vector2.UnitY;
                // ball.direction = -1.0f * Vector2.UnitY;       // Changing direction explicitly makes the ball more predictable

                // Distance from the leftmost to rightmost part of the paddle
                float dist = paddle.Width + ball.Radius * 2;

                // Where within this distance the ball is at
                float ballLocation = ball.position.X -
                    (paddle.position.X - ball.Radius - paddle.Width / 2);

                // Percent between leftmost and rightmost part of paddle
                float pct = ballLocation / dist;

                if (pct <= 0.20f)                               // far left
                    normal = new Vector2(-0.196f, -0.981f);     
                else if (pct > 0.20f && pct <= 0.40f)           // left
                    normal = new Vector2(-0.096f, -0.981f);
                //   ball.direction = new Vector2(-1, -0.981f); 
                else if (pct > 0.40f && pct <= .60f)            // middle
                    normal = new Vector2(0, -0.981f);
                // ball.direction = new Vector2(1, -0.981f);
                else if (pct > .60f && pct <= .80)              // right
                    normal = new Vector2(0.096f, -0.981f);
                else                                            // far right
                    normal = new Vector2(0.196f, -0.981f);      

                    
                ball.direction = Vector2.Reflect(ball.direction, normal);

                // Fix the direction if it's too steep
                AngleCorrection(ball);

                // No collisions between ball and paddle for 20 frames
                ballWithPaddle = 20;
            }
            else if (ballWithPaddle > 0)
            {
                ballWithPaddle--;
            }


            // Check for block collisions
            Block collidedBlock = null;

                
            foreach (Block b in blocks)
            {
                if (intersects(ball.position.X, ball.position.Y, ball.Radius, b.position.X + b.BlockWidth / 3, b.position.Y, b.BlockWidth, b.BlockHeight))
                {
                    collidedBlock = b;
                    break;
                }

            }

            if (collidedBlock != null)
            {
                if (!ball.IsFireBall)
                    blockHitSFX.Play();
                else
                    fireBallSFX.Play();

                int randNum = rand.Next(0, 100);

                if (randNum <= powerUpChance && (powerUps.Count <= 3) && powerUpTimer >= 3) //&& collidedBlock.durability < 1)  // max of 4 powerups dropped at a time
                {
                    DropPowerUp(collidedBlock.position);
                    powerUpTimer = 0;
                }
                        

                // Assume that if our Y is close to the top or bottom of the block,
                // we're colliding with the top or bottom
                if (!ball.IsFireBall)
                {
                    if ((ball.position.Y <
                        (collidedBlock.position.Y - collidedBlock.BlockHeight / 2)) ||
                        (ball.position.Y >
                        (collidedBlock.position.Y + collidedBlock.BlockHeight / 2)))
                    {
                        ball.direction.Y = -1.0f * ball.direction.Y;
                    }
                    else // otherwise, we have to be colliding from the sides
                    {
                        ball.direction.X = -1.0f * ball.direction.X;
                    }
                }

                // Now remove this block from the list, or damage block if durability >= 1
                if (collidedBlock.durability < 1)
                {
                    blocks.Remove(collidedBlock);
                    score += 20;
                }
                else
                {
                    collidedBlock.Texture = new Block(++collidedBlock.type, this).Texture;
                    collidedBlock.durability--;
                }
            }
                

            // Check walls
            if (Math.Abs(ball.position.X) < ball.Radius)
            {
                wallHitSFX.Play();
                ball.direction.X = -1.0f * ball.direction.X;
            }
            else if (Math.Abs(ball.position.X - graphics.PreferredBackBufferWidth + ball.Texture.Width * 2 ) < ball.Radius)
            {
                wallHitSFX.Play();
                ball.direction.X = -1.0f * ball.direction.X;
            }
            else if (Math.Abs(ball.position.Y) < ball.Radius)
            {
                wallHitSFX.Play();
                ball.direction.Y = -1.0f * ball.direction.Y;
            }
            else if (ball.position.Y > (graphics.PreferredBackBufferHeight + ball.Radius))
                ball.IsActive = false;

            // prevent low direction values that would slow the ball down too much
            if (!ball.IsPaddleBall)
            {
                if (ball.direction.Y <= 0.35f && ball.direction.Y >= 0)
                    ball.direction.Y += 0.2f;
                else if (ball.direction.Y >= -0.35f && ball.direction.Y <= 0)
                    ball.direction.Y -= 0.2f;
                if (ball.direction.X <= 0.35f && ball.direction.X >= 0)
                    ball.direction.X += 0.2f;
                else if (ball.direction.X >= -0.35f && ball.direction.X <= 0)
                    ball.direction.X -= 0.2f;
            }
        }


        // adjust angle of ball if it is too steep
        private void AngleCorrection(Ball ball)
        {
            // Fix the direction if it's too steep
            float dotResult = Vector2.Dot(ball.direction, Vector2.UnitX);
            if (dotResult > 0.9f)
            {
                ball.direction = new Vector2(0.906f, -0.423f);
            }
            dotResult = Vector2.Dot(ball.direction, -Vector2.UnitX);
            if (dotResult > 0.9f)
            {
                ball.direction = new Vector2(-0.906f, -0.423f);
            }
            dotResult = Vector2.Dot(ball.direction, -Vector2.UnitY);
            if (dotResult > 0.9f)
            {
                // check if clockwise or counter-clockwise
                Vector3 crossResult = Vector3.Cross(new Vector3(ball.direction, 0),
                    -Vector3.UnitY);
                if (crossResult.Z < 0)
                {
                    ball.direction = new Vector2(0.423f, -0.906f);
                }
                else
                {
                    ball.direction = new Vector2(-0.423f, -0.906f);
                }
            }
        }
        


        // spawn a new paddle and ball, and clear lists of all powerups, balls, and blocks
        protected void ClearLevel()
        {
            for (int i = powerUps.Count - 1; i >= 0; i--)
                powerUps.RemoveAt(i);

            for (int i = balls.Count - 1; i >= 0; i--)
                balls.RemoveAt(i);

            for (int i = blocks.Count - 1; i >= 0; i--) // not necessary, but added this to maybe add the option of skipping levels
                blocks.RemoveAt(i);

            paddle = new Paddle(this);
            paddle.LoadContent();
            paddle.position = new Vector2(screenWidth / 2, screenHeight - paddle.Height * 2);

            SpawnBall();
        }




        protected void CreateLevel()
        {
            startOfLevel = true;
            newLevelCounter = 0;
            level++;
            int blockDurability = 0;
            bool progressDifficulty = false;

            int[,] leastBlocks = new int[,] {
               {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
               {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
               {9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9},
               {12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12},
            };



            int[,] midBlocks = new int[,] {
               {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
               {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
               {9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9},
               {12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12},
               {15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
             };



            int[,] mostBlocks = new int[,] {
               {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
               {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
               {9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9},
               {12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12},
               {15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9},
               {12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12},
            };

            // right now just a bunch of switch statements to set levelParams, create loop later

            // set level
            switch (levelParams[(int)LevelParameter.Background])
            {
                case 0:
                    background = Content.Load<Texture2D>("sky_background");
                    break;
                case 1:
                    background = Content.Load<Texture2D>("underwater_background");
                    break;
                case 2:
                    background = Content.Load<Texture2D>("space_background");
                    break;
            }

            // set ball speed
            switch (levelParams[(int)LevelParameter.BallSpeed])
            {
                case 0:
                    Ball.DefaultSpeed = Ball.slow;
                    break;
                case 1:
                    Ball.DefaultSpeed = Ball.medium;
                    break;
                case 2:
                    Ball.DefaultSpeed = Ball.fast;
                    break;
            }

            // set paddle width
            switch (levelParams[(int)LevelParameter.PaddleWidth])
            {
                case 0:
                    paddle.Texture = Content.Load<Texture2D>("paddle");
                    break;
                case 1:
                    paddle.Texture = Content.Load<Texture2D>("paddle_2nd_smallest");
                    break;
                case 2:
                    paddle.Texture = Content.Load<Texture2D>("paddle_smallest");
                    break;
            }

            // set power-up frequency/chance of dropping a power-up
            powerUpChance = levelParams[(int)LevelParameter.PowerUpFrequency];

            // set map
            switch (levelParams[(int)LevelParameter.MapSize])
            {
                case 0:
                    for (int i = 0; i < leastBlocks.GetLength(0); i++)
                    {
                        for (int j = 0; j < leastBlocks.GetLength(1); j++)
                        {
                            Block b = new Block((BlockType)leastBlocks[i, j], this);
                            b.position = new Vector2(j * b.BlockWidth + screenWidth / screenWidthDivisor, screenHeight / screenHeightDivisor + b.BlockHeight * i);
                            b.durability = blockDurability;
                            blocks.Add(b);
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < midBlocks.GetLength(0); i++)
                    {
                        for (int j = 0; j < midBlocks.GetLength(1); j++)
                        {
                            Block b = new Block((BlockType)midBlocks[i, j], this);
                            b.position = new Vector2(j * b.BlockWidth + screenWidth / screenWidthDivisor, screenHeight / screenHeightDivisor + b.BlockHeight * i);
                            b.durability = blockDurability;
                            blocks.Add(b);
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < mostBlocks.GetLength(0); i++)
                    {
                        for (int j = 0; j < mostBlocks.GetLength(1); j++)
                        {
                            Block b = new Block((BlockType)mostBlocks[i, j], this);
                            b.position = new Vector2(j * b.BlockWidth + screenWidth / screenWidthDivisor, screenHeight / screenHeightDivisor + b.BlockHeight * i);
                            b.durability = blockDurability;
                            blocks.Add(b);
                        }
                    }
                    break;
            }

            // set block durability
            switch (levelParams[(int)LevelParameter.BlockDurability])
            {
                case 0:
                    blockDurability = 0;
                    break;
                case 1:
                    blockDurability = 1;
                    break;
                case 2:
                    blockDurability = 2;
                    break;
            }

            // set progressDifficulty
            switch (levelParams[(int)LevelParameter.ProgressDifficulty])
            {
                case 0:
                    progressDifficulty = false;
                    break;
                case 1:
                    progressDifficulty = true;
                    break;

            }


            if (progressDifficulty)
            {
                // increase ball speed
                if (level != 1)
                    Ball.DefaultSpeed += level * 10;
                // shrink paddle
                if (levelParams[(int)LevelParameter.PaddleWidth] != 2)
                    levelParams[(int)LevelParameter.PaddleWidth]++;
                // increase map size
                if (levelParams[(int)LevelParameter.MapSize] != 2)
                    levelParams[(int)LevelParameter.MapSize]++;
            }

            // rotate map backgrounds
            if (levelParams[(int)LevelParameter.Background] == 3)
                levelParams[(int)LevelParameter.Background] = 0;
            else
                levelParams[(int)LevelParameter.Background]++;

        }




        private void SpawnBall()
        {
            Ball b = new Ball(this);
            b.LoadContent();
            b.Radius = b.Texture.Width / 2;

            // level = 10; adjust to test ball speed at different levels

            if (balls.Count == 0)
            {
                b.IsPaddleBall = true;
                b.position = new Vector2(paddle.position.X, paddle.position.Y - b.Radius * 2f);
            }
            else
            {
                b.IsPaddleBall = false;
                b.IsMultiBall = true;
                b.position = new Vector2(balls[0].position.X, balls[0].position.Y);

                // slightly change the directions that the multiballs are going to separate from the original ball
                if (balls.Count < 2)
                {
                    b.direction = new Vector2(balls[0].direction.X + .15f, balls[0].direction.Y);
                    b.Speed -= 25f; // temporarily slow down multiballs to prevent balls hitting the paddle at the same time and phasing through (speed is corrected in Ball class)
                }

                else
                {
                    b.direction = new Vector2(balls[0].direction.X - .15f, balls[0].direction.Y);
                    b.Speed -= 40f;
                }
            }  
              
            balls.Add(b);
        }




        private void DropPowerUp(Vector2 blockPos)
        {
            int randNum = rand.Next(0, 100);
            PowerUpType pType = new PowerUpType();
            

            if (randNum <= 40)
                pType = PowerUpType.MultiBall;

            else if (randNum > 40 && randNum < 85)
                pType = PowerUpType.PaddleSizeIncrease;

            else if (randNum >= 85)
                pType = PowerUpType.FireBall;

            PowerUp p = new PowerUp(pType, this);
            p.position = blockPos;
            p.LoadContent();
            powerUps.Add(p);

        }



        private void ActivatePowerUp(PowerUp p, params Ball[] bList)
        {
            p.shouldRemove = true;
            p.isActive = true;
            powerUpSFX.Play();


            switch (p.type)
            {
                case PowerUpType.MultiBall:
                    if (balls.Count <= 1)   // max of 3 balls
                    {
                        SpawnBall();
                        SpawnBall();
                    }
                    else if (balls.Count == 2)
                        SpawnBall();
                     break;
                case PowerUpType.PaddleSizeIncrease:
                    paddle.LongPaddleTimer = 0f;  // reset timer if activated again

                    if (!paddle.IsLongPaddle)
                    {
                        paddle.IsLongPaddle = true;
                        paddle.Texture = Content.Load<Texture2D>("long_paddle");
                    }
                    break;
                case PowerUpType.FireBall:
                   balls[0].FireBallTimer = 0f;  

                    if (!balls[0].IsFireBall)
                    {
                        balls[0].Texture = Content.Load<Texture2D>("fireball");
                        balls[0].IsFireBall = true;
                    }
                    break;
            }
        }

        //public void SetDifficulty()
        //{
        //    if (gameDifficulty == 0)
        //    {
        //        levelParams[(int)LevelParameter.Background] = 0;
        //        levelParams[(int)LevelParameter.BallSpeed] = 0;
        //        levelParams[(int)LevelParameter.PaddleWidth] = 0;
        //        levelParams[(int)LevelParameter.PowerUpFrequency] = 50;
        //        levelParams[(int)LevelParameter.MapSize] = 0;
        //        levelParams[(int)LevelParameter.ProgressDifficulty] = 1;
        //    }
        //    else if (gameDifficulty == 1)
        //    {
        //        levelParams[(int)LevelParameter.Background] = 1;
        //        levelParams[(int)LevelParameter.BallSpeed] = 1;
        //        levelParams[(int)LevelParameter.PaddleWidth] = 1;
        //        levelParams[(int)LevelParameter.PowerUpFrequency] = 50;
        //        levelParams[(int)LevelParameter.MapSize] = 1;
        //        levelParams[(int)LevelParameter.ProgressDifficulty] = 1;
        //    }
        //    else if (gameDifficulty == 2)
        //    {
        //        levelParams[(int)LevelParameter.Background] = 2;
        //        levelParams[(int)LevelParameter.BallSpeed] = 2;
        //        levelParams[(int)LevelParameter.PaddleWidth] = 2;
        //        levelParams[(int)LevelParameter.PowerUpFrequency] = 50;
        //        levelParams[(int)LevelParameter.MapSize] = 2;
        //        levelParams[(int)LevelParameter.ProgressDifficulty] = 1;
        //    }
        //}
    }
}