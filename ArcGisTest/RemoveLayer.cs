using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;

namespace ArcGisTest
{
    public sealed class RemoveLayer : BaseCommand
    {
        private IMapControl3 m_mapControl;

        public RemoveLayer()
        {
            base.m_caption = "移除图层";
        }

        public override void OnClick()
        {
            ILayer layer = (ILayer)m_mapControl.CustomProperty;
            m_mapControl.Map.DeleteLayer(layer);
        }

        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
        }

    }
}
