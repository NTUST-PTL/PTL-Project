﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SolidWorks.Interop.sldworks;

namespace PTL.SolidWorks
{
    public class SolidWorksAppAdapter
    {
        public String SolidWorksPath { get { return FindSolidWorksPath(); } }
        public ISldWorks SWApp { get; protected set; }
        public IModelDoc2 ActiveDoc
        {
            get
            {
                return SWApp.ActiveDoc;
            }
        }

        public SolidWorksAppAdapter()
        {
            this.SWApp = CreatSolidWorksApplication();
        }

        protected virtual ISldWorks CreatSolidWorksApplication()
        {
            ISldWorks swApp = null;
            String SWPath = FindSolidWorksPath();
            if (SWPath != null)
            {
                //截入DLL檔
                System.Reflection.Assembly sldworks_dll = System.Reflection.Assembly.LoadFrom(SWPath + "SolidWorks.Interop.sldworks.dll");

                //開啟SolidWorks
                swApp = (ISldWorks)sldworks_dll.CreateInstance("SolidWorks.Interop.sldworks.SldWorksClass");
                swApp.Visible = true;
            }
            else
            {
                System.Windows.MessageBox.Show("找不到已安裝的SolidWorks");
            }
            return swApp;
        }
        protected virtual String FindSolidWorksPath()
        {
            String Path = null;

            //從註冊機尋找安裝路徑
            Microsoft.Win32.RegistryKey LocalMachineRegistryKey32 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey LocalMachineRegistryKey64 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);

            //從2012年檢查到2020年，尋找已安裝的SolidWorks的安裝目錄
            for (int i = 2012; i < 2020; i++)
            {
                Microsoft.Win32.RegistryKey SolidWorksSetupRegistryKey;

                //檢查是否安裝64位元版本的SolidWorks
                SolidWorksSetupRegistryKey = LocalMachineRegistryKey64.OpenSubKey(@"SOFTWARE\SolidWorks\SolidWorks " + i.ToString() + @"\Setup");
                if (SolidWorksSetupRegistryKey != null)
                {
                    Path = (String)SolidWorksSetupRegistryKey.GetValue("SolidWorks Folder");
                    if (Path != null)
                        break;
                }

                //檢查是否安裝32位元版本的SolidWorks
                SolidWorksSetupRegistryKey = LocalMachineRegistryKey32.OpenSubKey(@"SOFTWARE\SolidWorks\SolidWorks " + i.ToString() + @"\Setup");
                if (SolidWorksSetupRegistryKey != null)
                {
                    Path = (String)SolidWorksSetupRegistryKey.GetValue("SolidWorks Folder");
                    if (Path != null)
                        break;
                }
            }

            return Path;
        }

        public virtual IModelDoc2 CreatePart()
        {
            IModelDoc2 newModDoc2 = null;
            if (this.SWApp != null)
            {
                string modDocTemplate = this.SWApp.GetUserPreferenceStringValue(8);
                newModDoc2 = this.SWApp.NewDocument(modDocTemplate, 0, 0.0, 0.0);
            }
            return newModDoc2;
        }

        #region 轉接 ISldWorks 原有成員
        public virtual String GetUserPreferenceStringValue(int UserPreference)
        {
            return SWApp.GetUserPreferenceStringValue(UserPreference);
        }
        public virtual dynamic NewDocument(String TemplateName, int PaperSize, double Width, double Height)
        {
            return SWApp.NewDocument(TemplateName, PaperSize, Width, Height);
        }
        public virtual void SetUserPreferenceToggle(int UserPreferenceValue, bool OnFlag)
        {
            SWApp.SetUserPreferenceToggle(UserPreferenceValue, OnFlag);
        }
        #endregion 轉接 ISldWorks 原有成員
    }
}
