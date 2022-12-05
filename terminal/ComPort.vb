' -----------------------------------------------------------------------------
' Модуль работы с сом портом ПК через API win32
' Работать может только с 1 портом.
' Среда VB 2008 x86 (WIN32) !
' 2016
' -----------------------------------------------------------------------------
Module ComPort
    Dim ComHandle As Int32          ' описатель (HANDLE) файла

    Const BUF_TX_SIZE = 1024        ' длинна передающего буфера
    Dim buf_tx(BUF_TX_SIZE) As Byte ' буфер для передачи в порт

    Const GENERIC_READ = &H80000000
    Const GENERIC_WRITE = &H40000000
    Const OPEN_EXISTING = &H3
    Const FILE_FLAG_OVERLAPPED = &H40000000

    Const RTS_CONTROL_DISABLE = &H0
    Const DTR_CONTROL_DISABLE = &H0

    ' stop bits
    Public Const ONESTOPBIT = &H0
    Public Const ONE5STOPBITS = &H1
    Public Const TWOSTOPBITS = &H2

    ' parity
    Public Const NOPARITY = &H0
    Public Const ODDPARITY = &H1
    Public Const EVENPARITY = &H2
    Public Const MARKPARITY = &H3
    Public Const SPACEPARITY = &H4


    ' Состояния сигналов CTS DSR RI RLSD - function GetCommModemStatus
    Public Const MS_CTS_ON = &H10 ' The CTS (clear-to-send) signal is on.
    Public Const MS_DSR_ON = &H20 ' The DSR (data-set-ready) signal is on.
    Public Const MS_RING_ON = &H40 ' The ring indicator signal is on.
    Public Const MS_RLSD_ON = &H80 ' The RLSD (receive-line-signal-detect) signal is on. (CD)

    ' Состояние сигналов RTS, DTR - function EscapeCommFunction
    Public Const SETRTS = &H3 ' // Set RTS high
    Public Const CLRRTS = &H4 ' // Set RTS low
    Public Const SETDTR = &H5 ' // Set DTR high
    Public Const CLRDTR = &H6 ' // Set DTR low

    Declare Function CloseHandle Lib "kernel32" (ByVal hObject As UInt32) As Boolean 'Int32
    Declare Function GetLastError Lib "kernel32" () As UInt32
    Declare Function ReadFile Lib "kernel32" (ByVal hFile As UInt32, ByRef lpBuffer As Byte, ByVal nNumberOfBytesToRead As UInt32, ByRef lpNumberOfBytesRead As UInt32, ByVal lpOverlapped As UInt32) As Int32
    Declare Function WriteFile Lib "kernel32" (ByVal hFile As UInt32, ByRef lpBuffer As Byte, ByVal nNumberOfBytesToWrite As Int32, ByRef lpNumberOfBytesWritten As Int32, ByVal lpOverlapped As Int32) As Int32
    Declare Function SetCommTimeouts Lib "kernel32" (ByVal hFile As UInt32, ByRef lpCommTimeouts As COMMTIMEOUTS) As Int32
    Declare Function GetCommTimeouts Lib "kernel32" (ByVal hFile As UInt32, ByRef lpCommTimeouts As COMMTIMEOUTS) As Int32
    Declare Function BuildCommDCB Lib "kernel32" Alias "BuildCommDCBA" (ByVal lpDef As String, ByRef lpDCB As DCB) As Int32
    Declare Function SetCommState Lib "kernel32" (ByVal hCommDev As Int32, ByRef lpDCB As DCB) As Int32
    Declare Function GetCommState Lib "kernel32" (ByVal hCommDev As Int32, ByRef lpDCB As DCB) As Int32
    Declare Function CreateFile Lib "kernel32" Alias "CreateFileA" (ByVal lpFileName As String, ByVal dwDesiredAccess As Int32, ByVal dwShareMode As UInt32, ByVal lpSecurityAttributes As UInt32, ByVal dwCreationDisposition As UInt32, ByVal dwFlagsAndAttributes As UInt32, ByVal hTemplateFile As UInt32) As Int32
    Declare Function FlushFileBuffers Lib "kernel32" (ByVal hFile As UInt32) As Int32
    Declare Function GetCommModemStatus Lib "kernel32" (ByVal hFile As UInt32, ByRef lpModemStat As UInt32) As Boolean
    Declare Function EscapeCommFunction Lib "kernel32" (ByVal hFile As UInt32, ByVal dwFunc As UInt32) As Boolean





    Structure COMSTAT
        Dim fCtsHold As UInt32
        Dim fDsrHold As UInt32
        Dim fRlsdHold As UInt32
        Dim fXoffHold As UInt32
        Dim fXoffSent As UInt32
        Dim fEof As UInt32
        Dim fTxim As UInt32
        Dim fReserved As UInt32
        Dim cbInQue As UInt32
        Dim cbOutQue As UInt32
    End Structure

    Structure COMMTIMEOUTS
        Dim ReadIntervalTimeout As UInt32
        Dim ReadTotalTimeoutMultiplier As UInt32
        Dim ReadTotalTimeoutConstant As UInt32
        Dim WriteTotalTimeoutMultiplier As UInt32
        Dim WriteTotalTimeoutConstant As UInt32
    End Structure

    ' структура была переделана т.к. в ней присутствуют битовые поля ! которые в vb отсутсвуют
    Structure DCB
        Dim DCBlength As UInt32
        Dim BaudRate As UInt32

        Dim fFlagi As UInt32
        'Dim fBinary As UInt32
        'Dim fParity As UInt32
        'Dim fOutxCtsFlow As UInt32
        'Dim fOutxDsrFlow As UInt32
        'Dim fDtrControl As UInt32
        'Dim fDsrSensitivity As UInt32
        'Dim fTXContinueOnXoff As UInt32
        'Dim fOutX As UInt32
        'Dim fInX As UInt32
        'Dim fErrorChar As UInt32
        'Dim fNull As UInt32
        'Dim fRtsControl As UInt32
        'Dim fAbortOnError As UInt32
        'Dim fDummy2 As UInt32

        Dim wReserved As UInt16
        Dim XonLim As UInt16
        Dim XoffLim As UInt16
        Dim ByteSize As Byte
        Dim Parity As Byte
        Dim StopBits As Byte
        Dim XonChar As Byte
        Dim XoffChar As Byte
        Dim ErrorChar As Byte
        Dim EofChar As Byte
        Dim EvtChar As Byte
        Dim wReserved1 As UInt16
    End Structure

    ' Оригинальная структура
    'DWORD DCBlength;            // sizeof(DCB)
    'DWORD BaudRate;             // current baud rate
    'DWORD fBinary:1;            // binary mode, no EOF check
    'DWORD fParity:1;            // enable parity checking
    'DWORD fOutxCtsFlow:1;       // CTS output flow control
    'DWORD fOutxDsrFlow:1;       // DSR output flow control
    'DWORD fDtrControl:2;        // DTR flow control type
    'DWORD fDsrSensitivity:1;    // DSR sensitivity
    'DWORD fTXContinueOnXoff:1;  // XOFF continues Tx
    'DWORD fOutX:1;              // XON/XOFF out flow control
    'DWORD fInX:1;               // XON/XOFF in flow control
    'DWORD fErrorChar:1;         // enable error replacement
    'DWORD fNull:1;              // enable null stripping
    'DWORD fRtsControl:2;        // RTS flow control
    'DWORD fAbortOnError:1;      // abort reads/writes on error
    'DWORD fDummy2:17;           // reserved
    'WORD  wReserved;            // not currently used
    'WORD  XonLim;               // transmit XON threshold
    'WORD  XoffLim;              // transmit XOFF threshold
    'BYTE  ByteSize;             // number of bits/byte, 4-8
    'BYTE  Parity;               // 0-4=no,odd,even,mark,space
    'BYTE  StopBits;             // 0,1,2 = 1, 1.5, 2
    'char  XonChar;              // Tx and Rx XON character
    'char  XoffChar;             // Tx and Rx XOFF character
    'char  ErrorChar;            // error replacement character
    'char  EofChar;              // end of input character
    'char  EvtChar;              // received event character
    'WORD  wReserved1;           // reserved; do not use


    Structure OVERLAPPED
        Dim Internal As UInt32
        Dim InternalHigh As UInt32
        Dim offset As UInt32
        Dim OffsetHigh As UInt32
        Dim hEvent As UInt32
    End Structure

    Structure SECURITY_ATTRIBUTES
        Dim nLength As UInt32
        Dim lpSecurityDescriptor As UInt32
        Dim bInheritHandle As UInt32
    End Structure

    Function ComPortClose()
        ComPortClose = CloseHandle(ComHandle)
    End Function

    Function ComPortFlush()
        ComPortFlush = FlushFileBuffers(ComHandle)
    End Function

    Function ComPortInit(ByVal ComNumber As String, ByVal baudrate As UInt32, ByVal parity As UInt32, ByVal stopbits As UInt32) As Boolean

        On Error GoTo handelinitcom

        Dim ComSetup As DCB, Answer, Stat As COMSTAT, RetBytes As Int32
        Dim retval As Int32
        Dim CtimeOut As COMMTIMEOUTS, BarDCB As DCB

        ' Open the communications port for read/write (&HC0000000).
        ' Must specify existing file (3).
        'ComNum = CreateFile(ComNumber, &HC0000000, 0, 0&, &H3, 0, 0)
        ' для номеров портов выще 9 необходимо указывать (\\.\)
        ComHandle = CreateFile("\\.\" + ComNumber, GENERIC_READ + GENERIC_WRITE, 0, 0, OPEN_EXISTING, 0, 0)
        If ComHandle = -1 Then
            MsgBox("Com Port " & ComNumber & " not available. Use Serial settings (on the main menu) to setup your ports.", 48)
            ComPortInit = False
            Exit Function
        End If

        'Setup Time Outs for com port
        CtimeOut.ReadIntervalTimeout = 4294967295 '1 ' 20
        CtimeOut.ReadTotalTimeoutConstant = 0 ' 1
        CtimeOut.ReadTotalTimeoutMultiplier = 0
        CtimeOut.WriteTotalTimeoutConstant = 0 '0 '1 ' 10
        CtimeOut.WriteTotalTimeoutMultiplier = 1 '1 '0

        retval = SetCommTimeouts(ComHandle, CtimeOut)
        If retval = 0 Then
            retval = GetLastError()
            MsgBox("Unable to set timeouts for port " & ComNumber & " Error: " & retval)
            retval = CloseHandle(ComHandle)
            ComPortInit = False
            Exit Function
        End If

        ' эта функция не заполняет структуру BarDCB
        ' возможно из-за наличия битовых полей в структуре !

        'retval = BuildCommDCB(Comsettings, BarDCB)
        'If retval = -1 Then
        'retval = GetLastError()
        'MsgBox("Unable to build Comm DCB " & Comsettings & " Error: " & retval)
        'retval = CloseHandle(ComHandle)
        'ComPortInit = False
        'Exit Function
        'End If

        retval = GetCommState(ComHandle, BarDCB)
        If retval = 0 Then
            retval = GetLastError()
            MsgBox("Get comm state return Error: " & retval)
            retval = CloseHandle(ComHandle)
            ComPortInit = False
            Exit Function
        End If

        BarDCB.BaudRate = baudrate
        BarDCB.ByteSize = 8
        BarDCB.Parity = parity
        BarDCB.StopBits = stopbits

        retval = SetCommState(ComHandle, BarDCB)
        If retval = 0 Then
            retval = GetLastError()
            MsgBox("Unable to set Comm DCB " & " Error: " & retval & vbCrLf & "!!! Данная кофигурация: Скорость или настройки порта не поддерживаются драйвером !!!")
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
    ' RetBytes  - размер buf_out() буфера И размер прочитанных данных !
    '
    ' результат - 0 - Ошибка, !=0 - нет ошибок
    '-------------------------------------------------------------------
    Function ComPortRead(ByRef buf_r() As Byte, ByRef RetBytes As Int32) As Int32
        On Error GoTo handelpurecom

        Dim retval As Int32 = 0

        retval = ReadFile(ComHandle, buf_r(0), RetBytes, RetBytes, 0)
        If retval = 0 Then
            ComPortRead = 1
        Else
            ComPortRead = 0
        End If

handelpurecom:
        Exit Function
    End Function
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

    '-------------------------------------------------------------------
    ' Чтение состояния линий CTS, DSR, DCD и RI
    '
    '-------------------------------------------------------------------
    Function ComPortGetModemStatus(ByRef status As UInt32) As Boolean

        Return GetCommModemStatus(ComHandle, status)

    End Function

    '-------------------------------------------------------------------
    ' Установка линий DTR в заданное состояние
    '-------------------------------------------------------------------
    Function ComPortSetDTR(ByRef dtr As UInt32) As Boolean

        If dtr = 0 Then
            Return EscapeCommFunction(ComHandle, SETDTR)
        Else
            Return EscapeCommFunction(ComHandle, CLRDTR)
        End If

    End Function

    '-------------------------------------------------------------------
    ' Установка линий RTS в заданное состояние
    '-------------------------------------------------------------------
    Function ComPortSetRTS(ByRef rts As UInt32) As Boolean

        If rts = 0 Then
            Return EscapeCommFunction(ComHandle, SETRTS)
        Else
            Return EscapeCommFunction(ComHandle, CLRRTS)
        End If

    End Function






End Module
