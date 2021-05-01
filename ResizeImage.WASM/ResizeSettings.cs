using System.Collections.Generic;
using System.ComponentModel;

namespace ResizeImage.WASM
{
    public class ResizeSettings : INotifyPropertyChanged
    {
        public ResizeSettings()
        {
            _Values = new Dictionary<string, object>();
            _PreferredSizeWidth = 515;
            _PreferredSizeHeight = 450;

            _ShowSuccessMessage = false;
            _EnabledOpenSingleFileAfterResize = false;
            _ClearImageListAfterSuccess = true;
            _ImageQuality = 75;
            _KeepAspectRatio = false;
            _HeightPercent = 100;
            _WidthPercent = 100;
            _HeightCustom = 768;
            _WidthCustom = 1024;
            _OverwriteFiles = false;
            _RadioOptions = 4;
            _CheckGlobalWriteAccess = true;

            _SizeOptionOneWidth = 640;
            _SizeOptionOneHeight = 480;

            _SizeOptionTwoWidth = 800;
            _SizeOptionTwoHeight = 600;
            _UseBottomAppBar = false;
            _SaveFilesForSaveAsInSameFolder = false;

        }

        private Dictionary<string, object> _Values;
        public Dictionary<string, object> Values
        {
            get { return _Values; }
            set { SetProperty(ref _Values, value, nameof(Values)); }
        }

        private string _DefaultSaveAsTargetFolder;
        public string DefaultSaveAsTargetFolder
        {
            get { return _DefaultSaveAsTargetFolder; }
            set { SetProperty(ref _DefaultSaveAsTargetFolder, value, nameof(DefaultSaveAsTargetFolder)); }
        }

        private bool _CheckGlobalWriteAccess;
        public bool CheckGlobalWriteAccess
        {
            get { return _CheckGlobalWriteAccess; }
            set { SetProperty(ref _CheckGlobalWriteAccess, value, nameof(CheckGlobalWriteAccess)); }
        }

        private bool _SaveFilesForSaveAsInSameFolder;
        public bool SaveFilesForSaveAsInSameFolder
        {
            get { return _SaveFilesForSaveAsInSameFolder; }
            set { SetProperty(ref _SaveFilesForSaveAsInSameFolder, value, nameof(SaveFilesForSaveAsInSameFolder)); }
        }

        private int _RadioOptions;
        public int RadioOptions
        {
            get { return _RadioOptions; }
            set { SetProperty(ref _RadioOptions, value, nameof(RadioOptions)); }
        }


        private int _SizeOptionOneWidth;
        public int SizeOptionOneWidth
        {
            get { return _SizeOptionOneWidth; }
            set { SetProperty(ref _SizeOptionOneWidth, value, nameof(SizeOptionOneWidth)); }
        }


        private int _SizeOptionOneHeight;
        public int SizeOptionOneHeight
        {
            get { return _SizeOptionOneHeight; }
            set { SetProperty(ref _SizeOptionOneHeight, value, nameof(SizeOptionOneHeight)); }
        }


        private int _SizeOptionTwoWidth;
        public int SizeOptionTwoWidth
        {
            get { return _SizeOptionTwoWidth; }
            set { SetProperty(ref _SizeOptionTwoWidth, value, nameof(SizeOptionTwoWidth)); }
        }

        private int _SizeOptionTwoHeight;
        public int SizeOptionTwoHeight
        {
            get { return _SizeOptionTwoHeight; }
            set { SetProperty(ref _SizeOptionTwoHeight, value, nameof(SizeOptionTwoHeight)); }
        }


        private bool _OverwriteFiles;
        public bool OverwriteFiles
        {
            get { return _OverwriteFiles; }
            set { SetProperty(ref _OverwriteFiles, value, nameof(OverwriteFiles)); }
        }


        private int _WidthCustom;
        public int WidthCustom
        {
            get { return _WidthCustom; }
            set { SetProperty(ref _WidthCustom, value, nameof(WidthCustom)); }
        }


        private int _HeightCustom;
        public int HeightCustom
        {
            get { return _HeightCustom; }
            set { SetProperty(ref _HeightCustom, value, nameof(HeightCustom)); }
        }


        private int _WidthPercent;
        public int WidthPercent
        {
            get { return _WidthPercent; }
            set { SetProperty(ref _WidthPercent, value, nameof(WidthPercent)); }
        }


        private int _HeightPercent;
        public int HeightPercent
        {
            get { return _HeightPercent; }
            set { SetProperty(ref _HeightPercent, value, nameof(HeightPercent)); }
        }


        private bool _KeepAspectRatio;
        public bool KeepAspectRatio
        {
            get { return _KeepAspectRatio; }
            set { SetProperty(ref _KeepAspectRatio, value, nameof(KeepAspectRatio)); }
        }

        protected int _ImageQuality;
        public int ImageQuality
        {
            get { return _ImageQuality; }
            set { SetProperty(ref _ImageQuality, value, nameof(ImageQuality)); }
        }


        private bool _EnabledOpenSingleFileAfterResize;
        public bool EnabledOpenSingleFileAfterResize
        {
            get { return _EnabledOpenSingleFileAfterResize; }
            set { SetProperty(ref _EnabledOpenSingleFileAfterResize, value, nameof(EnabledOpenSingleFileAfterResize)); }
        }

        private bool _ClearImageListAfterSuccess;
        public bool ClearImageListAfterSuccess
        {
            get { return _ClearImageListAfterSuccess; }
            set { SetProperty(ref _ClearImageListAfterSuccess, value, nameof(ClearImageListAfterSuccess)); }
        }

        protected bool _ShowSuccessMessage;
        public bool ShowSuccessMessage
        {
            get { return _ShowSuccessMessage; }
            set { SetProperty(ref _ShowSuccessMessage, value, nameof(ShowSuccessMessage)); }
        }

        private double _PreferredSizeWidth;
        public double PreferredSizeWidth
        {
            get { return _PreferredSizeWidth; }
            set { SetProperty(ref _PreferredSizeWidth, value, nameof(PreferredSizeWidth)); }
        }


        private double _PreferredSizeHeight;
        public double PreferredSizeHeight
        {
            get { return _PreferredSizeHeight; }
            set { SetProperty(ref _PreferredSizeHeight, value, nameof(PreferredSizeHeight)); }
        }

        private bool _UseBottomAppBar;
        public bool UseBottomAppBar
        {
            get { return _UseBottomAppBar; }
            set { SetProperty(ref _UseBottomAppBar, value, nameof(UseBottomAppBar)); }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
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
    }
}