﻿using DocBuilder.Services.Utility.Attributes;
using System.Data;
using System.Reflection;

namespace DocBuilder.Services.Utility
{
    public class DocMapper : IDocMapper
    {
        public async  Task<List<T>> Map<T>(IEnumerable<DataRow> rows) where T : class
        {
            var instances = new List<T>();

            await Task.Run(() =>
            {
                var values = DocExtractor.GetAttributeProperty<DocAttribute, T>();
                var properties = typeof(T).GetProperties();
                int left = 0;
                var currentRows = rows.ToList();
                while (left < rows.Count())
                {
                    var instance = (T)Activator.CreateInstance(typeof(T)) ?? throw new Exception("Unable to create instance");
                    ExtractProperties<T>(instance, properties, values, currentRows, left);
                    left++;
                    instances.Add(instance);
                }
            });

            return instances;
        }

        private void ExtractProperties<T>(T instance,PropertyInfo[] properties,
            List<DocProperty> attributeValues, List<DataRow> currentRows, 
            int rowPosition ,int left = 0)
        {
            if(left < properties.Length) 
            {
                if (attributeValues.Where(s=>s.PropertyName == properties[left].Name).Select(s=>s.AttributeName).FirstOrDefault() is object value)
                {
                    var propertyName = value;

                    object propertyValue;

                    var fieldValue = ExtractValue(currentRows, propertyName, rowPosition);

                    propertyValue = Convert.ChangeType(fieldValue, properties[left].PropertyType);

                    if (propertyValue != null)
                        properties[left].SetValue(instance, propertyValue);
                }
                ExtractProperties(instance, properties, attributeValues, currentRows ,rowPosition, ++left);
            }
        }

        /// <summary>
        /// Extract values based on the field name or field position
        /// </summary>
        /// <typeparam name="TDataType">The data type of the property to map</typeparam>
        /// <param name="rows">Includes the extracted values</param>
        /// <param name="proposedName">The name of the column from the attribute</param>
        /// <param name="left">used for iteration</param>
        /// <returns>a string value/returns>
        private string ExtractValue(List<DataRow> rows, object proposedName, int left)
        {
            if (proposedName.GetType().Name == typeof(string).Name)
            {
                return rows[left].Field<string>((string)proposedName);
            }
            else
            {
                return rows[left].Field<string>((int)proposedName);
            }
        }
    }
}
