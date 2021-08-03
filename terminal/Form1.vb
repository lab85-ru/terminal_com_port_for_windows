Imports System
Imports System.IO
Imports System.IO.Ports
Imports System.Management
Imports System.Threading


'
' Необходимо реализовать:
' + передачу строки в нех кодировка как в фаре
' - прием ESC последовательностей



Public Class Form1

    Dim flag_thread_stop As UInteger = 0 ' остановка потока
    Dim RX_Thread As System.Threading.Thread = Nothing


    Const PORT_OPEN As String = "Открыть"
    Const PORT_CLOSE As String = "Закрыть"

    Const SEND_STR_START As String = "Строка"
    Const SEND_STR_STOP As String = "СТОП"

    Const SEND_FILE_START As String = "Файл"
    Const SEND_FILE_STOP As String = "СТОП"

    Const RX_COUNT_TXT As String = "RX: "
    Dim rx_counter_global As Long = 0     ' статистика, счетчик общего количества принятых байт
    Const TX_COUNT_TXT As String = "TX: "
    Dim tx_counter_global As Long = 0     ' статистика, счетчик общего количества переданных байт

    Enum port_status_e
        open = 1
        close = 0
    End Enum

    Dim com_port_speed_int As Integer = 0 ' числовое значение скорости (для расчета, таймаута между передачами блоков)
    Dim com_port_parity As UInt32
    Dim com_port_stop_bit As UInt32

    ' промежуточный линейный буфер для приема массива из порта
    Const BUFIN_SIZE = 2048
    Dim bufin(BUFIN_SIZE) As Byte     ' промежуточный буфер

    Const BUF_STR_TX_SIZE = 128
    Dim buf_str_tx(BUF_STR_TX_SIZE) As Byte  ' буфер для передачи строки
    Dim buf_str_tx_n As Integer              ' количество пеедаваемых байт

    Dim flag_write_log As Boolean      ' = 1 запись лога в файл
    Dim Ports As String()              ' список портов в системе
    Dim CPortStatus As port_status_e   ' состояниесом порта открыт - закрыт

    Dim rbSpeed_array() As RadioButton ' массив радиобутом скорость порта

    ' структура привязывает радиобутом к строке которую надо послать в конце строки
    Structure send_end_str_st
        Dim rb As RadioButton
        Dim s As String
    End Structure


    Dim rbAddEndStr_array() As send_end_str_st ' массив радиобутом - добавить в конец строки

    ' структура привязывает радиобутом к четности для упрощения поиска в массиве
    Structure parity_st
        Dim rb As RadioButton
        Dim p As UInt32
    End Structure

    Dim rbParity_array() As parity_st ' массив четность

    Structure stopbit_st
        Dim rb As RadioButton
        Dim s As UInt32
    End Structure

    Dim rbStopBit_array() As stopbit_st

    Dim f_log As FileStream             ' Лог файл

    ' структура с информацией о передаваемом файле
    Structure file_send_st
        Dim fs As FileStream
        Const BUF_SIZE = 1024
        Dim buf() As Byte
        Dim res As Integer
        Dim f As IO.FileInfo
        Dim file_size As Long
        Dim file_count As Long
    End Structure
    Enum avtomat_decoder_string_t
        wait
        hex
        delay
    End Enum


    Dim f_send_st As file_send_st


    '--------------------------------------------------------------
    ' Запрос списка СОМ портов в системе
    '--------------------------------------------------------------
    Private Sub GetAllSerialPortsName()
        Try
            Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_PnPEntity")
            For Each queryObj As ManagementObject In searcher.Get()
                If InStr(queryObj("Caption"), "(COM") > 0 Then
                    tbLogRx.AppendText(queryObj("Caption") + vbCrLf)
                End If
            Next
        Catch err As ManagementException
            MsgBox(err.Message)
        End Try
    End Sub

    '--------------------------------------------------------------
    ' Поиск СОМ портов в системе
    '--------------------------------------------------------------
    Sub find_com_port()
        Dim port As String
        Const STR_TIRE = "--------------------------------------"

        Ports = SerialPort.GetPortNames
        cbPorts.Items.Clear()

        tbLogRx.AppendText(vbCrLf + STR_TIRE + vbCrLf)
        tbLogRx.AppendText("Поиск доступных СОМ портов в системе:" + vbCrLf)
        For Each port In Ports
            tbLogRx.AppendText(port + vbCrLf)
            cbPorts.Items.Add(port)
        Next port
        tbLogRx.AppendText(STR_TIRE + vbCrLf)

        cbPorts.SelectedIndex = 0 ' Всегда выбираем самый первый порт, для заполнения поля а то ПУСТОЕ плохо смотриться

        GetAllSerialPortsName()
        tbLogRx.AppendText(STR_TIRE + vbCrLf)

    End Sub

    Private Sub btScanComPort_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btScanComPort.Click
        find_com_port()
    End Sub

    Private Sub btPOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btPOpen.Click

        Dim st_p As parity_st
        Dim st_s As stopbit_st
        Dim irb As RadioButton
        Dim com_port_parameter As String = ""

        If cbPorts.SelectedItem <> "" And CPortStatus = port_status_e.close Then

            rx_counter_global = 0
            tx_counter_global = 0

            ' Определение текущей скорости и заносим в строку скорость
            For Each irb In rbSpeed_array
                If irb.Checked = True Then
                    If irb.Checked = True And rbSpeedNumer.Checked = True Then ' поле ввода скорости
                        com_port_speed_int = Val(tbPortSpeedNumer.Text)
                    Else
                        com_port_speed_int = Val(irb.Text)
                    End If
                    Exit For
                End If
            Next

            If com_port_speed_int = 0 Or (com_port_speed_int < 110 Or com_port_speed_int > 3000000) Then
                MsgBox("ОШИБКА: Значение параметра скорость задано не верно !" + vbCrLf + "MIN=110 MAX=3000000")
                Exit Sub
            End If


            ' определение четности
            For Each st_p In rbParity_array
                If st_p.rb.Checked = True Then
                    com_port_parity = st_p.p
                    Exit For
                End If
            Next

            ' стоп бит
            For Each st_s In rbStopBit_array
                If st_s.rb.Checked = True Then
                    com_port_stop_bit = st_s.s
                    Exit For
                End If
            Next

            If ComPortInit(cbPorts.SelectedItem, com_port_speed_int, com_port_parity, com_port_stop_bit) = False Then
                tbLogRx.AppendText(vbCrLf + "Ошибка: открытия порта." + vbCrLf)
                Exit Sub
            End If

            CPortStatus = port_status_e.open
            btPOpen.Text = PORT_CLOSE

            rx_counter_global = 0 ' Сбрассываем счетчик принятых и переданных байт
            tx_counter_global = 0


            ' Гасим меню недаем выбрать- пока не закроют порт
            gbSetPortSpeed.Enabled = False
            gbSetPortStopBit.Enabled = False
            gbSetPortParity.Enabled = False

            gbTx.Enabled = True
            gbKey.Enabled = True

            cbPorts.Enabled = False ' выключаем выбор номера порта

            tbLogRx.AppendText(vbCrLf + cbPorts.SelectedItem + " ОТКРЫТ ++++++++++++++++++++++++++++++++++++++++++++++++++" + vbCrLf)

            ' запускаем второй поток - приема данных из порта
            flag_thread_stop = 0
            RX_Thread = New System.Threading.Thread(AddressOf Thread_com_port_rx)
            RX_Thread.Name = "COM port RX data thread"
            RX_Thread.Start()
            RX_Thread.IsBackground = True

        ElseIf CPortStatus = port_status_e.open Then ' Закрываем СОМ порт -------------------------------------
            GlobalComPortClose()
        End If
    End Sub

    ' Закрытие порта + отключение интерфейсов 
    Sub GlobalComPortClose()

        If CPortStatus <> port_status_e.open Then
            Exit Sub
        End If

        ComPortClose()

        flag_thread_stop = 1
        RX_Thread.Join()

        ' Включаем меню даем выбрать
        gbSetPortSpeed.Enabled = True
        gbSetPortStopBit.Enabled = True
        gbSetPortParity.Enabled = True

        gbTx.Enabled = False ' выключаем передача строки
        gbKey.Enabled = False

        cbPorts.Enabled = True ' включаем выбор номера порта

        Timer3.Enabled = False ' Выключам передачу файла - даже на посреди передачи
        btFileSend.Text = SEND_FILE_START

        Timer2.Enabled = False
        btSendString.Text = SEND_STR_START ' Выключаем таймер переодической передачи строки
        btSendString.Enabled = True
        gbStringEnd.Enabled = True
        gbTypeTxStr.Enabled = True
        cbStrSend.Enabled = True
        btFileSend.Enabled = True

        CPortStatus = port_status_e.close
        btPOpen.Text = PORT_OPEN
        tbLogRx.AppendText(vbCrLf + cbPorts.SelectedItem + " ЗАКРЫТ --------------------------------------------------" + vbCrLf)

    End Sub


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'RX_Thread.Priority = ThreadPriority.Highest

        CPortStatus = port_status_e.close
        btPOpen.Text = PORT_OPEN
        rbSpeed115200.Checked = True
        rbStopBit1.Checked = True
        rbParityNo.Checked = True

        flag_write_log = 0

        ' заносим в массив соотвествие скрости и радиобутона
        ReDim rbSpeed_array(7)
        rbSpeed_array(0) = rbSpeed4800
        rbSpeed_array(1) = rbSpeed9600
        rbSpeed_array(2) = rbSpeed19200
        rbSpeed_array(3) = rbSpeed38400
        rbSpeed_array(4) = rbSpeed57600
        rbSpeed_array(5) = rbSpeed115200
        rbSpeed_array(6) = rbSpeedNumer

        rbSpeed4800.Text = "4800"
        rbSpeed9600.Text = "9600"
        rbSpeed19200.Text = "19200"
        rbSpeed38400.Text = "38400"
        rbSpeed57600.Text = "57600"
        rbSpeed115200.Text = "115200"

        ' заносим в массив соотвествие четности и радиобутона
        ReDim rbParity_array(5)
        rbParity_array(0).rb = rbParityNo
        rbParity_array(0).p = NOPARITY

        rbParity_array(1).rb = rbParityOdd
        rbParity_array(1).p = ODDPARITY

        rbParity_array(2).rb = rbParityEven
        rbParity_array(2).p = EVENPARITY

        rbParity_array(3).rb = rbParityMark
        rbParity_array(3).p = MARKPARITY

        rbParity_array(4).rb = rbParitySpace
        rbParity_array(4).p = SPACEPARITY

        ' заносим в массив соотвествие стопового бита и радиобутона
        ReDim rbStopBit_array(3)
        rbStopBit_array(0).rb = rbStopBit1
        rbStopBit_array(0).s = ONESTOPBIT

        rbStopBit_array(1).rb = rbStopBit15
        rbStopBit_array(1).s = ONE5STOPBITS

        rbStopBit_array(2).rb = rbStopBit2
        rbStopBit_array(2).s = TWOSTOPBITS

        ReDim rbAddEndStr_array(4)
        rbAddEndStr_array(0).rb = rbAddStrEndClear
        rbAddEndStr_array(0).s = ""

        rbAddEndStr_array(1).rb = rbAddStrEnd0D
        rbAddEndStr_array(1).s = Chr(&HD)

        rbAddEndStr_array(2).rb = rbAddStrEnd0D0A
        rbAddEndStr_array(2).s = Chr(&HD) + Chr(&HA)

        rbAddEndStr_array(3).rb = rbAddStrEnd00
        rbAddEndStr_array(3).s = Chr(&H0)

        rbAddStrEndClear.Checked = True

        tx_counter_global = 0
        rx_counter_global = 0
        trx_count_update() ' обновление счетчиков TX RX в строке статуса

        rbTypeTx1.Checked = True

        btSendString.Text = SEND_STR_START

        ' изменяет размер метки в строке статуса
        tsslRxCounter.AutoSize = False
        Dim slab As Size = New Size(100, tsslRxCounter.Size.Height)
        tsslRxCounter.Size = slab

        tsslTxCounter.AutoSize = False
        slab = New Size(100, tsslRxCounter.Size.Height)
        tsslTxCounter.Size = slab

        gbTx.Enabled = False
        gbKey.Enabled = False

        ' настройка струкруры передачи файла
        ReDim f_send_st.buf(file_send_st.BUF_SIZE)

        find_com_port() ' Производим поиск ком портов в системы

        ToolTip1.IsBalloon = True ' Подсказка в стиле комикса
    End Sub

    ' таймер настроен на 10 мс (при 100 мс - неуспевает принимать, теряются данные)
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

    End Sub

    ' Обработка смены состояния: запись лог файла
    Private Sub cbLogFile_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbLogFile.CheckedChanged
        Dim fname As String = ""

        If cbLogFile.Checked = True Then
            flag_write_log = True
            fname = CreateLogFileName()
            f_log = New FileStream(fname, FileMode.CreateNew, FileAccess.Write)
        Else
            flag_write_log = False
            f_log.Close()
        End If
    End Sub

    ' Формируем имя файла для лога
    ' создает строку с текущей датой и расширением .log
    Function CreateLogFileName() As String
        Dim str As String
        Dim cc As String
        Dim i As Integer
        Dim str_date As String
        Dim str_date_out As String

        str_date = DateTime.Now
        str_date_out = str_date.ToString()

        str = ""
        For i = 1 To Len(str_date_out)
            cc = Mid(str_date_out, i, 1)
            If cc = "." Or cc = ":" Or cc = " " Then
                cc = "_"
            End If
            str = str + cc
        Next
        str = str + ".log"

        CreateLogFileName = str
    End Function
    '--------------------------------------------------------------------------
    ' Входная строка в виде ТХТ - НЕХ виде, пересылается в виде байт
    '--------------------------------------------------------------------------
    Private Sub btSendStringToHEX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btSendStringToHEX.Click
        ' добавляем строку в историю ввода если такой строки нет
        If cbStrSend.FindString(cbStrSend.Text) = -1 Then
            cbStrSend.Items.Add(cbStrSend.Text)
        End If

        decode_txt_hex_codes(cbStrSend.Text + vbCrLf)
    End Sub

    '--------------------------------------------------------------------------
    ' Событие нажата кнопка перечачи строки в порт
    ' Передать строку
    '--------------------------------------------------------------------------
    Private Sub btSendString_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btSendString.Click

        Dim d As Integer

        ' добавляем строку в историю ввода если такой строки нет
        If cbStrSend.FindString(cbStrSend.Text) = -1 Then
            cbStrSend.Items.Add(cbStrSend.Text)
        End If

        If rbTypeTxWhile.Checked = True Then ' циклическая передача
            If Timer2.Enabled = False Then
                d = Val(tbTxDelay.Text)
                If d = 0 Then
                    tbLogRx.AppendText("ВНИМАНИЕ: Период = 0, будет установлен период = 10 секундам." + vbCrLf)
                    d = 10000
                ElseIf d > 10000 Then
                    tbLogRx.AppendText("ВНИМАНИЕ: Период слишком большой, будет установлен период = 10 секундам." + vbCrLf)
                    d = 10000
                End If

                Timer2.Interval = d
                Timer2.Enabled = True
                btSendString.Text = SEND_STR_STOP

                gbStringEnd.Enabled = False
                gbTypeTxStr.Enabled = False
                cbStrSend.Enabled = False
                btFileSend.Enabled = False
            Else
                Timer2.Enabled = False
                btSendString.Text = SEND_STR_START

                gbStringEnd.Enabled = True
                gbTypeTxStr.Enabled = True
                cbStrSend.Enabled = True
                btFileSend.Enabled = True
            End If

        Else
            TxString()
        End If
    End Sub

    ' Передача строки 1 раз + вывод в лог(символы - НЕХ)
    Sub TxString()
        Dim s As String = ""
        Dim rba As send_end_str_st
        Dim s_hex As String = ""
        Dim i As Integer
        Dim c As Char

        s = cbStrSend.Text

        ' копируем строку в промежуточный буфер
        buf_str_tx_n = 0
        For i = 1 To Len(s)
            c = Mid(cbStrSend.Text, i, 1)
            buf_str_tx(buf_str_tx_n) = Convert.ToByte(c)
            buf_str_tx_n = buf_str_tx_n + 1
        Next

        ' добавить символы в конец строки (-, 0D, 0D0A, 00)
        For Each rba In rbAddEndStr_array
            If rba.rb.Checked = True Then
                If Len(rba.s) <> 0 Then
                    For i = 1 To Len(rba.s)
                        c = Mid(rba.s, i, 1)
                        buf_str_tx(buf_str_tx_n) = Asc(c)
                        buf_str_tx_n = buf_str_tx_n + 1
                    Next
                End If
                Exit For
            End If
        Next

        ' Вывод строки в виде HEX / TEXT
        s = ""
        If cbPrintHex.Checked = True Then
            s = ConvArrayByteToHEX(buf_str_tx, buf_str_tx_n)
            tbLogTx.AppendText(s)
        Else
            For i = 0 To buf_str_tx_n - 1 ' выводим строку по символьно в поле ----------------------------
                s = s + Convert.ToChar(buf_str_tx(i))
            Next
            tbLogTx.AppendText(s)
        End If

        ComPortWrite(buf_str_tx, buf_str_tx_n)

        tx_counter_global = tx_counter_global + buf_str_tx_n
        trx_count_update() ' обновление счетчиков TX RX в строке статуса

    End Sub

    ' Передача файла
    Private Sub btFileSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btFileSend.Click
        Dim t As Integer

        If btFileSend.Text = SEND_FILE_STOP Then
            tbLogTx.AppendText(vbCrLf + "Передача файла остановлена..." + vbCrLf)
            Timer3.Enabled = False
            btFileSend.Text = SEND_FILE_START

            gbStringEnd.Enabled = True
            gbTypeTxStr.Enabled = True
            cbStrSend.Enabled = True
            btSendString.Enabled = True

            Exit Sub
        End If

        f_send_st.file_count = 0
        f_send_st.file_size = 0
        f_send_st.res = 0

        tspbBar.Value = 0

        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = "C:\"
        OpenFileDialog1.Filter = "All files (*.*)|*.*"
        OpenFileDialog1.FilterIndex = 2
        OpenFileDialog1.RestoreDirectory = True

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

            f_send_st.f = New IO.FileInfo(OpenFileDialog1.FileName)
            f_send_st.file_size = f_send_st.f.Length   ' длинна файла нужна чтобы знать сколько всего байт передавать и расчитывать проценты
            If f_send_st.file_size = 0 Then
                Exit Sub
            End If

            f_send_st.fs = New FileStream(OpenFileDialog1.FileName, FileMode.Open, FileAccess.Read)

            tbLogTx.AppendText(vbCrLf + "Передача файла (размер = " + Str(f_send_st.file_size) + " байт) ..." + vbCrLf)

            If com_port_speed_int = 0 Then
                MsgBox("ОШИБКА: com_port_speed_int = 0, Деление на ноль !")
                Exit Sub
            End If

            ' Расчет интервала передачи блока
            t = (file_send_st.BUF_SIZE / (Val(com_port_speed_int) / 10)) * 1000

            If t = 0 Then
                t = 5
            End If

            Timer3.Interval = t
            Timer3.Enabled = True

            gbStringEnd.Enabled = False
            gbTypeTxStr.Enabled = False
            cbStrSend.Enabled = False
            btSendString.Enabled = False

            btFileSend.Text = SEND_FILE_STOP
        End If

    End Sub

    ' обновление счетчиков TX RX в строке статуса
    Sub trx_count_update()
        tsslRxCounter.Text = RX_COUNT_TXT + Str(rx_counter_global)
        tsslTxCounter.Text = TX_COUNT_TXT + Str(tx_counter_global)
    End Sub

    ' Удаление строк
    ' str_store - количество строк которое необходимо сохранить
    '
    Sub Del_Str(ByRef tb As TextBox, ByVal str_store As Integer)
        Dim l_n As Integer    ' количество линий
        Dim l As Integer      ' счетчик линий

        ' удаляем строки
        l_n = tb.Lines.Count

        ' Для отладки
        'tbLogTx.AppendText(Str(l_n) + vbCrLf)

        If l_n > str_store Then
            Dim newList As List(Of String) = tb.Lines.ToList
            For l = 0 To l_n - str_store
                newList.RemoveAt(0)        ' удаляем самую вернюю строку т.к. после удаления строки сдвигаются в верх
            Next
            tb.Clear()
            tb.Lines = newList.ToArray

            tb.SelectionStart = tb.Text.Length - 1
            tb.ScrollToCaret()
        End If

    End Sub

    ' Таймер переодической передачи строки
    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        TxString()
    End Sub

    ' Очистка лога передачи
    Private Sub btClearTxLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btClearTxLog.Click
        tbLogTx.Clear()
    End Sub

    ' Очистка лога приема
    Private Sub btClearRxLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btClearRxLog.Click
        tbLogRx.Clear()
    End Sub

    ' Передача файла по таймеру
    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick

        f_send_st.res = f_send_st.fs.Read(f_send_st.buf, 0, file_send_st.BUF_SIZE)

        If f_send_st.res = 0 Then ' все передали выходим
            tbLogTx.AppendText(vbCrLf + "Передача файла закончена." + vbCrLf)
            Timer3.Enabled = False
            btFileSend.Text = SEND_FILE_START

            gbStringEnd.Enabled = True
            gbTypeTxStr.Enabled = True
            cbStrSend.Enabled = True
            btSendString.Enabled = True

            Exit Sub
        End If

        ' время выполнения
        'Dim TStart As Date = Now

        ComPortWrite(f_send_st.buf, f_send_st.res)
        'tbLogTx.AppendText(Now.Subtract(TStart).TotalMilliseconds.ToString & "TX ms" & vbCrLf)

        tx_counter_global = tx_counter_global + f_send_st.res
        trx_count_update() ' обновление счетчиков TX RX в строке статуса

        f_send_st.file_count = f_send_st.file_count + f_send_st.res

        tspbBar.Value = f_send_st.file_count * 100 / f_send_st.file_size

    End Sub

    ' Отправка в порт всех нажатий на клавиатуре
    Private Sub tbLogTx_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles tbLogTx.KeyPress
        Dim s As String = ""
        Dim k(2) As Byte

        If CPortStatus = port_status_e.close Then
            Exit Sub
        End If

        k(0) = Asc(e.KeyChar)

        ' Вывод знака в виде HEX / TEXT
        s = ""
        If cbPrintHex.Checked = True Then
            e.Handled = True ' поглощаем символ

            s = Hex(k(0))
            If Len(s) = 1 Then
                s = " 0" + s
            Else
                s = " " + s
            End If
            tbLogTx.AppendText(s)
        Else
            ' не выводим т.к. итак символ будет в окне
            'tbLogTx.AppendText(Chr(k(0)))
        End If

        ComPortWrite(k, 1)

        tx_counter_global = tx_counter_global + 1
        trx_count_update() ' обновление счетчиков TX RX в строке статуса

    End Sub

    ' Обработка нажатий клавиш в окне приема
    Private Sub tbLogRx_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles tbLogRx.KeyPress
        Dim s As String = ""
        Dim k(2) As Byte

        If CPortStatus = port_status_e.close Then
            Exit Sub
        End If

        k(0) = Asc(e.KeyChar)
        e.Handled = True ' поглощаем символ

        ' Вывод знака в виде HEX / TEXT
        s = ""
        If cbPrintHex.Checked = True Then
            'e.Handled = True ' поглощаем символ

            s = Hex(k(0))
            If Len(s) = 1 Then
                s = " 0" + s
            Else
                s = " " + s
            End If
            tbLogTx.AppendText(s)
        Else
            ' выводим в окно передачи ! 
            If k(0) = &HD Then
                tbLogTx.AppendText(vbCrLf)
            Else
                tbLogTx.AppendText(Chr(k(0)))
            End If
        End If

        ComPortWrite(k, 1)

        tx_counter_global = tx_counter_global + 1
        trx_count_update() ' обновление счетчиков TX RX в строке статуса
    End Sub
    '--------------------------------------------------------------------------
    ' Проигрыватель строки
    '
    ' 0..f - перевод в hex и передача в порт
    ' >12345 - задержка в мс(пробел между > и числом не допускается)
    '
    ' 00 01 02 af b e -> 00 01 02 af 0b 0e 
    ' >100 -> задержка 100 мс
    ' >12345 -> задержка 12345 мс
    '
    '--------------------------------------------------------------------------
    Public Sub decode_txt_hex_codes(ByVal s As String)

        Dim c As Char ' символ из строки
        Dim i As Integer = 1 ' индекс по входной строке

        Dim delay_ms As UInteger ' задержка в мс, пересчет из delay_num
        Dim delay_cnt As UInteger ' счетчик числа символов в delay_num
        Dim delay_num(5) As Byte ' массив для сборки числа

        Dim hex1, hex2 As Byte ' hex старший, младший байт
        Dim send_byte As Byte ' байт для послылки в порт
        Dim st As avtomat_decoder_string_t = avtomat_decoder_string_t.wait
        Dim su As String = s.ToUpper
        Dim ss As Char

        ' накопительный буфер для формирования последовательности на передачу
        Const TX_ARRAY_SIZE = 1024
        Dim tx_array(TX_ARRAY_SIZE) As Byte
        Dim tx_array_cnt As UInteger = 0

        If s.Length = 0 Then
            Exit Sub
        End If

        While i <> s.Length
            ss = Mid(su, i, 1)
            c = Convert.ToChar(ss)
            i = i + 1

            Select Case st
                Case avtomat_decoder_string_t.wait
                    If (c >= "0" And c <= "9") Or (c >= "A" And c <= "F") Then
                        hex1 = Convert.ToByte(c)
                        st = avtomat_decoder_string_t.hex
                    ElseIf (c = ">") Then
                        st = avtomat_decoder_string_t.delay
                    End If

                Case avtomat_decoder_string_t.hex
                    If (c >= "0" And c <= "9") Or (c >= "A" And c <= "F") Then
                        hex2 = Convert.ToByte(c)
                        send_byte = build_hex(hex1, hex2)
                        tx_array(tx_array_cnt) = send_byte
                        tx_array_cnt = tx_array_cnt + 1
                        'print_log(send_byte.ToString("X2") + " ")
                        hex1 = 0
                        hex2 = 0
                        st = avtomat_decoder_string_t.wait
                    ElseIf (c = ">") Then
                        send_byte = build_hex(0, hex1)
                        'print_log(send_byte.ToString("X2") + " ")
                        tx_array(tx_array_cnt) = send_byte
                        tx_array_cnt = tx_array_cnt + 1
                        hex1 = 0
                        hex2 = 0
                        st = avtomat_decoder_string_t.delay
                    Else
                        send_byte = build_hex(0, hex1)
                        'print_log(send_byte.ToString("X2") + " ")
                        tx_array(tx_array_cnt) = send_byte
                        tx_array_cnt = tx_array_cnt + 1
                        hex1 = 0
                        hex2 = 0
                        st = avtomat_decoder_string_t.wait
                    End If

                Case avtomat_decoder_string_t.delay
                    If (c >= "0" And c <= "9") Then
                        If delay_cnt < 5 Then
                            delay_num(delay_cnt) = Convert.ToByte(c)
                            delay_cnt = delay_cnt + 1
                        End If
                    Else
                        delay_ms = build_delay(delay_num, delay_cnt)
                        print_log("Пауза: " + Str(delay_ms) + " ms" + vbCrLf)
                        delay_cnt = 0

                        Threading.Thread.Sleep(delay_ms)

                        st = avtomat_decoder_string_t.wait
                    End If

            End Select

            ' Передача массива при: переводе строки(построчная передача) или достижении максимального размера буфера
            If ((c = vbCr Or c = vbLf) And tx_array_cnt <> 0) Or tx_array_cnt = TX_ARRAY_SIZE Then
                SerialPort_data_send(tx_array, tx_array_cnt)
                tx_array_cnt = 0
            End If

        End While

    End Sub
    '--------------------------------------------------------------------------
    ' Проигрыванеи сценария из файла
    ' Пример сценария:
    ' Файл TXT, формат:
    ' 81 01 04 07 03 FF
    ' > 100
    ' 81 01 04 07 02 FF
    '
    ' Где:
    ' 81 01 04 07 03 FF - значения байт в TXT-HEX формате отправляемых в СОМ порт
    ' > 100             - Формирование паузы 100 мс.
    ' 81 01 04 07 02 FF - значения байт в HEX формате отправляемых в СОМ порт
    '
    '
    '--------------------------------------------------------------------------
    Private Sub bt_Load_TXT_File_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bt_Load_TXT_File.Click
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = "." '    "C:\"
        OpenFileDialog1.Filter = "All files (*.*)|*.*"
        OpenFileDialog1.FilterIndex = 2
        OpenFileDialog1.RestoreDirectory = True

        ReDim f_send_st.buf(file_send_st.BUF_SIZE)

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

            cbPrintHex.Checked = True ' Включаем вывод в виде HEX иначе на прием будет выводиться в виде строки символов

            f_send_st.f = New IO.FileInfo(OpenFileDialog1.FileName)
            f_send_st.file_size = f_send_st.f.Length   ' длинна файла нужна чтобы знать сколько всего байт передавать и расчитывать проценты
            If f_send_st.file_size = 0 Then
                Exit Sub
            End If

            Dim fileReader As System.IO.StreamReader
            fileReader = My.Computer.FileSystem.OpenTextFileReader(OpenFileDialog1.FileName)
            Dim stringReader As String

            If CPortStatus = port_status_e.close Then
                print_log("Ошибка порт не открыт, СТОП ..." + vbCrLf)
                Exit Sub
            End If

            print_log("Проигрывание сценария из файла..." + vbCrLf)
            print_log("+++ СТАРТ ++++++++++++++++++++++++++++++++" + vbCrLf)
            Do
                stringReader = fileReader.ReadLine()
                'print_log(stringReader + vbCrLf)

                ' Добавили перевод строки, используется как разделитель т.к. алгоритм ожидает конец строки
                ' а при чтении ReadLine - конец строки не поподает в строку
                decode_txt_hex_codes(stringReader + vbCrLf)

            Loop While stringReader <> Nothing ' .Length > 0

            print_log("+++ СТОП +++++++++++++++++++++++++++++++++" + vbCrLf)

            fileReader.Close()

        End If

    End Sub
    '--------------------------------------------------------------------------
    ' Вывод в окно лога + в файл
    '--------------------------------------------------------------------------
    Sub print_log(ByVal txt As String)

        tbLogRx.AppendText(txt)

        ' Запись в лог файл Приема
        If flag_write_log = True And txt.Length > 0 Then
            f_log.Write(System.Text.Encoding.Default.GetBytes(txt), 0, txt.Length)
        End If

    End Sub
    '--------------------------------------------------------------------------
    ' Преобразование символа в байт
    '--------------------------------------------------------------------------
    Function convert_char_to_byte(ByVal c As Byte) As Byte
        Dim b As Byte

        If (c >= Asc("0") And c <= Asc("9")) Then
            b = c - &H30
        ElseIf (c >= Asc("A") And c <= Asc("F")) Then
            b = c - &H37
        End If

        Return b
    End Function

    '--------------------------------------------------------------------------
    ' Преобразование пяти символов в число
    '--------------------------------------------------------------------------
    Function build_hex(ByVal h As Byte, ByVal l As Byte) As Byte
        Dim bl, bh As Byte

        bh = convert_char_to_byte(h) * 16
        bl = convert_char_to_byte(l)

        Return bh Or bl
    End Function

    '--------------------------------------------------------------------------
    ' Преобразование двух символов в байт
    '--------------------------------------------------------------------------
    Function build_delay(ByVal array() As Byte, ByVal len As UInteger) As UInteger
        Select Case len
            Case 1
                Return (array(0) - &H30)
            Case 2
                Return (array(0) - &H30) * 10 + (array(1) - &H30)
            Case 3
                Return (array(0) - &H30) * 100 + (array(1) - &H30) * 10 + (array(2) - &H30)
            Case 4
                Return (array(0) - &H30) * 1000 + (array(1) - &H30) * 100 + (array(2) - &H30) * 10 + (array(3) - &H30)
            Case 5
                Return (array(0) - &H30) * 10000 + (array(1) - &H30) * 1000 + (array(2) - &H30) * 100 + (array(3) - &H30) * 10 + (array(4) - &H30)
            Case Else
                Return 0
        End Select
    End Function
    '--------------------------------------------------------------------------
    ' Посылка пакета
    '--------------------------------------------------------------------------
    Sub SerialPort_data_send(ByVal data() As Byte, ByVal len As Integer)

        Dim s As String = ""

        If CPortStatus = port_status_e.close Then
            print_log("Ошибка: Порт закрыт.")
            Exit Sub
        End If

        ComPortWrite(data, len)

        s = "TX:" + vbCrLf
        print_log(s)

        s = ConvArrayByteToHEX(data, len)
        print_log(s)

    End Sub
    '--------------------------------------------------------------------------
    ' Второй поток приема данных из СОМ порта
    '--------------------------------------------------------------------------
    Sub Thread_com_port_rx()

        Do While flag_thread_stop = 0

            Thread.Sleep(2)

            If InvokeRequired Then
                BeginInvoke(New MethodInvoker(AddressOf decode_rx_com_port_data))
            End If

        Loop
    End Sub

    '--------------------------------------------------------------------------
    ' Обработка и вывод принятых данных из СОМ порта
    '--------------------------------------------------------------------------
    Sub decode_rx_com_port_data()
        Dim ub As Byte            ' принятый байт
        Dim s As String = ""      ' строка в виде НЕХ
        Dim s1 As String = ""     ' строка в виде БИН
        Dim s_out As String = ""  ' строка на вывод
        Dim sl_n As Integer = 0   ' счетчик длинны строки
        Dim sc As String = ""     ' строка одного символа НЕХ
        Dim sw As String = ""     ' строка для сохранения в файл
        Dim l_n As Integer = 0    ' количество линий
        Dim l_n2 As Integer = 0   ' количество линий в строке (для расчета)
        Const LINES_MAX = 25 * 10 ' максимальное количество строк в техт боксе
        Dim din As UInt32         ' количество пришедших данных
        Dim res As Int32          ' != 0 ошибка

        'If CPortStatus = port_status_e.close Then
        'Exit Sub
        'End If

        ' времы выполнения
        'Dim TStart As Date = Now
        'tbLogTx.AppendText(Now.Subtract(TStart).TotalMilliseconds.ToString & " ms" & vbCrLf)

        ' ----- ПРИЕМ ДАННЫХ -----------
        din = BUFIN_SIZE
        res = ComPortRead(bufin, din)
        If res <> 0 Then ' Произошда ошибка - закрываем порт
            GlobalComPortClose()
        End If


        'tbLogTx.AppendText(Now.Subtract(TStart).TotalMilliseconds.ToString & "RX ms" & vbCrLf)

        If din = 0 Then ' Пусто нет данных выходим
            Exit Sub
        End If

        'tbLogTx.AppendText(Str(din) + vbCrLf)

        rx_counter_global = rx_counter_global + din

        ' Запись в лог файл Приема
        If flag_write_log = True And din > 0 Then
            f_log.Write(bufin, 0, din)
        End If

        ' --------- ОБРАБОТКА ПРИНЯТЫХ БАННЫХ -----------------
        If cbPrintHex.Checked = True Then ' HEX -------------------------------------------------------------------------------
            s_out = ConvArrayByteToHEX(bufin, din)

        Else                               ' ASCII ------------------------------------------------------------------
            For i = 0 To din - 1
                ub = bufin(i)

                Select Case ub
                    Case &H0 To &H9, &HB, &HC, &HE To &H19
                        ub = &H2E ' за место кода 00 выводим точку "."
                        s_out = s_out + Chr(ub)

                    Case &HD ' CR
                        s_out = s_out + vbCr

                    Case &HA ' LF
                        If cbCode_0A_to_0D0A.Checked = True Then ' Преобразовать код \n -> \n\r
                            s_out = s_out + vbCrLf
                        Else
                            s_out = s_out + vbLf
                        End If

                    Case Else
                        s_out = s_out + Chr(ub)
                End Select

            Next i

        End If

        If cbPrintHex.Checked = True Then ' HEX -------------------------------------------------------------------------------
            l_n = tbLogRx.Lines.Count
            tbLogRx.AppendText(s_out)
            l_n = tbLogRx.Lines.Count
            If l_n > LINES_MAX + 100 Then
                Del_Str(tbLogRx, LINES_MAX)
            End If

        Else ' ASCII ----------------------------------------------------------------------------------------------------------
            l_n2 = Len(s_out)
            tbLogRx.AppendText(s_out)
            If l_n > LINES_MAX + 100 Then
                Del_Str(tbLogRx, LINES_MAX)
            End If

        End If

        trx_count_update() ' обновление счетчиков TX RX в строке статуса

    End Sub

    Private Sub Form1_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing

        If IsNothing(RX_Thread) Then

        ElseIf RX_Thread.ThreadState <> ThreadState.Unstarted Then
            flag_thread_stop = 1 ' Останавливаем второй поток
            RX_Thread.Join() '     Ожидаем завершения потока
        End If

    End Sub
End Class
