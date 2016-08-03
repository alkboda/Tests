using System;

namespace TextProcessor.Processing.Attributes
{
    public interface IProcessedFieldAttribute
    {
        String FieldName { get; set; }
    }
}
