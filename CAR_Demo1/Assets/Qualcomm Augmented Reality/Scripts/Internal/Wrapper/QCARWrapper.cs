/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

/// <summary>
/// This class serves as a facade to the native QCARWrapper.
/// Native methods are exposed in the IQCARWrapper interface.
/// The 'Instance' singleton property will return either an implementation that provides native access,
/// or a null implementation for the non-pro unity version that does not support native plugins.
/// </summary>
public static class QCARWrapper
{
    private static IQCARWrapper sWrapper = null;

    public static IQCARWrapper Instance
    {
        get
        {
            if (sWrapper == null)
            {
                Create();
            }

            return sWrapper;
        }
    }

    public static void Create()
    {
        if (QCARRuntimeUtilities.IsQCAREnabled())
            sWrapper = new QCARNativeWrapper();
        else
            sWrapper = new QCARNullWrapper();
    }

    /// <summary>
    /// Forces the singleton to a specific interface implementation
    /// Mainly used for Unit Tests.
    /// </summary>
    public static void SetImplementation(IQCARWrapper implementation)
    {
        sWrapper = implementation;
    }
}