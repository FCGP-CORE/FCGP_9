Friend Module NativeMethods
#Region "win32 Souris"
    ''' <summary>pour envoyer une nouvelle position du curseur de souris à une application</summary>
    <DllImport("user32.dll", SetLastError:=False, CharSet:=CharSet.Auto)>
    Private Function SetCursorPos(X As Integer, Y As Integer) As Integer
    End Function
    Friend ReadOnly Property PositionnerCurseur(X As Integer, Y As Integer) As Integer
        Get
            Return SetCursorPos(X, Y)
        End Get
    End Property
    ''' <summary>pour envoyer un des boutons de souris appuyé ou relevé à une application. attention pas 'Mouse_event' sinon pas trouver dans user32.dll</summary>
    <DllImport("user32.dll", SetLastError:=False, CharSet:=CharSet.Auto)>
    Private Sub mouse_event(dwFlags As Integer, dx As Integer, dy As Integer,
                                                    cButtons As Integer, dwExtraInfo As IntPtr)
    End Sub
    ''' <summary>Uniquement pour encapsuler les appels des fonctions ou procédures API. Provoque l'appui du bouton gauche de la souris</summary>
    Friend Sub BGAppui()
        mouse_event(MOUSEEVENTF.LEFTDOWN + MOUSEEVENTF.ABSOLUTE, 0, 0, 0, IntPtr.Zero)
    End Sub
    ''' <summary>Uniquement pour encapsuler les appels des fonctions ou procédures API. Provoque le relachement du bouton gauche de la souris</summary>
    Friend Sub BGRelache()
        mouse_event(MOUSEEVENTF.LEFTUP + MOUSEEVENTF.ABSOLUTE, 0, 0, 0, IntPtr.Zero)
    End Sub
    ''' <summary>Uniquement pour encapsuler les appels des fonctions ou procédures API. Provoque le relachement du bouton gauche de la souris</summary>
    Friend Sub BGClick()
        mouse_event(MOUSEEVENTF.LEFTDOWN + MOUSEEVENTF.ABSOLUTE, 0, 0, 0, IntPtr.Zero)
        mouse_event(MOUSEEVENTF.LEFTUP + MOUSEEVENTF.ABSOLUTE, 0, 0, 0, IntPtr.Zero)
    End Sub
    'Constantes pour l'envoi des évenements souris
    Private Enum MOUSEEVENTF
        ABSOLUTE = &H8000
        LEFTDOWN = &H2
        LEFTUP = &H4
        RIGHTDOWN = &H8
        RIGHTUP = &H10
    End Enum
#End Region
#Region "win32 Window"
    <Flags()>
    Friend Enum WindowStylesEx As UInteger
        WS_INCONNU = &H800
        WS_EX_ACCEPTFILES = &H10
        WS_EX_APPWINDOW = &H40000
        WS_EX_CLIENTEDGE = &H200
        WS_EX_COMPOSITED = &H2000000
        WS_EX_CONTEXTHELP = &H400
        WS_EX_CONTROLPARENT = &H10000
        WS_EX_DLGMODALFRAME = &H1
        WS_EX_LAYERED = &H80000
        WS_EX_LAYOUTRTL = &H400000
        WS_EX_LEFT = &H0
        WS_EX_LEFTSCROLLBAR = &H4000
        WS_EX_MDICHILD = &H40
        WS_EX_NOACTIVATE = &H8000000
        WS_EX_NOINHERITLAYOUT = &H100000
        WS_EX_NOPARENTNOTIFY = &H4
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE Or WS_EX_CLIENTEDGE
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE Or WS_EX_TOOLWINDOW Or WS_EX_TOPMOST
        WS_EX_RIGHT = &H1000
        WS_EX_RTLREADING = &H2000
        WS_EX_STATICEDGE = &H20000
        WS_EX_TOOLWINDOW = &H80
        WS_EX_TOPMOST = &H8
        WS_EX_TRANSPARENT = &H20
        WS_EX_WINDOWEDGE = &H100
    End Enum
    ''' <summary>Window Styles.The following styles can be specified wherever a window style is required. After the control has been created, these styles cannot be modified, except as noted.</summary>
    <Flags()>
    Friend Enum WindowStyles As UInteger
        WS_BORDER = &H800000
        WS_CAPTION = &HC00000
        WS_CHILD = &H40000000
        WS_CLIPCHILDREN = &H2000000
        WS_CLIPSIBLINGS = &H4000000
        WS_DISABLED = &H8000000
        WS_DLGFRAME = &H400000
        WS_HSCROLL = &H100000
        WS_MAXIMIZE = &H1000000
        WS_MAXIMIZEBOX = &H10000
        WS_MINIMIZE = &H20000000
        WS_MINIMIZEBOX = &H20000
        WS_OVERLAPPED = &H0
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED Or WS_CAPTION Or WS_SYSMENU Or WS_SIZEFRAME Or WS_MINIMIZEBOX Or WS_MAXIMIZEBOX
        WS_POPUP = &H80000000UI
        WS_POPUPWINDOW = WS_POPUP Or WS_BORDER Or WS_SYSMENU
        WS_SIZEFRAME = &H40000
        WS_SYSMENU = &H80000
        WS_VISIBLE = &H10000000
        WS_VSCROLL = &H200000
    End Enum
    Friend Enum ShowWindowCommands As Integer
        Hide = 0
        Normal = 1
        ShowMinimized = 2
        Maximize = 3
        ShowNoActivate = 4
        Show = 5
        Minimize = 6
        ShowMinNoActive = 7
        ShowNA = 8
        Restore = 9
        ShowDefault = 10
        ForceMinimize = 11
    End Enum
    Friend Structure RECT
        Private ReadOnly _Left As Integer
        Private ReadOnly _Top As Integer
        Private ReadOnly _Right As Integer
        Private ReadOnly _Bottom As Integer
        Friend ReadOnly Property Size() As Size
            Get
                Return New Size(_Right - _Left, _Bottom - _Top)
            End Get
        End Property
        Friend ReadOnly Property X As Integer
            Get
                Return _Left
            End Get
        End Property
        Friend ReadOnly Property Y As Integer
            Get
                Return _Top
            End Get
        End Property
    End Structure
    ''' <summary> Donne des informations sur la mémoire de l'ordinateur </summary>
    Friend Structure MEMORYSTATUSEX
        Friend dwLength As UInteger
        Friend dwMemoryLoad As UInteger
        Friend ullTotalPhys As ULong
        Friend ullAvailPhys As ULong
        Friend ullTotalPageFile As ULong
        Friend ullAvailPageFile As ULong
        Friend ullTotalVirtual As ULong
        Friend ullAvailVirtual As ULong
        Friend ullAvailExtendedVirtual As ULong
    End Structure
    <DllImport("Kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=False)>
    Friend Function GlobalMemoryStatusEx(ByRef lpBuffer As MEMORYSTATUSEX) As Boolean
    End Function
    ''' <summary> renvoie le nb d'octets la memoire total de l'ordinateur </summary>
    Friend ReadOnly Property GetTotalMemory() As ULong
        Get
            Dim Memoire As New MEMORYSTATUSEX()
            Memoire.dwLength = CUInt(Marshal.SizeOf(Memoire))
            GlobalMemoryStatusEx(Memoire)
            Return Memoire.ullTotalPhys
        End Get
    End Property

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure WINDOWINFO
        Dim cbSize As Integer
        Dim rcWindow As RECT
        Dim rcClient As RECT
        Dim dwStyle As WindowStyles
        Dim dwExStyle As WindowStylesEx
        Dim dwWindowStatus As UInteger
        Dim cxWindowBorders As UInteger
        Dim cyWindowBorders As UInteger
        Dim atomWindowType As UShort
        Dim wCreatorVersion As Short
    End Structure

    <DllImport("user32.dll", SetLastError:=False, CharSet:=CharSet.Auto)>
    Private Function GetWindowInfo(hwnd As IntPtr, ByRef pwi As WINDOWINFO) As Boolean
    End Function
    Friend ReadOnly Property ObtenirInformationFenetre(hwnd As IntPtr) As WINDOWINFO
        Get
            Dim pwi = New WINDOWINFO
            Dim Ok As Boolean = GetWindowInfo(hwnd, pwi)
            Return pwi
        End Get
    End Property
    <DllImport("user32.dll", SetLastError:=False, CharSet:=CharSet.Auto)>
    Private Function ShowWindow(hwnd As IntPtr, nCmdShow As ShowWindowCommands) As Boolean
    End Function
    ''' <summary>Affichage direct d'une fenetre par windows avec les options definies en paramètre</summary>
    Friend ReadOnly Property AfficherFenetre(hwnd As IntPtr, nCmdShow As ShowWindowCommands) As Boolean
        Get
            Return ShowWindow(hwnd, nCmdShow)
        End Get
    End Property
    <DllImport("user32.dll", SetLastError:=False, CharSet:=CharSet.Auto)>
    Private Function SetWindowPos(hWnd As IntPtr, hWndInsertAfter As IntPtr, X As Integer, Y As Integer, cx As Integer,
                                   cy As Integer, uFlags As Integer) As Boolean
    End Function
    ''' <summary>dimensionnement direct d'une fenetre par windows avec les options definies en paramètre</summary>
    Friend ReadOnly Property DimensionnerFenetre(hWnd As IntPtr, hWndInsertAfter As IntPtr, X As Integer, Y As Integer, cx As Integer,
                               cy As Integer, uFlags As Integer) As Boolean
        Get
            Return SetWindowPos(hWnd, hWndInsertAfter, X, Y, cx, cy, uFlags)
        End Get
    End Property
    <DllImport("user32.dll", SetLastError:=False)>
    Private Function SetForegroundWindow(destination As IntPtr) As Boolean
    End Function
    ''' <summary>renvoie par windows du handle de la fenêtre active</summary>
    Friend ReadOnly Property RendreFenetreActive(destination As IntPtr) As Boolean
        Get
            Return SetForegroundWindow(destination)
        End Get
    End Property
    <DllImport("user32.dll", SetLastError:=False)>
    Private Function GetForegroundWindow() As IntPtr
    End Function
    ''' <summary>renvoie par windows du handle de la fenêtre active</summary>
    Friend ReadOnly Property DonnerFenetreActive() As IntPtr
        Get
            Return GetForegroundWindow()
        End Get
    End Property
    <DllImport("ntdll.dll", SetLastError:=False, EntryPoint:="memcpy")>
    Private Sub CopyMemoryOld(destination As IntPtr, source As IntPtr, length As UInteger)
    End Sub

    <DllImport("msvcrt.dll", SetLastError:=False, EntryPoint:="memcpy", CallingConvention:=CallingConvention.Cdecl)>
    Private Sub CopyMemory(destination As IntPtr, source As IntPtr, length As UIntPtr)
    End Sub
    ''' <summary> copie par windows d'une zone mémoire source vers une zone mémoire destination </summary>
    Friend Sub CopierMemoire(destination As IntPtr, source As IntPtr, length As Integer)
        Try
            CopyMemory(destination, source, New UIntPtr(CUInt(length)))
        Catch
            Throw New Exception("Erreur copie mémoire")
        End Try
    End Sub

    <DllImport("msvcrt.dll", EntryPoint:="memset", CallingConvention:=CallingConvention.Cdecl, SetLastError:=False)>
    Private Function MemSet(dest As IntPtr, c As Integer, byteCount As Integer) As IntPtr
    End Function
    Friend Sub InitialiserMemoire(dest As IntPtr, Valeur As Integer, byteCount As Integer)
        Try
            MemSet(dest, Valeur, byteCount)
        Catch
            Throw New Exception("Erreur remplissage mémoire")
        End Try
    End Sub
#End Region
End Module
