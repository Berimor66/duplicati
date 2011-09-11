using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Duplicati.GUI.HelperControls {
	
	public class FileSizeFormatter {

		private static readonly Decimal OneKiloByte = 1024M;
		private static readonly Decimal OneMegaByte = OneKiloByte * 1024M;
		private static readonly Decimal OneGigaByte = OneMegaByte * 1024M;

		public static string Format(long fileSize, int precision) {
			Decimal size;

			size = Convert.ToDecimal(fileSize);

			string suffix;
			if( size > OneGigaByte ) {
				size /= OneGigaByte;
				suffix = " GB";
			} else if( size > OneMegaByte ) {
				size /= OneMegaByte;
				suffix = " MB";
			} else if( size > OneKiloByte ) {
				size /= OneKiloByte;
				suffix = " KB";
			} else {
				suffix = " B";
			}

			return String.Format("{0:N" + precision + "}{1}", size, suffix);
		}
		public static string Format(long fileSize) {
			return Format(fileSize, 2);
		}


	}
}
