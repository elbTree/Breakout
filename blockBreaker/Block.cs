using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace blockBreaker
{

    public enum BlockColor
    {
        Red = 0,
        Yellow,
        Blue,
        Green,
        Purple,
        Orange,
        Grey,
        Rainbow
    }

    class Block
    {

        // durability determines how many hits to break the block
        // status indicates whether the block is fractured/shattered
        private int durability, status;
        public int BlockWidth = 50;
        public int BlockHeight = 20;
        public Vector2 position;
        Texture2D texture;
        BlockColor color;

        public Block(BlockColor myColor, Game myGame)
        {
           
            color = myColor;
            switch (color)
            {
                case (BlockColor.Red):
                    texture = myGame.Content.Load<Texture2D>("RedBlockFX");
                    break;
                case (BlockColor.Yellow):
                    texture = myGame.Content.Load<Texture2D>("YellowBlockFX");
                    break;
                case (BlockColor.Blue):
                    texture = myGame.Content.Load<Texture2D>("BlueBlockFX");
                    break;
                case (BlockColor.Green):
                    texture = myGame.Content.Load<Texture2D>("GreenBlockFX");
                    break;
                case (BlockColor.Purple):
                    texture = myGame.Content.Load<Texture2D>("PurpleBlockFX");
                    break;
                case (BlockColor.Orange):
                    texture = myGame.Content.Load<Texture2D>("OrangeBlockFX");
                    break;
                case (BlockColor.Grey):
                    texture = myGame.Content.Load<Texture2D>("GrayBlockFX");
                    break;
                case (BlockColor.Rainbow):
                    texture = myGame.Content.Load<Texture2D>("RainbowBlockFX");
                    break;
            }
        }

        public Texture2D Texture
        {
            get { return texture; }

            set { texture = value; }
        }

        public int Durability
        {
            get { return durability; }

            set { durability = value; }
        }

        public int Status
        {
            get { return status; }

            set { status = value; }
        }

        public Block(Texture2D t) { texture = t; }

    }

}