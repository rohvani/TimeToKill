using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TimeToKill;
using Creatures;

namespace Entities
{
    public class BasicEnemy : Creature
    {
        public BasicEnemy(Texture2D texture, Vector2 position) : base(texture, position)
        {
            this.position = position;
			this.origin = new Vector2(texture.Width/2, texture.Height/2);
			this.texture = texture;

			this.health = 10;

			this.lastAction = TimeSpan.FromSeconds(5f);
			this.rotation = 0f;
        }

        public override void Update(GameTime gameTime, Main game)
        {

        }
    }
}
