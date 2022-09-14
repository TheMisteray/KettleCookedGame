using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

/// <summary>
/// Place to keep all enemies together
/// </summary>
namespace WorkingGame
{
    //EnemyManager class
    //Manages the behavior of all enemies within a particular level
    internal class EnemyManager
    {
        //Fields
        private List<Enemy> enemies;

        //Properties
        /// <summary>
        /// Read-only property for the list of enemies in a level
        /// </summary>
        public List<Enemy> Enemies
        {
            get => enemies;
        }

        /// <summary>
        /// Read-only property for the number of enemies in a level
        /// </summary>
        public int Count
        {
            get => enemies.Count;
        }

        //Constructor
        /// <summary>
        /// Constructs a new instance of the EnemyManager class, being the
        /// initialization of the list of enemies
        /// </summary>
        public EnemyManager()
        {
            enemies = new List<Enemy>();
        }

        //Methods
        /// <summary>
        /// Adds an enemy to the manager
        /// </summary>
        /// <param name="enemy">The enemy to be added</param>
        public void AddEnemy(Enemy enemy)
        {
            enemies.Add(enemy);
        }

        /// <summary>
        /// Clears all enemies
        /// </summary>
        public void Clear()
        {
            enemies.Clear();
        }

        public void Draw(SpriteBatch sb, Vector2 offset, bool debugMode, GraphicsDevice graphics)
        {
            foreach (Enemy e in enemies)
                e.Draw(sb, offset, debugMode, graphics);
        }

        /// <summary>
        /// Runs the update method on all enemies in the level
        /// </summary>
        /// <param name="game"></param>
        public void UpdateAll(GameTime game)
        {
            foreach (Enemy e in enemies)
                e.Update(game);
        }

        /// <summary>
        /// Passes reference for player, trimmed from prior release
        /// </summary>
        /// <param name="player"></param>
        public void GetTarget(Player player)
        {
            foreach (Enemy e in enemies)
            {
                //Updates the delegate for path completion to target the player upon completion
                e.PathFollower.completed += () => { e.PathFollower.setTarget(player); };
            }
        }
    }
}