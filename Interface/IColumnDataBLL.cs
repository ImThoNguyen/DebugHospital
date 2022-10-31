using DebugHospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugHospital.Interface
{
    public interface IColumnDataBLL
    {
        List<ColumnData> DataSource { get; }
        void RefeshDataSource();
        string GetDDLName(string columnName);
    }
}
