// https://github.com/lbugnion/mvvmlight/blob/master/GalaSoft.MvvmLight/GalaSoft.MvvmLight%20(PCL)/Messaging/Messenger.cs#L722

namespace GalaSoft.MvvmLight.Helpers
{
    internal struct WeakActionAndToken
    {
        public WeakAction Action { get; set; }

        public int Token { get; set; }
    }
}
