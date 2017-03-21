using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.ADF.COMSupport;
using stdole;
using ESRI.ArcGIS.Framework;
using System.Collections.Generic;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.TrackingAnalyst;


namespace ArcGisTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Main = this;
        }
        bool mousestation = false;
        int mouseflag = -1;
        private ControlsSynchronizer m_controlsSynchronizer = null;
        double x;
        double y;
        public Form Form2;
        private ITOCControl m_tocControl;
        private IToolbarMenu m_menuLayer;
        public static MainForm Main;

        private void Form1_Load(object sender, EventArgs e)
        {
          // this.axMapControl1.Map.ClearLayers();

           axTOCControl1.SetBuddyControl(this.axMapControl1);
            //IMapControl3 mapcont3 = this.axMapControl1.Object as IMapControl3;
            // IPageLayoutControl2 pagecont2 = this.axPageLayoutControl1.Object as IPageLayoutControl2;
            // m_controlsSynchronizer = new ControlsSynchronizer(mapcont3, pagecont2);
            //  m_controlsSynchronizer.BindControls(true);
            //  m_controlsSynchronizer.AddFrameworkControl(this.axToolbarControl1.Object);
            //  m_controlsSynchronizer.AddFrameworkControl(this.axTOCControl1.Object);
           OpenNewMapDocument openMapDoc = new OpenNewMapDocument(m_controlsSynchronizer);
          // axToolbarControl1.AddItem(openMapDoc, -1, 0, false, -1, esriCommandStyles.esriCommandStyleIconOnly);
            this.axMapControl2.Map = new MapClass();
            // 添加主地图控件中的所有图层到鹰眼控件中 
            for (int i = 0; i < this.axMapControl1.LayerCount; i++)
            {
                this.axMapControl2.AddLayer(this.axMapControl1.get_Layer(this.axMapControl1.LayerCount - i - 1));
                comboBox1.Items.Add(this.axMapControl1.get_Layer(i).Name);

            }
            // 设置 MapControl 显示范围至数据的全局范围
            this.axMapControl2.Extent = this.axMapControl1.FullExtent;
            // 刷新鹰眼控件地图
            this.axMapControl2.Refresh(); ;
            m_menuLayer = new ToolbarMenuClass();
            m_menuLayer.AddItem(new RemoveLayer(), -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new ScaleThresholds(), 1, 1, true, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new ScaleThresholds(), 2, 2, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new ScaleThresholds(), 3, 3, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new LayerSelectable(), 1, 4, true, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new LayerSelectable(), 2, 5, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new ZoomToLayer(), -1, 6, true, esriCommandStyles.esriCommandStyleTextOnly);
            //Set the hook of each menu
            m_menuLayer.SetHook(axMapControl1);
           

        }
        public void display(string str)
        {
            try
            {
                this.axMapControl1.Map.ClearSelection();
                IFeatureLayer pfeaturelayer = this.axMapControl1.Map.get_Layer(0) as IFeatureLayer;
                IQueryFilter Filter = new QueryFilterClass();
                Filter.WhereClause = "OBJECTID=" + str;
                IFeatureSelection pselection = pfeaturelayer as IFeatureSelection;
                pselection.SelectFeatures(Filter, esriSelectionResultEnum.esriSelectionResultNew, false);
                IFeatureCursor Ics;
                Ics = pfeaturelayer.Search(Filter, false);
                IFeature p = Ics.NextFeature();
                if (p == null) return;
                IEnvelope ie;
                IGeometry igeo = p.ShapeCopy;
                ie = igeo.Envelope;
                if (ie.Width == 0)
                {
                    ie.Expand(1, 1, false);
                }
                //this.axMapControl1.Extent = ie;
                this.axMapControl1.FullExtent = ie;
                this.axMapControl1.ActiveView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void axMapControl1_OnMapReplaced(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMapReplacedEvent e)
        {
            comboBox1.Items.Clear();
            this.axMapControl2.Map = new MapClass();
            // 添加主地图控件中的所有图层到鹰眼控件中
            for (int i = 0; i < this.axMapControl1.LayerCount; i++)
            {
                this.axMapControl2.AddLayer(this.axMapControl1.get_Layer(this.axMapControl1.LayerCount - i - 1));
                comboBox1.Items.Add(this.axMapControl1.get_Layer(i).Name);

            }
            
            // 设置 MapControl 显示范围至数据的全局范围
            this.axMapControl2.Extent = this.axMapControl1.FullExtent;
            // 刷新鹰眼控件地图
            this.axMapControl2.Refresh();
            // CopyAndOverwriteMap();

        }

        private void axMapControl1_OnExtentUpdated(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            IEnvelope pEnv = (IEnvelope)e.newEnvelope;
            IGraphicsContainer pGra = axMapControl2.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            // 在绘制前，清除 axMapControl2 中的任何图形元素
            pGra.DeleteAllElements();
            IRectangleElement pRectangleEle = new RectangleElementClass();
            IElement pEle = pRectangleEle as IElement;
            pEle.Geometry = pEnv;
            // 设置鹰眼图中的红线框
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;
            // 产生一个线符号对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 2;
            pOutline.Color = pColor;
            // 设置颜色属性
            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 0;
            // 设置填充符号的属性
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            IFillShapeElement pFillShapeEle = pEle as IFillShapeElement;
            pFillShapeEle.Symbol = pFillSymbol;
            pGra.AddElement((IElement)pFillShapeEle, 0);
            // 刷新
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void axMapControl2_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.axMapControl2.Map.LayerCount != 0)
            {
                // 按下鼠标左键移动矩形框
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    IEnvelope pEnvelope = this.axMapControl1.Extent;
                    pEnvelope.CenterAt(pPoint);
                    this.axMapControl1.Extent = pEnvelope;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                // 按下鼠标右键绘制矩形框
                else if (e.button == 2)
                {
                    IEnvelope pEnvelop = this.axMapControl2.TrackRectangle();
                    this.axMapControl1.Extent = pEnvelop;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }
        }

        private void axMapControl2_OnMouseMove(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);
            this.axMapControl1.CenterAt(pPoint);
            this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        private void 标注ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mouseflag = 2;
            mousestation = true;
        }
        //添加标注,比TextElment功能更强大
        public static void aAddAnnotate(ILayer layer, string fieldName)
        {
            IGeoFeatureLayer pGeoLayer = layer as IGeoFeatureLayer;
            IAnnotateLayerPropertiesCollection IPALPColl = pGeoLayer.AnnotationProperties;
            IPALPColl.Clear();
            IRgbColor pColor = GetColor(255, 0, 0, 255);
            IFontDisp pFont = new StdFont()
            {
                Name = "宋体",
                Bold = true
            } as IFontDisp;

            ITextSymbol pTextSymbol = new TextSymbolClass()
            {
                Color = pColor,
                Font = pFont,
                Size = 12
            };

            //用来控制标注和要素的相对位置关系
            ILineLabelPosition pLineLpos = new LineLabelPositionClass()
            {
                Parallel = false,  //修改标注的属性
                Perpendicular = true,
                InLine = true
            };
            //用来控制标注冲突
            ILineLabelPlacementPriorities pLinePlace = new LineLabelPlacementPrioritiesClass()
            {
                AboveStart = 5, //让above 和start的优先级为5
                BelowAfter = 4
            };
            //用来实现对ILineLabelPosition 和 ILineLabelPlacementPriorities以及更高级属性的控制
            IBasicOverposterLayerProperties pBOLP = new BasicOverposterLayerPropertiesClass()
            {
                FeatureType = esriBasicOverposterFeatureType.esriOverposterPolygon,
                LineLabelPlacementPriorities = pLinePlace,
                LineLabelPosition = pLineLpos
            };

            //创建标注对象
            ILabelEngineLayerProperties pLableEngine = new LabelEngineLayerPropertiesClass()
            {
                Symbol = pTextSymbol,
                BasicOverposterLayerProperties = pBOLP,
                IsExpressionSimple = true,
                Expression = "[" + fieldName + "]"
            };

            //设置标注的参考比例尺
            IAnnotateLayerTransformationProperties pAnnoLyrPros = pLableEngine as IAnnotateLayerTransformationProperties;
            pAnnoLyrPros.ReferenceScale = 2500000;

            //设置标注可见的最大最小比例尺
            IAnnotateLayerProperties pAnnoPros = pLableEngine as IAnnotateLayerProperties;
            pAnnoPros.AnnotationMaximumScale = 2500000;
            pAnnoPros.AnnotationMinimumScale = 25000000;
            //pAnnoPros.WhereClause属性  设置过滤条件

            IPALPColl.Add(pAnnoPros);
            pGeoLayer.DisplayAnnotation = true;
        }
        public static RgbColor GetColor(int r, int g, int b, byte a)
        {
            RgbColorClass pColor = new RgbColorClass();
            pColor.Red = r;
            pColor.Green = g;
            pColor.Blue = b;
            pColor.Transparency = a;
            return pColor;
        }

        private void CopyAndOverwriteMap()
        {
            IObjectCopy objectCopy = new ObjectCopyClass();
            //Get IUnknown interface (map to copy)
            object toCopyMap = axMapControl1.Map;
            //Get IUnknown interface (copied map)
            object copiedMap = objectCopy.Copy(toCopyMap);
            //Get IUnknown interface (map to overwrite)
            object toOverwriteMap = axPageLayoutControl1.ActiveView.FocusMap;
            //Overwrite the PageLayoutControl's map
            objectCopy.Overwrite(copiedMap, ref toOverwriteMap);
        }
        private void axPageLayoutControl1_OnMouseDown(object sender, IPageLayoutControlEvents_OnMouseDownEvent e)
        {

            if (e.button == 2)
            {
                int i = 0;
                IToolbarMenu mapPopMenu = null;
                mapPopMenu = new ToolbarMenu();
                mapPopMenu.AddItem(new ControlsSelectTool(), -1, i++, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsMapPanTool(), -1, i++, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsMapFullExtentCommand(), i++, 2, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsMapIdentifyTool(), -1, 3, false, esriCommandStyles.esriCommandStyleIconAndText);//识别工具
                mapPopMenu.AddItem(new ControlsMapZoomInFixedCommand(), i++, 4, false, esriCommandStyles.esriCommandStyleIconAndText);//
                mapPopMenu.AddItem(new ControlsMapZoomInFixedCommand(), -1, i++, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsSelectFeaturesTool(), -1, i++, false, esriCommandStyles.esriCommandStyleIconAndText);//选择要素工具
                mapPopMenu.AddItem(new ControlsZoomToSelectedCommand(), -1, i++, true, esriCommandStyles.esriCommandStyleTextOnly);
                mapPopMenu.AddItem(new ControlsClearSelectionCommand(), -1, i++, false, esriCommandStyles.esriCommandStyleIconAndText);//缩放所选要素
                mapPopMenu.AddItem(new ControlsEditingCutCommand(), -1, i++, false, esriCommandStyles.esriCommandStyleIconAndText);//缩放所选要素                 
                mapPopMenu.AddItem(new ControlsAddDataCommand(), -1, i++, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsEditingSaveCommand(), -1, i++, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsEditingSketchPropertiesCommand(), -1, i++, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.SetHook(axPageLayoutControl1);//// 得到地图视窗右键菜单
                mapPopMenu.PopupMenu(e.x, e.y, axPageLayoutControl1.hWnd);//弹出显示
                mousestation = false;
                mouseflag = -1;
            }
            if (mouseflag == 2 && mousestation == true)
            {
                IActiveView pActiveView;
                pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                ITextElement pTextEle;
                pTextEle = new TextElementClass();
                IElement pEles;
                pTextEle.Text = "文本";
                pEles = pTextEle as IElement;
                //设置文字字符的几何形体属性
                IPoint pPoint;
                pPoint = new PointClass();
                pPoint.PutCoords(e.pageX, e.pageY);
                pEles.Geometry = pPoint;
                IGraphicsContainer pGraphicsContainer;
                pGraphicsContainer = pActiveView as IGraphicsContainer;
                pGraphicsContainer.AddElement(pEles, 0);
                axPageLayoutControl1.Refresh();
                // pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
        }


        private void 渲染ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* ClassRender pClassRender = new ClassRender(axMapControl1, axMapControl1.Map.get_Layer(0) as IFeatureLayer, 5, "area");
             tocMain.ActiveView.Refresh();*/
        }

        private void 添加面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IEnvelope pEnvelop = this.axMapControl2.TrackRectangle();
            this.axMapControl1.Extent = pEnvelop;
            this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        public static void AddTempElement(AxMapControl pMapCtrl, IElement pEle, IElementCollection pEleColl)
        {
            try
            {
                IMap pMap = pMapCtrl.Map;
                IGraphicsContainer pGCs = pMap as IGraphicsContainer;
                if (pEle != null)
                    pGCs.AddElement(pEle, 0);
                if (pEleColl != null)
                    if (pEleColl.Count > 0)
                        pGCs.AddElements(pEleColl, 0);
                IActiveView pAV = (IActiveView)pMap;//需要刷新才能即时显示
                pAV.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, pAV.Extent);
            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                PrintAuto(this.axMapControl1.ActiveView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "提示!");
            }
        }
        private void PrintAuto(IActiveView pActiveView)
        {

            IPaper pPaper = new Paper();
            IPrinter pPrinter = new EmfPrinterClass();

            System.Drawing.Printing.PrintDocument sysPrintDocumentDocument = new System.Drawing.Printing.PrintDocument();

            //pPaper.PrinterName = sysPrintDocumentDocument.PrinterSettings.PrinterName;
            pPrinter.Paper = pPaper;

            int Resolution = pPrinter.Resolution;

            double w, h;
            IEnvelope PEnvelope = pActiveView.Extent;
            w = PEnvelope.Width;
            h = PEnvelope.Height;
            double pw, ph;//纸张
            pPrinter.QueryPaperSize(out pw, out ph);
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

            ITrackCancel pCancel = new CancelTrackerClass();
            int hdc = pPrinter.StartPrinting(pDriverBounds, 0);

            pActiveView.Output(hdc, pPrinter.Resolution,
            ref userRECT, pActiveView.Extent, pCancel);

            pPrinter.FinishPrinting();

        }


        private void axToolbarControl1_OnMouseDown(object sender, IToolbarControlEvents_OnMouseDownEvent e)
        {

        }

        private void 添加线ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ControlsNewCurveTool Cmd = new ControlsNewCurveTool();
            Cmd.OnCreate(this.axMapControl1.Object);
            this.axMapControl1.CurrentTool = Cmd as ITool;
            /*try
            {
                mouseflag = 3;
                mousestation = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "提示");
             *}
            /*IMap pMap = axMapControl1.Map;
            IActiveView pActive = pMap as IActiveView;
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();  //设置Symbol属性
            pLineSymbol.Color.RGB = 125;
            pLineSymbol.Width = 3;
            IGeometry pGeo = axMapControl1.TrackLine();

            ILineElement pLineElement = new LineElementClass();
            IElement pEle = pLineElement as IElement;
            pLineElement.Symbol = pLineSymbol;
            pEle.Geometry = pGeo;
            IGraphicsContainer pContainer = pMap as IGraphicsContainer;
            pContainer.AddElement(pEle, 0);
            pActive.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);*/
        }

        private void 图例ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IElement pElement = this.axPageLayoutControl1.FindElementByName("Legend");
            if (pElement != null)
            {
                this.axPageLayoutControl1.ActiveView.GraphicsContainer.DeleteElement(pElement);  //删除已经存在的图例  
            }
            try
            {
                AddLegend(axPageLayoutControl1.PageLayout);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "tishi");
            }
        }
        private void AddLegend(IPageLayout pageLayout)
        {
            IActiveView pActiveView = pageLayout as IActiveView;
            IGraphicsContainer container = pageLayout as IGraphicsContainer;
            // 获得MapFrame           
            IMapFrame mapFrame = container.FindFrame(pActiveView.FocusMap) as IMapFrame;
            //根据MapSurround的uid，创建相应的MapSurroundFrame和MapSurround               
            UID uid = new UIDClass();
            uid.Value = "esriCarto.Legend";
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uid, null);
            //设置图例的Title                
            ILegend2 legend = mapSurroundFrame.MapSurround as ILegend2;
            legend.Title = "图例";
            ILegendFormat format = new LegendFormatClass();
            ITextSymbol symbol = new TextSymbolClass();
            symbol.Size = 4;
            format.TitleSymbol = symbol;
            legend.Format = format;
            //QI，确定mapSurroundFrame的位置               
            IElement element = mapSurroundFrame as IElement;
            IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords(2, 2, 8, 8);
            element.Geometry = envelope;
            //使用IGraphicsContainer接口添加显示               
            container.AddElement(element, 0);
            pActiveView.Refresh();
        }
        private void axMapControl1_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            //Get IActiveView interface
            IActiveView activeView = (IActiveView)
            axPageLayoutControl1.ActiveView.FocusMap;
            //Get IDisplayTransformation interface
            IDisplayTransformation displayTransformation =
            activeView.ScreenDisplay.DisplayTransformation;
            //Set the visible extent of the focus map
            displayTransformation.VisibleBounds = axMapControl1.Extent;
            axPageLayoutControl1.ActiveView.Refresh(); //根据MapControl的视图范围,确定PageLayoutControl的视图范围
            CopyAndOverwriteMap();
        }

        private void axMapControl1_OnViewRefreshed(object sender, IMapControlEvents2_OnViewRefreshedEvent e)
        {
            CopyAndOverwriteMap();
        }

        public void MapOperate(AxMapControl axMapControl1, IMapControlEvents2_OnMouseDownEvent e, string strOperat)
        {
            RgbColor pColor = null;
            if (pColor == null)
            {
                pColor = new RgbColorClass();
                pColor.Blue = 5;
                pColor.Red = 213;
                pColor.Green = 9;


            }
            switch (strOperat)
            {
                case "strLKoper":
                    {
                        axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                        axMapControl1.Extent = axMapControl1.TrackRectangle();
                        axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                        break;
                    }
                case "strMapPan":
                    {
                        axMapControl1.Pan();
                        break;
                    }


                //绘制线
                case "strDrawLine":
                    {
                        //绘制线

                        IMap pMap;
                        IActiveView pActiveView;
                        pMap = axMapControl1.Map;
                        pActiveView = pMap as IActiveView;
                        IPolyline pPolyline;
                        pPolyline = axMapControl1.TrackLine() as IPolyline;
                        //产生一个SimpleLineSymbol符号
                        ISimpleLineSymbol pSimpleLineSym;
                        pSimpleLineSym = new SimpleLineSymbolClass();
                        pSimpleLineSym.Style = esriSimpleLineStyle.esriSLSSolid;//需要用户动态选择
                        //设置符号颜色

                        pSimpleLineSym.Color = pColor;//需要用户动态选择
                        pSimpleLineSym.Width = 1;
                        //产生一个PolylineElement对象
                        ILineElement pLineEle;
                        pLineEle = new LineElementClass();
                        IElement pEle;
                        pEle = pLineEle as IElement;
                        pEle.Geometry = pPolyline;
                        try
                        {  //将元素添加到Map对象之中
                            IGraphicsContainer pGraphicsContainer;

                            pGraphicsContainer = pMap as IGraphicsContainer;
                            pGraphicsContainer.AddElement(pEle, 0);
                            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                        }
                        catch (ExecutionEngineException e3)
                        {
                            MessageBox.Show(e3.ToString(), "两点距离太近");
                            return;
                        }
                        break;
                    }
                //绘制面
                case "strDrawPolygon":
                    {
                        //绘制面
                        IMap pMap;
                        IActiveView pActiveView;
                        pMap = axMapControl1.Map;
                        pActiveView = pMap as IActiveView;
                        IPolygon pPolygion;
                        pPolygion = axMapControl1.TrackPolygon() as IPolygon;
                        //产生一个SimpleFillSymbol符号
                        ISimpleFillSymbol pSimpleFillSym;
                        pSimpleFillSym = new SimpleFillSymbolClass();
                        pSimpleFillSym.Style = esriSimpleFillStyle.esriSFSDiagonalCross;//需要用户动态选择


                        pSimpleFillSym.Color = pColor;//需要用户动态选择
                        //产生一个PolygonElement对象
                        IFillShapeElement pPolygonEle;
                        pPolygonEle = new PolygonElementClass();
                        pPolygonEle.Symbol = pSimpleFillSym;
                        IElement pEle;
                        pEle = pPolygonEle as IElement;
                        pEle.Geometry = pPolygion;
                        //将元素添加到Map对象之中
                        IGraphicsContainer pGraphicsContainer;
                        pGraphicsContainer = pMap as IGraphicsContainer;
                        pGraphicsContainer.AddElement(pEle, 0);
                        pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

                        break;
                    }
                //地图标注
                case "strMapLable":
                    {
                        //地图标注
                        IMap pMap;
                        IActiveView pActiveView;
                        pMap = axMapControl1.Map;
                        pActiveView = pMap as IActiveView;

                        ITextElement pTextEle;
                        IElement pEles;
                        //建立文字符号对象，并设置相应的属性
                        pTextEle = new TextElementClass();

                        pTextEle.Text = "地图标注";
                        pEles = pTextEle as IElement;
                        //设置文字字符的几何形体属性
                        IPoint pPoint;
                        pPoint = new PointClass();
                        pPoint.PutCoords(x, y);

                        pEles.Geometry = pPoint;
                        //添加到Map对象中，并刷新显示
                        IGraphicsContainer pGraphicsContainer;
                        pGraphicsContainer = pMap as IGraphicsContainer;
                        pGraphicsContainer.AddElement(pEles, 0);
                        pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

                        break;
                    }
                case "strSelect":
                    {
                        //要素选择
                        //得到一个  包络线Envelope对象
                        IEnvelope pEnv;
                        pEnv = axMapControl1.TrackRectangle();
                        //新建选择集环境对象
                        ISelectionEnvironment pSelectionEnv;
                        pSelectionEnv = new SelectionEnvironmentClass();
                        //改变选择集的默认颜色



                        pSelectionEnv.DefaultColor = pColor;
                        //选择要素，并将其放入选择集
                        axMapControl1.Map.SelectByShape(pEnv, pSelectionEnv, false);
                        axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                        //需要遍历所选要素

                        break;
                    }
                case "LKselect":
                    {

                        IMap pMap;
                        IActiveView pActiveView;
                        pMap = axMapControl1.Map;
                        pActiveView = pMap as IActiveView;
                        IEnvelope pEnv;
                        pEnv = axMapControl1.TrackRectangle();
                        ISelectionEnvironment pSelct;
                        pSelct = new SelectionEnvironmentClass();

                        pSelct.DefaultColor = pColor;
                        pMap.SelectByShape(pEnv, pSelct, false);
                        pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                        break;

                    }
                case "PointSelect":
                    {
                        IMap pMap;
                        IActiveView pActiveView;
                        pMap = axMapControl1.Map;
                        pActiveView = pMap as IActiveView;
                        IPoint pPt;
                        pPt = new PointClass();
                        pPt.PutCoords(e.mapX, e.mapY);
                        IEnumElement pEnumEle;
                        //选择元素，其中1为容差
                        IGraphicsContainer pGraphicsContainer;
                        pGraphicsContainer = pMap as IGraphicsContainer;

                        pEnumEle = pGraphicsContainer.LocateElements(pPt, 10);
                        IElement pElement;
                        //获得单个元素
                        try
                        {
                            pElement = pEnumEle.Next();
                            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, pActiveView.Extent);
                        }
                        catch
                        {
                            return;
                        }
                        break;


                    }
                case "strDrawPoint":
                    {

                        ISimpleMarkerSymbol pMarkerSymbol;
                        pMarkerSymbol = new SimpleMarkerSymbolClass();
                        pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;


                        pMarkerSymbol.Color = pColor;
                        pMarkerSymbol.Angle = 60;
                        pMarkerSymbol.Size = 6;
                        pMarkerSymbol.Outline = true;
                        pMarkerSymbol.OutlineSize = 2;
                        pMarkerSymbol.OutlineColor = pColor;
                        IPoint pPoint;
                        pPoint = new PointClass();
                        pPoint.PutCoords(e.mapX, e.mapY);
                        object oMarkerSymbol = pMarkerSymbol;
                        axMapControl1.DrawShape(pPoint, ref oMarkerSymbol);
                        break;



                    }
                default: break;
            }
        }

        private void 添加点ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                mousestation = true;
                mouseflag = 1;
                //axMapControl1_OnMouseDown(null, null);
                //ILayer layer=this.axMapControl1.Map.get_Layer(0);
                //addpoint(layer, double.Parse(Xbox.Text.ToString()), double.Parse(Ybox.Text.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "sa");
            }
        }

        private void 指北针ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IElement pElement = this.axPageLayoutControl1.FindElementByName("MarkerNorthArrow");
            if (pElement != null)
            {
                this.axPageLayoutControl1.ActiveView.GraphicsContainer.DeleteElement(pElement);  //删除已经存在的图例  
            }
            AddNorthArrow(axPageLayoutControl1.PageLayout);
        }
        public void AddNorthArrow(ESRI.ArcGIS.Carto.IPageLayout pageLayout)
        {
            IGraphicsContainer container = pageLayout as IGraphicsContainer;
            IActiveView activeView = pageLayout as IActiveView;
            IFrameElement frameElement = container.FindFrame(activeView.FocusMap);
            IMapFrame mapFrame = frameElement as IMapFrame;
            //根据MapSurround的uid，创建相应的MapSurroundFrame和MapSurround   
            UID uid = new UIDClass();
            uid.Value = "esriCarto.MarkerNorthArrow";
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uid, null);
            //设置MapSurroundFrame中指北针的点符号 
            IMapSurround mapSurround = mapSurroundFrame.MapSurround;
            IMarkerNorthArrow markerNorthArrow = mapSurround as IMarkerNorthArrow;
            IMarkerSymbol markerSymbol = markerNorthArrow.MarkerSymbol;
            markerSymbol.Size = 1;
            markerNorthArrow.MarkerSymbol = markerSymbol;
            //QI，确定mapSurroundFrame的位置   
            IElement element = mapSurroundFrame as IElement;
            IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords(0.2, 0.2, 5, 5);
            element.Geometry = envelope;
            //使用IGraphicsContainer接口添加显示   
            container.AddElement(element, 0);
            activeView.Refresh();
        }

        private void 比例尺ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddScalebar(axPageLayoutControl1.PageLayout);
        }
        public void AddScalebar(IPageLayout pageLayout)
        {
            IGraphicsContainer container = pageLayout as IGraphicsContainer;
            IActiveView activeView = pageLayout as IActiveView;
            // 获得MapFrame   
            IFrameElement frameElement = container.FindFrame(activeView.FocusMap);
            IMapFrame mapFrame = frameElement as IMapFrame;
            //根据MapSurround的uid，创建相应的MapSurroundFrame和MapSurround 
            UID uid = new UIDClass();
            uid.Value = "esriCarto.AlternatingScaleBar";
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uid, null);
            //设置MapSurroundFrame中比例尺的样式   
            IMapSurround mapSurround = mapSurroundFrame.MapSurround;
            IScaleBar markerScaleBar = ((IScaleBar)mapSurround);
            markerScaleBar.LabelPosition = esriVertPosEnum.esriBottom;
            markerScaleBar.UseMapSettings();
            //QI，确定mapSurroundFrame的位置   
            IElement element = mapSurroundFrame as IElement;
            IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords(0.2, 0.2, 1, 2);
            element.Geometry = envelope;
            //使用IGraphicsContainer接口添加显示              
            container.AddElement(element, 0);
            activeView.Refresh();
        }

        //private void axPageLayoutControl1_OnAfterScreenDraw(object sender, IPageLayoutControlEvents_OnAfterScreenDrawEvent e)
        //{

        //}

        private void axPageLayoutControl1_OnMouseMove(object sender, IPageLayoutControlEvents_OnMouseMoveEvent e)
        {
            blank.Text = "On pagelayout!";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IColor pColor = new RgbColor();
            pColor.RGB = 255;
            tagRECT pTag = new tagRECT();
            pTag.left = this.Left + button1.Left + button1.Width;
            pTag.bottom = this.Top + button1.Top + button1.Height;
            IColorPalette pColorPalette = new ColorPalette();
            pColorPalette.TrackPopupMenu(ref pTag, pColor, false, 0);
            pColor = pColorPalette.Color;
            MessageBox.Show(pColor.ToString());
        }


        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            int index = axToolbarControl1.HitTest(e.x, e.y, false);
            if (index != -1)
            {
                IToolbarItem bar = axToolbarControl1.GetItem(index);
                messagelabel.Text = bar.Command.Message;
            }
            else
            {
                messagelabel.Text = "就绪";
            }
            scalelabel.Text = "比例尺1：" + ((long)this.axMapControl1.MapScale).ToString();
            coordinatelabel.Text = "当前坐标X: " + e.mapX.ToString().Substring(0, 5) + " Y:" + e.mapY.ToString().Substring(0, 5) + this.axMapControl1.MapUnits;
        }

        private void 添加面ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            mousestation = true;
            mouseflag = 4;
        }

        private void axMapControl1_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            //MessageBox.Show(e.mapX.ToString(), e.mapY.ToString());
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {

            if (e.button == 2)
            {
                mouseflag = -1;
                IToolbarMenu mapPopMenu = null;
                mapPopMenu = new ToolbarMenu();
                //地图视窗右键菜单功能
                mapPopMenu.AddItem(new ControlsSelectTool(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsMapPanTool(), -1, 1, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsMapFullExtentCommand(), -1, 2, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsMapIdentifyTool(), -1, 3, false, esriCommandStyles.esriCommandStyleIconAndText);//识别工具
                mapPopMenu.AddItem(new ControlsMapZoomInFixedCommand(), -1, 4, false, esriCommandStyles.esriCommandStyleIconAndText);//
                mapPopMenu.AddItem(new ControlsMapZoomInFixedCommand(), -1, 5, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsSelectFeaturesTool(), -1, 6, false, esriCommandStyles.esriCommandStyleIconAndText);//选择要素工具
                mapPopMenu.AddItem(new ControlsClearSelectionCommand(), -1, 7, false, esriCommandStyles.esriCommandStyleIconAndText);//缩放所选要素
                mapPopMenu.AddItem(new ControlsZoomToSelectedCommand(), -1, 8, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsMapZoomToLastExtentBackCommand(), -1, 9, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsMapZoomToLastExtentForwardCommand(), -1, 10, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.AddItem(new ControlsEditingSketchPropertiesCommand(), -1, -1, false, esriCommandStyles.esriCommandStyleIconAndText);
                mapPopMenu.SetHook(axMapControl1);//// 得到地图视窗右键菜单
                mapPopMenu.PopupMenu(e.x, e.y, axMapControl1.hWnd);//弹出显示
            }
            if (mousestation == true && mouseflag == 1)
            {
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                IMap pMap = axMapControl1.Map;
                IActiveView activeView = axMapControl1.ActiveView;
                IPoint pPoint = new PointClass();
                pPoint.X = e.mapX;
                pPoint.Y = e.mapY;
                IMarkerElement pMarkerElement = new MarkerElementClass();
                ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
                pMarkerSymbol.Color.RGB = 125;
                pMarkerSymbol.Size = 5;
                pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSX;
                IElement pElement = (IElement)pMarkerElement;
                pElement.Geometry = pPoint;
                pMarkerElement.Symbol = pMarkerSymbol;
                IGraphicsContainer pGraphicsContainer = (IGraphicsContainer)pMap;
                pGraphicsContainer.AddElement((IElement)pMarkerElement, 0);
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            IGeometry geometry = null;
            if (mousestation == true && mouseflag == 3)
            {
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                geometry = axMapControl1.TrackLine();
                drawMapShape(geometry);
            }
            if (mousestation = true && mouseflag == 4)
            {
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                x = e.mapX;
                y = e.mapY;
                geometry = axMapControl1.TrackPolygon();
                drawMapShape(geometry);
            }
            if (mousestation = true && mouseflag == 5)
            {
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                geometry = axMapControl1.TrackRectangle();
                drawMapShape(geometry);
            }
            if (mousestation = true && mouseflag == 6)
            {
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                geometry = axMapControl1.TrackCircle();
                drawMapShape(geometry);
            }
            if (e.button == 1 && mouseflag == 7)
            {

                IGeometry g = null;
                IEnvelope pEnv;
                IActiveView pActiveView = axMapControl1.ActiveView;
                IMap pMap = axMapControl1.Map;
                pEnv = axMapControl1.TrackRectangle();
                if (pEnv is IPoint)
                {

                    pEnv.Expand(1.0, 1.0, false);
                }
                g = pEnv as IGeometry;
                SelectFeature(g);
                pActiveView.Refresh();
            }
            if (mousestation == true && mouseflag == 100)
            {
                IGeometry ige;
                ige = this.axMapControl1.TrackLine();
                IElement element = new LineElementClass();
                element.Geometry = ige;
                addElement(element, this.axMapControl1.get_Layer(0) as IFeatureLayer);

            }
            if (e.button == 1 && mouseflag == 8)
            {
                try
                {
                    pFeatures.Clear();
                    int i = 0;
                    while (i < 2)
                    {
                        IFeatureLayer P = this.axMapControl1.Map.get_Layer(0) as IFeatureLayer;
                        IPoint pp = new PointClass();
                        System.Drawing.Point p1 = MousePosition;
                        System.Drawing.Point p2 = this.PointToClient(p1);
                        pp.X = p2.X;
                        pp.Y = p2.Y;
                        ITopologicalOperator pTOpo = pp as ITopologicalOperator;
                        IGeometry pBuffer = pTOpo.Buffer(5.0);
                        IGeometry pGeomentry = pBuffer.Envelope;
                        IQueryFilter qfilter = null;
                        IFeatureSelection pselection = P as IFeatureSelection;
                        pselection.SelectFeatures(qfilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                        IFeatureCursor fcursor = P.Search(qfilter, false);
                        IFeature pFeature = fcursor.NextFeature();
                        while (pFeature != null)
                        {
                            // MessageBox.Show(pFeature.get_Value(pFeature.Fields.FindField("NAME")).ToString());
                            if (CheckGeometryContain((IGeometry)pFeature.Shape, (IGeometry)pp))
                            {
                                //MessageBox.Show("一个要素");
                                pFeatures.Add(pFeature);
                                break;
                            }
                            else
                                pFeature = fcursor.NextFeature();
                        }
                        i++;
                        System.Threading.Thread.Sleep(3000);
                    }
                    IGeometry A = pFeatures[0].Shape as IGeometry;
                    IGeometry B = pFeatures[1].Shape as IGeometry;
                    double distance = GetTwoGeometryDistance(A, B);
                    MessageBox.Show(pFeatures[0].get_Value(pFeatures[0].Fields.FindField("NAME")).ToString() + "距" +
                        pFeatures[1].get_Value(pFeatures[1].Fields.FindField("NAME")).ToString() + distance.ToString());

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            if (mousestation == true && mouseflag == 9)
            {
                IArea Test;
                IGeometry ie;
                ie = axMapControl1.TrackRectangle() as IGeometry;
                Test = ie as IArea;
                MessageBox.Show("所画圆的面积为：" + Test.Area.ToString(), "结果");
            }
        }
        private double GetTwoGeometryDistance(IGeometry pGeometryA, IGeometry pGeometryB)
        {
            IProximityOperator pProOperator = pGeometryA as IProximityOperator;
            if (pGeometryA != null || pGeometryB != null)
            {
                double distance = pProOperator.ReturnDistance(pGeometryB);
                return distance;
            }
            else
            {
                return 0;
            }
        }
        private bool CheckGeometryContain(IGeometry pGeometryA, IGeometry pGeometryB)
        {
            IRelationalOperator pRelOperator = pGeometryA as IRelationalOperator;
            if (pRelOperator.Contains(pGeometryB))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private Property form = null;
        bool ISformshow = false;
        private void
            SelectFeature(IGeometry Pgeometry)
        {
            IFeatureLayer P = this.axMapControl1.Map.get_Layer(0) as IFeatureLayer;
            if (P == null)
            {
                MessageBox.Show("无图层！");
            }
            ISpatialFilter pspatil = new SpatialFilterClass();
            IQueryFilter qfilter = pspatil as ISpatialFilter;
            pspatil.Geometry = Pgeometry;
            //List<IFeature> pList = new List<IFeature>();
            pspatil.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureSelection pselection = P as IFeatureSelection;
            pselection.SelectFeatures(qfilter, esriSelectionResultEnum.esriSelectionResultNew, false);
            ISelectionSet iset = pselection.SelectionSet;
            IFeatureLayerDefinition pFDefinition = P as IFeatureLayerDefinition;
            //MessageBox.Show(iset.Count.ToString());
            //创建新图层
            if (iset != null)
            {
                if (ISformshow)
                {
                    form.Close();
                    form.Dispose();
                    //form.dataGridView1.Columns.Clear();
                }
                form = new Property();
                form.Owner = this;
                IFeatureLayer pNewFeatureLayer = pFDefinition.CreateSelectionLayer("newlayerName", true, null, null);
                pNewFeatureLayer.Name = "选择要素集";
                axMapControl1.Map.AddLayer(pNewFeatureLayer as ILayer);//将要素集创建新图层并显示
                qfilter = null;
                IFeatureCursor fcursor = pNewFeatureLayer.Search(qfilter, false);
                IFeature pFeature = fcursor.NextFeature();
                int i = 0;
                //遍历选择要素图层并将要素信息添加到Gridview中
                bool Added = true;
                while (pFeature != null)
                {
                    try
                    {

                        this.axMapControl1.Map.SelectFeature(this.axMapControl1.Map.get_Layer(0), pFeature);
                        if (Added == true)
                        {
                            for (int j = 0; j < pFeature.Fields.FieldCount; j++)
                            {
                                form.dataGridView1.Columns.Add(null, pFeature.Fields.Field[j].AliasName.ToString());
                            }
                            Added = false;
                        }
                        form.dataGridView1.Rows.Add();
                        for (int j = 0; j < pFeature.Fields.FieldCount; j++)
                        {
                            //form.dataGridView1.Columns.Add(null, null);
                            form.dataGridView1.Rows[i].Cells[j].Value = pFeature.get_Value(j).ToString();
                        }
                        pFeature = fcursor.NextFeature();
                        i++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                }
                //弹出显示选择要素信息
                form.Show();
                ISformshow = true;
                this.axMapControl1.Map.ClearLayers();
                this.axMapControl1.AddLayer(P);
            }
        }

        private void drawMapShape(IGeometry pGeom)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            object symbol = null;
            if (pGeom.GeometryType == esriGeometryType.esriGeometryPolyline)
            {
                ISimpleLineSymbol simpleLineSymbol;
                simpleLineSymbol = new SimpleLineSymbolClass();
                simpleLineSymbol.Color = pColor;
                simpleLineSymbol.Width = 5;
                symbol = simpleLineSymbol;
            }
            else
            {
                ISimpleFillSymbol simpleFillSymbol;
                simpleFillSymbol = new SimpleFillSymbolClass();
                simpleFillSymbol.Color = pColor;
                symbol = simpleFillSymbol;
            }

            axMapControl1.DrawShape(pGeom, ref symbol);
            IElement pEle = pGeom as IElement;
            IGraphicsContainer pGC = this.axMapControl1.Map as IGraphicsContainer;
            pGC.AddElement(pEle, 0);
            this.axMapControl1.Refresh();



        }

        private void axMapControl1_OnKeyUp(object sender, IMapControlEvents2_OnKeyUpEvent e)
        {
            mousestation = false;
            mouseflag = 0;
        }

        private void 渲染ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private void axTOCControl1_OnMouseDown_1(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            if (e.button != 2) return;
            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap map = null;
            ILayer layer = null;
            object other = null;
            object index = null;
            //Determine what kind of item has been clicked on
            axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
            // Only implemented a context menu for NALayers.  Exit if the layer is anything else.
            //if ((layer as INALayer) == null)
            //return;
            m_tocControl = (ITOCControl)axTOCControl1.Object;
            axTOCControl1.SelectItem(layer);
            // Set the layer into the CustomProperty.
            // This is used by the other commands to know what layer was right-clicked on
            // in the table of contents.   
            axMapControl1.CustomProperty = layer;
            //Popup the correct context menu and update the TOC when it's done.
            if (item == esriTOCControlItem.esriTOCControlItemLayer)
            {
                m_menuLayer.PopupMenu(e.x, e.y, m_tocControl.hWnd);
                ITOCControl toc = axTOCControl1.Object as ITOCControl;
                toc.Update();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ILayer sElectedLayer;
                sElectedLayer = this.axMapControl1.Map.get_Layer(comboBox1.SelectedIndex);
                this.axMapControl1.ClearLayers();
                this.axMapControl1.AddLayer(sElectedLayer);
                this.axMapControl1.Refresh();
                comboBox1.Items.Clear();
                comboBox1.Items.Add(sElectedLayer.Name);

            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.ToString());
            }
        }

        private void 添加矩形ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mousestation = true;
            mouseflag = 5;
        }

        private void 添加圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mousestation = true;
            mouseflag = 6;
        }

        private void 设置ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                IActiveView pActive;
                pActive = this.axMapControl1.Map as IActiveView;
                PrintAuto(pActive);
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.ToString(), "提示！");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                IFeatureLayer pFeatureLayer = this.axMapControl1.get_Layer(0) as IFeatureLayer;
                //QI到FeatureSelection
                IFeatureSelection pFeatureSelection = pFeatureLayer as IFeatureSelection;

                //创建过滤器
                IQueryFilter pQueryFilter = new QueryFilterClass();

                //设置过滤器对象的查询条件
                pQueryFilter.WhereClause = "AREA>10 and area<40";
                //根据查询条件选择要素
                pFeatureSelection.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

                //QI到ISelectionSet
                ISelectionSet pSelectionSet = pFeatureSelection.SelectionSet;
                //MessageBox.Show(pSelectionSet.Count.ToString());

                if (pSelectionSet.Count > 0)
                {

                    IFeatureLayerDefinition pFDefinition = pFeatureLayer as IFeatureLayerDefinition;

                    //创建新图层

                    IFeatureLayer pNewFeatureLayer = pFDefinition.CreateSelectionLayer("newlayerName", true, null, null);
                    pNewFeatureLayer.Name = "查询结果";
                    axMapControl1.AddLayer(pNewFeatureLayer as ILayer);
                }
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.ToString());
            }
        }

        private void 要素查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Property form = new Property();
                IActiveView IA = this.axMapControl1.Map as IActiveView;
                IFeatureLayer pFeaturelayer = this.axMapControl1.Map.get_Layer(0) as IFeatureLayer;
                IQueryFilter queryfliter = new QueryFilterClass();
                // queryfliter.WhereClause = "NAME  like '%" + textBox1.Text + "%'";
                IFeatureCursor cursor = pFeaturelayer.Search(queryfliter, false);
                IFeature pFeature;
                pFeature = cursor.NextFeature();
                if (pFeature == null)
                {
                    MessageBox.Show("没有查询到任何要素");
                    return;
                }
                int i = 0;
                while (pFeature != null)
                {
                    form.dataGridView1.Rows.Add();
                    try
                    {
                        //MessageBox.Show(pFeature.get_Value(pFeature.Fields.FindField("NAME")).ToString(), "提示");
                        //name.Text = pFeature.get_Value(pFeature.Fields.FindField("NAME")).ToString();
                        //// ADcode93.Text = pFeature.get_Value(pFeature.Fields.FindField("ADCODE93")).ToString();
                        //ADcode99.Text = pFeature.get_Value(pFeature.Fields.FindField("ADCODE99")).ToString();
                        // AREA.Text = pFeature.get_Value(pFeature.Fields.FindField("AREA")).ToString();
                        // PERIMETER.Text = pFeature.get_Value(pFeature.Fields.FindField("PERIMETER")).ToString();
                        //this.axMapControl1.Map.SelectFeature(this.axMapControl1.Map.get_Layer(0), pFeature);                        
                        //this.dataGridView1[1, 0].Value = pFeature.get_Value(pFeature.Fields.FindField("NAME")).ToString();
                        form.dataGridView1.Rows[i].Cells[0].Value = i + 1;//要素编号
                        form.dataGridView1.Rows[i].Cells[1].Value = pFeature.get_Value(pFeature.Fields.FindField("FID")).ToString();
                        form.dataGridView1.Rows[i].Cells[2].Value = pFeature.get_Value(pFeature.Fields.FindField("NAME")).ToString();
                        form.dataGridView1.Rows[i].Cells[3].Value = pFeature.get_Value(pFeature.Fields.FindField("ADCODE93")).ToString();
                        form.dataGridView1.Rows[i].Cells[4].Value = pFeature.get_Value(pFeature.Fields.FindField("ADCODE99")).ToString();
                        form.dataGridView1.Rows[i].Cells[5].Value = pFeature.get_Value(pFeature.Fields.FindField("AREA")).ToString();
                        form.dataGridView1.Rows[i].Cells[6].Value = pFeature.get_Value(pFeature.Fields.FindField("PERIMETER")).ToString();
                        i++;
                        pFeature = cursor.NextFeature();


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                form.Show();
                //axMapControl1.MoveLayerTo(1,0);
                IA.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpF = new OpenFileDialog();
            OpF.Title = "打开地图文档";
            //文件类型过滤
            OpF.Filter = ".mxd|*.mxd|.shp|*.shp|.GDB|*gdb";
            //多选
            OpF.Multiselect = true;
            string filePath = null;
            OpF.ShowDialog();
            filePath = OpF.FileName;

            if (filePath != null && filePath.Contains("mxd"))
            {
                if (axMapControl1.CheckMxFile(filePath))
                {
                    //加载地图
                    axMapControl1.LoadMxFile(filePath);
                }
            }
            if (filePath.Contains("shp"))
            {
                string preFix = System.IO.Path.GetFileName(filePath);
                //得到绝对路径
                string pAth = System.IO.Path.GetDirectoryName(filePath);
                //得到文件名
                string fileName = System.IO.Path.GetFileNameWithoutExtension(preFix);
                IWorkspaceFactory IworkSpace = new ShapefileWorkspaceFactoryClass();
                IWorkspace shpWorkSpace = IworkSpace.OpenFromFile(pAth, 0);
                IFeatureWorkspace featureClassWorkspace = shpWorkSpace as IFeatureWorkspace;
                IFeatureClass shpFeatureClass = featureClassWorkspace.OpenFeatureClass(fileName);
                IFeatureLayer PfeatureLayer = new FeatureLayerClass();
                PfeatureLayer.Name = shpFeatureClass.AliasName;
                PfeatureLayer.FeatureClass = shpFeatureClass;
                //  this.axMapControl1.ClearLayers();
                this.axMapControl1.AddLayer(PfeatureLayer as ILayer);
                this.axMapControl1.Refresh();
            }


        }
        private void 要素识别ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            mouseflag = 7;
        }

        private void 绘制直线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlsNewLineTool Cmd = new ControlsNewLineTool();
            Cmd.OnCreate(this.axMapControl1.Object);
            this.axMapControl1.CurrentTool = Cmd as ITool;
        }

        private void 要素查询ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void 打印ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            IHookHelper layout_hookHelper = new HookHelperClass();
            layout_hookHelper.Hook = this.axPageLayoutControl1.Object;
            axPageLayoutControl1.PageLayout = layout_hookHelper.PageLayout;
            Print PPLFrm = new Print(layout_hookHelper);
            PPLFrm.Show();
        }

        private void axPageLayoutControl1_OnDoubleClick(object sender, IPageLayoutControlEvents_OnDoubleClickEvent e)
        {
            try
            {
                IElement pelement, selectElement = null;
                IPageLayoutControlDefault pPageLayout = this.axPageLayoutControl1.Object as IPageLayoutControlDefault;
                IGraphicsContainerSelect pGraphicsContainerSelect = pPageLayout.PageLayout as IGraphicsContainerSelect;
                IGraphicsContainer pGraphicsContainer = pPageLayout.PageLayout as IGraphicsContainer;
                pGraphicsContainer.Reset();
                pelement = pGraphicsContainer.Next();
                while (pelement != null)
                {
                    if (pelement.HitTest(e.pageX, e.pageY, 0.1))
                    {
                        if (pelement is ITextElement)
                        {
                            selectElement = pelement;
                        }

                    }
                    pelement = pGraphicsContainer.Next();
                }
                if (selectElement is ITextElement)
                {
                    ITextElement rr = selectElement as ITextElement;
                    TextChange propertiesForm = new TextChange(pPageLayout, selectElement as ITextElement);
                    propertiesForm.TopMost = true;
                    propertiesForm.Show(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*   try
               {
                   if (axMapControl1.DocumentFilename != null)
                   {
                       IMapDocument mapDoc = new MapDocumentClass();
                       mapDoc.Open(axMapControl1.DocumentFilename, string.Empty);
                       mapDoc.ReplaceContents((IMxdContents)axMapControl1.Map);
                       mapDoc.Save(mapDoc.UsesRelativePaths, false);
                       mapDoc.Close();
                   }
               }
               catch (Exception ex) {
                   MessageBox.Show(ex.ToString());
               }*/
        }

        private void axMapControl1_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            IPoint pp = new PointClass();
            pp.X = e.mapX;
            pp.Y = e.mapY;
            IEnvelope objEnvelope = null;
            //objEnvelope.PutCoords(e.mapX, e.mapY - 5, e.mapX, e.mapY);
            objEnvelope = axMapControl1.Extent;
            objEnvelope.Expand(0.2, 0.2, true);
            axMapControl1.Extent = objEnvelope;
            /*IEnvelope pEnv;
            IActiveView pActiveView = this.axMapControl1.ActiveView;
            IMap pMap =this.axMapControl1.Map;
            pEnv = this.axMapControl1.TrackRectangle();
            if (pEnv.IsEmpty == true)                    //点选
            {
                tagRECT r;
                r.bottom =((int)e.mapY)+ 5;
                r.top = ((int)e.mapY) - 5;
                r.left = ((int)e.mapX) - 5;
                r.right = ((int)e.mapX) + 5;
                pActiveView.ScreenDisplay.DisplayTransformation.TransformRect(pEnv, ref r, 4);
                pEnv.SpatialReference = pActiveView.FocusMap.SpatialReference;
            }
            ISelectionEnvironment m_SelectEnvir = new SelectionEnvironmentClass();
            m_SelectEnvir = this.axMapControl1.Map.get_Layer(0) as ISelectionEnvironment;
            pMap.SelectByShape(pEnv,m_SelectEnvir, false);
            pActiveView.Refresh();
            pEnv = null;*/
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*mApDocument=new MapDocument();
            SaveFileDialog sFd = new SaveFileDialog();
            sFd.ShowDialog();
            sFd.Title = "保存文件";
            sFd.Filter=".mxd|*.mxd";
            string FilePath = sFd.FileName;
            try
            {
                if (FilePath == "") return;
                if (mApDocument.get_IsReadOnly(mApDocument.DocumentFilename) == true)
                {
                    MessageBox.Show("只读文档！");
                    return;
                }
                else
                {
                    mApDocument.SaveAs(FilePath, true, true);
                    MessageBox.Show("已成功保存到" + FilePath.ToString(), "提示");
                }

            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }*/

        }

        private void statusStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void axTOCControl1_OnDoubleClick(object sender, ITOCControlEvents_OnDoubleClickEvent e)
        {
            esriTOCControlItem itemType = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap basicMap = this.axMapControl1.Map as IBasicMap;
            ILayer layer = null;
            object unk = null;
            object data = null;
            try
            {
                axTOCControl1.HitTest(e.x, e.y, ref itemType, ref basicMap, ref layer, ref unk, ref data);
                //if (itemType == esriTOCControlItem.esriTOCControlItemLegendClass)
                //取得图例 
                ILegendClass pLegendClass = ((ILegendGroup)unk).get_Class((int)data);
                //创建符号选择器SymbolSelector实例 
                SymbolSelect SymbolSelectorFrm = new SymbolSelect(pLegendClass, layer);
                SymbolSelectorFrm.Visible = false;
                SymbolSelectorFrm.ShowDialog();
                if (SymbolSelectorFrm.ShowDialog() == DialogResult.OK)
                {
                    //局部更新主Map控件 
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                    //设置新的符号 
                    pLegendClass.Symbol = SymbolSelectorFrm.pSymbol;
                    //更新主Map控件和图层控件 
                    this.axMapControl1.ActiveView.Refresh();
                    this.axTOCControl1.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void 点密度图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取当前图层 ，并把它设置成IGeoFeatureLayer的实例             
            IMap pMap = axMapControl1.Map;
            ILayer pLayer = pMap.get_Layer(0) as IFeatureLayer;
            IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
            IGeoFeatureLayer pGeoFeatureLayer = pLayer as IGeoFeatureLayer;
            //获取图层上的feature 
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
            IFeature pFeature = pFeatureCursor.NextFeature();
            ///////////////////////             
            ///////////////////////////////////////////////////////////////////            
            //定义点密度图渲染组件 
            IDotDensityRenderer DotDensityRenderer = new DotDensityRendererClass();
            //定义点密度图渲染组件对象的渲染字段对象 
            IRendererFields flds = (IRendererFields)DotDensityRenderer;
            flds.AddField("AREA", "AREA");
            //flds.AddField("Shape", "Shape");           
            //定义点密度图渲染得符号对象 
            IDotDensityFillSymbol ddSym = new DotDensityFillSymbolClass();
            IRgbColor BackColor = new RgbColorClass();
            BackColor.Red = 234;
            BackColor.Blue = 128;
            BackColor.Green = 220;
            IRgbColor SymbolColor = new RgbColorClass();
            SymbolColor.Red = 234;
            SymbolColor.Blue = 128;
            SymbolColor.Green = 220;
            ////点密度图渲染背景颜色 
            //ddSym.BackgroundColor = BackColor;            
            ddSym.DotSize = 8;
            ddSym.FixedPlacement = true;
            //ddSym.Color = SymbolColor; 
            ILineSymbol pLineSymbol = new CartographicLineSymbolClass();
            ddSym.Outline = pLineSymbol;
            //定义符号数组  
            ISymbolArray symArray = (ISymbolArray)ddSym;
            //添加点密度图渲染的点符号到符号数组中去 
            ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            pMarkerSymbol.Size = 2;
            pMarkerSymbol.Color = SymbolColor; ;
            symArray.AddSymbol(pMarkerSymbol as ISymbol);
            //设置点密度图渲染的点符号        
            //DotDensityRenderer.DotDensitySymbol =symArray;               
            DotDensityRenderer.DotDensitySymbol = ddSym;
            //确定一个点代表多少值
            DotDensityRenderer.DotValue = 0.2;
            //点密度渲染采用的颜色模式 
            DotDensityRenderer.ColorScheme = "Custom";
            //创建点密度图渲染图例 
            DotDensityRenderer.CreateLegend();
            //设置符号大小是否固定 
            DotDensityRenderer.MaintainSize = true;
            //将点密度图渲染对象与渲染图层挂钩 
            pGeoFeatureLayer.Renderer = (IFeatureRenderer)DotDensityRenderer;
            //刷新地图和TOOCotrol 
            IActiveView pActiveView = axMapControl1.Map as IActiveView;
            pActiveView.Refresh();
            axTOCControl1.Update();
        }

        private void 分层着色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IMap pMap = axMapControl1.Map;
            ILayer pLayer = pMap.get_Layer(0) as IFeatureLayer;
            IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
            IGeoFeatureLayer pGeoFeatureLayer = pLayer as IGeoFeatureLayer;
            //获取图层上的feature 
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
            IFeature pFeature = pFeatureCursor.NextFeature();                                     // 
            IFeatureRenderer PR = pGeoFeatureLayer.Renderer;
            //JoinData("县级区域", "DZGB", "sectioncode");  
            //join外部表           
            // int DC    
            int desiredClasses = 5;
            string fieldName = "AREA";
            int classesCount;
            double[] classes;
            string strOutput = "";
            bool ok;
            object dataFrequency;
            object dataValues;
            ITable pTable;
            //IClassify pClassify;             
            EqualIntervalClass pClassify;
            //IBasicHistogram pTableHistogram = new BasicTableHistogramClass();            
            //IHistogram pTableHistogram = new BasicTableHistogramClass(); 
            ITableHistogram pTableHistogram = new BasicTableHistogramClass() as ITableHistogram;
            IBasicHistogram pHistogram;
            IClassBreaksRenderer pClassBreaksRenderer;
            IHsvColor pFromColor;
            IHsvColor pToColor;
            IAlgorithmicColorRamp pAlgorithmicColorRamp;
            IEnumColors pEnumColors;
            IColor pColor;
            ISimpleFillSymbol pSimpleFillSymbol;
            pLayer = (IFeatureLayer)axMapControl1.get_Layer(0);
            pGeoFeatureLayer = (IGeoFeatureLayer)pLayer;
            pTable = (ITable)pGeoFeatureLayer;
            pHistogram = (IBasicHistogram)pTableHistogram;
            // Get values and frequencies for the field            
            pTableHistogram.Field = fieldName;
            pTableHistogram.Table = pTable;
            pHistogram.GetHistogram(out dataValues, out dataFrequency);
            // Put values and frequencies into an Equal Interval Classify Object            
            pClassify = new EqualIntervalClass();
            //pClassify = new NaturalBreaksClass(); 
            pClassify.SetHistogramData(dataValues, dataFrequency);
            pClassify.Classify(dataValues, dataFrequency, ref desiredClasses);
            //pClassify.Classify(ref desiredClasses);            
            classes = (double[])pClassify.ClassBreaks;
            classesCount = classes.Length;
            // Initialise a new Class Breaks renderer 
            // Supply the number of Class Breaks and the field to perform. the class breaks on           
            pClassBreaksRenderer = new ClassBreaksRendererClass();
            pClassBreaksRenderer.Field = fieldName;
            pClassBreaksRenderer.BreakCount = classesCount;
            pClassBreaksRenderer.SortClassesAscending = true;
            // Use algorithmic color ramp to generate an range of colors between YELLOW to RED            
            // Initial color: YELLOW 
            pFromColor = new HsvColorClass();
            pFromColor.Hue = 60;
            pFromColor.Saturation = 100;
            pFromColor.Value = 96;
            // Final color: RED 
            pToColor = new HsvColorClass();
            pToColor.Hue = 0;
            pToColor.Saturation = 100;
            pToColor.Value = 96;
            // Set up HSV Color ramp to span from YELLOW to RED         
            pAlgorithmicColorRamp = new AlgorithmicColorRampClass();
            pAlgorithmicColorRamp.Algorithm = esriColorRampAlgorithm.esriHSVAlgorithm;
            pAlgorithmicColorRamp.FromColor = pFromColor;
            pAlgorithmicColorRamp.ToColor = pToColor;
            pAlgorithmicColorRamp.Size = classesCount;
            pAlgorithmicColorRamp.CreateRamp(out ok);
            pEnumColors = pAlgorithmicColorRamp.Colors;
            for (int index = 0; index < classesCount - 1; index++)
            {
                //int count = 0;
                pColor = pEnumColors.Next();
                pSimpleFillSymbol = new SimpleFillSymbolClass();
                pSimpleFillSymbol.Color = pColor;
                pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                pClassBreaksRenderer.set_Symbol(index, (ISymbol)pSimpleFillSymbol);
                pClassBreaksRenderer.set_Break(index, classes[index + 1]);
                // Store each break value for user output                
                strOutput += "-" + classes[index + 1] + "\n";
            }
            pGeoFeatureLayer.Renderer = (IFeatureRenderer)pClassBreaksRenderer;
            //this.axMapControl1.Refresh(); 
            /////////////////////////////////////////////////////////////////////////////////////////           
            //////////////////////////////////////////////////////////////////////////////////////////              
            //get the custom property from which is supposed to be the layer to be saved             
            object customProperty = null;
            //IMapControl3 mapControl = null; 
            customProperty = axMapControl1.CustomProperty;
            //ask the user to set a name for the new layer file          
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Layer File|*.lyr|All Files|*.*";
            saveFileDialog.Title = "生成专题图";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = System.IO.Path.Combine(saveFileDialog.InitialDirectory, pGeoFeatureLayer.Name + ".lyr");
            //get the layer name from the user 
            DialogResult dr = saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "" && dr == DialogResult.OK)
            {
                if (System.IO.File.Exists(saveFileDialog.FileName))
                {
                    //try to delete the existing file 
                    System.IO.File.Delete(saveFileDialog.FileName);
                }
                //create a new LayerFile instance 
                ILayerFile layerFile = new LayerFileClass();
                //create a new layer file 
                layerFile.New(saveFileDialog.FileName);
                //attach the layer file with the actual layer 
                layerFile.ReplaceContents((ILayer)pGeoFeatureLayer);
                //save the layer file                
                layerFile.Save();
                //ask the user whether he'd like to add the layer to the map 
                if (DialogResult.Yes == MessageBox.Show("Would you like to add the layer to the map?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    axMapControl1.AddLayerFromFile(saveFileDialog.FileName, 0);
                }
            }
        }
        List<IFeature> pFeatures = new List<IFeature>();
        private void 要素距离查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            mouseflag = 8;
            mousestation = true;
        }

        private void iAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {

            mousestation = true;
            mouseflag = 9;
        }

        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IHookHelper layout_hookHelper = new HookHelperClass();
            layout_hookHelper.Hook = this.axPageLayoutControl1.Object;
            axPageLayoutControl1.PageLayout = layout_hookHelper.PageLayout;
            Print PPLFrm = new Print(layout_hookHelper);
            PPLFrm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.axMapControl1.Map.ClearLayers();
            try
            {
                string Path = @"C:\Program Files (x86)\ArcGIS\DeveloperKit10.0\Samples\data\SanFrancisco\SanFrancisco.gdb";
                IWorkspaceFactory pWorkspaceFactory;
                IFeatureWorkspace pFeatureWorkspace;
                IFeatureLayer pFeatureLayer = new FeatureLayer();
                // IFeatureDataset pFeatureDataset;
                pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                //打开工作空间并遍历数据集 
                // IWorkspace pWorkspace = 
                pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(Path, 0) as IFeatureWorkspace;
                IFeatureClass pf = pFeatureWorkspace.OpenFeatureClass("Parks");
                pFeatureLayer.FeatureClass = pf;
                pFeatureLayer.Name = "Parks";
                axMapControl1.Map.AddLayer(pFeatureLayer as ILayer);
                axMapControl1.ActiveView.Refresh();
                /* IEnumDataset pEnumDataset = pWorkspace.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTAny);
                 pEnumDataset.Reset();
                 IDataset pDataset;
                 while (pEnumDataset != null)
                 {
                     pDataset = pEnumDataset.Next();
                     //到达数据集末尾跳出循环
                     if( pDataset==null)return;
                     //如果数据集是IFeatureDataset,则遍历它下面的子类
                     if (pDataset is IFeatureDataset)
                     {
                         pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(Path, 0);
                         pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset(pDataset.Name);
                         IEnumDataset pEnumDataset1 = pFeatureDataset.Subsets;
                         pEnumDataset1.Reset();
                         IDataset pDataset1 = pEnumDataset1.Next();
                         //如果子类是FeatureClass，则遍历之   
                         while (pDataset1 != null)
                         {
                             if (pDataset1 is IFeatureClass)
                             {
                                 pFeatureLayer = new FeatureLayerClass();
                                 pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(pDataset1.Name);
                                 pFeatureLayer.Name = pFeatureLayer.FeatureClass.AliasName;
                                 //找到目标图层添加后终止循环
                                 if (pFeatureLayer.Name == "Highways")
                                 {
                                     axMapControl1.Map.AddLayer(pFeatureLayer);
                                     axMapControl1.ActiveView.Refresh();
                                     break;
                                 }
                              
                             }
                             else
                             {
                                // MessageBox.Show("无要素图层");
                                 //不是要素图层则退出
                                 break;
                             }
                             pDataset1 = pEnumDataset1.Next();
                         }
                     }
                     else
                     {
                         continue;
                     }
                 }

            }*/
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.ToString());
            }
        }
        public static List<ILayer> ReadLayerFromGDB(List<string> filePathList)
        {
            List<ILayer> layerList = new List<ILayer>();

            if (filePathList.Count == 0) return null;
            else
            {
                foreach (string path in filePathList)
                {
                    IWorkspaceFactory pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                    IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(path, 0);
                    IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;

                    IEnumDataset pEnumDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass) as IEnumDataset;
                    pEnumDataset.Reset();
                    IDataset pDataset = pEnumDataset.Next();
                    while (pDataset is IFeatureClass)
                    {
                        IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                        pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(pDataset.Name);
                        pFeatureLayer.Name = pDataset.Name;
                        ILayer pLayer = pFeatureLayer as ILayer;
                        layerList.Add(pFeatureLayer as ILayer);
                        pDataset = pEnumDataset.Next();
                    }
                }
                return layerList;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string FromPath = @"C:\Program Files (x86)\ArcGIS\DeveloperKit10.0\Samples\data\SanFrancisco\SanFrancisco.gdb";
                string ToPath = @"E:\Highways";
                IWorkspaceFactory IINwsc = new FileGDBWorkspaceFactoryClass();
                IFeatureWorkspace Ifw;
                Ifw = IINwsc.OpenFromFile(FromPath, 0) as IFeatureWorkspace;
                IFeatureClass IfClass = Ifw.OpenFeatureClass("Highways");
                IWorkspaceFactory Outwsc = new ShapefileWorkspaceFactoryClass();
                IWorkspace ExpertSwc = Outwsc.OpenFromFile(ToPath, 0) as IWorkspace;
                if (ConvertFeatureClassToShapeFile())
                {
                    MessageBox.Show("Exported succesfully!");
                }
                else
                {
                    MessageBox.Show("Exported failed!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exporting failed, Because of" + ex.ToString(), "提示");
            }

        }
        private bool ExportToShapefile(IFeatureClass fc, IWorkspace outWorkspace)
        {
            if (fc == null || outWorkspace == null) return false;

            IDataset inDataSet = fc as IDataset;
            IFeatureClassName inFCName = inDataSet.FullName as IFeatureClassName;
            IWorkspace inWorkspace = inDataSet.Workspace;

            IDataset outDataSet = outWorkspace as IDataset;
            IWorkspaceName outWorkspaceName = outDataSet.FullName as IWorkspaceName;

            IFeatureClassName outFCName = new FeatureClassNameClass();
            IDatasetName dataSetName = outFCName as IDatasetName;
            dataSetName.WorkspaceName = outWorkspaceName;
            dataSetName.Name = fc.AliasName.ToString();

            IFieldChecker fieldChecker = new FieldCheckerClass();
            fieldChecker.InputWorkspace = inWorkspace;
            fieldChecker.ValidateWorkspace = outWorkspace;

            IFields fields = fc.Fields;
            IFields outFields = null;
            IEnumFieldError enumFieldError = null;
            fieldChecker.Validate(fields, out enumFieldError, out outFields);

            IFeatureDataConverter featureDataConverter = new FeatureDataConverterClass();
            featureDataConverter.ConvertFeatureClass(inFCName, null, null, outFCName, null, outFields, "", 100, 0);


            return true;
        }
        public bool ConvertFeatureClassToShapeFile()
        {
            try
            {
                string FromPath = @"C:\Program Files (x86)\ArcGIS\DeveloperKit10.0\Samples\data\SanFrancisco\SanFrancisco.gdb";
                string ToPath = @"E:\Highways";
                IWorkspaceFactory WorkFactory = new FileGDBWorkspaceFactoryClass();
                IWorkspaceName sourceWorkspaceName = new WorkspaceNameClass()
                {
                    WorkspaceFactoryProgID = "esriDataSourcesFile.FileGDBWorkspaceFactory",
                    PathName = FromPath
                };

                IName sourceWorkspaceIName = (IName)sourceWorkspaceName;
                IWorkspace sourceWorkspace = (IWorkspace)sourceWorkspaceIName.Open();
                //////////////////////////////////////////////////////////////
                IWorkspaceName targetWorkspaceName = new WorkspaceNameClass()
                {
                    WorkspaceFactoryProgID = "esriDataSourcesGDB.ShapefileWorkspaceFactory",
                    PathName = ToPath
                };
                IName targetWorkspaceIName = (IName)targetWorkspaceName;
                IWorkspace targetWorkspace = (IWorkspace)targetWorkspaceIName.Open();
                ////////////////////////////////////////////////////////////////
                IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
                IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
                sourceDatasetName.Name = "MajorRoads";
                sourceDatasetName.WorkspaceName = sourceWorkspaceName;

                IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
                IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
                targetDatasetName.Name = "MajorRoads";
                targetDatasetName.WorkspaceName = targetWorkspaceName;

                IName sourceName = (IName)sourceFeatureClassName;
                IFeatureClass sourceFeatureClass = (IFeatureClass)sourceName.Open();

                IFieldChecker fieldChecker = new FieldCheckerClass();
                IFields sourceFields = sourceFeatureClass.Fields;
                IFields targetFields = null;
                IEnumFieldError enumFieldError = null;

                // Set the required properties for the IFieldChecker interface.
                fieldChecker.InputWorkspace = sourceWorkspace;
                fieldChecker.ValidateWorkspace = targetWorkspace;
                fieldChecker.Validate(sourceFields, out enumFieldError, out targetFields);

                IFeatureDataConverter featureDataConverter = new FeatureDataConverterClass();
                featureDataConverter.ConvertFeatureClass(sourceFeatureClassName, null, null, targetFeatureClassName, null, targetFields, "", 100, 0);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }


        }
        public void ConvertShapefileToFeatureClass()
        {
            // Create a name object for the source (shapefile) workspace and open it.
            IWorkspaceName sourceWorkspaceName = new WorkspaceNameClass
            {
                WorkspaceFactoryProgID = "esriDataSourcesFile.ShapefileWorkspaceFactory",
                PathName = @"C:\Data\Shapefiles"
            };
            IName sourceWorkspaceIName = (IName)sourceWorkspaceName;
            IWorkspace sourceWorkspace = (IWorkspace)sourceWorkspaceIName.Open();

            // Create a name object for the target (file GDB) workspace and open it.
            IWorkspaceName targetWorkspaceName = new WorkspaceNameClass
            {
                WorkspaceFactoryProgID = "esriDataSourcesGDB.FileGDBWorkspaceFactory",
                PathName = @"C:\Data\Public.gdb"
            };
            IName targetWorkspaceIName = (IName)targetWorkspaceName;
            IWorkspace targetWorkspace = (IWorkspace)targetWorkspaceIName.Open();

            // Create a name object for the source dataset.
            IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
            sourceDatasetName.Name = "Can_Mjr_Cities";
            sourceDatasetName.WorkspaceName = sourceWorkspaceName;

            // Create a name object for the target dataset.
            IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
            targetDatasetName.Name = "Cities";
            targetDatasetName.WorkspaceName = targetWorkspaceName;

            // Open source feature class to get field definitions.
            IName sourceName = (IName)sourceFeatureClassName;
            IFeatureClass sourceFeatureClass = (IFeatureClass)sourceName.Open();

            // Create the objects and references necessary for field validation.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IFields sourceFields = sourceFeatureClass.Fields;
            IFields targetFields = null;
            IEnumFieldError enumFieldError = null;

            // Set the required properties for the IFieldChecker interface.
            fieldChecker.InputWorkspace = sourceWorkspace;
            fieldChecker.ValidateWorkspace = targetWorkspace;

            // Validate the fields and check for errors.
            fieldChecker.Validate(sourceFields, out enumFieldError, out targetFields);
            if (enumFieldError != null)
            {
                // Handle the errors in a way appropriate to your application.
                Console.WriteLine("Errors were encountered during field validation.");
            }

            // Find the shape field.
            String shapeFieldName = sourceFeatureClass.ShapeFieldName;
            int shapeFieldIndex = sourceFeatureClass.FindField(shapeFieldName);
            IField shapeField = sourceFields.get_Field(shapeFieldIndex);

            // Get the geometry definition from the shape field and clone it.
            IGeometryDef geometryDef = shapeField.GeometryDef;
            IClone geometryDefClone = (IClone)geometryDef;
            IClone targetGeometryDefClone = geometryDefClone.Clone();
            IGeometryDef targetGeometryDef = (IGeometryDef)targetGeometryDefClone;

            // Cast the IGeometryDef to the IGeometryDefEdit interface.
            IGeometryDefEdit targetGeometryDefEdit = (IGeometryDefEdit)targetGeometryDef;

            // Set the IGeometryDefEdit properties.
            targetGeometryDefEdit.GridCount_2 = 1;
            targetGeometryDefEdit.set_GridSize(0, 0.75);

            // Create a query filter to only select cities with a province (PROV) value of 'NS.'
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = "PROV = 'NS'";
            queryFilter.SubFields = "Shape, NAME, TERM, Pop1996";

            // Create the converter and run the conversion.
            IFeatureDataConverter featureDataConverter = new FeatureDataConverterClass();
            IEnumInvalidObject enumInvalidObject = featureDataConverter.ConvertFeatureClass
                (sourceFeatureClassName, queryFilter, null, targetFeatureClassName,
                targetGeometryDef, targetFields, "", 1000, 0);

            // Check for errors.
            IInvalidObjectInfo invalidObjectInfo = null;
            enumInvalidObject.Reset();
            while ((invalidObjectInfo = enumInvalidObject.Next()) != null)
            {
                // Handle the errors in a way appropriate to the application.
                Console.WriteLine("Errors occurred for the following feature: {0}",
                    invalidObjectInfo.InvalidObjectID);
            }
        }

        private void 连接SDEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                getFeatureClassFromSDE();

                IWorkspaceFactory workSpaceFactory = new ShapefileWorkspaceFactoryClass();
                IWorkspace targetWorkSpace = workSpaceFactory.OpenFromFile(@"E:\SDEExport", 0);
                if (ExportToShapefile(PFeatureclass, targetWorkSpace) &&
                 ExportToShapefile(LFeatureclass, targetWorkSpace))
                {
                    MessageBox.Show("OK");
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }

    
            
        }
        public void getFeatureClassFromSDE()
        {
            try
            {
                IWorkspaceFactory workFactory;
                IWorkspace sdeWorkspace;
                //连接属性设置
                IPropertySet propertySet = new PropertySetClass();
                propertySet.SetProperty("SERVER", "128.1.3.163");
                propertySet.SetProperty("INSTANCE", "5151/tcp");
                propertySet.SetProperty("USER", "sa");
                propertySet.SetProperty("PASSWORD", "p@ssw0rd");
                propertySet.SetProperty("VERSION", "sde.default");
                //得到SDE工作空间
                workFactory = new SdeWorkspaceFactoryClass();
                sdeWorkspace = workFactory.Open(propertySet, 0);
                IFeatureWorkspace Ifworkspace;
                IFeatureClass regionFeatureClass, lineFeatureClass, pointFeatureClass, Line2FeatureClass;
                // 工作空间转移给要素工作空间并得到要素类
                Ifworkspace = sdeWorkspace as IFeatureWorkspace;
                regionFeatureClass = Ifworkspace.OpenFeatureClass("sde.DBO.WaterFittings");
               // lineFeatureClass = Ifworkspace.OpenFeatureClass("sde.DBO.WaterHydrants");
               //pointFeatureClass = Ifworkspace.OpenFeatureClass("sde.DBO.WaterPipes");
               // Line2FeatureClass = Ifworkspace.OpenFeatureClass("sde.DBO.WaterAbandonedLines");
                //定义点线面并赋值
                RFeatureclass = regionFeatureClass;
             //   PFeatureclass = pointFeatureClass;
              //  LFeatureclass = lineFeatureClass;
              //  L2FeatureClass = Line2FeatureClass;
                //创建要素图层并显示
                //List<IFeatureLayer> featurelayer;
                IFeatureLayer p1Featurelayer = new FeatureLayerClass();
                IFeatureLayer p2Featurelayer = new FeatureLayerClass();
                IFeatureLayer p3Featurelayer = new FeatureLayerClass();
                IFeatureLayer p4Featurelayer = new FeatureLayerClass();

                p1Featurelayer.FeatureClass = regionFeatureClass;
                p1Featurelayer.Name = regionFeatureClass.AliasName;
              //  p2Featurelayer.FeatureClass = pointFeatureClass;
               // p2Featurelayer.Name = pointFeatureClass.AliasName;
               // p3Featurelayer.FeatureClass = lineFeatureClass;
              //  p3Featurelayer.Name = lineFeatureClass.AliasName;
              //  p4Featurelayer.FeatureClass = Line2FeatureClass;
              //  p4Featurelayer.Name = Line2FeatureClass.AliasName;
                this.axMapControl1.ClearLayers();
                this.axMapControl1.AddLayer(p2Featurelayer as ILayer);
                this.axMapControl1.AddLayer(p3Featurelayer as ILayer);
                this.axMapControl1.AddLayer(p1Featurelayer as ILayer);
                this.axMapControl1.AddLayer(p4Featurelayer as ILayer);
                this.axMapControl1.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        IFeatureClass RFeatureclass;
        IFeatureClass LFeatureclass;
        IFeatureClass PFeatureclass;
        IFeatureClass L2FeatureClass;
        private void 导出SHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IWorkspaceFactory workSpaceFactory = new ShapefileWorkspaceFactoryClass();
            IWorkspace targetWorkSpace = workSpaceFactory.OpenFromFile(@"E:\SDEExport", 0);
            if (ExportToShapefile(RFeatureclass, targetWorkSpace))
            {
                MessageBox.Show("Exported Successfully！");
            }
            else
            {
                MessageBox.Show("Exported Failed！");
            }
        }

        private void 点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFeature pFeaeture;
            pFeaeture = PFeatureclass.GetFeature(50);
            IPoint pPoint = new PointClass();
            pPoint = pFeaeture.ShapeCopy as IPoint;
            MessageBox.Show("X:" + pPoint.X + "Y:" + pPoint.X);
        }
        private void 线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFeature LFeaeture;
            LFeaeture = LFeatureclass.GetFeature(1);
            IPointCollection pCollet = LFeaeture.ShapeCopy as IPointCollection;
            getXY(pCollet);

        }
        public void getXY(IPointCollection pCollet)
        {
            try
            {
                int i = 0;
                Property form = new Property();
                form.dataGridView1.Columns.Add("X坐标值", null);
                form.dataGridView1.Columns.Add("Y坐标值", null);

                while (i < pCollet.PointCount)
                {
                    form.dataGridView1.Rows.Add(pCollet.get_Point(i).X.ToString(), pCollet.get_Point(i).X.ToString());
                    i++;
                }
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void 面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFeature RFeaeture;
            RFeaeture = RFeatureclass.GetFeature(1);
            IPointCollection pCollet = RFeaeture.ShapeCopy as IPointCollection;
            getXY(pCollet);
        }

        private void sHP文件转存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            try
            {
                string FromPath = @"E:\SDEExport";
                IWorkspaceFactory workSpace;
                IWorkspace shpWorkSpace;
                workSpace = new ShapefileWorkspaceFactoryClass();
                shpWorkSpace = workSpace.OpenFromFile(FromPath, 0);
                IFeatureWorkspace featureWorkspace = shpWorkSpace as IFeatureWorkspace;
                IFeatureClass SourceFeatureClass = featureWorkspace.OpenFeatureClass("一级公路");
            
                //获取源要素集每一个要素，并插入目的数据库中
                /*IFeatureLayer oldFeaturelayer = new FeatureLayerClass();
                oldFeaturelayer.FeatureClass = 1tarGetFeaturClass;
                oldFeaturelayer.Name = "旧图层";
                this.axMapControl1.AddLayer(oldFeaturelayer as ILayer);*/
                for (int i = 0; i < SourceFeatureClass.FeatureCount(null); i++)
                {
                   
                    IFeature pFeature = SourceFeatureClass.GetFeature(i);
                    if (pFeature.Shape is Polygon)
                    {
                        IPolygon pp = new PolygonClass();
                        pp = pFeature.Shape as IPolygon;
                      
                        MessageBox.Show("minX:"+pp.FromPoint.X.ToString()+"minY:"+pp.FromPoint.Y.ToString()
                           +"\n"+"maxX:"+pp.ToPoint.X.ToString()+"maxY:"+pp.ToPoint.Y.ToString()+"\n"+"length"+
                           pp.Length.ToString());
                    }
                    
                    featureclassadd(pFeature);
                }
                MessageBox.Show("Insert Successfully!");
                this.Cursor = System.Windows.Forms.Cursors.Default;
                IFeatureLayer newFeaturelayer = new FeatureLayer();
                newFeaturelayer.FeatureClass = tarGetFeaturClass;
                newFeaturelayer.Name = "新图层";
                this.axMapControl1.ClearLayers();
                this.axMapControl1.AddLayer(newFeaturelayer as ILayer);
                this.axMapControl1.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        IFeatureClass tarGetFeaturClass;
        public void featureclassadd(IFeature SourceFeature)
        {
            IWorkspaceFactory workFactory;
            IWorkspace accessWorkspace;
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("DATABASE", @"C:\Users\Administrator\Desktop\test.mdb");
            //打开Sde工作空间
            workFactory = new AccessWorkspaceFactoryClass();
            accessWorkspace = workFactory.Open(propertySet, 0);
            IFeatureWorkspace featureWorkSpace = accessWorkspace as IFeatureWorkspace;
            //得到目的要素集 
            tarGetFeaturClass = featureWorkSpace.OpenFeatureClass("YJGL");
            IFeatureBuffer bufffeature = tarGetFeaturClass.CreateFeatureBuffer();
            bufffeature.Shape = SourceFeature.ShapeCopy;
            IFeatureCursor InsertCusor = tarGetFeaturClass.Insert(true);
            IField Fld = new FieldClass();
            IFields Flds = SourceFeature.Fields;
            for (int i = 0; i < Flds.FieldCount; i++)
            {
                //获取对应字段的索引
                Fld = Flds.get_Field(i);
                int index = bufffeature.Fields.FindField(Fld.Name);
                if ((index != -1 && Fld.Name == "Shape") || Fld.Name == "roadname")
                {
                    //对应字段赋值
                    bufffeature.set_Value(index, SourceFeature.get_Value(i));

                }
            }
            InsertCusor.InsertFeature(bufffeature);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(InsertCusor);

        }

        private void poingMovingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void addElement(IElement EleMent, IFeatureLayer pFlayer)
        {
            IGraphicsContainer Ighcs = this.axMapControl1.Map as IGraphicsContainer;
            Ighcs.AddElement(EleMent, 0);
            this.axMapControl1.Refresh();


        }

        private void 线ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mouseflag = 100;
            mousestation = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                IGraphicsContainer ipc = this.axMapControl1.ActiveView as IGraphicsContainer;
                IFeature pfeature = getfeature();
                IEnvelope iev = pfeature.Extent;
                IPoint pp = new PointClass();
                IElement ie = iev as IElement;
                ipc.AddElement(ie, 0);
                this.axMapControl1.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }



        }
        public IFeature getfeature()
        {
            IFeature pfeature;
            IFeatureLayer flayer = new FeatureLayerClass();
            flayer = this.axMapControl1.get_Layer(0) as IFeatureLayer;
            IQueryFilter qq = new QueryFilterClass();
            qq.WhereClause = "NAME='贵州省'";
            IFeatureCursor Fcusor = flayer.Search(qq, false) as IFeatureCursor;
            pfeature = Fcusor.NextFeature();
            if (pfeature != null) return pfeature;
            else return null;

        }
        IPoint ppp = new PointClass();
        public void movepoint()
        {


        }

        private void button6_Click(object sender, EventArgs e)
        {
            MoveAllElements(this.axPageLayoutControl1.ActiveView);
        }
        public void fangda() { 
     IActiveView activeView = this.axMapControl1.ActiveView as IActiveView;
            IEnvelope envelope = new EnvelopeClass();
           envelope=     this.axMapControl1.Extent;

            // Get the center point of the envelope.
            IPoint centerPoint = new PointClass();
            centerPoint.X = ((envelope.XMax - envelope.XMin) / 2) + envelope.XMin;
            centerPoint.Y = ((envelope.YMax - envelope.YMin) / 2) + envelope.YMin;

            //Resize the envelope width.
            envelope.Width = envelope.Width / 2;

            //Resize the envelope height.
            envelope.Height = envelope.Height / 2;

            //Move the envelope to the center point.
            envelope.CenterAt(centerPoint);

            //Set the envelope to view extent.
            activeView.Extent= envelope;
            activeView.Refresh();
    }
        public void CountPolygonGraphicElements(IPageLayout pageLayout)
        {
            //The page layout is a graphics container, so it can be enumerated.
            IGraphicsContainer graphicsContainer = (IGraphicsContainer)pageLayout;

            //Reset the container to start from the beginning.
            graphicsContainer.Reset();
            Int32 int32_Counter = 0;
            IElement element = graphicsContainer.Next();

            //Loop through the container until the end, denoted by a null value.
            while (element != null)
            {
                //Test the element to see if it's a certain type. This can be ITextElement, IPolygonElement, etc.
               // if (element is  IPolygonElement)
               // {
                    //If a match is found, increment the counter.
                    int32_Counter = int32_Counter + 1;
              //  }

                //Step to the next element in the container.
                element = graphicsContainer.Next();
            }

            //Display the results of the count.
            MessageBox.Show("The pagelayout contains " + int32_Counter +
                " Element(s)");
        }
        public void AddTextElement(IMap map)
        {
            IGraphicsContainer graphicsContainer = map as IGraphicsContainer;
            IElement element = new TextElementClass();
            ITextElement textElement = element as ITextElement;

            //Create a point as the shape of the element.
            IPoint point = new PointClass();
            point.X = 100;
            point.Y = 30;
            element.Geometry = point;
            textElement.Text = "Hello World";
            graphicsContainer.AddElement(element, 0);

            //Flag the new text to invalidate.
          //  IActiveView activeView = map as IActiveView;
           // activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            this.axMapControl1.Refresh();
        }
        public void MoveAllElements(IActiveView activeView)
        {
            IPageLayout pageLayout = new PageLayoutClass();

            if (activeView is IPageLayout)
            {
                pageLayout = activeView as IPageLayout;
                IGraphicsContainer graphicsContainer = pageLayout as IGraphicsContainer;

                //Loop through all the elements and move each 1 inch.
                graphicsContainer.Reset();
                ITransform2D transform2D = null;
                IElement element = graphicsContainer.Next();
                if (element != null)
                {
                    transform2D = element as ITransform2D;
                    transform2D.Move(1, 0);
                    element = graphicsContainer.Next();
                }
            }
            else
            {
                MessageBox.Show("This tool only works in pagelayout view.");
            }

            //Refresh only the page layout's graphics.
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void axToolbarControl1_OnMouseDown_1(object sender, IToolbarControlEvents_OnMouseDownEvent e)
        {

        }
    }
}                                             

