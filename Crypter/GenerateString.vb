Imports System.Security.Cryptography
Public Class Randomization
    Public Class RandomPassword
        Private Shared DEFAULT_MIN_PASSWORD_LENGTH As Integer = 8
        Private Shared DEFAULT_MAX_PASSWORD_LENGTH As Integer = 10
        Private Shared PASSWORD_CHARS_LCASE As String = "abcdefgijkmnopqrstwxyz"
        Private Shared PASSWORD_CHARS_UCASE As String = "ABCDEFGHJKLMNPQRSTWXYZ"
        Private Shared PASSWORD_CHARS_NUMERIC As String = "0123456789"
        Private Shared PASSWORD_CHARS_NUMERIC2 As String = "123456789"
        Public Shared Function Generate() As String
            Generate = Generate(DEFAULT_MIN_PASSWORD_LENGTH, _
                                DEFAULT_MAX_PASSWORD_LENGTH)
        End Function
        Public Shared Function Generate(ByVal length As Integer) As String
            Generate = Generate(length, length)
        End Function
        Public Shared Function Generate(ByVal minLength As Integer, _
                                ByVal maxLength As Integer) _
          As String
            If (minLength <= 0 Or maxLength <= 0 Or minLength > maxLength) Then
                Generate = Nothing
            End If
            Dim charGroups As Char()() = New Char()() _
            { _
                PASSWORD_CHARS_LCASE.ToCharArray(), PASSWORD_CHARS_UCASE.ToCharArray(), PASSWORD_CHARS_NUMERIC.ToCharArray()}
            Dim charsLeftInGroup As Integer() = New Integer(charGroups.Length - 1) {}
            Dim I As Integer
            For I = 0 To charsLeftInGroup.Length - 1
                charsLeftInGroup(I) = charGroups(I).Length
            Next
            Dim leftGroupsOrder As Integer() = New Integer(charGroups.Length - 1) {}
            For I = 0 To leftGroupsOrder.Length - 1
                leftGroupsOrder(I) = I
            Next
            Dim randomBytes As Byte() = New Byte(3) {}
            Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()
            rng.GetBytes(randomBytes)
            Dim seed As Integer = ((randomBytes(0) And &H7F) << 24 Or _
                                    randomBytes(1) << 16 Or _
                                    randomBytes(2) << 8 Or _
                                    randomBytes(3))
            Dim random As Random = New Random(seed)
            Dim password As Char() = Nothing
            If (minLength < maxLength) Then
                password = New Char(random.Next(minLength - 1, maxLength)) {}
            Else
                password = New Char(minLength - 1) {}
            End If
            Dim nextCharIdx As Integer
            Dim nextGroupIdx As Integer
            Dim nextLeftGroupsOrderIdx As Integer
            Dim lastCharIdx As Integer
            Dim lastLeftGroupsOrderIdx As Integer = leftGroupsOrder.Length - 1
            For I = 0 To password.Length - 1
                If (lastLeftGroupsOrderIdx = 0) Then
                    nextLeftGroupsOrderIdx = 0
                Else
                    nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx)
                End If
                nextGroupIdx = leftGroupsOrder(nextLeftGroupsOrderIdx)
                lastCharIdx = charsLeftInGroup(nextGroupIdx) - 1
                If (lastCharIdx = 0) Then
                    nextCharIdx = 0
                Else
                    nextCharIdx = random.Next(0, lastCharIdx + 1)
                End If
                password(I) = charGroups(nextGroupIdx)(nextCharIdx)
                If (lastCharIdx = 0) Then
                    charsLeftInGroup(nextGroupIdx) = _
                                    charGroups(nextGroupIdx).Length
                Else
                    If (lastCharIdx <> nextCharIdx) Then
                        Dim temp As Char = charGroups(nextGroupIdx)(lastCharIdx)
                        charGroups(nextGroupIdx)(lastCharIdx) = _
                                    charGroups(nextGroupIdx)(nextCharIdx)
                        charGroups(nextGroupIdx)(nextCharIdx) = temp
                    End If

                    charsLeftInGroup(nextGroupIdx) = _
                               charsLeftInGroup(nextGroupIdx) - 1
                End If
                If (lastLeftGroupsOrderIdx = 0) Then
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1
                Else
                    If (lastLeftGroupsOrderIdx <> nextLeftGroupsOrderIdx) Then
                        Dim temp As Integer = _
                                    leftGroupsOrder(lastLeftGroupsOrderIdx)
                        leftGroupsOrder(lastLeftGroupsOrderIdx) = _
                                    leftGroupsOrder(nextLeftGroupsOrderIdx)
                        leftGroupsOrder(nextLeftGroupsOrderIdx) = temp
                    End If
                    lastLeftGroupsOrderIdx = lastLeftGroupsOrderIdx - 1
                End If
            Next
            Generate = New String(password)
        End Function
        Public Shared Function Number(ByVal minLength As Integer, _
                                       ByVal maxLength As Integer) _
            As String
            If (minLength <= 0 Or maxLength <= 0 Or minLength > maxLength) Then
                Number = Nothing
            End If
            Dim charGroups As Char()() = New Char()() _
            {PASSWORD_CHARS_NUMERIC2.ToCharArray()}
            Dim charsLeftInGroup As Integer() = New Integer(charGroups.Length - 1) {}
            Dim I As Integer
            For I = 0 To charsLeftInGroup.Length - 1
                charsLeftInGroup(I) = charGroups(I).Length
            Next
            Dim leftGroupsOrder As Integer() = New Integer(charGroups.Length - 1) {}
            For I = 0 To leftGroupsOrder.Length - 1
                leftGroupsOrder(I) = I
            Next
            Dim randomBytes As Byte() = New Byte(3) {}
            Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()
            rng.GetBytes(randomBytes)
            Dim seed As Integer = ((randomBytes(0) And &H7F) << 24 Or _
                                    randomBytes(1) << 16 Or _
                                    randomBytes(2) << 8 Or _
                                    randomBytes(3))
            Dim random As Random = New Random(seed)
            Dim password As Char() = Nothing
            If (minLength < maxLength) Then
                password = New Char(random.Next(minLength - 1, maxLength)) {}
            Else
                password = New Char(minLength - 1) {}
            End If
            Dim nextCharIdx As Integer
            Dim nextGroupIdx As Integer
            Dim nextLeftGroupsOrderIdx As Integer
            Dim lastCharIdx As Integer
            Dim lastLeftGroupsOrderIdx As Integer = leftGroupsOrder.Length - 1
            For I = 0 To password.Length - 1
                If (lastLeftGroupsOrderIdx = 0) Then
                    nextLeftGroupsOrderIdx = 0
                Else
                    nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx)
                End If
                nextGroupIdx = leftGroupsOrder(nextLeftGroupsOrderIdx)
                lastCharIdx = charsLeftInGroup(nextGroupIdx) - 1
                If (lastCharIdx = 0) Then
                    nextCharIdx = 0
                Else
                    nextCharIdx = random.Next(0, lastCharIdx + 1)
                End If
                password(I) = charGroups(nextGroupIdx)(nextCharIdx)
                If (lastCharIdx = 0) Then
                    charsLeftInGroup(nextGroupIdx) = _
                                    charGroups(nextGroupIdx).Length
                Else
                    If (lastCharIdx <> nextCharIdx) Then
                        Dim temp As Char = charGroups(nextGroupIdx)(lastCharIdx)
                        charGroups(nextGroupIdx)(lastCharIdx) = _
                                    charGroups(nextGroupIdx)(nextCharIdx)
                        charGroups(nextGroupIdx)(nextCharIdx) = temp
                    End If
                    charsLeftInGroup(nextGroupIdx) = _
                               charsLeftInGroup(nextGroupIdx) - 1
                End If
                If (lastLeftGroupsOrderIdx = 0) Then
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1
                Else
                    If (lastLeftGroupsOrderIdx <> nextLeftGroupsOrderIdx) Then
                        Dim temp As Integer = _
                                    leftGroupsOrder(lastLeftGroupsOrderIdx)
                        leftGroupsOrder(lastLeftGroupsOrderIdx) = _
                                    leftGroupsOrder(nextLeftGroupsOrderIdx)
                        leftGroupsOrder(nextLeftGroupsOrderIdx) = temp
                    End If

                    lastLeftGroupsOrderIdx = lastLeftGroupsOrderIdx - 1
                End If
            Next
            Number = New String(password)
        End Function
    End Class
End Class