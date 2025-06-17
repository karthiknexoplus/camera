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
    public delegate void BufferFillEventHandler(IntPtr data, int size);
    public enum WaveFormats
    {
        Pcm = 1,
        Float = 3
    }
    [Flags]
    public enum WaveHeaderFlags
    {
        /// <summary>
        /// WHDR_BEGINLOOP
        /// This buffer is the first buffer in a loop.  This flag is used only with output buffers.
        /// </summary>
        BeginLoop = 0x00000004,
        /// <summary>
        /// WHDR_DONE
        /// Set by the device driver to indicate that it is finished with the buffer and is returning it to the application.
        /// </summary>
        Done = 0x00000001,
        /// <summary>
        /// WHDR_ENDLOOP
        /// This buffer is the last buffer in a loop.  This flag is used only with output buffers.
        /// </summary>
        EndLoop = 0x00000008,
        /// <summary>
        /// WHDR_INQUEUE
        /// Set by Windows to indicate that the buffer is queued for playback.
        /// </summary>
        InQueue = 0x00000010,
        /// <summary>
        /// WHDR_PREPARED
        /// Set by Windows to indicate that the buffer has been prepared with the waveInPrepareHeader or waveOutPrepareHeader function.
        /// </summary>
        Prepared = 0x00000002
    }
    //[StructLayout(LayoutKind.Sequential)]
    //public struct WAVEHDR
    //{
    //    public IntPtr lpData;                 /* pointer to locked data buffer */
    //    public int dwBufferLength;         /* length of data buffer */
    //    public int dwBytesRecorded;        /* used for input only */
    //    public IntPtr dwUser;                 /* for client's use */
    //    public WaveHeaderFlags dwFlags;                /* assorted flags (see defines) */
    //    public int dwLoops;                /* loop control counter */
    //    public IntPtr lpNext;     /* reserved for driver */
    //    public IntPtr reserved;               /* reserved for driver */

    //}
    [StructLayout(LayoutKind.Sequential)]
    public struct WaveHdr
    {
        public IntPtr lpData; // pointer to locked data buffer
        public int dwBufferLength; // length of data buffer
        public int dwBytesRecorded; // used for input only
        public IntPtr dwUser; // for client's use
        public int dwFlags; // assorted flags (see defines)
        public int dwLoops; // loop control counter
        public IntPtr lpNext; // PWaveHdr, reserved for driver
        public int reserved; // reserved for driver
    }
    //[StructLayout(LayoutKind.Sequential)]
    //public struct WAVEFORMATEX
    //{
    //    public short wFormatTag;
    //    public short nChannels;
    //    public int nSamplesPerSec;
    //    public int nAvgBytesPerSec;
    //    public short nBlockAlign;
    //    public short wBitsPerSample;
    //    public short cbSize;

    //    public WAVEFORMATEX(int rate, int bits, int channels)
    //    {
    //        wFormatTag = (short)WaveFormats.Pcm;
    //        nChannels = (short)channels;
    //        nSamplesPerSec = rate;
    //        wBitsPerSample = (short)bits;
    //        cbSize = 0;

    //        nBlockAlign = (short)(channels * (bits / 8));
    //        nAvgBytesPerSec = nSamplesPerSec * nBlockAlign;
    //    }
    //}
    [StructLayout(LayoutKind.Sequential)]
    public class WaveFormat
    {
        public short wFormatTag;
        public short nChannels;
        public int nSamplesPerSec;
        public int nAvgBytesPerSec;
        public short nBlockAlign;
        public short wBitsPerSample;
        public short cbSize;

        public WaveFormat(int rate, int bits, int channels)
        {
            wFormatTag = (short)WaveFormats.Pcm;
            nChannels = (short)channels;
            nSamplesPerSec = rate;
            wBitsPerSample = (short)bits;
            cbSize = 0;

            nBlockAlign = (short)(channels * (bits / 8));
            nAvgBytesPerSec = nSamplesPerSec * nBlockAlign;
        }
    }
    public class MyWaveOut
    {
        public const int MMSYSERR_NOERROR = 0; // no error

        public const int MM_WOM_OPEN = 0x3BB;
        public const int MM_WOM_CLOSE = 0x3BC;
        public const int MM_WOM_DONE = 0x3BD;

        public const int MM_WIM_OPEN = 0x3BE;
        public const int MM_WIM_CLOSE = 0x3BF;
        public const int MM_WIM_DATA = 0x3C0;

        public const int CALLBACK_FUNCTION = 0x00030000;    // dwCallback is a FARPROC 

        public const int TIME_MS = 0x0001;  // time in milliseconds 
        public const int TIME_SAMPLES = 0x0002;  // number of wave samples 
        public const int TIME_BYTES = 0x0004;  // current byte offset 

        private const string mmdll = "winmm.dll";
        [DllImport("winmm.dll")]
        public static extern int waveOutGetNumDevs();//Get the number of audio cards
        [DllImport("winmm.dll", EntryPoint = "waveOutGetDevCaps")]
        public static extern int waveOutGetDevCapsA(int uDeviceID, IntPtr lpCaps, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveOutPrepareHeader(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveOutUnprepareHeader(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveOutWrite(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);
        //[DllImport("winmm.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int waveOutOpen(out IntPtr hWaveOut, IntPtr uDeviceID, WaveFormat lpFormat, WaveDelegate dwCallback, IntPtr dwInstance, int dwFlags);
        //[DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]
        //public static extern uint waveOutOpen(ref IntPtr hWaveOut, ref int uDeviceID, ref WaveFormat lpFormat, delegateWaveOutProc dwCallback, IntPtr dwInstance, uint dwFlags);


        [DllImport(mmdll)]
        public static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, WaveFormat lpFormat, WaveDelegate dwCallback, int dwInstance, int dwFlags);
        [DllImport("winmm.dll")]
        public static extern int waveOutReset(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern int waveOutClose(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern int waveOutPause(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern int waveOutRestart(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hWaveOut, int dwVolume);
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

        [DllImport("winmm.dll")]
        //public static extern int waveOutGetPosition(IntPtr hWaveOut, out int lpInfo, int uSize);
        public static extern int waveOutGetPosition(IntPtr hWaveOut, out int time, int uSize);

        // WaveIn calls
        [DllImport(mmdll)]
        public static extern int waveInGetNumDevs();
        [DllImport(mmdll)]
        public static extern int waveInAddBuffer(IntPtr hwi, ref WaveHdr pwh, int cbwh);
        [DllImport(mmdll)]
        public static extern int waveInClose(IntPtr hwi);
        [DllImport(mmdll)]
        public static extern int waveInOpen(out IntPtr phwi, int uDeviceID, WaveFormat lpFormat, WaveDelegate dwCallback, int dwInstance, int dwFlags);
        [DllImport(mmdll)]
        public static extern int waveInPrepareHeader(IntPtr hWaveIn, ref WaveHdr lpWaveInHdr, int uSize);
        [DllImport(mmdll)]
        public static extern int waveInUnprepareHeader(IntPtr hWaveIn, ref WaveHdr lpWaveInHdr, int uSize);
        [DllImport(mmdll)]
        public static extern int waveInReset(IntPtr hwi);
        [DllImport(mmdll)]
        public static extern int waveInStart(IntPtr hwi);
        [DllImport(mmdll)]
        public static extern int waveInStop(IntPtr hwi);

        const int MAX_BUF = 30;
        const int AUDIO_BUF_LEN = 100 * 1024;

        private IntPtr m_hWaveOut;
        private WaveHdr m_Header;
        private WaveHdr[] m_WaveHeader = new WaveHdr[MAX_BUF];
        private GCHandle m_HeaderHandle;
        private GCHandle m_HeaderDataHandle;

        
        private int m_iBufIndex;

        private WaveFormat m_WaveFormat;

        //public delegate void delegateWaveOutProc(IntPtr hdrvr, int uMsg, IntPtr dwUser, WaveHdr wavhdr, IntPtr dwParam2);
        public delegate void WaveDelegate(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2);
        public WaveDelegate cb = null;

        private object waveOutLock;
        private void nCallBack(IntPtr hdrvr, int uMsg, IntPtr dwInstance, WaveHdr wavhdr, IntPtr dwReserved)
        {
            if (uMsg == MM_WOM_DONE)
            {
                try
                {
                    GCHandle h = (GCHandle)wavhdr.dwUser;
                    WaveOutBuffer buf = (WaveOutBuffer)h.Target;
                    //Interlocked.Decrement(ref queuedBuffers);
                    //buf.OnCompleted();
                }
                catch
                {
                }
            }
        }

        public MyWaveOut()
        {
            m_iBufIndex = 0;
            m_hWaveOut = IntPtr.Zero;
            waveOutLock = new object();
        }
        internal static void WaveOutProc(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2)
        {
            if (uMsg == MM_WOM_DONE)
            {
                try
                {
                    wavhdr.dwUser = IntPtr.Zero;
                    //GCHandle h = (GCHandle)wavhdr.dwUser;
                    //WaveOutBuffer buf = (WaveOutBuffer)h.Target;
                    //buf.OnCompleted();
                }
                catch
                {
                }
            }
        }
        public bool Start(WaveFormat wavf)
        {
            lock (waveOutLock)
            {
                m_WaveFormat = wavf;
                cb = WaveOutProc;
                int tempa = waveOutOpen(out m_hWaveOut, -1, m_WaveFormat, cb, 0, CALLBACK_FUNCTION);
                if (0 != tempa)
                {
                    m_hWaveOut = IntPtr.Zero;
                    return false;
                }
                //m_Thread = new Thread(new ThreadStart(ThreadProc));
                //m_Thread.Start();
                for (int i = 0; i < MAX_BUF; i++)
                {
                    m_WaveHeader[i].lpData = new IntPtr(102400);
                    m_WaveHeader[i].dwBufferLength = 102400;
                    m_WaveHeader[i].dwBytesRecorded = 0;
                    m_WaveHeader[i].dwLoops = 1;
                    m_WaveHeader[i].dwUser = IntPtr.Zero;
                    m_WaveHeader[i].dwFlags = 1;
                    //m_WaveHeader[i].dwFlags = WaveHeaderFlags.Done;
                    waveOutPrepareHeader(m_hWaveOut, ref m_WaveHeader[i], Marshal.SizeOf(m_Header));
                }
            }
            return true;
        }
        public void WriteBuf(IntPtr pbyte, int len)
        {
            lock (waveOutLock)
            {
                if (m_hWaveOut == IntPtr.Zero || len > m_WaveFormat.nAvgBytesPerSec)
                    return;

                if (m_WaveHeader[m_iBufIndex].dwUser == IntPtr.Zero)
                {
                    m_WaveHeader[m_iBufIndex].lpData = pbyte;
                    m_WaveHeader[m_iBufIndex].dwBufferLength = len;
                    m_WaveHeader[m_iBufIndex].dwUser = new IntPtr(1);
                    //m_WaveHeader[m_iBufIndex].dwFlags = 18;
                    int tempInt = waveOutWrite(m_hWaveOut, ref m_WaveHeader[m_iBufIndex], Marshal.SizeOf(typeof(WaveHdr)));   //播放声音             
                    m_iBufIndex++;
                    if (m_iBufIndex >= MAX_BUF)
                        m_iBufIndex = 0;
                }
            }                      
        }
        private bool m_Pause;
        private bool m_Finished;
        private BufferFillEventHandler m_FillProc;
        private WaveOutBuffer m_Buffers; // linked list
        private WaveOutBuffer m_CurrentBuffer;
        private Thread m_Thread;
        private byte m_zero;
        private void ThreadProc()
        {
            m_Pause = false;
            while (!m_Finished)
            {
                // pause
                while (m_Pause)
                {
                    Thread.Sleep(10);
                }

                Advance();
                if (m_FillProc != null && !m_Finished)
                    m_FillProc(m_CurrentBuffer.Data, m_CurrentBuffer.Size);
                else if (m_CurrentBuffer != null)
                {
                    // zero out buffer
                    byte v = m_zero;
                    byte[] b = new byte[m_CurrentBuffer.Size];
                    for (int i = 0; i < b.Length; i++)
                        b[i] = v;
                    Marshal.Copy(b, 0, m_CurrentBuffer.Data, b.Length);
                }

                if (m_CurrentBuffer != null)
                {
                    m_CurrentBuffer.Play();
                }
            }
            WaitForAllBuffers();
        }
        private void Advance()
        {
            m_CurrentBuffer = m_CurrentBuffer == null ? m_Buffers : m_CurrentBuffer.NextBuffer;
            if (m_CurrentBuffer != null)
            {
                m_CurrentBuffer.WaitFor();
            }
        }
        private void WaitForAllBuffers()
        {
            WaveOutBuffer Buf = m_Buffers;
            while (m_Buffers != null && Buf.NextBuffer != m_Buffers)
            {
                Buf.WaitFor();
                Buf = Buf.NextBuffer;
            }
        }
        public bool SetVolume(int dwVolume)
        {
            if ((0 <= dwVolume) && (dwVolume <= 100))
            {
                if (m_hWaveOut != IntPtr.Zero)
                {
                    waveOutSetVolume(m_hWaveOut, dwVolume);
                    return true;
                }
            }
            return false;
        }
        public int GetVolume()
        {
            int dwVolume;
            if (m_hWaveOut != IntPtr.Zero)
            {
                if (MMSYSERR_NOERROR == waveOutGetVolume(m_hWaveOut, out dwVolume))
                {
                    return dwVolume;
                }
            }
            return 0;
        }
        public void SetPlaybackRate(int dwRate)
        {

        }
        public void Stop()
        {
            if (m_hWaveOut != IntPtr.Zero)
            {
                waveOutReset(m_hWaveOut);
                waveOutUnprepareHeader(m_hWaveOut, ref m_Header, Marshal.SizeOf(m_Header));
                //m_HeaderHandle.Free();
                m_Header.lpData = IntPtr.Zero;

		        waveOutClose(m_hWaveOut);
		        m_hWaveOut = IntPtr.Zero;
            }		    	
        }


        
    }
    internal class WaveOutHelper
    {
        public static void Try(int err)
        {
            if (err != MyWaveOut.MMSYSERR_NOERROR)
                throw new Exception(err.ToString());
        }
    }
    

    internal class WaveOutBuffer : IDisposable
    {
        public WaveOutBuffer NextBuffer;

        private AutoResetEvent m_PlayEvent = new AutoResetEvent(false);
        private IntPtr m_WaveOut;

        private WaveHdr m_Header;
        private byte[] m_HeaderData;
        private GCHandle m_HeaderHandle;
        private GCHandle m_HeaderDataHandle;

        private bool m_Playing;

        public WaveOutBuffer(IntPtr waveOutHandle, int size)
        {
            m_WaveOut = waveOutHandle;

            m_HeaderHandle = GCHandle.Alloc(m_Header, GCHandleType.Pinned);
            m_Header.dwUser = (IntPtr)GCHandle.Alloc(this);
            m_HeaderData = new byte[size];
            m_HeaderDataHandle = GCHandle.Alloc(m_HeaderData, GCHandleType.Pinned);
            m_Header.lpData = m_HeaderDataHandle.AddrOfPinnedObject();
            m_Header.dwBufferLength = size;
            WaveOutHelper.Try(MyWaveOut.waveOutPrepareHeader(m_WaveOut, ref m_Header, Marshal.SizeOf(m_Header)));
        }
        ~WaveOutBuffer()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (m_Header.lpData != IntPtr.Zero)
            {
                MyWaveOut.waveOutUnprepareHeader(m_WaveOut, ref m_Header, Marshal.SizeOf(m_Header));
                m_HeaderHandle.Free();
                m_Header.lpData = IntPtr.Zero;
            }
            m_PlayEvent.Close();
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

        public bool Play()
        {
            lock (this)
            {
                m_PlayEvent.Reset();
                m_Playing = MyWaveOut.waveOutWrite(m_WaveOut, ref m_Header, Marshal.SizeOf(m_Header)) == MyWaveOut.MMSYSERR_NOERROR;
                return m_Playing;
            }
        }
        public void WaitFor()
        {
            if (m_Playing)
            {
                m_Playing = m_PlayEvent.WaitOne();
            }
            else
            {
                Thread.Sleep(0);
            }
        }
        public void OnCompleted()
        {
            m_PlayEvent.Set();
            m_Playing = false;
        }
    }
}
