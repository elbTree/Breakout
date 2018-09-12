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
    public enum PowerUpType
    {
        MultiBall = 0,
        PaddleSizeIncrease,
        Lasers,
        FireBall,
        FastBall,
        PaddleSizeDecrease
    }

    public class PowerUp : GameObject
    {
        public PowerUpType type;
        public float speed = 400;
        public bool shouldRemove = false;
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
                case (PowerUpType.Lasers):
                    textureName = "laser_powerUp";
                    break;
                case (PowerUpType.FireBall):
                    textureName = "fireball_powerUp";
                    break;
                case (PowerUpType.FastBall):
                    textureName = "fastball_powerUp";
                    break;
                case (PowerUpType.PaddleSizeDecrease):
                    textureName = "decrease_paddle_width_powerUp";
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
