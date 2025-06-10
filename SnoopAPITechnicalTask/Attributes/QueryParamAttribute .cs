using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnoopAPITechnicalTask.Attributes
{
    /// <summary>
    /// Specifies the name of the query parameter to be used when serializing
    /// a property to a query string. Allows customization of the query parameter
    /// name different from the property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryParamAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the query parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParamAttribute"/> class
        /// with the specified query parameter name.
        /// </summary>
        /// <param name="name">The name to use for the query parameter.</param>
        public QueryParamAttribute(string name)
        {
            Name = name;
        }
    }
}
