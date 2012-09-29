using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Cards
{
    // Animation representing the player
    public Texture2D imgTexture;

    // Position of the Player relative to the upper left side of the screen
    public Vector2 Position;

    // State of the player
    public bool Active;

    // Get the width of the player ship
    public int Width
    {
        get { return imgTexture.Width; }
    }

    // Get the height of the player ship
    public int Height
    {
        get { return imgTexture.Height; }
    }


    public void Initialize(Texture2D texture, Vector2 position)
    {
        imgTexture = texture;

        Position = position;

        Active = true;
    }

    public void Update(int imgScreenPos)
    {
        int imageXPos = 0;
        int imageYPos = 0;


        if (imgScreenPos % 2 != 0)
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
        spriteBatch.Draw(imgTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }
}
