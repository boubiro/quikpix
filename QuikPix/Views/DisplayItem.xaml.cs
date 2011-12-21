using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QuikPix.Core.TransferObjects;

namespace QuikPix
{
	/// <summary>
	/// Interaction logic for DisplayItem.xaml
	/// </summary>
	public partial class DisplayItem : UserControl
	{
        

		public DisplayItem(IDisplayItem displayItem)
		{
			this.InitializeComponent();

            if (displayItem == null)
                throw new ArgumentNullException("displayItem");

            this.DataContext = displayItem;
		}
	}
}