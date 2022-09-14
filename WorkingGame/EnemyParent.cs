using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System;

//Aiden Grieshaber
//2/18/22
//Enemy parent
namespace WorkingGame
{
    //Delegate to handle enemy interaction with light collision
    internal delegate void OnLightInteraction(Enemy enemy);

    //EnemyParent class
    //Handles the behavior inherited by the enemy
    internal abstract class EnemyParent : IGameObject
    {
        //Fields
        protected Rectangle rect;

        protected Texture2D tex;
        protected PathFollower pathFollower;

        //Sound effect for completing path
        protected SoundEffect pathComplete;

        private Texture2D hitbox;

        protected int frame;
        protected const int numFrames = 7;
        protected int frameCounter;
        protected float scale;

        private Color color;

        /// <summary>
        /// Read-only property for the rectangle representing the enemy
        /// </summary>
        private bool playerCollision;

        private bool lightCollision;

        public bool PlayerCollision
        {
            get => playerCollision;
            set => playerCollision = value;
        }

        public bool LightCollision
        {
            get => lightCollision;
            set => lightCollision = value;
        }

        public Rectangle Rect
        {
            get => rect;
        }

        /// <summary>
        /// Read-only property for the enemy
        /// </summary>
        public Texture2D Tex
        {
            get => tex;
        }

        /// <summary>
        /// Read-only property for the checkpoints along the enemy path
        /// </summary>
        public List<Vector2> Checkpoints
        {
            get => pathFollower.Checkpoints;
        }

        /// <summary>
        /// Read and write property for the position of the enemy
        /// </summary>
        public Vector2 Position
        {
            get => new Vector2(rect.X, rect.Y);
            set { rect.X = (int)value.X; rect.Y = (int)value.Y; }
        }

        /// <summary>
        /// Read-only property for the enemy's PathFollower object
        /// </summary>
        public PathFollower PathFollower { get => pathFollower; }

        /// <summary>
        /// Read-only property for the enemy's width
        /// </summary>
        public int Width
        {
            get => (int)((tex.Width / numFrames) * scale);
        }

        /// <summary>
        /// Read-only property for the enemy's height
        /// </summary>
        public int Height
        {
            get => (int)(tex.Height * scale);
        }

        /// <summary>
        /// Read-only property for the color of the enemy
        /// </summary>
        public Color Color { get => color; }

        /// <summary>
        /// Constructs a new instance of EnemyParent
        /// </summary>
        /// <param name="tex">The texure of the image</param>
        /// <param name="pos">The position of the enemy</param>
        /// <param name="hitbox">The enemy's hitbox</param>
        public EnemyParent(Texture2D tex, Vector2 pos, Texture2D hitbox, SoundEffect pathComplete)
        {
            scale = .5f;
            this.tex = tex;
            rect = new Rectangle((int)pos.X, (int)pos.Y, (int)((tex.Width / numFrames) * scale), (int)(tex.Height * scale));
            pathFollower = new PathFollower(pos);
            frameCounter = 0;
            this.hitbox = hitbox;
            color = Color.White;
            playerCollision = false;
            lightCollision = false;
            this.pathComplete = pathComplete;
        }

        /// <summary>
        /// Updates enemy position and other frame-by-frame changes
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Designed to set the enemy to a random color. Currently sets them
        /// to blue
        /// </summary>
        public void RandomColor()
        {
            color = Palette.GetRandomColor();
        }

        /// <summary>
        /// Draw enemy
        /// </summary>
        /// <param name="sb">game spritebatch</param>
        public void Draw(SpriteBatch sb, Vector2 offset, bool debugMode, GraphicsDevice graphics)
        {
            // Normal enemy
            if (!debugMode)
            {
                sb.Draw(tex,
                new Rectangle((int)(rect.X - offset.X), (int)(rect.Y - offset.Y), rect.Width, rect.Height),
                new Rectangle((tex.Width / numFrames) * frame, 0, tex.Width / numFrames, tex.Height),
                color);
            }

            // Debug Enemy
            else
            {
                Texture2D debugRect = new Texture2D(graphics, rect.Width, rect.Height);

                Color[] color = new Color[rect.Width * rect.Height];
                for (int i = 0; i < color.Length; ++i)
                {
                    color[i] = Color.White;
                }
                debugRect.SetData(color);

                // Light collision
                if (lightCollision)
                {
                    sb.Draw(
                        debugRect,
                        new Rectangle((int)(rect.X - offset.X), (int)(rect.Y - offset.Y), rect.Width, rect.Height),
                        new Rectangle((tex.Width / numFrames) * frame, 0, tex.Width / numFrames, tex.Height),
                        Color.PaleGoldenrod);
                }

                // Player collision
                else if (playerCollision)
                {
                    sb.Draw(
                        debugRect,
                        new Rectangle((int)(rect.X - offset.X), (int)(rect.Y - offset.Y), rect.Width, rect.Height),
                        new Rectangle((tex.Width / numFrames) * frame, 0, tex.Width / numFrames, tex.Height),
                        Color.Cyan);
                }

                // No collision
                else
                {
                    //light color debug
                    sb.Draw(
                    debugRect,
                    new Rectangle(
                        (int)(rect.X - offset.X - rect.Width / 8), (int)(rect.Y - offset.Y - rect.Height / 8), rect.Width + rect.Width / 4, rect.Height + rect.Height / 4),
                    new Rectangle(tex.Width
                                  / numFrames
                                  * frame, 0, tex.Width / numFrames, tex.Height),
                    this.color * .90f);
                    sb.Draw(
                        debugRect,
                        new Rectangle((int)(rect.X - offset.X), (int)(rect.Y - offset.Y), rect.Width, rect.Height),
                        new Rectangle((tex.Width / numFrames) * frame, 0, tex.Width / numFrames, tex.Height),
                        Color.HotPink);
                }
            }
        }
    }
}