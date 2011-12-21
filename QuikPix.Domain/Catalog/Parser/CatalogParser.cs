using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using NHibernate;

namespace QuikPix.Core.Catalog.Parser
{
    public class CatalogParser
    {
        private readonly ISession session;
        private readonly IList<Genre> genres;

        public CatalogParser(ISession session)
        {
            this.session = session;
            this.genres = session.CreateCriteria<Genre>().List<Genre>();
        }

        public IEnumerable<Title> ParseCatalog(string xmlFile)
        {
            if (string.IsNullOrWhiteSpace(xmlFile))
                throw new ArgumentException("xmlFile");
            if (!File.Exists(xmlFile))
                throw new FileNotFoundException("File not found: " + xmlFile);

            var xdoc = XDocument.Load(xmlFile);

            return xdoc.Descendants("catalog_title").Select(x => ParseCatalogTitle(x));
        }


        private Title ParseCatalogTitle(XElement titleEl)
        {
            var title = new Title(titleEl.Element("id").Value, titleEl.Element("title").Attribute("regular").Value);

            title.ShortTitle = titleEl.Element("title").Attribute("short").Value;
            if (titleEl.Element("release_year") != null && titleEl.Element("release_year").Value != "")
                title.ReleaseYear = Convert.ToInt32(titleEl.Element("release_year").Value);
            if (titleEl.Element("average_rating") != null)
                title.AverageRating = Convert.ToDecimal(titleEl.Element("average_rating").Value);

            XElement childEl = null;

            if (TryGetLinkElement(titleEl, "http://schemas.netflix.com/catalog/titles/synopsis", out childEl))
                title.Synopsis = childEl.Element("synopsis").Value;

            if (TryGetLinkElement(titleEl, "http://schemas.netflix.com/catalog/titles/synopsis.short", out childEl))
                title.ShortSynopsis = childEl.Element("short_synopsis").Value;

            if (TryGetLinkElement(titleEl, "http://schemas.netflix.com/catalog/titles/format_availability", out childEl) && childEl.Element("delivery_formats").HasElements)
            {
                var availability = childEl.Element("delivery_formats").Element("availability");

                title.AvailableFrom = ConvertFromUnixTimestamp(Convert.ToDouble(availability.Attribute("available_from").Value));
                title.AvailableUntil = ConvertFromUnixTimestamp(Convert.ToDouble(availability.Attribute("available_until").Value));

                if (availability.Element("runtime") != null)
                    title.RunTime = TimeSpan.FromSeconds(Convert.ToInt32(availability.Element("runtime").Value));

                XElement instantEl = null;

                if (TryGetCategoryElement(availability, "http://api.netflix.com/categories/title_formats", "instant", out instantEl))
                {
                    XElement ratingEl = null;
                    if (TryGetCategoryElement(instantEl, "http://api.netflix.com/categories/ca_movie_ratings", out ratingEl))
                        title.Rating = ratingEl.Attribute("label").Value;

                    XElement qualityEl = null;
                    if (TryGetCategoryElement(instantEl, "http://api.netflix.com/categories/title_formats/quality", out qualityEl))
                        title.Quality = qualityEl.Attribute("label").Value;
                }
            }

            // Box Art
            
            if (TryGetLinkElement(titleEl, "http://schemas.netflix.com/catalog/titles/box_art", out childEl) && childEl.Element("box_art") != null) {
                XElement boxArtEl = null;
                if (TryGetLinkElement(childEl.Element("box_art"), "http://schemas.netflix.com/catalog/titles/box_art/284pix_w", out boxArtEl)) {
                    title.BoxArtLarge = boxArtEl.Attribute("href").Value;
                } else if (TryGetLinkElement(childEl.Element("box_art"), "http://schemas.netflix.com/catalog/titles/box_art/166pix_w", out boxArtEl)) {
                    title.BoxArtLarge = boxArtEl.Attribute("href").Value;
                }

                if (TryGetLinkElement(childEl.Element("box_art"), "http://schemas.netflix.com/catalog/titles/box_art/64pix_w", out boxArtEl)) {
                    title.BoxArtSmall = boxArtEl.Attribute("href").Value;
                }
            }
            
            //
            //



            var categories = titleEl.Elements("category");
            foreach (var category in categories)
            {
                if (!category.Attribute("scheme").Value.Equals("http://api.netflix.com/categories/maturity_level", StringComparison.OrdinalIgnoreCase))
                {
                    var genre = genres.FirstOrDefault(x => x.GenreId.Equals(category.Attribute("scheme").Value, StringComparison.OrdinalIgnoreCase));
                    if (genre != null)
                    {
                        title.Genres.Add(genre);

                        if (!genre.Label.Equals(category.Attribute("label").Value))
                        {
                            genre.Label = category.Attribute("label").Value;
                            session.SaveOrUpdate(genre);
                        }
                    }
                }
            }

            return title;
        }

        private bool TryGetLinkElement(XElement parent, string hrefType, out XElement el)
        {
            el = null;
            el = parent.Elements("link")
                .FirstOrDefault(x => x.Attribute("rel").Value.EndsWith(hrefType, StringComparison.OrdinalIgnoreCase));
            return el != null;
        }

        private bool TryGetCategoryElement(XElement parent, string scheme, out XElement el)
        {
            el = null;
            el = parent.Elements("category")
                .FirstOrDefault(x => x.Attribute("scheme").Value.Equals(scheme, StringComparison.OrdinalIgnoreCase));
            return el != null;
        }

        private bool TryGetCategoryElement(XElement parent, string scheme, string label, out XElement el)
        {
            el = null;
            el = parent.Elements("category")
                .Where(x =>
                    x.Attribute("scheme").Value.Equals(scheme, StringComparison.OrdinalIgnoreCase) &&
                    x.Attribute("label").Value.Equals(label, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            return el != null;
        }

        private DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
