Imports System
Imports System.IO
Imports System.IO.Ports
Imports System.Threading


Public Class Form1

    Const PORT_OPEN As String = "Открыть"
    Const PORT_CLOSE As String = "Закрыть"

    Const SEND_STR_START As String = "Послать"
    Const SEND_STR_STOP As String = "СТОП"

    Const RX_COUNT_TXT As String = "RX: "
    Dim rx_counter_global As Long = 0     ' статистика, счетчик общего количества принятых байт
    Const TX_COUNT_TXT As String = "TX: "
    Dim tx_counter_global As Long = 0     ' статистика, счетчик общего количества переданных байт

    Enum port_status_e
        open = 1
        close = 0
    End Enum

    ' Очередь
    Const BUF_RX_SIZE As UInt32 = 10 * 1024 * 1024   ' размер входного приемного буфера
    Dim buf_rx_in As UInt32 = 0                 ' входной счетчик принятых байт в буфере
    Dim buf_rx_out As UInt32 = 0                ' вЫходной счетчик принятых байт в буфере
    Dim buf_rx(BUF_RX_SIZE) As Byte             ' сам приемный буфер

    ' промежуточный линейный буфер для приема массива из порта
    Const BUFIN_SIZE = 1000
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
        Dim p As Parity
    End Structure

    Dim rbParity_array() As parity_st ' массив четность


    Structure stopbit_st
        Dim rb As RadioButton
        Dim s As StopBits
    End Structure

    Dim rbStopBit_array() As stopbit_st



    Private Sub btScanComPort_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btScanComPort.Click
        Ports = SerialPort.GetPortNames
        Dim port As String

        cbPorts.Items.Clear()

        For Each port In Ports
            tbLogRx.AppendText(port + vbCrLf)
            cbPorts.Items.Add(port)
        Next port
    End Sub

    Private Sub btPOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btPOpen.Click

        Dim com_port_speed As String = ""
        Dim com_port_parity As Parity
        Dim com_port_stop_bit As StopBits
        Dim st_p As parity_st
        Dim st_s As stopbit_st
        Dim i As RadioButton

        If cbPorts.SelectedItem <> "" And CPortStatus = port_status_e.close Then

            rx_counter_global = 0
            tx_counter_global = 0

            SerialPort1.PortName = cbPorts.SelectedItem

            ' Определение текущей скорости и заносим в строку скорость
            For Each i In rbSpeed_array
                If i.Checked = True Then
                    If i.Checked = True And rbSpeedNumer.Checked = True Then ' поле ввода скорости
                        com_port_speed = tbPortSpeedNumer.Text
                    Else
                        com_port_speed = i.Text
                    End If
                    Exit For
                End If
            Next

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


            Try
                SerialPort1.Parity = com_port_parity
                SerialPort1.BaudRate = Int(com_port_speed)
                SerialPort1.StopBits = com_port_stop_bit
                SerialPort1.Open()
            Catch ex As Exception
                tbLogRx.AppendText("ОШИБКА: Открытия порта. " + ex.Message + vbCrLf)
                Exit Sub
            End Try

            CPortStatus = port_status_e.open
            btPOpen.Text = PORT_CLOSE

            ' Гасим меню недаем выбрать- пока не закроют порт
            gbSetPortSpeed.Enabled = False
            gbSetPortStopBit.Enabled = False
            gbSetPortParity.Enabled = False

            tbLogRx.AppendText("Порт Открыт." + vbCrLf)
        ElseIf CPortStatus = port_status_e.open Then
            SerialPort1.Close()

            ' Включаем меню даем выбрать
            gbSetPortSpeed.Enabled = True
            gbSetPortStopBit.Enabled = True
            gbSetPortParity.Enabled = True

            CPortStatus = port_status_e.close
            btPOpen.Text = PORT_OPEN
            tbLogRx.AppendText("Порт закрыт." + vbCrLf)
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
        rbParity_array(0).p = Parity.None

        rbParity_array(1).rb = rbParityOdd
        rbParity_array(1).p = Parity.Odd

        rbParity_array(2).rb = rbParityEven
        rbParity_array(2).p = Parity.Even

        rbParity_array(3).rb = rbParityMark
        rbParity_array(3).p = Parity.Mark

        rbParity_array(4).rb = rbParitySpace
        rbParity_array(4).p = Parity.Space

        ' заносим в массив соотвествие стопового бита и радиобутона
        ReDim rbStopBit_array(3)
        rbStopBit_array(0).rb = rbStopBit1
        rbStopBit_array(0).s = StopBits.One

        rbStopBit_array(1).rb = rbStopBit2
        rbStopBit_array(1).s = StopBits.Two

        rbStopBit_array(2).rb = rbStopBit15
        rbStopBit_array(2).s = StopBits.OnePointFive

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
    End Sub

    ' RX: Прием данных с порта
    Private Sub SerialPort1_DataReceived(ByVal sender As System.Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Dim fr As UInt32      ' количество свободного места
        Dim din As UInt32     ' количество пришедших данных
        Dim din_c As UInt32 = 0

        din = SerialPort1.Read(bufin, 0, SerialPort1.BytesToRead)

        If buf_rx_in >= buf_rx_out Then
            fr = BUF_RX_SIZE - buf_rx_in + buf_rx_out
        Else
            fr = buf_rx_out + buf_rx_in
        End If

        If fr > din + 1 Then

            While din_c <> din
                buf_rx(buf_rx_in) = bufin(din_c)
                buf_rx_in = buf_rx_in + 1
                din_c = din_c + 1

                If buf_rx_in = BUF_RX_SIZE Then
                    buf_rx_in = 0
                End If
            End While
        Else
            MsgBox("Ощибка:Переполнение приемного буффера.")
        End If

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim ub As Byte        ' принятый байт
        'Dim i As UInt32       ' счетчик по принятым байтам
        Dim s As String = ""  ' строка в виде НЕХ
        Dim s1 As String = "" ' строка в виде БИН
        Const SL = 16         ' длинна строки
        Dim sl_n As Integer   ' счетчик длинны строки
        Dim sc As String      ' строка одного символа НЕХ
        Dim sw As String = "" ' строка для сохранения в файл
        Dim l_n As Integer    ' количество линий
        'Dim l As Integer      ' счетчик линий
        Const LINES_MAX = 25 * 10 ' максимальное количество строк в техт боксе

        Dim TStart1 As Date ' = Now
        'MsgBox(Now.Subtract(TStart).TotalMilliseconds.ToString & " ms", , "Замер производительности")


        If buf_rx_in = buf_rx_out Then ' Пусто нет данных выходим
            Exit Sub
        End If

        If cbPrintHex.Checked = True Then ' HEX -------------------------------------------------------------------------------
            s = ""
            s1 = ""
            sl_n = 0

            While buf_rx_in <> buf_rx_out

                ub = buf_rx(buf_rx_out)
                buf_rx_out = buf_rx_out + 1

                rx_counter_global = rx_counter_global + 1

                sw = sw + Chr(ub)

                If buf_rx_out = BUF_RX_SIZE Then
                    buf_rx_out = 0
                End If

                ' добавляем 0 с переди если в строке один символ 1 -> 01, 4 -> 04
                sc = Hex(ub)
                If Len(sc) = 1 Then
                    sc = "0" + sc
                End If
                s = s + sc + " "

                If ub <= &H1F And ub >= 0 Then
                    ub = &H2E ' "."
                End If
                s1 = s1 + Chr(ub)

                sl_n = sl_n + 1

                If sl_n = SL Then
                    tbLogRx.AppendText(s + "    " + s1 + vbCrLf)
                    s = ""
                    s1 = ""
                    sl_n = 0

                    l_n = tbLogRx.Lines.Count
                    If l_n > LINES_MAX + 10 Then
                        TStart1 = Now
                        Del_Str(tbLogRx, LINES_MAX)
                        tbLogTx.AppendText("1 " + Now.Subtract(TStart1).TotalMilliseconds.ToString & " ms" + vbCrLf)
                    End If

                End If

            End While

            If Len(s) > 0 Then ' если строка была короче SL символов, то выводим строку и в конец довадяем пробелы, т.е. выравниваем
                tbLogRx.AppendText(s)
                sc = ""
                While sl_n <> SL
                    sc = sc + "   "
                    sl_n = sl_n + 1
                End While

                tbLogRx.AppendText(sc + "    " + s1 + vbCrLf)

            End If

        Else                               ' ASCII ------------------------------------------------------------------
            While buf_rx_out <> buf_rx_in
                ub = buf_rx(buf_rx_out)
                buf_rx_out = buf_rx_out + 1
                rx_counter_global = rx_counter_global + 1

                If buf_rx_out = BUF_RX_SIZE Then
                    buf_rx_out = 0
                End If

                sw = sw + Chr(ub)

                'If ub <= &H1F And ub >= 0 Then
                If ub = 0 Then
                    ub = &H2E ' "."
                    s = s + Chr(ub)
                Else
                    s = s + Chr(ub)
                End If
            End While
            tbLogRx.AppendText(s)

            l_n = tbLogRx.Lines.Count
            If l_n > LINES_MAX + 100 Then
                Del_Str(tbLogRx, LINES_MAX)
            End If


        End If

        If flag_write_log = True And Len(sw) > 0 Then
            Print(1, sw)
        End If


        'l_n = tbLogRx.Lines.Length ' количество линий

        ' удаляем строки
        'l_n = tbLogRx.Lines.Count

        'tbLogTx.AppendText(Str(l_n) + vbCrLf)

        'If l_n > LINES_MAX Then
        'Dim newList As List(Of String) = tbLogRx.Lines.ToList
        ' newList.RemoveAt(newList.Count - 1)
        ' Remove the first line.  
        ' newList.RemoveAt(0)

        'For l = 0 To l_n - LINES_MAX
        'newList.RemoveAt(0)        ' удаляем самую вернюю строку т.к. после удаления строки сдвигаются в верх
        'Next
        'tbLogRx.Clear()
        'tbLogRx.Lines = newList.ToArray
        'End If

        trx_count_update() ' обновление счетчиков TX RX в строке статуса

    End Sub



    ' смена состояния запись лог файла
    Private Sub cbLogFile_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbLogFile.CheckedChanged
        Dim fname As String = ""

        fname = CreateLogFileName()

        If cbLogFile.Checked = True Then
            flag_write_log = 1
            fname = CreateLogFileName()
            FileOpen(1, fname, OpenMode.Output) ' Создает и открывает файл
        Else
            flag_write_log = 0
            FileClose(1)
        End If
    End Sub


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

    ' Передать строку
    Private Sub btSendString_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btSendString.Click

        Dim d As Integer

        If rbTypeTxWhile.Checked = True Then ' циклическая передача
            If Timer2.Enabled = False Then
                d = Val(tbTxDelay.Text)
                If d = 0 Then
                    d = 1
                ElseIf d > 10000 Then
                    d = 10000
                End If

                Timer2.Interval = d
                Timer2.Enabled = True
                btSendString.Text = SEND_STR_STOP

                gbStringEnd.Enabled = False
                gbTypeTxStr.Enabled = False
                tbStrSend.Enabled = False
            Else
                Timer2.Enabled = False
                btSendString.Text = SEND_STR_START

                gbStringEnd.Enabled = True
                gbTypeTxStr.Enabled = True
                tbStrSend.Enabled = True
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

        s = tbStrSend.Text

        ' копируем строку в промежуточный буфер
        buf_str_tx_n = 0
        For i = 1 To Len(s)
            buf_str_tx(buf_str_tx_n) = Mid(tbStrSend.Text, i, 1)
            buf_str_tx_n = buf_str_tx_n + 1
        Next

        ' добавить символы в конец строки (-, 0D, 0D0A, 00)
        For Each rba In rbAddEndStr_array
            If rba.rb.Checked = True Then
                If Len(rba.s) <> 0 Then
                    For i = 1 To Len(rba.s)
                        c = Mid(rba.s, i, 1)
                        buf_str_tx(buf_str_tx_n) = AscW(c)
                        buf_str_tx_n = buf_str_tx_n + 1
                    Next
                End If
                Exit For
            End If
        Next

        ' Вывод строки в виде HEX / TEXT
        If cbPrintHex.Checked = True Then
            s_hex = ""
            s = ""
            For i = 0 To buf_str_tx_n - 1 ' выводим строку в виде НЕХ -------------------------------------
                s_hex = Hex(buf_str_tx(i))
                If Len(s_hex) = 1 Then
                    s_hex = "0" + s_hex
                End If
                s = s + " " + s_hex
            Next
            tbLogTx.AppendText(s)
        Else
            For i = 0 To buf_str_tx_n - 1 ' выводим строку по символьно в поле ----------------------------
                s_hex = Convert.ToString(buf_str_tx(i))
                tbLogTx.AppendText(s_hex)
            Next
        End If

        SerialPort1.Write(buf_str_tx, 0, buf_str_tx_n)

        rx_counter_global = rx_counter_global + buf_str_tx_n
        trx_count_update() ' обновление счетчиков TX RX в строке статуса

    End Sub

    Private Sub btFileSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btFileSend.Click
        Dim fs As FileStream
        Const BUF_SIZE = 1024
        Dim buf(BUF_SIZE) As Byte
        Dim res As Integer = 0
        Dim f As IO.FileInfo
        Dim file_size As Long = 0
        Dim file_count As Long = 0

        tspbBar.Value = 0

        OpenFileDialog1.InitialDirectory = "c:\"
        OpenFileDialog1.Filter = "All files (*.*)|*.*"
        OpenFileDialog1.FilterIndex = 2
        OpenFileDialog1.RestoreDirectory = True

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

            f = New IO.FileInfo(OpenFileDialog1.FileName)
            file_size = f.Length   ' длинна файла нужна чтобы знать сколько всего байт передавать и расчитывать проценты
            If file_size = 0 Then
                Exit Sub
            End If

            fs = New FileStream(OpenFileDialog1.FileName, FileMode.Open, FileAccess.Read)

next_:
            res = fs.Read(buf, 0, BUF_SIZE)

            If res = 0 Then ' все передали выходим
                Exit Sub
            End If

            SerialPort1.Write(buf, 0, res)

            'Thread.Sleep(80)

            tx_counter_global = tx_counter_global + res
            trx_count_update() ' обновление счетчиков TX RX в строке статуса

            file_count = file_count + res

            tspbBar.Value = file_count * 100 / file_size

            GoTo next_

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

        tbLogTx.AppendText(Str(l_n) + vbCrLf)

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
End Class
