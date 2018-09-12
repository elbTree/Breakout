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
        float speed = 400, radius;
        public Vector2 position, direction = new Vector2(0.707f, -0.707f);

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
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public void Update(float deltaTime)
        {
            position += direction * speed * deltaTime;
        }
        
    }
}

