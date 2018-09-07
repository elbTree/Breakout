using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace blockBreaker
{

    class Block
    {
        // durability determines how many hits to break the block
        // status indicates whether the block is fractured/shattered
        private int durability, status;
        Texture2D blockType;
        public Vector2 position;
        public int BlockWidth = 50;
        public int BlockHeight = 25;

        public Texture2D BlockType
        {
            get { return blockType; }

            set { blockType = value; }
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

        public Block(Texture2D bType, int dur, int stat) { blockType = bType; durability = dur; status = stat; }

        public Block(Texture2D bType) { blockType = bType; }
    }

}
