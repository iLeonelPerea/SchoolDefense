using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; 

namespace WindowsGame2
{
    class World
    {
        // Variable que contendra la imagen del alien 
        Texture2D worldTexture;
        // Variable que servira para indicar la posicion donde se dibujara el alien
        Rectangle rectangle;        
        private ContentManager _content;

        public World(int width, int height) {
            rectangle = new Rectangle(0, // X position of top left corner
                                      0, // Y position of top left corner
                                      width, // rectangle width
                                      height); // rectangle height);
        }

        public void LoadContent(ContentManager Content)
        {
            this._content = Content;
            worldTexture = Content.Load<Texture2D>("sprites\\world1");
        }
     
        public void Draw(SpriteBatch sprite) {
            sprite.Draw(worldTexture, rectangle, Color.White);
        }

    }
}
