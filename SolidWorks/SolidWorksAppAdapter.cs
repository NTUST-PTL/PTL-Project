using System;
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
        public ISldWorks SWApp { get; protected set; }
        public String SWPath { get; protected set; }
        public String SWVersion { get; protected set; }

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
            IEnumerable<KeyValuePair<String, String>> SWVersionAndPaths = FindSolidWorksPath();
            foreach (var VersionAndPaths in SWVersionAndPaths)
            {
                //截入DLL檔
                if (System.IO.File.Exists(VersionAndPaths.Value + "SolidWorks.Interop.sldworks.dll"))
                {
                    System.Reflection.Assembly sldworks_dll = System.Reflection.Assembly.LoadFrom(VersionAndPaths.Value + "SolidWorks.Interop.sldworks.dll");

                    //開啟SolidWorks
                    swApp = (ISldWorks)sldworks_dll.CreateInstance("SolidWorks.Interop.sldworks.SldWorksClass");
                    swApp.Visible = true;

                    SWVersion = VersionAndPaths.Key;
                    SWPath = VersionAndPaths.Value;

                    return swApp;
                }
            }

            throw new TypeAccessException("找不到已安裝的SolidWorks!");
        }
        protected virtual IEnumerable<KeyValuePair<String, String>> FindSolidWorksPath()
        {
            //從註冊機尋找安裝路徑
            Microsoft.Win32.RegistryKey LocalMachineRegistryKey32 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey LocalMachineRegistryKey64 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);

            //從2012年檢查到2020年，尋找已安裝的SolidWorks的安裝目錄
            for (int i = 2012; i < 2050; i++)
            {
                Microsoft.Win32.RegistryKey SolidWorksSetupRegistryKey;

                //檢查是否安裝64位元版本的SolidWorks
                SolidWorksSetupRegistryKey = LocalMachineRegistryKey64.OpenSubKey(@"SOFTWARE\SolidWorks\SolidWorks " + i.ToString() + @"\Setup");
                if (SolidWorksSetupRegistryKey != null)
                {
                    String Path = (String)SolidWorksSetupRegistryKey.GetValue("SolidWorks Folder");
                    if (Path != null)
                        yield return new KeyValuePair<string, string>("SolidWorks " + i.ToString(), Path);
                }

                //檢查是否安裝32位元版本的SolidWorks
                SolidWorksSetupRegistryKey = LocalMachineRegistryKey32.OpenSubKey(@"SOFTWARE\SolidWorks\SolidWorks " + i.ToString() + @"\Setup");
                if (SolidWorksSetupRegistryKey != null)
                {
                    String Path = (String)SolidWorksSetupRegistryKey.GetValue("SolidWorks Folder");
                    if (Path != null)
                        yield return new KeyValuePair<string, string>("SolidWorks " + i.ToString(), Path);
                }
            }
        }

        public virtual IModelDoc2 CreatePart()
        {
            IModelDoc2 newModDoc2 = null;
            if (this.SWApp != null)
            {
                string modDocTemplate = System.IO.File.Exists("C:\\ProgramData\\SolidWorks\\" + SWVersion + "\\templates\\零件.prtdot")
                    ? "C:\\ProgramData\\SolidWorks\\" + SWVersion + "\\templates\\零件.prtdot"
                    : this.SWApp.GetUserPreferenceStringValue(8);
                if (!System.IO.File.Exists(modDocTemplate))
                {
                    throw new TypeAccessException("找不到SolidWorks零件樣板檔案!");
                }
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
        public virtual dynamic OpenDoc6(String docPath, int type, int option, string config, ref int error, ref int wornings)
        {
            return SWApp.OpenDoc6(docPath, type, option, config, ref error, ref wornings);
        }
        public virtual void SetUserPreferenceToggle(int UserPreferenceValue, bool OnFlag)
        {
            SWApp.SetUserPreferenceToggle(UserPreferenceValue, OnFlag);
        }
        #endregion 轉接 ISldWorks 原有成員
    }
}
