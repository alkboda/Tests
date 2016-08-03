using System;

namespace TextProcessor.Processing.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvFieldAttribute : Attribute, IProcessedFieldAttribute
    {
        public String FieldName { get; set; }
        public UInt16 FieldIndex { get; set; }
        public Boolean NotEmpty { get; set; } = true;
    }
}
