using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    /// <summary>
    /// A helper class for building tree assertion failure messages in a consistent format.
    /// </summary>
    public class TreeFailureMessage : CollectionFailureMessage
    {
        /// <summary>
        /// The path of nodes from the root to the failing node.
        /// </summary>
        public ICollection<object> FailurePathValues { get; set; }

        /// <summary>
        /// Outputs the path of nodes from the root to the failing node.
        /// </summary>
        public string FailurePath
        {
            get
            {
                return new[] { "root" }
                    .Concat(
                        (FailurePathValues ?? Enumerable.Empty<object>()))
                            .Select(v => v.ToString())
                    .Join(" -> ");
            }
        }
    }
}