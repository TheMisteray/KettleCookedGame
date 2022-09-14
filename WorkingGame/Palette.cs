using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace WorkingGame
{
    //passes the color around to other objects in case method calls /references get messy
    internal delegate void OnGlowColorChange(Color color);

    /// <summary>
    ///
    /// </summary>
    internal static class Palette
    {
        /// <summary>
        /// All game objects can cycle through these color options
        /// </summary>
        private static Color[] colors = new Color[] { Color.White, Color.Orange, Color.Aqua, Color.Red, Color.Chartreuse };

        //fields:

        private static Random rng = new Random();

        //event for when color changes for the player

        public static event OnGlowColorChange onGlow;

        private static Color curEyeGlow = Color.White;

        //returns the length if player index goes pass the amount of color options
        public static int Amount
        {
            get => colors.Length;
        }

        public static Color CurEyeGlow { get => curEyeGlow; }

        /// <summary>
        ///  returns random color from colors.
        /// </summary>
        /// <returns> Color </returns>
        public static Color GetRandomColor()
        {
            return colors[rng.Next(0, colors.Length)];
        }

        /// <summary>
        /// Changes the color incrementally
        /// </summary>
        /// <param name="i"></param>
        /// <returns> Color </returns>
        public static Color ChangeColorIncre(int i)
        {
            onGlow?.Invoke(colors[i]);
            curEyeGlow = colors[i];
            return colors[i];
        }
    }
}