using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyAutoIt
{
    public class MyLogger
    {
        RichTextBox richTextBox;
        public MyLogger()
        {

        }
        public MyLogger(RichTextBox txt )
        {
            richTextBox = txt;
        }

        public void logStr(String str)
        {
            if (richTextBox != null)
            {
                richTextBox.AppendText(str + "\n");
                return;
            }
            Console.WriteLine(str);
        }
    }
}
