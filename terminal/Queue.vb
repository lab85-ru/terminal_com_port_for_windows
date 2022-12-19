' -----------------------------------------------------------------------------
' Модуль работы с кольцевой очередью
' -----------------------------------------------------------------------------
Module Queue

    Structure queue_buf_t
        Dim queue() As Byte '   ukazatel na buffer, kotoriy budet kolchevim
        Dim len As UInteger '   dlinna buffera kolchevogo
        Dim din As UInteger '   ukazatel dla zapisi danih
        Dim dout As UInteger '  ukazatel dla chteniya danih
    End Structure


    '/*******************************************************************************
    '* get free size from queue buf
    '*******************************************************************************/
    Function get_free_size_queue(ByRef q As queue_buf_t) As Integer
        If q.din = q.dout Then
            Return q.len
        End If

        If q.din > q.dout Then
            Return q.len - q.din + q.dout
        Else
            Return q.dout - q.din
        End If

    End Function

    '/*******************************************************************************
    '* get data size from queue buf
    '*******************************************************************************/
    Function get_data_size_queue(ByRef q As queue_buf_t) As UInteger
        Return q.len - get_free_size_queue(q)
    End Function

    '/*******************************************************************************
    '* push data from rw_buf to queue buf
    '*******************************************************************************/
    Function push_data_queue(ByRef q As queue_buf_t, ByRef din_buf() As Byte, ByVal din_buf_size As UInteger) As Integer
        Dim i As UInteger

        If get_free_size_queue(q) < din_buf_size Then '// Error No Memory to queue for Write
            Return 1
        End If

        For i = 0 To din_buf_size - 1
            q.queue(q.din) = din_buf(i)
            q.din = q.din + 1

            If q.din >= q.len Then
                q.din = 0
            End If
        Next

        Return 0
    End Function

    '/*******************************************************************************
    '* pop data from queue to rw_buf buf
    '*******************************************************************************/
    Function pop_data_queue(ByRef q As queue_buf_t, ByRef dout_buf() As Byte, ByVal dout_buf_size As UInteger) As UInteger
        Dim i As UInteger

        If q.len - get_free_size_queue(q) < dout_buf_size Then '// Error No pop data
            Return 1
        End If

        For i = 0 To dout_buf_size - 1
            dout_buf(i) = q.queue(q.dout)
            q.dout = q.dout + 1
            If q.dout >= q.len Then
                q.dout = 0
            End If
        Next
        Return 0
    End Function

End Module
