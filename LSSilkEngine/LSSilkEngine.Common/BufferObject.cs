using Silk.NET.OpenGL;
using System;

namespace LSSilkEngine.Common
{
    //buffer object abstraction.
    public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        private uint _handle;
        private BufferTargetARB _bufferType;
        private GL _gl;

        public unsafe BufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType)
        {
            //Setting the gl instance and storing our buffer type.
            _gl = gl;
            _bufferType = bufferType;

            //Getting the handle, and then uploading the data to said handle.
            _handle = _gl.GenBuffer();
            Bind();
            fixed (void* d = data)
            {
                _gl.BufferData(bufferType, (nuint)(data.Length * sizeof(TDataType)), d, BufferUsageARB.StaticDraw);
            }
        }

        public void Bind()
        {
            //Binding the buffer object, with the correct buffer type.
            _gl.BindBuffer(_bufferType, _handle);
        }

        public void Dispose()
        {
            //Remember to delete our buffer.
            _gl.DeleteBuffer(_handle);
        }
    }
}