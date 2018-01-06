using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CandyCrushPlugin
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }
        public void dbgLine(string str)
        {
            if (InvokeRequired)
            {
                // We're on a thread other than the GUI thread
                Invoke(new MethodInvoker(() => dbgLine(str)));
                return;
            }
            this.txtDebug.AppendText(str + Environment.NewLine);
        }

    }
}
