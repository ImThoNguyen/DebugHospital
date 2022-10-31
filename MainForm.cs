using DebugHospital.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DebugHospital.Interface;
using Microsoft.Extensions.Logging;

namespace DebugHospital
{
    public partial class MainForm : Form
    {
        private readonly ILogger<MainForm> _logger;
        private IResultBLL _resultBLL;
        public MainForm(IResultBLL resultBLL, ILogger<MainForm> logger)
        {
            _logger = logger;
            _resultBLL = resultBLL;
            
            InitializeComponent();
            
        }

        static readonly string  formatResult = "<body><b>+ {0}</b>{2}{1}{2}{2}</boby>";
        List<Result> _result;
        StringBuilder _content = new StringBuilder();
        private void rictbInput_TextChanged(object sender, EventArgs e)
        {
            _result = _resultBLL.Generate(rictbInput.Text);
            _content.Clear();
            foreach (Result item in _result)
            {
                _content.Append(string.Format(formatResult, item.IdDao, item.Query, KEYGLOBAL.NewLine));
            }
            webBrowser1.DocumentText = _content.ToString().ToUpper();
        }
    }
}

