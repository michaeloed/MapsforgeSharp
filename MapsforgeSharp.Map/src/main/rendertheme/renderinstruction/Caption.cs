﻿/*
 * Copyright 2010, 2011, 2012, 2013 mapsforge.org
 * Copyright 2014-2015 Ludwig M Brinckmann
 * Copyright 2014 devemux86
 * Copyright 2016 Dirk Weltz
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

namespace org.mapsforge.map.rendertheme.renderinstruction
{
    using System.Collections.Generic;
    using System.Xml;

    using Align = MapsforgeSharp.Core.Graphics.Align;
    using IBitmap = MapsforgeSharp.Core.Graphics.IBitmap;
    using Color = MapsforgeSharp.Core.Graphics.Color;
    using Display = MapsforgeSharp.Core.Graphics.Display;
    using FontFamily = MapsforgeSharp.Core.Graphics.FontFamily;
    using FontStyle = MapsforgeSharp.Core.Graphics.FontStyle;
    using IGraphicFactory = MapsforgeSharp.Core.Graphics.IGraphicFactory;
    using IPaint = MapsforgeSharp.Core.Graphics.IPaint;
    using Position = MapsforgeSharp.Core.Graphics.Position;
    using Style = MapsforgeSharp.Core.Graphics.Style;
    using PolylineContainer = org.mapsforge.map.layer.renderer.PolylineContainer;
    using DisplayModel = org.mapsforge.map.model.DisplayModel;
    using PointOfInterest = org.mapsforge.core.datastore.PointOfInterest;

    /// <summary>
    /// Represents a text label on the map.
    /// 
    /// If a bitmap symbol is present the caption position is calculated relative to the bitmap, the
    /// center of which is at the point of the POI. The bitmap itself is never rendered.
    /// 
    /// </summary>
    public class Caption : RenderInstruction
	{

		private IBitmap bitmap;
		private Position position;
		private Display display;
		private float dy;
		private readonly IDictionary<sbyte?, float?> dyScaled;

		private readonly IPaint fill;
		private readonly IDictionary<sbyte?, IPaint> fills;

		private float fontSize;
		private readonly float gap;
		private readonly int maxTextWidth;
		private int priority;
		private readonly IPaint stroke;
		private readonly IDictionary<sbyte?, IPaint> strokes;

		private TextKey textKey;
		public const float DEFAULT_GAP = 5f;

		internal string symbolId;

		public Caption(IGraphicFactory graphicFactory, DisplayModel displayModel, string elementName, XmlReader reader, IDictionary<string, Symbol> symbols) : base(graphicFactory, displayModel)
		{
			this.fill = graphicFactory.CreatePaint();
			this.fill.Color = Color.BLACK;
			this.fill.Style = Style.FILL;
			this.fills = new Dictionary<sbyte?, IPaint>();

			this.stroke = graphicFactory.CreatePaint();
			this.stroke.Color = Color.BLACK;
			this.stroke.Style = Style.STROKE;
			this.strokes = new Dictionary<sbyte?, IPaint>();
			this.dyScaled = new Dictionary<sbyte?, float?>();


			this.display = Display.IFSPACE;

			this.gap = DEFAULT_GAP * displayModel.ScaleFactor;

			ExtractValues(graphicFactory, displayModel, elementName, reader);

			if (!string.ReferenceEquals(this.symbolId, null))
			{
				Symbol symbol = symbols[this.symbolId];
				if (symbol != null)
				{
					this.bitmap = symbol.Bitmap;
				}
			}

            if (this.position == null)
            {
                // sensible defaults: below if symbolContainer is present, center if not
                if (this.bitmap == null)
                {
                    this.position = Position.CENTER;
                }
                else
                {
                    this.position = Position.BELOW;
                }
            }
            else if (this.position == Position.CENTER || this.position == Position.BELOW || this.position == Position.ABOVE)
            {
                this.stroke.TextAlign = Align.CENTER;
                this.fill.TextAlign = Align.CENTER;
            }
            else if (this.position == Position.BELOW_LEFT || this.position == Position.ABOVE_LEFT || this.position == Position.LEFT)
            {
                this.stroke.TextAlign = Align.RIGHT;
                this.fill.TextAlign = Align.RIGHT;
            }
            else if (this.position == Position.BELOW_RIGHT || this.position == Position.ABOVE_RIGHT || this.position == Position.RIGHT)
            {
                this.stroke.TextAlign = Align.LEFT;
                this.fill.TextAlign = Align.LEFT;
            }
            else {
                throw new System.ArgumentException("Position invalid");
            }

			this.maxTextWidth = displayModel.MaxTextWidth;
		}

		public override void Destroy()
		{
			// no-op
		}

		public override void RenderNode(RenderCallback renderCallback, RenderContext renderContext, PointOfInterest poi)
		{

			if (Display.NEVER == this.display)
			{
				return;
			}

			string caption = this.textKey.GetValue(poi.Tags);
			if (string.ReferenceEquals(caption, null))
			{
				return;
			}

			float horizontalOffset = 0f;

			float verticalOffset = this.dyScaled[renderContext.rendererJob.tile.ZoomLevel] ?? this.dy;

			if (this.bitmap != null)
			{
				horizontalOffset = ComputeHorizontalOffset();
				verticalOffset = ComputeVerticalOffset(renderContext.rendererJob.tile.ZoomLevel);
			}

			renderCallback.RenderPointOfInterestCaption(renderContext, this.display, this.priority, caption, horizontalOffset, verticalOffset, getFillPaint(renderContext.rendererJob.tile.ZoomLevel), getStrokePaint(renderContext.rendererJob.tile.ZoomLevel), this.position, this.maxTextWidth, poi);
		}

		public override void RenderWay(RenderCallback renderCallback, RenderContext renderContext, PolylineContainer way)
		{

			if (Display.NEVER == this.display)
			{
				return;
			}

			string caption = this.textKey.GetValue(way.Tags);
			if (string.ReferenceEquals(caption, null))
			{
				return;
			}

			float horizontalOffset = 0f;
            float verticalOffset = this.dyScaled[renderContext.rendererJob.tile.ZoomLevel] ?? this.dy;

			if (this.bitmap != null)
			{
				horizontalOffset = ComputeHorizontalOffset();
				verticalOffset = ComputeVerticalOffset(renderContext.rendererJob.tile.ZoomLevel);
			}

			renderCallback.RenderAreaCaption(renderContext, this.display, this.priority, caption, horizontalOffset, verticalOffset, getFillPaint(renderContext.rendererJob.tile.ZoomLevel), getStrokePaint(renderContext.rendererJob.tile.ZoomLevel), this.position, this.maxTextWidth, way);
		}

		public override void ScaleStrokeWidth(float scaleFactor, sbyte zoomLevel)
		{
			// do nothing
		}

		public override void ScaleTextSize(float scaleFactor, sbyte zoomLevel)
		{
			IPaint f = graphicFactory.CreatePaint(this.fill);
			f.TextSize = this.fontSize * scaleFactor;
			this.fills[zoomLevel] = f;

			IPaint s = graphicFactory.CreatePaint(this.stroke);
			s.TextSize = this.fontSize * scaleFactor;
			this.strokes[zoomLevel] = s;

			this.dyScaled[zoomLevel] = this.dy * scaleFactor;
		}

		private float ComputeHorizontalOffset()
		{
			// compute only the offset required by the bitmap, not the text size,
			// because at this point we do not know the text boxing
			if (Position.RIGHT == this.position || Position.LEFT == this.position || Position.BELOW_RIGHT == this.position || Position.BELOW_LEFT == this.position || Position.ABOVE_RIGHT == this.position || Position.ABOVE_LEFT == this.position)
			{
				float horizontalOffset = this.bitmap.Width / 2f + this.gap;
				if (Position.LEFT == this.position || Position.BELOW_LEFT == this.position || Position.ABOVE_LEFT == this.position)
				{
					horizontalOffset *= -1f;
				}
				return horizontalOffset;
			}
			return 0;
		}

		private float ComputeVerticalOffset(sbyte zoomLevel)
		{
			float verticalOffset = this.dyScaled[zoomLevel].Value;

			if (Position.ABOVE == this.position || Position.ABOVE_LEFT == this.position || Position.ABOVE_RIGHT == this.position)
			{
				verticalOffset -= this.bitmap.Height / 2f + this.gap;
			}
			else if (Position.BELOW == this.position || Position.BELOW_LEFT == this.position || Position.BELOW_RIGHT == this.position)
			{
				verticalOffset += this.bitmap.Height / 2f + this.gap;
			}
			return verticalOffset;
		}

        private void ExtractValues(IGraphicFactory graphicFactory, DisplayModel displayModel, string elementName, XmlReader reader)
		{
			FontFamily fontFamily = FontFamily.DEFAULT;
			FontStyle fontStyle = FontStyle.NORMAL;

			for (int i = 0; i < reader.AttributeCount; ++i)
			{
                reader.MoveToAttribute(i);

				string name = reader.Name;
				string value = reader.Value;

				if (K.Equals(name))
				{
					this.textKey = TextKey.getInstance(value);
				}
				else if (POSITION.Equals(name))
				{
					this.position = Position.FromString(value);
				}
				else if (CAT.Equals(name))
				{
					this.category = value;
				}
				else if (DISPLAY.Equals(name))
				{
					this.display = Display.FromString(value);
				}
				else if (DY.Equals(name))
				{
					this.dy = float.Parse(value) * displayModel.ScaleFactor;
				}
				else if (FONT_FAMILY.Equals(name))
				{
					fontFamily = FontFamily.FromString(value);
				}
				else if (FONT_STYLE.Equals(name))
				{
					fontStyle = FontStyle.FromString(value);
				}
				else if (FONT_SIZE.Equals(name))
				{
					this.fontSize = XmlUtils.ParseNonNegativeFloat(name, value) * displayModel.ScaleFactor;
				}
				else if (FILL.Equals(name))
				{
					this.fill.Color = (Color)XmlUtils.GetColor(graphicFactory, value);
				}
				else if (PRIORITY.Equals(name))
				{
					this.priority = int.Parse(value);
				}
				else if (STROKE.Equals(name))
				{
					this.stroke.Color = (Color)XmlUtils.GetColor(graphicFactory, value);
				}
				else if (STROKE_WIDTH.Equals(name))
				{
					this.stroke.StrokeWidth = XmlUtils.ParseNonNegativeFloat(name, value) * displayModel.ScaleFactor;
				}
				else if (SYMBOL_ID.Equals(name))
				{
					this.symbolId = value;
				}
				else
				{
					throw XmlUtils.CreateXmlReaderException(elementName, name, value, i);
				}
			}

			this.fill.SetTypeface(fontFamily, fontStyle);
			this.stroke.SetTypeface(fontFamily, fontStyle);

			XmlUtils.CheckMandatoryAttribute(elementName, K, this.textKey);
		}

		private IPaint getFillPaint(sbyte zoomLevel)
		{
			IPaint paint = fills[zoomLevel];
			if (paint == null)
			{
				paint = this.fill;
			}
			return paint;
		}

		private IPaint getStrokePaint(sbyte zoomLevel)
		{
			IPaint paint = strokes[zoomLevel];
			if (paint == null)
			{
				paint = this.stroke;
			}
			return paint;
		}
	}
}