using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
        Texture2D paddleType;
        Vector2 paddlePosition;
        // mapSize determines the number of blocks, difficulty determines strength of blocks 
        // and how many strong/unbreakable blocks there are
        int mapSize, difficulty;
        Block[,] map; // holds array of blocks
        Ball ball;
        float paddleSpeed;
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
            paddlePosition = new Vector2(graphics.PreferredBackBufferWidth / 2, 
                                         graphics.PreferredBackBufferHeight / 1.2f);
            ball.Position = new Vector2(graphics.PreferredBackBufferWidth / 2, 
                                       graphics.PreferredBackBufferHeight / 1.2f - 16); // draw on top of paddle (-y is up)
            paddleSpeed = 125f;
            ball.Speed = 125f;
            ball.Direction = new Vector2(0.707f, -0.707f);
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
            paddleType = Content.Load<Texture2D>("paddle");
            ball.BallTexture = Content.Load<Texture2D>("ball");
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
                paddlePosition.X += paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                paddlePosition.X -= paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            

            ball.Position += ball.Direction * ball.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            paddlePosition.X = MathHelper.Clamp(paddlePosition.X, 0, 
                                                graphics.PreferredBackBufferWidth - (paddleType.Width));
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
                

            spriteBatch.Draw(paddleType, paddlePosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(ball.BallTexture, ball.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
