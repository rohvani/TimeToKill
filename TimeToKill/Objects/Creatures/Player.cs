
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Specials;

namespace Creatures
{
    public class Player
    {
        public Texture2D PlayerTexture;
        public Vector2 Position;
		public int Health;

		public float Speed;

		public Vector2 Origin;
        public float Rotation;

		public int killCount;
		public TimeSpan lastAction;
		public List<Special> Specials;

		public bool usingController;
		public int controllerSpecialSelection;
   
        public int Width
        {
            get { return PlayerTexture.Width; }

        }
        public int Height
        {
            get { return PlayerTexture.Height; }

        }

        public void Initialize(Texture2D texture, Vector2 position)
        {
            this.PlayerTexture = texture;
			this.Position = position;

			this.killCount = 0;
			this.Health = 100;

			this.Speed = 4.7f;

			this.Specials = new List<Special>();

			this.Rotation = 0;
			this.Origin = new Vector2(Width, Height / 2);

			this.usingController = false;
			controllerSpecialSelection = 0;
		}

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
			spriteBatch.Draw(PlayerTexture, Position, null, Color.White, Rotation, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
