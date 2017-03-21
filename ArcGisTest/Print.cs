using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry; 
namespace ArcGisTest
{
    public partial class Print : Form
    {

        public Print(IHookHelper hookHelper)
        {
            InitializeComponent();
            axPageLayoutControl1.PageLayout = hookHelper.PageLayout;  
       
    
        }
       /* IHookHelper layout_hookHelper = new HookHelperClass(); 
        layout_hookHelper.Hook=this.axPageLayoutControl1.Object;
        PrintPageLayoutForm PPLFrm = new PrintPageLayoutForm(layout_hookHelper);  
        PPLFrm.ShowDialog(); */
        private void axPageLayoutControl1_OnMouseDown(object sender, AxESRI.ArcGIS.Controls.IPageLayoutControlEvents_OnMouseDownEvent e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
        private void PrintAuto(IActiveView pActiveView)
        {
            IPaper pPaper = new Paper();
            IPrinter pPrinter = new EmfPrinterClass();
            System.Drawing.Printing.PrintDocument sysPrintDocumentDocument = new System.Drawing.Printing.PrintDocument();
            pPaper.PrinterName = sysPrintDocumentDocument.PrinterSettings.PrinterName;
            pPrinter.Paper = pPaper;
            int Resolution = pPrinter.Resolution;
            double w, h;
            IEnvelope PEnvelope = pActiveView.Extent;
            w = PEnvelope.Width;
            h = PEnvelope.Height;
            double pw, ph;
            //纸张 
            pPrinter.QueryPaperSize(out pw, out ph);
            tagRECT userRECT = pActiveView.ExportFrame;
            userRECT.left = (int)(pPrinter.PrintableBounds.XMin * Resolution);
            userRECT.top = (int)(pPrinter.PrintableBounds.YMin * Resolution);
            if ((w / h) > (pw / ph))//以宽度来调整高度     
            {
                userRECT.right = userRECT.left + (int)(pPrinter.PrintableBounds.Width * Resolution);
                userRECT.bottom = userRECT.top + (int)((pPrinter.PrintableBounds.Width * Resolution) * h / w);
            }
            else
            {
                userRECT.bottom = userRECT.top + (int)(pPrinter.PrintableBounds.Height * Resolution);
                userRECT.right = userRECT.left + (int)(pPrinter.PrintableBounds.Height * Resolution * w / h);
            }
            IEnvelope pDriverBounds = new EnvelopeClass();
            pDriverBounds.PutCoords(userRECT.left, userRECT.top, userRECT.right, userRECT.bottom);
            ITrackCancel pCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();
            int hdc = pPrinter.StartPrinting(pDriverBounds, 0);
            pActiveView.Output(hdc, pPrinter.Resolution, ref userRECT, pActiveView.Extent, pCancel);
            pPrinter.FinishPrinting();
        }
        private void print_Load(object sender, EventArgs e)
        {
          
            //Display printer details  
            //参数定义  
            IHookHelper layout_hookHelper = new HookHelperClass();
            //参数赋值 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                PrintAuto(this.axPageLayoutControl1.ActiveView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
