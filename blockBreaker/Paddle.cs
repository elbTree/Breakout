using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace blockBreaker
{
    class Paddle : GameObject
    {
        Texture2D paddleTexture;
        float speed = 500;

        public Paddle(Game myGame) :
            base(myGame)
        {
            textureName = "paddle";
        }

        public Texture2D PaddleTexture
        {
            get { return paddleTexture; }
            set { paddleTexture = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public override void Update(float deltaTime)
        {
            float screenWidth = Game1.graphics.PreferredBackBufferWidth;
            // Paddle movement
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Left))
            {
                position.X -= speed * deltaTime;
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                position.X += speed * deltaTime;
            }
            // Clamp paddle to valid range
            position.X = MathHelper.Clamp(position.X, texture.Width / 2, screenWidth - texture.Width / 2); 
            base.Update(deltaTime);
        }

    }
}
