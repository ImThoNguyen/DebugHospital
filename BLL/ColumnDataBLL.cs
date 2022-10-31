using DebugHospital.Interface;
using DebugHospital.Models;
using DebugHospital.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DebugHospital.BLL
{
    public class ColumnDataBLL : IColumnDataBLL
    {
        private readonly ILogger<ColumnDataBLL> _logger;

        string _path = Path.Combine(Environment.CurrentDirectory, @"FileData\\Data.xlsx");
        private List<ColumnData> _dataSource;
        public ColumnDataBLL(ILogger<ColumnDataBLL> logger)
        {
            _logger = logger;
            RefeshDataSource();
        }
        public List<ColumnData> DataSource
        {
            get
            {
                if (_dataSource == null)
                    _dataSource = this.GetDataSource(_path);
                return _dataSource;
            }
            set  
            {
                _dataSource = value;
            }
        }

        public void RefeshDataSource()
        {
            this.DataSource = this.GetDataSource(_path);
        }

        public string GetDDLName(string columnName)
        {
            string result = string.Empty;
            if (this.DataSource != null)
            {
                ColumnData column = this.DataSource.Find(x => x.ColumnName.ToUpper() == columnName.ToUpper());
                if (column != null)
                    result = column.DDLName.ToUpper();
                else
                    result = "NVARCHAR(500)"; //default
            }
            return result;
        }

        /// <summary>
        /// this method will read the excel file and copy its data into a List<COLUMNDATA>
        /// </summary>
        /// <param name="fileName">name of the file</param>
        /// <returns></returns>
        private List<ColumnData> GetDataSource(string fileName)
        {
            try
            {
                List<ColumnData> listData = new List<ColumnData>();
                if (File.Exists(fileName))
                {
                    DataTable data = ExcelExtensions.ReadExcel(fileName);
                    if (data != null && data.Rows.Count > 0)
                    {
                        listData.AddRange(DataSetExtensions.ConvertDataTable<ColumnData>(data));
                    }
                }
                
                return listData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }

        }
    }
}
