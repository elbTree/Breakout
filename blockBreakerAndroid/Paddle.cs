using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FitMi_Research_Puck;

namespace blockBreakerAndroid
{
    class Paddle : GameObject
    {
        HIDPuckDongle puckDongle = new HIDPuckDongle();
        float speed = 500;
        bool isLongPaddle = false, 
             bluePuckPressed = false, 
             yellowPuckPressed = false;
        float longPaddleTimer = 0f;

        public Paddle(Game myGame) :
            base(myGame)
        {
            textureName = "paddle";
            puckDongle.Open();
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

            puckDongle.CheckForNewPuckData();

            // grab values from pressing on the pucks ~500 = 0, ~1000 is max
            var bluePuckLoad = puckDongle.PuckPack0.Loadcell;
            var yellowPuckLoad = puckDongle.PuckPack1.Loadcell;

            // puck accelerometer values
            var bluePuckAccX = puckDongle.PuckPack0.Accelerometer[0];
            var bluePuckAccY = puckDongle.PuckPack0.Accelerometer[1];
            var bluePuckAccZ = puckDongle.PuckPack0.Accelerometer[2];

            var yellowPuckAccX = puckDongle.PuckPack1.Accelerometer[0];
            var yellowPuckAccY = puckDongle.PuckPack1.Accelerometer[1];
            var yellowPuckAccZ = puckDongle.PuckPack1.Accelerometer[2];

            // puck gyrometer values
            var bluePuckGyroX = puckDongle.PuckPack0.Gyrometer[0];
            var bluePuckGyroY = puckDongle.PuckPack0.Gyrometer[1];
            var bluePuckGyroZ = puckDongle.PuckPack0.Gyrometer[2];

            var yellowPuckGyroX = puckDongle.PuckPack1.Gyrometer[0];
            var yellowPuckGyroY = puckDongle.PuckPack1.Gyrometer[1];
            var yellowPuckGyroZ = puckDongle.PuckPack1.Gyrometer[2];


            if (bluePuckLoad >= 550)
            {
                bluePuckPressed = true;
                position.X -= speed * deltaTime;
            }
            else
                bluePuckPressed = false;

            if (yellowPuckLoad >= 550)
            {
                yellowPuckPressed = true;
                position.X += speed * deltaTime;
            }
            else
                yellowPuckPressed = false;

            if ((bluePuckPressed && yellowPuckPressed) && !Game1.startOfLevel)
            {
                Ball.isPaddleBall = false;
                bluePuckLoad = 500;
                yellowPuckLoad = 500;
            }

            if (bluePuckAccX <= -10f)
                position.X += speed * deltaTime;
            if (bluePuckAccX >= 10f)
                position.X -= speed * deltaTime;

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
