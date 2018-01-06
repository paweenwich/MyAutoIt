using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MyAutoIt.Utils;

namespace MyAutoIt
{
    public class CENet
    {
        public static byte CMD_OPENPROCESS = 3;
        public static byte CMD_READPROCESSMEMORY = 9;
        public static byte CMD_WRITEPROCESSMEMORY = 10;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct OPCommand
        {
            public byte command;
            public int pid;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ReadProcessMemoryCommand
        {
            public byte command;
            public int handle;
            public ulong address;
            public int size;
            public byte compress;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WriteProcessMemoryCommand
        {
            public byte command;
            public int handle;
            public ulong address;
            public int size;
        }

        /*
        #define CMD_GETVERSION 0
        #define CMD_CLOSECONNECTION 1
        #define CMD_TERMINATESERVER 2

        #define CMD_CREATETOOLHELP32SNAPSHOT 4
        #define CMD_PROCESS32FIRST 5
        #define CMD_PROCESS32NEXT 6
        #define CMD_CLOSEHANDLE 7
        #define CMD_VIRTUALQUERYEX 8
        
        #define CMD_STARTDEBUG 11
        #define CMD_STOPDEBUG 12
        #define CMD_WAITFORDEBUGEVENT 13
        #define CMD_CONTINUEFROMDEBUGEVENT 14
        #define CMD_SETBREAKPOINT 15
        #define CMD_REMOVEBREAKPOINT 16
        #define CMD_SUSPENDTHREAD 17
        #define CMD_RESUMETHREAD 18
        #define CMD_GETTHREADCONTEXT 19
        #define CMD_SETTHREADCONTEXT 20
        #define CMD_GETARCHITECTURE 21
        #define CMD_MODULE32FIRST 22
        #define CMD_MODULE32NEXT 23
        */
        public int port = 52736;
        public String host = "127.0.0.1";
        public int handle;
        public TcpClient client;
        public NetworkStream ns;
        public bool Connect()
        {
            client = new TcpClient(host, port);
            ns = client.GetStream();
            return ns != null;
        }

        public void Close()
        {
            if (ns != null)
            {
                ns.Close();
                client.Close();
            }
        }

        public int NetReadInt()
        {
            byte[] recvData = new byte[4];
            ns.Read(recvData, 0, recvData.Length);
            return Utils.ByteArrayToInt(recvData);
        }
        public byte[] NetReadByteArray(int size)
        {
            byte[] recvData = new byte[size];
            ns.Read(recvData, 0, recvData.Length);
            return recvData;
        }

        public int OpenProcess(int processID)
        {
            if (ns != null)
            {
                OPCommand op = new OPCommand();
                op.command = CMD_OPENPROCESS;
                op.pid = processID;

                RawSerializer<OPCommand> rs = new RawSerializer<OPCommand>();
                byte[] data = rs.RawSerialize(op);
                ns.Write(data, 0, data.Length);

                handle = NetReadInt();
                Console.WriteLine(String.Format("Handle={0}", handle));
                return handle;
            }
            else
            {
                return 0;
            }
        }

        public byte[] ReadProcessMemory(ulong address, int size)
        {
            if (ns != null)
            {
                ReadProcessMemoryCommand cmd = new ReadProcessMemoryCommand();
                cmd.command = CMD_READPROCESSMEMORY;
                cmd.address = address;
                cmd.handle = handle;
                cmd.size = size;
                cmd.compress = 0;
                RawSerializer<ReadProcessMemoryCommand> rs = new RawSerializer<ReadProcessMemoryCommand>();
                byte[] data = rs.RawSerialize(cmd);
                ns.Write(data, 0, data.Length);
                int resultSize = NetReadInt();

                byte[] recvData = NetReadByteArray(resultSize);
                return recvData;
            }
            else
            {
                return null;
            }
        }

        public int WriteProcessMemory(ulong address,byte[] writeData)
        {
            if (ns != null)
            {
                WriteProcessMemoryCommand cmd = new WriteProcessMemoryCommand();
                cmd.command = CMD_WRITEPROCESSMEMORY;
                cmd.address = address;
                cmd.handle = handle;
                cmd.size = writeData.Length;
                RawSerializer<WriteProcessMemoryCommand> rs = new RawSerializer<WriteProcessMemoryCommand>();
                byte[] data = rs.RawSerialize(cmd);
                ns.Write(data, 0, data.Length);
                ns.Write(writeData, 0, writeData.Length);

                int numWrite = NetReadInt();
                return numWrite;
            }
            else
            {
                return 0;
            }
        }

        public int ReadInt(ulong address)
        {
            return Utils.ByteArrayToInt(ReadProcessMemory(address, 4));
        }

        public int WriteInt(ulong address, int value)
        {
            return WriteProcessMemory(address, Utils.IntToByteArray(value));
        }
    }

}
