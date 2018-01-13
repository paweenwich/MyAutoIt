using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Drawing.Imaging;
using System.Security.Principal;
using System.IO;

namespace MyAutoIt
{
    public class Utils
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string sClass, string sWindow);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(MyEnumWindowsProc lpEnumFunc, ref WindowSearchData data);

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, MyEnumWindowsProc callback, ref WindowSearchData data);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        public delegate bool MyEnumWindowsProc(IntPtr hWnd, ref WindowSearchData lParam);
        public static MyEnumWindowsProc callBackPtr;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData,UIntPtr dwExtraInfo);

        [DllImport("gdi32.dll")]
        public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcmp(IntPtr b1, IntPtr b2, long count);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        // REQUIRED STRUCTS
        public struct MEMORY_BASIC_INFORMATION
        {
            public int BaseAddress;
            public int AllocationBase;
            public int AllocationProtect;
            public int RegionSize;
            public int State;
            public int Protect;
            public int lType;
            public override String ToString()
            {
                String type = String.Format("{0:X8}", lType);
                switch (lType)
                {
                    case 0x1000000: type = "MEM_IMAGE"; break;
                    case 0x40000: type = "MEM_MAPPED"; break;
                    case 0x20000: type = "MEM_PRIVATE"; break;
                }

                String protect = String.Format("{0:X8}", Protect);
                switch (Protect)
                {
                    case 0x01: protect = "PAGE_NOACCESS"; break;
                    case 0x02: protect = "PAGE_READONLY"; break;
                    case 0x04: protect = "PAGE_READWRITE"; break;
                    case 0x08: protect = "PAGE_WRITECOPY"; break;
                    case 0x10: protect = "PAGE_EXECUTE"; break;
                    case 0x20: protect = "PAGE_EXECUTE_READ"; break;
                    case 0x40: protect = "PAGE_EXECUTE_READWRITE"; break;

                }

                String state = String.Format("{0:X8}", State);
                switch (State)
                {
                    case 0x1000: state = "MEM_COMMIT"; break;
                    case 0x2000: state = "MEM_RESERVE"; break;
                    case 0x10000: state = "MEM_FREE"; break;
                }
                return String.Format("Addr={0:X8} Type={2} Protect={3} State={4} Size={1} ", BaseAddress, RegionSize, type, protect, state);
            }
        }

        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;
            public IntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }


        public const int PROCESS_QUERY_INFORMATION = 0x0400;
        public const int MEM_COMMIT = 0x00001000;
        public const int MEM_FREE = 0x10000;
        public const int MEM_RESERVE = 0x2000;

        public const int PAGE_NOACCESS = 0x01;
        public const int PAGE_READONLY = 0x02;
        public const int PAGE_READWRITE = 0x04;
        public const int PAGE_WRITECOPY = 0x08;
        public const int PAGE_EXECUTE = 0x10;
        public const int PAGE_EXECUTE_READ = 0x20;
        public const int PAGE_EXECUTE_READWRITE = 0x40;
        public const int PROCESS_WM_READ = 0x0010;

        public const int MEM_IMAGE = 0x1000000;
        public const int MEM_MAPPED = 0x40000;
        public const int MEM_PRIVATE = 0x20000;


        ///summary>
        /// Virtual Messages
        /// </summary>
        public enum WMessages : int
        {
            WM_LBUTTONDOWN = 0x201, //Left mousebutton down
            WM_LBUTTONUP = 0x202,  //Left mousebutton up
            WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
            WM_RBUTTONDOWN = 0x204, //Right mousebutton down
            WM_RBUTTONUP = 0x205,   //Right mousebutton up
            WM_RBUTTONDBLCLK = 0x206, //Right mousebutton doubleclick
            WM_KEYDOWN = 0x100,  //Key down
            WM_KEYUP = 0x101,   //Key up
        }

        /// <summary>
        /// Virtual Keys
        /// </summary>
        public enum VKeys : int
        {
            VK_LBUTTON = 0x01,   //Left mouse button 
            VK_RBUTTON = 0x02,   //Right mouse button 
            VK_CANCEL = 0x03,   //Control-break processing 
            VK_MBUTTON = 0x04,   //Middle mouse button (three-button mouse) 
            VK_BACK = 0x08,   //BACKSPACE key 
            VK_TAB = 0x09,   //TAB key 
            VK_CLEAR = 0x0C,   //CLEAR key 
            VK_RETURN = 0x0D,   //ENTER key 
            VK_SHIFT = 0x10,   //SHIFT key 
            VK_CONTROL = 0x11,   //CTRL key 
            VK_MENU = 0x12,   //ALT key 
            VK_PAUSE = 0x13,   //PAUSE key 
            VK_CAPITAL = 0x14,   //CAPS LOCK key 
            VK_ESCAPE = 0x1B,   //ESC key 
            VK_SPACE = 0x20,   //SPACEBAR 
            VK_PRIOR = 0x21,   //PAGE UP key 
            VK_NEXT = 0x22,   //PAGE DOWN key 
            VK_END = 0x23,   //END key 
            VK_HOME = 0x24,   //HOME key 
            VK_LEFT = 0x25,   //LEFT ARROW key 
            VK_UP = 0x26,   //UP ARROW key 
            VK_RIGHT = 0x27,   //RIGHT ARROW key 
            VK_DOWN = 0x28,   //DOWN ARROW key 
            VK_SELECT = 0x29,   //SELECT key 
            VK_PRINT = 0x2A,   //PRINT key
            VK_EXECUTE = 0x2B,   //EXECUTE key 
            VK_SNAPSHOT = 0x2C,   //PRINT SCREEN key 
            VK_INSERT = 0x2D,   //INS key 
            VK_DELETE = 0x2E,   //DEL key 
            VK_HELP = 0x2F,   //HELP key
            VK_0 = 0x30,   //0 key 
            VK_1 = 0x31,   //1 key 
            VK_2 = 0x32,   //2 key 
            VK_3 = 0x33,   //3 key 
            VK_4 = 0x34,   //4 key 
            VK_5 = 0x35,   //5 key 
            VK_6 = 0x36,    //6 key 
            VK_7 = 0x37,    //7 key 
            VK_8 = 0x38,   //8 key 
            VK_9 = 0x39,    //9 key 
            VK_A = 0x41,   //A key 
            VK_B = 0x42,   //B key 
            VK_C = 0x43,   //C key 
            VK_D = 0x44,   //D key 
            VK_E = 0x45,   //E key 
            VK_F = 0x46,   //F key 
            VK_G = 0x47,   //G key 
            VK_H = 0x48,   //H key 
            VK_I = 0x49,    //I key 
            VK_J = 0x4A,   //J key 
            VK_K = 0x4B,   //K key 
            VK_L = 0x4C,   //L key 
            VK_M = 0x4D,   //M key 
            VK_N = 0x4E,    //N key 
            VK_O = 0x4F,   //O key 
            VK_P = 0x50,    //P key 
            VK_Q = 0x51,   //Q key 
            VK_R = 0x52,   //R key 
            VK_S = 0x53,   //S key 
            VK_T = 0x54,   //T key 
            VK_U = 0x55,   //U key 
            VK_V = 0x56,   //V key 
            VK_W = 0x57,   //W key 
            VK_X = 0x58,   //X key 
            VK_Y = 0x59,   //Y key 
            VK_Z = 0x5A,    //Z key
            VK_NUMPAD0 = 0x60,   //Numeric keypad 0 key 
            VK_NUMPAD1 = 0x61,   //Numeric keypad 1 key 
            VK_NUMPAD2 = 0x62,   //Numeric keypad 2 key 
            VK_NUMPAD3 = 0x63,   //Numeric keypad 3 key 
            VK_NUMPAD4 = 0x64,   //Numeric keypad 4 key 
            VK_NUMPAD5 = 0x65,   //Numeric keypad 5 key 
            VK_NUMPAD6 = 0x66,   //Numeric keypad 6 key 
            VK_NUMPAD7 = 0x67,   //Numeric keypad 7 key 
            VK_NUMPAD8 = 0x68,   //Numeric keypad 8 key 
            VK_NUMPAD9 = 0x69,   //Numeric keypad 9 key 
            VK_SEPARATOR = 0x6C,   //Separator key 
            VK_SUBTRACT = 0x6D,   //Subtract key 
            VK_DECIMAL = 0x6E,   //Decimal key 
            VK_DIVIDE = 0x6F,   //Divide key
            VK_F1 = 0x70,   //F1 key 
            VK_F2 = 0x71,   //F2 key 
            VK_F3 = 0x72,   //F3 key 
            VK_F4 = 0x73,   //F4 key 
            VK_F5 = 0x74,   //F5 key 
            VK_F6 = 0x75,   //F6 key 
            VK_F7 = 0x76,   //F7 key 
            VK_F8 = 0x77,   //F8 key 
            VK_F9 = 0x78,   //F9 key 
            VK_F10 = 0x79,   //F10 key 
            VK_F11 = 0x7A,   //F11 key 
            VK_F12 = 0x7B,   //F12 key
            VK_SCROLL = 0x91,   //SCROLL LOCK key 
            VK_LSHIFT = 0xA0,   //Left SHIFT key
            VK_RSHIFT = 0xA1,   //Right SHIFT key
            VK_LCONTROL = 0xA2,   //Left CONTROL key
            VK_RCONTROL = 0xA3,    //Right CONTROL key
            VK_LMENU = 0xA4,      //Left MENU key
            VK_RMENU = 0xA5,   //Right MENU key
            VK_PLAY = 0xFA,   //Play key
            VK_ZOOM = 0xFB, //Zoom key 
        }


        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x0800,

    }

    public const short SWP_NOMOVE = 0X2;
        public const short SWP_NOSIZE = 1;
        public const short SWP_NOZORDER = 0X4;
        public const int SWP_SHOWWINDOW = 0x0040;


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;
            override public String ToString()
            {
                return ("[" + left + "," + top + "," + right + "," + bottom + "]");
            }
            public static RECT fromInt(int l,int t, int r, int b)
            {
                RECT ret = new RECT();
                ret.left = l;
                ret.top = t;
                ret.right = r;
                ret.bottom = b;
                return (ret);
            }
            public int Width {
                get { return right - left; }
            }
            public int Height
            {
                get { return bottom - top; }
            }

        }

        public class WindowSearchData
        {
            public string Title;
            public string ClassName;
            public bool flgActivate;
            public IntPtr hwnd;
        }

        public static IntPtr lastHwnd;

        static public int MakeLParam(int LoWord, int HiWord)
        {
            return (int)((HiWord << 16) | (LoWord & 0xFFFF));
        }


        static public System.Drawing.Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = intToColor(pixel);
            return color;
        }
        static public Color intToColor(uint pixel)
        {
            return (Color.FromArgb((int)(pixel & 0x000000FF),
                         (int)(pixel & 0x0000FF00) >> 8,
                         (int)(pixel & 0x00FF0000) >> 16));
        }

        // approx YU distance https://www.compuphase.com/cmetric.htm
        public static double ColourDistance(Color e1, Color e2)
        {
            long rmean = ((long)e1.R + (long)e2.R) / 2;
            long r = (long)e1.R - (long)e2.R;
            long g = (long)e1.G - (long)e2.G;
            long b = (long)e1.B - (long)e2.B;
            return Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
        }

        //
        public static double diffColor(Color c, Color color)
        {
            double d = ((c.R - color.R) * (c.R - color.R))
                + ((c.G - color.G) * (c.G - color.G))
                + ((c.B - color.B) * (c.B - color.B));
            return (d);
        }
        public static Color getCloseColorIndex(Color sample, Color[] colors, int maxError = 5000)
        {
            Color ret = Color.FromArgb(255, 0, 0, 0);
            double diff = 200000; // > 255²x3
            Color c = sample;
            foreach (Color color in colors)
            {

                double d = diffColor(c, color);
                if (d < diff)
                {
                    diff = d;
                    ret = color;
                }
            }
            //txtDebug.AppendText("dist = " + diff + "\n");
            if (diff > maxError)
            {
                return (Color.FromArgb(255, 255, 255, 255));
            }

            return (ret);
        }
        public static Color avgColor(Color[] c)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            foreach (Color cl in c)
            {
                r += cl.R;
                g += cl.G;
                b += cl.B;
            }
            return (Color.FromArgb(255, r / c.Length, g / c.Length, b / c.Length));
        }

        public static Bitmap CaptureApplication(String procName)
        {
            var proc = Process.GetProcessesByName(procName)[0];
            IntPtr hwnd = GetWindowHandleByProcessName(new String[] { procName });
            lastHwnd = hwnd;
            return PrintWindow(hwnd);
        }

        public static IntPtr GetWindowHandleByProcessName(String[] procNames)
        {
            foreach (String procName in procNames)
            {
                foreach (Process p in Process.GetProcessesByName(procName))
                {
                    if (p.MainWindowHandle != IntPtr.Zero)
                    {
                        return p.MainWindowHandle;
                    }
                }
            }
            return IntPtr.Zero;
        }

        public static IntPtr GetWindowHandleByClassName(String className)
        {
            return FindWindow(className,null);

        }


        public static Bitmap GetScreenShot()
        {
            Bitmap result = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            {
                using (Graphics gfx = Graphics.FromImage(result))
                {
                    gfx.CopyFromScreen(System.Windows.Forms.Screen.PrimaryScreen.Bounds.X, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y, 0, 0, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                }
            }
            return result;
        }

        public static Bitmap PrintWindow(IntPtr hwnd)
        {
            RECT rc;
            GetWindowRect(hwnd, out rc);

            Bitmap bmp = new Bitmap(rc.right - rc.left, rc.bottom - rc.top, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            PrintWindow(hwnd, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }
        public static Bitmap cropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea,bmpImage.PixelFormat);
            return (bmpCrop);
        }

        public static bool Report(IntPtr hwnd, ref WindowSearchData searchData)
        {
            int size = 10240;

            if (searchData.ClassName != null)
            {
                String winClass = GetWinClass(hwnd);
                Debug.WriteLine(String.Format("{0:X}", hwnd.ToInt32()) + " ClassName=[" + winClass + "]");
                if (winClass.IndexOf(searchData.ClassName) >= 0)
                {
                    searchData.hwnd = hwnd;
                    Debug.WriteLine("Window handle is " + hwnd + " class=" + winClass.ToString());
                    if (searchData.flgActivate)
                    {
                        SetForegroundWindow(hwnd);
                        Thread.Sleep(500);
                    }
                    return (false);
                }
            }
            else
            {
                StringBuilder winTitle = new StringBuilder(size);
                GetWindowText(hwnd, winTitle, size);
                String title = winTitle.ToString();

                //Debug.WriteLine("Report:" + title);
                if (title.IndexOf(searchData.Title) >= 0)
                {
                    searchData.hwnd = hwnd;
                    Debug.WriteLine("Window handle is " + hwnd + " title=" + winTitle.ToString());
                    if (searchData.flgActivate)
                    {
                        SetForegroundWindow(hwnd);
                        Thread.Sleep(500);
                    }
                    return (false);
                }
            }
            return (true);
        }

        public static string GetWinClass(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return null;
            StringBuilder classname = new StringBuilder(100);
            IntPtr result = GetClassName(hwnd, classname, classname.Capacity);
            if (result != IntPtr.Zero)
                return classname.ToString();
            return null;
        }


        public static IntPtr SearchWindow(String title,bool flgA=false)
        {
            WindowSearchData sd = new WindowSearchData { Title = title, flgActivate = flgA };
            callBackPtr = new MyEnumWindowsProc(Report);  
            EnumWindows(callBackPtr, ref sd);
            return sd.hwnd;
        }
        public static IntPtr SearchWindowByClassName(IntPtr hwnd, String className, bool flgA = false)
        {
            WindowSearchData sd = new WindowSearchData { ClassName = className, flgActivate = flgA };
            callBackPtr = new MyEnumWindowsProc(Report);
            EnumChildWindows(hwnd, callBackPtr, ref sd);
            //EnumWindows(callBackPtr, ref sd);
            return sd.hwnd;
        }


        public static String AdbExec(String command)
        {
            string[] adbs = {
                @"C:\Users\Administrator\AppData\Local\Android\sdk\platform-tools\adb.exe",
                @"C:\adb\adb.exe",
            };
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "";
            for (int i=0;i< adbs.Length; i++)
            {
                if (File.Exists(adbs[i]))
                {
                    process.StartInfo.FileName = adbs[i];
                }
            }
            if(process.StartInfo.FileName == "")
            {
                Console.WriteLine("ERROR: adb.exe not found for [{0}]",command);
                return "adb.exe not found";
            }
            //process.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Android\sdk\platform-tools\adb.exe";

            if (ADBDevice != "")
            {
                process.StartInfo.Arguments = "-s " + ADBDevice + " " + command;
            }
            else
            {
                process.StartInfo.Arguments = command;
            }
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.Close();
            return output;
        }

        public static String ADBDevice = "";

        public static Dictionary<String,String> AdbListDevice()
        {
            Dictionary<String, String> ret = new Dictionary<string, string>();
            String argument = String.Format("devices -l");
            String strList =  AdbExec(argument);
            String[] lines = strList.Split(new[] { '\r', '\n' });
            foreach(String s in lines)
            {
                if (!s.Contains("device ")) continue;
                String[] tmp =  s.Split(new[] {" device " }, StringSplitOptions.None);
                if (tmp.Length == 2)
                {
                    ret.Add(tmp[0].Trim(), tmp[1].Trim());
                }
            }
            return ret;
        }


        public static String AdbMouseClick(int x, int y)
        {
            Console.WriteLine("AdbMouseClick: " + x + " " + y);
            String argument = String.Format("shell input mouse tap {0} {1}", x, y);
            return AdbExec(argument);
/*            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = @"C:\adb\adb.exe";
            process.StartInfo.Arguments = argument;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            
            process.Close();
            return output;*/
        }

        public static String AdbScroll(int x,int y,int x2, int y2)
        {
            //adb shell input swipe 300 300 500 1000
            Console.WriteLine("AdbMouseClick: " + x + " " + y);
            String argument = String.Format("shell input swipe {0} {1} {2} {3}", x, y, x2,y2);
            return AdbExec(argument);
            /*System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = @"C:\adb\adb.exe";
            process.StartInfo.Arguments = argument;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.Close();
            return output;*/
        }

        public static String AdbCpatureToFile(String fileName)
        {
            //Console.WriteLine("AdbCpatureToFile: " + fileName);
            String cmd1 = String.Format("shell screencap -p /data/local/tmp/screen.png");
            AdbExec(cmd1);
            String cmd2 = String.Format("pull /data/local/tmp/screen.png {0}",fileName);
            return AdbExec(cmd2);
        } 

        public static void MouseClick(String button,int x, int y)
        {
            MouseMove(x, y);
            if (button == "LEFT")
            {
                Utils.mouse_event((int)(Utils.MouseEventFlags.LEFTUP |Utils.MouseEventFlags.LEFTDOWN  | Utils.MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);
                //Thread.Sleep(100);
                //Utils.mouse_event((int)(Utils.MouseEventFlags.LEFTUP | Utils.MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);
            }
            if (button == "RIGHT")
            {
                Utils.mouse_event((int)(Utils.MouseEventFlags.RIGHTDOWN | Utils.MouseEventFlags.RIGHTUP | Utils.MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);
            }

        }

        public static void MoseWheel(int value)
        {
            Utils.mouse_event((int)(Utils.MouseEventFlags.WHEEL), 0, 0, value, UIntPtr.Zero);
        }

        public static void MouseMove(int x, int y)
        {
            int w = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int h = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            x = ((x * 65535) / w);
            y = ((y * 65535) / h);
            Utils.mouse_event((int)(Utils.MouseEventFlags.MOVE | Utils.MouseEventFlags.ABSOLUTE),x,y, 0, UIntPtr.Zero);
            //SetCursorPos(x, y);
            Thread.Sleep(1);
        }

        public static Point PixelSearchWin7(Utils.RECT rect, Color cl, double diff = 0)
        {
            Bitmap bmp = GetScreenShot();
            double minD = 100000;
            for (int y = rect.top; y < rect.bottom; y++)
            {
                for (int x = rect.left; x < rect.right; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    double d = diffColor(c, cl);
                    if (d < minD)
                    {
                        minD = d;
                        if (d <= diff)
                        {
                            return (new Point(x, y));
                        }
                    }
                }
            }
            Debug.WriteLine(" " + minD +"\n");
            return (new Point(0, 0));
        }
        public static Point PixelSearch(Utils.RECT rect, Color cl, double diff = 0)
        {

            if (System.Environment.OSVersion.Version.Major == 6)
            {
                return (PixelSearchWin7(rect,cl,diff));
            }

            IntPtr hdc = GetDC(IntPtr.Zero);
            for (int y = rect.top; y < rect.bottom; y++)
            {
                for (int x = rect.left; x < rect.right; x++)
                {
                    uint pixel = GetPixel(hdc, x, y);
                    Color c = intToColor(pixel);
                    double d = diffColor(c, cl);
                    //Debug.WriteLine(c + " " + cl + " " + d + "[" + x + "," + y + "]");
                    if (d <= diff)
                    {
                        ReleaseDC(IntPtr.Zero, hdc);
                        return (new Point(x, y));
                    }
                }
            }
            ReleaseDC(IntPtr.Zero, hdc);
            return (new Point(0,0));
        }

        public static void drawRect(Utils.RECT rect)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            Rectangle(hdc, rect.left-10, rect.top-10, rect.right+10, rect.bottom+10);
            ReleaseDC(IntPtr.Zero, hdc);
        }

        public static Boolean activateWindow(String title)
        {
            SearchWindow(title, true);
            return (true);
        }

        public static bool CompareMemCmp(Bitmap b1, Bitmap b2)
        {
            if ((b1 == null) != (b2 == null)) return false;
            if (b1.Size != b2.Size) return false;

            BitmapData bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }

        public static Bitmap LoadAndDeleteBitmap(String fileName)
        {
            try
            {
                Image img = Image.FromFile(fileName);
                Bitmap bmp = new Bitmap(img);
                img.Dispose();
                File.Delete(fileName);
                return bmp;
            }
            catch
            {
                return null;
            }
        }


        public static Point findBitmap(Bitmap src, Utils.RECT rect, Bitmap pattern, int step=5,double error1 = 10000, double error2 = 100000)
        {
            Point p = new Point(0, 0);
            double minD = 100000000;
            for (int y = rect.top; y < rect.bottom - pattern.Height; y++)
            {
                for (int x = rect.left; x < rect.right - pattern.Width; x++)
                {
                    double count = 0;
                    for (int i = 0; i < pattern.Width; i += step)
                    {
                        for (int j = 0; j < pattern.Height; j += step)
                        {
                            Color cPattern = pattern.GetPixel(i, j);
                            Color cScr = src.GetPixel(x + i, y + j);
                            double d = diffColor(cPattern, cScr);
                            if (d > error1)
                            {
                                i = pattern.Width;
                                count = minD;
                                break;
                            }
                            count += d;
                            if (count > error2)
                            {
                                i = pattern.Width;
                                count = minD;
                                break;
                            }
                        }
                    }
                    if (count < minD)
                    {
                        minD = count;
                        p.X = x;
                        p.Y = y;
                        //Console.WriteLine("x=" + x + " y=" + y + " min=" + minD + " count=" + count);
                        if (minD == 0)
                        {
                            return (p);
                        }
                    }
                }
                //Console.WriteLine(" y=" + y + " min=" + minD);
            }
            //Console.WriteLine("p=" + p + " min=" + minD);
            return (p);
        }

        public static Point findBitmap(Utils.RECT rect, Bitmap pattern)
        {
            return(findBitmap(GetScreenShot(),rect,pattern));
        }

        public static Point SmartFindBitmap(Bitmap src, Utils.RECT rect, Bitmap pattern, int step = 5, double error1 = 10000, double error2 = 100000)
        {
            List<Point> lst = GetSubPositions(new MyRawBitmapData(src), new MyRawBitmapData(pattern), rect, step, error1);
            if (lst.Count > 0)
            {
                return (lst[0]);
            }
            return(new Point(0, 0));
        }

        public static Point SmartFindBitmap(MyRawBitmapData src, Utils.RECT rect, MyRawBitmapData pattern, int step = 5, double error1 = 10000, double error2 = 100000)
        {
            List<Point> lst = GetSubPositions(src, pattern, rect, step, error1);
            if (lst.Count > 0)
            {
                return (lst[0]);
            }
            return (new Point(0, 0));
        }

        public static List<Point> GetSubPositions(MyRawBitmapData main, MyRawBitmapData sub, Utils.RECT rect, int step=1, double colorError=1, bool flgSingle=true)
        {
            List<Point> possiblepos = new List<Point>();

            int subwidth = sub.width;
            int subheight = sub.height;

            int movewidth = rect.right - subwidth;
            int moveheight = rect.bottom - subheight;

            int maxX = rect.left + movewidth - subwidth;
            int maxY = rect.top + moveheight - subheight;
            int cX = subwidth / 2;
            int cY = subheight / 2;
            MyColor firstColor = sub.GetColor(0, 0);
            MyColor CenterColor = sub.GetColor(cX, cY);
            for (int y = rect.top; y < moveheight; ++y)
            {
                for (int x = rect.left; x < movewidth; ++x)
                {
                    MyColor curcolor = main.GetColor(x, y);
                    if (possiblepos.Count > 0)
                    {
                        foreach (var item in possiblepos.ToArray())
                        {
                            int xsub = x - item.X;
                            int ysub = y - item.Y;
                            if (xsub >= subwidth || ysub >= subheight || xsub < 0)
                            {
                                continue;
                            }

                            MyColor subcolor = sub.GetColor(xsub, ysub);
                            if (!curcolor.nearColor(subcolor, colorError))
                            {
                                possiblepos.Remove(item);
                            }
                        }
                    }
                    // add part 
                    // we should not add pixel that will out of bound 
                    if (x > maxX)
                    {
                        continue;
                    }
                    if (y > maxY)
                    {
                        continue;
                    }

                    // add if first pixel match
                    if (curcolor.nearColor(firstColor, colorError))
                    {
                        if (CenterColor.nearColor(main.GetColor(x + cX, y + cY), colorError))
                        {
                            possiblepos.Add(new Point(x, y));
                        }
                    }
                }
                if (flgSingle && (possiblepos.Count > 0))
                {
                    if (y - subheight > possiblepos[0].Y)
                    {
                        //Console.WriteLine("found break");
                        break;
                    }
                }
                //Console.WriteLine("Y=" + y + " Count=" + possiblepos.Count);
            }
            return possiblepos;
        }

        public static void ControlClickWindow(IntPtr hWnd, string button, int x, int y, bool doubleklick)
        {
            int LParam = MakeLParam(x, y);

            int btnDown = 0;
            int btnUp = 0;

            if (button == "LEFT")
            {
                btnDown = (int)WMessages.WM_LBUTTONDOWN;
                btnUp = (int)WMessages.WM_LBUTTONUP;
            }

            if (button == "RIGHT")
            {
                btnDown = (int)WMessages.WM_RBUTTONDOWN;
                btnUp = (int)WMessages.WM_RBUTTONUP;
            }


            if (doubleklick == true)
            {
                SendMessage(hWnd, btnDown, 0, LParam);
                SendMessage(hWnd, btnUp, 0, LParam);
                SendMessage(hWnd, btnDown, 0, LParam);
                SendMessage(hWnd, btnUp, 0, LParam);
            }else {
                SendMessage(hWnd, btnDown, 0, LParam);
                SendMessage(hWnd, btnUp, 0, LParam);
            }

        }

        public static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        public static string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = asciiSymbol(b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }

        static char asciiSymbol(byte val)
        {
            if (val < 32) return '.';  // Non-printable ASCII
            if (val < 127) return (char)val;   // Normal ASCII
                                               // Handle the hole in Latin-1
            if (val == 127) return '.';
            if (val < 0x90) return "€.‚ƒ„…†‡ˆ‰Š‹Œ.Ž."[val & 0xF];
            if (val < 0xA0) return ".‘’“”•–—˜™š›œ.žŸ"[val & 0xF];
            if (val == 0xAD) return '.';   // Soft hyphen: this symbol is zero-width even in monospace fonts
            return (char)val;   // Normal Latin-1
        }

        public static IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }



        public static byte[] IntToByteArray(int data)
        {
            RawSerializer<int> rs = new RawSerializer<int>();
            return rs.RawSerialize(data);
        }
        public static int ByteArrayToInt(byte[] data)
        {
            RawSerializer<int> rs2 = new RawSerializer<int>();
            return rs2.RawDeserialize(data);
        }
        public class MyRawBitmapData
        {
             public int stride;
             public byte[] data;
             public int width;
             public int height;
             public MyRawBitmapData(Bitmap main)
             {
                 width = main.Width;
                 height = main.Height;
                 BitmapData bmMainData = main.LockBits(new Rectangle(0, 0, main.Width, main.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                 int bytesMain = Math.Abs(bmMainData.Stride) * main.Height;
                 stride = bmMainData.Stride;
                 data = new byte[bytesMain];
                 System.Runtime.InteropServices.Marshal.Copy(bmMainData.Scan0, data, 0, bytesMain);
                 main.UnlockBits(bmMainData);
             }
             public  MyColor GetColor(Point point)
             {
                return GetColor(point.X, point.Y);
             }
             public MyColor GetColor(int x,int y)
             {
                 int pos = y * stride + x * 4;
                 byte a = data[pos + 3];
                 byte r = data[pos + 2];
                 byte g = data[pos + 1];
                 byte b = data[pos + 0];
                 return MyColor.FromARGB(a, r, g, b);
             }

        }

        public struct MyColor
        {
            byte A;
            byte R;
            byte G;
            byte B;

            public static MyColor FromARGB(byte a, byte r, byte g, byte b)
            {
                MyColor mc = new MyColor();
                mc.A = a;
                mc.R = r;
                mc.G = g;
                mc.B = b;
                return mc;
            }

            public bool EqualsColor(object obj)
            {
                if (!(obj is MyColor))
                    return false;
                MyColor color = (MyColor)obj;
                if (color.R == this.R && color.G == this.G && color.B == this.B && color.A == this.A)
                    return true;
                return false;
            }
            public bool nearColor(MyColor c, double value)
            {
                double d = diffColor(c);
                return (d <= value);
            }
            public double diffColor(MyColor c)
            {
                double d = ((c.R - R) * (c.R - R))
                    + ((c.G - G) * (c.G - G))
                    + ((c.B - B) * (c.B - B));
                return (d);
            }

            public override String ToString()
            {
                return ("[" + R + "," + G + "," + B + "]");
            }

            public class EqualityComparer : IEqualityComparer<MyColor>
            {
                #region IEqualityComparer<MyColor> Members

                bool IEqualityComparer<MyColor>.Equals(MyColor x, MyColor y)
                {
                    return (x.R == y.R && x.G == y.G && x.B == y.B && x.A == y.A);
                }

                int IEqualityComparer<MyColor>.GetHashCode(MyColor x)
                {
                    return (x.R * x.G * x.B);
                }

                #endregion
            }
        }


    }
    public class RawSerializer<T>
    {
        public T RawDeserialize(byte[] rawData)
        {
            return RawDeserialize(rawData, 0);
        }

        public T RawDeserialize(byte[] rawData, int position)
        {
            int rawsize = Marshal.SizeOf(typeof(T));
            if (rawsize > rawData.Length)
                return default(T);

            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
            T obj = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);
            return obj;
        }

        public byte[] RawSerialize(T item)
        {
            int rawSize = Marshal.SizeOf(typeof(T));
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(item, buffer, false);
            byte[] rawData = new byte[rawSize];
            Marshal.Copy(buffer, rawData, 0, rawSize);
            Marshal.FreeHGlobal(buffer);
            return rawData;
        }



    }



}
