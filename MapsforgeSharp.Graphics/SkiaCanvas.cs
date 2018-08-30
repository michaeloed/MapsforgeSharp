/*
 * Copyright 2016 Dirk Weltz
 * Copyright 2016 Michael Oed
 *
 * This program is free software: you can redistribute it and/or modify it under the
 * terms of the GNU Lesser General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
 * PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License along with
 * this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using SkiaSharp;
using MapsforgeSharp.Core.Graphics;
using MapsforgeSharp.Core.Model;

namespace MapsforgeSharp.Graphics
{
	public class SkiaCanvas : ICanvas
    {
        private readonly SKSurface nativeSurface;
        private readonly SKCanvas nativeCanvas;
        private readonly SKPaint nativePaint = new SKPaint() { IsAntialias = true, };
		private readonly int width;
		private readonly int height;

		public SkiaCanvas() : this(256, 256)
        {
		}

		public SkiaCanvas(int width, int height)
		{
			nativeSurface = SKSurface.Create(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
			nativeCanvas = nativeSurface.Canvas;

			this.width = width;
			this.height = height;
		}

		public SkiaCanvas(SKCanvas canvas)
        {
            nativeCanvas = canvas;
        }

		public SKCanvas NativeCanvas
		{
			get { return nativeCanvas; }
		}

        public SKImage Image
        {
            get
            {
				// TODO
				return nativeSurface.Snapshot();
			}
		}

        public Dimension Dimension
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Height
        {
            get
            {
				return this.height;
            }
        }

        public int Width
        {
            get
            {
				return this.width;
            }
        }

		public IBitmap Bitmap
		{
			set
			{
				throw new NotImplementedException();
			}
		}

		public void Destroy()
        {
			nativePaint.Dispose();
			nativeCanvas.Dispose();
			nativeSurface.Dispose();
        }

        public void DrawBitmap(IBitmap bitmap, IMatrix matrix)
        {
            throw new NotImplementedException();
        }

        public void DrawBitmap(IBitmap bitmap, int left, int top)
        {
            throw new NotImplementedException();
        }

        public void DrawCircle(int x, int y, int radius, IPaint paint)
        {
			nativeCanvas.DrawOval(new SKRect(x, y, x + radius, y + radius), ((SkiaPaint)paint).NativePaint);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, IPaint paint)
        {
			nativeCanvas.DrawLine(x1, y1, x2, y2, ((SkiaPaint)paint).NativePaint);
        }

        public void DrawPath(IPath path, IPaint paint)
        {
			//((SkiaPaint)paint).NativePaint.StrokeWidth = 1f;
			nativeCanvas.DrawPath(((SkiaPath)path).NativePath, ((SkiaPaint)paint).NativePaint);
        }

        public void DrawText(string text, int x, int y, IPaint paint)
        {
			nativeCanvas.DrawText(text, x, y, ((SkiaPaint)paint).NativePaint);
        }

        public void DrawTextRotated(string text, int x1, int y1, int x2, int y2, IPaint paint)
        {
            //throw new NotImplementedException();
        }

        public void FillColor(int color)
        {
			var white = SKColors.White;
			var skred = SKColors.Red;
			var skgreen = SKColors.Green;
			byte red = (byte)(color >> 16 & 0xff);
			byte green = (byte)(color >> 8 & 0xff);
			byte blue = (byte)(color & 0xff);
			byte alpha = (byte)(color >> 24 & 0xff);
			nativeCanvas.Clear(new SKColor((byte)(color >> 16 & 0xff), (byte)(color >> 8 & 0xff), (byte)(color & 0xff), (byte)(color >> 24 & 0xff)));
			//nativeCanvas.DrawColor(new SKColor((byte)(color >> 16 & 0xff), (byte)(color >> 8 & 0xff), (byte)(color & 0xff), (byte)(color >> 24 & 0xff)), SKXferMode.Clear);
		}

        public void FillColor(Color color)
        {
            throw new NotImplementedException();
        }

        public void ResetClip()
        {
			nativeCanvas.ClipRect(new SKRect(0, 0, width, height));
        }

        public void SetClip(int left, int top, int width, int height)
        {
			nativeCanvas.ClipRect(new SKRect(left, top, left + width, top + height));
		}

		public void SetClipDifference(int left, int top, int width, int height)
        {
			nativeCanvas.ClipRect(new SKRect(left, top, left + width, top + height));
        }
    }
}
