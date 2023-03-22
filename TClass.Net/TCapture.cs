using System.Drawing;

namespace TClass.Net
{
    public class TCapture
    {
        public class Capture
        {
            private OpenCvSharp.VideoCapture? _videoCapture;

            public delegate void VideoCaptureError(string messages);
            public event VideoCaptureError? OnError;

            public delegate void VideoFrameHeader(Bitmap bitmap);
            public event VideoFrameHeader? OnFrameHeader;

            public delegate void VideoCaptureStop();
            public event VideoCaptureStop? OnVideoStop;

            public delegate void VideoCaptureStarted();
            public event VideoCaptureStarted? OnVideoStarted;

            private bool _onStarted = false;

            public bool _isRunning { get; set; }

            private int _frameRate = 50;

            public int width { get; set; } = 1280;
            public int height { get; set; } = 720;

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
            private System.Threading.Timer? _timer;
            public Capture()
            {
                width  = 1280;
                height = 720;
            }
            private void FrameCapture(object state)
            {
                try
                {
                    if (!_videoCapture.IsOpened())
                    {
                        return;
                    }
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
                        using (Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame))
                        {
                            OnFrameHeader?.Invoke(bitmap);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(ex.Message);
                }
            }
            public void Start(int device)
            {
                if (_videoCapture != null)
                {
                    _videoCapture.Dispose();
                }

                _videoCapture = new OpenCvSharp.VideoCapture(device);
                _videoCapture.Open(device);
                setFrame(width, height);
                _isRunning = true;
                _onStarted = true;

                if (_timer != null)
                {
                    _timer.Dispose();
                }
                #pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
                _timer = new System.Threading.Timer(callback: FrameCapture, state: null, dueTime: 0, period: _frameRate);
                #pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
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