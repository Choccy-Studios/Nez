using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;


namespace Nez
{
	/// <summary>
	/// DEBUG only. Where the last rendered frame's draw calls came from: per Renderer/PostProcessor and per texture.
	/// </summary>
	public static class DrawCallStats
	{
		public struct Section
		{
			public object Source;
			public int Draws;
			public int Batches;
		}

		public struct TextureUse
		{
			public int Draws;
			public int Sprites;
		}

		public static bool Frozen;
		public static readonly List<Section> Sections = new List<Section>();
		public static readonly Dictionary<Texture, TextureUse> Textures = new Dictionary<Texture, TextureUse>();

		static long _sectionStartDraws;
		static int _sectionBatches;


		[Conditional("DEBUG")]
		public static void BeginFrame()
		{
			if (Frozen)
				return;

			Sections.Clear();
			Textures.Clear();
		}

		[Conditional("DEBUG")]
		public static void BeginSection()
		{
			_sectionStartDraws = Core.GraphicsDevice.Metrics.DrawCount;
			_sectionBatches = 0;
		}

		[Conditional("DEBUG")]
		public static void EndSection(object source)
		{
			if (Frozen)
				return;

			Sections.Add(new Section
			{
				Source = source,
				Draws = (int) (Core.GraphicsDevice.Metrics.DrawCount - _sectionStartDraws),
				Batches = _sectionBatches
			});
		}

		[Conditional("DEBUG")]
		public static void CountBatch() => _sectionBatches++;

		[Conditional("DEBUG")]
		public static void CountDraw(Texture texture, int sprites)
		{
			if (Frozen)
				return;

			Textures.TryGetValue(texture, out var use);
			use.Draws++;
			use.Sprites += sprites;
			Textures[texture] = use;
		}
	}
}
