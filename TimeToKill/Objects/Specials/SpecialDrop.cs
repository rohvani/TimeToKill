using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Specials
{
	class SpecialDrop
	{
		private Texture2D texture;
		public Vector2 position;
		public TimeSpan timeDropped;
		public int playerSpecialIndex;

		public SpecialDrop(int playerSpecialIndex,Texture2D texture, GameTime gameTime, Vector2 position)
		{
			this.playerSpecialIndex = playerSpecialIndex;
			this.position = position;
			this.texture = texture;
			this.timeDropped = gameTime.TotalGameTime;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
		}
	}
}
