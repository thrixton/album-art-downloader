using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace AlbumArtDownloader.Controls
{
	internal static class Utilities
	{
		public static IEnumerable<T> CastEnumerable<T>(IEnumerable enumerable)
			where T: class
		{
			foreach (object item in enumerable)
			{
				yield return item as T;
			}
		}

		public static Typeface GetTypeface(TextBlock textBlock)
		{
			return new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
		}
	}
}
