using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame2
{
    //{    0,        1,      2,      3  }   
    public enum Direccion { IZQUIERDA, DERECHA, ARRIBA, ABAJO };
    // Esta estructura contiene cada una de las rutas de un Mundo
    public struct Ruta
    {
        public Direccion direccion;
        public int X;
        public int Y;
    } 

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static SpriteFont font;        
        Texture2D lunch;        
        public static Ruta[] ruta;        
        private static int aliensActivos=-1;
        int iteraciones = 0;        
        public static int NO_DESAYUNOS = 10;
        World world1;
        GrupoTorres grupoTorres = new GrupoTorres();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";            
        }

        protected override void Initialize()
        {            
            this.Window.Title = "Tower Defense";
            // Mostramos el raton
            this.IsMouseVisible = true;
            ruta = Utileria.getRuta("nivel1.txt");                       
            // Creamos un mundo
            world1 = new World(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            // Creamos el arreglo de los aliens. 
            GrupoAliens.Initialize();
            GrupoTorres.Initialize();
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>("SpriteFont1");
            lunch = Content.Load<Texture2D>("sprites\\lunch");
            world1.LoadContent(Content);
            // Cargamos el contenido de la lista que contiene todos los aliens
            foreach (Alien o in GrupoAliens.objList)
            {
                o.LoadContent(this.Content);
            }
        }

        protected override void UnloadContent()
        {
            
        }
        // Este metodo se ejecuta 60 veces por segundo
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit. If Escape key was pressed
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            // Por cada arreglo que existe en la lista, siempre estaremos llamandos su metodo Update.
            foreach (Alien o in GrupoAliens.objList)
            {
                o.Update();
            }
            iteraciones++;
            /*
             * Cada 5 Segundos (300/60) le daremos vida a un alien. No debemos sobrepasar el tamaño de la lista.
             */ 
            if (iteraciones == 300 && aliensActivos<9)
            {
                aliensActivos++; // Indicamos que se active el siguiente alien.
                iteraciones = 0; // Volvemos a reiniciar el contador de segundos, para volver a darle vida a otro despues de otros 5 seg.                
                GrupoAliens.objList[aliensActivos].alive = true; // Le damos vida al alien que sigue en la lista
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                grupoTorres.agregarTorre(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), this.Content);
            base.Update(gameTime);
        }        
        // El metodo Draw se ejecuta inmediatamente despues del Update, es decir tambien 60 veces por segundo
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            
            // Vector donde colocaremos el no de vidas que le quedan al jugador            
            spriteBatch.Begin();     
       
            world1.Draw(spriteBatch);            
            spriteBatch.Draw(lunch, new Rectangle(725, 420, 48, 48), Color.White);
            spriteBatch.DrawString(font, "Breakfast:"+NO_DESAYUNOS, new Vector2(300, 5), Color.Red);
            spriteBatch.DrawString(font, "Time :" + gameTime.TotalGameTime.Minutes + ":" + gameTime.TotalGameTime.Seconds, new Vector2(5, 5), Color.Red);
            spriteBatch.DrawString(font, "Iteraciones :" +iteraciones, new Vector2(5, 25), Color.Red);
            spriteBatch.DrawString(font, "Active Aliens :" + (aliensActivos+1), new Vector2(5, 45), Color.Red);

            spriteBatch.DrawString(font, "posicion del mouse X:" + (Mouse.GetState().X) + " Y: " + (Mouse.GetState().Y), new Vector2(5, 65), Color.Red);
            // Por cada alien contenido en la lista, llamaremos su metodo Draw()
            foreach (Alien o in GrupoAliens.objList)
            {
                o.Draw(this.spriteBatch);
            }
            foreach (Torre o in GrupoTorres.objList)
            {
                o.Draw(this.spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }   
}