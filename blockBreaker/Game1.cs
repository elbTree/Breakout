using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

// TODO: Add constructors for ball and paddle 
// NOTES: Implement 'physics' into game. Add a new background. Grey blocks are unbreakable. 
//       Maybe manually design shape of level in combination with randomly adding unbreakable blocks.
//       Want to be able to adjust paddle width and sensetivity;
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
        int ballWithPaddle;
        // mapSize determines the number of blocks, difficulty determines strength of blocks 
        // and how many strong/unbreakable blocks there are
        int mapSize, difficulty;
        List<Block> blocks = new List<Block>();
        Ball ball;
        Random rand;

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
            ball = new Ball();
            paddle = new Paddle();
            paddle.position = new Vector2(graphics.PreferredBackBufferWidth / 4, // CHANGE TO / 2
                                         graphics.PreferredBackBufferHeight / 1.2f);
            ball.position = new Vector2(graphics.PreferredBackBufferWidth / 2, 
                                       graphics.PreferredBackBufferHeight / 1.2f - 16); // draw on top of paddle (-y is up)
            paddle.Speed = 145f;
            ball.Speed = 145f;
            ball.direction = new Vector2(0.707f, -0.707f);
            mapSize = 20;
            difficulty = 1;
            rand = new Random();
            map = new Block[mapSize,mapSize];
            
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

            for (int i = 0; i < mapSize; i++)
            {
                int randNum = rand.Next(0, 100);

                for (int j = 0; j < mapSize; j++)
                {
                    if (randNum >= 75)
                        map[i,j] = new Block(Content.Load<Texture2D>("BlueBlockFX"));
                    else if (randNum < 75 && randNum >= 50)
                        map[i,j] = new Block(Content.Load<Texture2D>("GreenBlockFX"));
                    else if (randNum < 50 && randNum >= 25)
                        map[i,j] = new Block(Content.Load<Texture2D>("RedBlockFX"));
                    else
                        map[i,j] = new Block(Content.Load<Texture2D>("OrangeBlockFX"));

                    randNum = rand.Next(0, 100);
                }
            }
            paddle.PaddleTexture = Content.Load<Texture2D>("paddle");
            paddle.Width = paddle.PaddleTexture.Width;
            paddle.Height = paddle.PaddleTexture.Height;
            ball.BallTexture = Content.Load<Texture2D>("ball");
            ball.Radius = ball.BallTexture.Width / 2;
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
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                paddle.position.X += paddle.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                paddle.position.X -= paddle.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            

            ball.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
            
            CheckCollisions();
            paddle.position.X = MathHelper.Clamp(paddle.position.X, 0, 
                                                graphics.PreferredBackBufferWidth - paddle.Width);
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

            for (int i = 0; i < mapSize / 2; i++)
                for (int j = 0; j < 5; j++)
                    spriteBatch.Draw(map[i,j].BlockType, new Rectangle(i * 50 + graphics.PreferredBackBufferWidth / 6, 
                                                                       j * 25, 50, 25), Color.White);
                

            spriteBatch.Draw(paddle.PaddleTexture, paddle.position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(ball.BallTexture, ball.position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void CheckCollisions()
        {
            // Check for paddle
            if (ballWithPaddle == 0 &&
                (ball.position.X > (paddle.position.X - ball.Radius - paddle.Width / 12)) && // Left, Right, and top half of paddle
                (ball.position.X < (paddle.position.X + ball.Radius + paddle.Width)) &&
                (ball.position.Y < paddle.position.Y) &&
                (ball.position.Y > (paddle.position.Y - ball.Radius - paddle.Height / 2)))
            {
                // Reflect based on which part of the paddle is hit

                // By default, set the normal to up
                Vector2 normal = -1.0f * Vector2.UnitY;

                // Distance from the leftmost to rightmost part of the paddle
                float dist = paddle.Width + ball.Radius * 2;
                // Where within this distance the ball is at
                float ballLocation = ball.position.X -
                    (paddle.position.X - ball.Radius - paddle.Width / 2);
                // Percent between leftmost and rightmost part of paddle
                float pct = ballLocation / dist;

                if (pct < 0.33f)
                {
                    normal = new Vector2(-0.196f, -0.981f);
                }
                else if (pct > 0.66f)
                {
                    normal = new Vector2(0.196f, -0.981f);
                }

                ball.direction = Vector2.Reflect(ball.direction, normal);
                // No collisions between ball and paddle for 20 frames
                ballWithPaddle = 20;
            }
            else if (ballWithPaddle > 0)
            {
                ballWithPaddle--;
            }
            // Check walls
            if (Math.Abs(ball.position.X) < ball.Radius / 4)
            {
                ball.direction.X = -1.0f * ball.direction.X;
            }
            else if (Math.Abs(ball.position.X - graphics.PreferredBackBufferWidth) < ball.Radius * 2)
            {
                ball.direction.X = -1.0f * ball.direction.X;
            }
            else if (Math.Abs(ball.position.Y) < ball.Radius)
            {
                ball.direction.Y = -1.0f * ball.direction.Y;
            }
            else if (ball.position.Y > (graphics.PreferredBackBufferHeight + ball.Radius))
            {
               // LoseLife();
            }
        }
    }
}
