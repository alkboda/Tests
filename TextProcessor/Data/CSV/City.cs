using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextProcessor.Processing.Attributes;

namespace TextProcessor.Data.CSV
{
    public class City : ACsv
    {
        #region CSV Settings
        /// <summary>
        /// Delimiter of fields within *.csv file
        /// </summary>
        public override String FieldDelimeter { get { return "|"; } }

        /// <summary>
        /// Prefix of interstate number string
        /// </summary>
        public virtual String InterstatePrefix { get { return "I-"; } }

        /// <summary>
        /// What string delimits interstates
        /// </summary>
        protected virtual String InterstateDelimiter { get { return ";"; } }
        #endregion CSV Settings

        #region Data
        [CsvField(FieldIndex = 0)]
        /// <summary>
        /// Population, in hundreds of thousands
        /// </summary>
        public Int32 Population { get; set; }

        [CsvField(FieldIndex = 1)]
        /// <summary>
        /// City name
        /// </summary>
        public String Name { get; set; }

        [CsvField(FieldIndex = 2)]
        /// <summary>
        /// State name
        /// </summary>
        public String State { get; set; }

        [CsvField(FieldIndex = 3)]
        /// <summary>
        /// Interstate field value
        /// </summary>
        protected String Interstate { get; set; }

        /// <summary>
        /// Interstates numbers that run through this city
        /// </summary>
        public IEnumerable<UInt16> InterstateNumbers
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Interstate))
                {
                    return new UInt16[0];
                }

                var parts = Interstate.Split(new[] { InterstateDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                var prefixRegex = new System.Text.RegularExpressions.Regex($@"^{InterstatePrefix}");
                return parts.Select(_ => prefixRegex.Replace(_, String.Empty)).Select(_ => UInt16.Parse(_));
            }
        }
        #endregion Data
    }
}
