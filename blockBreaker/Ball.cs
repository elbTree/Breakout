using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace blockBreaker
{
    class Ball
    {
        Texture2D ballTexture;
        float speed = 125f, radius;
        Vector2 position, direction;

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
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        // still need to add constructors
    }
}

