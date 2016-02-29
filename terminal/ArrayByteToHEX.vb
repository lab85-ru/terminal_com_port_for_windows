Module ArrayByteToHEX

    ' Конвертирует массив байт в строкИ в виде НЕХ
    Function ConvArrayByteToHEX(ByVal buf() As Byte, ByVal len As Integer) As String
        Dim ub As Byte          ' Сам байт из входного массива
        Dim b_i As Integer = 0  ' Счетчик входных прочитаных байт 
        Dim sc As String = ""   ' строка 1 НЕХ значение
        Dim s As String = ""    ' выходная строка
        Dim sl_n As Integer = 0 ' Счетчик число сивловов НЕХ в строке
        Const SL = 16           ' Количество символов в строке
        Dim s1 As String = ""   ' Строка с символами после НЕХ строки
        Dim s_out As String = ""   ' Строка выходная

        While b_i <> len

            ub = buf(b_i)
            b_i = b_i + 1

            ' добавляем 0 с переди если в строке один символ 1 -> 01, 4 -> 04
            sc = Hex(ub)
            If sc.Length = 1 Then
                sc = "0" + sc
            End If
            s = s + sc + " "

            If ub <= &H1F And ub >= 0 Then
                ub = &H2E ' "."
            End If
            s1 = s1 + Chr(ub)

            sl_n = sl_n + 1

            If sl_n = SL Then
                s_out = s_out + s + "    " + s1 + vbCrLf

                s = ""
                s1 = ""
                sl_n = 0

            End If

        End While

        If s.Length > 0 Then ' если строка была короче SL символов, то выводим строку и в конец довадяем пробелы, т.е. выравниваем
            sc = ""
            While sl_n <> SL
                sc = sc + "   "
                sl_n = sl_n + 1
            End While

            s_out = s_out + s + sc + "    " + s1 + vbCrLf
        End If

        ConvArrayByteToHEX = s_out

    End Function

End Module
