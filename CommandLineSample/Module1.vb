Imports System.Text
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Configuration

Module Module1

	Sub Main()

		' verifica creacion de base de datos.
		Workbeat.Entities.Utilities.ObjectMapper.DB.VerifyDatabase()

		Dim settings As NameValueCollection = DirectCast(ConfigurationManager.GetSection("workbeat.connect"), NameValueCollection)
		Dim clientType As String = settings.Get("clientType")
		Dim mgr As New Workbeat.ConnectManager.Manager
		mgr.SyncStrategy = New Workbeat.ConnectManager.Strategies.WorkbeatMaster
		mgr.clientType = clientType
		mgr.clientName = "MIDBSQLite"
		mgr.Sync(Workbeat.Entities.EntityTypes.Posicion)



		mgr.StartListening(clientType)


	End Sub

End Module
