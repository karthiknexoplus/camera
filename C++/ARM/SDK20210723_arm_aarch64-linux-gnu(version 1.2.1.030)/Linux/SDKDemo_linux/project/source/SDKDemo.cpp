// SDKDemo.cpp : 定义控制台应用程序的入口点。
//

#include "DeviceManager.h"

typedef struct _channel_storage
{
	LONG		lLiveHandle;
	FILE		*pFile;
}CHANNEL_STORAGE;

CHANNEL_STORAGE		g_channelStorage[16] = {0};

LONG g_downloadHandle = -1;
LONG g_playHandle = -1;
LONG g_talkHandle = -1;

CDeviceManager *m_pDeviceMan = CDeviceManager::Instance();

void ShowNodeData(list<DEV_RGN_NODE>::iterator &it, list<DEV_RGN_NODE>::iterator end, int depth)
{
	unsigned int parentID = (*it).parentID;
	unsigned int selfID = (*it).selfID;

	for(int i = 0; i < depth; i++)
	{
		if(i == (depth-1))
		{
			printf("├──");
		}
		else
		{
			printf("│    ");
		}
	}

	if((*it).nodeType == TREE_NODE_DEVICE)
	{
		DEVICE_INFO deviceInfo;
		m_pDeviceMan->GetDeviceInfo((*it).selfID, deviceInfo);
		printf("%s(%d)[%s:%d]\n", deviceInfo.deviceName, deviceInfo.deviceID, deviceInfo.serverAddr, \
			deviceInfo.serverPort);

		CHANNEL_INFO *pChannelInfo = new CHANNEL_INFO[deviceInfo.channelNum];
		int channelNum = m_pDeviceMan->GetDeviceChannel(deviceInfo.deviceID, pChannelInfo, deviceInfo.channelNum);

		for(int channel = 0; channel < channelNum; channel++)
		{
			for(int i = 0; i < (depth+1); i++)
			{
				if(i == depth)
				{
					printf("├──");
				}
				else
				{
					printf("│    ");
				}
			}
			printf("channel %d (%d)\n", pChannelInfo[channel].channel, pChannelInfo[channel].channelID);
		}
		
		delete [] pChannelInfo;
	}
	else
	{
		printf("%s(%d)\n", (*it).devRgnName, (*it).selfID);
	}

	it++;
	while(it != end)
	{
		if((*it).parentID == selfID)
		{
			depth += 1;
			ShowNodeData(it, end, depth);
			depth -= 1;
		}
		else if((*it).parentID != parentID)
		{
			break;
		}
		else
		{
			ShowNodeData(it, end, depth);
		}
	}
}

void CALLBACK LiveDataCallBack(POINTERHANDLE lLiveHandle, NET_SDK_FRAME_INFO frameInfo, BYTE *pBuffer, void *pUser)
{
    printf("%ld : dataLen = %d\n", lLiveHandle, frameInfo.length);

	for(LONG channel = 0; channel < 16; channel++)
	{
		if(lLiveHandle == g_channelStorage[channel].lLiveHandle)
		{
			if(NULL != g_channelStorage[channel].pFile)
			{
				fwrite(pBuffer, frameInfo.length, 1, g_channelStorage[channel].pFile);
			}
		}
	}
}

void excertionCallback( DWORD dwType, LONG lUserID, LONG lHandle, void *pUser )
{

   
       printf("\r\ntype:%d, userID:%d, handle:%d---NETWORK_DISCONNECT\r\n", dwType, lUserID, lHandle);

   
}

void CALLBACK SubscribCallBack( LONG lUserID, DWORD lCommand, char *pBuf, DWORD dwBufLen, void *pUser )
{

    switch(lCommand)
    {
        case NET_SDK_N9000_ALARM_TYPE_VEHICE:
            int restLen = dwBufLen;
            NET_SDK_IVE_VEHICE_HEAD_INFO *vehiceHeadInfo = (NET_SDK_IVE_VEHICE_HEAD_INFO*)pBuf;
            unsigned int cntVheCle = vehiceHeadInfo->item_cnt;
            unsigned int offset = sizeof(NET_SDK_IVE_VEHICE_HEAD_INFO);
            if (cntVheCle > 0)
                        {
                            for(int i = 0;i< cntVheCle;i++)
                            {
                                NET_SDK_IVE_VEHICE_ITEM_INFO vehicleinfo;
                                memset(&vehicleinfo,0,sizeof(NET_SDK_IVE_VEHICE_ITEM_INFO));
                                memcpy(&vehicleinfo, pBuf + offset,sizeof(NET_SDK_IVE_VEHICE_ITEM_INFO));
                                offset += sizeof(NET_SDK_IVE_VEHICE_ITEM_INFO);//指针偏移 pointer offset

                                if (vehicleinfo.jpeg_len > 0)
                                {
                                    if(i == 0)//第1张是源图片 the 1st picture is the source picture
                                    {
                                        FILE *backfp = fopen("./vehicleSorce.jpg", "wb");
                                        if(backfp)
                                        {
                                            int fret = fwrite(pBuf+offset, vehicleinfo.jpeg_len, 1, backfp );
                                            fclose(backfp);
                                        }
                                    }
                                    else//其他是车牌照片 other picture is the vehicle plate picture
                                    {

                                        FILE *backfp = fopen("./vehicle.jpg", "wb");
                                        if(backfp)
                                        {
                                            int fret = fwrite(pBuf+offset, vehicleinfo.jpeg_len, 1, backfp );
                                            fclose(backfp);
                                        }

                                        printf("\tpalte number is:%s,charactor count:%d\n",vehicleinfo.plate,vehicleinfo.plateCharCount);
                                    }

                                }
                                offset += (vehicleinfo.jpeg_vir_len - vehicleinfo.jpeg_len);
                                //那么这个时候不用加偏移
                                if (vehicleinfo.jpeg_vir_len <= vehicleinfo.jpeg_len)
                                {
                                    offset += vehicleinfo.jpeg_len;
                                }
                                else
                                {
                                    offset += vehicleinfo.jpeg_len;
                                }
                            }
                        }

            break;
    }
    //printf("msgCallback lCommand :%d, \n", lCommand);
    //return 0;
}

void AcceptRegisterProc(LONG lUserID, LONG lRegisterID, LPNET_SDK_DEVICEINFO pDeviceInfo, void *pUser)
{

    printf("##########################\n\n");
    printf("AcceptRegisterProc lUserID=%d, lRegisterID=%d \n", lUserID, lRegisterID);
    printf("\n##########################\n");
}

int main(int argc, char * argv[])
{
//#ifdef __ENVIRONMENT_WIN32__
//	DeleteFileA("..\\config\\rgn_dev.bin");
//#else
//	unlink("./config/rgn_dev.bin");
//#endif
	char command;
	DWORD dwErrorCode = 0;
	printf("********************************************************\n");
	printf("\tq : quit\n");
	printf("\t1 : display the device tree\n");
	printf("\t2 : add region(parentId name)\n");
	printf("\t3 : add device(parentId name addr port user pwd chnnNum)\n");
	printf("\t4 : remove region\n");
	printf("\t5 : remove device\n");
	printf("\t6 : modify region\n");
	printf("\t7 : modify device\n");
	printf("\t8 : add group\n");
	printf("\t9 : add channel to group\n");
	printf("\ta : remove group\n");
	printf("\tb : remove channel from group\n");
	printf("\tc : modify group\n");
	printf("\td : display group\n");
	printf("\te : start live stream\n");
	printf("\tf : stop live stream\n");
	printf("\tg : search record data\n");
	printf("\th : start download\n");
	printf("\ti : get download progress\n");
	printf("\tj : stop download\n");
	printf("\tm : start talk\n");
	printf("\tn : stop talk\n");
	printf("\to : get device current time\n");
	printf("\tp : capture JPEG data\n");
    printf("\tv : get vehicle alarm info\n");
    printf("\ts : search device\n");
    printf("\tu : upgrade device\n");
	printf("********************************************************\n");
	printf("Please select an option:\n");

	NET_SDK_Init();
	NET_SDK_SetConnectTime();
	NET_SDK_SetReconnect(10000, TRUE);

	m_pDeviceMan->Initial();
	m_pDeviceMan->Start();
    //NET_SDK_SetSDKMessageCallBack(0, 0, excertionCallback, NULL);
    //NET_SDK_SetDVRMessageCallBack(msgCallback, NULL);
    NET_SDK_SetSubscribCallBack(SubscribCallBack, NULL);

    NET_SDK_SetRegisterPort(2009);
    NET_SDK_SetRegisterCallback(AcceptRegisterProc, NULL);
	while((command = getchar()) != 'q')
	{
		switch(command)
		{
		case '1':
			{
				list<DEV_RGN_NODE> listDevRgn;
				m_pDeviceMan->GetDevRgnNode(listDevRgn);
				if(listDevRgn.size() > 0)
				{
					list<DEV_RGN_NODE>::iterator it = listDevRgn.begin();
					ShowNodeData(it, listDevRgn.end(), 0);
				}
				break;
			}
		case '2':
			{
				printf("Please enter a region : \n");
				int parentID = 0;
				char regionName[64] = {0};
#if 0
				parentID = 0;
				strcpy(regionName, "rgn");
#else
				scanf("%d %s", &parentID, regionName);
#endif
				if(m_pDeviceMan->RegionNameExist(regionName))
				{
					printf("The region %s has already existed.\n", regionName);
					break;
				}
				dwErrorCode = m_pDeviceMan->AddRegion(parentID, regionName);
				if (dwErrorCode == RGN_DEV_INVALID_ID)
				{
					printf("AddRegion faild! ErrorCode=%d\r\n", dwErrorCode);
				}
				else
				{
					printf("AddRegion success! regionID=%d\r\n", dwErrorCode);
				}
				break;
			}
		case '3':
			{
				printf("Please enter a device : \n");
				int parentID = 0;
				char deviceName[64] = {0};
				int nPort = 0;
				char svrAddr[64] = {0};
				char user[64] = {0};
				char pwd[64] = {0};
				int channelNum = {0};
#if 0
				parentID = 1;
                strcpy(deviceName, "rgn");
				nPort = 6036;
				strcpy(svrAddr, "192.168.3.127");
				strcpy(user, "admin");
				strcpy(pwd, "123456");
                channelNum = 9;
#else
				scanf("%d %s %s %d %s %s %d", &parentID, deviceName, svrAddr, &nPort, user, pwd, &channelNum);
#endif
				if(m_pDeviceMan->DeviceNameExist(deviceName))
				{
					printf("The device name %s has already existed.\n", deviceName);
					break;
				}
				if(m_pDeviceMan->DeviceExist(svrAddr, nPort))
				{
					printf("The device (%s:%d) has already existed.\n", svrAddr, nPort);
					break;
				}
				dwErrorCode = m_pDeviceMan->AddDevice(parentID, deviceName, svrAddr, nPort, user, pwd, channelNum);
				if (dwErrorCode == RGN_DEV_INVALID_ID)
				{
					printf("AddDevice faild! ErrorCode=%d\r\n", dwErrorCode);
				}
				else
				{
					printf("AddDevice success! DeviceID=%d\r\n", dwErrorCode);
				}

				break;
			}
		case '4':
			{
				printf("Please enter a region id to remove : \n");
				int regionID;
				scanf("%d", &regionID);
				m_pDeviceMan->RemoveRegion(regionID);
				break;
			}
		case '5':
			{
				printf("Please enter a device id to remove : \n");
				int deviceID;
				scanf("%d", &deviceID);
				m_pDeviceMan->RemoveDevice(deviceID);
				break;
			}
		case '6':
			{
				printf("Please enter a region to modify: \n");
				int regionID;
				char regionName[64];
				scanf("%d %s", &regionID, regionName);
				if(m_pDeviceMan->RegionNameExist(regionName))
				{
					printf("The region %s has already existed.\n", regionName);
					break;
				}
				m_pDeviceMan->ModifyRegion(regionID, regionName);
				break;
			}
		case '7':
			{
				printf("Please enter a device to modify: \n");
				int deviceID;
				char deviceName[64];
				int nPort;
				char svrAddr[64];
				char user[64];
				char pwd[64];
				int channelNum;
				scanf("%d %s %d %s %s %s %d", &deviceID, deviceName, &nPort, svrAddr, user, pwd, &channelNum);
				m_pDeviceMan->ModifyDevice(deviceID, deviceName, svrAddr, nPort, user, pwd);
				break;
			}
		case '8':
			{
				printf("Please enter a group name : \n");
				char groupName[64];
				scanf("%s", groupName);
				if(m_pDeviceMan->GroupNameExist(groupName))
				{
					printf("The group name %s has already existed.\n", groupName);
				}
				m_pDeviceMan->AddGroup(groupName);
				break;
			}
		case '9':
			{
				printf("Please enter a group and a channel (group channel): \n");
				LONG groupID, channelID;
				scanf("%d %d", &groupID, &channelID);
				m_pDeviceMan->AddChannelToGroup(channelID, groupID);
				break;
			}
		case 'a':
			{
				printf("Please enter a group to remove : \n");
				int groupID;
				scanf("%d", &groupID);
				m_pDeviceMan->RemoveGroup(groupID);
				break;
			}
		case 'b':
			{
				printf("Please enter a group and a channel (group channel): \n");
				LONG groupID, channelID;
				scanf("%d %d", &groupID, &channelID);
				m_pDeviceMan->RemoveChannelFromGroup(channelID, groupID);
				break;
			}
		case 'c':
			{
				printf("Please enter a group to modify (id name) : \n");
				int groupID;
				char groupName[64];
				scanf("%d %s", &groupID, groupName);
				if(m_pDeviceMan->GroupNameExist(groupName))
				{
					printf("The group name %s has already existed.\n", groupName);
				}
				m_pDeviceMan->ModifyGroup(groupID, groupName);
				break;
			}
		case 'd':
			{
				vector<GROUP_INFO> groupInfo;
				m_pDeviceMan->GetGroupNode(groupInfo);
				for(int i = 0; i < groupInfo.size(); i++)
				{
					printf("%s(%d)\n", groupInfo[i].groupName, groupInfo[i].groupID);
					CHANNEL_INFO channelInfo[MAX_CHANNEL_NUM];
					int channelNum = m_pDeviceMan->GetGroupChannelInfo(groupInfo[i].groupID, channelInfo, MAX_CHANNEL_NUM);
					for(int channel = 0; channel < channelNum; channel++)
					{
						DEVICE_INFO deviceInfo;
						m_pDeviceMan->GetDeviceInfo(channelInfo[channel].deviceID, deviceInfo);

						printf("├──");
						printf("%s:channel%d(%d)\n", deviceInfo.deviceName, channelInfo[channel].channel, channelInfo[channel].channelID);
					}
				}
				break;
			}
		case 'e':
			{
				printf("Please enter the device and channel to play(deviceID, channel, streamType): sizeof(int):%d\n", sizeof(int));
				int deviceID = 2;
				int channel = 0;
				int streamType = 0;
				scanf("%d %d %d", &deviceID, &channel, &streamType);

				CHANNEL_INFO channelInfo[16];
				m_pDeviceMan->GetDeviceChannel(deviceID, channelInfo, 16);

				LONG playHandle = m_pDeviceMan->StartLivePlay(channelInfo[channel].channelID, streamType, LiveDataCallBack, NULL);
				if(playHandle != -1)
				{
					char fileName[64];
					sprintf(fileName, "..\\channel%02d.dat", channel+1);
					g_channelStorage[channel].lLiveHandle = playHandle;
					g_channelStorage[channel].pFile = fopen(fileName, "wb");
				}

			}
			break;
		case 'f':
			{
#if 1
				for(LONG channel = 0; channel < 16; channel++)
				{
					if(g_channelStorage[channel].lLiveHandle > 0)
					{
						m_pDeviceMan->StopLivePlay(g_channelStorage[channel].lLiveHandle);
						g_channelStorage[channel].lLiveHandle = 0;
					}
					if(g_channelStorage[channel].pFile != NULL)
					{
						fclose(g_channelStorage[channel].pFile);
						g_channelStorage[channel].pFile = NULL;
					}
				}
#else
				printf("Please enter the playhandle to stop(LiveHandle):\n");
				LONG liveHandle;
				scanf("%d", &liveHandle);
				NET_SDK_StopLivePlay(liveHandle);
				fclose(pFile);
				pFile = NULL;
#endif
			}
			break;
		case 'g':
			{
				tm startTime, endTime;
				int deviceID, channel, searchType;
				printf("please input start time (year month day hour minute second)\n");
				scanf("%d %d %d %d %d %d", &startTime.tm_year, &startTime.tm_mon, &startTime.tm_mday, \
					&startTime.tm_hour, &startTime.tm_min, &startTime.tm_sec);

				printf("please input end time (year month day hour minute second)\n");
				scanf("%d %d %d %d %d %d", &endTime.tm_year, &endTime.tm_mon, &endTime.tm_mday, \
					&endTime.tm_hour, &endTime.tm_min, &endTime.tm_sec);

				startTime.tm_year -= 1900;
				startTime.tm_mon -= 1;
				endTime.tm_year -= 1900;
				endTime.tm_mon -= 1;

				printf("please input : deviceID, channel, searchType(0-time, 1-event, 2-file, 3-record date)\n");
				scanf("%d %d %d", &deviceID, &channel, &searchType);

				if(0 == searchType)
				{
					list<DD_REC_TIME> timeList;
					if(m_pDeviceMan->SearchTime(deviceID, channel, TmToDVRTime32(startTime), TmToDVRTime32(endTime), timeList))
					{
						int index = 1;
						printf("No.\tDevice\tchannel\t     startTime     \t      endTime      \n");
						for(list<DD_REC_TIME>::iterator it = timeList.begin(); it != timeList.end(); it++)
						{
							startTime = DVRTime32ToTm((*it).startTime);
							endTime = DVRTime32ToTm((*it).stopTime);
							startTime.tm_year += 1900;
							startTime.tm_mon += 1;
							endTime.tm_year += 1900;
							endTime.tm_mon += 1;
							printf("%d\t%d\t%d\t%d-%02d-%02d %02d:%02d:%02d\t%d-%02d-%02d %02d:%02d:%02d\n", index, \
								(*it).deviceID, (*it).dwChannel+1, startTime.tm_year, startTime.tm_mon, startTime.tm_mday, \
								startTime.tm_hour, startTime.tm_min, startTime.tm_sec, endTime.tm_year, endTime.tm_mon, \
								endTime.tm_mday, endTime.tm_hour, endTime.tm_min, endTime.tm_sec);
							index++;
						}
					}
					else
					{
						printf("no record data\n");
					}
				}
				else if(1 == searchType)
				{
					list<DD_REC_EVENT> eventList;
					if(m_pDeviceMan->SearchEvent(deviceID, channel, 0xff, TmToDVRTime32(startTime), TmToDVRTime32(endTime), eventList))
					{
						int index = 1;
						printf("No.\tDevice\tchannel\t     startTime     \t      endTime      \n");
						for(list<DD_REC_EVENT>::iterator it = eventList.begin(); it != eventList.end(); it++)
						{
							startTime = DVRTime32ToTm((*it).startTime);
							endTime = DVRTime32ToTm((*it).stopTime);
							startTime.tm_year += 1900;
							startTime.tm_mon += 1;
							endTime.tm_year += 1900;
							endTime.tm_mon += 1;
							printf("%d\t%d\t%d\t%d-%02d-%02d %02d:%02d:%02d\t%d-%02d-%02d %02d:%02d:%02d\n", index, \
								(*it).deviceID, (*it).dwChannel+1, startTime.tm_year, startTime.tm_mon, startTime.tm_mday, \
								startTime.tm_hour, startTime.tm_min, startTime.tm_sec, endTime.tm_year, endTime.tm_mon, \
								endTime.tm_mday, endTime.tm_hour, endTime.tm_min, endTime.tm_sec);
							index++;
						}
					}
					else
					{
						printf("no record data\n");
					}
				}
				else if(2 == searchType)
				{
					list<DD_REC_FILE> fileList;
					if(m_pDeviceMan->SearchFile(deviceID, channel, TmToDVRTime32(startTime), TmToDVRTime32(endTime), fileList))
					{
						int index = 1;
						printf("No.\tDevice\tchannel\t     startTime     \t      endTime      \n");
						for(list<DD_REC_FILE>::iterator it = fileList.begin(); it != fileList.end(); it++)
						{
							startTime = DVRTime32ToTm((*it).startTime);
							endTime = DVRTime32ToTm((*it).stopTime);
							startTime.tm_year += 1900;
							startTime.tm_mon += 1;
							endTime.tm_year += 1900;
							endTime.tm_mon += 1;
							printf("%d\t%d\t%d\t%d-%02d-%02d %02d:%02d:%02d\t%d-%02d-%02d %02d:%02d:%02d\n", index, \
								(*it).deviceID, (*it).dwChannel+1, startTime.tm_year, startTime.tm_mon, startTime.tm_mday, \
								startTime.tm_hour, startTime.tm_min, startTime.tm_sec, endTime.tm_year, endTime.tm_mon, \
								endTime.tm_mday, endTime.tm_hour, endTime.tm_min, endTime.tm_sec);
							index++;
						}
					}
					else
					{
						printf("no record data\n");
					}
				}
				else if(3 == searchType) //record date
				{
					list<DD_DATE> recDataList;
					if (m_pDeviceMan->SearchRecDate(deviceID, recDataList))
					{
						std::string strLog("\n*****************Record date***********\n");
						list<DD_DATE>::iterator it = recDataList.begin();
						for ( ; it != recDataList.end(); it++)
						{
							char szTemp[512] = {0};
							sprintf(szTemp, "\n%04d-%02d-%0d", it->year, it->month, it->mday);
							strLog += szTemp;
						}
						strLog += "\n*****************Record date end***********\n";
						printf(strLog.c_str());
					}
					else
					{
						printf("faild to SearchRecDate!\n");
					}
				}
				else
				{
					printf("input searchType error!\n");
				}
			}
			break;
		case 'h':
			{
				tm startTime, endTime;
				int deviceID, channel;
				char fileName[] = "./abc.avi";
				printf("please input start time (year month day hour minute second)\n");
				scanf("%d %d %d %d %d %d", &startTime.tm_year, &startTime.tm_mon, &startTime.tm_mday, \
					&startTime.tm_hour, &startTime.tm_min, &startTime.tm_sec);

				printf("please input end time (year month day hour minute second)\n");
				scanf("%d %d %d %d %d %d", &endTime.tm_year, &endTime.tm_mon, &endTime.tm_mday, \
					&endTime.tm_hour, &endTime.tm_min, &endTime.tm_sec);

				startTime.tm_year -= 1900;
				startTime.tm_mon -= 1;
				endTime.tm_year -= 1900;
				endTime.tm_mon -= 1;

				printf("please input : deviceID, channel\n");
				scanf("%d %d", &deviceID, &channel);
				g_downloadHandle = m_pDeviceMan->GetFileByTime(deviceID, channel, TmToDVRTime32(startTime), TmToDVRTime32(endTime), fileName);
				printf("g_downloadHanle = 0x%x\n", g_downloadHandle);
			}
			break;
		case 'i':
			{
				if(-1 != g_downloadHandle)
				{
					LONG pos = m_pDeviceMan->GetFilePos(g_downloadHandle);
					printf("download %d%%\n", pos);
				}
			}
			break;
		case 'j':
			{
				if(-1 != g_downloadHandle)
				{
					m_pDeviceMan->StopDownLoad(g_downloadHandle);
					g_downloadHandle = -1;
				}
			}
			break;
		case 'k':
			{
				tm startTime, endTime;
				int deviceID;
				printf("please input start time (year month day hour minute second)\n");
				scanf("%d %d %d %d %d %d", &startTime.tm_year, &startTime.tm_mon, &startTime.tm_mday, \
					&startTime.tm_hour, &startTime.tm_min, &startTime.tm_sec);
				printf("please input end time (year month day hour minute second)\n");
				scanf("%d %d %d %d %d %d", &endTime.tm_year, &endTime.tm_mon, &endTime.tm_mday, \
					&endTime.tm_hour, &endTime.tm_min, &endTime.tm_sec);
				printf("Please input deviceID\n");
				scanf("%d", &deviceID);
				startTime.tm_year -= 1900;
				startTime.tm_mon -= 1;
				endTime.tm_year -= 1900;
				endTime.tm_mon -= 1;
				LONG channels[1] = {0};
				g_playHandle = m_pDeviceMan->PlaybackByTime(deviceID, channels, 1, TmToDVRTime32(startTime), TmToDVRTime32(endTime));
				if(g_playHandle != -1)
				{
					NET_SDK_SetPlayDataCallBack(g_playHandle, LiveDataCallBack, NULL);
				}
			}
			break;
		case 'l':
			{
				if(g_playHandle != -1)
				{
					m_pDeviceMan->StopPlayback(g_playHandle);
					g_playHandle = -1;
				}
			}
			break;
		case 'm':
			{
				printf("Please enter the device:\n");
				int deviceID;
				scanf("%d", &deviceID);

				LONG playHandle = m_pDeviceMan->StartTalk(deviceID);
				if(playHandle != -1)
				{
					g_talkHandle = playHandle;
				}
				else
				{
					printf("start talk failed!\n");
				}
			}
			break;
		case 'n':
			{
				if (g_talkHandle != -1)
				{
					m_pDeviceMan->StopTalk(g_talkHandle);
				}
				else
				{
					printf("talk handle == -1\n!");
				}
			}
			break;
		case 'o':
			{
				printf("Please enter the device:\n");
				int deviceID;
				scanf("%d", &deviceID);
				m_pDeviceMan->GetDeviceTime(deviceID);
			}
			break;
		case 'p':
			{
				BOOL bResult = FALSE;
				long deviceID, channel;
				printf("please input : deviceID and channel(deviceID channel)\n");
				scanf("%d %d", &deviceID, &channel);

				DWORD dwRetJpegLen = 0;
				const int JPEG_DATA_LEN = 1024*1024*4;
				char *pJPEGBuf = new char[JPEG_DATA_LEN];
				if (pJPEGBuf)
				{
					memset(pJPEGBuf, 0x00, JPEG_DATA_LEN);
					if ((bResult = m_pDeviceMan->CaptureJPEGData(deviceID, channel, pJPEGBuf, JPEG_DATA_LEN, &dwRetJpegLen)))
					{
						FILE* fp = fopen("./capture.jpg", "wb");
						if (fp)
						{
							fwrite(pJPEGBuf, sizeof(char), dwRetJpegLen, fp);
							fclose(fp);
						}
					}
					else
					{
						printf("faild to capture jpeg data!\n");
					}

					delete []pJPEGBuf;
					pJPEGBuf = NULL;
				}
			}
			break;
        case 'v':
            {
                long deviceID;
                printf("please input : deviceID)\n");
                scanf("%d", &deviceID);
                NET_DVR_SUBSCRIBE_REPLY sSmartSubscrib;
                bool bret = NET_SDK_SmartSubscrib(deviceID, NET_IPC_SMART_VIHICLE, 0, &sSmartSubscrib);
                if(bret)
                {
                    printf("sub vehicle alarm success!\n");
                }
                break;
            }
        case 's':
            {
                NET_SDK_DEVICE_DISCOVERY_INFO devs[100];
                int num = NET_SDK_DiscoverDevice(devs, 100, 20);
                for(int i = 0;i < num;i++)
                {
                    printf("discovered:search device ip is %s\n",devs[i].strIP);
                }
                break;
            }
        case 'u':
            {
                long deviceID;
                printf("please input : deviceID)\n");
                scanf("%d", &deviceID);
                long lHandle = NET_SDK_UpgradeIPC(deviceID,"opt/AP_E3_2MP_5.0.1.0(5475)_20200115.tar", 0);
                if(lHandle > 0)
                {
                     printf("upgrade ipc ret");
                }
                else
                {
                    printf("upgrade ipc return");
                }
                break;
            }
		case '\n':
			{
				printf("********************************************************\n");
				printf("\tq : quit\n");
				printf("\t1 : display the device tree\n");
				printf("\t2 : add region(parentId regionname)\n");
				printf("\t3 : add device(parentId name addr port user pwd chnnNum)\n");
				printf("\t4 : remove region\n");
				printf("\t5 : remove device\n");
				printf("\t6 : modify region\n");
				printf("\t7 : modify device\n");
				printf("\t8 : add group\n");
				printf("\t9 : add channel to group\n");
				printf("\ta : remove group\n");
				printf("\tb : remove channel from group\n");
				printf("\tc : modify group\n");
				printf("\td : display group\n");
				printf("\te : start live stream\n");
				printf("\tf : stop live stream\n");
				printf("\tg : search record data\n");
				printf("\th : start download\n");
				printf("\ti : get download progress\n");
				printf("\tj : stop download\n");
				printf("\tk : start playback\n");
				printf("\tl : stop playback\n");
				printf("\tm : start talk\n");
				printf("\tn : stop talk\n");
				printf("\to : get device current time\n");
				printf("\tp : capture JPEG data\n");
                printf("\tv : get vehicle alarm info\n");
                printf("\ts : search device\n");
                printf("\tu : upgrade device\n");
				printf("********************************************************\n");
				printf("Please select an option:\n");
				break;
			}
		default:
			{
				printf("You entered an invalid option.\n");
			}
		}
	}

	m_pDeviceMan->Stop();
	m_pDeviceMan->Quit();

	NET_SDK_Cleanup();

	return 0;
}

