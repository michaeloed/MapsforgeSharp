﻿//----------------------------------------------------------------------------------------
//	Copyright © 2007 - 2016 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class includes methods to convert Java rectangular arrays (jagged arrays
//	with inner arrays of the same length).
//----------------------------------------------------------------------------------------
internal static class RectangularArrays
{
    internal static sbyte[][] ReturnRectangularSbyteArray(int size1, int size2)
    {
        sbyte[][] newArray = new sbyte[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new sbyte[size2];
        }

        return newArray;
    }
}