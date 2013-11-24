using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TimeToKill;
using Creatures;
using Entities.Projectiles;

namespace Entities
{
	public class PlayerTurret : Entity
	{
		int totalShots;

		public PlayerTurret(Texture2D texture, Vector2 position) : base(texture, position)
		{
			this.texture = texture;
			this.position = position;
			this.rotation = 0;

			totalShots = 0;
		}

		public override void Update(GameTime gameTime, Main game)
		{
			Player player = game.player;
			position = player.Position;

			Creature closestEnemy = game.enemyList[0];
			double lastDistance = 100000f;

			foreach (Creature enemy in game.enemyList)
			{
				double distance = Math.Pow((player.Position.X - enemy.position.X), 2) + Math.Pow((player.Position.Y - enemy.position.Y), 2);
				if (distance < lastDistance)// && Vector2.Dot((enemy.Position - player.Position), (new Vector2(1, (float)Math.Sin(rotation)) + player.Position)) > 0)
				{
					closestEnemy = enemy;
					lastDistance = distance;
				}
			}

			Vector2 projectileTarget = closestEnemy.position - player.Position;

			if ((gameTime.TotalGameTime - lastAction) > TimeSpan.FromSeconds(0.25f))
			{
				game.playerProjectileList.Add(new Projectile(this.position, closestEnemy.position, game.textureCache[2], true));
				lastAction = gameTime.TotalGameTime;
				totalShots++;
			}

			if (totalShots == 25) game.entityList.Remove(this);

			rotation = (float) Math.Atan2(projectileTarget.Y, projectileTarget.X); 
		}
	}
}
