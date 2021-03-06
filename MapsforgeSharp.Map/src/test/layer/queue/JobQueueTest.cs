﻿/*
 * Copyright 2010, 2011, 2012, 2013 mapsforge.org
 * Copyright 2014 Ludwig M Brinckmann
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
namespace org.mapsforge.map.layer.queue
{

	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using Tile = org.mapsforge.core.model.Tile;
	using FixedTileSizeDisplayModel = org.mapsforge.map.model.FixedTileSizeDisplayModel;
	using MapViewPosition = org.mapsforge.map.model.MapViewPosition;

	public class JobQueueTest
	{

		private const int TILE_SIZE = 256;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void jobQueueTest() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void jobQueueTest()
		{
			MapViewPosition mapViewPosition = new MapViewPosition(new FixedTileSizeDisplayModel(256));
			JobQueue<Job> jobQueue = new JobQueue<Job>(mapViewPosition, new FixedTileSizeDisplayModel(256));
			Assert.assertEquals(0, jobQueue.size());

			Tile tile1 = new Tile(0, 0, (sbyte) 1, TILE_SIZE);
			Tile tile2 = new Tile(0, 0, (sbyte) 0, TILE_SIZE);
			Tile tile3 = new Tile(0, 0, (sbyte) 2, TILE_SIZE);

			Job job1 = new Job(tile1, false);
			Job job2 = new Job(tile2, false);
			Job job3 = new Job(tile3, false);
			jobQueue.add(job1);
			jobQueue.add(job2);
			jobQueue.add(job3);
			Assert.assertEquals(3, jobQueue.size());

			jobQueue.add(job1);
			Assert.assertEquals(3, jobQueue.size());

			jobQueue.notifyWorkers();

			Assert.assertEquals(job2, jobQueue.get());
			Assert.assertEquals(job1, jobQueue.get());
			Assert.assertEquals(job3, jobQueue.get());

			Assert.assertEquals(0, jobQueue.size());

			jobQueue.remove(job1);
			jobQueue.remove(job2);
			jobQueue.remove(job3);

		}
	}

}