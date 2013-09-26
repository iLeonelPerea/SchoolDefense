using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame2
{
    class GrupoAliens
    {
        public static List<Alien> objList = new List<Alien>();

        public static void Initialize()
        {

            for (int i = 0; i < 10; i++)
            {
                Alien o = new Alien(new Vector2(-48, 126), Game1.ruta, "alien1");   
                o.alive = false;
                objList.Add(o);
            }
            
        }

    }
}
