using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Nez {
	public static class Sdl2 {
		private static IntPtr sdl;

		static Sdl2() {
			Initialize();
			BindMethods();
		}

		private static void Initialize() {
			var mg = typeof(Game).Assembly;
			var mgSdl = mg.GetType("Sdl");
			sdl = (IntPtr) mgSdl.GetField("NativeLibrary").GetValue(null);
		}

		private static void BindMethods() {
			SDL_GetError = Native.LoadFunction<d_sdl_geterror>(sdl, nameof(SDL_GetError));
			SDL_SetClipboardText = Native.LoadFunction<d_sdl_setclipboardtext>(sdl, nameof(SDL_SetClipboardText));
			SDL_GetClipboardText = Native.LoadFunction<d_sdl_getclipboardtext>(sdl, nameof(SDL_GetClipboardText));
			SDL_setenv = Native.LoadFunction<d_sdl_setenv>(sdl, nameof(SDL_setenv));
			SDL_getenv = Native.LoadFunction<d_sdl_getenv>(sdl, nameof(SDL_getenv));
			SDL_GetWindowDisplayIndex = Native.LoadFunction<d_sdl_getwindowdisplayindex>(sdl, nameof(SDL_GetWindowDisplayIndex));
			SDL_GetDisplayBounds = Native.LoadFunction<d_sdl_getdisplaybounds>(sdl, nameof(SDL_GetDisplayBounds));
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr d_sdl_geterror();

		public static d_sdl_geterror SDL_GetError;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int d_sdl_setclipboardtext(string text);

		public static d_sdl_setclipboardtext SDL_SetClipboardText;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate string d_sdl_getclipboardtext();

		public static d_sdl_getclipboardtext SDL_GetClipboardText;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int d_sdl_setenv(string name, string value, int overwrite);

		public static d_sdl_setenv SDL_setenv;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr d_sdl_getenv(string name);

		private static d_sdl_getenv SDL_getenv;

		public static string SDL_GetEnv(string name) {
			return Native.Utf8ToString(SDL_getenv(name));
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SDL_Rect {
			public int x;
			public int y;
			public int w;
			public int h;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int d_sdl_getwindowdisplayindex(IntPtr window);

		public static d_sdl_getwindowdisplayindex SDL_GetWindowDisplayIndex;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int d_sdl_getdisplaybounds(int displayIndex, out SDL_Rect rect);

		public static d_sdl_getdisplaybounds SDL_GetDisplayBounds;

		/// <summary>
		/// desktop-space bounds of the display the window is on. MonoGame only exposes that display's size, not where it sits.
		/// </summary>
		public static bool TryGetWindowDisplayBounds(IntPtr window, out Rectangle bounds) {
			bounds = default;
			if (SDL_GetWindowDisplayIndex == null || SDL_GetDisplayBounds == null)
				return false;

			var displayIndex = SDL_GetWindowDisplayIndex(window);
			if (displayIndex < 0 || SDL_GetDisplayBounds(displayIndex, out var rect) != 0)
				return false;

			bounds = new Rectangle(rect.x, rect.y, rect.w, rect.h);
			return rect.w > 0 && rect.h > 0;
		}
	}
}