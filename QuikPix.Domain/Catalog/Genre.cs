using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace QuikPix.Core.Catalog
{
    public class Genre
    {
        protected Genre() { }
        public Genre(string genreId, string label)
        {
            this.GenreId = genreId;
            this.Label = label;

            this.ParentGenres = new List<Genre>();
            this.ChildGenres = new List<Genre>();
            this.Titles = new List<Title>();
        }

        public virtual int PrimaryKey { get; protected set; }

        public virtual string GenreId { get; protected set; }
        public virtual string Label { get; set; }
        public virtual bool RootGenre { get; set; }

        public virtual IList<Genre> ParentGenres { get; protected set; }
        public virtual IList<Genre> ChildGenres { get; protected set; }
        public virtual IList<Title> Titles { get; protected set; }

    }

    public class GenreMap : ClassMap<Genre>
    {
        public GenreMap()
        {
            Cache.ReadOnly();

            //Id(x => x.PrimaryKey).GeneratedBy.Identity();
            Id(x => x.GenreId).GeneratedBy.Assigned();
            Map(x => x.Label);
            Map(x => x.RootGenre);


            HasManyToMany<Genre>(x => x.ParentGenres)
                .LazyLoad()
                .Cascade.All()
                .ParentKeyColumn("GenreId")
                .ChildKeyColumn("ParentGenreId")
                .Table("GenreGenre");

            HasManyToMany<Genre>(x => x.ChildGenres)
                .LazyLoad()
                .Cascade.None()
                .ParentKeyColumn("ParentGenreId")
                .ChildKeyColumn("GenreId")
                .Table("GenreGenre");

            HasManyToMany<Title>(x => x.Titles)
                .LazyLoad()
                .ChildOrderBy("AverageRating DESC")
                .Cascade.None()
                .Inverse()
                .ParentKeyColumn("GenreId")
                .ChildKeyColumn("TitleId")
                .Table("TitleGenre");

            //References<Genre>(x => x.ParentGenre, "ParentGenreId")
            //    .ForeignKey("GenreId")
            //    .Nullable()
            //    .Index("idx_parentGenreId");
            //HasMany<Genre>(x => x.ChildGenres)
            //    .KeyColumn("ParentGenreId")
            //    .Cascade.All();
            //HasManyToMany<Title>(x => x.Titles)
            //    .Cascade.None()
            //    .Inverse()
            //    .Table("TitleGenre")
            //    .ParentKeyColumn("TitleId")
            //    .ChildKeyColumn("GenreId");
        }

    }

}
