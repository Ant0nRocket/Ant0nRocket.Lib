using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ant0nRocket.Lib.Infrastructure
{
    /// <summary>
    /// My implementation of sequential ID generator.
    /// Usefull in databases, when unique ID generation required
    /// without information about other machines generated IDs.
    /// </summary>
    public static class IdGenerator
    {
        /// <summary>
        /// Generates new sequential ID
        /// </summary>
        public static long New()
        {

            return long.MaxValue;
        }
    }
}
