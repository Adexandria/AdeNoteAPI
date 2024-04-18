using DocBuilder.Models;
using DocBuilder.Services.Utility;
using DocBuilder.Services.Utility.Attributes;
using Xceed.Document.NET;
using Xceed.Words.NET;
using DocProperty = DocBuilder.Services.Utility.DocProperty;

namespace DocBuilder.Services
{
    internal class DocService : IDocService
    {
        public Stream ExportToWord<T>(IEntityDoc<T> doc, Stream template) where T : class 
        {
            var extractedAttributes = DocExtractor.GetAttributeProperty<DocRecordAttribute, T>();
            return CreateDocument(doc,extractedAttributes, template);
        }

        private Stream CreateDocument<T>(IEntityDoc<T> doc, List<DocProperty> extractedAttributes, Stream stream) 
        {
            var ms = new MemoryStream();

            using var document = DocX.Load(stream);
       
            var table = CreateTable(document,doc,extractedAttributes);

            document.InsertTable(table);

            document.SaveAs(ms);

            ms.Position = 0;

            return ms;
        }

        private Table CreateTable<T>(DocX document, IEntityDoc<T> doc ,List<Utility.DocProperty> extractedAttributes)
        {
            var table = document.AddTable(doc.Entities.Count + 1, extractedAttributes.Count);
            table.Design = TableDesign.ColorfulList;
            table.Alignment = Alignment.center;
            table.AutoFit = AutoFit.Contents;
            for (int i = 0; i < extractedAttributes.Count; i++)
            {
                if (extractedAttributes[i].AttributeName is int value)
                {
                    table.Rows[0].Cells[value].Paragraphs[0]
                        .Append(extractedAttributes[i].PropertyName).Bold();
                    InsertRows(table, doc.Entities, extractedAttributes[i], value);
                }
                else
                {
                    table.Rows[0].Cells[i].Paragraphs[0]
                       .Append(extractedAttributes[i].AttributeName.ToString()).Bold();
                    InsertRows(table, doc.Entities, extractedAttributes[i], i);
                }
            }

            return table;
        }

        private void InsertRows<T>(Table table, IList<T> entities, DocProperty extractedAttribute, int cellNumber , int rowNumber = 1)
        {
            if(rowNumber <= entities.Count )
            {
                var entity = entities[rowNumber - 1];
                var propertyType = entity.GetType().GetProperty(extractedAttribute.PropertyName);
                var propertyValue = propertyType.GetValue(entity).ToString();

                table.Rows[rowNumber].Cells[cellNumber].Paragraphs[0].Append(propertyValue);

                rowNumber++;

                InsertRows(table, entities, extractedAttribute, cellNumber, rowNumber);
            }
        }
    }
}
