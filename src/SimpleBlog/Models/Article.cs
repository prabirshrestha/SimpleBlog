namespace SimpleBlog.Models
{
    using System;
    using System.Globalization;

    public class Article
    {
        private static readonly string[] DateFormats = new[] {
            "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz"
        };

        public dynamic Metadata { get; set; }

        public string Title
        {
            get { return Metadata.Title; }
            set { Metadata.Value = value; }
        }

        public bool Draft
        {
            get
            {
                if (!Metadata.Draft.HasValue) return true;
                return Metadata.Draft;
            }
            set { Metadata.Draft = value; }
        }

        public DateTime Date
        {
            get
            {
                string dt = Metadata.Date ?? string.Empty;
                if (dt.Contains("("))
                {
                    dt = dt.Substring(0, dt.LastIndexOf(" ", StringComparison.Ordinal));
                }

                DateTime dateTime;
                if (!DateTime.TryParseExact(dt, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
                {
                    return DateTime.MaxValue;
                }

                return dateTime;
            }
            set { Metadata.Date = value.ToString(DateFormats[0]); }
        }

        public string Slug
        {
            get { return Metadata.Slug; }
            set { Metadata.Slug = value; }
        }

        public string RawContent { get; set; }
        public string HtmlContent { get; set; }

        public bool IsHidden(DateTime currentTime)
        {
            return Draft || Date >= currentTime;
        }
    }
}