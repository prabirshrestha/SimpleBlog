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
            get { return Metadata.Draft; }
            set { Metadata.Draft = value; }
        }

        public DateTime Date
        {
            get
            {
                DateTime dateTime;
                if (!DateTime.TryParseExact(Metadata.Date, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
                {
                    return DateTime.MaxValue;
                }
                return dateTime;
            }
            set { Metadata.Date = value.ToString(DateFormats[0]); }
        }

        public bool IsHidden(DateTime currentTime)
        {
            return Draft || Date >= currentTime;
        }

        public string RawContent
        {
            get { return Metadata.RawContent; }
            set { Metadata.RawContent = value; }
        }

        public string HtmlContent { get; set; }
    }
}