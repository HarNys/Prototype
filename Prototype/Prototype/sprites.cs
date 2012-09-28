using System;

public class Sprits
{
    private Texture2D SpriteTexture;
    private Rectangle TitleSafe;

    public Sprits()
	{
        spriteBatch = new SpriteBatch(GraphicsDevice);
        SpriteTextre = Content.Load<Texture2D>("ship");
        TitleSafe = uGetTitleSafeArea(.8f);
	}

    protected override void Draw(GameTime gameTime)
    {
        graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        
        spriteBatch.Begin();
        Vector2 pos = new Vector2(TitleSafe.Left, TitleSafe.Top);
        spriteBatch.Draw(SpriteTexture, pos, Color.White);
        spriteBatch.End();

        base.Draw(gameTime);
    }
}
