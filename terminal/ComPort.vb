' -----------------------------------------------------------------------------
' Модуль работы с сом портом ПК через API win32
' Работать может только с 1 портом.
' Среда VB 2008 x86 (WIN32) !
' 2016
' -----------------------------------------------------------------------------
Module ComPort
    Dim ComHandle As Int32          ' описатель (HANDLE) файла

    Const BUF_RX_SIZE = 1024        ' длинна приемного буфера 
    Const BUF_TX_SIZE = 1024        ' длинна передающего буфера
    Dim buf_rx(BUF_RX_SIZE) As Byte ' буфер для приема из порта
    Dim buf_tx(BUF_RX_SIZE) As Byte ' буфер для передачи в порт

    Const GENERIC_READ = &H80000000
    Const GENERIC_WRITE = &H40000000
    Const OPEN_EXISTING = &H3
    Const FILE_FLAG_OVERLAPPED = &H40000000

    Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Int32) As Int32
    Declare Function GetLastError Lib "kernel32" () As Int32
    Declare Function ReadFile Lib "kernel32" (ByVal hFile As Int32, ByRef lpBuffer As Byte, ByVal nNumberOfBytesToRead As Int32, ByRef lpNumberOfBytesRead As Int32, ByVal lpOverlapped As Int32) As Int32
    Declare Function WriteFile Lib "kernel32" (ByVal hFile As Int32, ByRef lpBuffer As Byte, ByVal nNumberOfBytesToWrite As Int32, ByRef lpNumberOfBytesWritten As Int32, ByVal lpOverlapped As Int32) As Int32
    Declare Function SetCommTimeouts Lib "kernel32" (ByVal hFile As Int32, ByRef lpCommTimeouts As COMMTIMEOUTS) As Int32
    Declare Function GetCommTimeouts Lib "kernel32" (ByVal hFile As Int32, ByRef lpCommTimeouts As COMMTIMEOUTS) As Int32
    Declare Function BuildCommDCB Lib "kernel32" Alias "BuildCommDCBA" (ByVal lpDef As String, ByRef lpDCB As DCB) As Int32
    Declare Function SetCommState Lib "kernel32" (ByVal hCommDev As Int32, ByRef lpDCB As DCB) As Int32
    Declare Function CreateFile Lib "kernel32" Alias "CreateFileA" (ByVal lpFileName As String, ByVal dwDesiredAccess As Int32, ByVal dwShareMode As Int32, ByVal lpSecurityAttributes As Int32, ByVal dwCreationDisposition As Int32, ByVal dwFlagsAndAttributes As Int32, ByVal hTemplateFile As Int32) As Int32
    Declare Function FlushFileBuffers Lib "kernel32" (ByVal hFile As Int32) As Int32


    Structure COMSTAT
        Dim fCtsHold As Int32
        Dim fDsrHold As Int32
        Dim fRlsdHold As Int32
        Dim fXoffHold As Int32
        Dim fXoffSent As Int32
        Dim fEof As Int32
        Dim fTxim As Int32
        Dim fReserved As Int32
        Dim cbInQue As Int32
        Dim cbOutQue As Int32
    End Structure

    Structure COMMTIMEOUTS
        Dim ReadIntervalTimeout As Int32
        Dim ReadTotalTimeoutMultiplier As Int32
        Dim ReadTotalTimeoutConstant As Int32
        Dim WriteTotalTimeoutMultiplier As Int32
        Dim WriteTotalTimeoutConstant As Int32
    End Structure

    Structure DCB
        Dim DCBlength As Int32
        Dim BaudRate As Int32
        Dim fBinary As Int32
        Dim fParity As Int32
        Dim fOutxCtsFlow As Int32
        Dim fOutxDsrFlow As Int32
        Dim fDtrControl As Int32
        Dim fDsrSensitivity As Int32
        Dim fTXContinueOnXoff As Int32
        Dim fOutX As Int32
        Dim fInX As Int32
        Dim fErrorChar As Int32
        Dim fNull As Int32
        Dim fRtsControl As Int32
        Dim fAbortOnError As Int32
        Dim fDummy2 As Int32
        Dim wReserved As Integer
        Dim XonLim As Integer
        Dim XoffLim As Integer
        Dim ByteSize As Byte
        Dim Parity As Byte
        Dim StopBits As Byte
        Dim XonChar As Byte
        Dim XoffChar As Byte
        Dim ErrorChar As Byte
        Dim EofChar As Byte
        Dim EvtChar As Byte
    End Structure

    Structure OVERLAPPED
        Dim Internal As Int32
        Dim InternalHigh As Int32
        Dim offset As Int32
        Dim OffsetHigh As Int32
        Dim hEvent As Int32
    End Structure

    Structure SECURITY_ATTRIBUTES
        Dim nLength As Int32
        Dim lpSecurityDescriptor As Int32
        Dim bInheritHandle As Int32
    End Structure

    Function ComPortClose()
        ComPortClose = CloseHandle(ComHandle)
    End Function

    Function ComPortFlush()
        ComPortFlush = FlushFileBuffers(ComHandle)
    End Function

    Function ComPortInit(ByVal ComNumber As String, ByVal Comsettings As String) As Boolean

        On Error GoTo handelinitcom

        Dim ComSetup As DCB, Answer, Stat As COMSTAT, RetBytes As Int32
        Dim retval As Int32
        Dim CtimeOut As COMMTIMEOUTS, BarDCB As DCB

        ' Open the communications port for read/write (&HC0000000).
        ' Must specify existing file (3).
        'ComNum = CreateFile(ComNumber, &HC0000000, 0, 0&, &H3, 0, 0)
        ComHandle = CreateFile(ComNumber, GENERIC_READ Or GENERIC_WRITE, 0, 0, OPEN_EXISTING, 0, 0)
        If ComHandle = -1 Then
            MsgBox("Com Port " & ComNumber & " not available. Use Serial settings (on the main menu) to setup your ports.", 48)
            ComPortInit = False
            Exit Function
        End If

        'Setup Time Outs for com port
        CtimeOut.ReadIntervalTimeout = 1 ' 20
        CtimeOut.ReadTotalTimeoutConstant = 1 ' 1
        CtimeOut.ReadTotalTimeoutMultiplier = 0
        CtimeOut.WriteTotalTimeoutConstant = 0 '0 '1 ' 10
        CtimeOut.WriteTotalTimeoutMultiplier = 1 '1 '0

        retval = SetCommTimeouts(ComHandle, CtimeOut)
        If retval = -1 Then
            retval = GetLastError()
            MsgBox("Unable to set timeouts for port " & ComNumber & " Error: " & retval)
            retval = CloseHandle(ComHandle)
            ComPortInit = False
            Exit Function
        End If

        retval = BuildCommDCB(Comsettings, BarDCB)
        If retval = -1 Then
            retval = GetLastError()
            MsgBox("Unable to build Comm DCB " & Comsettings & " Error: " & retval)
            retval = CloseHandle(ComHandle)
            ComPortInit = False
            Exit Function
        End If

        retval = SetCommState(ComHandle, BarDCB)
        If retval = -1 Then
            retval = GetLastError()
            MsgBox("Unable to set Comm DCB " & Comsettings & " Error: " & retval)
            retval = CloseHandle(ComHandle)
            ComPortInit = False
            Exit Function
        End If

        ComPortInit = True
        Exit Function

handelinitcom:
        ComPortInit = False
        CloseHandle(ComHandle)
        Exit Function
    End Function

    '-------------------------------------------------------------------
    ' Чтение данных из порта
    ' buf_r()   - массив куда будут скопированы данные
    ' RetBytes  - размер buf_out() буфера И размер причитанных данных !
    '-------------------------------------------------------------------
    Sub ComPortRead(ByRef buf_r() As Byte, ByRef RetBytes As Int32)
        On Error GoTo handelpurecom

        Dim i As Integer
        Dim retval As Int32 = 0

        'retval = ReadFile(ComNum, buf_rx(0), BUF_RX_SIZE, RetBytes, 0)
        retval = ReadFile(ComHandle, buf_rx(0), RetBytes, RetBytes, 0)
        If (RetBytes > 0) Then
            For i = 0 To RetBytes - 1
                buf_r(i) = buf_rx(i)
            Next i
        Else
            ComPortFlush()
        End If
handelpurecom:
        Exit Sub
    End Sub
    '-------------------------------------------------------------------
    ' Передача массива buf_t() байт, длинной len в порт
    ' Возвращает количество пеерданных байт
    '-------------------------------------------------------------------
    Function ComPortWrite(ByVal buf_t() As Byte, ByVal len As Int32) As Int32

        On Error GoTo handelwritelpt
        Dim RetBytes As Int32
        Dim LenVal As Int32
        Dim retval As Int32

        retval = WriteFile(ComHandle, buf_t(0), len, RetBytes, 0)
        ComPortFlush()

        ComPortWrite = RetBytes

handelwritelpt:
        Exit Function
    End Function
    '-------------------------------------------------------------------
    ' Передача строки в порт
    '
    '-------------------------------------------------------------------
    Function ComPortWrite(ByVal s As String) As Int32

        On Error GoTo handelwritelpt
        Dim RetBytes As Int32 = 0
        Dim RetBytesCount As Int32 = 0
        Dim LenVal As Int32 = 0
        Dim retval As Int32 = 0
        Dim i As Integer = 0

        If Len(s) > BUF_TX_SIZE Then
            ReDim buf_tx(Len(s))
        End If

        For LenVal = 0 To Len(s) - 1
            buf_tx(LenVal) = Asc(Mid$(s, LenVal + 1, 1))
        Next LenVal

        buf_tx(LenVal) = 0
        retval = WriteFile(ComHandle, buf_tx(0), Len(s), RetBytes, 0)
        ComPortFlush()

        ComPortWrite = RetBytes

handelwritelpt:
        Exit Function
    End Function

End Module
