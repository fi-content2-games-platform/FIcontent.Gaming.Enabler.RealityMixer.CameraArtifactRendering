/*==============================================================================
Copyright (c) 2010-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

/// <summary>
/// An interface for handling virtual button state changes.
/// </summary>
public interface IVirtualButtonEventHandler
{
    /// <summary>
    /// Called when the virtual button has just been pressed.
    /// </summary>
    void OnButtonPressed(VirtualButtonBehaviour vb);

    /// <summary>
    /// Called when the virtual button has just been released.
    /// </summary>
    void OnButtonReleased(VirtualButtonBehaviour vb);
}
