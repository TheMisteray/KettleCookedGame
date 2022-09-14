using Microsoft.Xna.Framework.Graphics;

namespace WorkingGame
{
    internal class FloorTile : Tile
    {
        /// <summary>
        /// inheritance to help filter for level manager
        /// </summary>
        /// <param name="tilePosX"></param>
        /// <param name="tilePosY"></param>
        /// <param name="tex"></param>
        public FloorTile(int tilePosX, int tilePosY, Texture2D tex)
            : base(tilePosX, tilePosY, tex) { }
    }
}