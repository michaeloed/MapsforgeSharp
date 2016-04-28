﻿/*
 * Copyright 2010, 2011, 2012, 2013 mapsforge.org
 * Copyright 2014 Ludwig M Brinckmann
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

namespace org.mapsforge.map.util
{
    using System;

    using BoundingBox = org.mapsforge.core.model.BoundingBox;
	using Dimension = org.mapsforge.core.model.Dimension;
	using LatLong = org.mapsforge.core.model.LatLong;
	using MapPosition = org.mapsforge.core.model.MapPosition;
	using Point = org.mapsforge.core.model.Point;
	using MercatorProjection = org.mapsforge.core.util.MercatorProjection;

	public sealed class MapPositionUtil
	{
		public static BoundingBox GetBoundingBox(MapPosition mapPosition, Dimension canvasDimension, int tileSize)
		{
			long mapSize = MercatorProjection.GetMapSize(mapPosition.zoomLevel, tileSize);
			double pixelX = MercatorProjection.LongitudeToPixelX(mapPosition.latLong.longitude, mapSize);
			double pixelY = MercatorProjection.LatitudeToPixelY(mapPosition.latLong.latitude, mapSize);

			int halfCanvasWidth = canvasDimension.width / 2;
			int halfCanvasHeight = canvasDimension.height / 2;

			double pixelXMin = Math.Max(0, pixelX - halfCanvasWidth);
			double pixelYMin = Math.Max(0, pixelY - halfCanvasHeight);
			double pixelXMax = Math.Min(mapSize, pixelX + halfCanvasWidth);
			double pixelYMax = Math.Min(mapSize, pixelY + halfCanvasHeight);

			double minLatitude = MercatorProjection.PixelYToLatitude(pixelYMax, mapSize);
			double minLongitude = MercatorProjection.PixelXToLongitude(pixelXMin, mapSize);
			double maxLatitude = MercatorProjection.PixelYToLatitude(pixelYMin, mapSize);
			double maxLongitude = MercatorProjection.PixelXToLongitude(pixelXMax, mapSize);

			return new BoundingBox(minLatitude, minLongitude, maxLatitude, maxLongitude);
		}

		public static Point GetTopLeftPoint(MapPosition mapPosition, Dimension canvasDimension, int tileSize)
		{
			LatLong centerPoint = mapPosition.latLong;

			int halfCanvasWidth = canvasDimension.width / 2;
			int halfCanvasHeight = canvasDimension.height / 2;

			long mapSize = MercatorProjection.GetMapSize(mapPosition.zoomLevel, tileSize);
			double pixelX = Math.Round(MercatorProjection.LongitudeToPixelX(centerPoint.longitude, mapSize));
			double pixelY = Math.Round(MercatorProjection.LatitudeToPixelY(centerPoint.latitude, mapSize));
			return new Point((int) pixelX - halfCanvasWidth, (int) pixelY - halfCanvasHeight);
		}

		private MapPositionUtil()
		{
			throw new System.InvalidOperationException();
		}
	}
}