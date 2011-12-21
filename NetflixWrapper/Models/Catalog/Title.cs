using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetflixWrapper.Models.Catalog
{
    public class Title
    {
        protected Title() { }

        public Title(string titleId, string regularTitle)
        {
            this.TitleId = titleId;
            this.RegularTitle = regularTitle;

            this.BoxArt = new List<BoxArt>();
            this.Genres = new List<Genre>();
        }

        public string TitleId { get; protected set; }
        public string RegularTitle { get; protected set; }
        public string ShortTitle { get; set; }
        public string Synopsis { get; set; }
        public string ShortSynopsis { get; set; }
        public int ReleaseYear { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableUntil { get; set; }
        public string Rating { get; set; }
        public string Quality { get; set; }
        public TimeSpan RunTime { get; set; }
        public decimal AverageRating { get; set; }

        public IList<BoxArt> BoxArt { get; protected set; }
        public IList<Genre> Genres { get; protected set; }
    }
}
