using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace blockBreaker
{
    class Ball
    {
        Texture2D ballTexture;
        float defaultSpeed = 400;
        float speed = 400, radius;
        bool isActive = true, isPaddleBall = true;
        public Vector2 position, direction = new Vector2(-.747f,.747f);

        public Texture2D BallTexture
        {
            get { return ballTexture; }
            set { ballTexture = value; }
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

        public void Update(float deltaTime)
        {
            position += direction * speed * deltaTime;
        }

        public Ball(Texture2D bTexture) { ballTexture = bTexture; }
        
    }
}

