using System;
using System.Linq;
using System.Xml.Linq;
using NetflixWrapper.Models.Catalog;

namespace NetflixCatalogParser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var doc = XDocument.Load("full_catalog.xml");

            var titles = doc.Descendants("catalog_title").Select(el => CreateCatalogTitle(el));

            foreach(var title in titles) {
                Console.WriteLine(title.RegularTitle);
            }

            Console.ReadLine();
        }

        private static Title CreateCatalogTitle(XElement titleEl) {
            var title = new Title(titleEl.Element("id").Value, titleEl.Element("title").Attribute("regular").Value);
            
            title.ShortTitle = titleEl.Element("title").Attribute("short").Value;
            if (titleEl.Element("release_year") != null && titleEl.Element("release_year").Value != "")
                title.ReleaseYear = Convert.ToInt32(titleEl.Element("release_year").Value);
            if (titleEl.Element("average_rating") != null)
                title.AverageRating = Convert.ToDecimal(titleEl.Element("average_rating").Value);

            XElement childEl = null;

            if (TryGetLinkElement(titleEl, "synopsis", out childEl))
                title.Synopsis = childEl.Element("synopsis").Value;

            if (TryGetLinkElement(titleEl, "short_synopsis", out childEl))
                title.ShortSynopsis = childEl.Element("short_synopsis").Value;

            if (TryGetLinkElement(titleEl, "format_availability", out childEl) && childEl.Element("delivery_formats").HasElements)
            {
                var availability = childEl.Element("delivery_formats").Element("availability");

                title.AvailableFrom = ConvertFromUnixTimestamp(Convert.ToDouble(availability.Attribute("available_from").Value));
                title.AvailableUntil = ConvertFromUnixTimestamp(Convert.ToDouble(availability.Attribute("available_until").Value));
                
                if (availability.Element("runtime") != null)
                    title.RunTime = TimeSpan.FromSeconds(Convert.ToInt32(availability.Element("runtime").Value));

                XElement instantEl = null;

                if (TryGetCategoryElement(availability, "http://api.netflix.com/categories/title_formats", "instant", out instantEl)) {
                    XElement ratingEl = null;
                    if (TryGetCategoryElement(instantEl, "http://api.netflix.com/categories/ca_movie_ratings", out ratingEl))
                        title.Rating = ratingEl.Attribute("label").Value;

                    XElement qualityEl = null;
                    if (TryGetCategoryElement(instantEl, "http://api.netflix.com/categories/title_formats/quality", out qualityEl))
                        title.Quality = qualityEl.Attribute("label").Value;
                }
            }

            var categories = titleEl.Elements("category");
            foreach (var category in categories)
            {
                if (!category.Attribute("scheme").Value.Equals("http://api.netflix.com/categories/maturity_level", StringComparison.OrdinalIgnoreCase))
                    title.Genres.Add(new Genre() { 
                        TitleID = title.TitleId, 
                        GenreID = category.Attribute("scheme").Value, 
                        Label = category.Attribute("label").Value, 
                        Term = category.Attribute("term").Value });
            }

            return title;
        }

        private static bool TryGetLinkElement(XElement parent, string hrefType, out XElement el) {
            el = parent.Elements("link").FirstOrDefault(x => x.Attribute("href").Value.EndsWith(hrefType, StringComparison.OrdinalIgnoreCase));
            return el != null;
        }

        private static bool TryGetCategoryElement(XElement parent, string scheme, out XElement el) {
            el = parent.Elements("category").FirstOrDefault(x => x.Attribute("scheme").Value == scheme);
            return el != null;
        }

        private static bool TryGetCategoryElement(XElement parent, string scheme, string label, out XElement el)
        {
            el = parent.Elements("category")
                .Where(x =>
                    x.Attribute("scheme").Value.Equals(scheme, StringComparison.OrdinalIgnoreCase) &&
                    x.Attribute("label").Value.Equals(label, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
               
            return el != null;
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

    }


}
