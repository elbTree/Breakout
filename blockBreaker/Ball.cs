using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace blockBreaker
{
    class Ball : GameObject
    {
        float defaultSpeed = 265,
              speed = 265,
              radius,
              fireBallTimer = 0f;

        bool isActive = true, isPaddleBall = true, isFireBall = false;
        public Vector2 direction = new Vector2(0, -1);

        public Ball(Game myGame) :
            base(myGame)
        {
            textureName = "ball";
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
        public float DefaultSpeed
        {
            get { return defaultSpeed; }
            set { defaultSpeed = value; }
        }
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public bool IsPaddleBall
        {
            get { return isPaddleBall; }
            set { isPaddleBall = value; }
        }

        public bool IsFireBall
        {
            get { return isFireBall; }
            set { isFireBall = value; }
        }

        public float FireBallTimer
        {
            get { return fireBallTimer; }
            set { fireBallTimer = value; }
        }

        public void Update(float deltaTime)
        {
            position += direction * speed * deltaTime;

            if (isFireBall)
            {
                fireBallTimer += deltaTime;

                if (fireBallTimer > 10f)
                {
                    this.textureName = "ball";
                    this.LoadContent();
                    fireBallTimer = 0f;
                    isFireBall = false;
                }
            }
        }

    }
}

