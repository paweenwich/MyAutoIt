using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyAutoIt
{
    public class RichTextWriter : TextWriter
    {
        public RichTextBox txt;
        public RichTextWriter(RichTextBox richText)
        {
            txt = richText;
        }
        public override Encoding Encoding { get { return Encoding.UTF8; } }
        public override void Write(string value)
        {
            txt.AppendText(value);
        }

        public override void WriteLine(string value)
        {
            txt.AppendText(value + "\n");
        }
    }
}
