using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RectanglePacker
{
	public class DynamicRectanglePacker
	{
		List<Rectangle> freeRectangles;
		List<Rectangle> allocatedRectangles;

		public Size Size { get; }

		public IEnumerable<Rectangle> FreeRectangles
		{
			get
			{
				return freeRectangles;
			}
		}

		public IEnumerable<Rectangle> AllocatedRectangles
		{
			get
			{
				return allocatedRectangles;
			}
		}

		public int FreeArea
		{
			get
			{
				int total = 0;
				foreach (var rect in FreeRectangles)
				{
					total += rect.Width * rect.Height;
				}
				return total;
			}
		}

		public int AllocatedArea
		{
			get
			{
				int total = 0;
				foreach (var rect in AllocatedRectangles)
				{
					total += rect.Width * rect.Height;
				}
				return total;
			}
		}

		public DynamicRectanglePacker(Size size)
		{
			Size = size;

			Clear();
		}

		public Rectangle? Add(Size size)
		{
			// Find a free rectangle that is large enough to contain a rectangle with the specified size.
			int index = freeRectangles.FindIndex(rect => rect.Width >= size.Width && rect.Height >= size.Height);

			// If no rectangle large enough is found, return null.
			if (index == -1)
			{
				return null;
			}

			// Remove the rectangle to be split from the list of free rectangles.
			Rectangle rect = freeRectangles[index];
			freeRectangles.RemoveAt(index);


			// If the width taken up by the new rectangle is less than the width of the free rectangle, re-add the remaining space.
			int xRemainder = rect.Width - size.Width;
			if (xRemainder > 0)
			{
				freeRectangles.Add(new Rectangle(rect.X + size.Width, rect.Y, xRemainder, size.Height));
			}

			// Do the same, but with the space left over on the horizontal axis.
			int yRemainder = rect.Height - size.Height;
			if (yRemainder > 0)
			{
				freeRectangles.Add(new Rectangle(rect.X, rect.Y + size.Height, rect.Width, yRemainder));
			}

			freeRectangles.Sort((r1, r2) => r1.Height - r2.Height);

			Rectangle result = new Rectangle(rect.Location, size);
			allocatedRectangles.Add(result);

			return result;
		}

		public void Clear()
		{
			freeRectangles = new List<Rectangle>
			{
				new Rectangle(Point.Empty, Size)
			};
			allocatedRectangles = new List<Rectangle>();
		}
	}
}
