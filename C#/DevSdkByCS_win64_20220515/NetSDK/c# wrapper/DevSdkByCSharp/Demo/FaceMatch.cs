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
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Demo
{
    partial class Live_Dialog
    {
        private Dictionary<int, NET_SDK_FACE_INFO_GROUP_ITEM> m_groupList = new Dictionary<int, NET_SDK_FACE_INFO_GROUP_ITEM>();
        private Dictionary<int, NET_SDK_FACE_INFO_LIST_ITEM> m_faceInfoList = new Dictionary<int, NET_SDK_FACE_INFO_LIST_ITEM>();
        private Dictionary<int, NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO> m_ipcFaceInfoList = new Dictionary<int, NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO>();
        private Dictionary<int, NET_SDK_FACE_IMG_INFO_CH> m_chSnapFaceList = new Dictionary<int, NET_SDK_FACE_IMG_INFO_CH>();
        private Dictionary<int, NET_SDK_SEARCH_IMAGE_ITEM> m_searchImgList = new Dictionary<int, NET_SDK_SEARCH_IMAGE_ITEM>();
        private Dictionary<int, NET_SDK_FACE_MATCH_ALARM_TRIGGER> m_faceMatchAlarmList = new Dictionary<int, NET_SDK_FACE_MATCH_ALARM_TRIGGER>();
        private Dictionary<int, Searched_IPC_FACE_INFO> m_searchedIpcFaceList = new Dictionary<int, Searched_IPC_FACE_INFO>();

        private UInt32 similarity;
        private UInt32 faceFeatureGroupsNum;
        private UInt32[] enableCH;
             
        public const int groupMaxNum = 16;//the max num of group is 16
        private List<FaceInfo> Faces = new List<FaceInfo>();
        private void FaceMatchInit()
        {
            uint lpBytesReturned = 0;
            //int size = sizeof(int);
            int size = Marshal.SizeOf(typeof(int));
            IntPtr sp = Marshal.AllocHGlobal(size);
            int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_GET_FACE_MATCH_SUPPORT;
            bool bResult = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, IntPtr.Zero, 0, sp, (uint)size, ref lpBytesReturned);
            int iSupport = Marshal.ReadInt32(sp);
            //int iSupport = (int)Marshal.PtrToStructure(sp, typeof(int));
            if (iSupport != 1)
            {
                //foreach (Control ct in tabPage5.Controls)
                    //ct.Enabled = false;                
            }
            Marshal.FreeHGlobal(sp);
            if (iSupport != 1)
                return;
            int chnCount = NET_SDK_CH_DEVICE_STATUS.GetSize() * oNET_SDK_DEVICEINFO.videoInputNum;
            NET_SDK_CH_DEVICE_STATUS[] chnInfo = new NET_SDK_CH_DEVICE_STATUS[chnCount];
            IntPtr intptrChnInfo = PointArrayToIntPtr(chnInfo);
            IntPtr pTmp = intptrChnInfo;
            int bufferSize = NET_SDK_CH_DEVICE_STATUS.GetSize() * chnCount;
            bool ret = DevSdkHelper.NET_SDK_GetDeviceCHStatus(userId, intptrChnInfo, bufferSize, ref chnCount);
            if (ret)
            {
                for (int i = 0; i < chnCount; i++, pTmp = IntPtr.Add(pTmp, NET_SDK_CH_DEVICE_STATUS.GetSize()))
                {
                    byte[] pByte = new byte[NET_SDK_CH_DEVICE_STATUS.GetSize()];
                    Marshal.Copy(pTmp, pByte, 0, NET_SDK_CH_DEVICE_STATUS.GetSize());
                    chnInfo[i] = (NET_SDK_CH_DEVICE_STATUS)BytesToStruct(pByte, typeof(NET_SDK_CH_DEVICE_STATUS));
                    cb_CH.Items.Add(chnInfo[i].channel);
                    //cb_CH.Items.Add("Channel:" + chnInfo[i].channel);
                }
            }
            Marshal.FreeHGlobal(intptrChnInfo);


            //cb_CH.SelectedIndex = 0;

            DateTime now = DateTime.Now;
            DateTime tempDt = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            dtp_face_start.Value = tempDt;
            dtp_face_start1.Value = tempDt;
            tempDt = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            dtp_face_end.Value = tempDt;          
            dtp_face_end1.Value = tempDt;
            if (oNET_SDK_DEVICEINFO.deviceType == 2)//IPC
            {
                pictureBox3.Visible = true;
                pictureBox4.Visible = true;
            }
            else
            {
                pictureBox3.Visible = false;
                pictureBox4.Visible = false;
            }
        }
        private void btn_Get_Groups_Click(object sender, EventArgs e)
        {
            if (oNET_SDK_DEVICEINFO.deviceType == 2)//IPC
            {
                lv_FaceInfo.Items.Clear();
                m_faceInfoList.Clear();

                NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_INFO pAlbumInfo = new NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_INFO();
                pAlbumInfo.iPageNum = 0;
                pAlbumInfo.iPageSize = 10;
                pAlbumInfo.bUseTypeFilter = 1;
                pAlbumInfo.iType = 1;

                int tempSize = Marshal.SizeOf(pAlbumInfo);
                IntPtr tempBuf = Marshal.AllocHGlobal(tempSize);
                Marshal.StructureToPtr(pAlbumInfo, tempBuf, true);

                IntPtr outBuf = Marshal.AllocHGlobal(1000 * 1024);
                uint retLen = 0;
                int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_GET_FACE_IPC_LIST;
                bool bResult = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, tempBuf, (uint)tempSize, outBuf, 1000 * 1024, ref retLen);
                Marshal.FreeHGlobal(tempBuf);
                if (bResult && retLen > 0)
                {
                    IntPtr copyOutBuf = outBuf;

                    int totalNum = (int)Marshal.PtrToStructure(copyOutBuf, typeof(int));//总数量
                    copyOutBuf += Marshal.SizeOf(typeof(int));

                    int currentPageNum = (int)Marshal.PtrToStructure(copyOutBuf, typeof(int));//当前页的数量
                    copyOutBuf += Marshal.SizeOf(typeof(int));
                    m_ipcFaceInfoList.Clear();
                    for (int i = 0; i < currentPageNum; i++)
                    {
                        NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO replyAlbum = new NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO();
                        replyAlbum = (NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO)Marshal.PtrToStructure(copyOutBuf, typeof(NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO));
                        copyOutBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO));
                        copyOutBuf += replyAlbum.stBaseInfo.iPicSize;

                        string name = DevSdkHelper.ByteToStr(replyAlbum.stBaseInfo.szName);
                        ListViewItem item = new ListViewItem(new string[] { (i + 1).ToString(), replyAlbum.iKey.ToString(), name });
                        lv_FaceInfo.Items.Add(item);
                        m_ipcFaceInfoList.Add(lv_FaceInfo.Items.Count, replyAlbum);
                    }

                }
                Marshal.FreeHGlobal(outBuf);
//                lv_FaceInfo.Items.Clear();
//                m_faceInfoList.Clear();

//                string res = "";
//                string num = "";
//                string szUrl = "GetTargetFace";
//                string sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
//                                                <config version='1.7' xmlns='http://www.ipc.com/ver10'>//                                                <types><queryType><enum>byPersonID</enum>//                                                <enum>byListType</enum><enum>byName</enum>//                                                <enum>byIdentifyNumber</enum></queryType><listType>//                                                <enum>strangerList</enum><enum>whiteList</enum>//                                                <enum>blackList</enum></listType></types></config>";
//                int lpBytesReturned = 0;
//                byte[] rec = new byte[1024 * 1024];
//                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
//                bool ret = DevSdkHelper.NET_SDK_ApiInterface(userId, sendXml, szUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
//                if (ret)
//                {
//                    Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
//                    res = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
//                    System.Xml.XmlNode tempnode = CommonMethods.GetNodePosByIndex(res, "personID");
//                    num = CommonMethods.GetNodeAttribute(tempnode, "count");
//                }
//                if (num.Length > 0)
//                {
//                    for (int i = 0; i < Convert.ToInt16(num); i++)
//                    {
//                        NET_SDK_FACE_INFO_LIST_ITEM Face_Item = new NET_SDK_FACE_INFO_LIST_ITEM();
//                        Face_Item.itemId = Convert.ToUInt32(CommonMethods.GetNodeValue(res, "item", i));
//                        ListViewItem item = new ListViewItem(ChangeToItems(Face_Item, i + 1));
//                        lv_FaceInfo.Items.Add(item);
//                        m_faceInfoList.Add(lv_FaceInfo.Items.Count, Face_Item);
//                    }
//                }
//                Marshal.FreeHGlobal(lpOutBuffer);
                
            }
            else if (oNET_SDK_DEVICEINFO.deviceType == 3)//NVR
            {
                lv_FaceGroup.Items.Clear();
                m_groupList.Clear();

                uint lpBytesReturned = 0;
                IntPtr sp = Marshal.AllocHGlobal(sizeof(int));
                int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_GET_FACE_INFO_GROUP_LIST;
                bool bResult = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, 1, IntPtr.Zero, 0, sp, sizeof(int), ref lpBytesReturned);
                int iSupport = (int)Marshal.PtrToStructure(sp, typeof(int));
                if (iSupport != 1)
                {
                    MessageBox.Show("This device doesn't support faceMatch!");
                    return; //support face match continue
                }
                //else
                //    MessageBox.Show("OK!");
                Marshal.FreeHGlobal(sp);

                NET_SDK_FACE_INFO_GROUP_ITEM[] g_Group_Item = new NET_SDK_FACE_INFO_GROUP_ITEM[groupMaxNum];
                int g_GroupNum = 0;
                lpBytesReturned = 0;
                NET_SDK_FACE_INFO_GROUP_ITEM Group_Item = new NET_SDK_FACE_INFO_GROUP_ITEM();
                int nSizeOfGroupItem = Marshal.SizeOf(Group_Item);
                IntPtr intPtrGroupItem = Marshal.AllocHGlobal(nSizeOfGroupItem * 16);
                bResult = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, IntPtr.Zero, 0, intPtrGroupItem, (UInt32)nSizeOfGroupItem * 16, ref lpBytesReturned);
                if (bResult)
                {
                    for (int i = 0; i < lpBytesReturned / nSizeOfGroupItem; ++i, ++g_GroupNum)
                    {
                        g_Group_Item[i] = (NET_SDK_FACE_INFO_GROUP_ITEM)Marshal.PtrToStructure(intPtrGroupItem + nSizeOfGroupItem * i, typeof(NET_SDK_FACE_INFO_GROUP_ITEM));

                        ListViewItem item = new ListViewItem(ChangeToItems(g_Group_Item[i], i + 1));

                        lv_FaceGroup.Items.Add(item);
                        m_groupList.Add(lv_FaceGroup.Items.Count, g_Group_Item[i]);
                    }
                }
                else
                    ShowErrorMessage();
                Marshal.FreeHGlobal(intPtrGroupItem);
            }

        }
        private string[] ChangeToItems(NET_SDK_FACE_INFO_GROUP_ITEM file,int index)
        {
            string strIndex = index.ToString();
            string id = file.groupId.ToString();
            string strGuid = Encoding.Default.GetString(file.guid);
            string strName = Encoding.UTF8.GetString(file.name);

            string[] tempStrings = { strIndex, id, strGuid, strName};
            return tempStrings;
        }
        private void btn_Del_Groups_Click(object sender, EventArgs e)
        {
            if (lv_FaceGroup.SelectedItems.Count > 0)
            {
                int Index = lv_FaceGroup.SelectedItems[0].Index;
                if (Index < 3)
                {
                    MessageBox.Show("White list and black list can't be delete");
                    return;
                }
                NET_SDK_FACE_INFO_GROUP_ITEM temp = m_groupList[Index + 1];
                NET_SDK_FACE_INFO_GROUP_DEL itemToBeDel = new NET_SDK_FACE_INFO_GROUP_DEL();
                itemToBeDel.guid = temp.guid;
                int nSizeOfGroupDel = Marshal.SizeOf(itemToBeDel);
                IntPtr intPtrGroupDel = Marshal.AllocHGlobal(nSizeOfGroupDel);
                Marshal.StructureToPtr(itemToBeDel, intPtrGroupDel, true);
                uint lpBytesReturned = 0;
                int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_DEL_FACE_INFO_GROUP;
                bool bResult = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrGroupDel, (UInt32)nSizeOfGroupDel, IntPtr.Zero, 0, ref lpBytesReturned);
                if (bResult)
                {
                    MessageBox.Show("Delete success!");
                    btn_Get_Groups_Click(null, null);
                }
                else
                    ShowErrorMessage();
                Marshal.FreeHGlobal(intPtrGroupDel);

            }
        }
        private void btn_Add_Groups_Click(object sender, EventArgs e)
        {
            string groupName = tb_Group.Text;
            if (groupName.Length > 0)
            {
                uint lpBytesReturned = 0;
                int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_ADD_FACE_INFO_GROUP;
                NET_SDK_FACE_INFO_GROUP_ADD Group_Add = new NET_SDK_FACE_INFO_GROUP_ADD();
                Group_Add.name = new byte[64];
                byte[] buffer = Encoding.UTF8.GetBytes(groupName.ToString());
                Array.Copy(buffer, Group_Add.name, buffer.Length);
                Group_Add.property = (UInt32)NET_SDK_FACE_INFO_GROUP_PROPERTY_TYPE.NET_SDK_FACE_INFO_GROUP_PROPERTY_ALLOW;
                int nSizeOfGroupAdd = Marshal.SizeOf(Group_Add);
                IntPtr intPtrGroupAdd = Marshal.AllocHGlobal(nSizeOfGroupAdd);
                Marshal.StructureToPtr(Group_Add, intPtrGroupAdd, true);
                bool bResult = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrGroupAdd, (UInt32)nSizeOfGroupAdd, IntPtr.Zero, 0, ref lpBytesReturned);
                if (bResult)
                {
                    MessageBox.Show("Add group success!");
                    btn_Get_Groups_Click(null, null);
                }
                else
                    ShowErrorMessage();
                Marshal.FreeHGlobal(intPtrGroupAdd);
            }
            else           
                MessageBox.Show("GroupName is null !");
            
        }
        private void btn_Edit_Groups_Click(object sender, EventArgs e)
        {
            if (lv_FaceGroup.SelectedItems.Count > 0)
            {
                int Index = lv_FaceGroup.SelectedItems[0].Index;

                string groupName = tb_Group.Text;
                if (groupName.Length > 0)
                {
                    uint lpBytesReturned = 0;
                    int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SET_FACE_INFO_GROUP;
                    NET_SDK_FACE_INFO_GROUP_ITEM Group_Set = new NET_SDK_FACE_INFO_GROUP_ITEM();
                    Group_Set = m_groupList[Index + 1];
                    Group_Set.name = new byte[64];
                    byte[] buffer = Encoding.UTF8.GetBytes(groupName.ToString());
                    Array.Copy(buffer, Group_Set.name, buffer.Length);
                    int nSizeOfGroupSet = Marshal.SizeOf(Group_Set);
                    IntPtr intPtrGroupSet = Marshal.AllocHGlobal(nSizeOfGroupSet);
                    Marshal.StructureToPtr(Group_Set, intPtrGroupSet, true);
                    bool bResult = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrGroupSet, (UInt32)nSizeOfGroupSet, IntPtr.Zero, 0, ref lpBytesReturned);
                    if (bResult)
                    {
                        MessageBox.Show("Edit group success!");
                        btn_Get_Groups_Click(null, null);
                    }
                    else
                        ShowErrorMessage();
                    Marshal.FreeHGlobal(intPtrGroupSet);
                }
                else
                    MessageBox.Show("GroupName is null !");
            }
            else
                MessageBox.Show("No group is selected !");
        }
        private void lv_FaceGroup_MouseDoubleClick(object sender, MouseEventArgs e)
        {
             ListViewHitTestInfo info = lv_FaceGroup.HitTest(e.X, e.Y);
             if (info != null)
             {
                 var videoitem = info.Item as ListViewItem;


                 lv_FaceInfo.Items.Clear();
                 m_faceInfoList.Clear();

                 uint lpBytesReturned = 0;
                 int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_GET_FACE_INFO_LIST;

                 NET_SDK_FACE_INFO_LIST_GET Face_List_Get = new NET_SDK_FACE_INFO_LIST_GET();    //input parameter
                 NET_SDK_FACE_INFO_LIST Face_List = new NET_SDK_FACE_INFO_LIST();                                //output parameter
                 NET_SDK_FACE_INFO_LIST_ITEM Face_Item = new NET_SDK_FACE_INFO_LIST_ITEM();        //sub item of output
                 Face_List_Get.pageSize = 15;
                 Face_List_Get.pageIndex = 1;
                 Face_List_Get.groupId = m_groupList[videoitem.Index + 1].groupId;
                 int nSizeOfFaceGet = Marshal.SizeOf(Face_List_Get);
                 IntPtr intPtrFaceGet = Marshal.AllocHGlobal(nSizeOfFaceGet);
                 Marshal.StructureToPtr(Face_List_Get, intPtrFaceGet, true);

                 int nSizeOfFaceItem = Marshal.SizeOf(Face_Item);
                 int nSizeOfFace = Marshal.SizeOf(Face_List);
                 IntPtr intPtrFace = Marshal.AllocHGlobal(nSizeOfFace + nSizeOfFaceItem * 5);
                 Marshal.StructureToPtr(Face_List, intPtrFace, true);
                 bool  bResult = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrFaceGet, (UInt32)nSizeOfFaceGet, intPtrFace, (UInt32)(nSizeOfFace + nSizeOfFaceItem*5), ref lpBytesReturned);
                 if (bResult)
                 {
                     Face_List = (NET_SDK_FACE_INFO_LIST)Marshal.PtrToStructure(intPtrFace, typeof(NET_SDK_FACE_INFO_LIST));
                     for (int i = 0; i < Face_List.listNum; ++i)
                     {
                         Face_Item = (NET_SDK_FACE_INFO_LIST_ITEM)Marshal.PtrToStructure(Face_List.pFaceInfoListItem + i * nSizeOfFaceItem, typeof(NET_SDK_FACE_INFO_LIST_ITEM));

                         ListViewItem item = new ListViewItem(ChangeToItems(Face_Item, i + 1));

                         lv_FaceInfo.Items.Add(item);
                         m_faceInfoList.Add(lv_FaceInfo.Items.Count, Face_Item);
                     }
                 }
                 else
                     ShowErrorMessage();
                 Marshal.FreeHGlobal(intPtrFaceGet);
                 Marshal.FreeHGlobal(intPtrFace);
             }
        }
        private string[] ChangeToItems(NET_SDK_FACE_INFO_LIST_ITEM file,int index)
        {
            string strIndex = index.ToString();
            string id = file.itemId.ToString();
            if(file.name == null)
                return new string[] { strIndex, id };
            else
                return new string[] { strIndex, id, Encoding.UTF8.GetString(file.name) };
                
        }
        private IPC_FACE_INFO GetIpcFaceInfoById(uint id)
        {
            IPC_FACE_INFO resFace = new IPC_FACE_INFO();
            string res = "";
            string szUrl = "GetTargetFace";
            string tempXML = @"<?xml version = '1.0' encoding='utf-8'?>
                                                <config version='1.7' xmlns='http://www.ipc.com/ver10'>                                                <types> 
                                                    <queryType> 
                                                      <enum>byPersonID</enum>  
                                                      <enum>byListType</enum>  
                                                      <enum>byName</enum>  
                                                      <enum>byIdentifyNumber</enum> 
                                                    </queryType>  
                                                    <listType> 
                                                      <enum>strangerList</enum>  
                                                      <enum>whiteList</enum>  
                                                      <enum>blackList</enum> 
                                                    </listType> 
                                                  </types>  
                                                  <queryAction> 
                                                    <queryType type='queryType'>byPersonID</queryType>  
                                                    <personID type='uint32'>{0}</personID> 
                                                  </queryAction> 
                                                </config>";
            string sendXml = string.Format(tempXML, id);
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, sendXml, szUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                res = DevSdkHelper.ByteToStr(rec,false);
                //res = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                resFace.listType = CommonMethods.GetNodeValue(res, "listType", 1);
                resFace.name = CommonMethods.GetNodeValue(res, "name");
                resFace.sex = CommonMethods.GetNodeValue(res, "sex");
                resFace.age = CommonMethods.GetNodeValue(res, "age");
                resFace.identifyNumber = CommonMethods.GetNodeValue(res, "identifyNumber");
                resFace.telephone = CommonMethods.GetNodeValue(res, "telephone");
                resFace.pictureData = CommonMethods.GetNodeValue(res, "pictureData");
            }
            resFace.itemId = id.ToString();
            return resFace;
        }
        private void LoadPicture(byte[] data,PictureBox pb)
        {
            if (data.Length < 1)
                return;
            MemoryStream stream = new MemoryStream(data);
            Image img = Image.FromStream(stream);
            //将图像序列化给指定的流，再将流序列化为二进制数组。
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream mstream = new MemoryStream();
            binaryFormatter.Serialize(mstream, img);
            byte[] ImgByte = mstream.ToArray();
            mstream.Close();
            stream.Close();
            //将二进制数组放到流中，再反序列为图像
            MemoryStream memostream = new MemoryStream(ImgByte);
            this.Invoke(new Action(() => { pb.Image = (Image)binaryFormatter.Deserialize(memostream); }));            
            memostream.Close();

        }
        private void lv_FaceInfo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lv_FaceInfo.HitTest(e.X, e.Y);
            if (info != null)
            {
                var videoitem = info.Item as ListViewItem;               
                if (oNET_SDK_DEVICEINFO.deviceType == 2)//IPC
                {
                    //IPC_FACE_INFO resFace = GetIpcFaceInfoById(selectFace.itemId);
                    //tb_Face_Name.Text = resFace.name;
                    //tb_Face_CertificateNum.Text = resFace.identifyNumber;
                    //tb_Face_Mobile.Text = resFace.telephone;
                    //LoadPicture(Convert.FromBase64String(resFace.pictureData), pictureBox1);
                    NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO selectIPCFace = m_ipcFaceInfoList[videoitem.Index + 1];
                    NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_INFO pAlbumInfo = new NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_INFO();
                    pAlbumInfo.iPageNum = 0;
                    pAlbumInfo.iPageSize = 10;
                    //pAlbumInfo.bUseTypeFilter = true;
                    //pAlbumInfo.iType = 1;

                    //pAlbumInfo.bUseKeyFilter = true;
                    //pAlbumInfo.iKey = selectIPCFace.iKey;

                    int tempSize = Marshal.SizeOf(pAlbumInfo);
                    IntPtr tempBuf = Marshal.AllocHGlobal(tempSize);
                    Marshal.StructureToPtr(pAlbumInfo, tempBuf, true);

                    IntPtr outBuf = Marshal.AllocHGlobal(1000 * 1024);
                    uint retLen = 0;
                    int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_GET_FACE_IPC_LIST;
                    bool bResult = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, tempBuf, (uint)tempSize, outBuf, 1000 * 1024, ref retLen);
                    Marshal.FreeHGlobal(tempBuf);
                    if (bResult && retLen > 0)
                    {
                        IntPtr copyOutBuf = outBuf;

                        int  totalNum= (int)Marshal.PtrToStructure(copyOutBuf, typeof(int));//总数量
                        copyOutBuf += Marshal.SizeOf(typeof(int));

                        int currentPageNum = (int)Marshal.PtrToStructure(copyOutBuf, typeof(int));//当前页的数量
                        copyOutBuf += Marshal.SizeOf(typeof(int));

                        for (int i = 0; i < currentPageNum; i++)
                        {
                            NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO replyAlbum = new NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO();
                            replyAlbum = (NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO)Marshal.PtrToStructure(copyOutBuf, typeof(NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO));

                            if (replyAlbum.iKey == selectIPCFace.iKey)
                            {
                                copyOutBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO));
                                byte[] picData = new byte[replyAlbum.stBaseInfo.iPicSize];
                                Marshal.Copy(copyOutBuf, picData, 0, replyAlbum.stBaseInfo.iPicSize);
                                LoadPicture(picData, pictureBox1);
                                Invoke(new Action(() =>
                                {
                                    tb_Face_Name.Text = DevSdkHelper.ByteToStr(replyAlbum.stBaseInfo.szName);
                                    tb_Face_CertificateNum.Text = DevSdkHelper.ByteToStr(replyAlbum.stBaseInfo.szIdentifyNum);
                                    tb_Face_Mobile.Text = DevSdkHelper.ByteToStr(replyAlbum.stBaseInfo.szTel);
                                }));
                                break;
                            }
                            else
                            {
                                copyOutBuf += Marshal.SizeOf(typeof(NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO));
                                copyOutBuf += replyAlbum.stBaseInfo.iPicSize;
                            }                         
                        }
                    }
                    Marshal.FreeHGlobal(outBuf);

                }
                else if (oNET_SDK_DEVICEINFO.deviceType == 3)//NVR
                {
                    NET_SDK_FACE_INFO_LIST_ITEM selectFace = m_faceInfoList[videoitem.Index + 1];
                    tb_Face_Name.Text = Encoding.UTF8.GetString(selectFace.name);
                    tb_Face_Birthday.Text = selectFace.birthday.ToString();
                    tb_Face_NativePlace.Text = Encoding.UTF8.GetString(selectFace.nativePlace);
                    tb_Face_CertificateNum.Text = Encoding.UTF8.GetString(selectFace.certificateNum);
                    tb_Face_Mobile.Text = Encoding.UTF8.GetString(selectFace.mobile);
                    tb_Face_Number.Text = Encoding.UTF8.GetString(selectFace.number);

                    uint lpBytesReturned = 0;
                    int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_GET_FACE_INFO_IMG;
                    NET_SDK_FACE_INFO_IMG_GET sFACE_INFO_IMG_GET = new NET_SDK_FACE_INFO_IMG_GET();//input parameter
                    NET_SDK_FACE_INFO_IMG_DATA sFaceImg = new NET_SDK_FACE_INFO_IMG_DATA();//output parameter

                    sFACE_INFO_IMG_GET.index = 1;
                    sFACE_INFO_IMG_GET.itemId = selectFace.itemId;

                    int nSizeOfFaceImgGet = Marshal.SizeOf(sFACE_INFO_IMG_GET);
                    IntPtr intPtrFaceImgGet = Marshal.AllocHGlobal(nSizeOfFaceImgGet);
                    Marshal.StructureToPtr(sFACE_INFO_IMG_GET, intPtrFaceImgGet, true);

                    int nSizeOfFaceImgData = 100 * 1024;
                    IntPtr intPtrFaceImgData = Marshal.AllocHGlobal(nSizeOfFaceImgData);

                    //bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrFaceImgGet, (UInt32)nSizeOfFaceImgGet, intPtrFaceImgData, (UInt32)nSizeOfFaceImgData, ref lpBytesReturned);
                    bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrFaceImgGet, (UInt32)nSizeOfFaceImgGet, intPtrFaceImgData, 100 * 1024, ref lpBytesReturned);
                    if (ret)
                    {
                        if (pictureBox1.Image != null)
                            pictureBox1.Image.Dispose();
                        pictureBox1.Image = null;

                        sFaceImg = (NET_SDK_FACE_INFO_IMG_DATA)(Marshal.PtrToStructure(intPtrFaceImgData, typeof(NET_SDK_FACE_INFO_IMG_DATA)));

                        byte[] data = new byte[sFaceImg.imgLen];
                        Marshal.Copy(sFaceImg.imgData, data, 0, (int)sFaceImg.imgLen);

                        MemoryStream stream = new MemoryStream(data);
                        pictureBox1.Image = Image.FromStream(stream);
                    }
                    else
                        ShowErrorMessage();
                    Marshal.FreeHGlobal(intPtrFaceImgData);
                    Marshal.FreeHGlobal(intPtrFaceImgGet);
                }
               

                
            }
        }
        private void btn_Add_Face_Click(object sender, EventArgs e)
        {           
            string faceName = tb_Face_Name.Text.Trim();


           

             string utf8_faceName = get_uft8(faceName);
             byte[] byteArray = System.Text.Encoding.Default.GetBytes(utf8_faceName);
            string testGB = Encoding.UTF8.GetString(byteArray);
            if (faceName.Length < 1)
            {
                MessageBox.Show("Face Name is empty !!");
                return;
            }
            UInt32 faceBirth = Convert.ToUInt32(tb_Face_Birthday.Text.Trim());
            if (tb_Face_Birthday.Text.Trim().Length < 1)
            {
                MessageBox.Show("Birthday is empty !!");
                return;
            }
            string nativePlace = tb_Face_NativePlace.Text.Trim();
            if (nativePlace.Length < 1)
            {
                MessageBox.Show("Native place is empty !!");
                return;
            }
            string certificateNum = tb_Face_CertificateNum.Text.Trim();
            if (certificateNum.Length < 1)
            {
                MessageBox.Show("Certificate number is empty !!");
                return;
            }
            string mobile = tb_Face_Mobile.Text.Trim();
            if (mobile.Length < 1)
            {
                MessageBox.Show("mobile is empty !!");
                return;
            }
            string number = tb_Face_Number.Text.Trim();
            if (number.Length < 1)
            {
                MessageBox.Show("number is empty !!");
                return;
            }
            if (pictureBox1.Image == null)
                MessageBox.Show("picture is empty !!");
            if (oNET_SDK_DEVICEINFO.deviceType == 2)//IPC
            {
                //MemoryStream ms = new MemoryStream();
                //pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                //byte[] picData = ms.ToArray();
                ////byte[] array = ms.ToArray();
                //ms.Close();
                //string imgData64 = Convert.ToBase64String(picData);
                //byte[] array = Encoding.UTF8.GetBytes(imgData64);

                //int pSize = Marshal.SizeOf(typeof(NET_SDK_IVE_FACE_MATCH_ADD_ALBUM_INFO)) + array.Length;
                //IntPtr inBuff = Marshal.AllocHGlobal(pSize);

                //NET_SDK_IVE_FACE_MATCH_ADD_ALBUM_INFO targetInfo = new NET_SDK_IVE_FACE_MATCH_ADD_ALBUM_INFO();
                //targetInfo.szName = new byte[128];
                //byte[] tempName = Encoding.UTF8.GetBytes(faceName);
                //System.Array.Copy(tempName, targetInfo.szName, tempName.Length);
                ////targetInfo.szName
                //Marshal.StructureToPtr(targetInfo, inBuff, true);
                //IntPtr copyInBuff = inBuff;
                //copyInBuff += Marshal.SizeOf(typeof(NET_SDK_IVE_FACE_MATCH_ADD_ALBUM_INFO));
                //Marshal.Copy(array, 0, copyInBuff, array.Length);

                //uint dwCommand = (uint)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_ADD_FACE_IPC;
                //IntPtr outBuff = Marshal.AllocHGlobal(1024);
                //int retLen = 0;
                //bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, inBuff, (uint)pSize, outBuff, 1024, ref retLen);
                //Marshal.FreeHGlobal(inBuff);
                //if (ret && retLen > 0)
                //{
                //    NET_SDK_IVE_FACE_MATCH_ADD_FACE_REPLY_T reply = new NET_SDK_IVE_FACE_MATCH_ADD_FACE_REPLY_T();
                //    reply = (NET_SDK_IVE_FACE_MATCH_ADD_FACE_REPLY_T)Marshal.PtrToStructure(outBuff, typeof(NET_SDK_IVE_FACE_MATCH_ADD_FACE_REPLY_T));

                //    MessageBox.Show(reply.dwResult.ToString());
                //}
                //Marshal.FreeHGlobal(outBuff);



                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] array = ms.ToArray();
                ms.Close();
                string imgData64 = Convert.ToBase64String(array);
                string szUrl = "AddTargetFace";
                string tempXML = @"<?xml version = '1.0' encoding='UTF-8'?>
                                                <config version='1.7' xmlns='http://www.ipc.com/ver10' >
                                                <types>
                                                <listType> 
                                                  <enum>strangerList</enum>  
                                                  <enum>whiteList</enum>  
                                                  <enum>blackList</enum> 
                                                </listType>  
                                                <sexType> 
                                                  <enum>male</enum>  
                                                  <enum>female</enum> 
                                                </sexType>  
                                                <formatType> 
                                                  <enum>jpg</enum>  
                                                </formatType> 
                                                </types>
                                                <personInfo>
                                                <listType type='listType'>{0}</listType> 
                                                <name type='string' maxLen='127'><![CDATA[{1}]]></name>  
                                                <sex type='sexType'>{2}</sex>
                                                <age type='uint32'>{3}</age>
                                                <identifyNumber type='string' maxLen='127'><![CDATA[{4}]]></identifyNumber>
                                                <telephone type='string' maxLen='63'><![CDATA[{5}]]></telephone>
                                                </personInfo>
                                                <faceImgs type='list' maxCount='5' count='{6}'>
                                                <item>
                                                <pictureData><![CDATA[{7}]]></pictureData>
                                                <pictureNum>{8}</pictureNum>
                                                <Width>{9}</Width>
                                                <Height>{10}</Height>
                                                <Height>{10}</Height>
                                                <format>{11}</format>
                                                <size>{12}</size>
                                                </item>
	                                            </faceImgs>
                                            </config>";
                //string sendXML = string.Format(tempXML, "whiteList", faceName, "female", "18", certificateNum, mobile, 1, imgData64,
                //    1, tempSize.Width, tempSize.Height, "jpg", aa);
                byte[] bufferFaceName = Encoding.UTF8.GetBytes(faceName);
                string str = System.Text.Encoding.Default.GetString(bufferFaceName);
                string sendXML = string.Format(tempXML, "whiteList", faceName, "female", "18", certificateNum, mobile, 1, imgData64,
                    1, pictureBox1.Image.Width, pictureBox1.Image.Height, "jpg", array.Length);
                //string sendXML = string.Format(tempXML, "whiteList", get_uft8(faceName), "female", "18", certificateNum, mobile, 1, imgData64,
                //    1, pictureBox1.Image.Width, pictureBox1.Image.Height, "jpg", array.Length);

                string utf8_SendXml = get_uft8(sendXML);

                int lpBytesReturned = 0;

                Log abc = new Log();
                abc.Initial(true, Application.StartupPath, "AddfaceLog");//
                for (int jj = 0; jj < 1; jj++)
                {
                    IntPtr lpOutBuffer = Marshal.AllocHGlobal(100 * 1024);
                    bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, utf8_SendXml, szUrl, lpOutBuffer, 100 * 1024, ref lpBytesReturned);
                    if (ret)
                    {
                        //btn_Get_Groups.PerformClick();
                        //MessageBox.Show("add ipc face image success");
                        byte[] rec = new byte[lpBytesReturned];
                        Marshal.Copy(lpOutBuffer, rec, 0, lpBytesReturned);
                        string res = DevSdkHelper.ByteToStr(rec);
                        //MessageBox.Show(res);

                        abc.WriteLog(res, 0);
                    }
                    else
                        abc.WriteLog("add face failed", 0);
                    Marshal.FreeHGlobal(lpOutBuffer);
                    Thread.Sleep(2000);
                }
                
			
            }
            else if (oNET_SDK_DEVICEINFO.deviceType == 3)//NVR
            {
                if (lv_FaceGroup.SelectedItems.Count < 1)
                {
                    MessageBox.Show("select GroupList first !!");
                    return;
                }
                int Index = lv_FaceGroup.SelectedItems[0].Index;
                NET_SDK_FACE_INFO_GROUP_ITEM targetGroup = m_groupList[Index + 1];

                NET_SDK_FACE_INFO_ADD Face_Add = new NET_SDK_FACE_INFO_ADD();
                Face_Add.sFaceInfoItem = new NET_SDK_FACE_INFO_LIST_ITEM();

                Face_Add.sFaceInfoItem.name = new byte[64];
                byte[] bufferFaceName = Encoding.UTF8.GetBytes(faceName);
                Array.Copy(bufferFaceName, Face_Add.sFaceInfoItem.name, bufferFaceName.Length);

                Face_Add.sFaceInfoItem.birthday = faceBirth;

                Face_Add.sFaceInfoItem.nativePlace = new byte[64];
                byte[] bufferNative = Encoding.UTF8.GetBytes(nativePlace);
                Array.Copy(bufferNative, Face_Add.sFaceInfoItem.nativePlace, bufferNative.Length);

                Face_Add.sFaceInfoItem.certificateNum = new byte[20];
                byte[] bufferCertify = Encoding.UTF8.GetBytes(certificateNum);
                Array.Copy(bufferCertify, Face_Add.sFaceInfoItem.certificateNum, bufferCertify.Length);

                Face_Add.sFaceInfoItem.mobile = new byte[20];
                byte[] bufferMobile = Encoding.UTF8.GetBytes(mobile);
                Array.Copy(bufferMobile, Face_Add.sFaceInfoItem.mobile, bufferMobile.Length);

                Face_Add.sFaceInfoItem.number = new byte[20];
                byte[] bufferNumber = Encoding.UTF8.GetBytes(number);
                Array.Copy(bufferNumber, Face_Add.sFaceInfoItem.number, bufferNumber.Length);

                Face_Add.sFaceInfoItem.groups = new NET_SDK_FACE_INFO_LIST_ITEM_GROUPS[16];
                Face_Add.sFaceInfoItem.groups[0].guid = targetGroup.guid;
                Face_Add.sFaceInfoItem.groups[0].groupId = targetGroup.groupId;
                Face_Add.sFaceInfoItem.itemId = 1;
                Face_Add.imgNum = 1;
                Face_Add.haveImgData = 1;
                Face_Add.sFaceImgInfo = new NET_SDK_FACE_IMG_INFO_CH[5];

                //byte[] data = new byte[sFaceImg.imgLen];
                //Marshal.Copy(sFaceImg.imgData, data, 0, (int)sFaceImg.imgLen);
                //MemoryStream stream = new MemoryStream(data);
                //pictureBox1.Image = Image.FromStream(stream);

                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] array = ms.ToArray();

                Face_Add.imgData = Marshal.AllocHGlobal(array.Length);
                Marshal.Copy(array, 0, Face_Add.imgData, array.Length);
                Face_Add.imgWidth = 256;    //timg.jpg的宽 //图片必须是jpg的，高宽可以不传
                Face_Add.imgHeight = 256;   //timg.jpg的高
                Face_Add.imgLen = (UInt32)array.Length;

                int nSizeOfFaceAdd = Marshal.SizeOf(Face_Add);
                IntPtr intPtrFaceAdd = Marshal.AllocHGlobal(nSizeOfFaceAdd + array.Length);
                Marshal.StructureToPtr(Face_Add, intPtrFaceAdd, true);

                uint lpBytesReturned = 0;
                int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_ADD_FACE_INFO;

                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);

                bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrFaceAdd, (UInt32)(nSizeOfFaceAdd + array.Length), lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret)
                {
                    //MessageBox.Show("add face information success !");                   
                    int faceId = (int)Marshal.PtrToStructure(lpOutBuffer, typeof(int));
                      
                }
                else
                    ShowErrorMessage();
                Marshal.FreeHGlobal(intPtrFaceAdd);
                Marshal.FreeHGlobal(lpOutBuffer);
            }


        }
        private string GetFaceNum()
        {
            string num = "";
            string szUrl = "GetTargetFace";
            string sendXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                                <config xmlns='http://www.ipc.com/ver10' version='1.0'>                                                <types><queryType><enum>byPersonID</enum>                                                <enum>byListType</enum><enum>byName</enum>                                                <enum>byIdentifyNumber</enum></queryType><listType>                                                <enum>strangerList</enum><enum>whiteList</enum>                                                <enum>blackList</enum></listType></types></config>";
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, sendXml, szUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string res = Encoding.ASCII.GetString(rec);
                System.Xml.XmlNode tempnode = CommonMethods.GetNodePosByIndex(res, "personID");
                num = CommonMethods.GetNodeAttribute(tempnode, "count");
            }
            Marshal.FreeHGlobal(lpOutBuffer);
            return num;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "jpg files(*.jpg)|*.jpg||";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                FileStream file = new FileStream(path, FileMode.Open);
                byte[] b = new byte[file.Length];
                file.Read(b,0,b.Length);

                //MemoryStream stream = new MemoryStream(b);
                //if (pictureBox1.Image != null)
                //    pictureBox1.Image.Dispose();
                //pictureBox1.Image = null;
                //pictureBox1.Image = Image.FromStream(stream);
                //stream.Close();
                file.Close();
                LoadPicture(b, pictureBox1);
            }
        }

        private void btn_Edit_Face_Click(object sender, EventArgs e)
        {           
            if (lv_FaceInfo.SelectedItems.Count < 1)
            {
                MessageBox.Show("select face first !!");
                return;
            }
            string faceName = tb_Face_Name.Text.Trim();
            if (faceName.Length < 1)
            {
                MessageBox.Show("Face Name is empty !!");
                return;
            }            
            if (tb_Face_Birthday.Text.Trim().Length < 1)
            {
                MessageBox.Show("Birthday is empty !!");
                return;
            }
            UInt32 faceBirth = Convert.ToUInt32(tb_Face_Birthday.Text.Trim());
            string nativePlace = tb_Face_NativePlace.Text.Trim();
            if (nativePlace.Length < 1)
            {
                MessageBox.Show("Native place is empty !!");
                return;
            }
            string certificateNum = tb_Face_CertificateNum.Text.Trim();
            if (certificateNum.Length < 1)
            {
                MessageBox.Show("Certificate number is empty !!");
                return;
            }
            string mobile = tb_Face_Mobile.Text.Trim();
            if (mobile.Length < 1)
            {
                MessageBox.Show("mobile is empty !!");
                return;
            }
            string number = tb_Face_Number.Text.Trim();
            if (number.Length < 1)
            {
                MessageBox.Show("number is empty !!");
                return;
            }
            if (pictureBox1.Image == null)
                MessageBox.Show("picture is empty !!");

            int Index = lv_FaceInfo.SelectedItems[0].Index;
            
            if (oNET_SDK_DEVICEINFO.deviceType == 2)//IPC
            {
                NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO selectIPCFace = m_ipcFaceInfoList[Index + 1];
                IPC_FACE_INFO resFace = GetIpcFaceInfoById((uint)selectIPCFace.iKey);
                resFace.name = faceName;
                resFace.identifyNumber = certificateNum;
                resFace.telephone = mobile;
                //resFace.pictureData = 
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] array = ms.ToArray();
                ms.Close();
                string imgData64 = Convert.ToBase64String(array);
                string szUrl = "EditTargetFace";
                string tempXML = @"<?xml version = '1.0' encoding='utf-8'?>
                                                <config version='1.7' xmlns='http://www.ipc.com/ver10' >
                                                <types>
                                                <listType> 
                                                  <enum>strangerList</enum>  
                                                  <enum>whiteList</enum>  
                                                  <enum>blackList</enum> 
                                                </listType>  
                                                <sexType> 
                                                  <enum>male</enum>  
                                                  <enum>female</enum> 
                                                </sexType>  
                                                <formatType> 
                                                  <enum>jpg</enum>  
                                                </formatType> 
                                                </types>
                                                <personID type='uint32'>{13}</personID> 
                                                <personInfo>
                                                <listType type='listType'>{0}</listType> 
                                                <name type='string' maxLen='127'><![CDATA[{1}]]></name>  
                                                <sex type='sexType'>{2}</sex>
                                                <age type='uint32'>{3}</age>
                                                <identifyNumber type='string' maxLen='127'><![CDATA[{4}]]></identifyNumber>
                                                <telephone type='string' maxLen='63'><![CDATA[{5}]]></telephone>
                                                </personInfo>
                                                <faceImgs type='list' maxCount='5' count='{6}'>
                                                <item>
                                                <pictureData><![CDATA[{7}]]></pictureData>
                                                <pictureNum>{8}</pictureNum>
                                                <Width>{9}</Width>
                                                <Height>{10}</Height>
                                                <Height>{10}</Height>
                                                <format>{11}</format>
                                                <size>{12}</size>
                                                </item>
	                                            </faceImgs>
                                            </config>";
                string sendXML = string.Format(tempXML, resFace.listType, get_uft8(resFace.name), resFace.sex, resFace.age, resFace.identifyNumber, resFace.telephone, 1, imgData64,
                    1, pictureBox1.Image.Width, pictureBox1.Image.Height, "jpg", array.Length, resFace.itemId);
                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, sendXML, szUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret)
                {
                    //btn_Get_Groups.PerformClick();
                    MessageBox.Show("edit ipc face image success");                
                }
                Marshal.FreeHGlobal(lpOutBuffer);
            }
            else if (oNET_SDK_DEVICEINFO.deviceType == 3)//NVR
            {
                NET_SDK_FACE_INFO_LIST_ITEM targetFace = m_faceInfoList[Index + 1];
                int tIndex = lv_FaceGroup.SelectedItems[0].Index;
                NET_SDK_FACE_INFO_GROUP_ITEM targetGroup = m_groupList[tIndex + 1];

                NET_SDK_FACE_INFO_EDIT Face_Set = new NET_SDK_FACE_INFO_EDIT();
                NET_SDK_FACE_INFO_ADD Face_Add = new NET_SDK_FACE_INFO_ADD();

                Face_Add.sFaceInfoItem.name = new byte[64];
                byte[] bufferFaceName = Encoding.UTF8.GetBytes(faceName);
                Array.Copy(bufferFaceName, Face_Add.sFaceInfoItem.name, bufferFaceName.Length);

                Face_Add.sFaceInfoItem.birthday = faceBirth;

                Face_Add.sFaceInfoItem.nativePlace = new byte[64];
                byte[] bufferNative = Encoding.UTF8.GetBytes(nativePlace);
                Array.Copy(bufferNative, Face_Add.sFaceInfoItem.nativePlace, bufferNative.Length);

                Face_Add.sFaceInfoItem.certificateNum = new byte[20];
                byte[] bufferCertify = Encoding.UTF8.GetBytes(certificateNum);
                Array.Copy(bufferCertify, Face_Add.sFaceInfoItem.certificateNum, bufferCertify.Length);

                Face_Add.sFaceInfoItem.mobile = new byte[20];
                byte[] bufferMobile = Encoding.UTF8.GetBytes(mobile);
                Array.Copy(bufferMobile, Face_Add.sFaceInfoItem.mobile, bufferMobile.Length);

                Face_Add.sFaceInfoItem.number = new byte[20];
                byte[] bufferNumber = Encoding.UTF8.GetBytes(number);
                Array.Copy(bufferNumber, Face_Add.sFaceInfoItem.number, bufferNumber.Length);

                Face_Add.sFaceInfoItem.groups = targetFace.groups;
                //Face_Add.sFaceInfoItem.groups = new NET_SDK_FACE_INFO_LIST_ITEM_GROUPS[16];
                Face_Add.sFaceInfoItem.groups[0].guid = targetGroup.guid;
                Face_Add.sFaceInfoItem.groups[0].groupId = targetGroup.groupId;
                Face_Add.sFaceInfoItem.itemId = 1;
                Face_Add.imgNum = 1;
                Face_Add.haveImgData = 1;
                Face_Add.sFaceImgInfo = new NET_SDK_FACE_IMG_INFO_CH[5];

                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] array = ms.ToArray();

                Face_Add.imgData = Marshal.AllocHGlobal(array.Length);
                Marshal.Copy(array, 0, Face_Add.imgData, array.Length);
                Face_Add.imgWidth = 256;    //timg.jpg的宽 //图片必须是jpg的，高宽可以不传
                Face_Add.imgHeight = 256;   //timg.jpg的高
                Face_Add.imgLen = (UInt32)array.Length;

                Face_Set.sFaceInfoItem = Face_Add;
                Face_Set.sFaceInfoItem.sFaceInfoItem.itemId = targetFace.itemId;
                Face_Set.delFaceImgs = new UInt32[5];
                Face_Set.delFaceImgs[0] = 1;

                int nSizeOfFaceSet = Marshal.SizeOf(Face_Set);
                IntPtr intPtrFaceSet = Marshal.AllocHGlobal(nSizeOfFaceSet + array.Length);
                Marshal.StructureToPtr(Face_Set, intPtrFaceSet, true);

                uint lpBytesReturned = 0;
                int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SET_FACE_INFO;

                bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrFaceSet, (UInt32)(nSizeOfFaceSet + array.Length), IntPtr.Zero, 0, ref lpBytesReturned);
                if (ret)
                {
                    MessageBox.Show("edit face information success !");

                }
                else
                    ShowErrorMessage();
                Marshal.FreeHGlobal(intPtrFaceSet);
            }
            
            
        }
        private void btn_Del_Face_Click(object sender, EventArgs e)
        {
            if (lv_FaceInfo.SelectedItems.Count < 1)
            {
                MessageBox.Show("select face first !!");
                return;
            }
            int Index = lv_FaceInfo.SelectedItems[0].Index;
            
            
            if (oNET_SDK_DEVICEINFO.deviceType == 2)//IPC
            {
                NET_SDK_IVE_FACE_MATCH_QUERY_ALBUM_REPLY_INFO selectIPCFace = m_ipcFaceInfoList[Index + 1];
                string szUrl = "DeleteTargetFace";
                string tempXML = @"<?xml version = '1.0' encoding='utf-8'?>
                                                <config version='1.7' xmlns='http://www.ipc.com/ver10'>                                                <types> 
                                                    <deleteType> 
                                                      <enum>byPersonID</enum>  
                                                      <enum>byListType</enum>  
                                                      <enum>byName</enum>  
                                                      <enum>byIdentifyNumber</enum> 
                                                    </deleteType>  
                                                    <listType> 
                                                      <enum>strangerList</enum>  
                                                      <enum>whiteList</enum>  
                                                      <enum>blackList</enum> 
                                                    </listType> 
                                                  </types>  
                                                  <deleteAction> 
                                                    <deleteType type='deleteType'>byPersonID</deleteType>  
                                                    <personID type='uint32'>{0}</personID> 
                                                  </deleteAction> 
                                                </config>";
                string sendXml = string.Format(tempXML, selectIPCFace.iKey);
                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, sendXml, szUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret)
                {
                    btn_Get_Groups.PerformClick();
                    MessageBox.Show("delete success!");
                }
                Marshal.FreeHGlobal(lpOutBuffer);
            }
            if (oNET_SDK_DEVICEINFO.deviceType == 3)//NVR
            {
                NET_SDK_FACE_INFO_LIST_ITEM targetFace = m_faceInfoList[Index + 1];
                NET_SDK_FACE_INFO_DEL Face_Del = new NET_SDK_FACE_INFO_DEL();
                Face_Del.groupsId = new UInt32[16];
                Face_Del.faceInfoListItemId = targetFace.itemId;

                int nSizeOfFaceDel = Marshal.SizeOf(Face_Del);
                IntPtr intPtrFaceDel = Marshal.AllocHGlobal(nSizeOfFaceDel);
                Marshal.StructureToPtr(Face_Del, intPtrFaceDel, true);

                uint lpBytesReturned = 0;
                int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_DEL_FACE_INFO;

                bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrFaceDel, (UInt32)nSizeOfFaceDel, IntPtr.Zero, 0, ref lpBytesReturned);
                if (ret)
                {
                    MessageBox.Show("delete face information success !");
                }
                else
                    ShowErrorMessage();
                Marshal.FreeHGlobal(intPtrFaceDel);
            }

            

        }
        private void btn_Get_ChsnapList_Click(object sender, EventArgs e)
        {           
            if (oNET_SDK_DEVICEINFO.deviceType == 2)//IPC
            {
                string szUrl = "SearchSnapFaceByTime";
                string tempXML = @"<?xml version = '1.0' encoding='utf-8'?>
                                                <config version='1.7' xmlns='http://www.ipc.com/ver10'>                                                <search> 
                                                    <starttime type='string'><![CDATA[{0}]]></starttime>  
                                                    <endtime type='string'><![CDATA[{1}]]></endtime> 
                                                </search> 
                                            </config>";
                string start = dtp_face_start.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string end = dtp_face_end.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string sendXml = string.Format(tempXML, start, end);
                int lpBytesReturned = 0;
                byte[] rec = new byte[1024 * 1024];
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, sendXml, szUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                if (ret)
                {
                    m_searchedIpcFaceList.Clear();
                    lv_ChSnapFaceImage.Items.Clear();

                    Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                    string res = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));

                    System.Xml.XmlNode tempnode = CommonMethods.GetNodePosByIndex(res, "captureFaceList");
                    string num = CommonMethods.GetNodeAttribute(tempnode, "count");
                    if (num.Length > 0)
                    {
                        for (int i = 0; i < Convert.ToInt16(num); i++)
                        {
                            Searched_IPC_FACE_INFO Face_Item = new Searched_IPC_FACE_INFO();
                            Face_Item.faceID = CommonMethods.GetNodeValue(res, "faceID", i);
                            Face_Item.snapTime = CommonMethods.GetNodeValue(res, "snapTime", i);
                            ListViewItem item = new ListViewItem(new string[]{(i + 1).ToString(), Face_Item.faceID, "", Face_Item.snapTime});
                            lv_ChSnapFaceImage.Items.Add(item);
                            m_searchedIpcFaceList.Add(lv_FaceInfo.Items.Count, Face_Item);
                        }
                    }
                    

                    MessageBox.Show("search snap face by time success!");
                }
                Marshal.FreeHGlobal(lpOutBuffer);
            }
            if (oNET_SDK_DEVICEINFO.deviceType == 3)//NVR
            {
                uint lpBytesReturned = 0;
                int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SEARCH_CH_SNAP_FACE_IMG_LIST;
                NET_SDK_CH_SNAP_FACE_IMG_LIST_SEARCH Img_List = new NET_SDK_CH_SNAP_FACE_IMG_LIST_SEARCH();
                NET_SDK_CH_SNAP_FACE_IMG_LIST Img_List_Out = new NET_SDK_CH_SNAP_FACE_IMG_LIST();
                NET_SDK_FACE_IMG_INFO_CH Img_info = new NET_SDK_FACE_IMG_INFO_CH();

                Img_List.dwChannel = Convert.ToUInt16(cb_CH.SelectedItem);
                Img_List.pageIndex = 1;
                Img_List.pageSize = 18;
                Img_List.startTime.year = (ushort)DateTime.Now.Year;
                Img_List.startTime.month = Convert.ToByte(dtp_face_start.Value.Month);
                Img_List.startTime.mday = Convert.ToByte(dtp_face_start.Value.Day);
                Img_List.startTime.hour = 0;
                Img_List.startTime.minute = 0;
                Img_List.startTime.second = 0;

                Img_List.endTime.year = (ushort)DateTime.Now.Year;
                Img_List.endTime.month = Convert.ToByte(dtp_face_end.Value.Month);
                Img_List.endTime.mday = Convert.ToByte(dtp_face_end.Value.Day);
                Img_List.endTime.hour = 23;
                Img_List.endTime.minute = 59;
                Img_List.endTime.second = 59;
                int nSizeOfImgList = Marshal.SizeOf(Img_List);
                IntPtr intPtrImgList = Marshal.AllocHGlobal(nSizeOfImgList);
                Marshal.StructureToPtr(Img_List, intPtrImgList, true);

                int nSizeofImgInfo = Marshal.SizeOf(Img_info);
                int nSizeofImgListOut = Marshal.SizeOf(Img_List_Out);
                IntPtr intPtrImgListOut = Marshal.AllocHGlobal(nSizeofImgListOut + nSizeofImgInfo * 100);
                Marshal.StructureToPtr(Img_List_Out, intPtrImgListOut, true);

                bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrImgList, (UInt32)(nSizeOfImgList), intPtrImgListOut, (UInt32)(nSizeofImgListOut + nSizeofImgInfo * 100), ref lpBytesReturned);
                if (ret)
                {
                    m_chSnapFaceList.Clear();
                    lv_ChSnapFaceImage.Items.Clear();

                    Img_List_Out = (NET_SDK_CH_SNAP_FACE_IMG_LIST)Marshal.PtrToStructure(intPtrImgListOut, typeof(NET_SDK_CH_SNAP_FACE_IMG_LIST));
                    for (int i = 0; i < Img_List_Out.listNum; ++i)
                    {
                        Img_info = (NET_SDK_FACE_IMG_INFO_CH)Marshal.PtrToStructure(intPtrImgListOut + nSizeofImgListOut + i * nSizeofImgInfo, typeof(NET_SDK_FACE_IMG_INFO_CH));

                        ListViewItem item = new ListViewItem(ChangeToItems(Img_info, i + 1));

                        lv_ChSnapFaceImage.Items.Add(item);
                        m_chSnapFaceList.Add(lv_ChSnapFaceImage.Items.Count, Img_info);

                    }
                }
                else
                    ShowErrorMessage();
                Marshal.FreeHGlobal(intPtrImgList);
                Marshal.FreeHGlobal(intPtrImgListOut);

            }
            
        }
        private string[] ChangeToItems(NET_SDK_FACE_IMG_INFO_CH file, int index)
        {
            string strIndex = index.ToString();
            string id = file.targetImgId.ToString();
            string chl = file.chl.ToString();
            string startTime = (file.frameTime.year) + "-" + AddZero(Convert.ToByte(file.frameTime.month)) + "-" + AddZero(file.frameTime.mday) + " " +
                AddZero(file.frameTime.hour) + ":" + AddZero(file.frameTime.minute) + ":" + AddZero(file.frameTime.second);

            string[] tempStrings = {strIndex, id, chl, startTime};
            return tempStrings;
        }
        private void lv_ChSnapFaceImage_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
            ListViewHitTestInfo info = lv_ChSnapFaceImage.HitTest(e.X, e.Y);
            if (info != null)
            {
                var videoitem = info.Item as ListViewItem;
                if (oNET_SDK_DEVICEINFO.deviceType == 2)//IPC
                {
                    Searched_IPC_FACE_INFO sifi = m_searchedIpcFaceList[videoitem.Index + 1];

                    string szUrl = "SearchSnapFaceByKey";
                    string tempXML = @"<?xml version = '1.0' encoding='utf-8'?>
                                                <config version='1.7' xmlns='http://www.ipc.com/ver10'>                                                <search> 
                                                <snapTime type='uint64'>{0}</snapTime>  
                                                <faceID type='uint32'>{1}</faceID>  
                                                <requestPanoramicPic type='boolean'>true</requestPanoramicPic>  
                                                <requestPersonPic type='boolean'>true</requestPersonPic> 
                                              </search> 
                                            </config>";
                    string sendXml = string.Format(tempXML, sifi.snapTime, sifi.faceID);
                    int lpBytesReturned = 0;
                    byte[] rec = new byte[1024 * 1024];
                    IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
                    bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, sendXml, szUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
                    if (ret)
                    {
                        Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                        string res = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));
                      
                        string pictureData = CommonMethods.GetNodeValue(res, "pictureData",0);
                        LoadPicture(Convert.FromBase64String(pictureData), pictureBox2);//display the snapFace
                        if (CommonMethods.GetNodePosByIndex(res, "panoramicInfo") != null)
                        {
                            pictureData = CommonMethods.GetNodeValue(res, "pictureData", 1);
                            LoadPicture(Convert.FromBase64String(pictureData), pictureBox3);//display the panoramicInfo
                        }
                        if (CommonMethods.GetNodePosByIndex(res, "matchInfo") != null)
                        {
                            if (CommonMethods.GetNodePosByIndex(res, "personInfo") != null)
                            {
                                pictureData = CommonMethods.GetNodeValue(res, "pictureData", 2);
                                LoadPicture(Convert.FromBase64String(pictureData), pictureBox4);//display the panoramicInfo
                            }
                            else
                                MessageBox.Show("match failed");
                        }
                        else
                            MessageBox.Show("no match");
                        
                    }
                    Marshal.FreeHGlobal(lpOutBuffer);
                }
                if (oNET_SDK_DEVICEINFO.deviceType == 3)//NVR
                {
                    NET_SDK_FACE_IMG_INFO_CH Img_CH = m_chSnapFaceList[videoitem.Index + 1];

                    NET_SDK_FACE_INFO_IMG_DATA IMGData = new NET_SDK_FACE_INFO_IMG_DATA();
                    //Img_CH.chl = 17;
                    int nSizeofImgInfo = Marshal.SizeOf(typeof(NET_SDK_FACE_IMG_INFO_CH));
                    IntPtr intPtrImgCH = Marshal.AllocHGlobal(nSizeofImgInfo);
                    Marshal.StructureToPtr(Img_CH, intPtrImgCH, true);

                    int nSizeOfIMGData = Marshal.SizeOf(IMGData);
                    IntPtr intPtIMGData = Marshal.AllocHGlobal(nSizeOfIMGData + 1024 * 100);
                    Marshal.StructureToPtr(IMGData, intPtIMGData, true);

                    uint lpBytesReturned = 0;
                    int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SEARCH_CH_SNAP_FACE_IMG;
                    bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrImgCH, (UInt32)nSizeofImgInfo, intPtIMGData, (UInt32)(nSizeOfIMGData + 1024 * 100), ref lpBytesReturned);
                    if (ret)
                    {
                        if (pictureBox2.Image != null)
                            pictureBox2.Image.Dispose();
                        pictureBox2.Image = null;

                        IMGData = (NET_SDK_FACE_INFO_IMG_DATA)Marshal.PtrToStructure(intPtIMGData, typeof(NET_SDK_FACE_INFO_IMG_DATA));
                        byte[] imagdata = new byte[IMGData.imgLen];
                        Marshal.Copy(IMGData.imgData, imagdata, 0, (Int32)IMGData.imgLen);


                        MemoryStream stream = new MemoryStream(imagdata);
                        pictureBox2.Image = Image.FromStream(stream);
                    }
                    Marshal.FreeHGlobal(intPtrImgCH);
                    Marshal.FreeHGlobal(intPtIMGData);
                }
                
            }
        }
        private void btn_Search_Imgbyimg_Click(object sender, EventArgs e)
        {
            if (lv_ChSnapFaceImage.SelectedItems.Count < 1)
            {
                MessageBox.Show("no snap face image selected! ");
                return;
            }                
            int Index = lv_ChSnapFaceImage.SelectedItems[0].Index;
            NET_SDK_FACE_IMG_INFO_CH targetFaceImg = m_chSnapFaceList[Index + 1];

            uint lpBytesReturned = 0;
            int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SEARCH_IMAGE_BY_IMG;
            NET_SDK_SEARCH_IMAGE_BY_IMAGE ImgByImg = new NET_SDK_SEARCH_IMAGE_BY_IMAGE();
            ImgByImg.pageIndex = 1;
            ImgByImg.pageSize = 10;
            ImgByImg.similarity = 75;
            ImgByImg.resultCountLimit = 100;
            ImgByImg.startTime.year = (ushort)DateTime.Now.Year;
            ImgByImg.startTime.month = Convert.ToByte(dtp_face_start.Value.Month);
            ImgByImg.startTime.mday = Convert.ToByte(dtp_face_start.Value.Day);
            ImgByImg.startTime.hour = 0;
            ImgByImg.startTime.minute = 0;
            ImgByImg.startTime.second = 0;

            ImgByImg.endTime.year = (ushort)DateTime.Now.Year;
            ImgByImg.endTime.month = Convert.ToByte(dtp_face_end.Value.Month);
            ImgByImg.endTime.mday = Convert.ToByte(dtp_face_end.Value.Day);
            ImgByImg.endTime.hour = 23;
            ImgByImg.endTime.minute = 59;
            ImgByImg.endTime.second = 59;
            ImgByImg.searchType = (UInt32)NET_SDK_SEARCH_IMAGE_BY_IMAGE_TYPE.SEARCH_IMAGE_BY_FACE_IMAGES;

            ////ImgByImg.sfaceFeatures.itemId = 89;
            //ImgByImg.sfaceImgs.chl = targetFaceImg.chl;
            //ImgByImg.sfaceImgs.imgId = targetFaceImg.imgId;
            //ImgByImg.sfaceImgs.frameTime = targetFaceImg.frameTime;

            ImgByImg.srecognizedFilter.isContainRecognized = 1;
            ImgByImg.srecognizedFilter.isContainNotRecognized = 0;
            ImgByImg.srecognizedFilter.groupsId = 1;//目标库id

            int nSizeOfImgByImg = Marshal.SizeOf(ImgByImg);
            IntPtr intPtrImgByImg = Marshal.AllocHGlobal(nSizeOfImgByImg);
            Marshal.StructureToPtr(ImgByImg, intPtrImgByImg, true);

            NET_SDK_SEARCH_IMAGE_BY_IMAGE_LIST Img_by_Img_List_Out = new NET_SDK_SEARCH_IMAGE_BY_IMAGE_LIST();
            NET_SDK_SEARCH_IMAGE_ITEM Img_Item = new NET_SDK_SEARCH_IMAGE_ITEM();
            int nSizeofImg_by_Img_List_Out = Marshal.SizeOf(Img_by_Img_List_Out);
            int nSizeofImg_Item = Marshal.SizeOf(Img_Item);
            IntPtr intPtrImg_by_Img_List_Out = Marshal.AllocHGlobal(nSizeofImg_by_Img_List_Out + nSizeofImg_Item * 10);
            Marshal.StructureToPtr(Img_by_Img_List_Out, intPtrImg_by_Img_List_Out, true);

            bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrImgByImg, (UInt32)nSizeOfImgByImg, intPtrImg_by_Img_List_Out, (UInt32)(nSizeofImg_by_Img_List_Out + nSizeofImg_Item * 10), ref lpBytesReturned);
            if (ret)
            {
                lv_SearchImgRes.Items.Clear();
                m_searchImgList.Clear();

                Img_by_Img_List_Out = (NET_SDK_SEARCH_IMAGE_BY_IMAGE_LIST)Marshal.PtrToStructure(intPtrImg_by_Img_List_Out, typeof(NET_SDK_SEARCH_IMAGE_BY_IMAGE_LIST));
                for (int i = 0; i < Img_by_Img_List_Out.listNum; ++i)
                {
                    Img_Item = (NET_SDK_SEARCH_IMAGE_ITEM)Marshal.PtrToStructure(intPtrImg_by_Img_List_Out + nSizeofImg_by_Img_List_Out + i * nSizeofImg_Item, typeof(NET_SDK_SEARCH_IMAGE_ITEM));
                    ListViewItem item = new ListViewItem(ChangeToItems(Img_Item, i + 1));

                    lv_SearchImgRes.Items.Add(item);
                    m_searchImgList.Add(lv_SearchImgRes.Items.Count, Img_Item);

                }
                Marshal.FreeHGlobal(intPtrImgByImg);
                Marshal.FreeHGlobal(intPtrImg_by_Img_List_Out);
            }
        }
        private string[] ChangeToItems(NET_SDK_SEARCH_IMAGE_ITEM file, int index)
        {
            string strIndex = index.ToString();
            string startTime = (file.recStartTime.year) + "-" + AddZero(Convert.ToByte(file.recStartTime.month)) + "-" + AddZero(file.recStartTime.mday) + " " +
                AddZero(file.recStartTime.hour) + ":" + AddZero(file.recStartTime.minute) + ":" + AddZero(file.recStartTime.second);
            string similarity = file.similarity.ToString();
            string faceFeatureId = file.faceFeatureId.ToString();

            string[] tempStrings = { strIndex, startTime, similarity, faceFeatureId };
            return tempStrings;
        }
        private void btn_GetAlarmTrigger_Click(object sender, EventArgs e)
        {

            uint lpBytesReturned = 0;
            int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_GET_FACE_MATCH_ALARM;
            NET_SDK_FACE_MATCH_ALARM Alarm_Get = new NET_SDK_FACE_MATCH_ALARM();
            NET_SDK_FACE_MATCH_ALARM_TRIGGER Alarm_Trigger = new NET_SDK_FACE_MATCH_ALARM_TRIGGER();
            int nSizeOfAlarmTrigger = Marshal.SizeOf(Alarm_Trigger);

            int nSizeOfAlarmGet = Marshal.SizeOf(Alarm_Get);
            IntPtr intPtrAlarmGet = Marshal.AllocHGlobal(nSizeOfAlarmGet + nSizeOfAlarmTrigger * 16);
            Marshal.StructureToPtr(Alarm_Get, intPtrAlarmGet, true);

            bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, IntPtr.Zero, 0, intPtrAlarmGet, (UInt32)(nSizeOfAlarmGet + nSizeOfAlarmTrigger * 16), ref lpBytesReturned);
            if (ret)
            {
                lv_FaceMatchAlarm.Items.Clear();
                m_faceMatchAlarmList.Clear();

                Alarm_Get = (NET_SDK_FACE_MATCH_ALARM)Marshal.PtrToStructure(intPtrAlarmGet, typeof(NET_SDK_FACE_MATCH_ALARM));
                similarity = Alarm_Get.similarity;
                faceFeatureGroupsNum = Alarm_Get.faceFeatureGroupsNum;
                enableCH = Alarm_Get.enableCH;
                for (int i = 0; i < Alarm_Get.faceFeatureGroupsNum; ++i)
                {
                    Alarm_Trigger = (NET_SDK_FACE_MATCH_ALARM_TRIGGER)Marshal.PtrToStructure(Alarm_Get.sFaceMatchAlarmTrigger + i * nSizeOfAlarmTrigger, typeof(NET_SDK_FACE_MATCH_ALARM_TRIGGER));
                    ListViewItem item = new ListViewItem(ChangeToItems(Alarm_Trigger, i + 1));
                    lv_FaceMatchAlarm.Items.Add(item);
                    m_faceMatchAlarmList.Add(lv_FaceMatchAlarm.Items.Count, Alarm_Trigger);
                }                  
            }
            Marshal.FreeHGlobal(intPtrAlarmGet);
        }
        private string[] ChangeToItems(NET_SDK_FACE_MATCH_ALARM_TRIGGER file, int index)
        {
            string strIndex = index.ToString();
            string id = file.groupId.ToString();
            string groupSwitch = file.groupSwitch.ToString();
            string recSwitch = file.recSwitch.ToString();
            string[] tempStrings = { strIndex, id, groupSwitch, recSwitch };
            return tempStrings;
        }

        private void lv_FaceMatchAlarm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
             ListViewHitTestInfo info = lv_FaceMatchAlarm.HitTest(e.X, e.Y);
             if (info != null)
             {
                 var videoitem = info.Item as ListViewItem;
                 string[] args = { videoitem.SubItems[0].Text, videoitem.SubItems[1].Text, videoitem.SubItems[2].Text, videoitem.SubItems[3].Text };
                 EditAlarmTirgger eat = new EditAlarmTirgger(args);
                 eat.ShowDialog();
                 if (eat.DialogResult == DialogResult.OK)
                 {
                     args = eat.returnStrs;
                     videoitem.SubItems[0].Text = args[0];
                     videoitem.SubItems[1].Text = args[1];
                     videoitem.SubItems[2].Text = args[2];
                     videoitem.SubItems[3].Text = args[3];
                 }
             }
        }
        private void btn_SetAlarmTrigger_Click(object sender, EventArgs e)
        {
            int count = lv_FaceMatchAlarm.Items.Count;
            if (count < 1)
                return;

            NET_SDK_FACE_MATCH_ALARM_TRIGGER[] alarmTriggers = new NET_SDK_FACE_MATCH_ALARM_TRIGGER[count];
            for (int i = 0; i < lv_FaceMatchAlarm.Items.Count; i++ )
            {
                ListViewItem tempItem = lv_FaceMatchAlarm.Items[i];
                alarmTriggers[i] = m_faceMatchAlarmList[i + 1];
                alarmTriggers[i].groupSwitch = Convert.ToByte(tempItem.SubItems[2].Text);
                alarmTriggers[i].recSwitch = Convert.ToByte(tempItem.SubItems[3].Text);
            }

            int nSizeOfAlarmTrigger = Marshal.SizeOf(typeof(NET_SDK_FACE_MATCH_ALARM_TRIGGER));
            uint lpBytesReturned = 0;
            int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_SET_FACE_MATCH_ALARM;
            NET_SDK_FACE_MATCH_ALARM Alarm_Set = new NET_SDK_FACE_MATCH_ALARM();

            Alarm_Set.sFaceMatchAlarmTrigger = PointArrayToIntPtr(alarmTriggers);
            Alarm_Set.faceFeatureGroupsNum = faceFeatureGroupsNum;
            Alarm_Set.similarity = similarity;
            Alarm_Set.enableCH = enableCH;
         
            int nSizeOfAlarmSet = Marshal.SizeOf(Alarm_Set) + nSizeOfAlarmTrigger * (int)Alarm_Set.faceFeatureGroupsNum;
            IntPtr intPtrAlarmSet = Marshal.AllocHGlobal(nSizeOfAlarmSet);
            Marshal.StructureToPtr(Alarm_Set, intPtrAlarmSet, true);
            bool ret = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, intPtrAlarmSet, (UInt32)nSizeOfAlarmSet, IntPtr.Zero, 0, ref lpBytesReturned);
            if (ret)
                MessageBox.Show("set alarm success !");
            else
                MessageBox.Show("set alarm fail !");
            Marshal.FreeHGlobal(intPtrAlarmSet);
        }

        private IntPtr PointArrayToIntPtr(NET_SDK_FACE_MATCH_ALARM_TRIGGER[] points)
        {
            if (points == null || points.Length == 0)
                return IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(NET_SDK_FACE_MATCH_ALARM_TRIGGER));
            IntPtr lpoints = Marshal.AllocHGlobal(size * points.Length);
            IntPtr tmp = lpoints;
            for (int i = 0; i < points.Length; i++, tmp += size)
            {
                Marshal.StructureToPtr(points[i], tmp, false);
            }
            return lpoints;
        }
        
        private void getFacesByAPI(string tempXML, string szUrl, long pageIndex, string startTime, string endTime, string chls, string groups)
        {
            string sendXml = string.Format(tempXML, pageIndex.ToString(), startTime, endTime, chls, groups);
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);

            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, sendXml, szUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            if (ret)
            {
                if (pageIndex == 1)
                {
                    lv_Faces.Items.Clear();
                    //Faces.Clear();
                }
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string res = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));

                int faceNum = CommonMethods.GetNodeNum(res, "item");
                for (int i = 0; i < faceNum; i++)
                {
                    FaceInfo fi = new FaceInfo();
                    fi.time = CommonMethods.GetNodeValue(res, "frameTime", i);
                    fi.idNum = CommonMethods.GetNodeValue(res, "certificateNum", i);
                    ListViewItem item = new ListViewItem(new string[] { (i + 1 + lv_Faces.Items.Count).ToString(), fi.time, fi.idNum });
                    lv_Faces.Items.Add(item);
                    //Faces.Add(fi);
                }
                if (progressBar1.Value < 99)
                    progressBar1.Value++;
                Marshal.FreeHGlobal(lpOutBuffer);
                string pageEnd = CommonMethods.GetNodeAttribute(CommonMethods.GetNodePosByIndex(res, "content", 0), "end");
                if (pageEnd == "false")
                    getFacesByAPI(tempXML, szUrl, pageIndex + 1, startTime, endTime, chls, groups);
                else
                {
                    progressBar1.Value = 100;
                    MessageBox.Show("搜索完毕");
                }

            }
            else
                Marshal.FreeHGlobal(lpOutBuffer);
            
        }
        private void progresStatus(object obj)
        {
            
                progressBar1.Value++;
        }
        private List<string> GetChlIds()
        {
            string szUrl = "queryDevList";
            string emptyXml = @"<?xml version = '1.0' encoding='utf-8'?>
                                    <request version='1.0' systemType='NVMS-9000' clientType='WEB'>
                                    <requireField><chlType/></requireField></request>";
            int lpBytesReturned = 0;
            byte[] rec = new byte[1024 * 1024];
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(1024 * 1024);
            bool ret = DevSdkHelper.NET_SDK_TransparentConfig(userId, emptyXml, szUrl, lpOutBuffer, 1024 * 1024, ref lpBytesReturned);
            List<string> chlIds = new List<string>();
            if (ret)
            {
                Marshal.Copy(lpOutBuffer, rec, 0, 1024 * 1024);
                string res = DevSdkHelper.RemoveEmptyChar(Encoding.UTF8.GetString(rec));

                int totalChlNum = CommonMethods.GetNodeNum(res, "item");
                
                for (int i = 0; i < totalChlNum; i++)
                    if (CommonMethods.GetNodeValue(res, "chlType", i) == "digital")
                        chlIds.Add(CommonMethods.GetNodeAttribute(CommonMethods.GetNodePosByIndex(res, "item", i), "id"));
                

            }
            Marshal.FreeHGlobal(lpOutBuffer);
            return chlIds;                              

        }
        private List<string> GetGroupIds()
        {
            List<string> groupIds = new List<string>();
            uint lpBytesReturned = 0;
            int dwCommand = (int)NET_SDK_FACE_MATCH_OPERATE_TYPE.NET_SDK_GET_FACE_INFO_GROUP_LIST;                
            NET_SDK_FACE_INFO_GROUP_ITEM[] g_Group_Item = new NET_SDK_FACE_INFO_GROUP_ITEM[groupMaxNum];
            int g_GroupNum = 0;

            NET_SDK_FACE_INFO_GROUP_ITEM Group_Item = new NET_SDK_FACE_INFO_GROUP_ITEM();
            int nSizeOfGroupItem = Marshal.SizeOf(Group_Item);
            IntPtr intPtrGroupItem = Marshal.AllocHGlobal(nSizeOfGroupItem * 16);
            bool bResult = DevSdkHelper.NET_SDK_FaceMatchOperate(userId, dwCommand, IntPtr.Zero, 0, intPtrGroupItem, (UInt32)nSizeOfGroupItem * 16, ref lpBytesReturned);
            if (bResult)
            {
                for (int i = 0; i < lpBytesReturned / nSizeOfGroupItem; ++i, ++g_GroupNum)
                {
                    g_Group_Item[i] = (NET_SDK_FACE_INFO_GROUP_ITEM)Marshal.PtrToStructure(intPtrGroupItem + nSizeOfGroupItem * i, typeof(NET_SDK_FACE_INFO_GROUP_ITEM));
                    groupIds.Add(g_Group_Item[i].groupId.ToString());                    
                }
            }
            Marshal.FreeHGlobal(intPtrGroupItem);
            return groupIds;
        }
        private int getJpgSize(string FileName, out Size JpgSize, out float Wpx, out float Hpx)
        {//C#快速获取JPG图片大小及英寸分辨率
            JpgSize = new Size(0, 0);
            Wpx = 0; Hpx = 0;
            int rx = 0;
            if (!File.Exists(FileName)) return rx;
            FileStream F_Stream = File.OpenRead(FileName);
            int ff = F_Stream.ReadByte();
            int type = F_Stream.ReadByte();
            if (ff != 0xff || type != 0xd8)
            {//非JPG文件
                F_Stream.Close();
                return rx;
            }
            long ps = 0;
            do
            {
                do
                {
                    ff = F_Stream.ReadByte();
                    if (ff < 0) //文件结束
                    {
                        F_Stream.Close();
                        return rx;
                    }
                } while (ff != 0xff);

                do
                {
                    type = F_Stream.ReadByte();
                } while (type == 0xff);

                //MessageBox.Show(ff.ToString() + "," + type.ToString(), F_Stream.Position.ToString());
                ps = F_Stream.Position;
                switch (type)
                {
                    case 0x00:
                    case 0x01:
                    case 0xD0:
                    case 0xD1:
                    case 0xD2:
                    case 0xD3:
                    case 0xD4:
                    case 0xD5:
                    case 0xD6:
                    case 0xD7:
                        break;
                    case 0xc0: //SOF0段
                        ps = F_Stream.ReadByte() * 256;
                        ps = F_Stream.Position + ps + F_Stream.ReadByte() - 2; //加段长度

                        F_Stream.ReadByte(); //丢弃精度数据
                        //高度
                        JpgSize.Height = F_Stream.ReadByte() * 256;
                        JpgSize.Height = JpgSize.Height + F_Stream.ReadByte();
                        //宽度
                        JpgSize.Width = F_Stream.ReadByte() * 256;
                        JpgSize.Width = JpgSize.Width + F_Stream.ReadByte();
                        //后面信息忽略
                        if (rx != 1 && rx < 3) rx = rx + 1;
                        break;
                    case 0xe0: //APP0段
                        ps = F_Stream.ReadByte() * 256;
                        ps = F_Stream.Position + ps + F_Stream.ReadByte() - 2; //加段长度

                        F_Stream.Seek(5, SeekOrigin.Current); //丢弃APP0标记(5bytes)
                        F_Stream.Seek(2, SeekOrigin.Current); //丢弃主版本号(1bytes)及次版本号(1bytes)
                        int units = F_Stream.ReadByte(); //X和Y的密度单位,units=0：无单位,units=1：点数/英寸,units=2：点数/厘米

                        //水平方向(像素/英寸)分辨率
                        Wpx = F_Stream.ReadByte() * 256;
                        Wpx = Wpx + F_Stream.ReadByte();
                        if (units == 2) Wpx = (float)(Wpx * 2.54); //厘米变为英寸
                        //垂直方向(像素/英寸)分辨率
                        Hpx = F_Stream.ReadByte() * 256;
                        Hpx = Hpx + F_Stream.ReadByte();
                        if (units == 2) Hpx = (float)(Hpx * 2.54); //厘米变为英寸
                        //后面信息忽略
                        if (rx != 2 && rx < 3) rx = rx + 2;
                        break;

                    default: //别的段都跳过////////////////
                        ps = F_Stream.ReadByte() * 256;
                        ps = F_Stream.Position + ps + F_Stream.ReadByte() - 2; //加段长度
                        break;
                }
                if (ps + 1 >= F_Stream.Length) //文件结束
                {
                    F_Stream.Close();
                    return rx;
                }
                F_Stream.Position = ps; //移动指针
            } while (type != 0xda); // 扫描行开始
            F_Stream.Close();
            return rx;
        }
        public static string get_uft8(string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            byte[] gb;
            gb = utf8.GetBytes(text);
            gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
            //返回转换后的字符   
            return utf8.GetString(gb);
        }
    }
    public struct FaceInfo
    {
        public string time;
        public string idNum;
    }

}
