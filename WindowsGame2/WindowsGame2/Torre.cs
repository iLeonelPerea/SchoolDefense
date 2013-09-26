using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame2
{
    class Torre
    {
        // Variable que contendra la imagen del alien 
        Texture2D torreTexture;
        // Variable que servira para indicar la posicion donde se dibujara el alien
        Rectangle rectangle;
        // Esta variable va a guardar la posicion actual del alien
        Vector2 posicion;
        // Variable que sirve para indicar el numero de pixeles que ira avanzando el alien (Velocidad)
        int velocidad = 100;
        //Nombre de la imagen para este alien
        String spriteTorre;
        // Variable que guarda la resistencia que tiene el alien. Esta sera la variable que descontaremos si una torre lo ataca
        int fuerza = 30;
        int alcanze = 150;
        private ContentManager content;

        public Torre(Vector2 posicionInicial, String spriteTorre)
        {
            // Cuando se crea el objeto, establecemos la posicion inicial del alien.
            this.posicion = new Vector2(posicionInicial.X - 24, posicionInicial.Y - 24);
            // Creamos el rectangulo que contendra el alien, en la posicion inicial
            this.rectangle = new Rectangle((int)posicionInicial.X, (int)posicionInicial.Y, 48, 48);
            this.spriteTorre = spriteTorre;
        }

        public void LoadContent(ContentManager content)
        {
            this.content = content;
            this.torreTexture = content.Load<Texture2D>("sprites\\" + this.spriteTorre);
        }

        public void Update()
        {
            if (velocidad == 100)
                velocidad = 0;
            else
                velocidad++;
        }

        public int Atacar(Vector2 posicionAlien)
        {
            if (velocidad == 0)
            {
                Rectangle rangoAtaque = new Rectangle((int)posicion.X - alcanze, (int)posicion.Y - alcanze, 48 + alcanze, 48 + alcanze);
                Rectangle alien = new Rectangle((int)posicionAlien.X, (int)posicionAlien.Y, 48, 48);
                if (rangoAtaque.Intersects(alien))
                {
                    return 30;
                }
            }
            return 0;
        }

        public void Draw(SpriteBatch sprite)
        {
            sprite.Draw(torreTexture, new Rectangle((int)posicion.X, (int)posicion.Y, 48, 48), Color.White);
        }

        public void mover()
        {
            
        }

    }
}
