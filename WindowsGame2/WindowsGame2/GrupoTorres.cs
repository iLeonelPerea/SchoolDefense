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
    class GrupoTorres
    {
        public static List<Torre> objList = new List<Torre>();

        public static void Initialize()
        {
        }

        public void agregarTorre(Vector2 posicionInicial,ContentManager content)
        {
            Torre o = new Torre(posicionInicial, "torre1");
            o.LoadContent(content);
            objList.Add(o);
        }

    }
}
