﻿using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.MagicLeap
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ManagedReferenceImage : IDisposable
    {
        public ManagedReferenceImage(XRReferenceImage referenceImage)
        {
            guid = referenceImage.guid;
            textureGuid = referenceImage.textureGuid;
            size = referenceImage.specifySize ? referenceImage.size : Vector2.zero;
            name = GCHandle.ToIntPtr(GCHandle.Alloc(referenceImage.name));
            texture = GCHandle.ToIntPtr(GCHandle.Alloc(referenceImage.texture));
        }

        public unsafe XRReferenceImage ToReferenceImage()
        {
            Vector2? maybeSize;
            if (size.x > 0)
            {
                maybeSize = size;
            }
            else
            {
                maybeSize = null;
            }

            return new XRReferenceImage(
                AsSerializedGuid(guid),
                AsSerializedGuid(textureGuid),
                maybeSize,
                ResolveGCHandle<string>(name),
                ResolveGCHandle<Texture2D>(texture));
        }

        public void Dispose()
        {
            GCHandle.FromIntPtr(texture).Free();
            texture = IntPtr.Zero;

            GCHandle.FromIntPtr(name).Free();
            name = IntPtr.Zero;
        }

        unsafe SerializableGuid AsSerializedGuid(Guid guid)
        {
            TrackableId trackableId;
            *(Guid*)&trackableId = guid;
            return new SerializableGuid(trackableId.subId1, trackableId.subId2);
        }

        static T ResolveGCHandle<T>(IntPtr ptr) where T : class => (ptr == IntPtr.Zero) ? null : GCHandle.FromIntPtr(ptr).Target as T;

        public Guid guid;
        public Guid textureGuid;
        public Vector2 size;

        // The editor remoting will throw exceptions if this is not set to ignore
        [NativeDisableUnsafePtrRestriction]
        public IntPtr name;
        [NativeDisableUnsafePtrRestriction]
        public IntPtr texture;
    }
}
