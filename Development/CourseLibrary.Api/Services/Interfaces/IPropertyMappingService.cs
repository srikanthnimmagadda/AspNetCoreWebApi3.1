using System.Collections.Generic;

namespace CourseLibrary.Api.Services.Interfaces
{
    public interface IPropertyMappingService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        bool HasValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}