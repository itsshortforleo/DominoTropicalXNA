using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Domino_Beta_v0._1.Entidades
{
    class BotonDeMenu
    {

        #region Campos

        Texture2D _imagen;          // Tamano del boton
        Vector2 _posicion;          // Posicion del boton
        Rectangle _rectangulo;      // Rectangulo para utilizar interseccion

        Color color = new Color(255, 255, 255, 255);    // Color que tomara la imagen

        public Vector2 tamano;      // Tamano del boton
            
        #endregion

        #region Propiedades
        
        #endregion

        #region Constructor

        public BotonDeMenu(Texture2D nuevaImagen, GraphicsDevice graphics)
        {
            _imagen = nuevaImagen;

            //ScreenWidth = 1366, ScreenHeight = 768
            //ImagenWidth =  300, ImagenWidth  = 50

            tamano = new Vector2(graphics.Viewport.Width / 4.55f, graphics.Viewport.Height / 15.36f);
        }


        public BotonDeMenu(Texture2D nuevaImagen, GraphicsDevice graphics, Vector2 posicion)
        {
            _imagen = nuevaImagen;

            //ScreenWidth = 1366, ScreenHeight = 768
            //ImagenWidth =  300, ImagenWidth  = 50

            tamano = new Vector2(graphics.Viewport.Width, graphics.Viewport.Height);
        }

        public BotonDeMenu(Texture2D nuevaImagen, GraphicsDevice graphics, int h)
        {
            _imagen = nuevaImagen;

            //ScreenWidth = 1366, ScreenHeight = 768
            //ImagenWidth =  150, ImagenWidth  = 60

            tamano = new Vector2(graphics.Viewport.Width / 18.21f, graphics.Viewport.Height / 25.6f);

        }
        
        
        #endregion

        #region Metodos/Funciones

        bool pulsado;
        public bool seHizoClic;
        public void Update(MouseState mouse)
        {


            _rectangulo = new Rectangle((int)_posicion.X, (int)_posicion.Y,
                (int)tamano.X, (int)tamano.Y);
            Rectangle rectanguloDeMouse = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (rectanguloDeMouse.Intersects(_rectangulo))
            {
                if (color.A == 255) pulsado = false;
                if (color.A == 0) pulsado = true;
                if (pulsado) color.A += 3; else color.A -= 3;
                if (mouse.LeftButton == ButtonState.Pressed) seHizoClic = true;
            }
            else if (color.A < 255)
            {
                color.A += 3;
                seHizoClic = false;
            }
        }

        // Establece la posicion del elemento que se va a dibujar
        public void EstablecerPosicion(Vector2 nuevaPosicion)
        {
            _posicion = nuevaPosicion;
        }

        // Sobrecarga que, aparte de que stablece la posicion del elemento que se va a dibujar, tambien ajusta la escala
        public void EstablecerPosicion(Vector2 nuevaPosicion, Vector2 tamano)
        {
            _posicion = nuevaPosicion;
            this.tamano = tamano;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_imagen, _rectangulo, color);
        }

        public void Draw(SpriteBatch spriteBatch, Color colorDeseado)
        {
            spriteBatch.Draw(_imagen, _rectangulo, colorDeseado);
        }

        public void Draw(SpriteBatch spriteBatch, float Profundidad)
        {
            spriteBatch.Draw(_imagen, _rectangulo,null,Color.White,0f,Vector2.Zero,SpriteEffects.None, Profundidad);
        }

        #endregion

       

    }   
}
