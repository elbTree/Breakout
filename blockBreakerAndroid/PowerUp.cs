using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace blockBreakerAndroid
{
    public enum PowerUpType
    {
        MultiBall = 0,
        PaddleSizeIncrease,
        FireBall
    }

    public class PowerUp : GameObject
    {
        public PowerUpType type;
        public static float speed = 150;
        public bool shouldRemove = false,
                    isActive = false;
        public int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        public PowerUp(PowerUpType myType, Game myGame) :
            base(myGame)
        {
            type = myType;

            switch (type)
            {
                case (PowerUpType.MultiBall):
                    textureName = "multi_ball_powerUp";
                    break;
                case (PowerUpType.PaddleSizeIncrease):
                    textureName = "wide_paddle_powerUp";
                    break;
                case (PowerUpType.FireBall):
                    textureName = "fireball_powerUp";
                    break;
            }
        }

        public override void Update(float deltaTime)
        {
            position.Y += speed * deltaTime;
            if (position.Y > (screenHeight + Height / 2))
            {
                shouldRemove = true;
            }
            base.Update(deltaTime);
        }
    }
}
