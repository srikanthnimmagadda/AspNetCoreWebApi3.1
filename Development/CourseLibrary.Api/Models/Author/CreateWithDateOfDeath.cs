using System;

namespace CourseLibrary.Api.Models.Author
{
    public class CreateWithDateOfDeath : Create
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset? DateOfDeath { get; set; }
    }
}
