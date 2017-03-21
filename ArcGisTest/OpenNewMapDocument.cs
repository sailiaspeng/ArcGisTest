using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
namespace ArcGisTest
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    [Guid("d9a5af06-e313-429d-a863-e31f1dc4a40f")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ArcGisTest.OpenNewMapDocument")]
    public sealed class OpenNewMapDocument : BaseCommand
    {   
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion
        private IHookHelper m_hookHelper;
        private string m_sDocumentPath = string.Empty;  
        private ControlsSynchronizer m_controlsSynchronizer = null;
        public OpenNewMapDocument(ControlsSynchronizer controlsSynchronizer)
        {  
           // TODO: Define values for the public properties
            //设定相关属性值
            base.m_category = "Generic"; //localizable text
            base.m_caption = "Open";  //localizable text
            base.m_message = "This should work in ArcMap/MapControl/PageLayoutControl";  //     localizable text
            base.m_toolTip = "Open";  //localizable text
            base.m_name = "Generic_Open";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")
            //初始化m_controlsSynchronizer
            m_controlsSynchronizer = controlsSynchronizer;
            try
            {
                // TODO: change bitmap name if necessary
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

        }  
        
        public override void OnClick()
        {
           // TODO: Add OpenNewMapDocument.OnClick implementation
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Map Documents (*.mxd)|*.mxd|SHP(*.SHP)|*.shp";
            dlg.Multiselect = false;
            dlg.Title = "Open Map Document";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string docName = dlg.FileName;
                IMapDocument mapDoc = new MapDocumentClass();
                if (mapDoc.get_IsPresent(docName) && !mapDoc.get_IsPasswordProtected(docName))
                {
                    mapDoc.Open(docName, string.Empty);
                    IMap map = mapDoc.get_Map(0);
                    m_controlsSynchronizer.ReplaceMap(map);
                    mapDoc.Close();
                }
            }
        }
        public string DocumentFileName
        {
            get { return m_sDocumentPath; }
        }  
    }
}