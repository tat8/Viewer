using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Viewer.Services
{
    public static class ExportService
    {
        private static SpreadsheetDocument _spreadsheetDocument;
        private static WorkbookPart _workbookPart;
        private static WorksheetPart _worksheetPart;
        private static Sheets _sheets;
        private static Sheet _sheet;
        private static SharedStringTablePart _shareStringPart;

        private static List<string> _letters = new List<string>
        {
            "A",
            "B",
            "C",
            "D",
            "E"
        };

        private static uint _recordsCount = 0;

        private static void FillCells(ObservableCollection<Models.Node> nodes, int level = 0)
        {
            var i = 0;
            while (i < nodes.Count)
            {
                InsertText(nodes[i].Name, _letters[level], _recordsCount + 1);
                _recordsCount++;
                if (level < 3)
                {
                    FillCells(nodes[i].Nodes, level + 1);
                }
                i++;
            }
        }

        public static void Export(string filepath, ObservableCollection<Models.Node> nodes)
        {
            CreateSpreadsheetWorkbook(filepath);
            FillCells(nodes);
            _recordsCount = 0;
            CloseSpreadsheetWorkbook();
        }

        private static void CreateSpreadsheetWorkbook(string filepath)
        {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            _spreadsheetDocument = SpreadsheetDocument.
                Create(filepath, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            _workbookPart = _spreadsheetDocument.AddWorkbookPart();
            _workbookPart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            _worksheetPart = _workbookPart.AddNewPart<WorksheetPart>();
            _worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            _sheets = _spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            _sheet = new Sheet()
            {
                Id = _spreadsheetDocument.WorkbookPart.
                    GetIdOfPart(_worksheetPart),
                SheetId = 1,
                Name = "Protocols"
            };
            _sheets.Append(_sheet);

            if (_spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Any())
            {
                _shareStringPart = _spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                _shareStringPart = _spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
            }
            
        }

        private static void InsertText(string text, string columnName, uint rowIndex)
        {
            // Insert the text into the SharedStringTablePart.
            int index = InsertSharedStringItem(text, _shareStringPart);

            // Insert cell into the new worksheet.
            Cell cell = InsertCellInWorksheet(columnName, rowIndex);

            // Set the value of cell
            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        }

        private static void CloseSpreadsheetWorkbook()
        {
            // Save the new worksheet.
            _worksheetPart.Worksheet.Save();
            _shareStringPart.SharedStringTable.Save();
            _worksheetPart.Worksheet.Save();
            _workbookPart.Workbook.Save();
            _spreadsheetDocument.Close();
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            text = text.Replace("\0", "");

            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));

            return i;
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex)
        {
            Worksheet worksheet = _worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                return newCell;
            }
        }

    }
}
