/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/


public class MultiTargetImpl : TrackableImpl, MultiTarget
{
    public MultiTargetImpl(string name, int id)
        : base(name, id)
    {
        Type = TrackableType.MULTI_TARGET;
    }
}