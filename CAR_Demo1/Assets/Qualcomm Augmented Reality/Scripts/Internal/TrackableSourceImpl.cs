/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using System;

public class TrackableSourceImpl : TrackableSource
{
    #region PROPERTIES

    public IntPtr TrackableSourcePtr 
    { get; private set; }

    #endregion



    #region CONSTRUCTION

    public TrackableSourceImpl(IntPtr trackableSourcePtr)
    {
        TrackableSourcePtr = trackableSourcePtr;
    }

    #endregion // CONSTRUCTION
}