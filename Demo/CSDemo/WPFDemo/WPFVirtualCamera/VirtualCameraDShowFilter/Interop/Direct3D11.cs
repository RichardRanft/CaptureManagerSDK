﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Interop
{
    [ComVisible(false)]
    internal sealed class Direct3D11 : IDisposable
    {
        private const uint D3D_DRIVER_TYPE_HARDWARE = 1;

        private const uint D3D11_CREATE_DEVICE_SINGLETHREADED = 0x1;

        private const uint D3D11_CREATE_DEVICE_VIDEO_SUPPORT = 0x800;

        private const uint D3D11_CREATE_DEVICE_BGRA_SUPPORT = 0x20;

        private const int D3D_FEATURE_LEVEL_11_0 = 0xb000;

        private const uint D3D11_SDK_VERSION = 7;

        private ComInterface.ID3D11Device comObject;
        private IntPtr native = IntPtr.Zero;
        private ComInterface.CreateTexture2D        createTexture2D;
        private ComInterface.GetImmediateContext    getImmediateContext;
        private ComInterface.OpenSharedResource     openSharedResource;


        public IntPtr Native { get { return native; } }

        private Direct3D11(ComInterface.ID3D11Device a_object)
        {
            comObject = a_object;
            ComInterface.GetComMethod(this.comObject, 5, out this.createTexture2D);
            ComInterface.GetComMethod(this.comObject, 28, out this.openSharedResource);
            ComInterface.GetComMethod(this.comObject, 40, out this.getImmediateContext);
            native = Marshal.GetIUnknownForObject(comObject);
        }

        public void Dispose()
        {
            this.Release();
            GC.SuppressFinalize(this);
        }

        private void Release()
        {
            if (this.comObject != null)
            {
                Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
                this.createTexture2D = null;
            }
        }

        public static Direct3D11 Create()
        {
            ComInterface.ID3D11Device obj;

            uint l_Flags = D3D11_CREATE_DEVICE_VIDEO_SUPPORT | D3D11_CREATE_DEVICE_BGRA_SUPPORT | D3D11_CREATE_DEVICE_SINGLETHREADED;

            IntPtr l_featureLevels = Marshal.AllocHGlobal(4);

            Marshal.WriteInt32(l_featureLevels, D3D_FEATURE_LEVEL_11_0);

            IntPtr l_NULL = IntPtr.Zero;

            var l_result = NativeMethods.D3D11CreateDevice(IntPtr.Zero, D3D_DRIVER_TYPE_HARDWARE, IntPtr.Zero, l_Flags,
                l_featureLevels, 1, D3D11_SDK_VERSION, out obj, l_NULL, out l_NULL);

            Marshal.FreeHGlobal(l_featureLevels);
            
            return new Direct3D11(obj);
        }

        public D3D11Texture2D CreateTexture2D(NativeStructs.D3D11_TEXTURE2D_DESC a_Desc)
        {
            ComInterface.ID3D11Texture2D l_obj = null;

            int l_result = createTexture2D(this.comObject, a_Desc, IntPtr.Zero, out l_obj);

            return new D3D11Texture2D(l_obj);
        }

        public D3D11Texture2D CreateTexture2D(IntPtr a_sharedHandler)
        {
            IntPtr l_refUnk;

            var l_guid = typeof(ComInterface.ID3D11Texture2D).GUID;

            int l_result = openSharedResource(this.comObject, a_sharedHandler, ref l_guid, out l_refUnk);

            var l_obj = (ComInterface.ID3D11Texture2D)Marshal.GetObjectForIUnknown(l_refUnk);

            Marshal.Release(l_refUnk);

            Marshal.Release(l_refUnk);

            return new D3D11Texture2D(l_obj);
        }

        public D3D11DeviceContext GetImmediateContext()
        {
            ComInterface.ID3D11DeviceContext l_obj = null;

            getImmediateContext(this.comObject, out l_obj);

            return new D3D11DeviceContext(l_obj);
        }
    }
}
