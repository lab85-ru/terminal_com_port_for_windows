﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.cbPorts = New System.Windows.Forms.ComboBox
        Me.btPOpen = New System.Windows.Forms.Button
        Me.btScanComPort = New System.Windows.Forms.Button
        Me.gbRxLog = New System.Windows.Forms.GroupBox
        Me.tbLogRx = New System.Windows.Forms.TextBox
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.gbSetPortSpeed = New System.Windows.Forms.GroupBox
        Me.rbSpeedNumer = New System.Windows.Forms.RadioButton
        Me.rbSpeed115200 = New System.Windows.Forms.RadioButton
        Me.rbSpeed57600 = New System.Windows.Forms.RadioButton
        Me.rbSpeed38400 = New System.Windows.Forms.RadioButton
        Me.rbSpeed19200 = New System.Windows.Forms.RadioButton
        Me.rbSpeed9600 = New System.Windows.Forms.RadioButton
        Me.rbSpeed4800 = New System.Windows.Forms.RadioButton
        Me.tbPortSpeedNumer = New System.Windows.Forms.TextBox
        Me.gbSetPortParity = New System.Windows.Forms.GroupBox
        Me.rbParitySpace = New System.Windows.Forms.RadioButton
        Me.rbParityMark = New System.Windows.Forms.RadioButton
        Me.rbParityEven = New System.Windows.Forms.RadioButton
        Me.rbParityOdd = New System.Windows.Forms.RadioButton
        Me.rbParityNo = New System.Windows.Forms.RadioButton
        Me.gbSetPortStopBit = New System.Windows.Forms.GroupBox
        Me.rbStopBit15 = New System.Windows.Forms.RadioButton
        Me.rbStopBit2 = New System.Windows.Forms.RadioButton
        Me.rbStopBit1 = New System.Windows.Forms.RadioButton
        Me.gbLogFile = New System.Windows.Forms.GroupBox
        Me.cbLogFile = New System.Windows.Forms.CheckBox
        Me.gbPrint = New System.Windows.Forms.GroupBox
        Me.cbPrintHex = New System.Windows.Forms.CheckBox
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.gbTx = New System.Windows.Forms.GroupBox
        Me.gbTypeTxStr = New System.Windows.Forms.GroupBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.tbTxDelay = New System.Windows.Forms.TextBox
        Me.rbTypeTxWhile = New System.Windows.Forms.RadioButton
        Me.rbTypeTx1 = New System.Windows.Forms.RadioButton
        Me.gbStringEnd = New System.Windows.Forms.GroupBox
        Me.rbAddStrEnd00 = New System.Windows.Forms.RadioButton
        Me.rbAddStrEnd0D0A = New System.Windows.Forms.RadioButton
        Me.rbAddStrEnd0D = New System.Windows.Forms.RadioButton
        Me.rbAddStrEndClear = New System.Windows.Forms.RadioButton
        Me.tbStrSend = New System.Windows.Forms.TextBox
        Me.btSendString = New System.Windows.Forms.Button
        Me.btFileSend = New System.Windows.Forms.Button
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.gbTxLog = New System.Windows.Forms.GroupBox
        Me.tbLogTx = New System.Windows.Forms.TextBox
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.tsslRxCounter = New System.Windows.Forms.ToolStripStatusLabel
        Me.tsslTxCounter = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspbBar = New System.Windows.Forms.ToolStripProgressBar
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.btClearTxLog = New System.Windows.Forms.Button
        Me.btClearRxLog = New System.Windows.Forms.Button
        Me.GroupBox1.SuspendLayout()
        Me.gbRxLog.SuspendLayout()
        Me.gbSetPortSpeed.SuspendLayout()
        Me.gbSetPortParity.SuspendLayout()
        Me.gbSetPortStopBit.SuspendLayout()
        Me.gbLogFile.SuspendLayout()
        Me.gbPrint.SuspendLayout()
        Me.gbTx.SuspendLayout()
        Me.gbTypeTxStr.SuspendLayout()
        Me.gbStringEnd.SuspendLayout()
        Me.gbTxLog.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cbPorts)
        Me.GroupBox1.Controls.Add(Me.btPOpen)
        Me.GroupBox1.Controls.Add(Me.btScanComPort)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(76, 94)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Настройки"
        '
        'cbPorts
        '
        Me.cbPorts.FormattingEnabled = True
        Me.cbPorts.Location = New System.Drawing.Point(8, 42)
        Me.cbPorts.Name = "cbPorts"
        Me.cbPorts.Size = New System.Drawing.Size(58, 21)
        Me.cbPorts.TabIndex = 2
        '
        'btPOpen
        '
        Me.btPOpen.Location = New System.Drawing.Point(8, 65)
        Me.btPOpen.Name = "btPOpen"
        Me.btPOpen.Size = New System.Drawing.Size(58, 23)
        Me.btPOpen.TabIndex = 1
        Me.btPOpen.Text = "Открыть"
        Me.btPOpen.UseVisualStyleBackColor = True
        '
        'btScanComPort
        '
        Me.btScanComPort.Location = New System.Drawing.Point(8, 15)
        Me.btScanComPort.Name = "btScanComPort"
        Me.btScanComPort.Size = New System.Drawing.Size(58, 23)
        Me.btScanComPort.TabIndex = 0
        Me.btScanComPort.Text = "Поиск"
        Me.btScanComPort.UseVisualStyleBackColor = True
        '
        'gbRxLog
        '
        Me.gbRxLog.Controls.Add(Me.btClearRxLog)
        Me.gbRxLog.Controls.Add(Me.tbLogRx)
        Me.gbRxLog.Location = New System.Drawing.Point(12, 263)
        Me.gbRxLog.Name = "gbRxLog"
        Me.gbRxLog.Size = New System.Drawing.Size(771, 507)
        Me.gbRxLog.TabIndex = 1
        Me.gbRxLog.TabStop = False
        Me.gbRxLog.Text = "Прием"
        '
        'tbLogRx
        '
        Me.tbLogRx.Font = New System.Drawing.Font("Lucida Console", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbLogRx.Location = New System.Drawing.Point(8, 19)
        Me.tbLogRx.MaxLength = 4096
        Me.tbLogRx.Multiline = True
        Me.tbLogRx.Name = "tbLogRx"
        Me.tbLogRx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.tbLogRx.Size = New System.Drawing.Size(760, 482)
        Me.tbLogRx.TabIndex = 0
        '
        'SerialPort1
        '
        '
        'gbSetPortSpeed
        '
        Me.gbSetPortSpeed.Controls.Add(Me.rbSpeedNumer)
        Me.gbSetPortSpeed.Controls.Add(Me.rbSpeed115200)
        Me.gbSetPortSpeed.Controls.Add(Me.rbSpeed57600)
        Me.gbSetPortSpeed.Controls.Add(Me.rbSpeed38400)
        Me.gbSetPortSpeed.Controls.Add(Me.rbSpeed19200)
        Me.gbSetPortSpeed.Controls.Add(Me.rbSpeed9600)
        Me.gbSetPortSpeed.Controls.Add(Me.rbSpeed4800)
        Me.gbSetPortSpeed.Controls.Add(Me.tbPortSpeedNumer)
        Me.gbSetPortSpeed.Location = New System.Drawing.Point(94, 8)
        Me.gbSetPortSpeed.Name = "gbSetPortSpeed"
        Me.gbSetPortSpeed.Size = New System.Drawing.Size(194, 93)
        Me.gbSetPortSpeed.TabIndex = 2
        Me.gbSetPortSpeed.TabStop = False
        Me.gbSetPortSpeed.Text = "Скорость"
        '
        'rbSpeedNumer
        '
        Me.rbSpeedNumer.AutoSize = True
        Me.rbSpeedNumer.Location = New System.Drawing.Point(116, 19)
        Me.rbSpeedNumer.Name = "rbSpeedNumer"
        Me.rbSpeedNumer.Size = New System.Drawing.Size(73, 17)
        Me.rbSpeedNumer.TabIndex = 7
        Me.rbSpeedNumer.TabStop = True
        Me.rbSpeedNumer.Text = "Значение"
        Me.rbSpeedNumer.UseVisualStyleBackColor = True
        '
        'rbSpeed115200
        '
        Me.rbSpeed115200.AutoSize = True
        Me.rbSpeed115200.Location = New System.Drawing.Point(61, 65)
        Me.rbSpeed115200.Name = "rbSpeed115200"
        Me.rbSpeed115200.Size = New System.Drawing.Size(61, 17)
        Me.rbSpeed115200.TabIndex = 6
        Me.rbSpeed115200.TabStop = True
        Me.rbSpeed115200.Text = "115200"
        Me.rbSpeed115200.UseVisualStyleBackColor = True
        '
        'rbSpeed57600
        '
        Me.rbSpeed57600.AutoSize = True
        Me.rbSpeed57600.Location = New System.Drawing.Point(61, 42)
        Me.rbSpeed57600.Name = "rbSpeed57600"
        Me.rbSpeed57600.Size = New System.Drawing.Size(55, 17)
        Me.rbSpeed57600.TabIndex = 5
        Me.rbSpeed57600.TabStop = True
        Me.rbSpeed57600.Text = "57600"
        Me.rbSpeed57600.UseVisualStyleBackColor = True
        '
        'rbSpeed38400
        '
        Me.rbSpeed38400.AutoSize = True
        Me.rbSpeed38400.Location = New System.Drawing.Point(61, 19)
        Me.rbSpeed38400.Name = "rbSpeed38400"
        Me.rbSpeed38400.Size = New System.Drawing.Size(55, 17)
        Me.rbSpeed38400.TabIndex = 4
        Me.rbSpeed38400.TabStop = True
        Me.rbSpeed38400.Text = "38400"
        Me.rbSpeed38400.UseVisualStyleBackColor = True
        '
        'rbSpeed19200
        '
        Me.rbSpeed19200.AutoSize = True
        Me.rbSpeed19200.Location = New System.Drawing.Point(6, 65)
        Me.rbSpeed19200.Name = "rbSpeed19200"
        Me.rbSpeed19200.Size = New System.Drawing.Size(55, 17)
        Me.rbSpeed19200.TabIndex = 3
        Me.rbSpeed19200.TabStop = True
        Me.rbSpeed19200.Text = "19200"
        Me.rbSpeed19200.UseVisualStyleBackColor = True
        '
        'rbSpeed9600
        '
        Me.rbSpeed9600.AutoSize = True
        Me.rbSpeed9600.Location = New System.Drawing.Point(6, 42)
        Me.rbSpeed9600.Name = "rbSpeed9600"
        Me.rbSpeed9600.Size = New System.Drawing.Size(49, 17)
        Me.rbSpeed9600.TabIndex = 2
        Me.rbSpeed9600.TabStop = True
        Me.rbSpeed9600.Text = "9600"
        Me.rbSpeed9600.UseVisualStyleBackColor = True
        '
        'rbSpeed4800
        '
        Me.rbSpeed4800.AutoSize = True
        Me.rbSpeed4800.Location = New System.Drawing.Point(6, 19)
        Me.rbSpeed4800.Name = "rbSpeed4800"
        Me.rbSpeed4800.Size = New System.Drawing.Size(49, 17)
        Me.rbSpeed4800.TabIndex = 1
        Me.rbSpeed4800.TabStop = True
        Me.rbSpeed4800.Text = "4800"
        Me.rbSpeed4800.UseVisualStyleBackColor = True
        '
        'tbPortSpeedNumer
        '
        Me.tbPortSpeedNumer.Location = New System.Drawing.Point(122, 42)
        Me.tbPortSpeedNumer.Name = "tbPortSpeedNumer"
        Me.tbPortSpeedNumer.Size = New System.Drawing.Size(65, 20)
        Me.tbPortSpeedNumer.TabIndex = 0
        Me.tbPortSpeedNumer.Text = "0"
        Me.tbPortSpeedNumer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'gbSetPortParity
        '
        Me.gbSetPortParity.Controls.Add(Me.rbParitySpace)
        Me.gbSetPortParity.Controls.Add(Me.rbParityMark)
        Me.gbSetPortParity.Controls.Add(Me.rbParityEven)
        Me.gbSetPortParity.Controls.Add(Me.rbParityOdd)
        Me.gbSetPortParity.Controls.Add(Me.rbParityNo)
        Me.gbSetPortParity.Location = New System.Drawing.Point(294, 7)
        Me.gbSetPortParity.Name = "gbSetPortParity"
        Me.gbSetPortParity.Size = New System.Drawing.Size(105, 93)
        Me.gbSetPortParity.TabIndex = 3
        Me.gbSetPortParity.TabStop = False
        Me.gbSetPortParity.Text = "Четность"
        '
        'rbParitySpace
        '
        Me.rbParitySpace.AutoSize = True
        Me.rbParitySpace.Location = New System.Drawing.Point(52, 42)
        Me.rbParitySpace.Name = "rbParitySpace"
        Me.rbParitySpace.Size = New System.Drawing.Size(54, 17)
        Me.rbParitySpace.TabIndex = 4
        Me.rbParitySpace.TabStop = True
        Me.rbParitySpace.Text = "space"
        Me.rbParitySpace.UseVisualStyleBackColor = True
        '
        'rbParityMark
        '
        Me.rbParityMark.AutoSize = True
        Me.rbParityMark.Location = New System.Drawing.Point(52, 19)
        Me.rbParityMark.Name = "rbParityMark"
        Me.rbParityMark.Size = New System.Drawing.Size(48, 17)
        Me.rbParityMark.TabIndex = 3
        Me.rbParityMark.TabStop = True
        Me.rbParityMark.Text = "mark"
        Me.rbParityMark.UseVisualStyleBackColor = True
        '
        'rbParityEven
        '
        Me.rbParityEven.AutoSize = True
        Me.rbParityEven.Location = New System.Drawing.Point(6, 65)
        Me.rbParityEven.Name = "rbParityEven"
        Me.rbParityEven.Size = New System.Drawing.Size(49, 17)
        Me.rbParityEven.TabIndex = 2
        Me.rbParityEven.TabStop = True
        Me.rbParityEven.Text = "even"
        Me.rbParityEven.UseVisualStyleBackColor = True
        '
        'rbParityOdd
        '
        Me.rbParityOdd.AutoSize = True
        Me.rbParityOdd.Location = New System.Drawing.Point(6, 42)
        Me.rbParityOdd.Name = "rbParityOdd"
        Me.rbParityOdd.Size = New System.Drawing.Size(43, 17)
        Me.rbParityOdd.TabIndex = 1
        Me.rbParityOdd.TabStop = True
        Me.rbParityOdd.Text = "odd"
        Me.rbParityOdd.UseVisualStyleBackColor = True
        '
        'rbParityNo
        '
        Me.rbParityNo.AutoSize = True
        Me.rbParityNo.Location = New System.Drawing.Point(6, 19)
        Me.rbParityNo.Name = "rbParityNo"
        Me.rbParityNo.Size = New System.Drawing.Size(47, 17)
        Me.rbParityNo.TabIndex = 0
        Me.rbParityNo.TabStop = True
        Me.rbParityNo.Text = "НЕТ"
        Me.rbParityNo.UseVisualStyleBackColor = True
        '
        'gbSetPortStopBit
        '
        Me.gbSetPortStopBit.Controls.Add(Me.rbStopBit15)
        Me.gbSetPortStopBit.Controls.Add(Me.rbStopBit2)
        Me.gbSetPortStopBit.Controls.Add(Me.rbStopBit1)
        Me.gbSetPortStopBit.Location = New System.Drawing.Point(406, 8)
        Me.gbSetPortStopBit.Name = "gbSetPortStopBit"
        Me.gbSetPortStopBit.Size = New System.Drawing.Size(68, 93)
        Me.gbSetPortStopBit.TabIndex = 4
        Me.gbSetPortStopBit.TabStop = False
        Me.gbSetPortStopBit.Text = "Стоп бит"
        '
        'rbStopBit15
        '
        Me.rbStopBit15.AutoSize = True
        Me.rbStopBit15.Location = New System.Drawing.Point(6, 66)
        Me.rbStopBit15.Name = "rbStopBit15"
        Me.rbStopBit15.Size = New System.Drawing.Size(40, 17)
        Me.rbStopBit15.TabIndex = 2
        Me.rbStopBit15.TabStop = True
        Me.rbStopBit15.Text = "1.5"
        Me.rbStopBit15.UseVisualStyleBackColor = True
        '
        'rbStopBit2
        '
        Me.rbStopBit2.AutoSize = True
        Me.rbStopBit2.Location = New System.Drawing.Point(6, 43)
        Me.rbStopBit2.Name = "rbStopBit2"
        Me.rbStopBit2.Size = New System.Drawing.Size(31, 17)
        Me.rbStopBit2.TabIndex = 1
        Me.rbStopBit2.TabStop = True
        Me.rbStopBit2.Text = "2"
        Me.rbStopBit2.UseVisualStyleBackColor = True
        '
        'rbStopBit1
        '
        Me.rbStopBit1.AutoSize = True
        Me.rbStopBit1.Location = New System.Drawing.Point(6, 19)
        Me.rbStopBit1.Name = "rbStopBit1"
        Me.rbStopBit1.Size = New System.Drawing.Size(31, 17)
        Me.rbStopBit1.TabIndex = 0
        Me.rbStopBit1.TabStop = True
        Me.rbStopBit1.Text = "1"
        Me.rbStopBit1.UseVisualStyleBackColor = True
        '
        'gbLogFile
        '
        Me.gbLogFile.Controls.Add(Me.cbLogFile)
        Me.gbLogFile.Location = New System.Drawing.Point(480, 10)
        Me.gbLogFile.Name = "gbLogFile"
        Me.gbLogFile.Size = New System.Drawing.Size(84, 44)
        Me.gbLogFile.TabIndex = 5
        Me.gbLogFile.TabStop = False
        Me.gbLogFile.Text = "Лог файл"
        '
        'cbLogFile
        '
        Me.cbLogFile.AutoSize = True
        Me.cbLogFile.Location = New System.Drawing.Point(6, 22)
        Me.cbLogFile.Name = "cbLogFile"
        Me.cbLogFile.Size = New System.Drawing.Size(74, 17)
        Me.cbLogFile.TabIndex = 0
        Me.cbLogFile.Text = "Записать"
        Me.cbLogFile.UseVisualStyleBackColor = True
        '
        'gbPrint
        '
        Me.gbPrint.Controls.Add(Me.cbPrintHex)
        Me.gbPrint.Location = New System.Drawing.Point(480, 60)
        Me.gbPrint.Name = "gbPrint"
        Me.gbPrint.Size = New System.Drawing.Size(84, 40)
        Me.gbPrint.TabIndex = 6
        Me.gbPrint.TabStop = False
        Me.gbPrint.Text = "Тип вывода"
        '
        'cbPrintHex
        '
        Me.cbPrintHex.AutoSize = True
        Me.cbPrintHex.Location = New System.Drawing.Point(6, 22)
        Me.cbPrintHex.Name = "cbPrintHex"
        Me.cbPrintHex.Size = New System.Drawing.Size(74, 17)
        Me.cbPrintHex.TabIndex = 0
        Me.cbPrintHex.Text = "ascii/HEX"
        Me.cbPrintHex.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        '
        'gbTx
        '
        Me.gbTx.Controls.Add(Me.gbTypeTxStr)
        Me.gbTx.Controls.Add(Me.gbStringEnd)
        Me.gbTx.Controls.Add(Me.tbStrSend)
        Me.gbTx.Location = New System.Drawing.Point(570, 10)
        Me.gbTx.Name = "gbTx"
        Me.gbTx.Size = New System.Drawing.Size(419, 90)
        Me.gbTx.TabIndex = 7
        Me.gbTx.TabStop = False
        Me.gbTx.Text = "Передать строку"
        '
        'gbTypeTxStr
        '
        Me.gbTypeTxStr.Controls.Add(Me.Label1)
        Me.gbTypeTxStr.Controls.Add(Me.tbTxDelay)
        Me.gbTypeTxStr.Controls.Add(Me.rbTypeTxWhile)
        Me.gbTypeTxStr.Controls.Add(Me.rbTypeTx1)
        Me.gbTypeTxStr.Location = New System.Drawing.Point(201, 16)
        Me.gbTypeTxStr.Name = "gbTypeTxStr"
        Me.gbTypeTxStr.Size = New System.Drawing.Size(208, 37)
        Me.gbTypeTxStr.TabIndex = 3
        Me.gbTypeTxStr.TabStop = False
        Me.gbTypeTxStr.Text = "Тип передачи"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(181, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(21, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "мс"
        '
        'tbTxDelay
        '
        Me.tbTxDelay.Location = New System.Drawing.Point(144, 13)
        Me.tbTxDelay.Name = "tbTxDelay"
        Me.tbTxDelay.Size = New System.Drawing.Size(38, 20)
        Me.tbTxDelay.TabIndex = 2
        Me.tbTxDelay.Text = "0"
        Me.tbTxDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'rbTypeTxWhile
        '
        Me.rbTypeTxWhile.AutoSize = True
        Me.rbTypeTxWhile.Location = New System.Drawing.Point(64, 14)
        Me.rbTypeTxWhile.Name = "rbTypeTxWhile"
        Me.rbTypeTxWhile.Size = New System.Drawing.Size(62, 17)
        Me.rbTypeTxWhile.TabIndex = 1
        Me.rbTypeTxWhile.TabStop = True
        Me.rbTypeTxWhile.Text = "Повтор"
        Me.rbTypeTxWhile.UseVisualStyleBackColor = True
        '
        'rbTypeTx1
        '
        Me.rbTypeTx1.AutoSize = True
        Me.rbTypeTx1.Location = New System.Drawing.Point(6, 14)
        Me.rbTypeTx1.Name = "rbTypeTx1"
        Me.rbTypeTx1.Size = New System.Drawing.Size(52, 17)
        Me.rbTypeTx1.TabIndex = 0
        Me.rbTypeTx1.TabStop = True
        Me.rbTypeTx1.Text = "1 раз"
        Me.rbTypeTx1.UseVisualStyleBackColor = True
        '
        'gbStringEnd
        '
        Me.gbStringEnd.Controls.Add(Me.rbAddStrEnd00)
        Me.gbStringEnd.Controls.Add(Me.rbAddStrEnd0D0A)
        Me.gbStringEnd.Controls.Add(Me.rbAddStrEnd0D)
        Me.gbStringEnd.Controls.Add(Me.rbAddStrEndClear)
        Me.gbStringEnd.Location = New System.Drawing.Point(6, 16)
        Me.gbStringEnd.Name = "gbStringEnd"
        Me.gbStringEnd.Size = New System.Drawing.Size(189, 37)
        Me.gbStringEnd.TabIndex = 2
        Me.gbStringEnd.TabStop = False
        Me.gbStringEnd.Text = "Добавить в конец строки"
        '
        'rbAddStrEnd00
        '
        Me.rbAddStrEnd00.AutoSize = True
        Me.rbAddStrEnd00.Location = New System.Drawing.Point(149, 14)
        Me.rbAddStrEnd00.Name = "rbAddStrEnd00"
        Me.rbAddStrEnd00.Size = New System.Drawing.Size(37, 17)
        Me.rbAddStrEnd00.TabIndex = 3
        Me.rbAddStrEnd00.TabStop = True
        Me.rbAddStrEnd00.Text = "00"
        Me.rbAddStrEnd00.UseVisualStyleBackColor = True
        '
        'rbAddStrEnd0D0A
        '
        Me.rbAddStrEnd0D0A.AutoSize = True
        Me.rbAddStrEnd0D0A.Location = New System.Drawing.Point(85, 14)
        Me.rbAddStrEnd0D0A.Name = "rbAddStrEnd0D0A"
        Me.rbAddStrEnd0D0A.Size = New System.Drawing.Size(58, 17)
        Me.rbAddStrEnd0D0A.TabIndex = 2
        Me.rbAddStrEnd0D0A.TabStop = True
        Me.rbAddStrEnd0D0A.Text = "0D+0A"
        Me.rbAddStrEnd0D0A.UseVisualStyleBackColor = True
        '
        'rbAddStrEnd0D
        '
        Me.rbAddStrEnd0D.AutoSize = True
        Me.rbAddStrEnd0D.Location = New System.Drawing.Point(40, 14)
        Me.rbAddStrEnd0D.Name = "rbAddStrEnd0D"
        Me.rbAddStrEnd0D.Size = New System.Drawing.Size(39, 17)
        Me.rbAddStrEnd0D.TabIndex = 1
        Me.rbAddStrEnd0D.TabStop = True
        Me.rbAddStrEnd0D.Text = "0D"
        Me.rbAddStrEnd0D.UseVisualStyleBackColor = True
        '
        'rbAddStrEndClear
        '
        Me.rbAddStrEndClear.AutoSize = True
        Me.rbAddStrEndClear.Location = New System.Drawing.Point(6, 14)
        Me.rbAddStrEndClear.Name = "rbAddStrEndClear"
        Me.rbAddStrEndClear.Size = New System.Drawing.Size(28, 17)
        Me.rbAddStrEndClear.TabIndex = 0
        Me.rbAddStrEndClear.TabStop = True
        Me.rbAddStrEndClear.Text = "-"
        Me.rbAddStrEndClear.UseVisualStyleBackColor = True
        '
        'tbStrSend
        '
        Me.tbStrSend.Location = New System.Drawing.Point(6, 59)
        Me.tbStrSend.Name = "tbStrSend"
        Me.tbStrSend.Size = New System.Drawing.Size(403, 20)
        Me.tbStrSend.TabIndex = 1
        '
        'btSendString
        '
        Me.btSendString.Location = New System.Drawing.Point(115, 19)
        Me.btSendString.Name = "btSendString"
        Me.btSendString.Size = New System.Drawing.Size(75, 23)
        Me.btSendString.TabIndex = 4
        Me.btSendString.Text = "Строка"
        Me.btSendString.UseVisualStyleBackColor = True
        '
        'btFileSend
        '
        Me.btFileSend.Location = New System.Drawing.Point(6, 19)
        Me.btFileSend.Name = "btFileSend"
        Me.btFileSend.Size = New System.Drawing.Size(75, 23)
        Me.btFileSend.TabIndex = 3
        Me.btFileSend.Text = "Файл"
        Me.btFileSend.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'gbTxLog
        '
        Me.gbTxLog.Controls.Add(Me.btClearTxLog)
        Me.gbTxLog.Controls.Add(Me.tbLogTx)
        Me.gbTxLog.Location = New System.Drawing.Point(12, 107)
        Me.gbTxLog.Name = "gbTxLog"
        Me.gbTxLog.Size = New System.Drawing.Size(771, 150)
        Me.gbTxLog.TabIndex = 8
        Me.gbTxLog.TabStop = False
        Me.gbTxLog.Text = "Передача"
        '
        'tbLogTx
        '
        Me.tbLogTx.Font = New System.Drawing.Font("Lucida Console", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.tbLogTx.Location = New System.Drawing.Point(8, 19)
        Me.tbLogTx.Multiline = True
        Me.tbLogTx.Name = "tbLogTx"
        Me.tbLogTx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.tbLogTx.Size = New System.Drawing.Size(757, 122)
        Me.tbLogTx.TabIndex = 0
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsslRxCounter, Me.tsslTxCounter, Me.tspbBar})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 774)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(997, 22)
        Me.StatusStrip1.TabIndex = 9
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'tsslRxCounter
        '
        Me.tsslRxCounter.Name = "tsslRxCounter"
        Me.tsslRxCounter.Size = New System.Drawing.Size(24, 17)
        Me.tsslRxCounter.Text = "RX:"
        '
        'tsslTxCounter
        '
        Me.tsslTxCounter.Name = "tsslTxCounter"
        Me.tsslTxCounter.Size = New System.Drawing.Size(24, 17)
        Me.tsslTxCounter.Text = "TX:"
        '
        'tspbBar
        '
        Me.tspbBar.Name = "tspbBar"
        Me.tspbBar.Size = New System.Drawing.Size(100, 16)
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btSendString)
        Me.GroupBox2.Controls.Add(Me.btFileSend)
        Me.GroupBox2.Location = New System.Drawing.Point(789, 107)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(200, 150)
        Me.GroupBox2.TabIndex = 10
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Передача"
        '
        'Timer2
        '
        '
        'btClearTxLog
        '
        Me.btClearTxLog.Location = New System.Drawing.Point(660, 0)
        Me.btClearTxLog.Name = "btClearTxLog"
        Me.btClearTxLog.Size = New System.Drawing.Size(75, 23)
        Me.btClearTxLog.TabIndex = 1
        Me.btClearTxLog.Text = "Очистка"
        Me.btClearTxLog.UseVisualStyleBackColor = True
        '
        'btClearRxLog
        '
        Me.btClearRxLog.Location = New System.Drawing.Point(660, 0)
        Me.btClearRxLog.Name = "btClearRxLog"
        Me.btClearRxLog.Size = New System.Drawing.Size(75, 23)
        Me.btClearRxLog.TabIndex = 2
        Me.btClearRxLog.Text = "Очистка"
        Me.btClearRxLog.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(997, 796)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.gbTxLog)
        Me.Controls.Add(Me.gbTx)
        Me.Controls.Add(Me.gbPrint)
        Me.Controls.Add(Me.gbLogFile)
        Me.Controls.Add(Me.gbSetPortStopBit)
        Me.Controls.Add(Me.gbSetPortParity)
        Me.Controls.Add(Me.gbSetPortSpeed)
        Me.Controls.Add(Me.gbRxLog)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Терминал"
        Me.GroupBox1.ResumeLayout(False)
        Me.gbRxLog.ResumeLayout(False)
        Me.gbRxLog.PerformLayout()
        Me.gbSetPortSpeed.ResumeLayout(False)
        Me.gbSetPortSpeed.PerformLayout()
        Me.gbSetPortParity.ResumeLayout(False)
        Me.gbSetPortParity.PerformLayout()
        Me.gbSetPortStopBit.ResumeLayout(False)
        Me.gbSetPortStopBit.PerformLayout()
        Me.gbLogFile.ResumeLayout(False)
        Me.gbLogFile.PerformLayout()
        Me.gbPrint.ResumeLayout(False)
        Me.gbPrint.PerformLayout()
        Me.gbTx.ResumeLayout(False)
        Me.gbTx.PerformLayout()
        Me.gbTypeTxStr.ResumeLayout(False)
        Me.gbTypeTxStr.PerformLayout()
        Me.gbStringEnd.ResumeLayout(False)
        Me.gbStringEnd.PerformLayout()
        Me.gbTxLog.ResumeLayout(False)
        Me.gbTxLog.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents gbRxLog As System.Windows.Forms.GroupBox
    Friend WithEvents tbLogRx As System.Windows.Forms.TextBox
    Friend WithEvents btScanComPort As System.Windows.Forms.Button
    Friend WithEvents SerialPort1 As System.IO.Ports.SerialPort
    Friend WithEvents btPOpen As System.Windows.Forms.Button
    Friend WithEvents cbPorts As System.Windows.Forms.ComboBox
    Friend WithEvents gbSetPortSpeed As System.Windows.Forms.GroupBox
    Friend WithEvents rbSpeedNumer As System.Windows.Forms.RadioButton
    Friend WithEvents rbSpeed115200 As System.Windows.Forms.RadioButton
    Friend WithEvents rbSpeed57600 As System.Windows.Forms.RadioButton
    Friend WithEvents rbSpeed38400 As System.Windows.Forms.RadioButton
    Friend WithEvents rbSpeed19200 As System.Windows.Forms.RadioButton
    Friend WithEvents rbSpeed9600 As System.Windows.Forms.RadioButton
    Friend WithEvents rbSpeed4800 As System.Windows.Forms.RadioButton
    Friend WithEvents tbPortSpeedNumer As System.Windows.Forms.TextBox
    Friend WithEvents gbSetPortParity As System.Windows.Forms.GroupBox
    Friend WithEvents rbParitySpace As System.Windows.Forms.RadioButton
    Friend WithEvents rbParityMark As System.Windows.Forms.RadioButton
    Friend WithEvents rbParityEven As System.Windows.Forms.RadioButton
    Friend WithEvents rbParityOdd As System.Windows.Forms.RadioButton
    Friend WithEvents rbParityNo As System.Windows.Forms.RadioButton
    Friend WithEvents gbSetPortStopBit As System.Windows.Forms.GroupBox
    Friend WithEvents rbStopBit15 As System.Windows.Forms.RadioButton
    Friend WithEvents rbStopBit2 As System.Windows.Forms.RadioButton
    Friend WithEvents rbStopBit1 As System.Windows.Forms.RadioButton
    Friend WithEvents gbLogFile As System.Windows.Forms.GroupBox
    Friend WithEvents cbLogFile As System.Windows.Forms.CheckBox
    Friend WithEvents gbPrint As System.Windows.Forms.GroupBox
    Friend WithEvents cbPrintHex As System.Windows.Forms.CheckBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents gbTx As System.Windows.Forms.GroupBox
    Friend WithEvents tbStrSend As System.Windows.Forms.TextBox
    Friend WithEvents gbStringEnd As System.Windows.Forms.GroupBox
    Friend WithEvents btFileSend As System.Windows.Forms.Button
    Friend WithEvents rbAddStrEndClear As System.Windows.Forms.RadioButton
    Friend WithEvents btSendString As System.Windows.Forms.Button
    Friend WithEvents rbAddStrEnd00 As System.Windows.Forms.RadioButton
    Friend WithEvents rbAddStrEnd0D0A As System.Windows.Forms.RadioButton
    Friend WithEvents rbAddStrEnd0D As System.Windows.Forms.RadioButton
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents gbTxLog As System.Windows.Forms.GroupBox
    Friend WithEvents tbLogTx As System.Windows.Forms.TextBox
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents tsslRxCounter As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tsslTxCounter As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspbBar As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents gbTypeTxStr As System.Windows.Forms.GroupBox
    Friend WithEvents tbTxDelay As System.Windows.Forms.TextBox
    Friend WithEvents rbTypeTxWhile As System.Windows.Forms.RadioButton
    Friend WithEvents rbTypeTx1 As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Timer2 As System.Windows.Forms.Timer
    Friend WithEvents btClearTxLog As System.Windows.Forms.Button
    Friend WithEvents btClearRxLog As System.Windows.Forms.Button

End Class