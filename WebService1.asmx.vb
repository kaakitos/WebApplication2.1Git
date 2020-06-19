Imports System.Web.Services
Imports System.ComponentModel
Imports IBM.Data.DB2.iSeries
Imports DocuWare.Platform.ServerClient
Imports System.Web.Configuration


' Pour autoriser l'appel de ce service Web depuis un script à l'aide d'ASP.NET AJAX, supprimez les marques de commentaire de la ligne suivante.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebService1
    Inherits System.Web.Services.WebService

    Private logger As Object

    Public Function requestDB21(param As String) As String
        'Dim conStr = "DataSource=10.64.20.29;UserID=BJBON;Password=Cambo64250;LibraryList=XDUXFICWEB,Y2SYEXEC;DataCompression=True;providerName=IBM.Data.DB2.iSeries;"
        Dim conStr = WebConfigurationManager.AppSettings("myConnString")
        Dim cnn = New iDB2Connection(conStr)
        cnn.Open()
        Dim cmd = cnn.CreateCommand()
        cmd.CommandText = "BGRKA.TESTCL"
        'TESTCL =  nom de procedure/ BGRKA= le 1er nivo(biblio)
        cmd.CommandType = CommandType.StoredProcedure

        ' create in And out parameters
        Dim parm = cmd.Parameters.Add("@RAISONCODE", iDB2DbType.iDB2Char, 7)
        cmd.Parameters("@RAISONCODE").Direction = ParameterDirection.InputOutput
        cmd.Parameters("@RAISONCODE").Value = ""

        parm = cmd.Parameters.Add("@CODE", iDB2DbType.iDB2Char, 10)
        cmd.Parameters("@CODE").Direction = ParameterDirection.Input
        cmd.Parameters("@CODE").Value = param


        parm = cmd.Parameters.Add("@RESULTAT", iDB2DbType.iDB2Char, 6)
        cmd.Parameters("@RESULTAT").Direction = ParameterDirection.Output


        '// Call the stored procedure
        cmd.ExecuteNonQuery()

        '// Retrieve output parameters
        Dim resultat = cmd.Parameters("@RESULTAT").Value
        cnn.Close()
        Return resultat
    End Function

    <WebMethod()>
    Public Sub numChrono(idDoc As Integer)
        Dim url = WebConfigurationManager.AppSettings("url")
        Dim Login = WebConfigurationManager.AppSettings("Login")
        Dim Password = WebConfigurationManager.AppSettings("Password")
        Dim Organisation = WebConfigurationManager.AppSettings("Organisation")
        Dim idFileCabinet = WebConfigurationManager.AppSettings("idFileCabinet")
        Dim metaDonnees As String
        metaDonnees = WebConfigurationManager.AppSettings("meta")

        Dim logger = log4net.LogManager.GetLogger("ErrorLog")
        'Dim field = New DocumentIndexField()
        Dim DWDocument As Document

        Try
            Dim conn = ServiceConnection.Create(New Uri(url), Login, Password, Organisation)
            DWDocument = conn.GetFromDocumentForDocumentAsync(Integer.Parse(idDoc), idFileCabinet).Result
            Dim parts As String() = metaDonnees.Split(New Char() {","c})
            Dim part As String
            For Each part In parts
                Dim field = DWDocument.Item(part)
                MsgBox(part & " : " & field.Item)
                ' MsgBox(part)
            Next
            conn.Disconnect()

        Catch exc As Exception
            logger.Error(exc.Message)
            Exit Try
        End Try


    End Sub


End Class