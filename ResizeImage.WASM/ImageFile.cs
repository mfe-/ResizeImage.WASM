using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ResizeImage.WASM
{
    public class ImageFile : IDisposable, INotifyPropertyChanged
    {
        protected Func<ImageFile, Stream> _openStreamFunction;
        public ImageFile(string path, Stream stream)
        {
            Path = path;
            Stream = stream;
        }
        public ImageFile(string path, Stream stream, int width, int height) : this(path, stream)
        {
            Width = width;
            Height = height;
        }
        public ImageFile(string path, Stream stream, int width, int height, FileInfo fileInfo) :
            this(path, stream, width, height)
        {
            FileInfo = fileInfo;
        }
        public ImageFile(string path, int width, int height, FileInfo fileInfo, Func<ImageFile, Stream> funcStreamCallBack) :
            this(path, null, width, height)
        {
            FileInfo = fileInfo;
            _openStreamFunction = funcStreamCallBack;
        }

        public String Name
        {
            get { return FileInfo?.Name; }
        }

        //
        // Summary:
        //     Gets the full file-system path of the item, if the item has a path.
        //
        // Returns:
        //     The full path of the item, if the item has a path in the user's file-system.
        public string Path { get; set; }

        private Stream _Stream;

        public Stream Stream
        {
            get
            {
                if (_Stream == null && _openStreamFunction != null)
                {
                    Stream = _openStreamFunction(this);
                }
                return _Stream;
            }
            set
            {
                SetProperty(ref _Stream, value, nameof(Stream));
            }
        }

        protected int _Width;
        public int Width
        {
            get { return _Width; }
            set { SetProperty(ref _Width, value, nameof(Width)); }
        }

        protected int _Height;
        public int Height
        {
            get { return _Height; }
            set { SetProperty(ref _Height, value, nameof(Height)); }
        }

        protected int _NewWidth;
        public int NewWidth
        {
            get { return _NewWidth; }
            set { SetProperty(ref _NewWidth, value, nameof(NewWidth)); }
        }

        protected int _NewHeight;
        public int NewHeight
        {
            get { return _NewHeight; }
            set { SetProperty(ref _NewHeight, value, nameof(NewHeight)); }
        }

        public FileInfo FileInfo { get; set; }

        public object Tag { get; set; }

        /// <summary>
        /// Get or sets whether the image file could not be modified (an other proccess has opened the file already, or no write permissions)
        /// </summary>
        protected bool _IsReadOnly;
        public bool IsReadOnly
        {
            get { return _IsReadOnly; }
            set { SetProperty(ref _IsReadOnly, value, nameof(IsReadOnly)); }
        }

        public DateTimeOffset LastModified { get; set; }

        public long Size => Stream.Length;

        public string ContentType { get; set; }

        public string ToBase64ImageSrc()
        {
            return $"data:{ (object)ContentType};base64, { Convert.ToBase64String((Stream as MemoryStream)?.ToArray() ?? new byte[0])}";
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
            Stream?.Dispose();
        }
    }
}
