﻿Imports System
Imports System.IO
Imports System.IO.Ports

'
' Необходимо реализовать:
' - передачу строки в нех кодировка как в фаре
' - прием ESC последовательностей



Public Class Form1

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

    Dim f_send_st As file_send_st

    ' Поиск СОМ портов в системе
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

            If com_port_speed_int = 0 Or (com_port_speed_int < 110 And com_port_speed_int > 256000) Then
                MsgBox("ОШИБКА: Значение параметра скорость задано не верно !" + vbCrLf + "MIN=110 MAX=256000")
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


            tbLogRx.AppendText(vbCrLf + "Порт Открыт." + vbCrLf)
        ElseIf CPortStatus = port_status_e.open Then ' Закрываем СОМ порт -------------------------------------
            ComPortClose()

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
            tbStrSend.Enabled = True
            btFileSend.Enabled = True

            CPortStatus = port_status_e.close
            btPOpen.Text = PORT_OPEN
            tbLogRx.AppendText(vbCrLf + "Порт == закрыт ===." + vbCrLf)
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

        rbStopBit_array(1).rb = rbStopBit2
        rbStopBit_array(1).s = ONE5STOPBITS

        rbStopBit_array(2).rb = rbStopBit15
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

        If CPortStatus = port_status_e.close Then
            Exit Sub
        End If

        ' времы выполнения
        'Dim TStart As Date = Now
        'tbLogTx.AppendText(Now.Subtract(TStart).TotalMilliseconds.ToString & " ms" & vbCrLf)

        ' ----- ПРИЕМ ДАННЫХ -----------
        din = BUFIN_SIZE
        ComPortRead(bufin, din)
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
                    Case &H0
                        ub = &H2E ' за место кода 00 выводим точку "."
                        s_out = s_out + Chr(ub)

                    Case &HD
                        s_out = s_out + vbCrLf

                    Case &HA
                        If cb0D0A_one.Checked = True Then
                            s_out = s_out + ""
                        Else
                            s_out = s_out + vbCrLf
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

    ' Событие нажата кнопка перечачи строки в порт
    ' Передать строку
    Private Sub btSendString_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btSendString.Click

        Dim d As Integer

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
                tbStrSend.Enabled = False
                btFileSend.Enabled = False
            Else
                Timer2.Enabled = False
                btSendString.Text = SEND_STR_START

                gbStringEnd.Enabled = True
                gbTypeTxStr.Enabled = True
                tbStrSend.Enabled = True
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

        s = tbStrSend.Text

        ' копируем строку в промежуточный буфер
        buf_str_tx_n = 0
        For i = 1 To Len(s)
            c = Mid(tbStrSend.Text, i, 1)
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
            tbStrSend.Enabled = True
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
            tbStrSend.Enabled = False
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
            tbStrSend.Enabled = True
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

End Class
