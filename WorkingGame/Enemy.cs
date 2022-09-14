using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace WorkingGame
{
    //Enemy class
    //Handles enemy behavior not initialized within EnemyParent
    internal class Enemy : EnemyParent
    {
        /// <summary>
        /// Creates a new instance of an enemy
        /// </summary>
        /// <param name="tex">The image representing the enemy</param>
        /// <param name="pos">The position of the enemy</param>
        /// <param name="path">The path the enemy travels</param>
        /// <param name="hitbox">The enemy's hitbox</param>
        public Enemy(Texture2D tex, Vector2 pos, List<Vector2> path, Texture2D hitbox, SoundEffect sound) : base(tex, pos, hitbox, sound)
        {
            this.pathFollower.DefinePath(path);
        }

        /// <summary>
        /// Updates the state of the enemy
        /// </summary>
        /// <param name="gameTime">The GameTime object</param>
        public override void Update(GameTime gameTime)
        {
            pathFollower.Update(gameTime, this);
            if (pathFollower.State != PathFollower_MovementStates.Destination)
            {
                this.Position = pathFollower.Position - new Vector2(Width / 2, Height / 2);
            }
            else
            {
                pathComplete.CreateInstance().Play();
                pathFollower.FollowPlayer();
                //this.Position = pathFollower.Position - new Vector2(Width / 2, Height / 2);
            }
            //animation hanlding
            frameCounter++;
            if (frameCounter % 10 == 0)
                frame++;
            if (frame > numFrames - 1)
                frame = 0;
        }
    }
}