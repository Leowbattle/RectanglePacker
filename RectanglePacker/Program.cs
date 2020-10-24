using SDL2;
using System;
using System.Drawing;
using System.Linq;

namespace RectanglePacker
{
	class Program
	{
		public IntPtr Window { get; private set; }
		public IntPtr Renderer { get; private set; }

		Size WindowSize = new Size(1920, 1080);

		DynamicRectanglePacker packer = new DynamicRectanglePacker(new Size(1024, 1024));

		Random random = new Random();

		void Run()
		{
			SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
			Window = SDL.SDL_CreateWindow("Rectangle Packer", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, WindowSize.Width, WindowSize.Height, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
			Renderer = SDL.SDL_CreateRenderer(Window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

			while (true)
			{
				while (SDL.SDL_PollEvent(out var e) != 0)
				{
					switch (e.type)
					{
						case SDL.SDL_EventType.SDL_QUIT:
							goto end;
						case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
							packer.Clear();
							for (int i = 0; i < 1000; i++)
							{
								var w = random.Next(10, 50);
								var h = random.Next(10, 50);
								packer.Add(new Size(w, h));
							}
							Console.WriteLine($"{packer.AllocatedRectangles.Count()}, wasted space = {packer.AllocatedArea / (float)(packer.Size.Width * packer.Size.Height) * 100}%");

							break;
						default:
							break;
					}
				}

				SDL.SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 0);
				SDL.SDL_RenderClear(Renderer);

				var rect = new Rectangle(Point.Empty, packer.Size);
				DrawRect(rect, Color.Green);

				foreach (var allocatedRect in packer.AllocatedRectangles)
				{
					allocatedRect.Inflate(-1, -1);
					DrawRect(allocatedRect, Color.Red);
				}

				foreach (var freeRect in packer.FreeRectangles)
				{
					freeRect.Inflate(-1, -1);
					DrawRect(freeRect, Color.Blue);
				}

				SDL.SDL_RenderPresent(Renderer);
			}
			end:;
		}

		void DrawRect(Rectangle rect, Color color)
		{
			SDL.SDL_SetRenderDrawColor(Renderer, color.R, color.G, color.B, color.A);

			var sdlRect = new SDL.SDL_Rect
			{
				x = rect.X,
				y = rect.Y,
				w = rect.Width,
				h = rect.Height
			};
			SDL.SDL_RenderFillRect(Renderer, ref sdlRect);
		}

		static void Main(string[] args)
		{
			new Program().Run();
		}
	}
}
