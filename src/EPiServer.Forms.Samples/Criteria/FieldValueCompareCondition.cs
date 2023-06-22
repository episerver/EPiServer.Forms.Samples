using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Forms.Samples.Criteria
{
    /// <summary>
    /// Enum representing compare conditions for booleans
    /// </summary>
    public enum FieldValueCompareCondition
    {
        /// <summary>
        /// Equality comparison
        /// </summary>
        Equals,
        /// <summary>
        /// Not equals comparison
        /// </summary>
        NotEquals,
        /// <summary>
        /// Contains comparison
        /// </summary>
        Contains,
        /// <summary>
        /// Not contains comparison
        /// </summary>
        NotContains
    }
}
