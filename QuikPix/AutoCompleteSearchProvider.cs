using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dragonz.actb.provider;
using QuikPix.Core;

namespace QuikPix
{
    public class AutoCompleteSearchProvider : IAutoCompleteDataProvider
    {
        public IEnumerable<string> GetItems(string textPattern)
        {
            if (textPattern.Length > 2)
                return QuikPixCore.Current.Autocomplete(textPattern);
            else
                return new String[] { };
        }
    }
}
