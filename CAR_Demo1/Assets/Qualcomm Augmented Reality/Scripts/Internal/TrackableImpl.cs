/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/


public abstract class TrackableImpl : Trackable
{

    protected TrackableImpl(string name, int id)
    {
        Name = name;
        ID = id;
    }

    public TrackableType Type 
    { get; protected set; }

    public string Name
    { get; protected set; }

    public int ID
    { get; protected set; }
}