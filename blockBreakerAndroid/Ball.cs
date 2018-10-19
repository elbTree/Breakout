using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace blockBreakerAndroid
{
    class Ball : GameObject
    {
        static public float DefaultSpeed = 150;
        float speed,
              radius,
              fireBallTimer = 0f, multiBallTimer = 0f;
        static public int slow = 125,
                          medium = 150,
                          fast = 175;
        static public bool isPaddleBall = true;
        bool isActive = true, isFireBall = false, isMultiBall = false;
        public Vector2 direction = new Vector2(0, -1);

        public Ball(Game myGame) :
            base(myGame)
        {
            textureName = "ball";
            speed = DefaultSpeed;
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

        public bool IsMultiBall
        {
            get { return isMultiBall; }
            set { isMultiBall = value; }
        }

        public float FireBallTimer
        {
            get { return fireBallTimer; }
            set { fireBallTimer = value; }
        }

        public override void Update(float deltaTime)
        {
            if (!isMultiBall)
                position += direction * DefaultSpeed * deltaTime;
            else
                position += direction * speed * deltaTime;  // multiball uses speed because they are temporarily (5s) slower when first spawned

            if (isFireBall)
            {
                fireBallTimer += deltaTime;

                if (fireBallTimer > 15f)
                {
                    this.textureName = "ball";
                    this.LoadContent();
                    fireBallTimer = 0f;
                    isFireBall = false;
                }
            }
            if (isMultiBall)
            {
                multiBallTimer += deltaTime;

                if (multiBallTimer > 5f)
                    speed = DefaultSpeed;
            }

        }

    }
}

