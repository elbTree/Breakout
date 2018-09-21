using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

// NOTES: Want to be able to adjust paddle width and sensetivity; Collect data (save exactly what the user is doing, and any events (like score, levels, bonuses etc.),
//        and output to a text file). Also want to keep track of how many days or time the user is playing the game.
namespace blockBreaker
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

        List<Block> blocks = new List<Block>();
        List<Ball> balls = new List<Ball>();
        List<PowerUp> powerUps = new List<PowerUp>();
        Random rand;
        
        SpriteFont font;

        int screenWidth = 1366;
        int screenHeight = 768;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
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

            rand = new Random();
            
            
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

            paddle = new Paddle(this);
            paddle.LoadContent();
            paddle.position = new Vector2(screenWidth / 2, screenHeight - paddle.Height * 2);

            SpawnBall();

            blockHitSFX = Content.Load<SoundEffect>("high_beep");
            paddleHitSFX = Content.Load<SoundEffect>("low_beep");
            wallHitSFX = Content.Load<SoundEffect>("mid_beep");
           // fireBallSFX = Content.Load<SoundEffect>("fire_ball_sound");
            powerUpSFX = Content.Load<SoundEffect>("powerup");

          //  font = Content.Load<SpriteFont>("Arial");

            CreateLevel();

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                foreach (Ball b in balls)
                    b.IsPaddleBall = false;
            }


            // TODO: Add your update logic here
            paddle.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            foreach (Ball b in balls)
            {
                if (b.IsActive && b.IsPaddleBall)
                {
                    b.Speed = paddle.Speed;
                    b.position = new Vector2(paddle.position.X, paddle.position.Y - b.Radius * 2.2f);
                    b.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    b.Speed = b.DefaultSpeed;
                    b.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }

            foreach (PowerUp p in powerUps)
            {
                Rectangle paddlePos = paddle.BoundingRect;

                if (!p.shouldRemove)
                    p.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                if (paddlePos.Intersects(p.BoundingRect) && (!p.isActive))
                    ActivatePowerUp(p);
            }

            for(int i = 0; i < powerUps.Count; i++)
            {
                if (powerUps[i].isActive)
                    powerUps.RemoveAt(i);
            }

            CheckCollisions();
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
            spriteBatch.Begin();

            foreach (Block b in blocks)
                spriteBatch.Draw(b.Texture, new Rectangle((int)b.position.X,(int)b.position.Y, (int)b.BlockWidth, (int)b.BlockHeight), Color.White);


            paddle.Draw(spriteBatch);

            foreach (Ball b in balls)
            {
                if (b.IsActive)
                    spriteBatch.Draw(b.Texture, b.position, Color.White);
            }
            foreach (PowerUp p in powerUps)
            {
                if (!p.shouldRemove)
                    p.Draw(spriteBatch);
            }
            //spriteBatch.DrawString(font, String.Format("Score: {0:#,###0}", score),
            //           new Vector2(40, 50), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

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

        protected void CheckCollisions()
        {
            Ball lostBall = null;

            foreach (Ball ball in balls)
            {
                // Check for paddle
                if ((ballWithPaddle == 0 &&
                    (ball.position.X > (paddle.position.X - ball.Radius - paddle.Width / 2)) && // Left, Right, and top half of paddle
                    (ball.position.X < (paddle.position.X + ball.Radius + paddle.Width / 2)) &&
                    (ball.position.Y < paddle.position.Y) &&
                    (ball.position.Y > (paddle.position.Y - ball.Radius - paddle.Height / 2))))
                {
                    if (!ball.IsPaddleBall)
                        paddleHitSFX.Play();

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
                        normal = new Vector2(-0.098f, -0.981f);
                    //   ball.direction = new Vector2(-1, -0.981f); 
                    else if (pct > 0.40f && pct <= .60f)            // middle
                        normal = new Vector2(0, -0.981f);
                    // ball.direction = new Vector2(1, -0.981f);
                    else if (pct > .60f && pct <= .80)              // right
                        normal = new Vector2(0.098f, -0.981f);
                    else                                            // far right
                        normal = new Vector2(0.196f, -0.981f);

                    
                    int randVal = rand.Next(0, 100);
                    
                    // prevent ball from bouncing from wall to wall with no change in the Y direction
                    if (ball.direction.Y == 0)
                    {
                        if (randVal > 50)
                            ball.direction.Y = -0.2f;
                        else
                            ball.direction.Y = 0.2f;
                        
                    }
                    // prevent ball from going straight up/down with no change in the X direction
                    if (ball.direction.X == 0)
                    {
                        if (randVal > 50)
                            ball.direction.X = -.1f;
                        else
                            ball.direction.X = .1f;
                    }

                    ball.direction = Vector2.Reflect(ball.direction, normal);

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

                // Determine ball reflection
                if (collidedBlock != null)
                {
                    blockHitSFX.Play();
                    int randNum = rand.Next(0, 100);

                    if (randNum >= 80 && (powerUps.Count <= 3))  // max of 4 powerups dropped at a time
                        DropPowerUp(collidedBlock.position);

                        score += 10;

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
                    // Now remove this block from the list
                    if (collidedBlock.Durability < 1)
                        blocks.Remove(collidedBlock);
                }

                // Check walls
                if (Math.Abs(ball.position.X) < ball.Radius)
                {
                    wallHitSFX.Play();
                    ball.direction.X = -1.0f * ball.direction.X;
                }
                else if (Math.Abs(ball.position.X - graphics.PreferredBackBufferWidth) < ball.Radius)
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
                {
                    lostBall = ball;
                    ball.IsActive = false;
                }
            }
            if (lostBall != null)
            {
                balls.Remove(lostBall);

                if (balls.Count == 0)
                    SpawnBall();
            }
        }

        
        // Currently just loads a simple rectangle level, need to create different levels using jagged arrays
        // and once all the shapes have been played through once, it will loop with a different background
        // and a slightly increased difficulty (slightly faster ball, more durable/unbreakable blocks)
        // Map ideas: triangle/level with sandy background, spaceship with space background, hexagon
        protected void CreateLevel()
        {
            int[,] blockLayout = new int[,] {
               {5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
               {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
               {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
               {4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
               {5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
               {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            };

            for (int i = 0; i < blockLayout.GetLength(0); i++)
            {
                for (int j = 0; j < blockLayout.GetLength(1); j++)
                {
                    Block b = new Block((BlockColor)blockLayout[i, j], this); 
                    b.position = new Vector2(j * b.BlockWidth + screenWidth / 12, screenHeight / 6 + b.BlockHeight * i);
                    blocks.Add(b);
                }
            }
        }

        private void SpawnBall()
        {
            Ball b = new Ball(this);
            b.LoadContent();
            b.Radius = b.Texture.Width / 2;

            if (balls.Count == 0)
                b.position = new Vector2(paddle.position.X, paddle.position.Y - b.Radius * 2.2f);
            else
            {
                b.IsPaddleBall = false;
                b.position = new Vector2(balls[0].position.X, balls[0].position.Y);
            }  
            balls.Add(b);
        }

        private void DropPowerUp(Vector2 blockPos)
        {
            int randNum = rand.Next(0, 80);
            PowerUpType pType = new PowerUpType();


            if (randNum <= 20)
                pType = PowerUpType.MultiBall;

            else if (randNum > 20 && randNum <= 60)
                pType = PowerUpType.PaddleSizeIncrease;

            else if (randNum > 0)
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


            // will implement this later, it will be used to determine how to activate the powerup
            switch (p.type)
            {
                case PowerUpType.MultiBall:
                    if (balls.Count <= 5)   // max of 6 balls
                    {
                        SpawnBall();
                        SpawnBall();
                    }
                    break;
                case PowerUpType.PaddleSizeIncrease:
                    paddle.LongPaddleTimer = 0f;

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
    }
}