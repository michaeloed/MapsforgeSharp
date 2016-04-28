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

namespace org.mapsforge.map.rendertheme.rule
{
    using System.Collections.Generic;

    using Tag = org.mapsforge.core.model.Tag;

	internal sealed class AnyMatcher : ElementMatcher, AttributeMatcher, ClosedMatcher
	{
		internal static readonly AnyMatcher INSTANCE = new AnyMatcher();

		private AnyMatcher()
		{
			// do nothing
		}

		public bool IsCoveredBy(AttributeMatcher attributeMatcher)
		{
			return attributeMatcher == this;
		}

		public bool IsCoveredBy(ClosedMatcher closedMatcher)
		{
			return closedMatcher == this;
		}

		public bool IsCoveredBy(ElementMatcher elementMatcher)
		{
			return elementMatcher == this;
		}

		public bool Matches(Closed closed)
		{
			return true;
		}

		public bool Matches(Element element)
		{
			return true;
		}

		public bool Matches(IList<Tag> tags)
		{
			return true;
		}
	}
}