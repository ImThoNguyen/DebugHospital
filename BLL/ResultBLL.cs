using DebugHospital.Interface;
using DebugHospital.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DebugHospital.BLL
{
    public class ResultBLL : IResultBLL
    {
        private IColumnDataBLL _columnDataBLL;

        static readonly Regex trimmer = new Regex(@"\s\s+");
        private List<Result> _resultAll;
        public ResultBLL(IColumnDataBLL columnDataBLL)
        {
            _columnDataBLL = columnDataBLL;
        }
        public List<ColumnData> DataSource
        {
            get { return _columnDataBLL.DataSource; }
        }

        public List<Result> Generate(string input)
        {
            try
            {
                if (_resultAll == null)
                    _resultAll = new List<Result>();
                else
                    _resultAll.Clear();
                input = input.ToUpper();
                Result result1 = new Result();
                StringReader reader = new StringReader(input);
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                ineligible:
                    if (string.IsNullOrEmpty(result1.IdDao))
                    {
                        result1.TextInput.AppendFormat("{0}{1}", line, KEYGLOBAL.NewLine);
                        if (line.Contains(KEY.Key1))
                        {
                            string idDao = GetStringFromTo(line, KEY.Key, KEY.Key1);
                            if (!string.IsNullOrEmpty(idDao))
                            {
                                string sql1 = GetStringFromTo(line, KEY.Key1, "").Replace(KEYGLOBAL.Char_OpenSquareBrackets, "").Replace(KEYGLOBAL.Char_CloseSquareBrackets, "").Trim();
                                if (!string.IsNullOrEmpty(sql1))
                                {
                                    sql1 = trimmer.Replace(sql1, " ");
                                    result1.Query.Append(sql1);
                                    result1.IdDao = idDao;
                                    _resultAll.Add(result1);
                                    continue;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(result1.IdDao))
                        {
                            result1.Query.Append("Unable to compile!");
                            _resultAll.Add(result1);
                            Log.Information("Unable to compile!");
                        }
                        result1 = new Result();
                    }
                    else
                    {
                        if (line.Contains(KEY.Key2) && GetStringFromTo(line, KEY.Key, KEY.Key2) == result1.IdDao)
                        {
                            string param = GetStringFromTo(line, KEY.Key2, string.Empty);
                            string[] param1 = param.Split(stringSeparators, StringSplitOptions.None);
                            for (int i = param1.Length - 1; i >= 0; i--)
                            {
                                string[] str1 = param1[i].Replace(KEYGLOBAL.Char_CloseSquareBrackets, "").Replace(KEYGLOBAL.Char_OpenSquareBrackets, "").Split(new string[] { KEYGLOBAL.Char_EqualSign }, StringSplitOptions.None);
                                if (str1.Length > 1)
                                {
                                    string[] str2 = str1[1].Split(new string[] { "," }, StringSplitOptions.None);
                                    if (str2.Length > 0)
                                    {
                                        string ddlName = _columnDataBLL.GetDDLName(str2[0]);
                                        string value = string.Empty;
                                        if (str2.Length > 1)
                                        {
                                            value = str2.Length > 1 ? str2[1].Trim() : string.Empty;
                                            value = FormatValue(value, ddlName);
                                        }
                                        if (string.IsNullOrEmpty(value)) value = "''";
                                        result1.Query.Insert(0, string.Format("DECLARE {0} {1} = {2}; {3}", str1[0].Trim(), ddlName, value, KEYGLOBAL.NewLine));
                                    }
                                }
                            }
                            result1.TextInput.AppendFormat("{0}{1}", line, KEYGLOBAL.NewLine);
                            result1 = new Result();
                            continue;
                        }
                        else
                        {
                            result1 = new Result();
                            goto ineligible;
                        }
                    }
                };
                return _resultAll;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw ex;
            }
        }
        string[] stringSeparators = new string[] { "]," };


        private string FormatValue(string value, string ddlName)
        {
            string result = value;
            if (!DATATYPE.TYPENUMBERIC.Any(ddlName.Contains) && !DATATYPE.VALUEERROR.Any(value.Contains))
            {
                if (ddlName.Contains("NVARCHAR"))
                    result = string.Format("N'{0}'", value);
                else
                    result = string.Format("'{0}'", value);
            }
            return result;
        }

        private string GetStringFromTo(string line, string keyFrom, string keyTo)
        {
            string result = string.Empty;
            int index = line.IndexOf(keyFrom);
            if (index >= 0)
            {
                int pFrom = index + keyFrom.Length;
                int pTo = line.Length;
                if (!string.IsNullOrEmpty(keyTo))
                {
                    pTo = line.IndexOf(keyTo);
                }
                result = line.Substring(pFrom, pTo - pFrom).Trim();
            }
            return result;
        }
    }
}
