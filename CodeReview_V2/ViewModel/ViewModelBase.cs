using System;
using System.Diagnostics;
using System.ComponentModel;

namespace CodeReview_V2.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        protected ViewModelBase()
        { }
        #endregion //Constructor

        #region DisplayWindowHeader
        public virtual string DisplayName { get; protected set; }
        #endregion //DisplayWindowHeader

        #region Debugging
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name = " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }
        #endregion //Debugging

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion //INotifyPropertyChanged


        #region IDisposible Members
        public void Dispose()
        {
            this.OnDispose();
        }

        protected virtual void OnDispose()
        {}

#if DEBUG
        ~ViewModelBase()
        {
            string msg = string.Format("{0} {1} {2} Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif
        #endregion //IDisposible members

    }
}
