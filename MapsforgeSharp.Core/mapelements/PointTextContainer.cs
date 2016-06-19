﻿/*
 * Copyright 2010, 2011, 2012, 2013 mapsforge.org
 * Copyright 2014 Ludwig M Brinckmann
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

using System.Text;
using MapsforgeSharp.Core.Graphics;
using MapsforgeSharp.Core.Model;

namespace MapsforgeSharp.Core.Mapelements
{
	public abstract class PointTextContainer : MapElementContainer
	{
		public readonly bool isVisible;
		public readonly int maxTextWidth;
		public readonly IPaint paintBack;
		public readonly IPaint paintFront;
		public readonly Position position;
		public readonly SymbolContainer symbolContainer;
		public readonly string text;
		public readonly int textHeight;
		public readonly int textWidth;

		/// <summary>
		/// Create a new point container, that holds the x-y coordinates of a point, a text variable, two paint objects, and
		/// a reference on a symbolContainer, if the text is connected with a POI.
		/// </summary>
		protected internal PointTextContainer(Point point, Display display, int priority, string text, IPaint paintFront, IPaint paintBack, SymbolContainer symbolContainer, Position position, int maxTextWidth) : base(point, display, priority)
		{

			this.maxTextWidth = maxTextWidth;
			this.text = text;
			this.symbolContainer = symbolContainer;
			this.paintFront = paintFront;
			this.paintBack = paintBack;
			this.position = position;
			if (paintBack != null)
			{
				this.textWidth = paintBack.GetTextWidth(text);
				this.textHeight = paintBack.GetTextHeight(text);
			}
			else
			{
				this.textWidth = paintFront.GetTextWidth(text);
				this.textHeight = paintFront.GetTextHeight(text);
			}
			this.isVisible = !this.paintFront.Transparent || (this.paintBack != null && !this.paintBack.Transparent);
		}

		public override bool ClashesWith(MapElementContainer other)
		{
			if (base.ClashesWith(other))
			{
				return true;
			}
			if (!(other is PointTextContainer))
			{
				return false;
			}
			PointTextContainer ptc = (PointTextContainer) other;
			if (this.text.Equals(ptc.text) && this.xy.Distance(ptc.xy) < 200)
			{
				return true;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is PointTextContainer))
			{
				return false;
			}
			PointTextContainer other = (PointTextContainer) obj;
			if (!this.text.Equals(other.text))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int result = base.GetHashCode();
			result = 31 * result + text.GetHashCode();
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.ToString());
			stringBuilder.Append(", text=");
			stringBuilder.Append(this.text);
			return stringBuilder.ToString();
		}
	}
}