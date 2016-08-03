using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TextProcessor.Processing.Attributes;

namespace TextProcessor.Data
{
    public abstract class ACsv
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        /// <summary>
        /// Delimiter of fields within *.csv file
        /// </summary>
        public abstract String FieldDelimeter { get; }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        /// <summary>
        /// Count of fields within one CSV string
        /// </summary>
        /// <remarks>
        /// By default count equals to public properties number within inherited class
        /// </remarks>
        public virtual Int32 FieldCount
        {
            get
            {
                return GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                            .Where
                            (
                                // Take only properties with appropriate attribute
                                //
                                _ => _.GetCustomAttributes(true).Count(a => typeof(IProcessedFieldAttribute).IsAssignableFrom(a.GetType())) == 1
                            ).Count();
            }
        }
    }
}
