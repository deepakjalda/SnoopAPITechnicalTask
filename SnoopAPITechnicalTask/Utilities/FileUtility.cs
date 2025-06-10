using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnoopAPITechnicalTask.Utilities
{
    /// <summary>
    /// Utility class for file path related helper methods.
    /// </summary>
    public class FileUtility
    {
        /// <summary>
        /// Gets the absolute path of the project root directory
        /// by navigating up from the application's base directory.
        /// </summary>
        /// <returns>The full path to the project root directory.</returns>
        public static string GetProjectRootPath()
        {
            return Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        }
    }

}
