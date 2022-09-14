using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WorkingGame
{
    //Box class
    //Handles behavior of the box objects used throughout the game
    internal class Box : IGameObject
    {
        //Fields
        private Rectangle rect;

        private Rectangle rotatedRect;
        private Texture2D tex;
        private bool isHeld;
        private bool inGoal;
        private float angle;

        //Properties
        /// <summary>
        /// Read and write property for the Rectangle object representing the box
        /// </summary>
        public Rectangle Rect
        {
            get => rect;
            set => rect = value;
        }

        /// <summary>
        /// Read and write property for the box's texture
        /// </summary>
        public Texture2D Tex
        {
            get => tex;
            set => tex = value;
        }

        /// <summary>
        /// Read and write property for whether this box is being held by the player
        /// </summary>
        public bool Held
        {
            get => isHeld;
            set => isHeld = value;
        }

        /// <summary>
        /// Read and write property for whether the box is in the goal position
        /// </summary>
        public bool InGoal
        {
            get => inGoal;
            set => inGoal = value;
        }

        //Constructor
        /// <summary>
        /// Constructs a new box object
        /// </summary>
        /// <param name="rect">The rectangle representation of the object</param>
        /// <param name="tex">The texutre of the box</param>
        public Box(Rectangle rect, Texture2D tex)
        {
            this.rect = rect;
            this.tex = tex;
            isHeld = false;

            //Constructs a rectangle to handle rotation
            rotatedRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws the box based on one of two states: held or not held
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="offset"></param>
        public void Draw(SpriteBatch sb, Vector2 offset)
        {
            if (!isHeld)
                sb.Draw(tex, new Rectangle((int)(rect.X - offset.X), (int)(rect.Y - offset.Y), rect.Width, rect.Height), Color.White);

            //Handles the rotation of the box image, using the rotatedRect object instead of rect
            else
            {
                sb.Draw(tex,
                    new Rectangle((int)(rotatedRect.X - offset.X),
                    (int)(rotatedRect.Y - offset.Y), rotatedRect.Width, rotatedRect.Height),
                    null,
                    Color.White,
                    angle,
                    new Vector2(0, 0),
                    SpriteEffects.None,
                    1);
            }
        }

        /// <summary>
        /// Updates the box, if held, based on the state of the player
        /// </summary>
        /// <param name="player">The plaer object</param>
        public void Update(Player player)
        {
            if (isHeld)
            {
                rect.X = (player.Rect.Center.X - this.rect.Width / 2);
                rect.Y = (player.Rect.Center.Y - this.rect.Height / 2);

                //Rotates the box relative to the center of the player
                Rotate(player);
                angle = player.Rotation;
            }
        }

        /// <summary>
        /// Centers the box around a particular area, generally the goal
        /// </summary>
        /// <param name="dest">The destination to be centered around</param>
        public void Center(Rectangle dest)
        {
            rect.X = dest.Center.X - (rect.Width / 2);
            rect.Y = dest.Center.Y - (rect.Height / 2);
        }

        /// <summary>
        /// Rotates the box based on the player 
        /// </summary>
        /// <param name="player"></param>
        public void Rotate(Player player)
        {
            //Sets the anchor point of the rotation, the center of the player
            int anchorX = player.Rect.Center.X;
            int anchorY = player.Rect.Center.Y;

            int x = rect.X;
            int y = rect.Y - this.rect.Height;
            float angle = player.Rotation;

            rotatedRect.X = (int)(((x - anchorX) * Math.Cos(angle))
                    + ((anchorY - y) * Math.Sin(angle)) + anchorX);
            rotatedRect.Y = (int)(anchorY - ((anchorY - y) * Math.Cos(angle))
                    + ((x - anchorX) * Math.Sin(angle)));
        }
    }
}