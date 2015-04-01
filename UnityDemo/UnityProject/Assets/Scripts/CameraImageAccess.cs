using UnityEngine;
using System.Collections;


public class CameraImageAccess : MonoBehaviour, ITrackerEventHandler
{
    private Image.PIXEL_FORMAT m_PixelFormat = Image.PIXEL_FORMAT.GRAYSCALE;
    private bool m_RegisteredFormat = false;
    private bool m_LogInfo = true;
    private string message = "message";

    void Start()
    {
        QCARBehaviour qcarBehaviour = this.GetComponent<QCARBehaviour>();
        if (qcarBehaviour)
        {
            qcarBehaviour.RegisterTrackerEventHandler(this);
        }
        else
            throw new MissingComponentException();
    }

    public void OnInitialized()
    {
   
    }

    public void OnTrackablesUpdated()
    {
        if (!m_RegisteredFormat)
        {
            CameraDevice.Instance.SetFrameFormat(m_PixelFormat, true);
            m_RegisteredFormat = true;
        }
        if (m_LogInfo)
        {
            CameraDevice cam = CameraDevice.Instance;
            Image image = cam.GetCameraImage(m_PixelFormat);
            if (image == null)
            {
                Debug.Log(m_PixelFormat + " image is not available yet");
            }
            else
            {
                message = "This demo is configured for a 16:9 screen and a camera image with an aspect ratio of 1.33 (4:3). Your camera has a resolution of " + image.Width + "x" + image.Height + " pixels, aspect ratio " + (Mathf.Round((float)image.Width / image.Height * 100) / 100);
                string s = m_PixelFormat + " image: \n";
                s += "  size: " + image.Width + "x" + image.Height + "\n";
                s += "  bufferSize: " + image.BufferWidth + "x" + image.BufferHeight + "\n";
                s += "  stride: " + image.Stride;
                Debug.Log(s);
                m_LogInfo = false;
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 600, 200), message);
    }
}