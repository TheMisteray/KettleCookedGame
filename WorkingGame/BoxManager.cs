using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace WorkingGame
{
    //BoxManager class
    //Manages the behavior of all the boxes within a level
    internal class BoxManager
    {
        //Fields
        private List<Box> boxes;

        //Properties
        /// <summary>
        /// Read-only property for the list of boxes in the level
        /// </summary>
        public List<Box> Boxes
        {
            get => boxes;
        }

        /// <summary>
        /// Read-only property for the number of boxes in the level
        /// </summary>
        public int Count
        {
            get => boxes.Count;
        }

        //Constructor
        /// <summary>
        /// Constructs a new instance of BoxManager, being the initialization
        /// of the list of boxes within the current level
        /// </summary>
        public BoxManager()
        {
            boxes = new List<Box>();
        }

        //Methods
        /// <summary>
        /// Adds a new box to the list of boxes
        /// </summary>
        /// <param name="box">The box to be added</param>
        public void Add(Box box)
        {
            boxes.Add(box);
        }

        /// <summary>
        /// Fully clears the list of boxes
        /// </summary>
        public void Clear()
        {
            boxes.Clear();
        }

        /// <summary>
        /// Draws each box to the screen
        /// </summary>
        /// <param name="sb">The sprite batch object</param>
        /// <param name="offset">The screen offset due to scrolling</param>
        public void Draw(SpriteBatch sb, Vector2 offset)
        {
            foreach (Box b in boxes)
                b.Draw(sb, offset);
        }

        /// <summary>
        /// Checks whether any boxes in the list of boxes have collided with an object
        /// </summary>
        /// <param name="otherRect">The rectangle of the other object</param>
        /// <returns>The box that collided with the object, returns null if no
        /// boxes collided with the object</returns>
        public Box CheckCollision(Rectangle otherRect)
        {
            foreach (Box b in boxes)
            {
                if (b.Rect.Intersects(otherRect))
                    return b;
            }
            return null;
        }

        /// <summary>
        /// Updates each box in the list of boxes
        /// </summary>
        /// <param name="player"></param>
        public void Update(Player player)
        {
            foreach (Box b in boxes)
                b.Update(player);
        }
    }
}