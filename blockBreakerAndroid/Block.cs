using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace blockBreakerAndroid
{

    public enum BlockType
    {
        Red = 0,
        RedShattering,
        RedFractured,
        Yellow, //3
        YellowShattering,
        YellowFractured,
        Blue, // 6
        BlueShattering,
        BlueFractured,
        Green, // 9
        GreenShattering,
        GreenFractured,
        Purple, // 12
        PurpleShattering,
        PurpleFractured,
        Orange, // 15
        OrangeShattering,
        OrangeFractured,
        Grey, // 18
        GreyShattering,
        GreyFractured,
        Rainbow // 21
    }

    class Block
    {

        // durability determines how many hits to break the block
        public int durability = 0;
        public int BlockWidth = 50;
        public int BlockHeight = 20;
        public Vector2 position;
        Texture2D texture;
        public BlockType type;

        public Block(BlockType t, Game myGame)
        {
           
            type = t;
            switch (type)
            {
                case (BlockType.Red):
                    texture = myGame.Content.Load<Texture2D>("RedBlockFX");
                    break;
                case (BlockType.RedShattering):
                    texture = myGame.Content.Load<Texture2D>("RedBlockShatteringFX");
                    break;
                case (BlockType.RedFractured):
                    texture = myGame.Content.Load<Texture2D>("RedBlockFracturedFX");
                    break;

                case (BlockType.Yellow):
                    texture = myGame.Content.Load<Texture2D>("YellowBlockFX");
                    break;
                case (BlockType.YellowShattering):
                    texture = myGame.Content.Load<Texture2D>("YellowBlockShatteringFX");
                    break;
                case (BlockType.YellowFractured):
                    texture = myGame.Content.Load<Texture2D>("YellowBlockFracturedFX");
                    break;

                case (BlockType.Blue):
                    texture = myGame.Content.Load<Texture2D>("BlueBlockFX");
                    break;
                
                case (BlockType.BlueShattering):
                    texture = myGame.Content.Load<Texture2D>("BlueBlockShatteringFX");
                    break;
                case (BlockType.BlueFractured):
                    texture = myGame.Content.Load<Texture2D>("BlueBlockFracturedFX");
                    break;
                case (BlockType.Green):
                    texture = myGame.Content.Load<Texture2D>("GreenBlockFX");
                    break;
                
                case (BlockType.GreenShattering):
                    texture = myGame.Content.Load<Texture2D>("GreenBlockShatteringFX");
                    break;
                case (BlockType.GreenFractured):
                    texture = myGame.Content.Load<Texture2D>("GreenBlockFracturedFX");
                    break;
                case (BlockType.Purple):
                    texture = myGame.Content.Load<Texture2D>("PurpleBlockFX");
                    break;
                
                case (BlockType.PurpleShattering):
                    texture = myGame.Content.Load<Texture2D>("PurpleBlockShatteringFX");
                    break;
                case (BlockType.PurpleFractured):
                    texture = myGame.Content.Load<Texture2D>("PurpleBlockFracturedFX");
                    break;
                case (BlockType.Orange):
                    texture = myGame.Content.Load<Texture2D>("OrangeBlockFX");
                    break;
                 case (BlockType.OrangeShattering):
                    texture = myGame.Content.Load<Texture2D>("OrangeBlockShatteringFX");
                    break;
                case (BlockType.OrangeFractured):
                    texture = myGame.Content.Load<Texture2D>("OrangeBlockFracturedFX");
                    break;

                case (BlockType.Grey):
                    texture = myGame.Content.Load<Texture2D>("GrayBlockFX");
                    break;
                
                case (BlockType.GreyShattering):
                    texture = myGame.Content.Load<Texture2D>("GrayBlockShatteringFX");
                    break;
                case (BlockType.GreyFractured):
                    texture = myGame.Content.Load<Texture2D>("GrayBlockFracturedFX");
                    break;
                case (BlockType.Rainbow):
                    texture = myGame.Content.Load<Texture2D>("RainbowBlockFX");
                    break;
            }
        }
        

        public Texture2D Texture
        {
            get { return texture; }

            set { texture = value; }
        }

        public Block(Texture2D t) { texture = t; }

    }

}