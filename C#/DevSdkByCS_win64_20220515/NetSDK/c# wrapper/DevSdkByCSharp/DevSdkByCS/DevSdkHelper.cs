using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DevSdkByCS
{
    using System.Net;
#if OS64
    using POINTERHANDLE = Int64;
#else
    using POINTERHANDLE = Int32;
#endif
    #region 委托
    public delegate void EXCEPTION_CALLBACK(UInt32 dwType, Int32 lUserID, Int32 lHandle, IntPtr pUser);
    public delegate void LIVE_DATA_CALLBACK(int lLiveHandle, NET_SDK_FRAME_INFO frameInfo, IntPtr pBuffer, IntPtr pUser);

    public delegate void LIVE_DATA_CALLBACK_EX(int lLiveHandle, uint dataType, IntPtr pBuffer,uint dataLen, IntPtr pUser);

    //public delegate void NET_MESSAGE_CALLBACK(Int32 lCommand, Int32 lUserID, String pBuf, UInt32 dwBufLen, IntPtr pUser);
    public delegate void NET_MESSAGE_CALLBACK(Int32 lCommand, Int32 lUserID, IntPtr pBuf, UInt32 dwBufLen, IntPtr pUser);
    public delegate void NET_MESSAGE_CALLBACK_EX(Int32 lCommand, Int32 lUserID, IntPtr pBuf, UInt32 dwBufLen, IntPtr pUser);
    //public delegate void ACCEPT_REGISTER_CALLBACK(Int32 lUserID, Int32 lRegisterID, ref NET_SDK_DEVICEINFO pDeviceInfo, IntPtr pUser);
    public delegate void ACCEPT_REGISTER_CALLBACK(Int32 lUserID, Int32 lRegisterID, IntPtr pDeviceInfo, IntPtr pUser);
    public delegate void DRAW_FUN_CALLBACK(int lLiveHandle, IntPtr hDC, IntPtr pUser);
    public delegate void BACKUP_DATA_CALLBACK(POINTERHANDLE lFileHandle, UInt32 dataType, IntPtr pBuffer, UInt32 dataLen,  IntPtr pUser);
    public delegate void SUBSCRIBE_CALLBACK(Int32 lUserID, Int32 dwCommand, IntPtr pBuf, UInt32 dwBufLen, IntPtr pUser);
    public delegate void PLAY_DATA_CALLBACK(int lLiveHandle, NET_SDK_FRAME_INFO frameInfo, IntPtr pBuffer, IntPtr pUser);
    public delegate void PLAY_YUV_DATA_CALLBACK(int lLiveHandle, DECODE_FRAME_INFO frameInfo, IntPtr pUser);
    public delegate void TALK_DATA_CALLBACK(int lVoiceComHandle, IntPtr pRecvDataBuffer, int dwBufSize, byte byAudioFlag, IntPtr pUser);
    public delegate void YUV_DATA_CALLBACK(int lLiveHandle, DECODE_FRAME_INFO frameInfo, IntPtr pUser);

    #endregion 委托

    #region 结构体，枚举定义


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FRAME_INFO
    {
        public UInt32 deviceID;
        public UInt32 channel;
        public UInt32 frameType;	//refer to DD_FRAME_TYPE
        public UInt32 length;
        public UInt32 keyFrame;	//0：not key frame 1：key frame
        public UInt32 width;
        public UInt32 height;
        public UInt32 frameIndex;
        public UInt32 frameAttrib;//refer to DD_FRAME_ATTRIB
        public UInt32 streamID;
        public Int64 time;	   //total time since 1970.1.1 00:00:00 (microsecond)
        public Int64 relativeTime;//relativeTime(microsecond)
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SDK_FRAME_INFO
    {
	    public UInt32 nLength;	//帧数据长度 the length of the frame
        public UInt32 frameType;	//参考DD_FRAME_TYPE refer to DD_FRAME_TYPE
        public UInt32 keyFrame;	//0：非关键帧 1：关键帧 0:not key frame 1:key frame
        public UInt32 nWidth;		//画面宽, 单位像素, 如果是音频数据则为0 the width of the image, be 0 if the frame is audio
        public UInt32 nHeight;	//画面高, 如果是音频数据则为0 the height of the image, be 0 if the frame is audio
        public Int64 nStamp;		//帧时间, 单位毫秒, 从1970年1月1日0点0分0秒开始的绝对时间 the frame's time stamp
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_DEVICEINFO
    {
        public byte localVideoInputNum;		//本地视频输入通道数目 number of local Video Input
        public byte audioInputNum;			//音频输入通道数目 number of audio Input
        public byte sensorInputNum;			//传感器输入通道数目 number of sensor Input
        public byte sensorOutputNum;		//继电器输出数目 number of sensor Output
        public UInt32 displayResolutionMask;  //监视器可选择的分辨率 option of displayed resolution

        public byte videoOuputNum;			//视频输出数目（及支持回放最大通道数）the number of the video output
        public byte netVideoOutputNum;		//网络回放最大通道数目 the maxium number of video playback
        public byte netVideoInputNum;		//数字信号接入通道数目 the number of the net video input
        public byte IVSNum;					//智能分析通道数目 the number of the intelligent anlysis video 

        public byte presetNumOneCH;			//每个通道预置点数目 the number of the preset per channel
        public byte cruiseNumOneCH;			//每个通道巡航线数目 the number of the cruise per channel
        public byte presetNumOneCruise;		//每个巡航线的预置点数目 the number of the preset per cruise
        public byte trackNumOneCH;			//每个通道轨迹数目 the number of the track per channel

        public byte userNum;				//用户数目 user number
        public byte netClientNum;			//最多客户端数目 the maxium number of client
        public byte netFirstStreamNum;		//主码流传输的通道最大数目，即同时可以有几个客户端查看主码流 the maxium number of first stream can be request
        public byte deviceType;				// NET_SDK_DEVICE_TYPE 0:DVR 1:DVS 2:IPC 3:NVR 4:SUPERDVR 5:decoder

        public byte doblueStream;			//是否有提供双码流 1:support double stream 0:not support
        public byte audioStream;			//是否有提供音频流 1:support audio stream 0:not support
        public byte talkAudio;				//是否有对讲功能: 1:表示有对讲功能;0表示没有 1:support talk 0:not support
        public byte bPasswordCheck;			//操作是否要输入密码 1:check password 0:not check

        public byte defBrightness;			//缺省亮度 default brightness value
        public byte defContrast;			//缺省对比度 default contrast value
        public byte defSaturation;			//缺省饱和度 default saturation value
        public byte defHue;					//缺省色调 default hue value

        public UInt16 videoInputNum;			//视频输入通道数目（本地加网络）the number of video input(analog and digital)
        public UInt16 deviceID;				//设备ID号 device id
        public UInt32 videoFormat;            //系统当前制式 video format


        //假如是FUNC_REMOTE_UPGRADE对应的功能，那么第FUNC_REMOTE_UPGRADE个比特为1，否则为零。
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 8)]
        public UInt32[] function;			//功能描述,  第0个int要转为二进制数据，每一位对应不同的功能，具体看intelist   convert the first item to binary,each bit means different function

        public UInt32 deviceIP;				//设备网络地址 device ip
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 6)]
        public byte[] deviceMAC;			//设备物理地址 device mac address
        public UInt16 devicePort;				//设备端口 device port

        public UInt32 buildDate;				//创建日期:year<<16 + month<<8 + mday
        public UInt32 buildTime;				//创建时间:hour<<16 + min<<8 + sec

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
        public byte[] deviceName;			//设备名称 device name

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
        public byte[] firmwareVersion;			//固件版本 firmware version
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
        public byte[] kernelVersion;				//内核版本 kernel version
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
        public byte[] hardwareVersion;			//硬件版本 hardware version
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
        public byte[] MCUVersion;					//单片机版本 MCU version
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
        public byte[] firmwareVersionEx;			//固件版本扩展，主要针对新产品 external firmware version

        public UInt32 softwareVer;			//软件协议版本，目前用于IPC software version
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
        public byte[] szSN;			//sn

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 28)]
        public byte[] deviceProduct;			//设备型号 model of the device


        public IPAddress GetDeviceIP()
        {
            return new IPAddress(deviceIP);   
        }
        public string GetDeviceIPStr()
        {
            return DevSdkHelper.IntToIp(deviceIP);
        }

        public string GetDeviceMAC()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < deviceMAC.Length; i++)
            {
                string str = deviceMAC[i].ToString("X");
                str = str.Length == 1 ? "0" + str : str;
                if (i < 5)
                    sb.Append(str + ":");
                else
                    sb.Append(str);
            }            
            return sb.ToString();

        }

        public string GetDeviceName()
        {
            return DevSdkHelper.ByteToStr(deviceName);
        }
        public string GetFirmwareVersion()
        {
            return DevSdkHelper.ByteToStr(firmwareVersion);
        }
        public string GetKernelVersion()
        {
            return DevSdkHelper.ByteToStr(kernelVersion);
        }
        public string GetHardwareVersion()
        {
            return DevSdkHelper.ByteToStr(hardwareVersion);
        }
        public string GetMCUVersion()
        {
            return DevSdkHelper.ByteToStr(MCUVersion);
        }
        public string GetFirmwareVersionEx()
        {
            return DevSdkHelper.ByteToStr(firmwareVersionEx);
        }
        public string GetDeviceProduct()
        {
            return DevSdkHelper.ByteToStr(deviceProduct);
        }

    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_DEVICEINFO_EX
    {
        public NET_SDK_DEVICEINFO devInfo;
        public int userId;
        public string devIP;
        public int chl;//-1:device itself 0:channel 1;1:channel 2.......
        public bool online;
        public bool isManualRecording;
        public string btn_ManualRecord_txt;
        public string devName;
        public int registerId;
        public bool subscribVFD;
        public bool subscribVFDMatch;
        public byte[] addressVFD;
        public byte[] addressVFDMatch;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IMAGE_EFFECT_T
    {
        public UInt32 minValue;             // 最小值 minimum value
        public UInt32 maxValue;             // 最大值 maximum value
        public UInt32 curValue;             // 当前值 current value
        public UInt32 defaultValue;          // 默认值default value

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_DEVICE_INFO
    { 
        public UInt32	iSize;		//本结构体长度 this struct's size

	    public UInt32	deviceID;	//设备ID(0~255) device id
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
        public byte[] deviceNo;		//设备序列号，可能使用字母 device number

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
        public byte[] deviceName;		//设备名称 device name
        
	
	    //以下为只读项
	    //version
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
        public byte[] firmwareVersion ;  //firmware version
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
	    public byte[] firmwareBuildDate ; //firmware build date
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
	    public byte[] hardwareVersion ;  //hardware version
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
	    public byte[] kernelVersion ; //kernel version
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
	    public byte[] mcuVersion; //mcu version

	    //接口参数
	    public byte	audioNum; //the number of audio
	    public byte	localVideoInNum;//the number of local video input
	    public byte	netVideoInNum; //the number of net video input
	    public byte	sensorInNum; //the number of sensor
	    public byte	relayOutNum; //the number of relay out

	    public byte	rs232Num; //the number of rs232 port
	    public byte	rs485Num; //the number of rs485 port
	    public byte	networkPortNum;// the number of network port 
	    public byte	diskCtrlNum; // the number of disk control
	    public byte	DiskNum;//the maximum of disk
	    public byte	vgaNum; //the number of vga port
	    public byte	usbNum; //the number of usb port
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IMAGE_SREACH
    {
        public UInt32 dwChannel;//抓图通道（从0开始） search channel
        public DD_TIME_EX StartTime; //时间 search start time 
        public DD_TIME_EX StopTime; //时间 search stop time
        public UInt32 pageIndex;//第几页 search result page index
        public UInt32 pageSize;//每页数 search results number per page
        public IMAGE_SORT_TYPE sort; //返回排列顺序 sort
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public byte[] resv; //reserve
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IMAGE
    { 
        public UInt32 dwChannel; //抓图通道 channel index
        public UInt32 dwImageType; //抓图类型 IMAGE_EVENT_TYPE image type
        public DD_TIME_EX captureTime;//时间 capture time
        public UInt32 totalNum;//图片总数  total number of image
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public byte[] resv; //reserve
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IMAGE_EX
    {
        public int userId;//user id
        public NET_SDK_IMAGE nsi; //
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IMAGE_INFO
    {
        public UInt32 imageSize; //image size
        public UInt32 imageMode;//IMAGE_MODE 图片格式 image mode
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public byte[] resv;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_BASIC_CONFIG
    {
        public UInt32 iSize;				//本结构体长度 // this struct's size
        public UInt32 videoFormat;			//视频制式，取值参考DD_VIDEO_FORMAT video format, refer to DD_VIDEO_FORMAT

        public UInt32 videoOut;				//视频输出设备 video out 
        public UInt32 videoOutResolution;	//输出分辨率，取值参考DD_VGA_RESOLUTION video out resolution, refer to DD_VGA_RESOLUTION
        public UInt32 VGARefresh;			//VGA刷新率the frequency of vga refresh
        public UInt32 screensaver;			//屏保时间（零表示不开启） screen saver time, 0 means close

        public UInt32 deviceLanguage;		//系统语言 device language
        public UInt32 passwordCheck;		//是否开启密码检查 password check 

        public UInt32 RecycleRecord;		//是否允许覆盖录像 recycle record

        //以下为一些只读项
        public UInt32 videoFormatMask;			//支持的视频制式掩码（只读），取值为DD_VIDEO_FORMAT定义数据的掩码组合 video format mask(read only)
        public UInt32 videoOutMask;				//支持的视频输出设备掩码（只读） video out mask(read only)
        public UInt32 videoOutResolutionMask;	//支持的视频输出设备分辨率掩码（只读），取值为DD_VGA_RESOLUTION定义数据的掩码组合 video out resolution mask(read only)
        public UInt32 languageMask;				//支持的语言掩码（只读） language mask(read only)
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVE_BASE_INFO
    {
        public Int64 i64SnapTime;               // 抓拍时间. snap time
	    public UInt32 iSnapPicId;                // 抓拍ID. snap picture id
	    public int iSimilarity;               // 人脸比对相似度.(0-100) similarity
        public int iPersonId;                 // 人员ID. the person's id
	    public int iType;                     // 名单类型 0:陌生人 1:白名单 2:黑名单. 0:stranger 1:white list 2: black list
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)] 
	    public byte[] szName;               // 姓名. name
	    public int iMale;                     // 性别 1:male 0:female.
	    public int iAge;                      // 年龄. age
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)] 
	    public byte[] szIdentifyNum;// 身份识别号.    identify number
	    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)] 
	    public byte[] szTel;                 // 电话. telphone number
	    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)] 
	    public byte[] szRes;                // reserve 
	    public int iSnapPicQuality;           // 抓拍人脸质量. snap picture's quality
	    public int iSnapPicAge;               // 抓拍人脸年龄. snap picture's age
	    public int iSnapPicSex;               // 抓拍人脸性别. snap picture's sex

        public byte livingBody;                     // 是否是活体 1活体,0非活 1:living body 0:not
        public byte comparisonRes;                  // 比对结果 1成功,0失败 comparision result 1:success 0:failed

        public byte wearmask;                  // 是否戴口罩:0是未检测，1是未戴口罩 2是戴口罩 if ware mask 0:not detect 1:not wear 2 wear mask
        public byte tempUnitsType;             // 温度单位 0摄氏度 1华氏度 temperature unit type 0:celsius 1:Fahrenheit

        public int temperature;                    // 体温 temperature
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)] 
        public byte[] keyID;                  //人脸库主键 key id

	    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 20)] 
	    public byte[] szReserve; //reserve
    }
    public struct NET_SDK_IVE_PICTURE_INFO
    {
        public int iWidth;                    // 图片宽. picture's width
        public int iHeight;                   // 图片高. picture's height
        public int iPicFormat;                // 图片格式. picture's format
        public int iPicSize;                  // 图片大小. picture's size
    }
    public enum IMAGE_SORT_TYPE
    {
        IMAGE_SORT_ASC,// ascending order 
        IMAGE_SORT_DESC,//descending order
    }
    //图片类型
    public enum IMAGE_MODE
    {
        IMAGE_MODE_JPG,//jpg
        IMAGE_MODE_PNG,//png
        IMAGE_MODE_BMP,//bmp
    }

    public enum NET_SDK_CONNECT_TYPE
    {
        NET_SDK_CONNECT_TCP = 0,			//TCP方式登陆 tcp
        NET_SDK_CONNECT_NAT = 1,			//NAT方式登陆 nat
        NET_SDK_CONNECT_NAT20 = 2,			//用NAT2.0方式登陆 nat  
        NET_SDK_CONNECT_END
    }

    // 智能事件类型
    public enum NET_DVR_SMART_TYPE
    {
        NET_DVR_SMART_AVD,            // 视频异常诊断功能检测 abnormal video detection
        NET_DVR_SMART_VFD,            // 人脸检测 video face detection
        NET_DVR_SMART_VFD_MATCH,    //人脸比对 video face match detection
        NET_DVR_SMART_PEA,                  // 区域入侵 Region intrusion
        NET_DVR_SMART_OSC,                  // 物品遗留及丢失 Object removal
        NET_DVR_SMART_CPC,                  // 人流量统计 People counting
        NET_DVR_SMART_CDD,                  // 人群密度检测 Crowd density detection
        NET_DVR_SMART_IPD,                  // 人员入侵侦测 People intrusion
        NET_IPC_SMART_VIHICLE,				//车牌识别 Vehicle detection
        NET_IPC_SMART_AOIENTRY,             //进入区域 Enter region
        NET_IPC_SMART_AOILEAVE,             //离开区域 Leave region
        NET_DVR_SMART_VFD_MATCH_FAILED,    //人脸比对失败。针对陌生人 video face match failed, for stranger
        NET_IPC_SMART_PASSLINE,             //过线统计 pass line

    }

    //人脸对比
    public enum NET_SDK_FACE_INFO_GROUP_PROPERTY_TYPE
    {
        NET_SDK_FACE_INFO_GROUP_PROPERTY_ALLOW, //allow
        NET_SDK_FACE_INFO_GROUP_PROPERTY_REJECT, //reject
        NET_SDK_FACE_INFO_GROUP_PROPERTY_LIMITED,//limited
    }

    //人脸库相关
    public enum NET_SDK_FACE_MATCH_OPERATE_TYPE
    {
        NET_SDK_GET_FACE_MATCH_SUPPORT=0x01,//check if the device support face match
        //目标组
        NET_SDK_GET_FACE_INFO_GROUP_LIST,//queryFacePersonnalInfoGroupList  get face information group list
        NET_SDK_ADD_FACE_INFO_GROUP,//createFacePersonnalInfoGroup  add face information group
        NET_SDK_SET_FACE_INFO_GROUP,//editFacePersonnalInfoGroup edit face information group
        NET_SDK_DEL_FACE_INFO_GROUP,//delFacePersonnalInfoGroups delete face information group
        //目标
        NET_SDK_GET_FACE_INFO_LIST,//queryFacePersonnalInfoList get face information list
        NET_SDK_ADD_FACE_INFO,//createFacePersonnalInfo add face information
        NET_SDK_SET_FACE_INFO,//editFacePersonnalInfo edit face information
        NET_SDK_DEL_FACE_INFO,//delFacePersonnalInfo delete face information
        //对比报警配置
        NET_SDK_GET_FACE_MATCH_ALARM,//queryFaceMatchAlarm  get the face match alarm config
        NET_SDK_SET_FACE_MATCH_ALARM,//editFaceMatchAlarm set the face match alarm config
        //请求人脸目标图片
        NET_SDK_GET_FACE_INFO_IMG,//requestFacePersonnalInfoImage get the face target image
        //以图搜图 searchImageByImage
        NET_SDK_SEARCH_IMAGE_BY_IMG, //searchImageByImage 
        //查询通道人脸图片
        NET_SDK_SEARCH_CH_SNAP_FACE_IMG_LIST,//queryChSnapFaceImageList query channel's snap face image list
        //请求通道人脸图片数据
        NET_SDK_SEARCH_CH_SNAP_FACE_IMG,//requestChSnapFaceImage  request channel's snap face image
        NET_SDK_ADD_FACE_IPC, //add face target to IPC
        NET_SDK_DEL_FACE_IPC,//add IPC's face target
        //    -----------------------------------------
        //    |	NET_SDK_IVE_FACE_MATCH_MODIFY_ALBUM_INFO_T		|
        //    ---------------------------------------------
        //    |	图片数据(如果未修改图片, 则没有此部分)	|
        NET_SDK_EDIT_FACE_IPC, //edit IPC's face target

        //    请求:
        //    ---------------------------------------------
        //    |	NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_INFO		|
        //    ---------------------------------------------
        //    回复:
        //    -------------------------------------------------
        //    |	int32	符合查询条件的总数目				|
        //    -------------------------------------------------
        //    |	int32	当前次返回的结果条目数				|
        //    -------------------------------------------------
        //    |	NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO		|
        //    -------------------------------------------------
        //    |	图片数据									|
        //    -------------------------------------------------
        //    |	NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO		|
        //    -------------------------------------------------
        //    |	图片数据									|
        //    -------------------------------------------------
        //    |	...											|
        //    -------------------------------------------------
        //   附加说明: 数据段前1-4个字节(int32)表示符合条件的总条目数, 失败返回<=0，固定存在; 数据段5-8个字节(int32)表示当前次返回的条目数, 固定存在。
        NET_SDK_GET_FACE_IPC_LIST, //get IPC's face list

        NET_SDK_COPY_FACE_INFO,//   copy face information
        NET_SDK_SEARCH_IMAGE_BY_IMG_V2,// search image by image version2
        NET_SDK_SEARCH_SMART_TARGET_LIST,//搜索目标 search face target list
        NET_SDK_SEARCH_SMART_TARGET_IMG,//搜索目标图片 search face target image
        NET_SDK_GET_FACE_IMG_STATISTIC_V2,//请求统计 get face image statistic version2
        NET_SDK_GET_FACE_INFO,//获取指定id的人脸目标图片信息 get face information

    }
    public enum NET_SDK_DEVICE_MSG_TYPE
    {
        NET_SDK_ALARM = 0,		//设备报警消息 device alarm message
        NET_SDK_RECORD,		//设备录像信息 device  record status
        NET_SDK_IVM_RULE,	//智能行为分析信息(暂时保留) reserve
        NET_SDK_TRADEINFO,	//ATM交易信息(暂时保留) reserve
        NET_SDK_IPCCFG,		//混合型DVR的IPC信息变更(暂时保留) reserve
    }
    public enum NET_SDK_ALARM_TYPE
    {
        NET_SDK_ALARM_TYPE_MOTION,		//移动侦测 motion alarm
        NET_SDK_ALARM_TYPE_SENSOR,		//信号量报警 sensor alarm
        NET_SDK_ALARM_TYPE_VLOSS,		//信号丢失 video loss
        NET_SDK_ALARM_TYPE_SHELTER,		//遮挡报警 shelter alarm
        NET_SDK_ALARM_TYPE_DISK_FULL,	//硬盘满 disk full alarm
        NET_SDK_ALARM_TYPE_DISK_UNFORMATTED,//硬盘未格式化 disk unformatted alarm
        NET_SDK_ALARM_TYPE_DISK_WRITE_FAIL,	//读写硬盘出错 disk write fail
        NET_SDK_ALARM_TYPE_EXCEPTION, //exception
    }

    public enum NET_SDK_N9000_ALARM_TYPE
    {
        NET_SDK_N9000_ALARM_TYPE_RANGE_BEGIN,
        NET_SDK_N9000_ALARM_TYPE_MOTION = 0x01,/////移动侦测报警输入 motion alarm
        NET_SDK_N9000_ALARM_TYPE_SENSOR,/////传感器报警输入 sensor alarm
        NET_SDK_N9000_ALARM_TYPE_VLOSS,////视频丢失报警输入 video loss
        NET_SDK_N9000_ALARM_TYPE_FRONT_OFFLINE, //////前端设备掉线报警 device offline
        NET_SDK_N9000_ALARM_TYPE_OSC,           ////物品看护侦测报警 object remove
        NET_SDK_N9000_ALARM_TYPE_AVD,           ////视频异常侦测报警 abnormal video detection
        NET_SDK_N9000_ALARM_TYPE_AVD_SCENE,//only IPC
        NET_SDK_N9000_ALARM_TYPE_AVD_CLARITY,//only IPC
        NET_SDK_N9000_ALARM_TYPE_AVD_COLOR,//only IPC

        NET_SDK_N9000_ALARM_TYPE_PEA_TRIPWIRE,  ////越界侦测报警 trip wire alarm 
        NET_SDK_N9000_ALARM_TYPE_PEA_PERIMETER, ////区域入侵侦测报警 perimeter alarm
        NET_SDK_N9000_ALARM_TYPE_VFD,           ////人脸侦测（目前仅IPC） video face detection(IPC only)
        NET_SDK_N9000_ALARM_TYPE_CDD,           ////Crowdy density
        NET_SDK_N9000_ALARM_TYPE_IPD,           ////people intrusion
        NET_SDK_N9000_ALARM_TYPE_CPC,           ////people counting
        NET_SDK_N9000_ALARM_TYPE_FACE_MATCH,    ////人脸比对报警 for nvr 9000 video face match(N9000)
        NET_SDK_N9000_ALARM_TYPE_FACE_MATCH_FOR_IPC,    ////人脸比对报警 for ipc video face match(IPC)
        NET_SDK_N9000_ALARM_TYPE_PEA_FOR_IPC,           ////越界侦测与区域入侵侦测  Region intrusion alarm for IPC
        NET_SDK_N9000_ALARM_TYPE_TRAJECT,  ////目标跟踪轨迹 traject

        NET_SDK_N9000_ALARM_TYPE_VEHICE,	///车牌for ipc vehicle detection for IPC

        NET_SDK_N9000_ALARM_TYPE_AOIENTRY,//进入区域for ipc   Enter region alarm(IPC)
        NET_SDK_N9000_ALARM_TYPE_AOILEAVE,//离开区域for ipc   leave region alarm(IPC)

        NET_SDK_N9000_ALARM_TYPE_PASSLINE,//passline counting for ipc

        NET_SDK_N9000_ALARM_TYPE_GPS_SPEED_OVER = 0x21,//和车载有关的报警，超速 speed over
        NET_SDK_N9000_ALARM_TYPE_GPS_CROSS_BOADER,//越界 cross boader
        NET_SDK_N9000_ALARM_TYPE_GPS_TEMPERATURE_OVER,//温度报警 temperature over
        NET_SDK_N9000_ALARM_TYPE_GPS_GSENSOR_X,//GSENSOR报警  gsensor 
        NET_SDK_N9000_ALARM_TYPE_GPS_GSENSOR_Y,
        NET_SDK_N9000_ALARM_TYPE_GPS_GSENSOR_Z,

        NET_SDK_N9000_ALARM_TYPE_EXCEPTION = 0x41,
        NET_SDK_N9000_ALARM_TYPE_IP_CONFLICT,   /////IP地址冲突 ip conflict
        NET_SDK_N9000_ALARM_TYPE_DISK_IO_ERROR, /////磁盘IO错误 disk io error
        NET_SDK_N9000_ALARM_TYPE_DISK_FULL,	   /////磁盘满 disk full
        NET_SDK_N9000_ALARM_TYPE_RAID_SUBHEALTH, //阵列亚健康 raid subhealth
        NET_SDK_N9000_ALARM_TYPE_RAID_UNAVAILABLE, //阵列不可用 raid unavailable
        NET_SDK_N9000_ALARM_TYPE_ILLEIGAL_ACCESS,  /////非法访问 illeigal access
        NET_SDK_N9000_ALARM_TYPE_NET_DISCONNECT,  /////网络断开 net disconnect
        NET_SDK_N9000_ALARM_TYPE_NO_DISK,		////盘组下没有磁盘 no disk
        NET_SDK_N9000_ALARM_TYPE_SIGNAL_SHELTER, //信号遮挡 signal shelter
        NET_SDK_N9000_ALARM_TYPE_HDD_PULL_OUT, //前面板硬盘拔出 hard disk pull out

        NET_SDK_N9000_ALARM_TYPE_ALARM_OUT = 0x51,  /////报警输出的类型，报警输出也有状态需要管理 alarm out

        NET_SDK_N9000_ALARM_TYPE_RANGE_END = 0xFF,////不能超过这个值  否则会出错 can't bigger than this
    }

    //录像类型 record type
    public enum DD_RECORD_TYPE
    {
        DD_RECORD_TYPE_NONE = 0x0000,			//无录像类型 none record

        DD_RECORD_TYPE_MANUAL = 0x0001,			//手动录像 manual record
        DD_RECORD_TYPE_SCHEDULE = 0x0002,			//定时录像 Schedule record
        DD_RECORD_TYPE_MOTION = 0x0004,			//移动侦测录像 motion record
        DD_RECORD_TYPE_SENSOR = 0x0008,			//传感器报警录像 sensor record

        DD_RECORD_TYPE_BEHAVIOR = 0x0010,			//行为分析报警录像behavior record
        DD_RECORD_TYPE_SHELTER = 0x20,		//遮挡报警 shelter record
        DD_RECORD_TYPE_OVERSPEED = 0x40,		//超速 over speed record
        DD_RECORD_TYPE_OVERBOUND = 0x80,		//越界 over bound record
        DD_RECORD_TYPE_OSC = 0x0100,     //物品看护侦测录像 object remove record
        DD_RECORD_TYPE_AVD = 0x0200,     //异常侦测 abnormal video detection record
        DD_RECORD_TYPE_TRIPWIRE = 0x0400,     //越界侦测 trip wire record
        DD_RECORD_TYPE_PERIMETER = 0x0800,     //区域入侵侦测 perimeter record
        DD_RECORD_TYPE_VFD = 0x1000,     //人脸识别 video face detection record
        DD_RECORD_TYPE_POS = 0x2000,     //POS pos record
        DD_RECORD_TYPE_INTELLIGENT = DD_RECORD_TYPE_OSC | DD_RECORD_TYPE_AVD | DD_RECORD_TYPE_TRIPWIRE | DD_RECORD_TYPE_PERIMETER | DD_RECORD_TYPE_VFD,
        //DD_RECORD_TYPE_ALL = 0xFFFFFFFF, //所有的录像类型
    }

    public enum NET_SDK_PLAYCTRL_TYPE
    {
        NET_SDK_PLAYCTRL_PAUSE = 0,//pause
        NET_SDK_PLAYCTRL_FF, //fast forward
        NET_SDK_PLAYCTRL_REW,//rewind
        NET_SDK_PLAYCTRL_RESUME,//resume
	    NET_SDK_PLAYCTRL_STOP,//stop
	    NET_SDK_PLAYCTRL_FRAME,//frame
	    NET_SDK_PLAYCTRL_NORMAL,//normal
	    NET_SDK_PLAYCTRL_STARTAUDIO,	//该控制类型及以下两个控制类型只有在SDK内部解码显示时才会起作用 audio control worked only if decode and display in SDK
        NET_SDK_PLAYCTRL_STOPAUDIO,//audio control worked only if decode and display in SDK
        NET_SDK_PLAYCTRL_AUDIOVOLUME,//audio control worked only if decode and display in SDK
	    NET_SDK_PLAYCTRL_SETPOS,// set position
    }
    public enum NET_SDK_RPB_SPEED
    {
        NET_SDK_RPB_SPEED_1_32X = 1,//1/32
        NET_SDK_RPB_SPEED_1_16X,   //1/16
        NET_SDK_RPB_SPEED_1_8X,		//1/8
        NET_SDK_RPB_SPEED_1_4X,		//1/4
        NET_SDK_RPB_SPEED_1_2X,		//1/2
        NET_SDK_RPB_SPEED_1X,		//1
        NET_SDK_RPB_SPEED_2X, //2
        NET_SDK_RPB_SPEED_4X,//4
        NET_SDK_RPB_SPEED_8X,//8
        NET_SDK_RPB_SPEED_16X,//16
        NET_SDK_RPB_SPEED_32X,//32
    }

    public enum DD_CONFIG_ITEM_ID
    {
        DD_CONFIG_ITEM_SYSTEM_BASE = 0x0100,
        DD_CONFIG_ITEM_DEVICE_INFO,				//DD_DEVICE_INFO
        DD_CONFIG_ITEM_SYSTEM_BASIC,			//DD_BASIC_CONFIG
        DD_CONFIG_ITEM_DATE_TIME,				//DD_DATE_TIME_CONFIG
        DD_CONFIG_ITEM_DAYLIGHT_INFO,			//DD_DAYLIGHT_INFO
        DD_CONFIG_ITEM_SYSTEM_END,

        DD_CONFIG_ITEM_LIVE_BASE = 0x0200,
        DD_CONFIG_ITEM_LIVE_MAIN_CAMERA,		//DD_LIVE_VIDEO_GROUP * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_LIVE_SPOT_CAMERA,		//DD_LIVE_VIDEO_GROUP * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_LIVE_AUDIO,				//DD_LIVE_AUDIO_GROUP * AUDIO_INPUT_NUM
        DD_CONFIG_ITEM_LIVE_DISPLAY,			//DD_LIVE_DISPLAY
        DD_CONFIG_ITEM_LIVE_END,

        DD_CONFIG_ITEM_CHNN_BASE = 0x0300,
        DD_CONFIG_ITEM_CHNN_CONFIG,				//DD_CHANNEL_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_VIDEO_COLOR,				//DD_VIDEO_COLOR_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_VIDEO_OSD,				//DD_VIDEO_OSD_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_CHNN_END,

        DD_CONFIG_ITEM_ENCODE_BASE = 0x0400,
        DD_CONFIG_ITEM_ENCODE_MASK_MAJOR,		//DD_ENCODE_CONFIG_SUPPORT
        DD_CONFIG_ITEM_ENCODE_MASK_MINOR,		//DD_ENCODE_CONFIG_SUPPORT
        DD_CONFIG_ITEM_ENCODE_SCHEDULE,			//DD_ENCODE_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_ENCODE_ALARM,			//DD_ENCODE_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_ENCODE_NETWORK,			//DD_ENCODE_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_ENCODE_END,

        DD_CONFIG_ITEM_RECORD_BASE = 0x0500,
        DD_CONFIG_ITEM_RECORD_MASK,				//DD_RECORD_CONFIG_MASK
        DD_CONFIG_ITEM_RECORD_SETUP,			//DD_RECORD_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_RECORD_SCHEDULE_SCHEDULE,//DD_WEEK_SCHEDULE * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_RECORD_SCHEDULE_MOTION,	//DD_WEEK_SCHEDULE * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_RECORD_SCHEDULE_SENSOR,	//DD_WEEK_SCHEDULE * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_RECORD_END,

        DD_CONFIG_ITEM_NETWORK_BASE = 0x0600,
        DD_CONFIG_ITEM_NETWORK_IP,				//DD_NETWORK_IP_CONFIG
        DD_CONFIG_ITEM_NETWORK_ADVANCE,			//DD_NETWORK_ADVANCE_CONFIG
        DD_CONFIG_ITEM_NETWORK_DDNS,			//DD_DDNS_CONFIG
        DD_CONFIG_ITEM_DDNS_SERVER_INFO,		//DD_DDNS_SERVER_INFO
        DD_CONFIG_ITEM_NETWORK_SMTP,			//DD_SMTP_CONFIG
        //2012-3-15增加DVR主动注册的功能
        DD_CONFIG_ITEM_AUTO_REPORT,				//DD_AUTO_REPORT
        DD_CONFIG_ITEM_NETWORK_PLATFORM,//平台ip设置
        DD_CONFIG_ITEM_NETWORK_END,

        DD_CONFIG_ITEM_ACCOUNT_BASE = 0x0700,
        DD_CONFIG_ITEM_ACCOUNT,					//DD_ACCOUNT_CONFIG * MAX_USER_NUM
        DD_CONFIG_ITEM_ACCOUNT_END,

        DD_CONFIG_ITEM_SENSOR_BASE = 0x0800,
        DD_CONFIG_ITEM_SENSOR_SETUP,			//DD_SENSOR_CONFIG * SENSOR_INPUT_NUM
        DD_CONFIG_ITEM_SENSOR_SCHEDULE,			//DD_WEEK_SCHEDULE * SENSOR_INPUT_NUM
        DD_CONFIG_ITEM_SENSOR_ALARM_OUT,		//DD_TRIGGER_ALARM_OUT * SENSOR_INPUT_NUM
        DD_CONFIG_ITEM_SENSOR_TO_RECORD,		//DD_TRIGGER_RECORD * SENSOR_INPUT_NUM
        DD_CONFIG_ITEM_SENSOR_TO_PTZ,			//(DD_TRIGGER_PTZ * VIDEO_INPUT_NUM) * SENSOR_INPUT_NUM
        DD_CONFIG_ITEM_SENSOR_END,

        DD_CONFIG_ITEM_MOTION_BASE = 0x0900,
        DD_CONFIG_ITEM_MOTION_SETUP,			//DD_MOTION_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_MOTION_SCHEDULE,			//DD_WEEK_SCHEDULE * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_MOTION_ALARM_OUT,		//DD_TRIGGER_ALARM_OUT * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_MOTION_TO_RECORD,		//DD_TRIGGER_RECORD * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_MOTION_TO_PTZ,			//(DD_TRIGGER_PTZ * VIDEO_INPUT_NUM) * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_MOTION_END,

        DD_CONFIG_ITEM_SHELTER_BASE = 0x0a00,
        DD_CONFIG_ITEM_SHELTER_SETUP,			//DD_MOTION_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_SHELTER_SCHEDULE,		//DD_WEEK_SCHEDULE * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_SHELTER_ALARM_OUT,		//DD_TRIGGER_ALARM_OUT * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_SHELTER_TO_RECORD,		//DD_TRIGGER_RECORD * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_SHELTER_TO_PTZ,			//(DD_TRIGGER_PTZ * VIDEO_INPUT_NUM) * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_SHELTER_END,

        DD_CONFIG_ITEM_VLOSS_BASE = 0x0b00,
        DD_CONFIG_ITEM_VLOSS_SCHEDULE,			//DD_WEEK_SCHEDULE * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_VLOSS_ALARM_OUT,			//DD_TRIGGER_ALARM_OUT * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_VLOSS_TO_RECORD,			//DD_TRIGGER_RECORD * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_VLOSS_TO_PTZ,			//(DD_TRIGGER_PTZ * VIDEO_INPUT_NUM) * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_VLOSS_END,

        DD_CONFIG_ITEM_ALARM_OUT_BASE = 0x0c00,
        DD_CONFIG_ITEM_RELAY_SETUP,				//DD_RELAY_CONFIG * RELAY_NUM
        DD_CONFIG_ITEM_RELAY_SCHEDULE,			//DD_WEEK_SCHEDULE * RELAY_NUM
        DD_CONFIG_ITEM_BUZZER_SETUP,			//DD_BUZZER_CONFIG
        DD_CONFIG_ITEM_BUZZER_SCHEDULE,			//DD_WEEK_SCHEDULE
        DD_CONFIG_ITEM_ALARM_OUT_END,

        DD_CONFIG_ITEM_PTZ_BASE = 0x0d00,
        DD_CONFIG_ITEM_PTZ_SETUP,				//DD_PTZ_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_PTZ_PRESET,				//DD_PTZ_PRESET_CONFIG * VIDEO_INPUT_NUM
        DD_CONFIG_ITEM_PTZ_PROTOCOL_INFO,       //DD_PTZ_PROTOCOL_INFO
        DD_CONFIG_ITEM_PTZ_END,

        DD_DECODER_CONFIG_ITEM_BASE = 0x0e00,
        DD_DECODER_CONFIG_DEVICE_INFO,			//DEC_DEVICE_CONFIG
        DD_DECODER_CONFIG_NETWORK_INFO,			//DEC_NETWORK_CONFIG
        DD_DECODER_CONFIG_NTP_INFO,				//DEC_ADVANCE_NETWORK
        DD_DECODER_CONFIG_DDNS_INFO,			//DEC_DDNS_INFO
        DD_DECODER_CONFIG_SENSOR_SETUP,			//DEC_SENSOR_SETUP* VIDEO_INPUT_NUM
        DD_DECODER_CONFIG_SENSOR_SCHEDULE,		//DEC_WEEK_SCHEDULE * VIDEO_INPUT_NUM
        DD_DECODER_CONFIG_OTHER_ALARMOUT,		//DEC_OTHER_ALARM* VIDEO_INPUT_NUM

        DD_ITEM_ALARM_STATUS = 0x0f00, //当前报警状态，仅N9000  DD_ALARM_STATUS_INFO * NUM

        DD_CONFIG_ITEM_END
    }

    public enum DD_FRAME_TYPE
    {
	    DD_FRAME_TYPE_NONE				= 0x00,//空类型数据帧 none
	    DD_FRAME_TYPE_VIDEO				= 0x01,//视频数据帧 video frame
	    DD_FRAME_TYPE_AUDIO				= 0x02,//音频数据帧 audio frame
	    DD_FRAME_TYPE_TALK_AUDIO		= 0x03,//对讲音频数据帧 talk audio frame
	    DD_FRAME_TYPE_JPEG				= 0x04,//JPEG图片流数据帧 jped data

	    DD_FRAME_TYPE_VIDEO_FORMAT		= 0x05,//视频格式帧 video format
	    DD_FRAME_TYPE_AUDIO_FORMAT		= 0x06,//音频格式帧 audio format
	    DD_FRAME_TYPE_TALK_AUDIO_FORMAT	= 0x07,//对讲音频格式帧 talk audio format

	    DD_FRAME_TYPE_RESV1				= 0x08,
	    DD_FRAME_TYPE_RESV2				= 0x09,

	    DD_FRAME_TYPE_END				= 0x0a,//结束帧
    };

    public enum PTZ_CMD_TYPE
    {
        PTZ_CMD_STOP			=0,	//停止 stop

	    PTZ_CMD_LEFT,				//左 left
	    PTZ_CMD_RIGHT,				//右 right
	    PTZ_CMD_UP,					//上 up
	    PTZ_CMD_DOWN,				//下 down

	    PTZ_CMD_LEFT_UP,			//左上 left up
	    PTZ_CMD_LEFT_DOWN,			//左下 left down
	    PTZ_CMD_RIGHT_UP,			//右上 right up
	    PTZ_CMD_RIGHT_DOWN,			//右下 right down

	    PTZ_CMD_NEAR,				//拉近 near
	    PTZ_CMD_FAR,				//调远 far

	    PTZ_CMD_ZOOM_OUT,			//缩小 zoom out
	    PTZ_CMD_ZOOM_IN,			//放大 zoom in

	    PTZ_CMD_IRIS_OPEN,			//开启光圈 iris open
	    PTZ_CMD_IRIS_CLOSE,			//关闭光圈 iris close

	    PTZ_CMD_PRESET_SET,			//设置预置点 set preset
	    PTZ_CMD_PRESET_GO,			//到第几个预置点 go to preset
	    PTZ_CMD_PRESET_DEL,			//删除预置点 delete preset

	    PTZ_CMD_CRUISE_CFG,			//设置巡航线,相当于执行Enter、Set、Leave三个命令 config cruise
	    PTZ_CMD_ENTER_CURISE_MODE,	//进入巡航模式,此后可以设置巡航的预置点 enter cruise mode
	    PTZ_CMD_CRUISE_SET,			//设置巡航线的预置点 set cruise
	    PTZ_CMD_LEAVE_CURISE_MODE,	//退出设置巡航 leave cruise mode
	    PTZ_CMD_CRUISE_RUN,			//选择一个巡航线进行巡航 run a cruise
	    PTZ_CMD_CRUISE_STOP,		//停止巡航 stop cruise
	    PTZ_CMD_CRUISE_DEL,			//删除巡航线 delete cruise

	    PTZ_CMD_TRACK_START,		//开始轨迹 start track
	    PTZ_CMD_TRACK_STOP,			//停止轨迹 stop track
	    PTZ_CMD_TRACK_START_RECORD,	//开始保存轨迹 start record track
	    PTZ_CMD_TRACK_STOP_RECORD,	//停止保存轨迹 stop record track

	    PTZ_CMD_AUTO_SCAN_START,	//开始自动扫描 start auto scan
	    PTZ_CMD_AUTO_SCAN_STOP,		//停止自动扫描 stop auto scan

	    PTZ_CMD_RESET = 0xF0,		//复位云台状态 reset
    }
    public enum DD_PTZ_CONFIG_E
    {
        DD_PTZ_CONFIG_PRESET = 0x1,
        DD_PTZ_CONFIG_CRUISE,
        DD_PTZ_CONFIG_CRUISE_POINT,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IPC_DEVICE_INFO
    {
        public UInt32 deviceID;				//设备ID(暂时未用上) //device id
	    public UInt16 channel;				//该网络设备在本地分配的通道(从0开始) channel index
	    public UInt16 status;					//连接状态（0代表离线，1表示在线） status 0:offline 1:online
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
	    public byte[] szEtherName;		//如果为空，默认为eth0 ether name
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
	    public byte[] szServer;			//网络设备IP地址 IP
	    public UInt16	nPort;					//网络设备端口号 port
	    public UInt16	nHttpPort;				//HTTP端口 http port
	    public UInt16	nCtrlPort;				//控制端口，一般与nPort相同 control port
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
	    public byte[] szID;				//网络设备标识(或是MAC地址) id
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
	    public byte[] username;			//用户名 user name

        public UInt32 manufacturerId;			//设备厂商ID(暂时未用上) manufacture id
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
	    public byte[] manufacturerName;	//设备厂商名(暂时未用上) manufacture name
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
	    public byte[] productModel;		//产品型号(暂时未用上) product model
	    public byte	bUseDefaultCfg;			//暂时未用上 if use default config
	    public byte	bPOEDevice;				//暂时未用上 if is poe device
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 2)]
	    public byte[] resv;				//暂时未用上
        public static int GetSize()
        {
            return Marshal.SizeOf(typeof(NET_SDK_IPC_DEVICE_INFO));
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_CH_DEVICE_STATUS
    {
        public UInt16 channel;				//通道(从0开始) channel
        public UInt16 status;					//连接状态（0:offline，videoloss. 1: online,signal） 
        public UInt32 chlType;               //E_DIGITAL_CHL_TYPE,数字通道 E_ANALOG_CHL_TYPE,模拟通道
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = DevSdkHelper.DD_MAX_CAMERA_NAME_BUF_LEN)]
        public byte[] name;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
        public byte[] resv;				//暂时未用上

        public string GetName()
        {
            return DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(name).Trim());
        }

        public static int GetSize()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(NET_SDK_CH_DEVICE_STATUS));
        }
    }


    //记录时间的结构体

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_TIME
    {
        public byte second;		//Seconds after minute (0–59)
        public byte minute;		//Minutes after hour (0–59)
        public byte hour;		//Hours since midnight (0–23)
        public byte wday;		//Day of week (0–6; Sunday = 0)
        public byte mday;		//Day of month (1–31)
        public byte month;		//Month (0–11; January = 0)
        public UInt16 year;		//Year (current year minus 1900)
        public Int32 nTotalseconds;		//总秒数
        public Int32 nMicrosecond;	//微秒
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_TIME_EX  //与DD_TIME的区别在于，DD_TIME中month、year赋值不同
    {
        public byte second;		//Seconds after minute (0–59)
        public byte minute;		//Minutes after hour (0–59)
        public byte hour;		//Hours since midnight (0–23)
        public byte wday;		//Day of week (0–6; Sunday = 0)
        public byte mday;		//Day of month (1–31)
        public byte month;		//Month (1–12; January = 1)
        public UInt16 year;		//Year (current year )
        public Int32 nTotalseconds;		//总秒数
        public Int32 nMicrosecond;	//微秒
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_CLIENTINFO
    {
        public Int32 lChannel; //channel
        public Int32 streamType;//stream type
        public IntPtr hPlayWnd;//play window handle
        public Int32 bNoDecode; //0:decode，1：not decode only for windows os，default is 0
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_REC_FILE
    {
        public UInt32 dwChannel; //channel
        public UInt32 bFileLocked;//if file locked
        public DD_TIME startTime;//start time
        public DD_TIME stopTime;//stop time
        public UInt32 dwRecType;//record type
        public UInt32 dwPartition;//partition
        public UInt32 dwFileIndex;//file index
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_REC_FILE_EX
    {
        public int userId;
        public NET_SDK_REC_FILE nsrf;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_REC_EVENT
    {
        public UInt32 dwChannel;//channel
        public DD_TIME startTime;//start time
        public DD_TIME stopTime;//stop time
        public UInt32 dwRecType;//record type
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_REC_EVENT_EX
    {
        public int userId;
        public NET_SDK_REC_EVENT nsre;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_REC_TIME
    {
        public UInt32 dwChannel;//channel
        public DD_TIME startTime;//start time
        public DD_TIME stopTime;//stop time
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_REC_TIME_EX
    {
        public int userId;
        public NET_SDK_REC_TIME nsrt;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_DATE
    {
        public byte mday;		//Day of month (1–31)
        public byte month;		//Month (1–12;)
        public UInt16 year;		//Year (current solar year)
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_DVR_IVE_POINT_T
    {
        public Int32 X;
        public Int32 Y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_DVR_IVE_VFD_RESULT_HEAD_T
    {
        public Int64 time;            //检测源数据当前时间 /* 微秒级 */ current time
        public Int64 relativeTime;    //检测源数据相对时间 /* 微秒级 */ relative time
        public UInt32 detectDataLen;   //检测源数据长度 detect data length
        public UInt32 softwareVersion; //软件版本号, 0xABCDEFGH,AB：厂家 CD：主版本 EFGH：子版本 厂商 1:欧姆龙 版本:V5.00 software version 0xABCDEFGH,AB:manufacture,CD:major version EFGH:minor version
        public UInt32 softwareBuildDate;//软件生成日期,0xYYYYMMDD software build date
        public UInt32 faceCnt;         //人脸的个数，最大40个 face count
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 40)]
        public uint[] faceDataLen; //每个人脸的长度 face data length
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_DVR_IVE_VFD_RESULT_DATA_INFO_T
    {
        public UInt32 type;  //<: 0, JPG; 1, YUV
        public UInt32 status; //<:0, INVALID; 1, VALID; 2, SAVED
        public UInt32 width; 
        public UInt32 height;
        public UInt32 dataLen;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_DVR_IVE_VFD_RESULT_FACE_DATA_INFO_T
    {
        public Int32 faceId;         /* face ID Number */
        public UInt32 ptWidth;        /*坐标比例宽*/ //width
        public UInt32 ptHeight;       /*坐标比例高*/ //height
        public NET_DVR_IVE_POINT_T ptLeftTop;      /* Left-Top     Face Coordinates    */
        public NET_DVR_IVE_POINT_T ptRightTop;     /* Right-Top    Face Coordinates    */
        public NET_DVR_IVE_POINT_T ptLeftBottom;   /* Left-Bottom  Face Coordinates    */
        public NET_DVR_IVE_POINT_T ptRightBottom;  /* Right-Bottom Face Coordinates    */
        public Int32 nPose;          /* Face Pose                        */
        public Int32 nConfidence;    /* Confidence Degree                */
        public Int32 age;//age
        public Int32 sex;//sex
        public Int32 dtFrames;
        public Int32 featureSize;
        public NET_DVR_IVE_POINT_T stPosFaceImg;  //人脸左上角坐标(在检测源图片的位置) the coodinate of the image left top

        //begin 新增算法部分给的参数 
        public float feature_score;      // 特征值判断人脸可信度0~100

        /*以下变量为IFaceDetectorEx接口支持，为负数时为无效状态*/
        public short eye_dist;           // 双眼距离
        public short blur;               // 模糊度

        public byte pose_est_score;     // 人脸姿态评估分数0~100
        public byte detect_score;       // 检测算法判断人脸可信度0~100
        public byte illumination;       // 抠图区域平均亮度
        public byte faceliveness;       // 活体0~100  

        public byte completeness;       // 遮挡0~100
        public byte glasses;            // 是否戴眼镜
        public byte wearmask;           // 是否戴口罩:0是未检测，1是未戴口罩 2是戴口罩
        public byte reserved1;

        public float comprehensive_score;   // 综合评分 [90,100)是优秀， [80,90)是良好，[70,80)是一般，[60,70)是合格，[50,60)可选，[0,50)差。
        //end 新增算法部分给的参数 
        public int temperature;        //体温

        public int foreheadX;          //额头坐标（临时添加）
        public int foreheadY;          //额头坐标（临时添加）

        public NET_DVR_IVE_POINT_T      stHotLeftTop;       //热成像人脸左上坐标
	    public NET_DVR_IVE_POINT_T      stHotRightBottom;   //热成像人脸右下坐标
	    public byte                      cTemperatureMode;   //热成像测温模式，0表示正常测温模式，1表示温度矫正模式
	    public byte                      tempUnitsType;      //温度单位，0摄氏度，1华氏度
	    public byte                      cTemperatureStatus; //体温状态，0体温正常，1低温异常，2高温异常
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 5)]
	    public byte []                  reserved;
        public NET_DVR_IVE_VFD_RESULT_DATA_INFO_T stFaceImgData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_INFO_GROUP_ITEM
    {
         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 48)]
         public byte[]	guid;				//GROUP GUID
         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = DevSdkHelper.DD_MAX_NAME_LEN)]
         public byte[] name;//GROUP NAME
         public UInt32	property;		//NET_SDK_FACE_INFO_GROUP_PROPERTY_TYPE
         public UInt32 groupId;               //
         public UInt32 enableAlarmSwitch;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_INFO_GROUP_ADD
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = DevSdkHelper.DD_MAX_NAME_LEN)]
        public byte[] name;//GROUP NAME
        public UInt32	property;		//NET_SDK_FACE_INFO_GROUP_PROPERTY_TYPE
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_INFO_GROUP_DEL
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 48)]
         public byte[]	guid;				//GROUP GUID
    }

   [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct NET_SDK_FACE_INFO_LIST_GET
    {
        public UInt32   pageIndex;  //1、2、3...
        public UInt32   pageSize;    //必填 1、2、3....
        public UInt32   groupId;     //GROUP id//必填 1、2、3....
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = DevSdkHelper.DD_MAX_NAME_LEN)]
        public byte[] name;//不填时清空 NET_SDK_FACE_INFO_LIST_ITEM中的name
        public UInt32    itemId;				//不填时清零 NET_SDK_FACE_INFO_LIST_ITEM中的itemId
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct NET_SDK_FACE_INFO_LIST
    {
            public UInt32    bEnd;				//
            public UInt32    listNum;//return NET_SDK_FACE_INFO_LIST_ITEM num
            public IntPtr pFaceInfoListItem;    //NET_SDK_FACE_INFO_LIST_ITEM
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_INFO_LIST_ITEM_GROUPS
    {
            public UInt32    groupId;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 48)]
            public byte[] guid;				//GROUP GUID
            public DD_TIME_EX    validStartTime;//property为limited时才有validStartTime，validEndTime
            public DD_TIME_EX validEndTime;//property为limited时才有validStartTime，validEndTime
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct NET_SDK_FACE_INFO_LIST_ITEM
    {
            public UInt32    itemId;				//id
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = DevSdkHelper.DD_MAX_NAME_LEN)]
            public byte[] name;		//必填
            public UInt32    sex; //0:male 1:female
            public UInt32    birthday;//eg:19900707
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = DevSdkHelper.DD_MAX_NAME_LEN)]
            public byte[] nativePlace;		//
            public UInt32    certificateType; //0:idCard

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 20)]
            public byte[] certificateNum;		//

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 20)]
            public byte[] mobile;		//
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 20)]
            public byte[] number;		//
            public UInt32    faceImgCount;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 16)]
            public NET_SDK_FACE_INFO_LIST_ITEM_GROUPS[] groups;
    }
    public struct IPC_FACE_INFO
    {
        public string itemId;				//id   
        public string listType;//名单类型
        public string name;		//名称
        public string sex; //male   female
        public string age;
        public string identifyNumber;
        public string telephone;
        public string pictureData;//图片数据

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_IMG_INFO_CH
    {
        public DD_TIME_EX frameTime;
        public uint snapImgId;//抓拍图片id
        public uint targetImgId;//目标图片id
        public UInt32    chl;
        public byte      isPanorama;//0表示抓拍图片，1表示全景图
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 7)]
        public byte[]   resv;//保留
    }
    public struct Searched_IPC_FACE_INFO
    {
        public string snapTime;
        public string faceID;
    }

    [StructLayout(LayoutKind.Sequential,Pack = 4, CharSet = CharSet.Unicode)]
    public struct NET_SDK_FACE_INFO_ADD
    {
        public NET_SDK_FACE_INFO_LIST_ITEM sFaceInfoItem;
        public UInt32                imgNum;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 5)]
        public NET_SDK_FACE_IMG_INFO_CH[] sFaceImgInfo;//最大5张
        public UInt32                haveImgData;//0、1
        public UInt32                imgWidth;//haveImgData ==1 有效
        public UInt32                imgHeight;//haveImgData ==1 有效
        public UInt32                imgLen;//haveImgData ==1 有效
        public IntPtr                  imgData;//haveImgData ==1 有效
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_INFO_EDIT
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 5)]
        public uint[] delFaceImgs;
        public NET_SDK_FACE_INFO_ADD   sFaceInfoItem;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_INFO_DEL
    {
         public uint    faceInfoListItemId;//NET_SDK_FACE_INFO_LIST_ITEM中的itemId
         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 16)]
         public uint[] groupsId;//NET_SDK_FACE_INFO_LIST_ITEM中的itemId-->groups-->groupId
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_MATCH_ALARM_TRIGGER
    {
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 48)]
        //public byte[] guid;				//GROUP GUID
        //public UInt32    groupId;//目标组ID
        //public byte    groupSwitch;//启用
        //public byte    alarmOutSwitch;//联动报警输出
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        //public byte[] alarmOut;//联动报警输出默认最多16个
        //public byte    recSwitch;//录像
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
        //public UInt32[] recCH;//联动录像通道
        //public byte    snapSwitch;//抓图
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
        //public UInt32[] snapCH;//联动抓图通道
        //public UInt32    popVideo;//视频弹出
        //public byte    msgPushSwitch;
        //public byte   buzzerSwitch;
        //public byte    popMsgSwitch;
        //public byte    emailSwitch;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 48)]
        public byte[]	guid;	//GROUP GUID
        public UInt32    groupId;    //组ID
        public byte    groupSwitch;//启用
        public byte    alarmOutSwitch;//联动报警输出
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[]    alarmOut;//联动报警输出默认最多16个 下标从1开始
        public byte    recSwitch;//录像
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 128)]
        public uint[]    recCH;//联动录像通道 下标从1开始
        public byte    snapSwitch;//抓图
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 128)]
        public uint[]    snapCH;//联动抓图通道 下标从1开始
        public uint popVideo;//视频弹出
        public byte    msgPushSwitch;
        public byte    buzzerSwitch;
        public byte    popMsgSwitch;
        public byte    emailSwitch;
        //public UInt32 mark;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_MATCH_CHAN_GROUP
    {
        public UInt32 faceFeatureGroupsNum;  //通道拥有的目标组个数
        public IntPtr pfaceFeatureGroupIDs; //通道拥有的目标组ID列表
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_MATCH_ALARM
    {
        public uint    similarity;//相似度
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 128)]
        public uint[] enableCH;//开启通道 通道下标从1开始

        public uint    faceFeatureGroupsNum;//目标组个数
        public IntPtr sFaceMatchAlarmTrigger; //NET_SDK_FACE_MATCH_ALARM_TRIGGER
    }
    public struct NET_SDK_DISK_INFO
    {
	    public int		diskIndex;			//磁盘编号
        public short diskStatus;			//磁盘状态，0:正常,1:未格式化,2:正在格式化,3:异常
        public short diskProperty;		//磁盘状态，0:可读写,1:只读,2:冗余
	    public uint		diskTotalSpace;		//磁盘总空间，单位MB
	    public uint		diskFreeSpace;		//磁盘剩余空间，单位MB
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_INFO_IMG_GET
    {
        public UInt32    itemId;	//目标id       
        public UInt32    index;//faceImgCount中的index 1开始
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_FACE_INFO_IMG_DATA
    {
        public UInt32               imgLen;//目标图片长度
        public uint grade;//人脸评分
        public IntPtr                 imgData;//目标图片数据
    }

    public enum NET_SDK_SEARCH_IMAGE_BY_IMAGE_TYPE
    {
        SEARCH_IMAGE_BY_FACE_FEATURES,//通过人脸特征搜索
        SEARCH_IMAGE_BY_FACE_IMAGES,//通过人脸图片搜索
        SEARCH_IMAGE_BY_FACE_FEATURE_GROUPS,//通过人脸特征分组搜索
        SEARCH_IMAGE_BY_RECONGNIZED_FILTER,//通过与指定人脸特征分组比对是否识别进行过滤
        SEARCH_IMAGE_BY_FACE_IMAGE_DATA,//通过图片数据进行搜索

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_SEARCH_IMAGE_BY_IMAGE_sfaceFeatures
    {
        public UInt32 itemId;	//目标id
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_SEARCH_IMAGE_BY_IMAGE_sfaceFeatureGroups
    {
        public UInt32 groupsId;				//GROUP Id
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_SEARCH_IMAGE_BY_IMAGE_srecognizedFilter
    {
        public UInt32 isContainRecognized; //0 or 1
        public UInt32 isContainNotRecognized; //0 or 1
        public UInt32 groupsId;				//GROUP Id
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_SEARCH_IMAGE_BY_IMAGE_sfaceImgData
    {
        public UInt32 imgWidth;//
        public UInt32 imgHeight;//
        public UInt32 imgLen;//
        public IntPtr imgData;//

    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_SEARCH_IMAGE_BY_IMAGE
    {
        public UInt32	pageIndex;		//必填1、2、3...
        public UInt32    pageSize;   //必填
        public UInt32    similarity;		//相似度
        public UInt32    resultCountLimit;		//结果个数限制
        public DD_TIME startTime;
        public DD_TIME endTime;
        public UInt32 searchType;//NET_SDK_SEARCH_IMAGE_BY_IMAGE_TYPE

        public NET_SDK_SEARCH_IMAGE_BY_IMAGE_sfaceFeatures sfaceFeatures;//SEARCH_IMAGE_BY_FACE_FEATURES

        public NET_SDK_FACE_IMG_INFO_CH sfaceImgs;//SEARCH_IMAGE_BY_FACE_IMAGES
        public NET_SDK_SEARCH_IMAGE_BY_IMAGE_sfaceFeatureGroups sfaceFeatureGroups; //SEARCH_IMAGE_BY_FACE_FEATURE_GROUPS

        public NET_SDK_SEARCH_IMAGE_BY_IMAGE_srecognizedFilter srecognizedFilter;//SEARCH_IMAGE_BY_RECONGNIZED_FILTER
        public NET_SDK_SEARCH_IMAGE_BY_IMAGE_sfaceImgData sfaceImgData;//SEARCH_IMAGE_BY_FACE_IMAGE_DATA
   
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_SEARCH_IMAGE_BY_IMAGE_V2
    {
	    public uint    limitNum;   //必填
        public DD_TIME_EX startTime;
        public DD_TIME_EX endTime;
	    public uint    similarity;		//相似度
	    public uint searchType;//NET_SDK_SEARCH_IMAGE_BY_IMAGE_TYPE_V2
	    public uint imgSourceType;//图片来源，当searchType为SEARCH_IMAGE_BY_IMAGE时用到，NET_SDK_SEARCH_IMAGE_SOURCE_V2
	    
        public uint imgNum;//图片id数量，当searchType为SEARCH_IMAGE_BY_IMAGE时,imgSourceType为FROM_ALBUM用到       
        public IntPtr imgId;//图片id，当searchType为SEARCH_IMAGE_BY_IMAGE时,imgSourceType为FROM_ALBUM用到
        public uint eventType;//当searchType为SEARCH_IMAGE_BY_EVENT时用到，NET_SDK_SEARCH_IMAGE_BY_EVENT_TYPE_V2
        
        public uint groupsNum;//目标库数量，当searchType为SEARCH_IMAGE_BY_IMAGE时,imgSourceType为FROM_GROUP用到
        public IntPtr groupsId;	//目标库Id，当searchType为SEARCH_IMAGE_BY_IMAGE时,imgSourceType为FROM_GROUP用到
        
        public uint sfaceImgsNum;//图片数量，当searchType为SEARCH_IMAGE_BY_IMAGE时,imgSourceType为FROM_SNAP用到
        public IntPtr sfaceImgs;//SEARCH_IMAGE_BY_FACE_IMAGES    NET_SDK_FACE_IMG_INFO_CH转化成IntPtr
        public NET_SDK_SEARCH_IMAGE_BY_IMAGE_sfaceImgData sfaceImgData;//SEARCH_IMAGE_BY_FACE_IMAGE_DATA
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_SEARCH_IMAGE_ITEM
    {
        public DD_TIME recStartTime;
        public DD_TIME recEndTime;
        public UInt32    similarity;		//相似度
        public UInt32    faceFeatureId; //通过人脸特征搜索时，和哪个特征匹配
        public NET_SDK_FACE_IMG_INFO_CH sfaceImg; //通过人脸图片搜索时，和哪个图片匹配
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
        public byte[] resv;//保留
    };

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_SEARCH_IMAGE_BY_IMAGE_LIST
    {
        public UInt32 bEnd;				//
        public UInt32 listNum;//return NET_SDK_SEARCH_IMAGE_ITEM num
        public IntPtr pSearchImageItem; //NET_SDK_SEARCH_IMAGE_ITEM
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_CH_SNAP_FACE_IMG_LIST_SEARCH
    {
        public UInt32		dwChannel;//抓图通道
        public DD_TIME startTime; //时间
        public DD_TIME endTime; //时间
        public UInt32 pageIndex;//第几页
        public UInt32 pageSize;//每页数
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public byte[]	resv;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_CH_SNAP_FACE_IMG_LIST
    {
        public UInt32    bEnd; //为1表述搜图个数已结束， 0表示后面还有
        public UInt32 listNum;//return NET_SDK_FACE_IMG_INFO_CH num
        public IntPtr pCHFaceImgItem;   //NET_SDK_FACE_IMG_INFO_CH
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
     public struct NET_SDK_IVE_FACE_MATCH_T
    {
        public DD_TIME_EX frameTime;   //帧时间
        public UInt32 dwRealFaceID;  //抓拍人脸ID
        public UInt32 dwGrpID;       //特征组ID
        public UInt32 dwLibFaceID;   //特征库人脸ID
        public UInt32 dwSimilar;     //相似度
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
        public byte[] byName; //姓名
        public Int32 Channel;         //通道ID
        public UInt32 imgLen;      //人脸数据长度
    }
    //[StructLayout(LayoutKind.Explicit, Pack = 4)]
    //public struct NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO
    //{
    //    [FieldOffset(0)]
    //    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
    //    public byte[] szName;                                    //名称

    //    [FieldOffset(32)]
    //    public uint dwBirth;                    //出生日期,如19991234

    //    [FieldOffset(36)]
    //    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
    //    public byte[] szNativePlace;             //籍贯

    //    [FieldOffset(52)]
    //    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
    //    public byte[] szNote;             //备注

    //    [FieldOffset(68)]
    //    public byte byPicNum;                    //图片数量，最大5张
    //    [FieldOffset(69)]
    //    public byte byTypeCredential;            //证件类型
    //    [FieldOffset(70)]
    //    public byte bySex;                       //性别 0 male      1 female
    //    [FieldOffset(71)]
    //    public byte byGroupCount;                //所属组ID
    //    [FieldOffset(72)]
    //    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
    //    public byte[] byGroupID;

    //    [FieldOffset(76)]
    //    public PeriodV1 pv1;
    //    [FieldOffset(76)]
    //    public PeriodV2 pv2;
    //    [FieldOffset(76)]
    //    public PlaceHolder pv3;

    //    [FieldOffset(108)]
    //    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
    //    public byte[] szCredential;//证件号码, '\0' 结束     具有唯一性

    //    [FieldOffset(140)]
    //    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
    //    public byte[] byPhoneNum;//手机号码

    //    [FieldOffset(156)]
    //    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
    //     public byte[] byIDParam;//具有唯一性                   
    //}
     [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_AVPSTORE_FACE_ABSTRACT_INFO
    {
         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
         public byte[] szName;                                    //名称

         public uint dwBirth;                    //出生日期,如19991234

         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
         public byte[] szNativePlace;             //籍贯

         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
         public byte[] szNote;             //备注
         public byte byPicNum;                    //图片数量，最大5张
         public byte byTypeCredential;            //证件类型
         public byte bySex;                       //性别 0 male      1 female
         public byte byGroupCount;                //所属组ID
         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
         public byte[] byGroupID;

         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
         public byte[] ress;
         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
         public byte[] szCredential;//证件号码, '\0' 结束     具有唯一性

         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
         public byte[] byPhoneNum;//手机号码

         [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
         public byte[] byIDParam;//具有唯一性                   
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
     public struct PeriodV1
    {
        public uint dwStartTime;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 3)]
        public uint[] dwReserve;
        public uint dwEndTime;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 11)]
        public byte[]  byReserve;
        public byte  byContentType;/////0=V1,1=V2......
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
     public struct PeriodV2
    {
        public uint byWeekOrDate;//everyday  date,   1  2   3  4   5   6  7
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 3)]
        public uint[] dwReserve;
        public ushort wStartTime;
        public ushort wEndTime;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 5)]
        public ushort wReserve;
        public byte  byMode;//byWeek   byDate
        public byte  byContentType;//0=V1,1=V2......        
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PlaceHolder
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 31)]
        public byte[]  dwReserve;
        public byte  byContentType;//0=V1,1=V2......        
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_TLV_BUFFER_DESC
    {
        public byte ucID;                        //数据ID,由应用方定义
	    public byte ucVersion;        //数据版本,由应用方定义
	    public ushort usNumber;        //单位数量,最多65535
        public uint dwSize;            //单个信息大小, 总长度=usNumber*dwSize
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
     public struct PTZ_3D_POINT_INFO
    {
        public int selBeginX;		//鼠标拖选方框起始点的x坐标（相对于当前窗口左上角）
	    public int selBeginY;		//鼠标拖选方框结束点的y坐标（相对于当前窗口左上角）
	    public int selEndX;		//鼠标拖选方框结束点的x坐标（相对于当前窗口左上角）
	    public int selEndY;		//鼠标拖选方框结束点的y坐标（相对于当前窗口左上角）
	    public int displayWidth;	//图像显示区域宽度
	    public int displayHeight; 	//图像显示区域高度
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I4, SizeConst = 2)]
        public int[] reserve; //保留

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_CRUISE_POINT_INFO
    {
        public UInt32 presetIndex;
        public UInt32 dwellSpeed;
        public UInt32 dwellTime;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_CH_CRUISE
    {
        public UInt32 channel;
        public UInt32 cruiseIndex;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
        public byte[] cruiseName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_PTZ_PRESET_CONFIG_Ex
    {
        public UInt32 channel;
        public UInt32 presetIndex;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
	    public byte[] presetName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_CHANNEL_PTZ
    {
        public UInt32 dwChannel;
        public UInt32 eChanneltype;//CHANNEL_TYPE
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public byte[] resv;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_LOG
    {
        public DD_TIME strLogTime;
        public UInt32 dwMajorType;
        public UInt32 dwMinorType;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
	    public byte[] sNetUser;
        public UInt32 dwRemoteHostAddr;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 512)]
        public byte[] sContent;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_EVENT
    {
        public UInt16 chnn;			//事件对应的通道
        public UInt16 type;			//事件类型：1:MOTION 2:SENSOR 4:V_LOSS 8:V_COVER
	    public DD_TIME startTime;		//事件产生的开始时间
	    public DD_TIME endTime;			//事件的结束时间
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_DEVICE_DISCOVERY_INFO
    {
        public UInt32 deviceType;//0:NET_SDK_DVR 1:NET_SDK_DVS 2:NET_SDK_IPCAMERA 3:NET_SDK_NVR 4: NET_SDK_SUPERDVR 5:NET_SDK_DECODER
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] productType;
        
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] strIP;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] strNetMask;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] strGateWay;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public byte[] byMac;
        public UInt16 netPort;
        public UInt16 httpPort;
        public UInt32 softVer;
        public UInt32 softBuildDate;
        public byte ucIPMode;			//0 静态IP 1 dhcp
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] dwSecondIP;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] dwSecondMask;
    }

    public struct NET_SDK_ALARMINFO
    {
        public UInt32 dwAlarmType;    //alarm type:enum NET_SDK_ALARM_TYPE
        public UInt32 dwSensorIn;		//传感器报警输入端口号
        public UInt32 dwChannel;		//在报警与通道相关时，表示报警的通道号（当报警类型为NET_SDK_ALARM_TYPE_SENSOR时，可作为pop video，>=0生效）
        public UInt32 dwDisk;			//在磁盘报警时，表示产生报警的磁盘号
    }
    public struct NET_SDK_ALARMINFO_EX
    {
        public UInt32 dwAlarmType;    //alarm type:enum NET_SDK_ALARM_TYPE
        public UInt32 dwSensorIn;		//传感器报警输入端口号
        public UInt32 dwChannel;		//在报警与通道相关时，表示报警的通道号（当报警类型为NET_SDK_ALARM_TYPE_SENSOR时，可作为pop video，>=0生效）
        public UInt32 dwDisk;			//在磁盘报警时，表示产生报警的磁盘号
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
        public byte[] sensorName; //在传感器报警时，传感器的名称
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 20)]
        public byte[] alarmTime;      //alarm time
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
	    public byte[] resv;      //预留
    }

    public struct NET_SDK_RECORD_STATUS
    {
        public UInt32 dwRecordType;  //record type:enum DD_RECORD_TYPE
        public UInt32 dwChannel;           
    }
    public struct NET_SDK_RECORD_STATUS_EX
    {
        public UInt32 dwRecordType;  //record type:enum DD_RECORD_TYPE
        public UInt32 dwChannel;
        public UInt32 dwRecordStatus;//record status
    }

    public struct DD_ALARM_STATUS_INFO
    {
        public UInt32 iSize;					//the length of this struct
        public Int32 chanl;	//报警通道，跟通道无关的报警是-1， 跟通道相关的是通道，io之类的报警是个数
        public UInt32 alarmType;	//enum NET_SDK_N9000_ALARM_TYPE

    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_ALARM_STATUS_INFO_Ex
    {
        public UInt32 iSize;					//the length of this struct
        public Int32 index;	//报警通道，跟通道无关的报警是-1， 跟通道相关的是通道，io之类的报警是个数
        public UInt32 alarmType;	//enum NET_SDK_N9000_ALARM_TYPE
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
        public byte[] alarmNode;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
        public byte[] recv;		//保留字节

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_AVD_CONFIG
    {
        public UInt32 alarmHoldTime; //报警持续时间
        public UInt32 sceneChangeSwitch;//场景变更使能，0 or 1
        public UInt32 clarityAbnormalSwitch;//视频模糊使能，0 or 1
        public UInt32 colorAbnormalSwitch;//视频偏色使能，0 or 1
        public UInt32 sensitivity;
        public UInt32 maxSensitivity;
        public UInt32 minSensitivity;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 40)]
        public byte[] resv;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_VFD_CONFIG
    {
        public UInt32 bSwitch; //使能，0 or 1
        public UInt32 alarmHoldTime;//报警持续时间
        public UInt32 saveFacePicture;//保存人脸检测脸图片
        public UInt32 saveSourcePicture;//保存人脸检测源图片
        public UInt32 rectangleNum;
        public RECTANGLE	rectangle;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 40)]
        public byte[] resv;
    }
    public struct RECTANGLE
    {
        public UInt32 X1;
        public UInt32 Y1;
        public UInt32 X2;
        public UInt32 Y2;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_VFD_TRIGGER_CONFIG
    {
        public UInt32 snapCHNum; //支持最大抓图通道数
        public byte     snapChannelId;//抓图通道
        public byte     snapBswitch ;//抓图使能
        public UInt32 recordCHNum; //支持最大录像通道数
        public byte     recordChannelId;//录像通道
        public byte     recordBswitch ;//录像使能
        public UInt32 alarmOutCHNum; //支持最大报警输出数
        public byte     alarmOutChannelId;//报警输出通道
        public byte     alarmOutBswitch ;//报警输出使能

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 40)]
        public byte[] Resv;//保留，将来用于扩展多通道

    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_VFD_SCHEDULE_CONFIG
    {
        public UInt32 wDayCount; //sScheduleWeek有效个数
        public UInt32 dayCount;  //sScheduleDate 有效个数
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 70)]
        public NET_DVR_SCHEDWEEK[] sScheduleWeek; //最多可以订70个
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 31)]
        public NET_DVR_SCHEDDATE[] sScheduleDate; //最多可以订31个  值大小来自ipc的协议

    }

    public struct NET_DVR_SCHEDWEEK
    {
        public UInt32 wDay; //在星期中的天数，周天为0，，，周六为6
        public NET_DVR_SCHEDTIME sScheduleTime;

    }
    public struct NET_DVR_SCHEDTIME
    {
        public byte    byStartHour;
        public byte    byStartMin;
        public byte    byStopHour;
        public byte    byStopMin;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_DVR_SCHEDDATE
    {
        public byte    mounth;
        public byte    day;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 2)]
        public byte[]    Resv;
        public NET_DVR_SCHEDTIME sScheduleTime;

    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_DVR_SUBSCRIBE_REPLY
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 256)]
        public byte[] serverAddress;           // 订阅标识 后期的续订和退订使用
        public Int64 currentTime;                    // 当前时间
        public Int64 terminationTime;                // 终止时间
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
        public byte[] resv;                       // 后期扩展用
    }


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_DEVICE_IP_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
	    public byte[] szMac;					//设备当前mac地址
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
	    public byte[] szIpAddr;				//要修改成的ip
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
	    public byte[] szMark;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
	    public byte[] szGateway;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
	    public byte[] szPassword;			//admin用户的密码
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
	    public byte[] szDdns1;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
        public byte[] szDdns2;
        public byte ucIPMode;					//0为静态IP, 1为DHCP
    }

    public struct DD_ENCODE_CONFIG_N9000
    {
        public UInt32 iSize;		//本结构体长度

        public UInt32 resolution;	//分辨率，1-QCIF,2-CIF,4-HD1,8-D1,10-QVGA,20-VGA
        public Int16 rate;		//帧率
        public Int16 encodeType;	//编码类型，1-变码流，2-固码流
        public Int16 quality;		//画质，1-LOWEST，2-LOWER，3-LOW，4-MEDIUM，5-HEIGHTER，6-HEIGHTEST

        public Int16 minBitrate;	//码流下限
        public Int16 maxBitrate;	//码流上限
        public Int16 encodeFormat;  //编码，0-H264，1-H265，2-MJPEG
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_ENCODE_CONFIG_N9000_Ex
    {
        public Int32 iSize;		//本结构体长度

        public Int32 resolution;	//分辨率，1-QCIF,2-CIF,4-HD1,8-D1,10-QVGA,20-VGA
        public Int16 rate;		//帧率
        public Int16 encodeType;	//编码类型，1-变码流，2-固码流
        public Int16 quality;		//画质，1-LOWEST，2-LOWER，3-LOW，4-MEDIUM，5-HEIGHTER，6-HEIGHTEST

        public Int16 minBitrate;	//码流下限
        public Int16 maxBitrate;	//码流上限
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I2, SizeConst = 16)]
        public short[] bitrateRange; //支持的码率项
    }
    public struct DECODE_FRAME_INFO
    {
	    public int nWidth;
	    public int nHeight;
        public uint time;
        public uint dwLen;
	    public IntPtr	pData;
    }
    public struct ENCODE_FRAME_INFO
    {
	    public int	keyFrame;
	    public int	nWidth;
	    public int	nHeight;
	    public int	time;
	    public int	frameIndex;
	    public int	dwLen;
	    public byte	pData;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_ACCOUNT_CONFIG
    {
        public Int32 iSize;		//The length of this struct
	    public Int32 enable;			//enable or disable this account
	    public Int32 bindMAC;			//bind or not bind mac address
	    public Int32 group;			//group  0:none;1:admin,have all the rights;2:advance,have most of the rights;3:normal,have basic rights
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public byte[] MAC;//binded mac address
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 132)]		
	    public byte[] name;	//account name
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 132)]		
	    public byte[] password;//account password
	    //the following are access rights
	    public byte logSearch;		//search log right
	    public byte systemSetup;		//system set up right
        public byte fileManagement;	//fileManagement right
        public byte diskManagement;	//diskManagement right
        public byte remoteLogin;		//remoteLogin right
        public byte twoWayAudio;		//twoWayAudio right
        public byte systemMaintain;	//systemMaintain right
        public byte OnlineUserManagement;	//OnlineUserManagement right
        public byte shutdown;			//shutdown right
        public byte alarmOutCtrl;		//alarmOutCtrl right
        public byte netAlarm;			//netAlarm right
        public byte netSerialCtrl;	//netSerialCtrl right
	
	    public byte authLive;
	    public byte authRecord;
	    public byte authPlayback;
	    public byte authBackup;
	    public byte authPTZ;
	    public byte netAuthView;
	    public byte netauthRecord;
	    public byte netauthPlayback;
	    public byte netauthBackup;
	    public byte netauthPTZ;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 2)]
        public byte[] recv;//reserved bytes 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] authLiveCH;          //live preview channel 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] authRecordCH;		//local record manually 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] authPlaybackCH;		//local search and playback 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] authBackupCH;	//local backup 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] authPTZCH;	//local PTZ control 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] netAuthViewCH;	//remote live preview 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] netAuthRecordCH;		//remote record manually 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] netAuthPlaybackCH;//remote playback 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] netAuthBackupCH;	//remote backup 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] netAuthPTZCH;	//remote PTZ control 
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_DATE_TIME_CONFIG
    {
        public Int32    iSize;		
        public byte    dateFormat;	//1:YMD;2:MDY;3:DMY
        public byte    timeFormat;	//12:12hours，24:24hours
        public byte timeZone;		//0:GMT_D12;1:GMT_D11; 2:GMT_D10;3:GMT_D9;4:GMT_D8;5:GMT_D7;6:GMT_D6;7:GMT_D5;8:GMT_D4_30;9:GMT_D4;10:GMT_D3_30
        //11:GMT_D3;12:GMT_D2;13:GMT_D1;14:GMT;15:GMT_A1;16:GMT_A2;17:GMT_A3;18:GMT_A3_30;19:GMT_A4;20:GMT_A4_30;21:GMT_A5;22:GMT_A5_30;23:GMT_A5_45
        //24:GMT_A6;25:GMT_A6_30;26:GMT_A7;27:GMT_A8;28:GMT_A9;29:GMT_A9_30;30:GMT_A10;31:GMT_A11;32:GMT_A12;33:GMT_A13
        public byte enableNTP;	//enable or disable NTP synchronize
        public Int16	ntpPort;	//NTP port
        public byte   daylightSwitch;//only n9000
        public Int16	recv;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 260)]
        public byte[] ntpServerAddr;	//NTP server address

    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_NETWORK_PLATFORM
    {
        //N9000支持国标和平台软件两种平台，ipc可忽略国标
	    public UInt32 CurrentPlat;	//当前平台，默认为1，表示平台软件，2表示国标

	    //平台软件
	    public UInt32 Switcher;	//1表示启用，0表示未启用
	    public UInt32 Port;			//端口
	    public UInt32 ReportId;	//设备ID
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        public byte[] szAddress;	//ip地址


	    //国标平台，ipc不使用，N9000使用
	    public UInt32 SwitchGB;	//1表示启用，0表示未启用
	    public UInt32 PortGB;		//端口
	    public UInt32 uLocalPort;	//本地端口
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
	    public byte[] szRelm;				//sip服务器域
	    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
	    public byte[] szAddressGB;	//地址
	    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
	    public byte[] szUserName;		//用户名
	    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
	    public byte[] szPassword;		//密码
	    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
	    public byte[] szDeviceIdGB;	//设备ID
	    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
	    public byte[] szServerIdGB;	//sip服务器ID
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DD_NETWORK_IP_CONFIG
    {
        public uint iSize;		//本结构体长度	
        public uint useDHCP;	//是否使用动态网络地址	
        public uint IP;		//网络地址		
        public uint subnetMask;	//子网掩码	
        public uint gateway;	//网关	
        public uint preferredDNS;//主网络域名服务器地址	
        public uint	alternateDNS;	
        public uint	usePPPoE;	
	    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 132)]
	    public byte[] account;	//sip服务器ID
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 132)]
	    public byte[] password;	//sip服务器ID
	
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public int biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;

        public void Init()
        {
            biSize = (uint)Marshal.SizeOf(this);
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVM_RULE_STATUS
    {
        public uint dwIVMType;		//NET_SDK_IVM_RULE_TYPE
        public uint dwIVMSize;      //NET_SDK_IVM_RULE_TYPE中事件对应的结构体大小
    }
    public struct NET_SDK_IVM_STATUS_INFO
    {
        public uint dwAlarmType;		//NET_SDK_IVM_RULE_TYPE
        public uint dwChannel;		//告警通道号 alarm channel
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_DEV_SUPPORT
    {
        public int support;//要转化成二进制数，每一位对应不同的功能，具体看devIntelist
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I4, SizeConst = 15)]
        public int[] resv;				//暂时未用上
    }
    public enum devIntelist
    {
        supportThermometry,//口罩跟体温
        supportVfd,//人脸检测
        supportVfdMatch,//人脸比对
        supportThermal,////热成像
        supportPassLine,//过线统计

    }
    public enum intelist
    {
        intelist_enable,//是否包含智能能力集列表
        intelist_Perimeter,//区域入侵
        intelist_Tripwire,//绊线侦测
        intelist_Osc,//物品看护
        intelist_Avd,//异常侦测
        intelist_Cpc,//人流量统计
        intelist_Cdd,//人群密度检测
        intelist_Ipd,//人员入侵侦测
        intelist_Vfd,//人脸抓拍
        intelist_Vfd_Match,//人脸比对
        intelist_Vehice,//车牌检测
        intelist_AoiEntry,//进入区域
        intelist_AoiLeave,//离开区域
        intelist_PassLineCount,//过线统计
        intelist_Vfd_Detect,//人脸侦测
        intelist_Traffic,//流量统计
        intelist_Thermal, //热成像测温
    };

    // 车牌相机协议
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVE_VEHICE_HEAD_INFO
    {
        public uint begin_flag;       //开始标识，0x5a5a5a5a(1515870810)
        public uint item_cnt;         /// NET_SDK_IVE_VEHICE_ITEM_INFO的个数 the number of NET_SDK_IVE_VEHICE_ITEM_INFO
	    public uint plate_cnt;        /// 检测到车牌个数 plate count
	    public long  relativeTime;     /// 检测源数据相对时间,相对时间 relative time
	    public long  absoluteTime;     /// 检测源数据当前时间,绝对时间 absolute time
	    public uint softwareVersion;  /// 软件版本号, 0xABCDEFGH,AB：厂家 CD：主版本 EFGH：子版本 厂商 1:欧姆龙 版本:V5.00 software version
	    public uint softwareBuildDate;/// 软件生成日期,0xYYYYMMDD software build date
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]                             /// 
	    public uint[] resver;
        public uint end_flag;         //结束标识，0xa5a5a5a5(2779096485)
    }
    public struct NET_SDK_IVE_RECT_T
    {
        public uint X1;   // 左上角x坐标
        public uint Y1;   // 左上角y坐标
        public uint X2;   // 右下角x坐标
        public uint Y2;   // 右下角y坐标
    }
    public struct NET_SDK_IVE_POINT_T
    {
        public int X;
        public int Y;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVE_VEHICE_ITEM_INFO
    {
        public uint begin_flag;         //开始标识，0x5a5a5a5a(1515870810)

	    public uint     data_type;          //0：JPG,1:YUV
	    public uint     image_type;         //0:原图，1：车牌  0:source image 1:vehicle palte image

	    // 车牌, 车牌坐标, 车牌置信度
	    public uint     plateId;            /// ID，作为标识用（可参考人脸ID的实现方式） plate id

	    public uint     plateCharCount;     // 车牌字符个数 the plate's charactor's count
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
	    public byte[]       plate;	        // 车牌号码, utf8编码 the palte number
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
	    public byte[]       plateCharConfid;   // 车牌字符置信度 the palte's charactor's confidence
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 32)]
        public NET_SDK_IVE_RECT_T[] ptPlateCharRect; // 车牌字符左上角坐标 the plate charactor's rectangle top left coordinate

	    public uint		ptWidth;            // 坐标比例宽（用于画车牌跟踪框，或在画面中圈出车牌的位置） width
	    public uint		ptHeight;           // 坐标比例高
        public NET_SDK_IVE_POINT_T ptLeftTop;          // 车牌左上角坐标 plate top left coordinate
        public NET_SDK_IVE_POINT_T ptRightTop;         // 车牌右上角坐标 plate top right coordinate
        public NET_SDK_IVE_POINT_T ptLeftBottom;       // 车牌左下角坐标 plate bottom left coordinate
        public NET_SDK_IVE_POINT_T ptRightBottom;      // 车牌右下角坐标 bottom top right coordinate

	    public ushort     plateWidth;            //车牌抓拍图片宽度 plate's width
        public ushort plateHeight;           //车牌抓拍图片高度 plate's height

	    public uint     plateConfidence;       //置信度 the plate's confidence

	    public uint     plateIntensity;     //车牌亮度 the plate's intensity
	    public byte      plateColor;         // 车牌颜色 // 0-蓝色 1-黑色 2-黄色 3-白色 4-绿色 5-红色 6-灰色 7-紫色(KISE) plate color 0:blue 1:black 2:yellow 3:white 4:green 5:red 6:gray 7:purple
	    public byte      plateStyle;         // 车辆类型 plate style
	    public byte      PlateColorRate;     // 颜色匹配程度；   plate color rate  
	    public byte      vehicleColor;       // 车身颜色 vehicle's color
        public uint plateAngleH;        //车牌水平倾斜角度 horizon plate angle
	    public uint     plateAngleV;        //车牌竖直倾斜角度 vertical plate angle

	    public uint     jpeg_len;           //Jpeg图片实际长度 the jpeg 's length
        public uint jpeg_vir_len;       //Jpeg图片数据按32个字节对齐的总长度 total jpeg's length(32 bit justified )
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
	    public byte[]        owner;          //车主名 owner's name
	    public int         listType;           //名单类型,0-未比对成功，1-陌生车辆，2-白名单，3-黑名单，产生比对类型【1-24】，每个分类预留8个空间,未比对【0,25-31】，未比对空间扩展到8
        //list type 0:match failed 1:stranger vehicle 2:white list 3:black list
	    public ulong     beginTime;           //开始时间
	    public ulong     endTime;             //结束时间
        public byte iVehicleDirect;    //车辆行驶方向，1未知 2靠近  3远离
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 11)]
	    public byte[]       resrv;
        public uint end_flag;           //结束标识，0xa5a5a5a5(2779096485)
    }
    public struct NET_SDK_IVE_LINE_T
    {
        public uint X1;   // 起点x坐标
        public uint Y1;   // 起点y坐标
        public uint X2;   // 终点x坐标
        public uint Y2;   // 终点y坐标
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVE_PASSLINECOUNT_INFO_T
    {
	    public uint         eventId;            // 事件ID
	    public byte          status;             // 报警状态,0:none 1:start 2:end 3:procedure
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 3)]
	    public byte[]          reserve;         // 预留
	    public uint         targetId;           // 目标ID
        public NET_SDK_IVE_LINE_T line;               // 绊线规则信息
        public NET_SDK_IVE_RECT_T rect;               // 目标矩形框
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVE_PASSLINECOUNT_T
    {

	    public uint          enterCarCount; //进入车辆数
	    public uint            enterPersonCount;//进入人数
	    public uint            enterBikeCount;//进入自行车数
	    public uint            leaveCarCount; //离开车辆数，如果单向计数，此字段值为0
	    public uint            leavePersonCount;//离开人数，如果单向计数，此字段值为0
	    public uint            leaveBikeCount;//离开自行车数，如果单向计数，此字段值为0
	    public uint            existCarCount;//留存车辆数，如果单向计数，此字段值为0
	    public uint            existPersonCount;//留存人数，如果单向计数，此字段值为0
	    public int 						existBikeCount;//留存自行车数，如果单向计数，此字段值为0
	    public uint          count;         //分析结果个数
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 32)]
	    public NET_SDK_IVE_PASSLINECOUNT_INFO_T[]  passLineInfo;      // 绊线分析结果信息
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVE_FACE_MATCH_ADD_FACE_REPLY_T
    {
	    public uint             dwResult;
	    public int                 iPersonId;                 // 人员ID.
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 32)]
	    public byte[]               szRes;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct REG_LOGIN_INFO
    {
	    public uint deviceId;//注册设备id
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
	    public byte[] m_szUserName;//注册设备用户名
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 36)]
	    public byte[] m_szPasswd;//注册设备密码
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_INFO
    {
        public int				iPageNum;				   // 页码(从0开始).
        public int				iPageSize;				   // 每页条目数.(范围1-10)
        public byte             bUseKeyFilter;             // 使用键值过滤删除.
        public int              iKey;                      // 键值.
        public byte bUseNameFilter;            // 使用姓名过滤查询.
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
        public byte[]           szName;                    // 姓名.
        public byte bUseTypeFilter;            // 使用黑白名单过滤查询.
        public int              iType;                     // 名单类型 0:陌生人 1:白名单 2:黑名单.
        public byte bUseSexFilter;             // 使用性别过滤查询.
        public int              iMale;                     // 性别 1:male 0:female.
        public byte bUseIdentify;              // 使用ID匹配.
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
        public byte[]           szIdentifyNum;
        /* 可依据需求扩展过滤条件, 进行模糊查询 */
        /* 若没有过滤条件则按顺序返回相册库信息 */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO
    {
        public int              				iKey;      // 键值.
        public NET_SDK_IVE_FACE_MATCH_ADD_ALBUM_INFO stBaseInfo;// 基本信息.
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVE_FACE_MATCH_ADD_ALBUM_INFO
    {
        public int                 iType;                     // 名单类型 0:陌生人 1:白名单 2:黑名单.
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
        public byte[]              szName;               // 姓名.
        public int                 iMale;                     // 性别 1:male 0:female.
        public int                 iAge;                      // 年龄.
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
        public byte[]              szIdentifyNum;        // 身份识别号.
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
        public byte[]              szTel;                 // 电话.
        public int                 iWidth;                    // 图片宽.
        public int                 iHeight;                   // 图片高.
        public int                 iPicFormat;                // 图片格式.
        public int                 iPicSize;                  // 图片大小.
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
        public byte[]              szRes;                // reserve
    }
    
    public struct NET_SDK_IVE_AVD_INFO_T
    {
        public uint      eventId;               // 事件ID
        public uint      status;                // 报警状态,0:none 1:start 2:end 3:procedure
        public uint      type;                  // 报警类型,0:none 1:Scene 2:Clarity 3:Color
    }
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NET_SDK_IVE_AVD_T
    {
        public uint          count;             // 个数
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 32)]
        public NET_SDK_IVE_AVD_INFO_T[]   avdInfo;       // 异常侦测信息
    }

    #endregion 结构体，枚举定义

    public class DevSdkHelper
    {
        #region 常量 const
        public const string DVR_NET_SDK = "\\DVR_NET_SDK.dll";
        public const string Video_Display_SDK = "\\VideoDisplaySDK.dll";
        public const Int32 DD_MAX_CAMERA_NAME_LEN = 64;
        public const Int32 DD_MAX_CAMERA_NAME_BUF_LEN = (DD_MAX_CAMERA_NAME_LEN * 2) + 4;
        public const Int32 DD_MAX_NAME_LEN = 64;	
        public const Int32 DD_MAX_NAME_BUF_LEN	= (DD_MAX_NAME_LEN * 2) + 4;

        public const int NET_SDK_IVM_RULE_TYPE_VFD = 0;//人脸识别报警  face recognize alarm
        public const int NET_SDK_IVM_RULE_TYPE_AVD_SCENE = 1; //视频异常诊断场景变化  scene change alarm
        public const int NET_SDK_IVM_RULE_TYPE_AVD_CLARITY = 2; //视频异常诊断视频模糊  scene's clarity too low
        public const int NET_SDK_IVM_RULE_TYPE_AVD_COLOR = 3; //视频异常诊断视频偏色  scene's color is abnormal

        public const int DD_RECORD_STATUS_OFF = 0;//录像停止状态  recording stoped
        public const int DD_RECORD_STATUS_ON = 1;//录像中 recording
        public const int DD_RECORD_STATUS_ABNORMAL = 2;//录像异常 record abnormal

        public const int SEARCH_IMAGE_BY_IMAGE = 0;//通过人脸图片搜索
	    public const int SEARCH_IMAGE_BY_EVENT = 1;//通过事件搜索

        public const int FROM_ALBUM = 0;//图片来自目标库
        public const int FROM_SNAP = 1;//图片来自抓拍
        public const int FROM_GROUP = 2;//图片来自整个目标库所有目标
        public const int FROM_EXTERNAL = 3;//图片来自自选图片仅支持一张，大小不能超过200KB


        public const int BY_ALL = 0;//搜索“所有” byAll
	    public const int BY_WHITELIST = 1;//搜索“常规” byWhiteList
	    public const int BY_STRANGERLIST = 2;//搜索“陌生人”byStrangerList
	    public const int BY_BLACKLIST = 3;//搜索“黑名单”byBlackList

        public const int DEVICE_DISCONNECT = 0;//设备掉线 device disconnect
        public const int DEVICE_RECONNECT = 1;//设备上线 device reconnect
        public const int CHL_DISCONNECT = 2;//通道掉线 channel disconnect
        public const int CHL_RECONNECT = 3;//通道上线 channel reconnect

        public const int PTZ_CMD_STOP = 0;//PTZ控制停止 PTZ control cmd stop
        public const int PTZ_CMD_LEFT = 1;//PTZ控制左转 PTZ control cmd left
        public const int PTZ_CMD_RIGHT = 2;//PTZ控制右转 PTZ control cmd right
        public const int PTZ_CMD_UP = 3;//PTZ控制上仰 PTZ control cmd up
        public const int PTZ_CMD_DOWN = 4;//PTZ控制下俯 PTZ control cmd down
        public const int PTZ_CMD_LEFT_UP = 5;//PTZ控制左上转 PTZ control cmd left up
        public const int PTZ_CMD_LEFT_DOWN = 6;//PTZ控制左下 PTZ control cmd 
        public const int PTZ_CMD_RIGHT_UP = 7;//PTZ控制右上 PTZ control cmd 
        public const int PTZ_CMD_RIGHT_DOWN = 8;//PTZ控制右下 PTZ control cmd 
        public const int PTZ_CMD_NEAR = 9;//PTZ控制焦点前焦 PTZ control cmd 
        public const int PTZ_CMD_FAR = 10;//PTZ控制焦点后焦 PTZ control cmd 
        public const int PTZ_CMD_ZOOM_OUT = 11;//PTZ控制焦距变小(倍率变小) PTZ control cmd 
        public const int PTZ_CMD_ZOOM_IN = 12;//PTZ控制焦距变大(倍率变大) PTZ control cmd 
        public const int PTZ_CMD_IRIS_OPEN = 13;//PTZ控制开启光圈 PTZ control cmd 
        public const int PTZ_CMD_IRIS_CLOSE = 14;//PTZ控制关闭光圈 PTZ control cmd 

        public const int PTZ_CMD_PRESET_SET = 15;//云台设置预设点 
        public const int PTZ_CMD_PRESET_GO = 16;//云台调用预设点 
        public const int PTZ_CMD_PRESET_DEL = 17;//云台删除预设点 

        public const int PTZ_CMD_CRUISE_CFG = 18;//设置巡航线,相当于执行Enter、Set、Leave三个命令
        public const int PTZ_CMD_ENTER_CURISE_MODE = 19;//进入巡航模式,此后可以设置巡航的预置点
        public const int PTZ_CMD_LEAVE_CURISE_MODE = 21;//退出设置巡航
        public const int PTZ_CMD_CRUISE_RUN = 22;//选择一个巡航线进行巡航
        public const int PTZ_CMD_CRUISE_STOP = 23;//云台停止巡航
        public const int PTZ_CMD_CRUISE_DEL = 24;//删除巡航线

        public const int PTZ_CMD_TRACK_START = 25;//开始轨迹
        public const int PTZ_CMD_TRACK_STOP = 26;//停止轨迹
        public const int PTZ_CMD_TRACK_START_RECORD = 27;//开始保存轨迹
        public const int PTZ_CMD_TRACK_STOP_RECORD = 28;//停止保存轨迹

        public const int PTZ_CMD_AUTO_SCAN_START = 29;//开始自动扫描
        public const int PTZ_CMD_AUTO_SCAN_STOP = 30;//停止自动扫描


        public const int NET_SDK_MAIN_STREAM = 0;//主码流
        public const int NET_SDK_SUB_STREAM = 1;//子码流
        public const int NET_SDK_THIRD_STREAM = 2;//第3码流
        public const int NET_SDK_FOURTH_STREAM = 3;//第4码流

        #endregion 常量 const


        #region SDK对外接口
        ////////////////////////////////////////////////////////////////////////////////
        /********************************SDK接口函数声明*********************************/
        ////////////////////////////////////////基本信息及登录设备////////////////////////////////////////

        //获取SDK的版本信息
        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_GetSDKVersion();
        //获取SDK的版本号和build信息
        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_GetSDKBuildVersion();


        // SDK初始化及退出
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_Init();

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_Cleanup();

        //设置重连参数
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetConnectTime(UInt32 dwWaitTime = 5000, UInt32 dwTryTimes = 3);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetReconnect(UInt32 dwInterval = 5000, bool bEnableRecon = true);


        // SDK被动接收DVR注册 的本地端口号
        [DllImport(DVR_NET_SDK)]
        //public static extern bool NET_SDK_SetRegisterPort(UInt16 wRegisterPort);
        public static extern bool NET_SDK_SetRegisterPort(UInt16 wRegisterPort, IntPtr ptr, int num);
        //添加主动注册设备的信息
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_AddRegisterDeviceInfo(IntPtr pLoginInfo, uint deviceNum);//pLoginInfo为REG_LOGIN_INFO数组转换成的IntPtr

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetRegisterCallback(ACCEPT_REGISTER_CALLBACK fRegisterCBFun, IntPtr pUser);


        // 异常消息回调函数
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetSDKMessageCallBack(UInt32 nMessage, IntPtr hWnd, EXCEPTION_CALLBACK fExceptionCallBack, IntPtr[] pUser);

        // 错误查询
        [DllImport(DVR_NET_SDK)]
        public static extern UInt32 NET_SDK_GetLastError();


        //登录登出设备
        [DllImport(DVR_NET_SDK)]
        //public static extern Int32 NET_SDK_LoginEx(String sDVRIP, UInt16 wDVRPort, String sUserName, String sPassword, IntPtr lpDeviceInfo, NET_SDK_CONNECT_TYPE eConnectType, String sDevSN);
        public static extern Int32 NET_SDK_LoginEx(String sDVRIP, UInt16 wDVRPort, String sUserName, String sPassword, ref NET_SDK_DEVICEINFO lpDeviceInfo, NET_SDK_CONNECT_TYPE eConnectType, String sDevSN);
        [DllImport(DVR_NET_SDK)]
        //public static extern Int32 NET_SDK_LoginEx(String sDVRIP, UInt16 wDVRPort, String sUserName, String sPassword, IntPtr lpDeviceInfo, NET_SDK_CONNECT_TYPE eConnectType, String sDevSN);
        public static extern Int32 NET_SDK_Login(String sDVRIP, UInt16 wDVRPort, String sUserName, String sPassword, ref NET_SDK_DEVICEINFO lpDeviceInfo);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_Logout(Int32 lUserID);


        // 设备报警或状态回调 device alarm or record status
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetDVRMessageCallBack(NET_MESSAGE_CALLBACK fMessageCallBack, IntPtr pUser);
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetDVRMessageCallBackEx(NET_MESSAGE_CALLBACK_EX fMessageCallBack, IntPtr pUser);

        [DllImport(DVR_NET_SDK)]
        public static extern Int32 NET_SDK_SetupAlarmChan(Int32 lUserID);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_CloseAlarmChan(Int32 lAlarmHandle);

        //主动查询当前报警信息：N9000当前报警， IPC，3.0设备主动上报的报警
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetAlarmStatus(Int32 lUserID, IntPtr lpOutBuffer, int dwOutBufferSize, ref int lpBytesReturned);
        
        //[DllImport(DVR_NET_SDK)]
        //public static extern bool NET_SDK_GetAlarmStatusEx(Int32 lUserID, IntPtr lpOutBuffer, int dwOutBufferSize, ref int lpBytesReturned, ref int exStructNum);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetDeviceManualAlarm(Int32 lUserID, int[] pAramChannel, int[] pValue, int lAramChannelCount, bool bAlarmOpen);



        ////////////////////////////////////////获取设备信息////////////////////////////////////////

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetDeviceInfo(Int32 lUserID, ref NET_SDK_DEVICEINFO pdecviceInfo);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetDeviceTime(Int32 lUserID, ref DD_TIME pTime);//获取设备当前时间

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetDeviceCHStatus(Int32 lUserID, IntPtr pDeviceCHStatus, Int32 lBuffSize, ref Int32 pCHCount);//获取NVR的通道状态
        //public static extern bool NET_SDK_GetDeviceCHStatus(Int32 lUserID, ref NET_SDK_CH_DEVICE_STATUS[] pDeviceCHStatus, Int32 lBuffSize, ref Int32 pCHCount);//获取NVR的通道状态

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetDeviceIPCInfo(Int32 lUserID, IntPtr pDeviceIPCInfo, Int32 lBuffSize, ref Int32 pCHCount);
        //public static extern bool NET_SDK_GetDeviceIPCInfo(Int32 lUserID, ref NET_SDK_IPC_DEVICE_INFO[] pDeviceIPCInfo, Int32 lBuffSize, ref Int32 pCHCount);



        ////////////////////////////////////////预览相关接口////////////////////////////////////////
        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_LivePlay(Int32 lUserID, ref NET_SDK_CLIENTINFO lpClientInfo, LIVE_DATA_CALLBACK fLiveDataCallBack, IntPtr pUser);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_LivePlayEx(Int32 lUserID, ref NET_SDK_CLIENTINFO lpClientInfo, LIVE_DATA_CALLBACK_EX fLiveDataCallBack, IntPtr pUser);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_StopLivePlay(int lLiveHandle);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetLiveDataCallBack(int lLiveHandle, LIVE_DATA_CALLBACK fLiveDataCallBack, IntPtr pUser);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_MakeKeyFrame(Int32 lUserID, Int32 lChannel);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_MakeKeyFrameSub(Int32 lUserID, Int32 lChannel);

        //动态生成I帧
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_MakeKeyFrameEx(Int32 lUserID, Int32 lChannel, int streamType);

        //根据文件时间查找录像文件
        [DllImport(DVR_NET_SDK)]
        //public static extern Int64 NET_SDK_FindFile(Int32 lUserID, Int32 lChannel, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime);
        public static extern POINTERHANDLE NET_SDK_FindFile(Int32 lUserID, Int32 lChannel, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime);

        [DllImport(DVR_NET_SDK)]
        //public static extern Int32 NET_SDK_FindNextFile(Int64 lFindHandle, ref NET_SDK_REC_FILE lpFindData);
        public static extern Int32 NET_SDK_FindNextFile(POINTERHANDLE lFindHandle, ref NET_SDK_REC_FILE lpFindData);

        [DllImport(DVR_NET_SDK)]
        //public static extern bool NET_SDK_FindClose(Int64 lFindHandle);
        public static extern bool NET_SDK_FindClose(POINTERHANDLE lFindHandle);

        //下载录像文件
        [DllImport(DVR_NET_SDK)]
        public static extern POINTERHANDLE NET_SDK_GetFileByTime(Int32 lUserID, Int32 lChannel, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime, String sSavedFileName);

        [DllImport(DVR_NET_SDK)]
        public static extern POINTERHANDLE NET_SDK_GetFileByTimeEx(Int32 lUserID, Int32 lChannel, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime, String sSavedFileName, bool bCustomFormat, bool bUseCallBack, BACKUP_DATA_CALLBACK fBackupDataCallBack, IntPtr pUser);

        [DllImport(DVR_NET_SDK)]
        public static extern POINTERHANDLE NET_SDK_GetFileByTimeExV2(Int32 lUserID, Int32 lChannel, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime, String sSavedFileName, byte recFormat, bool bFirstStream, bool bUseCallBack, BACKUP_DATA_CALLBACK fBackupDataCallBack, IntPtr pUser);


        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_StopGetFile(POINTERHANDLE lFileHandle);

        [DllImport(DVR_NET_SDK)]
        public static extern Int32 NET_SDK_GetDownloadPos(POINTERHANDLE lFileHandle);
     

        //按事件查找录像
        [DllImport(DVR_NET_SDK)]
        public static extern POINTERHANDLE NET_SDK_FindEvent(Int32 lUserID, Int32 lChannel, uint dwRecType, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_FindNextEvent(POINTERHANDLE lFindHandle, ref NET_SDK_REC_EVENT lpFindData);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_FindEventClose(POINTERHANDLE lFindHandle);

        //按时间查找录像
        [DllImport(DVR_NET_SDK)]
        public static extern POINTERHANDLE NET_SDK_FindTime(Int32 lUserID, Int32 lChannel, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_FindNextTime(POINTERHANDLE lFindHandle, ref NET_SDK_REC_TIME lpRecTime);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_FindTimeClose(POINTERHANDLE lFindHandle);

        //录像日期查找
        [DllImport(DVR_NET_SDK)]
        public static extern POINTERHANDLE NET_SDK_FindRecDate(Int32 lUserID);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_FindNextRecDate(POINTERHANDLE lFindHandle, ref DD_DATE lpRecDate);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_FindRecDateClose(POINTERHANDLE lFindHandle);

        //录像回放
        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_PlayBackByTime(Int32 lUserID, uint[] lChannels, int channelNum, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime, IntPtr[] hWnds);
        //录像回放，可选择码流
        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_PlayBackByTimeEx(Int32 lUserID, uint[] lChannels, int channelNum, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime, IntPtr[] hWnds, bool firstStream);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PlayBackControl(int playHandle, int dwControlCode, int dwInValue, int[] lpOutValue);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_StopPlayBack(int playHandle);

        //回放录像文件时的数据捕获
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetPlayDataCallBack(int playHandle, PLAY_DATA_CALLBACK fPlayDataCallBack, IntPtr pUser);
        //回放录像捕捉YUV数据
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetPlayYUVCallBack(int playHandle, PLAY_YUV_DATA_CALLBACK fYuvCallBack, IntPtr pUser);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PlayBackSaveData(int playHandle, Int32 lChannel, string filePath);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_StopPlayBackSave(int playHandle, Int32 lChannel);

        //回放的其他操作
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PlayBackCaptureFile(int playHandle, Int32 lChannel, string filePath);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_RefreshPlay(int playHandle);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetPlayBackOsdTime(int playHandle, ref DD_TIME lpOsdTime);

        //手动录像
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_StartDVRRecord(Int32 lUserID, int lChannel, int lRecordType);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_StopDVRRecord(Int32 lUserID, int lChannel);

        //////////////////////////人脸比对////////////////////////
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_FaceMatchOperate(Int32 lUserID, int dwCommand, IntPtr lpInBuffer, UInt32 dwInBufferSize, IntPtr lpOutBuffer, UInt32 dwOutBufferSize, ref uint lpBytesReturned);
        
        [DllImport(DVR_NET_SDK)]
        //画面叠加字符和图像
        public static extern bool NET_SDK_RegisterDrawFun(int lLiveHandle, DRAW_FUN_CALLBACK fDrawFun, IntPtr pUser);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetSubscribCallBack(SUBSCRIBE_CALLBACK fSubscribCallBack, IntPtr pUser);
        //实时视频抓图 capture picture(need preview)
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_CapturePicture(int lLiveHandle, string picFileName);

        //通道抓图（不需要开启预览）capture picture(no need preview)
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_CaptureJpeg(int lUserID, int lChannel, int dwResolution, IntPtr sJpegPicBuffer, int dwPicBufSize, ref int lpSizeReturned);


        //设备快照（仅支持N9000）
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_RemoteSnap(int lUserID, int lChannel);

        //搜索设备的快照（仅支持N9000）
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SearchPictures(int lUserID, NET_SDK_IMAGE_SREACH sInSreachImage, Int32 lInImageBufferSize, IntPtr pOutImageInfo,ref Int32 pOutImageNum);
        //下载设备的快照（仅支持N9000）
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_DownLoadPicture(int lUserID, NET_SDK_IMAGE captureImage, ref NET_SDK_IMAGE_INFO pOutImageInfo, IntPtr pOutBuffer, Int32 outBufferSize);

        //独占声卡模式下开启声音
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_OpenSound(int lLiveHandle);
        //调节播放音量
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_Volume(int lLiveHandle, int wVolume);
        //独占声卡模式下关闭声音
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_CloseSound();

        //实时预览时数据捕获
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SaveLiveData(int lLiveHandle, string filePath);//保存实时预览数据

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_StopSaveLiveData(int lLiveHandle);//停止数据捕获


        //获取预览视频显示参数
        [DllImport(DVR_NET_SDK)]
        //public static extern bool NET_SDK_GetVideoEffect_Ex(int lUserID, int lChannel, IntPtr BrightValue, IntPtr ContrastValue, IntPtr SaturationValue, IntPtr HueValue);
        public static extern bool NET_SDK_GetVideoEffect_Ex(int lUserID, int lChannel, ref NET_SDK_IMAGE_EFFECT_T BrightValue, ref NET_SDK_IMAGE_EFFECT_T ContrastValue,
            ref NET_SDK_IMAGE_EFFECT_T SaturationValue, ref NET_SDK_IMAGE_EFFECT_T HueValue);
        //设置预览视频显示参数
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetVideoEffect_Ex(int lUserID, int lChannel, int BrightValue, int ContrastValue, int SaturationValue, int HueValue);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetDefaultVideoEffect(int lUserID, ref int pBrightValue, ref int pContrastValue, ref int pSaturationValue, ref int pHueValue);
        //远程参数配置
        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_EnterDVRConfig(int lUserID);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_ExitDVRConfig(int lUserID);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetDVRConfig(int lUserID, uint dwCommand, int lChannel, IntPtr lpOutBuffer, int dwOutBufferSize, ref int lpBytesReturned, bool bDefautlConfig);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetDVRConfig(int lUserID, uint dwCommand, int lChannel, IntPtr lpOutBuffer, int dwOutBufferSize);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_ChangTime(int lUserID, Int32 time);

        //[DllImport(DVR_NET_SDK)]
        //public static extern void NET_SDK_FormatTime(Int32 time, DD_TIME pFormatTime);

        //********************************************************************************
        [DllImport(Video_Display_SDK)]
        public static extern bool DISPLAY_TextOut(IntPtr hWnd, UInt32 deviceID, int lChannel, Int32 streamType, int x, int y, string pText);

        [DllImport(Video_Display_SDK)]
        public static extern bool DisplayText(int lUserID, int lChannel, Int32 streamType, int x, int y, string pText);

        //********************************************************************************
        //PTZ
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZControl(int lLiveHandle, int dwPTZCommand, int dwSpeed);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZControl_Other(int lUserID, int lChannel, int dwPTZCommand, int dwSpeed);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZControl_3D(int lLiveHandle, int lChannel, ref PTZ_3D_POINT_INFO pPtz3DInfo);

        //Preset
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZPreset(int lLiveHandle, int dwPTZPresetCmd, int dwPresetIndex);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZPreset_Other(int lUserID, int lChannel, int dwPTZPresetCmd, int dwPresetIndex);

        //Cruise
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZCruise(int lLiveHandle, int dwPTZCruiseCmd, int byCruiseRoute);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZCruise_Other(int lUserID, int lChannel, int dwPTZCruiseCmd, int byCruiseRoute);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZSetCruise(int lLiveHandle, int byCruiseRoute, IntPtr pCruisePoint, int pointNum);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZSetCruise_Other(int lUserID, int lChannel, int byCruiseRoute, IntPtr pCruisePoint, int pointNum);

        //Track
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZTrack(int lLiveHandle, int dwPTZTrackCmd);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZTrack_Other(int lUserID, int lChannel, int dwPTZTrackCmd);

        //AutoScan
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZAutoScan(int lLiveHandle, int dwPTZAutoScanCmd, int dwSpeed, bool bIsAutoScan = true);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_PTZAutoScan_Other(int lUserID, int lChannel, bool dwPTZAutoScanCmd);

        //Dome information
        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetPTZCameraType(int lUserID, ref int pCameraType);//0 不支持云台的枪机 1支持云台的枪机 2支持云台的球机

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetSupportPtzList(int lUserID, int listNum, IntPtr pOutChannelPtz, ref int returnListNum);

        [DllImport(DVR_NET_SDK)]//dwCommand: 1:preset 2:cruise 3:presets of cruise
        public static extern bool NET_SDK_GetPTZConfig(int lUserID, int lChannel, int dwCommand, IntPtr lpInBuffer, int dwInBufferSize, IntPtr lpOutBuffer, int dwOutBufferSize, ref int lpBytesReturned);

        //Log
        [DllImport(DVR_NET_SDK)]//dwType:1:all 2:alarm log 3:operate log 4:config log 5:exception log 6:other
        public static extern POINTERHANDLE NET_SDK_FindDVRLog(int lUserID, int dwType, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_FindNextLog(POINTERHANDLE lLogHandle, IntPtr lpLogData);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_FindLogClose(POINTERHANDLE lLogHandle);

        //FindEvnet
        [DllImport(DVR_NET_SDK)]
        public static extern POINTERHANDLE NET_SDK_FindEventInfo(int lUserID, uint dwType, uint channlMask, ref DD_TIME lpStartTime, ref DD_TIME lpStopTime);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_FindNextEventInfo(POINTERHANDLE lEventHandle, ref NET_SDK_EVENT lpEventData);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_FindEventInfoClose(POINTERHANDLE lLogHandle);

        //Discover Devices
        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_DiscoverDevice(IntPtr pDeviceInfo, int bufNum, int waitSeconds);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_StartVoiceCom(int lUserID, bool bNeedCBNoEncData, TALK_DATA_CALLBACK fVoiceDataCallBack, IntPtr pUser);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetAudioInfo(int lVoiceComHandle, IntPtr pAudioInfo, int infoLen);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetVoiceComClientVolume(int lVoiceComHandle, int wVolume);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_StopVoiceCom(int lVoiceComHandle);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_StartVoiceCom_MR(int lUserID, bool bNeedNoEncodeData, TALK_DATA_CALLBACK fVoiceDataCallBack, IntPtr pUser);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_VoiceComSendData(int lVoiceComHandle, IntPtr pSendBuf, int dwBufSize);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_InitAudioDecoder(IntPtr pAudioInfo, UInt32 infoLen);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_DecodeAudioFrame(int lDecodeHandle, IntPtr pInBuffer, UInt32 inLen, IntPtr pOutBuffer, ref int pOutLen);

        [DllImport(DVR_NET_SDK)]
        public static extern void NET_SDK_ReleaseAudioDecoder(int lDecodeHandle);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_InitAudioEncoder(IntPtr pAudioInfo, int infoLen);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_EncodeAudioFrame(int lEncodeHandle, IntPtr pInBuffer, int inLen, IntPtr pOutBuffer, ref int pOutLen);

        [DllImport(DVR_NET_SDK)]
        public static extern void NET_SDK_ReleaseAudioEncoder(int lEncodeHandle);

        //smart alarm
        //[DllImport(DVR_NET_SDK)] //dwCommand 0:face match config 1:face alarm trigger config 2:face alarm schedule 3:video exception config
        //public static extern bool NET_SDK_GetIVMRuleConfig(int lUserID, int dwCommand, int lChannel, IntPtr lpOutBuffer, int dwOutBufferSize, ref int lpBytesReturned);

        //[DllImport(DVR_NET_SDK)] //dwCommand 0:face match config 1:face alarm trigger config 2:face alarm schedule 3:video exception config
        //public static extern bool NET_SDK_SetIVMRuleConfig(int lUserID, int dwCommand, int lChannel, IntPtr lpInBuffer, int dwInBufferSize);

        [DllImport(DVR_NET_SDK)] //dwCommand 0:video exception diagnosis 1:Face detection 
        public static extern bool NET_SDK_SmartSubscrib(int lUserID, int dwCommand, int lChannel, ref NET_DVR_SUBSCRIBE_REPLY pOutBuffer);

        [DllImport(DVR_NET_SDK)] //dwCommand 0:video exception diagnosis 1:Face detection 
        public static extern bool NET_SDK_UnSmartSubscrib(int lUserID, int dwCommand, int lChannel, IntPtr pInServerAddress, ref int dwResult);

        //remote upgrade
        [DllImport(DVR_NET_SDK)] //
        public static extern POINTERHANDLE NET_SDK_Upgrade(int lUserID, string sFileName);

        [DllImport(DVR_NET_SDK)] //fileType,type of the upgrade file; 0:software;1:kernel;2:Uboot;3:AIlib 
        public static extern POINTERHANDLE NET_SDK_UpgradeIPC(int lUserID, string sFileName,uint fileType);

        [DllImport(DVR_NET_SDK)] //-1表示失败，其他值定义如下:1－升级成功 2－正在升级 3－升级失败 4－网络断开，状态未知 5－升级文件语言版本不匹配
        public static extern int NET_SDK_GetUpgradeState(POINTERHANDLE lUpgradeHandle);

        [DllImport(DVR_NET_SDK)] //-1表示失败，0～100表示升级进度
        public static extern int NET_SDK_GetUpgradeProgress(POINTERHANDLE lUpgradeHandle);

        [DllImport(DVR_NET_SDK)] //关闭远程升级句柄，释放资源
        public static extern bool NET_SDK_CloseUpgradeHandle(POINTERHANDLE lUpgradeHandle);

        [DllImport(DVR_NET_SDK)] //恢复设备默认参数
        public static extern bool NET_SDK_RestoreConfig(int lUserID);


        [DllImport(DVR_NET_SDK)] //导出配置文件
        public static extern bool NET_SDK_GetConfigFile(int lUserID,string sFileName);

        [DllImport(DVR_NET_SDK)] //导入配置文件
        public static extern bool NET_SDK_SetConfigFile(int lUserID, string sFileName);

        //[DllImport(DVR_NET_SDK)] //
        //public static extern bool NET_SDK_ShutDownDVR(int lUserID);
        [DllImport(DVR_NET_SDK)] //
        public static extern bool NET_SDK_RebootDVR(int lUserID);

        [DllImport(DVR_NET_SDK)] //
        public static extern POINTERHANDLE NET_SDK_FindDisk(int lUserID);
        [DllImport(DVR_NET_SDK)] //
        public static extern bool NET_SDK_GetNextDiskInfo(POINTERHANDLE lDiskHandle, IntPtr pDiskInfo);
        [DllImport(DVR_NET_SDK)] //
        public static extern bool NET_SDK_FindDiskClose(POINTERHANDLE lDiskHandle);

        [DllImport(DVR_NET_SDK)] //lDiskNumber 硬盘号，从0开始，0xff表示对所有硬盘有效（不包括只读硬盘） only support 3.0DVR 
        public static extern POINTERHANDLE NET_SDK_FormatDisk(int lUserID, int lDiskNumber);
        [DllImport(DVR_NET_SDK)] // currentDisk从0开始，-1为初始状态;percent进度是0～100 ;status 0-正在格式化；1-硬盘全部格式化完成；2-格式化当前硬盘出错，不能继续格式化此硬盘，本地和网络硬盘都会出现此错误；3-由于网络异常造成网络硬盘丢失而不能开始格式化当前硬盘only support 3.0DVR 
        public static extern bool NET_SDK_GetFormatProgress(POINTERHANDLE lFormatHandle, ref int currentDisk, ref int percent, ref int status);
        [DllImport(DVR_NET_SDK)] //only support 3.0DVR 
        public static extern bool NET_SDK_CloseFormatHandle(POINTERHANDLE lFormatHandle);

        //[DllImport(DVR_NET_SDK)] //
        //public static extern bool NET_SDK_GetDVRConfig_SubStreamEncodeInfo(int lUserID, int lChannel, IntPtr lpOutBuffer, int dwOutBufferSize, ref int lpBytesReturned, bool bDefautlConfig);
        //[DllImport(DVR_NET_SDK)] //
        //public static extern bool NET_SDK_GetDVRConfig_SubStreamEncodeInfo_Ex(int lUserID, int lChannel, IntPtr lpOutBuffer, int dwOutBufferSize, ref int lpBytesReturned, bool bDefautlConfig);
        //[DllImport(DVR_NET_SDK)] //
        //public static extern bool NET_SDK_SetDVRConfig_SubStreamEncodeInfo(int lUserID, int lChannel, IntPtr lpInBuffer, int dwInBufferSize);
        [DllImport(DVR_NET_SDK)] //
        public static extern bool NET_SDK_ModifyDeviceNetInfo(ref NET_SDK_DEVICE_IP_INFO pDeviceIPInfo);

        [DllImport(DVR_NET_SDK)] //
        public static extern bool NET_SDK_SetYUVCallBack(int lLiveHandle, YUV_DATA_CALLBACK fYuvCallBack, IntPtr pUser);

        //[DllImport(DVR_NET_SDK)] //
        //public static extern bool NET_SDK_ApiInterface(int lUserID, string sendXML, string strUrl, IntPtr lpOutBuffer, int dwOutBufferSize, ref int lpBytesReturned);

        //[DllImport(DVR_NET_SDK)] //return pFirmware:0-NET_SDK_DEVICE_FIRMWARE_V3,1-NET_SDK_DEVICE_FIRMWARE_IPC,2-NET_SDK_DEVICE_FIRMWARE_NVMS_V1,3-NET_SDK_DEVICE_FIRMWARE_OTHER
        //public static extern bool NET_SDK_GetDeviceFirmwareType(int lUserID, ref int pFirmware);

        //[DllImport(DVR_NET_SDK)] //
        //public static extern int NET_SDK_GetProductSubID(string sDVRIP, int wDVRPort);

        [DllImport(DVR_NET_SDK)] //
        public static extern bool NET_SDK_TransparentConfig(int lUserID, string sendXML, string strUrl, IntPtr lpOutBuffer, int dwOutBufferSize, ref int lpBytesReturned);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetDeviceSupportFunction(int lUserID, ref NET_SDK_DEV_SUPPORT pDevSupport);

        //预览支持码流数
        [DllImport(DVR_NET_SDK)]
        public static extern uint NET_SDK_SupportStreamNum(int lUserID, int lChannel);

        [DllImport(DVR_NET_SDK)]
        public static extern int NET_SDK_GetSha1Encrypt(int lUserID, string pIn, int iLenIn, IntPtr szOut, int outLen, ref int lpBytesReturned);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_SetLogToFile(bool bLogEnable, string strLogDi, bool bAutoDel, int logLevel);

        [DllImport(DVR_NET_SDK)]//P2P2.0 设置，sdk运行期间仅能调用一次生效（2.0协议限制）config p2p2.0, can be call only one time(the p2p2.0 limited) 
        public static extern bool NET_SDK_SetNat2Addr(string sServerAddr, ushort wDVRPort);

        [DllImport(DVR_NET_SDK)]
        public static extern bool NET_SDK_GetRtspUrl(int lUserID, int lChlID, int lStreamType, IntPtr pRtspUrl);

        #endregion SDK对外接口

        #region 工具方法
        public static string RemoveEmptyChar(string value)
        {
            //return value.Replace('\0'.ToString(), string.Empty);
            return value.TrimEnd('\0');
        }
        public static byte[] RemoveZero(byte[] src)
        {
            List<byte> resList = new List<byte>();
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] != 0)
                    resList.Add(src[i]);
                else
                    break;
            }
            return resList.ToArray();
        }
        public static string ByteToStr(byte[] src)
        {
            return Encoding.UTF8.GetString(RemoveZero(src));
        }
        public static string ByteToStr(byte[] src,bool utf8)
        {
            if(utf8)
                return Encoding.UTF8.GetString(RemoveZero(src));
            else
                return Encoding.ASCII.GetString(RemoveZero(src));
        }
        public static string IntToIp(long ipInt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ipInt & 0xFF).Append(".");
            sb.Append((ipInt >> 8) & 0xFF).Append(".");
            sb.Append((ipInt >> 16) & 0xFF).Append(".");
            sb.Append((ipInt >> 24) & 0xFF);
            return sb.ToString();
        }
        public static UInt32 IpToInt(string ip)
        {
            char[] separator = new char[] { '.' };
            string[] items = ip.Split(separator);
            return UInt32.Parse(items[3]) << 24
                    | UInt32.Parse(items[2]) << 16
                    | UInt32.Parse(items[1]) << 8
                    | UInt32.Parse(items[0]);
        }
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint mixerGetNumDevs();
        public static string GetbiCompression(int biCompression)
        {
            string res = Convert.ToString(biCompression, 2);
            while (res.Length < 32)
                res = "0" + res;
            StringBuilder sbres = new StringBuilder();
            string part1 = res.Substring(24, 8);
            int ipart1 = Convert.ToInt32(part1, 2);
            part1 = Chr(ipart1);
            sbres.Append(part1);

            part1 = res.Substring(16, 8);
            ipart1 = Convert.ToInt32(part1, 2);
            part1 = Chr(ipart1);
            sbres.Append(part1);

            part1 = res.Substring(8, 8);
            ipart1 = Convert.ToInt32(part1, 2);
            part1 = Chr(ipart1);
            sbres.Append(part1);

            part1 = res.Substring(0, 8);
            ipart1 = Convert.ToInt32(part1, 2);
            part1 = Chr(ipart1);
            sbres.Append(part1);

            return sbres.ToString();
        }
        public static string Chr(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }
        public static DateTime GetDateTime(long timestamp)
        {
            //long begtime = timestamp * 10000000;
            long begtime = timestamp * 10;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 0, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime
            return dt;
        }

        #endregion 工具方法
    }
}
