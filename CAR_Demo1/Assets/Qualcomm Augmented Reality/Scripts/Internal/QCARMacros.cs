/*==============================================================================
Copyright (c) 2012-2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/


// Helper structure for defining macros.
struct QCARMacros
{
    #region PUBLIC_MEMBER_VARIABLES

#if UNITY_IPHONE && !UNITY_EDITOR
    public const string PLATFORM_DLL = "__Internal";
#else
    public const string PLATFORM_DLL = "QCARWrapper";
#endif

    #endregion // PUBLIC_MEMBER_VARIABLES
}
