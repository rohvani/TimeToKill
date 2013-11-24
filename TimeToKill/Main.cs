#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

using Entities;
using Entities.Projectiles;
using Creatures;

using Specials;

#endregion

namespace TimeToKill
{
	public class Main : Game
	{
		// Monogame Stuff
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public Player player;

		// Lists
		public List<Texture2D> textureCache = new List<Texture2D>();
		List<SpriteFont> fontCache = new List<SpriteFont>();

		List<Projectile> enemyProjectileList = new List<Projectile>();
		public List<Projectile> playerProjectileList = new List<Projectile>();

		public List<Creature> enemyList = new List<Creature>();
		public List<Entity> entityList = new List<Entity>();

		List<SpecialDrop> specialDropsList = new List<SpecialDrop>();

		public List<fastMessage> fastMessages = new List<fastMessage>();

		// Controller States
		MouseState oldMouseState;
		KeyboardState oldKeyboardState;
		GamePadState oldGamePadState;

		// Level Variables
		int wave;
		TimeSpan lastWave;
		TimeSpan lastClean;
		

		// Misc
		Random random;
		bool debugMode;

		// Structs
		public struct fastMessage
		{
			public string Message;
			public Color Color;
			public Vector2 Position;
			public TimeSpan Duration;
			public TimeSpan GameTime;

			public fastMessage(String message, Color color, Vector2 position, TimeSpan duration, TimeSpan gameTime)
			{
				this.Message = message;
				this.Color = color;
				this.Position = position;
				this.Duration = duration;
				this.GameTime = gameTime;
			}
		}
		
		// Game Time!  Whoo!
		public Main() : base()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = true;

			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			oldKeyboardState = Keyboard.GetState();
			player = new Player();

			lastWave = TimeSpan.Zero;
			lastClean = TimeSpan.Zero;
			wave = 1;

			debugMode = false;

			random = new Random();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Content loading
			textureCache.Add(Content.Load<Texture2D>("Graphics\\Ships\\Player.png"));
			textureCache.Add(Content.Load<Texture2D>("Graphics\\Ships\\GenericEnemy.png"));
			textureCache.Add(Content.Load<Texture2D>("Graphics\\Projectiles\\MissilePlayer.png"));
			textureCache.Add(Content.Load<Texture2D>("Graphics\\Projectiles\\MissileEnemy.png"));
			textureCache.Add(Content.Load<Texture2D>("Graphics\\Backgrounds\\background5.jpg"));

			textureCache.Add(Content.Load<Texture2D>("Graphics\\Specials\\Repair.png"));
			textureCache.Add(Content.Load<Texture2D>("Graphics\\Specials\\TurretBackup.png"));

			textureCache.Add(Content.Load<Texture2D>("Graphics\\Ships\\Components\\PlayerTurret.png"));

			textureCache.Add(Content.Load<Texture2D>("Graphics\\Specials\\GamePadSelection.png"));

			fontCache.Add(Content.Load<SpriteFont>("SpriteFont1"));

			// Startup Game Logic		
			player.Initialize(textureCache[0], new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));

			player.Specials.Add(new Repair(textureCache[5]));
			player.Specials.Add(new TurretBackup(textureCache[6]));

			enemyList.Add(new BasicEnemy(textureCache[1], new Vector2(0, 0)));
		}
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}
		protected override void Update(GameTime gameTime)
		{
			// [Game Logic] Projectile Logic
			foreach (Projectile missile in enemyProjectileList) missile.Update();
			foreach (Projectile missile in playerProjectileList) missile.Update();
			
			// [Controller Logic]
			MouseState newMouseState = Mouse.GetState();
			KeyboardState newKeyboardState = Keyboard.GetState();
			GamePadState newGamePadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

			if (!oldGamePadState.ThumbSticks.Left.Equals(newGamePadState.ThumbSticks.Left)) player.usingController = true;
			if (!oldMouseState.Equals(newMouseState)) player.usingController = false;
			 
			// [Controller Logic] Misc Actions
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit(); // Quit
			if (Keyboard.GetState().IsKeyDown(Keys.Q) && !oldKeyboardState.IsKeyDown(Keys.Q)) debugMode = !debugMode; // Debug mode for testing
			if (player.Health <= 0 && Keyboard.GetState().IsKeyDown(Keys.R)) restart(); // Retart upon death

			// [Controller Logic] Player Actions
			if (player.Health > 0)
			{
				if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released) playerProjectileList.Add(new Projectile(player.Position, new Vector2(newMouseState.X, newMouseState.Y), textureCache[2], true));
				if (newGamePadState.Buttons.A == ButtonState.Pressed && oldGamePadState.Buttons.A == ButtonState.Released) playerProjectileList.Add(new Projectile(player.Position, (player.Position + new Vector2(oldGamePadState.ThumbSticks.Left.X, (oldGamePadState.ThumbSticks.Left.Y * -1))), textureCache[2], true));
				if (newGamePadState.Buttons.X == ButtonState.Pressed && oldGamePadState.Buttons.X == ButtonState.Released && player.Specials[player.controllerSpecialSelection].unlocked) player.Specials[player.controllerSpecialSelection].performAction(gameTime, this);


				if (newGamePadState.Buttons.RightShoulder == ButtonState.Pressed && oldGamePadState.Buttons.RightShoulder == ButtonState.Released) player.controllerSpecialSelection = (int)player.controllerSpecialSelection == (byte)(player.Specials.Count - 1) ?  0 : (player.controllerSpecialSelection + 1);


				if (Keyboard.GetState().IsKeyDown(Keys.D1) && !oldKeyboardState.IsKeyDown(Keys.D1))
				{
					if ((gameTime.TotalGameTime - player.Specials[0].lastUse) > player.Specials[0].cooldown && player.Specials[0].unlocked)
					{
						player.Specials[0].performAction(gameTime, this);
					}
				}

				if (Keyboard.GetState().IsKeyDown(Keys.D2) && !oldKeyboardState.IsKeyDown(Keys.D2) && player.Specials[1].unlocked)
				{
					if ((gameTime.TotalGameTime - player.Specials[1].lastUse) > player.Specials[1].cooldown)
					{
						player.Specials[1].performAction(gameTime, this);
					}
				}

				if(player.usingController) player.Rotation = (float)(Math.Atan2(newGamePadState.ThumbSticks.Left.Y, newGamePadState.ThumbSticks.Left.X)) * -1;
				else player.Rotation = (float)(Math.Atan2(newMouseState.Y - player.Position.Y, newMouseState.X - player.Position.X));

				Console.WriteLine(player.usingController);

				// [Game Logic]
				updatePlayer(gameTime);
				processEnemies(gameTime);
				processDamage(gameTime);
				processDropPickups(gameTime);

				// [Game Logic] Spawn NPC Waves
				if (enemyList.Count == 0)
				{
					for (int i = 0; i < 5 + wave + ((wave / 5) * 2); i++)
					{
						enemyList.Add(new BasicEnemy(textureCache[1], new Vector2(random.Next(0, GraphicsDevice.Viewport.Width), random.Next(0, GraphicsDevice.Viewport.Height))));

					}
					lastWave = gameTime.TotalGameTime;
					wave++;
					Console.WriteLine("[WaveSpawner] Spawning new wave with {0} enemies.", 5 + wave + ((wave / 5) * 2));
				}

				// [Game Logic] Remove unseen objects
				if ((gameTime.TotalGameTime - lastClean) > TimeSpan.FromSeconds(7.5f)) disposeUnusedObjects(gameTime);
			}

			// [Controller Logic]
			oldMouseState = newMouseState;
			oldKeyboardState = newKeyboardState;
			oldGamePadState = newGamePadState;

			base.Update(gameTime);
		}
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin();

			// Draw Background
			spriteBatch.Draw(textureCache[4], Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			// Draw Game Objects
			foreach (Projectile projectile in enemyProjectileList) projectile.Draw(spriteBatch);
			foreach (Projectile projectile in playerProjectileList) projectile.Draw(spriteBatch);
			foreach (Creature enemy in enemyList) enemy.Draw(spriteBatch);
			foreach (SpecialDrop drop in specialDropsList) drop.Draw(spriteBatch);
			foreach (Entity entity in entityList) entity.Draw(spriteBatch);

			// Draw 
			drawFastMessages(spriteBatch, gameTime);
			if(player.Health > 0) player.Draw(spriteBatch); // this is here so that the player will render over fast messages
			drawHud(spriteBatch, gameTime);
			
			// Done
			spriteBatch.End();
			base.Draw(gameTime);
		}

		private void updatePlayer(GameTime gameTime)
		{
			for (int i = entityList.Count - 1; i >= 0; i--) entityList[i].Update(gameTime, this);

			if (player.usingController)
			{
				player.Position += (new Vector2(oldGamePadState.ThumbSticks.Left.X, (oldGamePadState.ThumbSticks.Left.Y * -1))) * player.Speed;
			}
			else
			{
				var mouseState = Mouse.GetState();
				Vector2 target = new Vector2(mouseState.X, mouseState.Y);

				target -= player.Position;

				if ((target.Length()) <= player.Speed) return;

				target.Normalize();
				target *= player.Speed;

				player.Position += target;
			}
		}
		private void processEnemies(GameTime gameTime)
		{
			for (int i = enemyList.Count - 1; i >= 0; i--)
			{
				if (enemyList[i].health <= 0)
				{
					fastMessage enemyDeathFastMessage = new fastMessage();

					enemyDeathFastMessage.Message = "boom!";
					enemyDeathFastMessage.Color = Color.OrangeRed;
					enemyDeathFastMessage.Position = enemyList[i].position - (enemyList[i].origin / 2);
					enemyDeathFastMessage.Duration = TimeSpan.FromSeconds(1);
					enemyDeathFastMessage.GameTime = gameTime.TotalGameTime;

					fastMessages.Add(enemyDeathFastMessage);

					foreach (Special special in player.Specials)
					{
						if (random.Next(0, 100) <= special.dropRate)
						{
							specialDropsList.Add(new SpecialDrop(player.Specials.IndexOf(special), special.texture, gameTime, enemyList[i].position));
							break;
						}
					}

					enemyList.RemoveAt(i);
					player.killCount++;

					return;
				}

				if ((gameTime.TotalGameTime - enemyList[i].lastAction) > TimeSpan.FromSeconds(1.5f))
				{
					Vector2 newProjDirection = new Vector2(player.Position.X + random.Next(-20, 20), player.Position.Y + random.Next(-20, 20));

					enemyProjectileList.Add(new Projectile(enemyList[i].position, newProjDirection, textureCache[3], false));
					enemyList[i].lastAction = gameTime.TotalGameTime;
				}

				Vector2 direction = (player.Position - enemyList[i].position);
				direction.Normalize();
				enemyList[i].position += direction;

				enemyList[i].rotation = (float)(Math.Atan2(direction.Y, direction.X));
			}
		}
		private void processDamage(GameTime gameTime)
		{
			if (!debugMode)
			{
				for (int i = enemyProjectileList.Count - 1; i >= 0; i--)
				{
					if (!enemyProjectileList[i].hasHit)
					{
						Vector2 diff = player.Position - enemyProjectileList[i].position;

						if (diff.Length() < 14)
						{
							enemyProjectileList[i].hasHit = true;
							player.Health -= 10;
								
							fastMessage playerHitFastMessage = new fastMessage();

							playerHitFastMessage.Message = "-10";
							playerHitFastMessage.Color = Color.Red;
							playerHitFastMessage.Position = player.Position;
							playerHitFastMessage.Duration = TimeSpan.FromSeconds(0.33);
							playerHitFastMessage.GameTime = gameTime.TotalGameTime;

							fastMessages.Add(playerHitFastMessage);
						}
					}
					else enemyProjectileList.RemoveAt(i);
				}
			}

			for (int i = playerProjectileList.Count - 1; i >= 0; i--)
			{
				if (!playerProjectileList[i].hasHit)
				{
					foreach (BasicEnemy enemy in enemyList)
					{
						Vector2 diff = enemy.position - playerProjectileList[i].position;

						if (diff.Length() < 14)
						{
							playerProjectileList[i].hasHit = true;
							enemy.health -= 10;
						}
					}
				}
				else playerProjectileList.RemoveAt(i);
			}
		}
		private void processDropPickups(GameTime gameTime)
		{
			for (int i = specialDropsList.Count - 1; i >= 0; i--)
			{
				if ((player.Position - specialDropsList[i].position).Length() <= 20)
				{
					player.Specials[specialDropsList[i].playerSpecialIndex].unlocked = true;
					specialDropsList.RemoveAt(i);
				}
				else
				{
					if((gameTime.TotalGameTime - specialDropsList[i].timeDropped) > TimeSpan.FromSeconds(5)) specialDropsList.RemoveAt(i);
				}
			}
		}
		private void disposeUnusedObjects(GameTime gameTime)
		{
			int objectsDisposedOf = 0;

			for (int i = (enemyProjectileList.Count - 1); i >= 0; i--)
			{
				if (enemyProjectileList[i].position.X > GraphicsDevice.DisplayMode.Width || enemyProjectileList[i].position.X < 0 ||
					enemyProjectileList[i].position.Y > GraphicsDevice.DisplayMode.Height || enemyProjectileList[i].position.Y < 0)
				{
					enemyProjectileList.RemoveAt(i);
					objectsDisposedOf++;
				}
			}

			for (int i = (playerProjectileList.Count - 1); i >= 0; i--)
			{
				if (playerProjectileList[i].position.X > GraphicsDevice.DisplayMode.Width || playerProjectileList[i].position.X < 0 || 
					playerProjectileList[i].position.Y > GraphicsDevice.DisplayMode.Height || playerProjectileList[i].position.Y < 0)
				{
					playerProjectileList.RemoveAt(i);
					objectsDisposedOf++;
				}
			}

			lastClean = gameTime.TotalGameTime;
			Console.WriteLine("[ObjectDisposer] {0} objects removed from scene.", objectsDisposedOf);
			//Console.WriteLine("[Debug] {0} active projectiles - {1} player, {2} enemy", (playerProjectileList.Count + enemyProjectileList.Count), playerProjectileList.Count, enemyProjectileList.Count);
		}

		private void restart()
		{
			player.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
			player.Health = 100;
			player.killCount = 0;
			wave = 1;

			enemyList.Clear();
			playerProjectileList.Clear();
			enemyProjectileList.Clear();

			enemyList.Add(new BasicEnemy(textureCache[1], Vector2.Zero));
		}
		private void drawHud(SpriteBatch spriteBatch, GameTime gameTime)
		{
			// Info/Stats (Top)
			spriteBatch.DrawString(fontCache[0], player.Health.ToString(), new Vector2(30, 30), Color.GreenYellow);
			spriteBatch.DrawString(fontCache[0], "Wave " + wave.ToString(), new Vector2((GraphicsDevice.Viewport.Width / 2 - 45), 30), Color.DarkGray);
			spriteBatch.DrawString(fontCache[0], player.killCount.ToString(), new Vector2((GraphicsDevice.Viewport.Width - 60), 30), Color.DarkGray);

			// Specials (Bottom)
			for (int i = (player.Specials.Count - 1); i >= 0; i--)
			{
				Color specialColor = Color.Black;
				if ((gameTime.TotalGameTime - player.Specials[i].lastUse) > player.Specials[i].cooldown && player.Specials[i].unlocked) specialColor = Color.White;

				spriteBatch.Draw(player.Specials[i].texture, new Vector2(100 + (i * 100), (GraphicsDevice.Viewport.Height - 100)), null, specialColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				if (player.usingController && player.controllerSpecialSelection == i) spriteBatch.Draw(textureCache[8], new Vector2(100 + (i * 100), (GraphicsDevice.Viewport.Height - 100)), null, specialColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			}	

			// Misc / Other
			if (debugMode) spriteBatch.DrawString(fontCache[0], "Debug Mode", new Vector2((GraphicsDevice.Viewport.Width - 135), GraphicsDevice.Viewport.Height - 30), Color.OrangeRed);
			if (player.Health <= 0) spriteBatch.DrawString(fontCache[0], "  You are dead!\nPress R to restart", new Vector2((GraphicsDevice.Viewport.Width / 2 - 93), (GraphicsDevice.Viewport.Height / 2)), Color.Maroon);
		}

		private void drawFastMessages(SpriteBatch spriteBatch, GameTime gameTime)
		{
			for (int i = (fastMessages.Count - 1); i >= 0; i--)
			{
				spriteBatch.DrawString(fontCache[0], fastMessages[i].Message, fastMessages[i].Position, fastMessages[i].Color);

				if ((gameTime.TotalGameTime - fastMessages[i].GameTime) > fastMessages[i].Duration) fastMessages.RemoveAt(i);
			}
		}
	}
}
