using Microsoft.Xna.Framework.Graphics;

namespace WorkingGame
{
    internal class GoalTile : FloorTile
    {
        //Fields
        private bool filled;

        //Properties
        public bool Filled
        {
            get => filled;
            set => filled = value;
        }

        /// <summary>
        /// special tile for placing boxes into
        /// </summary>
        /// <param name="tilePosX"></param>
        /// <param name="tilePosY"></param>
        /// <param name="tex"></param>
        public GoalTile(int tilePosX, int tilePosY, Texture2D tex)
            : base(tilePosX, tilePosY, tex)
        {
            filled = false;
        }
    }
}