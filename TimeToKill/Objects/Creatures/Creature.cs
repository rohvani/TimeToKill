using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TimeToKill;

namespace Entities
{
	public class Creature : Entity
	{
		public int health, maxHealth;

		public Creature(Texture2D texture, Vector2 position) : base(texture, position)
		{
			this.texture = texture;
			this.position = position;
			this.rotation = 0;

			this.health = 100;
			this.maxHealth = health;
		}
	}
}
