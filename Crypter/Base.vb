Option Infer Off
Option Strict On
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.CodeDom.Compiler
Imports System.IO

Public Class Base
    Public Shared Generator As Engine, Key As Byte()

    Shared Function [XOR](ByVal data As Byte()) As Byte()
        Dim X As Integer = Key.Length
        For I As Integer = 0 To data.Length - 1
            data(I) = data(I) Xor Key(I Mod X)
        Next
        Return data
    End Function

    'CHANGE 7: Updated CodeEngine
Class CodeEngine

        Sub New(ByVal e As Engine)
            G = e
        End Sub

        Property GlobalNoise As Integer
        Property PrivateNoise As Integer
        Property InstanceLimit As Byte

        Private Libs As String() = "advapi32.avifil32.cards.cfgmgr32.comctl32.comdlg32.credui.crypt32.dbghelp.dbghlp.dbghlp32.dhcpsapi.difxapi.dmcl40.dnsapi.dwmapi.faultrep.fwpuclnt.gdi32.gdiplus.glu32.glut32.gsapi.hhctrl.hid.hlink.httpapi.icmp.imm32.iphlpapi.iprop.irprops.kernel32.mapi32.mpr.mqrt.mscorsn.msdrm.msi.msvcrt.netapi32.ntdll.ntdsapi.odbc32.odbccp32.ole32.oleacc.oleaut32.opengl32.powrprof.printui.psapi.pstorec.query.quickusb.rasapi32.rpcrt4.secur32.setupapi.shell32.shlwapi.twain_32.unicows.urlmon.user32.userenv.uxtheme.winfax.winhttp.wininet.winmm.winscard.winspool.wintrust.winusb.wlanapi.ws2_32.wtsapi32.xolehlp".Split("."c)

        Private G As Engine, N As Char = Convert.ToChar(10)
        Private PrivateLimit As Integer, M As Match, Data As StringBuilder

        Function Generate(ByVal code As String) As String
            Data = New StringBuilder(code)
            PrivateLimit = _PrivateNoise

            Generate()

            Dim Limit As Integer = _GlobalNoise, GS As StringBuilder
            Do
                M = Regex.Match(Data.ToString, "\$GB", RegexOptions.Singleline)
                If Not M.Success Then Exit Do
                GS = New StringBuilder(M.Groups(1).Value)
                For I As Integer = 1 To Engine.Random(1, _InstanceLimit)
                    If Limit = 0 Then Exit For
                    GS.AppendLine(Generate(0))
                    Limit -= 1
                Next
                Data.Remove(M.Index, 3)
                Data.Insert(M.Index, GS)
            Loop

            Generate()

            Return Trim(Data.ToString)
        End Function

        Private Sub Generate()
            Do
                M = Regex.Match(Data.ToString, "\$PV(.*?)\$PV", RegexOptions.Singleline)
                If Not M.Success Then Exit Do
                Dim PS As New List(Of String)
                PS.AddRange(Parse(M.Groups(1).Value))
                For I As Integer = 1 To Engine.Random(1, _InstanceLimit)
                    If PrivateLimit = 0 Then Exit For
                    PS.InsertRange(Engine.Random(PS.Count), Parse(Generate(1)))
                    PrivateLimit -= 1
                Next
                Data.Remove(M.Index, M.Length)
                Data.Insert(M.Index, Normalize(PS.ToArray))
            Loop
        End Sub

        Private Function Generate(ByVal scope As Byte) As String
            Dim I As Byte = CByte(If(scope = 0, Engine.Random(4), Engine.Random(3, 6)))
            Select Case I
                Case 0
                    Return [Class]()
                Case 1
                    Return [Function]()
                Case 2
                    Return [Sub]()
                Case 3
                    Return Declaration()
                Case 4
                    Return [Loop]()
                Case Else
                    Return Conditional()
            End Select
        End Function

#Region " Global "
        Private Function [Class]() As String
            Return String.Format("Class {1}{0}$GB{0}End Class", N, G.Name)
        End Function
        Private Function [Function]() As String
            Dim T As Engine.Cast = Engine.GetCast
            If Engine.Chance(15) Then
                Return String.Format("Declare Function {0} Lib ""{1}""({2}) As {3}", G.Name, Libs(Engine.Random(Libs.Length)), G.Transform.Declaration, T)
            End If
            If Engine.Chance(40) Then
                Return String.Format("Delegate Function {0}({1}) As {2}", G.Name, G.Transform.Declaration, T)
            Else
                Return String.Format("Function {1}({2}) As {3}{0}$PV{0}$PV{0}Return {4}{0}End Function", N, G.Name, G.Transform.Declaration, T, Engine.GetValue(T))
            End If
        End Function
        Private Function [Sub]() As String
            If Engine.Chance(15) Then
                Return String.Format("Declare Sub {0} Lib ""{1}""({2})", G.Name, Libs(Engine.Random(Libs.Length)), G.Transform.Declaration)
            End If
            If Engine.Chance(40) Then
                Return String.Format("Delegate Sub {0}({1})", G.Name, G.Transform.Declaration)
            Else
                Return String.Format("Sub {1}({2}){0}$PV{0}$PV{0}End Sub", N, G.Name, G.Transform.Declaration)
            End If
        End Function
#End Region

#Region " Global and Private "
        Private Function Comment() As String
            Return String.Format("'{0}", Engine.RandomString)
        End Function
        Private Function Declaration() As String
            Dim T As Engine.Cast = Engine.GetCast
            Return String.Format("Dim {0} As {1}", G.Name, T) & If(Engine.Chance(75), " = " & Engine.GetValue(T), String.Empty)
        End Function
#End Region

#Region " Private "
        Private Function [Loop]() As String
            If Engine.Chance(70) Then
                Dim T As Engine.Cast = CType(Engine.Random(1, 12), Engine.Cast)
                Dim L As Integer = Engine.Random(0, 21), H As Integer = Engine.Random(20, 128)
                Return String.Format("For {1} As {2} = {3} To {4}{0}{5}{0}Next", N, G.Name, T, L, H, Space)
            Else
                Return String.Format("For Each {1} As Char In ""{2}""{0}{3}{0}Next", N, G.Name, Engine.RandomString, Space)
            End If
        End Function
        Private Function Conditional() As String
            If Engine.Chance(70) Then
                Return String.Format("Dim {1} As Boolean = {2}{0}If {1} Then{0}{3}{0}End If", N, G.Name, Engine.Chance(75), Space)
            Else
                Return String.Format("If {1} Then{0}{2}{0}End If", N, Engine.Chance(75), Space)
            End If
        End Function
#End Region


        Private Function Space() As String
            Return New String(N, Engine.Random(1, 4))
        End Function
        Private Function Parse(ByVal code As String) As String()
            Dim R As New IO.StringReader(code), T As New List(Of String)
            Do
                T.Add(R.ReadLine.Trim)
            Loop Until R.Peek = -1
            Parse = T.ToArray
            R.Close()
        End Function
        Private Function Trim(ByVal code As String) As String
            Dim R As New IO.StringReader(code), T As New StringBuilder(code.Length), B As String
            Do
                B = R.ReadLine.Trim
                If Not String.IsNullOrEmpty(B) Then T.AppendLine(B)
            Loop Until R.Peek = -1
            Trim = T.ToString
            R.Close()
        End Function
        Private Function Normalize(ByVal code As String()) As String
            Dim T As New StringBuilder
            For Each I As String In code
                T.AppendLine(I)
            Next
            Return T.ToString
        End Function

    End Class
    Class Engine
        Private Count As Integer, Taken As New List(Of String)

        ''' <summary>
        ''' Returns the last name randomly generated name.
        ''' </summary>
        Property Last As String

        ''' <summary>
        ''' Generates a dynamic routine signature, keeping the provided parameters intact.
        ''' </summary>
        Function Transform(ByVal ParamArray parameters As String()) As Routine
            Dim Signature As New List(Of Parameter)
            Dim C As Integer = Random(6), Cast As Cast
            For I As Integer = 1 To C
                Cast = GetCast()
                Signature.Add(New Parameter("ByVal " & Name() & " As " & Cast.ToString, -1, Cast))
            Next
            For I As Integer = 0 To parameters.Length - 1
                Signature.Insert(Random(C), New Parameter(parameters(I), I))
            Next
            Return New Routine(Signature.ToArray)
        End Function

        ''' <summary>
        ''' Obfuscates generated variable names.
        ''' </summary>
        Function Randomize(ByVal data As String) As String
            Dim Names As New List(Of String)
            For Each M As Match In Regex.Matches(data, "\$RN\d+")
                If Not Names.Contains(M.Value) Then Names.Add(M.Value)
            Next

            Dim Shuffle As String() = Names.ToArray
            Array.Sort(Shuffle, New LengthComparer)

            Dim T As New StringBuilder(data), Current As String
            For Each N As String In Shuffle
                Do
                    Current = Name(Random(1, 7))
                Loop Until Not Taken.Contains(Current)
                Taken.Add(Current)
                T.Replace(N, Current)
            Next

            Return T.ToString
        End Function
        Private Function Name(ByVal length As Integer) As String
            Dim Illegal As String = "alias.and.ansi.as.auto.byref.byte.byval.call.case.catch.cbool.cbyte.cchar.cdate.cdec.cdbl.char.cint.class.clng.cobj.const.cshort.csng.cstr.ctype.date.dim.do.each.else.end.enum.erase.error.event.exit.false.for.get.gosub.goto.if.in.is.let.lib.like.long.loop.me.mod.module.new.next.not.of.on.or.redim.rem.set.short.step.stop.sub.then.throw.to.true.try.until.when.while.with.xor"
            Do
                '//CHANGE 17: Removed extended ASCII characters
                Dim Range As String = "abcdefghijklmnopqrstuvwxyz0123456789_" 'µºÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝÞßÿ
                Dim T As New StringBuilder(Range(Random(26)))
                While T.Length < length
                    T.Append(Range(Random(Range.Length)))
                End While
                Name = T.ToString
            Loop Until Array.IndexOf(Illegal.Split("."c), Name.ToLower) = -1
            Return Name
        End Function

        Shared Function Random() As Double
            Return New Random(Guid.NewGuid.GetHashCode).NextDouble
        End Function
        Shared Function Random(ByVal max As Integer) As Integer
            Return New Random(Guid.NewGuid.GetHashCode).Next(0, max)
        End Function
        Shared Function Random(ByVal min As Integer, ByVal max As Integer) As Integer
            Return New Random(Guid.NewGuid.GetHashCode).Next(min, max)
        End Function

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Shared Function RandomString() As String
            Dim T As New StringBuilder
            Dim U As String = "abcdefgijkmnopqrstwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#%^&*()_-+={[}]\:;'<,>.?/ "
            While T.Length < Random(2, 24)
                T.Append(U(Random(U.Length)))
            End While
            Return T.ToString
        End Function
        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Shared Function Chance(ByVal probability As Double) As Boolean
            Return New Random(Guid.NewGuid.GetHashCode).Next(1, 100) >= 100 - probability
        End Function
        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Shared Function Range(ByVal min As Double, ByVal max As Double) As Double
            Return Math.Round(min + ((max - min) * Random()), Engine.Random(5))
        End Function

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Shared Function GetCast() As Cast
            Return DirectCast(CByte(Random(14)), Cast)
        End Function
        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Enum Cast As Byte
            [Boolean] = 0
            [Byte] = 1
            [SByte] = 2
            [Single] = 3
            [Short] = 4
            [UShort] = 5
            [Integer] = 6
            [UInteger] = 7
            [Long] = 8
            [ULong] = 9
            [Double] = 10
            [Decimal] = 11
            [String] = 12
            [Date] = 13
        End Enum
        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Shared Function GetValue(ByVal c As Cast) As String
            If Chance(8) Then Return "Nothing"
            If Chance(8) And c > 0 And c < 12 Then Return "0"
            Dim T As Double
            Select Case c
                Case Cast.Boolean
                    Return Chance(50).ToString
                Case Cast.Byte
                    T = Random(256)
                Case Cast.SByte
                    T = Random(-128, 128)
                Case Cast.Single
                    Return Range(Short.MinValue, Short.MaxValue) & "!"
                Case Cast.Short, Cast.UShort
                    T = Random(Short.MaxValue)
                Case Cast.UInteger, Cast.ULong
                    T = Random(0, Integer.MaxValue)
                Case Cast.Integer, Cast.Long, Cast.Decimal
                    T = Random(CInt(Integer.MinValue / 2), Integer.MaxValue)
                Case Cast.Double
                    T = Range(Short.MinValue, Short.MaxValue)
                Case Cast.String
                    Return """" & RandomString() & """"
                Case Cast.Date
                    Return "Date.Now"
            End Select
            Return T.ToString
        End Function

        ''' <summary>
        ''' Returns the next available name.
        ''' </summary>
        Function Name() As String
            Name = "$RN" & Count.ToString
            _Last = Name
            Count += 1
        End Function

        ''' <summary>
        ''' Parses the data, replacing each instance of ?N# or !N# with a randomized name. 
        ''' Returning !N# instances for use later on.
        ''' </summary>
        Function Parse(ByVal data As String) As [Module]
            Dim Names, Special As New List(Of String)
            For Each M As Match In Regex.Matches(data, "(\?|!)N\d+")
                If Not Names.Contains(M.Value) Then Names.Add(M.Value)
            Next

            Dim Shuffle As String() = Names.ToArray
            Array.Sort(Shuffle, New LengthComparer)

            Dim Current As String
            For Each N As String In Shuffle
                Current = Name()
                If N.Contains("!") Then Special.Add(Current)
                data = data.Replace(N, Current)
            Next

            Return New [Module](data, Special.ToArray)
        End Function

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)>
        Structure Routine
            Public Declaration As String
            Private Parameters As Parameter()
            Sub New(ByVal _parameters As Parameter())
                If _parameters.Length = 0 Then Return
                Parameters = _parameters
                Dim T As New StringBuilder
                For Each P As Parameter In _parameters
                    T.Append(P.Code & ", ")
                Next
                Declaration = T.ToString(0, T.Length - 2)
            End Sub
            Overloads Function ToString(ByVal ParamArray params As String()) As String
                Dim T As New StringBuilder
                For Each P As Parameter In Parameters
                    If P.ID > -1 Then
                        T.Append(params(P.ID) & ", ")
                    Else
                        T.Append(GetValue(P.Cast) & ", ")
                    End If
                Next
                Return T.ToString(0, T.Length - 2)
            End Function
        End Structure
        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)>
        Structure [Module]
            Public Code As String, Names As String()
            Sub New(ByVal _code As String, ByVal ParamArray _names As String())
                Code = _code
                Names = _names
            End Sub
        End Structure
        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Structure Parameter
            Dim Code As String, Cast As Cast, ID As Integer
            Sub New(ByVal _code As String, ByVal _id As Integer, Optional ByVal _cast As Cast = Cast.Byte)
                Code = _code
                ID = _id
                Cast = _cast
            End Sub
        End Structure

        Private Class LengthComparer
            Implements IComparer(Of String)
            Public Function Compare(ByVal x As String, ByVal y As String) As Integer Implements IComparer(Of String).Compare
                Return CInt(x.Length > y.Length)
            End Function
        End Class
    End Class

    Private Declare Auto Function GetShortPathName Lib "kernel32" (ByVal path As String, ByVal [short] As StringBuilder, ByVal length As Integer) As Integer

    Shared Function Compile(ByVal code As String, ByVal [out] As String, ByVal icon As String, ByVal resource As String) As Boolean
        '//CHANGE 14: Added reference to System.Drawing.dll. Required for GetInt32 and GetBytes
        Dim P As New CompilerParameters({"System.dll", "System.Drawing.dll", "System.Windows.Forms.dll"}, [out]) With {.MainClass = Regex.Match(code, "Module\s(.+)").Groups(1).Value}
        '//CHANGE 19: Removed extra compiler options
        P.CompilerOptions = "/platform:x86 /t:winexe"

        If Not String.IsNullOrEmpty(resource) Then P.EmbeddedResources.Add(resource)
        P.IncludeDebugInformation = True

        Dim I As New StringBuilder(256)
        If Not String.IsNullOrEmpty(icon) Then
            GetShortPathName(Path.GetTempPath, I, 256)
            I = New StringBuilder(Path.Combine(I.ToString, Path.GetRandomFileName) & ".ico")
            File.Copy(icon, I.ToString)
            P.CompilerOptions &= " /win32icon:" & I.ToString
        End If

        Dim C As New VBCodeProvider(New Dictionary(Of String, String) From {{"CompilerVersion", "v2.0"}})
        Dim R As CompilerResults = C.CompileAssemblyFromSource(P, code)

        If File.Exists(I.ToString) Then File.Delete(I.ToString)
        If File.Exists(resource) Then File.Delete(resource)

        If R.Errors.Count > 0 Then
#If DEBUG Then
            For Each CE As CompilerError In R.Errors
                MessageBox.Show(String.Format("[{0} #{1} @ {2}, {3}]: {4}", If(CE.IsWarning, "Warning", "Error"), CE.ErrorNumber, CE.Line, CE.Column, CE.ErrorText))
            Next
#End If
            Return False
        End If

        Return True
    End Function

    Structure Storage
        Dim Declaration, Instance As String
        Sub New(ByVal _declaration As String, ByVal _instance As String)
            Declaration = _declaration
            Instance = _instance
        End Sub
    End Structure

    Shared Function StoreNumeric(ByVal data As Byte()) As Storage
        Dim O1 As Integer = data.Length Mod 8, O2 As Integer = (data.Length - O1) Mod 22000

        Dim T, U As New StringBuilder, S As Storage
        If O1 > 0 Then
            S = CreateArray(Of Byte)(data, 0, O1)
            T.AppendLine(S.Declaration)
            U.Append(S.Instance & ",")
        End If
        If O2 > 0 Then
            S = CreateArray(Of ULong)(data, O1, O2)
            T.AppendLine(S.Declaration)
            U.Append(S.Instance & ",")
        End If

        For I As Integer = O1 + O2 To data.Length - 1 Step 22000
            S = CreateArray(Of ULong)(data, I, 22000)
            T.AppendLine(S.Declaration)
            U.Append(S.Instance & ",")
        Next

        Return New Storage(T.ToString, U.ToString(0, U.Length - 1))
    End Function
    Private Shared Function CreateArray(Of V)(ByVal data As Byte(), ByVal index As Integer, ByVal length As Integer) As Storage
        Dim T As New StringBuilder((length * 2) + 34)
        T.Append("Dim " & Generator.Name & " As " & GetType(V).Name & "() = New " & GetType(V).Name & "(){")

        If GetType(V).Name = "Byte" Then
            For I As Integer = 0 To length - 1
                T.Append(data(I) & ",")
            Next
        Else
            Dim U As String
            For I As Integer = index To index + length - 1 Step 8
                U = BitConverter.ToUInt64(data, I).ToString
                T.Append(U & If(U.Length > 18, "UL", String.Empty) & ",")
            Next
        End If

        Return New Storage(T.ToString(0, T.Length - 1) & "}", Generator.Last)
    End Function

    Shared Function StoreCString(ByVal data As Byte()) As Storage
        Dim T As New CLI248
        Dim O As String = T.Encode(data)
        Dim Declaration As String, Instance As String = Generator.Name
        Dim B1 As New StringBuilder, B2 As New StringBuilder("Dim " & Instance & " As String = ")

        For Each M As Match In Regex.Matches(O, ".{1,65000}")
            Declaration = Generator.Name
            B1.AppendLine("Dim " & Declaration & " As String = """ & M.Value & """")
            B2.Append(Declaration & " & ")
        Next

        B2.Remove(B2.Length - 3, 3)
        B1.Append(B2.ToString)

        Return New Storage(B1.ToString, Instance)
    End Function

    Structure Resource
        Dim Name As String, Data As Byte()
        Sub New(ByVal _name As String, ByVal _data As Byte())
            Name = _name
            Data = _data
        End Sub
    End Structure
    Shared Sub CreateResource(ByVal path As String, ByVal ParamArray resources As Resource())
        Using T As New Resources.ResourceWriter(path)
            For Each R As Resource In resources
                T.AddResource(R.Name, R.Data)
            Next
            T.Generate()
        End Using
    End Sub

    Structure NativeFile
        Dim Name, Path As String
    End Structure
    Declare Function UpdateResourceA Lib "kernel32" (ByVal handle As IntPtr, ByVal type As String, ByVal name As String, ByVal language As UShort, ByVal data As IntPtr, ByVal length As Integer) As Boolean
    Declare Function BeginUpdateResourceA Lib "kernel32" (ByVal path As String, ByVal clear As Boolean) As IntPtr
    Declare Function EndUpdateResourceA Lib "kernel32" (ByVal handle As IntPtr, ByVal cancel As Boolean) As Boolean
    Shared Function Write(ByVal path As String, ByVal data As Byte(), ByVal name As String) As Boolean
        Try
            Dim resID As Integer = Engine.Random(1, 500)
            Dim H As IntPtr = BeginUpdateResourceA(path, False)
            Dim GH As GCHandle = GCHandle.Alloc(data, GCHandleType.Pinned)
            '//CHANGE 3: Changed resource type from 'RT_RCDATA' to 'RC_IMAGE'
            Write = UpdateResourceA(H, "RC_IMAGE", name, CUShort(resID), GH.AddrOfPinnedObject, data.Length)
            GH.Free()
            EndUpdateResourceA(H, False)
        Catch
            Return False
        End Try
    End Function
    Shared Function ClearPadding(ByVal data As Byte()) As Byte()
        Dim T As String = Encoding.ASCII.GetString(data)
        For Each M As Match In Regex.Matches(T, "PADD(I(N(G(XX?)?)?)?)+")
            For I As Integer = M.Index To M.Index + M.Length - 1
                data(I) = 0
            Next
        Next
        Return data
    End Function

    Class CLI248
        Private Suppress As Byte() = New Byte() {0, 0, 10, 12, 13, 15, 34, 127, 133}
        Function Encode(ByVal data As Byte()) As String
            Dim Items As New Dictionary(Of Byte, Integer)(255), Rare(8) As Byte, Current As Integer
            Dim U As New Dictionary(Of Byte, Char)(255), B As Byte, T As New StringBuilder(data.Length)
            For I As Integer = 0 To 255
                B = CByte(I)
                Items.Add(B, 0)
                U.Add(B, Convert.ToChar(B))
            Next
            For I As Integer = 0 To data.Length - 1
                Items(data(I)) += 1
            Next
            For I As Byte = 1 To 8
                Items.Remove(Suppress(I))
            Next
            Dim S As New List(Of KeyValuePair(Of Byte, Integer))(Items)
            S.Sort(AddressOf Compare)
            Suppress(0) = S(0).Key
            For I As Byte = 0 To 8
                Rare(I) = S(I + 1).Key
            Next
            T.Append(U(Suppress(0)))
            For I As Byte = 0 To 8
                T.Append(U(Rare(I)))
            Next
            For I As Integer = 0 To data.Length - 1
                B = data(I)
                Current = Array.IndexOf(Rare, B)
                If Current > -1 Then
                    T.Append(U(Suppress(0)) & U(B))
                Else
                    Current = Array.IndexOf(Suppress, B)
                    If Current > -1 Then
                        T.Append(U(Rare(Current)))
                    Else
                        T.Append(U(B))
                    End If
                End If
            Next
            Return T.ToString
        End Function
        Private Function Compare(ByVal x As KeyValuePair(Of Byte, Integer), ByVal y As KeyValuePair(Of Byte, Integer)) As Integer
            Return CInt(x.Value < y.Value)
        End Function
    End Class

End Class
