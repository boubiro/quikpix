using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace QuikPix.Core.Catalog
{
    public class Title
    {
        protected Title() { }

        public Title(string titleId, string regularTitle)
        {
            this.TitleId = titleId;
            this.RegularTitle = regularTitle;
            this.Genres = new List<Genre>();
        }

        public virtual int PrimaryKey { get; protected set; }
        public virtual string TitleId { get; protected set; }
        public virtual string RegularTitle { get; protected set; }
        public virtual string ShortTitle { get; set; }
        public virtual string Synopsis { get; set; }
        public virtual string ShortSynopsis { get; set; }
        public virtual int ReleaseYear { get; set; }
        public virtual DateTime AvailableFrom { get; set; }
        public virtual DateTime AvailableUntil { get; set; }
        public virtual string Rating { get; set; }
        public virtual string Quality { get; set; }
        public virtual TimeSpan RunTime { get; set; }
        public virtual decimal AverageRating { get; set; }
        public virtual string BoxArtLarge { get; set; }
        public virtual string BoxArtSmall { get; set; }

        public virtual IList<Genre> Genres { get; protected set; }

        public virtual string GetIdPart()
        {
            if (!string.IsNullOrWhiteSpace(TitleId))
            {
                return TitleId.Substring(TitleId.LastIndexOf('/'));
            }
            return "";
        }
    }

    public class TitleMap : ClassMap<Title>
    {
        public TitleMap()
        {
            Cache.ReadOnly();

            Id(x => x.TitleId).GeneratedBy.Assigned();
            Map(x => x.RegularTitle);
            Map(x => x.ShortTitle);
            Map(x => x.Synopsis);
            Map(x => x.ShortSynopsis);
            Map(x => x.ReleaseYear);
            Map(x => x.AvailableFrom);
            Map(x => x.AvailableUntil);
            Map(x => x.Rating);
            Map(x => x.Quality);
            Map(x => x.RunTime);
            Map(x => x.AverageRating);

            Map(x => x.BoxArtLarge);
            Map(x => x.BoxArtSmall);

            HasManyToMany<Genre>(x => x.Genres)
                .LazyLoad()
                .Cascade.All()
                .ParentKeyColumn("TitleId")
                .ChildKeyColumn("GenreId")
                //.PropertyRef("TitleId")
                //.ChildPropertyRef("GenreId")
                .Table("TitleGenre");

            //HasManyToMany<Genre>(x => x.Genres)
            //    .Cascade.All()
            //    .Table("TitleGenre")
            //    .ParentKeyColumn("TitleId")
            //    .ChildKeyColumn("GenreId");

        }
    }
}
