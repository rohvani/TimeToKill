using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TimeToKill;
using Creatures;

namespace Specials
{
	public class Special
	{
		public bool unlocked;
		public int dropRate;

		public Texture2D texture;

		public TimeSpan cooldown;
		public TimeSpan lastUse;

		public Special(Texture2D texture)
		{

		}

		public virtual void performAction(GameTime gameTime, Main game)
		{

		}
	}
}
