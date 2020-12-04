using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;
using Terraria.ID;

namespace Terraria.IO
{
	public partial class ResourcePack
	{
		public static int[] defaultNPCFrames;
		public static DrawAnimation[] defaultItemFrames;
		public static int[] defaultTileFrames;
		public static byte[] defaultWallFrames;

		public Dictionary<int, int> npcFrames = new Dictionary<int, int>();
		public Dictionary<int, DrawAnimation> itemFrames = new Dictionary<int, DrawAnimation>();
		public Dictionary<int, int> tileFrames = new Dictionary<int, int>();
		public Dictionary<byte, byte> wallFrames = new Dictionary<byte, byte>();

		public static void Initialize() {
			defaultNPCFrames = Main.npcFrameCount;
			defaultItemFrames = new DrawAnimation[ItemID.Count];
			defaultTileFrames = Main.tileFrame;
			defaultWallFrames = Main.wallFrame;

			for (int i = 0; i < ItemID.Count; i++)
				defaultItemFrames[i] = Main.itemAnimations[i];
		}

		public void LoadFrameManifest() {
			if (HasFile("Content" + Path.DirectorySeparatorChar + "frames.json")) {
				JObject jObject;
				using (Stream stream = OpenStream("frames.json")) {
					using (StreamReader reader = new StreamReader(stream)) {
						jObject = JObject.Parse(reader.ReadToEnd());
					}
				}

				foreach (string npc in jObject["NPCs"].ToObject<string>().Split(';')) {
					string[] separated = npc.Split('=');

					npcFrames.Add(int.Parse(separated[0]), int.Parse(separated[1]));
				}

				foreach (string item in jObject["Items"].ToObject<string>().Split(';')) {
					string[] separated = item.Split('=');
					string[] animationSeparated = separated[1].Split(',');
					itemFrames.Add(int.Parse(separated[0]), new DrawAnimationVertical(int.Parse(animationSeparated[0]), int.Parse(animationSeparated[1])));
				}

				foreach (string tile in jObject["Tiles"].ToObject<string>().Split(';')) {
					string[] separated = tile.Split('=');

					tileFrames.Add(int.Parse(separated[0]), int.Parse(separated[1]));
				}

				foreach (string wall in jObject["Walls"].ToObject<string>().Split(';')) {
					string[] separated = wall.Split('=');

					wallFrames.Add(byte.Parse(separated[0]), byte.Parse(separated[1]));
				}
			}
		}

		public void LoadFrames() {
			// ItemID.Count is the largest Count
			for (int i = 0; i < ItemID.Count; i++)
				if (npcFrames.ContainsKey(i))
					Main.npcFrameCount[i] = npcFrames[i];

			for (int i = 0; i < TileID.Count; i++)
				if (itemFrames.ContainsKey(i))
					Main.itemAnimations[i] = itemFrames[i];

			for (int i = 0; i < TileID.Count; i++)
				if (tileFrames.ContainsKey(i))
					Main.tileFrame[i] = tileFrames[i];

			for (int i = 0; i < WallID.Count; i++)
				if (wallFrames.ContainsKey((byte)i))
					Main.wallFrame[i] = wallFrames[(byte)i];
		}
	}
}
