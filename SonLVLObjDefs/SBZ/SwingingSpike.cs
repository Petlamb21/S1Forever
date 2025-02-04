using SonicRetro.SonLVL.API;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace S1FObjectDefinitions.SBZ
{
	class SwingingSpike : ObjectDefinition
	{
		private PropertySpec[] properties = new PropertySpec[2];
		private readonly Sprite[] sprites = new Sprite[3];
		
		public override void Init(ObjectData data)
		{
			// #Forever - "BossRush" folder check
			if (LevelData.StageInfo.folder.EndsWith("BossRush")) // not sure if we should keep using EndsWith since Forever is based off of pre-Origins? may as well keep it like this anyways though, in case they update it
			{
				BitmapBits sheet = LevelData.GetSpriteSheet("MBZ/Objects.gif");
				sprites[0] = new Sprite(sheet.GetSection(76, 330, 16, 16), -8, -9); // (note the different offset)
				sprites[1] = new Sprite(sheet.GetSection(93, 330, 16, 16), -8, -8);
				sprites[2] = new Sprite(sheet.GetSection(44, 412, 48, 48), -24, -24);
			}
			else
			{
				BitmapBits sheet = LevelData.GetSpriteSheet("SBZ/Objects.gif");
				sprites[0] = new Sprite(sheet.GetSection(92, 31, 30, 30), -15, -23);
				sprites[1] = new Sprite(sheet.GetSection(65, 106, 16, 16), -8, -8);
				sprites[2] = new Sprite(sheet.GetSection(397, 182, 48, 48), -24, -24);
			}
			
			properties[0] = new PropertySpec("Size", typeof(int), "Extended",
				"How many chains the Spike should hang off of.", null,
				(obj) => obj.PropertyValue,
				(obj, value) => obj.PropertyValue = (byte)((int)value));
			
			properties[1] = new PropertySpec("Inverted", typeof(int), "Extended",
				"If the Spike's movement should be inverted, compared to other Spikes.", null,
				(obj) => (((V4ObjectEntry)obj).Direction == RSDKv3_4.Tiles128x128.Block.Tile.Directions.FlipX),
				(obj, value) => ((V4ObjectEntry)obj).Direction = (RSDKv3_4.Tiles128x128.Block.Tile.Directions)((bool)value ? 1 : 0)); // could be more direct instead of bool>int>Direction but the whole class name is p long, so..
		}
		
		public override ReadOnlyCollection<byte> Subtypes
		{
			get { return new ReadOnlyCollection<byte>(new byte[] {4, 5, 6, 7, 8, 9, 10}); } // it can be any value, but why not give a few starting ones
		}
		
		public override byte DefaultSubtype
		{
			get { return 6; }
		}
		
		public override PropertySpec[] CustomProperties
		{
			get { return properties; }
		}

		public override string SubtypeName(byte subtype)
		{
			return subtype + " chains";
		}

		public override Sprite Image
		{
			get { return sprites[2]; }
		}

		public override Sprite SubtypeImage(byte subtype)
		{
			return sprites[1];
		}

		public override Sprite GetSprite(ObjectEntry obj)
		{
			List<Sprite> sprs = new List<Sprite>();
			int sy = 16;
			for (int i = 0; i < obj.PropertyValue; i++)
			{
				sprs.Add(new Sprite(sprites[1], 0, sy));
				sy += 16;
			}
			sy -= 8;
			sprs.Add(new Sprite(sprites[2], 0, sy));
			sprs.Add(sprites[0]); // yeah the post is drawn last instead of first, don't ask me what's up with that
			return new Sprite(sprs.ToArray());
		}
		
		public override Sprite GetDebugOverlay(ObjectEntry obj)
		{
			int l = (obj.PropertyValue * 16) + 8;
			var overlay = new BitmapBits(2 * l + 1, l + 1);
			overlay.DrawCircle(6, l, 0, l); // LevelData.ColorWhite
			return new Sprite(overlay, -l, 0);
		}
		
		public override Rectangle GetBounds(ObjectEntry obj)
		{
			Rectangle bounds = sprites[2].Bounds;
			bounds.Offset(obj.X, obj.Y + (obj.PropertyValue * 16) + 8);
			return bounds;
		}
	}
}