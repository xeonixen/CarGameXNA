using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CarGameXNA
{
    class Background
    {
        public Texture2D texture;
        public Vector2 vector;
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, vector, Color.White);
        }
    }
    class Scrolling : Background
    {
        public Scrolling(Texture2D newTexture,Vector2 newVector)
        {
           texture = newTexture;
            vector = newVector;
        }
        public void Update(float x)
        {
            vector.X += x;
        }
    }
}
