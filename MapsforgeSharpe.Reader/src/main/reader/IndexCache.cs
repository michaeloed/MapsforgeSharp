﻿/*
 * Copyright 2010, 2011, 2012, 2013 mapsforge.org
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

namespace org.mapsforge.map.reader
{
    using core.util;
    using System;
    using System.Collections.Generic;
    using System.IO;

    //using LRUCache = org.mapsforge.core.util.LRUCache;
    using SubFileParameter = org.mapsforge.map.reader.header.SubFileParameter;

    /// <summary>
    /// A cache for database index blocks with a fixed size and LRU policy.
    /// </summary>
    internal class IndexCache
	{
		/// <summary>
		/// Number of index entries that one index block consists of.
		/// </summary>
		private const int INDEX_ENTRIES_PER_BLOCK = 128;

		/// <summary>
		/// Maximum size in bytes of one index block.
		/// </summary>
		private static readonly int SIZE_OF_INDEX_BLOCK = INDEX_ENTRIES_PER_BLOCK * SubFileParameter.BYTES_PER_INDEX_ENTRY;

		private readonly IDictionary<IndexCacheEntryKey, byte[]> map;
		private readonly Stream randomAccessFile;

		/// <param name="randomAccessFile">
		///            the map file from which the index should be read and cached. </param>
		/// <param name="capacity">
		///            the maximum number of entries in the cache. </param>
		/// <exception cref="IllegalArgumentException">
		///             if the capacity is negative. </exception>
		internal IndexCache(Stream randomAccessFile, int capacity)
		{
			this.randomAccessFile = randomAccessFile;
			this.map = (IDictionary < IndexCacheEntryKey, byte[]> ) new LRUCache<IndexCacheEntryKey, byte[]>(capacity);
		}

		/// <summary>
		/// Destroy the cache at the end of its lifetime.
		/// </summary>
		internal virtual void Destroy()
		{
			this.map.Clear();
		}

		/// <summary>
		/// Returns the index entry of a block in the given map file. If the required index entry is not cached, it will be
		/// read from the map file index and put in the cache.
		/// </summary>
		/// <param name="subFileParameter">
		///            the parameters of the map file for which the index entry is needed. </param>
		/// <param name="blockNumber">
		///            the number of the block in the map file. </param>
		/// <returns> the index entry. </returns>
		/// <exception cref="IOException">
		///             if an I/O error occurs during reading. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long getIndexEntry(org.mapsforge.map.reader.header.SubFileParameter subFileParameter, long blockNumber) throws java.io.IOException
		internal virtual long GetIndexEntry(SubFileParameter subFileParameter, long blockNumber)
		{
			// check if the block number is out of bounds
			if (blockNumber >= subFileParameter.NumberOfBlocks)
			{
				throw new IOException("invalid block number: " + blockNumber);
			}

			// calculate the index block number
			long indexBlockNumber = blockNumber / INDEX_ENTRIES_PER_BLOCK;

			// create the cache entry key for this request
			IndexCacheEntryKey indexCacheEntryKey = new IndexCacheEntryKey(subFileParameter, indexBlockNumber);

			// check for cached index block
			byte[] indexBlock = this.map[indexCacheEntryKey];
			if (indexBlock == null)
			{
				// cache miss, seek to the correct index block in the file and read it
				long indexBlockPosition = subFileParameter.IndexStartAddress + indexBlockNumber * SIZE_OF_INDEX_BLOCK;

				int remainingIndexSize = (int)(subFileParameter.IndexEndAddress - indexBlockPosition);
				int indexBlockSize = Math.Min(SIZE_OF_INDEX_BLOCK, remainingIndexSize);
				indexBlock = new byte[indexBlockSize];

				this.randomAccessFile.Seek(indexBlockPosition, SeekOrigin.Begin);
				if (this.randomAccessFile.Read(indexBlock, 0, indexBlockSize) != indexBlockSize)
				{
					throw new IOException("could not read index block with size: " + indexBlockSize);
				}

				// put the index block in the map
				this.map[indexCacheEntryKey] = indexBlock;
			}

			// calculate the address of the index entry inside the index block
			long indexEntryInBlock = blockNumber % INDEX_ENTRIES_PER_BLOCK;
			int addressInIndexBlock = (int)(indexEntryInBlock * SubFileParameter.BYTES_PER_INDEX_ENTRY);

			// return the real index entry
			return Deserializer.GetFiveBytesLong(indexBlock, addressInIndexBlock);
		}
	}
}