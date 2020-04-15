using CourseLibrary.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Services
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappingDictionary"></param>
        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary ?? 
                throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}
