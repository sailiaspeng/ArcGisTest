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
using ESRI.ArcGIS.SystemUI;

namespace ArcGisTest
{
    public sealed class LayerSelectable : BaseCommand, ICommandSubType
    {
        private IMapControl3 m_mapControl;
        private long m_subType;

        public LayerSelectable()
        {
        }

        public override void OnClick()
        {
            IFeatureLayer layer = (IFeatureLayer)m_mapControl.CustomProperty;
            if (m_subType == 1) layer.Selectable = true;
            if (m_subType == 2) layer.Selectable = false;
        }

        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
        }

        public override bool Enabled
        {
            get
            {
                ILayer layer = (ILayer)m_mapControl.CustomProperty;
                if (layer is IFeatureLayer)
                {
                    IFeatureLayer featureLayer = (IFeatureLayer)layer;
                    if (m_subType == 1) return !featureLayer.Selectable;
                    else return featureLayer.Selectable;
                }
                else
                {
                    return false;
                }
            }
        }

        public int GetCount()
        {
            return 2;
        }

        public void SetSubType(int SubType)
        {
            m_subType = SubType;
        }

        public override string Caption
        {
            get
            {
                if (m_subType == 1) return "图层可用";
                else return "图层不可用";
            }
        }
    }
}

