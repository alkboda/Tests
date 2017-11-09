using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TextProcessor.Data;
using TextProcessor.Processing.Attributes;

namespace TextProcessor.Processing.Processors
{
    public class CsvProcessor : IProcessor
    {
        public IEnumerable<TEntity> Process<TEntity>(string fileName) where TEntity : class
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            if (!typeof(ACsv).IsAssignableFrom(typeof(TEntity)))
            {
                throw new ArgumentException($"{nameof(CsvProcessor)} accepts only {nameof(ACsv)} types", nameof(TEntity));
            }

            if (!File.Exists(fileName))
            {
                yield break;
            }

            var strings = File.ReadAllLines(fileName);
            foreach (var csvString in strings)
            {
                yield return ParseString<TEntity>(csvString);
            }
        }

        public TEntity ParseString<TEntity>(string csvString) where TEntity : class
        {
            if (String.IsNullOrWhiteSpace(csvString))
            {
                throw new ArgumentNullException(nameof(csvString));
            }

            ACsv result = Activator.CreateInstance<TEntity>() as ACsv;
            var parts = csvString.Split(new[] { result.FieldDelimeter }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != result.FieldCount)
            {
                throw new ArgumentException($"Invalid fields count within {typeof(TEntity).Name} declaration.");
            }

            var properties = typeof(TEntity).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(_ => _.GetCustomAttributes<CsvFieldAttribute>(true)?.Any() ?? false);
            if (!properties.Any())
            {
                throw new Exception($"Type {typeof(TEntity).Name} doesn't contain any {nameof(CsvFieldAttribute)} attributes");
            }

            var attributesData = new Dictionary<CsvFieldAttribute, PropertyInfo>();
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes<CsvFieldAttribute>(true).First();
                if (String.IsNullOrWhiteSpace(attribute.FieldName))
                {
                    attribute.FieldName = property.Name;
                }
                attributesData.Add(attribute, property);
            }

            for (int i = 0; i < parts.Length; i++)
            {
                var attribute = attributesData.Keys.FirstOrDefault(_ => _.FieldIndex == i);
                if (attribute == null)
                {
                    continue;
                }

                var part = parts[i];
                if (attribute.NotEmpty && String.IsNullOrWhiteSpace(part))
                {
                    throw new InvalidDataException($"Field {attribute.FieldName} have to be filled.");
                }
                
                object val = part;
                var property = attributesData[attribute];
                if (property.PropertyType != typeof(String))
                {
                    switch (property.PropertyType.Name)
                    {
                        case nameof(Int16):
                            val = Int16.Parse(part);
                            break;
                        case nameof(Int32):
                            val = Int32.Parse(part);
                            break;
                        case nameof(Int64):
                            val = Int64.Parse(part);
                            break;
                        case nameof(UInt16):
                            val = UInt16.Parse(part);
                            break;
                        case nameof(UInt32):
                            val = UInt32.Parse(part);
                            break;
                        case nameof(UInt64):
                            val = UInt64.Parse(part);
                            break;
                        
                        default:
                            throw new InvalidCastException($"Can't cast to type {property.PropertyType}");
                    }
                }
                
                property.SetValue(result, val);
            }

            return result as TEntity;
        }
    }
}
