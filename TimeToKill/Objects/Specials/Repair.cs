using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TimeToKill;
using Creatures;

namespace Specials
{
	class Repair : Special
	{

		public Repair(Texture2D texture) : base(texture)
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

			Main.fastMessage playerHitFastMessage = new Main.fastMessage();

			playerHitFastMessage.Color = Color.GreenYellow;
			playerHitFastMessage.Position = player.Position;
			playerHitFastMessage.Duration = TimeSpan.FromSeconds(0.33);
			playerHitFastMessage.GameTime = gameTime.TotalGameTime;

			int healAmount = 15;

			if (player.Health == 100)
			{
				playerHitFastMessage.Message = "Full health!";
				game.fastMessages.Add(playerHitFastMessage);
				return;
			}
			else if(player.Health + healAmount > 100)
			{
				int healthIncrease = (100 - player.Health);
				player.Health = 100;
				playerHitFastMessage.Message = "+" + healthIncrease.ToString();
			}
			else
			{
				player.Health += healAmount;
				playerHitFastMessage.Message = "+" + healAmount.ToString();
			}
	
			game.fastMessages.Add(playerHitFastMessage);
			lastUse = gameTime.TotalGameTime;
			unlocked = false;
		}
	}
}
