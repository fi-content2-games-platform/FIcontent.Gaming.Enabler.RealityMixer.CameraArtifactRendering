/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Globalization;


/// <summary>
/// This class reads the *.dat-file of a target data set for retreiving cylinder parameters.
/// </summary>
public static class CylinderDatasetReader
{
    #region CONSTANTS

    private const string CONFIG_XML = "config.info";
    private const string CYLINDER_TARGET_ELEMENT = "CylinderTarget";
    private const string KEYFRAME_ELEMENT = "Keyframe";
    private const string NAME_ATTRIBUTE = "name";
    private const string SIDELENGTH_ATTRIBUTE = "sideLength";
    private const string TOP_DIAMETER_ATTRIBUTE = "topDiameter";
    private const string BOTTOM_DIAMETER_ATTRIBUTE = "bottomDiameter";


    #endregion



    #region PUBLIC_METHODS

    /// <summary>
    /// Read cylinder parameters for all specified targets.
    /// </summary>
    /// <param name="datFile">Path to dataset-file with extension .dat</param>
    /// <param name="targetData">Cylinder targets for which the corresponding parameters should be retreived from the file.</param>
    public static void Read(string datFile, ConfigData.CylinderTargetData[] targetData)
    {
        var ci = CultureInfo.InvariantCulture;

        using (var configReader = new XmlTextReader(UnzipFile(datFile, CONFIG_XML)))
        {
            var latestDataIdx = -1;
            var indices = new Dictionary<string, int>();
            for (var i = 0; i < targetData.Length; i++)
                indices[targetData[i].name] = i;

            var latestBottomImageFile = "";
            var latestTopImageFile = "";

            while (configReader.Read())
            {
                if (configReader.NodeType == XmlNodeType.Element)
                {
                    switch (configReader.Name)
                    {
                        case CYLINDER_TARGET_ELEMENT:

                            var objname = configReader.GetAttribute(NAME_ATTRIBUTE);
                            if (objname == null || !indices.ContainsKey(objname))
                            {
                                latestDataIdx = -1;
                                continue;
                            }
                            var idx = indices[objname];
                            var data = targetData[idx];

                            var sidelengthAttr = configReader.GetAttribute(SIDELENGTH_ATTRIBUTE);
                            var topDiameterAttr = configReader.GetAttribute(TOP_DIAMETER_ATTRIBUTE);
                            var bottomDiameterAttr = configReader.GetAttribute(BOTTOM_DIAMETER_ATTRIBUTE);
                            if (sidelengthAttr == null || topDiameterAttr == null || bottomDiameterAttr == null)
                                continue;

                            var sideLength = float.Parse(sidelengthAttr, ci);
                            var scale = 1.0f;
                            if (data.sideLength > 0)
                                scale = data.sideLength/sideLength;
                            else
                                data.sideLength = sideLength;
                            data.topDiameter = scale*float.Parse(topDiameterAttr, ci);
                            data.bottomDiameter = scale*float.Parse(bottomDiameterAttr, ci);
                           

                            latestDataIdx = idx;
                            targetData[idx] = data;

                            latestBottomImageFile = GetBottomImageFile(objname);
                            latestTopImageFile = GetTopImageFile(objname);

                            break;
                        case KEYFRAME_ELEMENT:
                            if (latestDataIdx >= 0)
                            {
                                var ldata = targetData[latestDataIdx];
                                var imgName = configReader.GetAttribute(NAME_ATTRIBUTE);

                                if (imgName == latestTopImageFile)
                                {
                                    ldata.hasTopGeometry = true;
                                }
                                else if (imgName == latestBottomImageFile)
                                {
                                    ldata.hasBottomGeometry = true;
                                }
                                targetData[latestDataIdx] = ldata;
                            }
                            break;
                    }
                }
            }
        }
    }

    #endregion



    #region PRIVATE_METHODS


#if !EXCLUDE_JAVASCRIPT
    private static Stream UnzipFile(string path, string fileNameinZip)
    {
        return Unzip.Unzip(path, fileNameinZip);
    }
#else
    private static Stream UnzipFile(string path, string fileNameInZip)
    {
        return null;
    }
#endif

    private static string GetBottomImageFile(string name)
    {
        return name + ".Bottom";
    }
    private static string GetTopImageFile(string name)
    {
        return name + ".Top";
    }

    #endregion
}

