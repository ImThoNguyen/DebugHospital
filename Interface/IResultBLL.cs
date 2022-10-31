using DebugHospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugHospital.Interface
{
    public interface IResultBLL 
    {
        List<ColumnData> DataSource { get; }
        List<Result> Generate(string input);
    }
}
