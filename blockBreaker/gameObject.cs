using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace blockBreaker
{
    public class GameObject
    {
        protected string textureName = "";
        protected Texture2D texture;
        protected Game game;
        public Vector2 position;

        public float Width
        {
            get { return texture.Width; }
        }
        

        public float Height
        {
            get { return texture.Height; }
        }

        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle((int)(position.X - Width / 2),
                    (int)(position.Y + Height / 2),
                    (int)Width,
                    (int)Height);
            }
        }

        public GameObject(Game myGame)
        {
            game = myGame;
        }

        public virtual void LoadContent()
        {
            if (textureName != "")
            {
                texture = game.Content.Load<Texture2D>(textureName);
            }
        }

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void Draw(SpriteBatch batch)
        {
            if (texture != null)
            {
                Vector2 drawPosition = position;
                drawPosition.X -= texture.Width / 2;
                drawPosition.Y -= texture.Height / 2;
                batch.Draw(texture, drawPosition, Color.White);
            }
        }
    }
}