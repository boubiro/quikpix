using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuikPix.Core.Catalog;

namespace QuikPix.Core.TransferObjects
{
    public class TitleDisplayItem : IDisplayItem
    {
        public TitleDisplayItem(Title title) {
            if (title == null)
                throw new ArgumentNullException("title");

            this.TitleId = title.TitleId;
            this.Title = "";
            this.RegularTitle = title.ShortTitle;
            this.BoxArt = title.BoxArtLarge;
            this.Synopsis = (string.IsNullOrWhiteSpace(title.Synopsis) ? (string.IsNullOrWhiteSpace(title.ShortSynopsis) ? "No Description" : title.ShortSynopsis) : title.Synopsis);
            this.Quality = (string.IsNullOrEmpty(title.Quality) ? "SD" : title.Quality);
            this.Rating = title.Rating;
            this.RunTime = new DateTime(title.RunTime.Ticks).ToString("H:mm");
            this.ReleaseYear = title.ReleaseYear.ToString();
            this.AverageRating = title.AverageRating.ToString("0.0");
        }

        public string TitleId { get; private set; }
        public string Title { get; private set; }
        public string RegularTitle { get; private set; }
        public string BoxArt { get; private set; }
        public string Synopsis { get; private set; }
        public string Quality { get; private set; }
        public string Rating { get; private set; }
        public string AverageRating { get; private set; }
        public string RunTime { get; private set; }
        public string ReleaseYear { get; private set; }

    }
}
