using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TClass
{
    public class TCapture
    {
        public class Capture
        {
            private Thread _thread;
            private OpenCvSharp.VideoCapture _videoCapture;

            public delegate void VideoCaptureError(string messages);
            public event VideoCaptureError OnError;

            public delegate void VideoFrameHeader(Bitmap bitmap);
            public event VideoFrameHeader OnFrameHeader;

            public delegate void VideoFrameHeaderMat(OpenCvSharp.Mat mat);
            public event VideoFrameHeaderMat OnFrameHeaderMat;

            public delegate void VideoCaptureStop();
            public event VideoCaptureStop OnVideoStop;

            public delegate void VideoCaptureStarted();
            public event VideoCaptureStarted OnVideoStarted;

            private bool _onStarted = false;

            public bool _isRunning { get; set; }

            private int _frameRate = 50;

            public int width { get; set; }
            public int height { get; set; }

            public void setSize(int width, int height)
            {
                this.width = width;
                this.height = height;
            }
            public int frameRate
            {
                get { return _frameRate; }
                set { _frameRate = 1000 / value; }
            }

            public bool IsOpened
            {
                get { return IsOpen(); }
            }
            public bool IsOpen()
            {
                if (_videoCapture != null && _videoCapture.IsOpened())
                {
                    return true;
                }
                return false;
            }

            public Capture()
            {
                width = 1280;
                height = 720;
            }
            private System.Threading.Timer _timer;

            public void Start(int device)
            {
                try
                {
                    if (_videoCapture != null)
                    {
                        _videoCapture.Dispose();
                    }

                    _videoCapture = new OpenCvSharp.VideoCapture(device);

                    if (!_videoCapture.Open(device))
                    {
                        OnError?.Invoke("Cannot open the video capture device.");
                        return;
                    }

                    setFrame(width, height);
                    _isRunning = true;
                    _onStarted = true;

                    if (_timer != null)
                    {
                        _timer.Dispose();
                    }
                    _timer = new System.Threading.Timer(FrameCapture, null, 0, _frameRate);
                }
                catch (Exception ex)
                {
                    OnError?.Invoke($"Error while starting video capture: {ex.Message}");
                }
            }

            private void FrameCapture(object state)
            {
                try
                {
                    if (_videoCapture.IsOpened())
                    {
                        if (_onStarted)
                        {
                            OnVideoStarted?.Invoke();
                            _onStarted = false;
                        }
                        using (OpenCvSharp.Mat frame = _videoCapture.RetrieveMat())
                        {
                            if (frame.Empty())
                            {
                                OnError?.Invoke("Frame is empty");
                            }
                            else
                            {
                                using (Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame))
                                {
                                    OnFrameHeader?.Invoke(bitmap);
                                }

                                OnFrameHeaderMat?.Invoke(frame);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(ex.Message);
                }
            }

            public void setFrame(int width, int height)
            {
                _videoCapture?.Set(OpenCvSharp.VideoCaptureProperties.FrameWidth, width);
                _videoCapture?.Set(OpenCvSharp.VideoCaptureProperties.FrameHeight, height);
            }

            public void Stop()
            {
                _isRunning = false;
                if (_videoCapture != null)
                {
                    _videoCapture.Release();
                }
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
                OnVideoStop?.Invoke();
            }

            public void Dispose()
            {
                _isRunning = false;
                if (_videoCapture != null)
                {
                    _videoCapture.Dispose();
                }
                if (_thread != null)
                {
                    _thread.Abort();
                }
            }

            // Get Focus
            public int GetFocus()
            {
                return (int)_videoCapture.Get(OpenCvSharp.VideoCaptureProperties.Focus);
            }
            // Set Focus 
            public void setFocus(int value)
            {
                _videoCapture.Set(OpenCvSharp.VideoCaptureProperties.Focus, value);
            }
            // Auto Focus
            public void AutoFocus()
            {
                _videoCapture.Set(OpenCvSharp.VideoCaptureProperties.Focus, -1);
            }

            // Get Zoom
            public int GetZoom()
            {
                return (int)_videoCapture.Get(OpenCvSharp.VideoCaptureProperties.Zoom);
            }

            // Set Zoom
            public void setZoom(int value)
            {
                _videoCapture.Set(OpenCvSharp.VideoCaptureProperties.Zoom, value);
            }
            // Get Exposure
            public int getExposure()
            {
                return (int)_videoCapture.Get(OpenCvSharp.VideoCaptureProperties.Exposure);
            }

            // Set Exposure
            public void setExposure(int value)
            {
                _videoCapture.Set(OpenCvSharp.VideoCaptureProperties.Exposure, value);
            }

            // Get Gain
            public int GetGain()
            {
                return (int)_videoCapture.Get(OpenCvSharp.VideoCaptureProperties.Gain);
            }

            // Set Gain
            public void setGain(int value)
            {
                _videoCapture.Set(OpenCvSharp.VideoCaptureProperties.Gain, value);
            }

            // Set Brightness
            public void setBrightness(int value)
            {
                _videoCapture.Set(OpenCvSharp.VideoCaptureProperties.Brightness, value);
            }

            // Set Contrast
            public void setContrast(int value)
            {
                _videoCapture.Set(OpenCvSharp.VideoCaptureProperties.Contrast, value);
            }

            // Set Saturation
            public void setSaturation(int value)
            {
                _videoCapture.Set(OpenCvSharp.VideoCaptureProperties.Saturation, value);
            }

        }
    }

}
