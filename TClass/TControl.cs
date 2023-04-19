using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace TClass
{
    public class TControl
    {

        public IAMCameraControl cameraControl { get; private set; }

        public int fmin, fmax, fstep, fdefaultValue, fValue;
        private CameraControlFlags fflags;

        public int zmin, zmax, zstep, zdefaultValue, zValue;
        private CameraControlFlags zflags;

        public int pmin, pmax, pstep, pdefaultValue, pValue;
        private CameraControlFlags pflags;

        public int tmin, tmax, tstet, tdefaultValue, tValue;
        private CameraControlFlags tflags;

        public int rmin, rmax, rstep, rdefaultValue, rValue;
        private CameraControlFlags rflags;

        public int emin, emax, estep, edefaultValue, eValue;
        private CameraControlFlags eflags;

        public delegate void onError(string message);
        public onError OnError;

        public TControl(int index = -1)
        {
            try
            {
                if (index != -1)
                {
                    cameraControl = GetCameraControl(index);
                    if (cameraControl != null)
                    {
                        // Get Range
                        cameraControl.GetRange(CameraControlProperty.Focus, out fmin, out fmax, out fstep, out fdefaultValue, out fflags);
                        cameraControl.GetRange(CameraControlProperty.Zoom, out zmin, out zmax, out zstep, out zdefaultValue, out zflags);
                        cameraControl.GetRange(CameraControlProperty.Pan, out pmin, out pmax, out pstep, out pdefaultValue, out pflags);
                        cameraControl.GetRange(CameraControlProperty.Tilt, out tmin, out tmax, out tstet, out tdefaultValue, out tflags);
                        cameraControl.GetRange(CameraControlProperty.Roll, out rmin, out rmax, out rstep, out rdefaultValue, out rflags);
                        cameraControl.GetRange(CameraControlProperty.Exposure, out emin, out emax, out estep, out edefaultValue, out eflags);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                OnError?.Invoke(ex.Message);
            }
        }

        public void set(int index)
        {
            try
            {
                cameraControl = GetCameraControl(index);
                if (cameraControl != null)
                {
                    // Get Range
                    cameraControl.GetRange(CameraControlProperty.Focus, out fmin, out fmax, out fstep, out fdefaultValue, out fflags);
                    cameraControl.GetRange(CameraControlProperty.Zoom, out zmin, out zmax, out zstep, out zdefaultValue, out zflags);
                    cameraControl.GetRange(CameraControlProperty.Pan, out pmin, out pmax, out pstep, out pdefaultValue, out pflags);
                    cameraControl.GetRange(CameraControlProperty.Tilt, out tmin, out tmax, out tstet, out tdefaultValue, out tflags);
                    cameraControl.GetRange(CameraControlProperty.Roll, out rmin, out rmax, out rstep, out rdefaultValue, out rflags);
                    cameraControl.GetRange(CameraControlProperty.Exposure, out emin, out emax, out estep, out edefaultValue, out eflags);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                OnError?.Invoke(ex.Message);
            }
        }

        public void setFocus()
        {
            fValue = 0;
            cameraControl.Set(CameraControlProperty.Focus, 0, CameraControlFlags.Auto);
        }
        public void setFocus(int value)
        {
            if (value >= fmin && value <= fmax)
            {
                cameraControl?.Set(CameraControlProperty.Focus, value, CameraControlFlags.Manual);
                fValue = value;
            }
        }

        public void setZoom()
        {
            zValue = 0;
            cameraControl?.Set(CameraControlProperty.Zoom, 0, CameraControlFlags.Manual);
        }
        public void setZoom(int value)
        {
            if (value >= zmin && value <= zmax)
            {
                zValue = value;
                cameraControl?.Set(CameraControlProperty.Zoom, value, CameraControlFlags.Manual);
            }
        }

        public void setPan()
        {
            pValue = 0;
            cameraControl.Set(CameraControlProperty.Pan, 0, CameraControlFlags.Manual);
        }

        public void setPan(int value)
        {
            if (value >= pmin && value <= pmax)
            {
                pValue = value;
                cameraControl?.Set(CameraControlProperty.Pan, value, CameraControlFlags.Manual);
            }
        }

        public void setTilt()
        {
            tValue = 0;
            cameraControl?.Set(CameraControlProperty.Tilt, 0, CameraControlFlags.Manual);
        }
        public void setTilt(int value)
        {
            if (value >= tmin && value <= tmax)
            {
                tValue = value;
                cameraControl?.Set(CameraControlProperty.Tilt, value, CameraControlFlags.Manual);
            }
        }

        public void setRoll()
        {
            rValue = 0;
            cameraControl?.Set(CameraControlProperty.Roll, 0, CameraControlFlags.Manual);
        }
        public void setRoll(int value)
        {
            if (value >= rmin && value <= rmax)
            {
                rValue = value;
                cameraControl?.Set(CameraControlProperty.Roll, value, CameraControlFlags.Manual);
            }
        }

        public void setExposure()
        {
            eValue = 0;
            cameraControl?.Set(CameraControlProperty.Exposure, 0, CameraControlFlags.Auto);
        }
        public void setExposure(int value)
        {
            if (value >= emin && value <= emax)
            {
                eValue = value;
                cameraControl?.Set(CameraControlProperty.Exposure, value, CameraControlFlags.Manual);
            }
        }

        private IAMCameraControl GetCameraControl(int cameraIndex)
        {
            IBaseFilter captureFilter = null;
            IEnumMoniker classEnum = null;
            object sourceObject;

            //Guid category = PinCategory.Capture; FilterCategory.VideoInputDevice
            Guid category = FilterCategory.VideoInputDevice;
            Guid type = MediaType.Video;
            int hr;

            ICreateDevEnum deviceEnum = (ICreateDevEnum)new CreateDevEnum();
            hr = deviceEnum.CreateClassEnumerator(category, out classEnum, 0);

            if (hr != 0)
                throw new ApplicationException("No devices found.");

            int currentIndex = 0;
            IMoniker[] moniker = new IMoniker[1];
            while (classEnum.Next(1, moniker, IntPtr.Zero) == 0)
            {
                if (currentIndex == cameraIndex)
                {
                    Guid iid = typeof(IBaseFilter).GUID;
                    moniker[0].BindToObject(null, null, ref iid, out sourceObject);
                    captureFilter = (IBaseFilter)sourceObject;
                    break;
                }
                currentIndex++;
            }

            IAMCameraControl cameraControl = null;
            if (captureFilter != null)
            {
                cameraControl = captureFilter as IAMCameraControl;
            }

            return cameraControl;
        }
    }
}
