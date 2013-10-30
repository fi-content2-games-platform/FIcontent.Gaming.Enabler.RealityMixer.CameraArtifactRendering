/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System.Runtime.InteropServices;

/// <summary>
/// The supported trackable types.
/// </summary>
public enum TrackableType
{
    UNKNOWN_TYPE,       ///< A trackable of unknown type
    IMAGE_TARGET,       ///< A trackable of ImageTarget type
    MULTI_TARGET,       ///< A trackable of MultiTarget type
    CYLINDER_TARGET,    ///< A trackable of Cylinder type
    MARKER,             ///< A trackable of Marker type
    WORD,               ///< A trackable of Word type
}

/// <summary>
/// the most basic data of a trackabe
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SimpleTargetData
{
    public int id;
}

/// <summary>
/// The base class behaviour for all trackable types in Vuforia.
/// </summary>
public interface Trackable
{
    /// <summary>
    /// The type of the Trackable
    /// </summary>
    TrackableType Type { get; }

    /// <summary>
    /// The name of the Trackable
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The runtime Id of the Trackable
    /// </summary>
    int ID { get; }
}