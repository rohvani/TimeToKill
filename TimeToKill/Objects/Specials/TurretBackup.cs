using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TimeToKill;
using Creatures;
using Entities;

namespace Specials
{
	class TurretBackup : Special
	{

		public TurretBackup(Texture2D texture) : base(texture)
		{
			this.texture = texture;

			this.unlocked = false;
			this.dropRate = 15;

			this.cooldown = TimeSpan.Zero;
			this.lastUse = TimeSpan.Zero;
		}

		public override void performAction(GameTime gameTime, Main game)
		{
			Player player = game.player;

			game.entityList.Add(new PlayerTurret(game.textureCache[7], player.Position));

			lastUse = gameTime.TotalGameTime;
			unlocked = false;
		}
	}
}
