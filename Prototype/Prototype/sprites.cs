using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Cards
{

    public Texture2D imgTexture;        // Animation representing the hitbox
    public Vector2 Position;            // Position of the hitbox relative to the upper left side of the screen
    public bool Show;                   // State of the hitbox

    // Get the width of the hitbox
    public int Width
    {
        get 
        { 
            return imgTexture.Width; 
        }
    }

    // Get the height of the hitbox
    public int Height
    {
        get 
        { 
            return imgTexture.Height; 
        }
    }


    public void Initialize(Texture2D texture, Vector2 position)
    {
        imgTexture = texture;

        Position = position;

        Show = true;
    }

    public void Update(int imgScreenPos)
    {
        int imageXPos = 0;
        int imageYPos = 0;


        if (imgScreenPos % 2 == 0)
            {
                imageXPos = 0;
                imageYPos = ((imgScreenPos / 2) * 250);
            }
            else
            {
                imageXPos = 300;
                imageYPos = ((imgScreenPos / 2) * 250);
            }
            Vector2 tempPos = new Vector2(imageXPos, imageYPos);
            Position = tempPos;
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Show)
        {
            spriteBatch.Draw(imgTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
