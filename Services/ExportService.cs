using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Reflection;
using Viewer.Models;


namespace Viewer.Services
{
    public static class ExportService
    {
        // For working with Excel
        private static object _excelObject;
        private static object _workbooks;
        private static object _workbook;
        private static object _worksheets;
        private static object _worksheet;
        private static object _range;

        // For getting the next cell adress to save data
        private static uint _recordsCount;
        private static readonly List<string> Letters = new List<string>
        {
            "A",
            "B",
            "C",
            "D",
            "E"
        };

        /// <summary>
        /// Saves given data as Excel file to the destination path
        /// </summary>
        /// <param name="path"> destination path </param>
        /// <param name="nodes"> data to save </param>
        public static void Export(string path, ObservableCollection<Node> nodes)
        {
            GetExcelObject();
            CreateWorkbook();
            CreateWorksheet();
            FillCells(nodes);
            CloseExcel(path);
            _recordsCount = 0;
        }

        private static void GetExcelObject()
        {
            // Try to get running Excel or
            // start Excel if it is not started yet
            try
            {
                _excelObject = Marshal.GetActiveObject("Excel.Application");
            }
            catch (COMException)
            {
                var tExcelObj = Type.GetTypeFromProgID("Excel.Application");
                _excelObject = Activator.CreateInstance(tExcelObj);
            }
        }
        
        private static void CloseExcel(string path)
        {
            CloseWorkbook(path);

            Marshal.ReleaseComObject(_range);
            Marshal.ReleaseComObject(_worksheet);
            Marshal.ReleaseComObject(_worksheets);
            Marshal.ReleaseComObject(_workbook);
            Marshal.ReleaseComObject(_workbooks);
            Marshal.ReleaseComObject(_excelObject);
            
            // Вызываем сборщик мусора для немедленной очистки памяти
            GC.GetTotalMemory(true);
            GC.Collect();
        }

        private static void CreateWorkbook()
        {
            _workbooks = _excelObject.GetType().InvokeMember("Workbooks",
                BindingFlags.GetProperty, null, _excelObject, null);

            _workbook = _workbooks.GetType().InvokeMember("Add",
                BindingFlags.InvokeMethod, null, _workbooks, null);
        }

        /// <summary>
        /// Saves and closes a workbook
        /// </summary>
        /// <param name="path"> destination path </param>
        private static void CloseWorkbook(string path)
        {
            var args = new object[2];
            args[0] = true;
            args[1] = path;
            _workbook.GetType().InvokeMember("Close",
                BindingFlags.InvokeMethod, null, _workbook, args);
        }

        private static void CreateWorksheet()
        {
            _worksheets = _workbook.GetType().InvokeMember("Worksheets",
                BindingFlags.GetProperty, null, _workbook, null);

            var args = new object[1];
            args[0] = 1;
            _worksheet = _worksheets.GetType().InvokeMember("Item",
                BindingFlags.GetProperty, null, _worksheets, args);
        }

        private static void InsertText(string text, string columnName, uint rowIndex)
        {
            _range = _worksheet.GetType().InvokeMember("Range",
                BindingFlags.GetProperty,
                null,
                _worksheet,
                new object[] { columnName+rowIndex });
            
            _range.GetType().InvokeMember("Value", BindingFlags.SetProperty,
                null, _range, new object[] { text });

            _range.GetType().InvokeMember("WrapText", BindingFlags.SetProperty,
                null, _range, new object[] { false });
        }

        /// <summary>
        /// Recursively fills the cells of Excel file with the given data, simulating a tree
        /// </summary>
        /// <param name="nodes"> tree-like data to fill cells </param>
        /// <param name="level"> current tree depth (count starts with 0) </param>
        private static void FillCells(ObservableCollection<Node> nodes, int level = 0)
        {
            var i = 0;
            while (i < nodes.Count)
            {
                InsertText(nodes[i].Name, Letters[level], _recordsCount + 1);
                _recordsCount++;
                if (level < 3)
                {
                    FillCells(nodes[i].Nodes, level + 1);
                }

                i++;
            }
        }
    }
}
