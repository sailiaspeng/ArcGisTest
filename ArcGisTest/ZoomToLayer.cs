// Copyright 2010 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at &lt;your ArcGIS install location&gt;/DeveloperKit10.0/userestrictions.txt.
// 

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace ArcGisTest
{
    public sealed class ZoomToLayer : BaseCommand
    {
        private IMapControl3 m_mapControl;

        public ZoomToLayer()
        {
            base.m_caption = "放大到图层";
        }

        public override void OnClick()
        {
            ILayer layer = (ILayer)m_mapControl.CustomProperty;
            m_mapControl.Extent = layer.AreaOfInterest;
        }

        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
        }
    }
}

