
namespace FCGP
{
    internal static class NativeMethods
    {
        #region win32 Souris
        /// <summary>pour envoyer une nouvelle position du curseur de souris à une application</summary>
        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        private static extern int SetCursorPos(int X, int Y);
        internal static int PositionnerCurseur(int X, int Y)
        {
            return SetCursorPos(X, Y);
        }
        /// <summary>pour envoyer un des boutons de souris appuyé ou relevé à une application. attention pas 'Mouse_event' sinon pas trouver dans user32.dll</summary>
        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, IntPtr dwExtraInfo);
        /// <summary>Uniquement pour encapsuler les appels des fonctions ou procédures API. Provoque l'appui du bouton gauche de la souris</summary>
        internal static void BGAppui()
        {
            mouse_event((int)MOUSEEVENTF.LEFTDOWN + (int)MOUSEEVENTF.ABSOLUTE, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>Uniquement pour encapsuler les appels des fonctions ou procédures API. Provoque le relachement du bouton gauche de la souris</summary>
        internal static void BGRelache()
        {
            mouse_event((int)MOUSEEVENTF.LEFTUP + (int)MOUSEEVENTF.ABSOLUTE, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>Uniquement pour encapsuler les appels des fonctions ou procédures API. Provoque le relachement du bouton gauche de la souris</summary>
        internal static void BGClick()
        {
            mouse_event((int)MOUSEEVENTF.LEFTDOWN + (int)MOUSEEVENTF.ABSOLUTE, 0, 0, 0, IntPtr.Zero);
            mouse_event((int)MOUSEEVENTF.LEFTUP + (int)MOUSEEVENTF.ABSOLUTE, 0, 0, 0, IntPtr.Zero);
        }
        // Constantes pour l'envoi des évenements souris
        private enum MOUSEEVENTF
        {
            ABSOLUTE = 0x8000,
            LEFTDOWN = 0x2,
            LEFTUP = 0x4,
            RIGHTDOWN = 0x8,
            RIGHTUP = 0x10
        }
        #endregion
        #region win32 Window
        [Flags()]
        internal enum WindowStylesEx : uint
        {
            WS_INCONNU = 0x800U,
            WS_EX_ACCEPTFILES = 0x10U,
            WS_EX_APPWINDOW = 0x40000U,
            WS_EX_CLIENTEDGE = 0x200U,
            WS_EX_COMPOSITED = 0x2000000U,
            WS_EX_CONTEXTHELP = 0x400U,
            WS_EX_CONTROLPARENT = 0x10000U,
            WS_EX_DLGMODALFRAME = 0x1U,
            WS_EX_LAYERED = 0x80000U,
            WS_EX_LAYOUTRTL = 0x400000U,
            WS_EX_LEFT = 0x0U,
            WS_EX_LEFTSCROLLBAR = 0x4000U,
            WS_EX_MDICHILD = 0x40U,
            WS_EX_NOACTIVATE = 0x8000000U,
            WS_EX_NOINHERITLAYOUT = 0x100000U,
            WS_EX_NOPARENTNOTIFY = 0x4U,
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
            WS_EX_RIGHT = 0x1000U,
            WS_EX_RTLREADING = 0x2000U,
            WS_EX_STATICEDGE = 0x20000U,
            WS_EX_TOOLWINDOW = 0x80U,
            WS_EX_TOPMOST = 0x8U,
            WS_EX_TRANSPARENT = 0x20U,
            WS_EX_WINDOWEDGE = 0x100U
        }
        /// <summary>Window Styles.The following styles can be specified wherever a window style is required. After the control has been created, these styles cannot be modified, except as noted.</summary>
        [Flags()]
        internal enum WindowStyles : uint
        {
            WS_BORDER = 0x800000U,
            WS_CAPTION = 0xC00000U,
            WS_CHILD = 0x40000000U,
            WS_CLIPCHILDREN = 0x2000000U,
            WS_CLIPSIBLINGS = 0x4000000U,
            WS_DISABLED = 0x8000000U,
            WS_DLGFRAME = 0x400000U,
            WS_HSCROLL = 0x100000U,
            WS_MAXIMIZE = 0x1000000U,
            WS_MAXIMIZEBOX = 0x10000U,
            WS_MINIMIZE = 0x20000000U,
            WS_MINIMIZEBOX = 0x20000U,
            WS_OVERLAPPED = 0x0U,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUP = 0x80000000U,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEFRAME = 0x40000U,
            WS_SYSMENU = 0x80000U,
            WS_VISIBLE = 0x10000000U,
            WS_VSCROLL = 0x200000U
        }
        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }
        internal readonly struct RECT
        {
            private readonly int _Left;
            private readonly int _Top;
            private readonly int _Right;
            private readonly int _Bottom;
            internal Size Size
            {
                get
                {
                    return new Size(_Right - _Left, _Bottom - _Top);
                }
            }
            internal int X
            {
                get
                {
                    return _Left;
                }
            }
            internal int Y
            {
                get
                {
                    return _Top;
                }
            }
        }
        /// <summary> Donne des informations sur la mémoire de l'ordinateur </summary>
        internal struct MEMORYSTATUSEX
        {
            internal uint dwLength;
            internal uint dwMemoryLoad;
            internal ulong ullTotalPhys;
            internal ulong ullAvailPhys;
            internal ulong ullTotalPageFile;
            internal ulong ullAvailPageFile;
            internal ulong ullTotalVirtual;
            internal ulong ullAvailVirtual;
            internal ulong ullAvailExtendedVirtual;
        }
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
        /// <summary> renvoie le nb d'octets la memoire total de l'ordinateur </summary>
        internal static ulong GetTotalMemory
        {
            get
            {
                var Memoire = new MEMORYSTATUSEX();
                Memoire.dwLength = (uint)Marshal.SizeOf(Memoire);
                GlobalMemoryStatusEx(ref Memoire);
                return Memoire.ullTotalPhys;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWINFO
        {
            public int cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public WindowStyles dwStyle;
            public WindowStylesEx dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public short wCreatorVersion;
        }

        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);
        internal static WINDOWINFO ObtenirInformationFenetre(IntPtr hwnd)
        {
            var pwi = new WINDOWINFO();
            _ = GetWindowInfo(hwnd, ref pwi);
            return pwi;
        }
        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        private static extern bool ShowWindow(IntPtr hwnd, ShowWindowCommands nCmdShow);
        /// <summary>Affichage direct d'une fenetre par windows avec les options definies en paramètre</summary>
        internal static bool AfficherFenetre(IntPtr hwnd, ShowWindowCommands nCmdShow)
        {
            return ShowWindow(hwnd, nCmdShow);
        }
        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        /// <summary>dimensionnement direct d'une fenetre par windows avec les options definies en paramètre</summary>
        internal static bool DimensionnerFenetre(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags)
        {
            return SetWindowPos(hWnd, hWndInsertAfter, X, Y, cx, cy, uFlags);
        }
        [DllImport("user32.dll", SetLastError = false)]
        private static extern bool SetForegroundWindow(IntPtr destination);
        /// <summary>renvoie par windows du handle de la fenêtre active</summary>
        internal static bool RendreFenetreActive(IntPtr destination)
        {
            return SetForegroundWindow(destination);
        }
        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetForegroundWindow();
        /// <summary>renvoie par windows du handle de la fenêtre active</summary>
        internal static IntPtr DonnerFenetreActive
        {
            get
            {
                return GetForegroundWindow();
            }
        }
        [DllImport("ntdll.dll", SetLastError = false, EntryPoint = "memcpy")]
        private static extern void CopyMemoryOld(IntPtr destination, IntPtr source, uint length);

        [DllImport("msvcrt.dll", SetLastError = false, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CopyMemory(IntPtr destination, IntPtr source, UIntPtr length);
        /// <summary> copie par windows d'une zone mémoire source vers une zone mémoire destination </summary>
        internal static void CopierMemoire(IntPtr destination, IntPtr source, int length)
        {
            try
            {
                CopyMemory(destination, source, new UIntPtr((uint)length));
            }
            catch
            {
                throw new Exception("Erreur copie mémoire");
            }
        }

        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern IntPtr MemSet(IntPtr dest, int c, int byteCount);
        internal static void InitialiserMemoire(IntPtr dest, int Valeur, int byteCount)
        {
            try
            {
                MemSet(dest, Valeur, byteCount);
            }
            catch
            {
                throw new Exception("Erreur remplissage mémoire");
            }
        }
        #endregion
    }
}