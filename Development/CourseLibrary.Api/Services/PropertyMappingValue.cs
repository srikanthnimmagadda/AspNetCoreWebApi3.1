using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Services
{
    public class PropertyMappingValue
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> DestinationProperties { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasRevert { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationProperties"></param>
        /// <param name="hasRevert"></param>
        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool hasRevert = false)
        {
            DestinationProperties = destinationProperties
                ?? throw new ArgumentNullException(nameof(destinationProperties));
            HasRevert = hasRevert;
        }
    }
}
