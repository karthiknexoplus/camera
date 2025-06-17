using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevSdkByCS;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Demo
{
    public enum WAVE_IN_STATE
    {
        ERRNO_WAVEIN_OK = 0x01,
        ERRNO_WAVEIN_NO_MEM,
        ERRNO_WAVEIN_OPEN,
        ERRNO_WAVEIN_NOTOPEN,
        ERRNO_WAVEIN_PREPARE,
        ERRNO_WAVEIN_UNPREPARE,
        ERRNO_WAVEIN_RESET,
        ERRNO_WAVEIN_CLOSE,
        ERRNO_WAVEIN_ALREADYOPEN,
        ERRNO_WAVEIN_ADDBUFFER,
        ERRNO_WAVEIN_START,
        ERRNO_WAVEIN_STOP,
        ERRNO_WAVEIN_THREAD_NOT_END
    };
    public delegate void BufferDoneEventHandler(IntPtr data, int size);
    class MyWaveIn
    {
        const int WAVEIN_BUFF_LEN_DEF = 3200;
        const int WAVEIN_BUFF_NUM_DEF = 4;
        //初始化音频采集设备所使用的结构体WaveFormatEx的缺省值
        const int WAVEIN_FMT_AVGBYTEPERSEC_DEF = 16000;
        const int WAVEIN_FMT_BLOCKALIGN_DEF = 2;
        const int WAVEIN_FMT_CHANNELS_DEF = 1;
        const int WAVEIN_FMT_SAMPLESPERSEC_DEF = 8000;
        const int WAVEIN_FMT_BITSPERSAMPLE_DEF = 16;


        private int m_nBuffSize;
        private int m_nBuffNum;

        //语音数据回调
        BufferDoneEventHandler m_pTalkDataCallBack;//m_DoneProc
        //void* m_pTalkDataCBParam;
        WaveHdr m_pWaveHeader;
        private IntPtr m_hWaveIn;
        //	WAVEFORMATEX		WaveFormat;

        int m_ThreadHandle;	////获取Mic音频数据的线程句柄
        int m_ThreadID;		///////线程的ID
        bool m_bStartRec;	//////线程的运行标记

    }
    internal class WaveInHelper
    {
        public static void Try(int err)
        {
            if (err != MyWaveOut.MMSYSERR_NOERROR)
                throw new Exception(err.ToString());
        }

    }
    internal class WaveInBuffer : IDisposable
    {
        public WaveInBuffer NextBuffer;

        private AutoResetEvent m_RecordEvent = new AutoResetEvent(false);
        private IntPtr m_WaveIn;

        private WaveHdr m_Header;
        private byte[] m_HeaderData;
        private GCHandle m_HeaderHandle;
        private GCHandle m_HeaderDataHandle;

        private bool m_Recording;

        //internal static void WaveInProc(IntPtr hdrvr, int uMsg, IntPtr dwUser, WaveHdr wavhdr, IntPtr dwParam2)
        internal static void WaveInProc(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2)
        {
            if (uMsg == MyWaveOut.MM_WIM_DATA)
            {
                try
                {
                    GCHandle h = (GCHandle)wavhdr.dwUser;
                    WaveInBuffer buf = (WaveInBuffer)h.Target;
                    buf.OnCompleted();
                }
                catch
                {
                }
            }
        }

        public WaveInBuffer(IntPtr waveInHandle, int size)
        {
            m_WaveIn = waveInHandle;

            m_HeaderHandle = GCHandle.Alloc(m_Header, GCHandleType.Pinned);
            m_Header.dwUser = (IntPtr)GCHandle.Alloc(this);
            m_HeaderData = new byte[size];
            m_HeaderDataHandle = GCHandle.Alloc(m_HeaderData, GCHandleType.Pinned);
            m_Header.lpData = m_HeaderDataHandle.AddrOfPinnedObject();
            m_Header.dwBufferLength = size;
            WaveInHelper.Try(MyWaveOut.waveInPrepareHeader(m_WaveIn, ref m_Header, Marshal.SizeOf(m_Header)));
        }
        ~WaveInBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_Header.lpData != IntPtr.Zero)
            {
                MyWaveOut.waveInUnprepareHeader(m_WaveIn, ref m_Header, Marshal.SizeOf(m_Header));
                m_HeaderHandle.Free();
                m_Header.lpData = IntPtr.Zero;
            }
            m_RecordEvent.Close();
            if (m_HeaderDataHandle.IsAllocated)
                m_HeaderDataHandle.Free();
            GC.SuppressFinalize(this);
        }

        public int Size
        {
            get { return m_Header.dwBufferLength; }
        }

        public IntPtr Data
        {
            get { return m_Header.lpData; }
        }

        public bool Record()
        {
            lock (this)
            {
                m_RecordEvent.Reset();
                m_Recording = MyWaveOut.waveInAddBuffer(m_WaveIn, ref m_Header, Marshal.SizeOf(m_Header)) == MyWaveOut.MMSYSERR_NOERROR;
                return m_Recording;
            }
        }

        public void WaitFor()
        {
            if (m_Recording)
                m_Recording = m_RecordEvent.WaitOne();
            else
                Thread.Sleep(0);
        }

        private void OnCompleted()
        {
            m_RecordEvent.Set();
            m_Recording = false;
        }
    }

    public class WaveInRecorder : IDisposable
    {
        private IntPtr m_WaveIn;
        private WaveInBuffer m_Buffers; // linked list
        private WaveInBuffer m_CurrentBuffer;
        private Thread m_Thread;
        private BufferDoneEventHandler m_DoneProc;
        private bool m_Finished;

        //public delegate void WaveDelegate(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2);
        public MyWaveOut.WaveDelegate cb = null;
        //private MyWaveOut.WaveDelegate m_BufferProc = new MyWaveOut.WaveDelegate(WaveInBuffer.WaveInProc);

        public static int DeviceCount
        {
            get { return MyWaveOut.waveInGetNumDevs(); }
        }

        public WaveInRecorder(int device, WaveFormat format, int bufferSize, int bufferCount, BufferDoneEventHandler doneProc)
        {
            cb = WaveInBuffer.WaveInProc;
            m_DoneProc = doneProc;
            WaveInHelper.Try(MyWaveOut.waveInOpen(out m_WaveIn, device, format, cb, 0, MyWaveOut.CALLBACK_FUNCTION));
            AllocateBuffers(bufferSize, bufferCount);
            for (int i = 0; i < bufferCount; i++)
            {
                SelectNextBuffer();
                m_CurrentBuffer.Record();
            }
            WaveInHelper.Try(MyWaveOut.waveInStart(m_WaveIn));
            m_Thread = new Thread(new ThreadStart(ThreadProc));
            m_Thread.Start();
        }
        ~WaveInRecorder()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (m_Thread != null)
                try
                {
                    m_Finished = true;
                    if (m_WaveIn != IntPtr.Zero)
                        MyWaveOut.waveInReset(m_WaveIn);
                    WaitForAllBuffers();
                    m_Thread.Join();
                    m_DoneProc = null;
                    FreeBuffers();
                    if (m_WaveIn != IntPtr.Zero)
                        MyWaveOut.waveInClose(m_WaveIn);
                }
                finally
                {
                    m_Thread = null;
                    m_WaveIn = IntPtr.Zero;
                }
            GC.SuppressFinalize(this);
        }
        private void ThreadProc()
        {
            while (!m_Finished)
            {
                Advance();
                if (m_DoneProc != null && !m_Finished)
                    m_DoneProc(m_CurrentBuffer.Data, m_CurrentBuffer.Size);
                m_CurrentBuffer.Record();
            }
        }
        private void AllocateBuffers(int bufferSize, int bufferCount)
        {
            FreeBuffers();
            if (bufferCount > 0)
            {
                m_Buffers = new WaveInBuffer(m_WaveIn, bufferSize);
                WaveInBuffer Prev = m_Buffers;
                try
                {
                    for (int i = 1; i < bufferCount; i++)
                    {
                        WaveInBuffer Buf = new WaveInBuffer(m_WaveIn, bufferSize);
                        Prev.NextBuffer = Buf;
                        Prev = Buf;
                    }
                }
                finally
                {
                    Prev.NextBuffer = m_Buffers;
                }
            }
        }
        private void FreeBuffers()
        {
            m_CurrentBuffer = null;
            if (m_Buffers != null)
            {
                WaveInBuffer First = m_Buffers;
                m_Buffers = null;

                WaveInBuffer Current = First;
                do
                {
                    WaveInBuffer Next = Current.NextBuffer;
                    Current.Dispose();
                    Current = Next;
                } while (Current != First);
            }
        }
        private void Advance()
        {
            SelectNextBuffer();
            m_CurrentBuffer.WaitFor();
        }
        private void SelectNextBuffer()
        {
            m_CurrentBuffer = m_CurrentBuffer == null ? m_Buffers : m_CurrentBuffer.NextBuffer;
        }
        private void WaitForAllBuffers()
        {
            WaveInBuffer Buf = m_Buffers;
            while (Buf.NextBuffer != m_Buffers)
            {
                Buf.WaitFor();
                Buf = Buf.NextBuffer;
            }
        }
    }
}
