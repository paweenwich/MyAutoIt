using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
                if (richTextBox.InvokeRequired)
                {
                    //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    richTextBox.Invoke((MethodInvoker)delegate {
                        //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                        richTextBox.AppendText(str + "\n");
                    });
                }
                else
                {
                    richTextBox.AppendText(str + "\n");
                }
                return;
            }
            Console.WriteLine(str);
        }

        public void logError(String str)
        {
            if (richTextBox != null)
            {
                if (richTextBox.InvokeRequired)
                {
                    richTextBox.Invoke((MethodInvoker)delegate {
                        richTextBox.AppendText(Color.Red, str + "\n");
                    });
                }
                else
                {
                    richTextBox.AppendText(Color.Red, str + "\n");
                }
                return;
            }
            Console.WriteLine("ERROR: " + str);
        }
    }

    public static class MyLoggerExtend
    {
        public static void AppendText(this RichTextBox box, Color color, string text)
        {
            int start = box.TextLength;
            box.AppendText(text);
            int end = box.TextLength;

            // Textbox may transform chars, so (end-start) != text.Length
            box.Select(start, end - start);
            {
                box.SelectionColor = color;
                // could set box.SelectionBackColor, box.SelectionFont too.
            }
            box.SelectionLength = 0; // clear
        }
    }
}
