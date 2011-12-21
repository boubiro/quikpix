using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuikPix.Core.TransferObjects
{
    public interface IDisplayItem
    {
        string Title { get; }
        string BoxArt { get; }
    }
}
