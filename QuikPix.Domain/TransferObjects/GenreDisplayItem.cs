using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuikPix.Core.Catalog;

namespace QuikPix.Core.TransferObjects
{
    public class GenreDisplayItem : IDisplayItem
    {
        public GenreDisplayItem(Genre genre)
        {
            if (genre == null)
                throw new ArgumentException("Genre is required.", "genre");
            
            this.GenreId = genre.GenreId;
            this.Title = genre.Label;

            var topTitles = genre.Titles.Take(20).ToList();
            var topTitle = topTitles[new Random((int)DateTime.Now.Ticks).Next(0, (topTitles.Count()> 5 ? 5 : topTitles.Count()))];

            this.BoxArt = topTitle.BoxArtLarge;
            this.TitleId = topTitle.TitleId;

            this.MiniTitles = topTitles.Select(t => new MiniTitleDisplayItem(t));
        }

        public string GenreId { get; private set; }

        public string Title { get; private set; }
        public string BoxArt { get; private set; }

        public string TitleId { get; private set; }

        public IEnumerable<MiniTitleDisplayItem> MiniTitles { get; private set; }
    }
}
