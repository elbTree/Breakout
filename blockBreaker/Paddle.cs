using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace blockBreaker
{
    class Paddle
    {
        Texture2D paddleTexture;
        Vector2 position;
        float speed;
        int height, width;

        public Texture2D PaddleTexture
        {
            get { return paddleTexture; }
            set { paddleTexture = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        // still need to add constructors
    }
}
