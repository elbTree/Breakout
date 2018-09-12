using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

// TODO: Add constructors for ball and paddle 
// NOTES: Implement 'physics' into game. Add a new background. Grey blocks are unbreakable. 
//       Maybe manually design shape of level in combination with randomly adding unbreakable blocks.
//       Want to be able to adjust paddle width and sensetivity; Collect data (save exactly what the user is doing, and any events (like score, levels, bonuses etc.),
 //      and output to a text file). Also want to keep track of how many days or time the user is playing the game. Idea for 
namespace blockBreaker
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Paddle paddle;
        Ball ball;

        SoundEffect ballHitSFX,
                    ballBounceSFX,
                    powerUpSFX;

        int ballWithPaddle;

        // mapSize determines the number of blocks, difficulty determines the width of the paddle
        // and the speed of the ball
        int mapSize,
            difficulty,
            score = 0;

        List<Block> blocks = new List<Block>();
        Random rand;
        PowerUp powerUp;

        SpriteFont font;

        int FSscreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        int FSscreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        int windowScreenWidth = 1366;
        int windowScreenHeight = 768;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = windowScreenWidth;
            graphics.PreferredBackBufferHeight = windowScreenHeight;
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
            ball = new Ball();
            ball.position = new Vector2(graphics.PreferredBackBufferWidth / 2, 
                                       graphics.PreferredBackBufferHeight / 1.2f - 16);
            ball.Speed = 395f;
            ball.direction = new Vector2(0.707f, -0.707f);
            mapSize = 20;
            difficulty = 1;
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
            paddle.position = new Vector2(windowScreenWidth / 2, windowScreenHeight - paddle.Height * 2);

            ball.BallTexture = Content.Load<Texture2D>("ball");
            ball.Radius = ball.BallTexture.Width / 2;

            ballHitSFX = Content.Load<SoundEffect>("ball_hit");
            ballBounceSFX = Content.Load<SoundEffect>("ball_bounce");
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

            // TODO: Add your update logic here
            paddle.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            ball.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            
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
            spriteBatch.Draw(ball.BallTexture, ball.position, Color.White);

            powerUp.Draw();

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
            // Check for paddle
            if (ballWithPaddle == 0 &&
                (ball.position.X > (paddle.position.X - ball.Radius*2 - paddle.Width / 2)) && // Left, Right, and top half of paddle
                (ball.position.X < (paddle.position.X + ball.Radius*2 + paddle.Width / 2)) &&
                (ball.position.Y < paddle.position.Y) &&
                (ball.position.Y > (paddle.position.Y - ball.Radius*2 - paddle.Height / 2)))
            {
                ballBounceSFX.Play();

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

                if (pct < 0.33f)
                    normal = new Vector2(-0.196f, -0.981f);
                //   ball.direction = new Vector2(-1, -0.981f); 

                else if (pct > 0.66f)
                    normal = new Vector2(0.196f, -0.981f);
                // ball.direction = new Vector2(1, -0.981f);


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
                ballHitSFX.Play();

                rand = Random(0, 100);
                
                if (rand > 80) 
                    DropPowerUp(collidedBlock.position);
                
                score += 10;
                
                // Assume that if our Y is close to the top or bottom of the block,
                // we're colliding with the top or bottom
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

                // Now remove this block from the list
                blocks.Remove(collidedBlock);
            }

            // Check walls
            if (Math.Abs(ball.position.X) < ball.Radius)
            {
                ballBounceSFX.Play();
                ball.direction.X = -1.0f * ball.direction.X;
            }
            else if (Math.Abs(ball.position.X - graphics.PreferredBackBufferWidth) < ball.Radius)
            {
                ballBounceSFX.Play();
                ball.direction.X = -1.0f * ball.direction.X;
            }
            else if (Math.Abs(ball.position.Y) < ball.Radius)
            {
                ballBounceSFX.Play();
                ball.direction.Y = -1.0f * ball.direction.Y;
            }
            else if (ball.position.Y > (graphics.PreferredBackBufferHeight + ball.Radius))
            {
                // respawn ball
            }
            
        }

        
        // Currently just loads a simple rectangle level, need to create different levels using jagged arrays
        // and once all the shapes have been played through once, it will loop with a different background
        // and an increased difficulty (slightly faster ball, more durable/unbreakable blocks, slightly lower powerup frequency)
        protected void CreateLevel()
        {
            int[,] blockLayout = new int[,]{
               {5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
               {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
               {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
               {4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
               {5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            };

            for (int i = 0; i < blockLayout.GetLength(0); i++)
            {
                for (int j = 0; j < blockLayout.GetLength(1); j++)
                {
                    Block b = new Block((BlockColor)blockLayout[i, j], this); 
                    b.position = new Vector2(j * b.BlockWidth + windowScreenWidth / 6, windowScreenHeight / 6 + b.BlockHeight * i);
                    blocks.Add(b);
                }
            }
        }

        protected void DropPowerUp(Vector2 blockPos)
        {
            rand = Random(0, 120);
            PowerUpType pType;
            

            if (rand <= 20)
                pType = PowerUpType.MultiBall;
            
            else if (rand > 20 && rand <= 40)
                pType = PowerUpType.PaddleSizeIncrease;
            
            else if (rand > 40 && rand <= 60)
                pType = PowerUpType.Lasers;
            
            else if (rand > 60 && rand <= 80)
                pType = PowerUpType.FireBall;
            
            else if (rand > 80 && rand <= 100)
                pType = PowerUpType.FastBall;
            
             else
                pType = PowerUpType.PaddleSizeDecrease;

            powerUp = new PowerUp(pType, myGame);
            powerUp.position = blockPos;
        }
    }
}