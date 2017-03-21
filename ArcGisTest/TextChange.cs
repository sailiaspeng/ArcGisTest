using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

namespace ArcGisTest
{
    public partial class TextChange : Form
    {
        IPageLayoutControlDefault pPageLayout = null;
        ITextElement pTextElement = null;
        ITextSymbol pTextSymbol = null;
        ICharacterOrientation pCharacterOrientation = null;
        IGraphicsContainer pGraphicsContainer = null;
        public TextChange(IPageLayoutControlDefault pageLayout, ITextElement textElement)
        {
            InitializeComponent();
            pPageLayout = pageLayout;
            pTextElement = textElement;
            pTextSymbol = pTextElement.Symbol;
            pGraphicsContainer = pageLayout.ActiveView.GraphicsContainer;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                Font selectFont = fontDialog.Font;
                textBox2.Text = selectFont.Name + "  " + selectFont.Size.ToString();
                stdole.IFontDisp pFont = ESRI.ArcGIS.ADF.COMSupport.OLE.GetIFontDispFromFont(selectFont) as stdole.IFontDisp;
                pTextSymbol.Font = pFont;
            }
        }

        private void TEXT_Load(object sender, EventArgs e)
        {
            textBox1.Text =pTextElement.Text;
            textBox2.Text = pTextSymbol.Font.Name + "  " + pTextSymbol.Font.Size.ToString();
            numericUpDown1.Value = (decimal)(pTextSymbol.Angle);
            checkB.Checked = pTextSymbol.Font.Bold;
            checkI.Checked = pTextSymbol.Font.Underline;
            checkU.Checked = pTextSymbol.Font.Italic;
            pCharacterOrientation = pTextSymbol as ICharacterOrientation;
            checkO.Checked = pCharacterOrientation.CJKCharactersRotation;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pCharacterOrientation = pTextSymbol as ICharacterOrientation;
            pCharacterOrientation.CJKCharactersRotation = checkO.Checked;
            stdole.IFontDisp pFont = pTextSymbol.Font;
            pFont.Bold = checkB.Checked;
            pFont.Underline = checkI.Checked;
            pFont.Italic = checkU.Checked;
            pTextSymbol.Font = pFont;
            pTextElement.Text = textBox1.Text;
            pTextElement.Symbol = pTextSymbol;
            pGraphicsContainer.UpdateElement(pTextElement as IElement);
            pPageLayout.ActiveView.Refresh();
            //pPageLayout.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            this.Close();
            this.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
