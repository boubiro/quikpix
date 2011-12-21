using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuikPix.Core.Catalog;

namespace QuikPix.Core.TransferObjects
{
    public class MiniTitleDisplayItem
    {
        public MiniTitleDisplayItem(Title title)
        {
            if (title == null)
                throw new ArgumentNullException("title");

            this.BoxArt = title.BoxArtSmall;
            this.Title = title.RegularTitle;
        }

        public string BoxArt { get; private set; }
        public string Title { get; private set; }
    }
}
