using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace blockBreaker
{
    class Paddle : GameObject
    {
        float speed = 500;
        bool isLongPaddle = false;
        float longPaddleTimer = 0f;

        public Paddle(Game myGame) :
            base(myGame)
        {
            textureName = "paddle";
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public float LongPaddleTimer
        {
            get { return longPaddleTimer; }
            set { longPaddleTimer = value; }
        }

        public bool IsLongPaddle
        {
            get { return isLongPaddle; }
            set { isLongPaddle = value; }
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

            if (isLongPaddle)
            {
                longPaddleTimer += deltaTime;

                if (longPaddleTimer > 10f)
                {
                    textureName = "paddle";
                    this.LoadContent();
                    longPaddleTimer = 0;
                    isLongPaddle = false;
                }  
            }

            base.Update(deltaTime);
        }

    }
}
