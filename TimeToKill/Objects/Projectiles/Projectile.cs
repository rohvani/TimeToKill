using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entities.Projectiles
{
	public class Projectile : Entity
	{
		private Vector2 target;

		public bool hasHit;
		public bool playerOwned;

		public Projectile(Vector2 position, Vector2 direction, Texture2D texture, bool playerOwned) : base( texture, position)
		{
			this.position = position;
			this.target = (direction - position);
			this.texture = texture;

			target.Normalize();
			target *= 8;

			origin = new Vector2(texture.Width / 2, texture.Height / 2);

			hasHit = false;
			this.playerOwned = playerOwned;
		}

		public void Update()
		{
			position += target;
		}
	}
}
