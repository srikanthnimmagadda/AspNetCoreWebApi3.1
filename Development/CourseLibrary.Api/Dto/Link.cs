namespace CourseLibrary.Api.Dto
{
    public class Link
    {
        /// <summary>
        /// 
        /// </summary>
        public string Href { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Rel { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="href"></param>
        /// <param name="rel"></param>
        /// <param name="method"></param>
        public Link(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
