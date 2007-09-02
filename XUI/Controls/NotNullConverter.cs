using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Returns True if the value is not null
	/// </summary>
	public class NotNullConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value != null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
