using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Domino.Entities
{
    class MenuButton
    {

        #region Fields

        Texture2D _image;          // Button size
        Vector2 _position;         // Button position
        Rectangle _rectangle;      // Rectangle for intersection

        Color color = new Color(255, 255, 255, 255);    // Color que tomara la imagen

        public Vector2 tamano;      // Tamano del boton
            
        #endregion

        #region Properties
        
        #endregion

        #region Constructor

        public MenuButton(Texture2D nuevaImagen, GraphicsDevice graphics)
        {
            _image = nuevaImagen;

            //ScreenWidth = 1366, ScreenHeight = 768
            //ImagenWidth =  300, ImagenWidth  = 50

            tamano = new Vector2(graphics.Viewport.Width / 4.55f, graphics.Viewport.Height / 15.36f);
        }


        public MenuButton(Texture2D nuevaImagen, GraphicsDevice graphics, Vector2 posicion)
        {
            _image = nuevaImagen;

            //ScreenWidth = 1366, ScreenHeight = 768
            //ImagenWidth =  300, ImagenWidth  = 50

            tamano = new Vector2(graphics.Viewport.Width, graphics.Viewport.Height);
        }

        public MenuButton(Texture2D nuevaImagen, GraphicsDevice graphics, int h)
        {
            _image = nuevaImagen;

            //ScreenWidth = 1366, ScreenHeight = 768
            //ImagenWidth =  150, ImagenWidth  = 60

            tamano = new Vector2(graphics.Viewport.Width / 18.21f, graphics.Viewport.Height / 25.6f);

        }
        
        
        #endregion

        #region Methods/Functions

        bool pulsado;
        public bool seHizoClic;
        public void Update(MouseState mouse)
        {


            _rectangle = new Rectangle((int)_position.X, (int)_position.Y,
                (int)tamano.X, (int)tamano.Y);
            Rectangle rectanguloDeMouse = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (rectanguloDeMouse.Intersects(_rectangle))
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
            _position = nuevaPosicion;
        }

        // Sobrecarga que, aparte de que stablece la posicion del elemento que se va a dibujar, tambien ajusta la escala
        public void EstablecerPosicion(Vector2 nuevaPosicion, Vector2 tamano)
        {
            _position = nuevaPosicion;
            this.tamano = tamano;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_image, _rectangle, color);
        }

        public void Draw(SpriteBatch spriteBatch, Color colorDeseado)
        {
            spriteBatch.Draw(_image, _rectangle, colorDeseado);
        }

        public void Draw(SpriteBatch spriteBatch, float Profundidad)
        {
            spriteBatch.Draw(_image, _rectangle,null,Color.White,0f,Vector2.Zero,SpriteEffects.None, Profundidad);
        }

        #endregion

       

    }   
}
