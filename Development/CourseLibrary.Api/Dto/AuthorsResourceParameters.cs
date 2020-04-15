namespace CourseLibrary.Api.Dto
{
    public class AuthorsResourceParameters
    {
        /// <summary>
        /// 
        /// </summary>
        const int maxPageSize = 20;

        /// <summary>
        /// 
        /// </summary>
        private int _pageSize = 10;

        /// <summary>
        /// 
        /// </summary>
        public string MainCategory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SearchQuery { get; set; }

        /// <summary>
        /// Default set to 1
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string OrderBy { get; set; } = "Name";

        /// <summary>
        /// 
        /// </summary>
        public string Fields { get; set; }
    }
}
