using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TimeToKill;

using Entities.Projectiles;
using Creatures;

namespace Entities
{
	public class Entity
	{
		public Texture2D texture;
		public Vector2 position;
		public Vector2 origin;
		public float rotation;

		public TimeSpan lastAction;

		public Entity(Texture2D texture, Vector2 position)
		{
			this.texture = texture;
			this.position = position;

			this.origin = new Vector2(texture.Width / 2, texture.Height / 2);

			rotation = 0f;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1.2f, SpriteEffects.None, 0f);
		}

		public virtual void Update(GameTime gameTime, Main game)
		{
			
		}
	}
}
