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

using System;
using MapsforgeSharp.Core.Model;
using MapsforgeSharp.Reader.Header;

namespace MapsforgeSharp.Reader
{
	internal class QueryParameters
	{
		internal long fromBaseTileX;
		internal long fromBaseTileY;
		internal long fromBlockX;
		internal long fromBlockY;
		internal int queryTileBitmask;
		internal int queryZoomLevel;
		internal long toBaseTileX;
		internal long toBaseTileY;
		internal long toBlockX;
		internal long toBlockY;
		internal bool useTileBitmask;

		public virtual void CalculateBaseTiles(Tile tile, SubFileParameter subFileParameter)
		{
			if (tile.ZoomLevel < subFileParameter.BaseZoomLevel)
			{
				// calculate the XY numbers of the upper left and lower right sub-tiles
				int zoomLevelDifference = subFileParameter.BaseZoomLevel - tile.ZoomLevel;
				this.fromBaseTileX = tile.TileX << zoomLevelDifference;
				this.fromBaseTileY = tile.TileY << zoomLevelDifference;
				this.toBaseTileX = this.fromBaseTileX + (1 << zoomLevelDifference) - 1;
				this.toBaseTileY = this.fromBaseTileY + (1 << zoomLevelDifference) - 1;
				this.useTileBitmask = false;
			}
			else if (tile.ZoomLevel > subFileParameter.BaseZoomLevel)
			{
				// calculate the XY numbers of the parent base tile
				int zoomLevelDifference = tile.ZoomLevel - subFileParameter.BaseZoomLevel;
				this.fromBaseTileX = (int)((uint)tile.TileX >> zoomLevelDifference);
				this.fromBaseTileY = (int)((uint)tile.TileY >> zoomLevelDifference);
				this.toBaseTileX = this.fromBaseTileX;
				this.toBaseTileY = this.fromBaseTileY;
				this.useTileBitmask = true;
				this.queryTileBitmask = QueryCalculations.CalculateTileBitmask(tile, zoomLevelDifference);
			}
			else
			{
				// use the tile XY numbers of the requested tile
				this.fromBaseTileX = tile.TileX;
				this.fromBaseTileY = tile.TileY;
				this.toBaseTileX = this.fromBaseTileX;
				this.toBaseTileY = this.fromBaseTileY;
				this.useTileBitmask = false;
			}
		}

		public virtual void CalculateBlocks(SubFileParameter subFileParameter)
		{
			// calculate the blocks in the file which need to be read
			this.fromBlockX = Math.Max(this.fromBaseTileX - subFileParameter.BoundaryTileLeft, 0);
			this.fromBlockY = Math.Max(this.fromBaseTileY - subFileParameter.BoundaryTileTop, 0);
			this.toBlockX = Math.Min(this.toBaseTileX - subFileParameter.BoundaryTileLeft, subFileParameter.BlocksWidth - 1);
			this.toBlockY = Math.Min(this.toBaseTileY - subFileParameter.BoundaryTileTop, subFileParameter.BlocksHeight - 1);
		}
	}
}