namespace SimpleBlog.Models
{
    public class Blog
    {
        public dynamic Metadata { get; set; }

        public string Title
        {
            get { return Metadata.Title.Value; }
            set { Metadata.Title = value; }
        }

        public string TagLine
        {
            get { return Metadata.TagLine; }
            set { Metadata.TagLine = value; }
        }

        public int PageSize
        {
            get
            {
                int pageSize;
                int.TryParse(Metadata.PageSize, out pageSize);
                if (pageSize <= 0)
                {
                    pageSize = 5;
                }
                return pageSize;
            }
            set
            {
                Metadata.PageSize = value;
            }
        }
    }
}