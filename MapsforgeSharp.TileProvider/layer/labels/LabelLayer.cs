﻿/*
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

namespace org.mapsforge.map.layer.labels
{
    using System.Collections.Generic;
    using System.Linq;

    using ICanvas = MapsforgeSharp.Core.Graphics.ICanvas;
	using IGraphicFactory = MapsforgeSharp.Core.Graphics.IGraphicFactory;
	using MapElementContainer = MapsforgeSharp.Core.Mapelements.MapElementContainer;
	using IMatrix = MapsforgeSharp.Core.Graphics.IMatrix;
	using BoundingBox = MapsforgeSharp.Core.Model.BoundingBox;
	using Point = MapsforgeSharp.Core.Model.Point;
	using Tile = MapsforgeSharp.Core.Model.Tile;
	using LayerUtil = org.mapsforge.map.util.LayerUtil;

	public class LabelLayer : Layer
	{
		private readonly LabelStore labelStore;
		private readonly IMatrix matrix;
		private IOrderedEnumerable<MapElementContainer> elementsToDraw;
		private ISet<Tile> lastTileSet;
		private int lastLabelStoreVersion;

		public LabelLayer(IGraphicFactory graphicFactory, LabelStore labelStore)
		{
			this.labelStore = labelStore;
			this.matrix = graphicFactory.CreateMatrix();
		}

		public override void Draw(BoundingBox boundingBox, sbyte zoomLevel, ICanvas canvas, Point topLeftPoint)
		{
			ISet<Tile> currentTileSet = LayerUtil.GetTiles(boundingBox, zoomLevel, displayModel.TileSize);
			if (!currentTileSet.Equals(lastTileSet) || lastLabelStoreVersion != labelStore.Version)
			{
				// only need to get new data set if either set of tiles changed or the label store
				lastTileSet = currentTileSet;
				lastLabelStoreVersion = labelStore.Version;
				IList<MapElementContainer> visibleItems = this.labelStore.GetVisibleItems(currentTileSet);

                // TODO this is code duplicated from CanvasRasterer::drawMapElements, should be factored out
                // what LayerUtil.collisionFreeOrdered gave us is a list where highest priority comes first,
                // so we need to reverse that in order to
                // draw elements in order of priority: lower priority first, so more important
                // elements will be drawn on top (in case of display=true) items.
                elementsToDraw = from element in LayerUtil.CollisionFreeOrdered(visibleItems) orderby element.Priority ascending select element;
            }

            foreach (MapElementContainer item in elementsToDraw)
			{
				item.Draw(canvas, topLeftPoint, this.matrix);
			}
		}
	}
}