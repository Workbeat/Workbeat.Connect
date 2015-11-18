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
		' este es el orden correcto de sincronizacion
		Dim allEntities() As String = {"Atributo", "ElementoAtributo", "TipoPlantilla", "Organizacion",
				  "DescripcionPuesto", "Ciudad", "MotivoBaja", "Puesto", "Posicion",
				  "Empleado", "MovPers"}

		' Se pone la lista de entidades entre comas. (ver explicacion mas abajo)
		Dim strEntities As String = "," + ConfigurationManager.AppSettings("Synchronize").ToString.Replace(" ", "") + ","
		
		Dim mgr As New Workbeat.ConnectManager.Manager
		mgr.SyncStrategy = New Workbeat.ConnectManager.Strategies.WorkbeatMaster

		'Para sicronizar los elementos en el orden correcto:
		' Se hace un ciclo en las entidades en el orden correcto 
		'y se checa si existen entre los elementos a sincronizar
		For Each ent In allEntities
			' para asegurarnos que es el nombre de la entidad completa, y no encuentre 
			' la palabra "Posicion" dentro de "PosicionesEmpleado" (si existiera en un futuro),
			' se pide encontrar la palabra completa, entre dos comas.
			If strEntities.IndexOf("," + ent + ",") >= 0 Then
				' si existe entre los elementos a sincronizar
				mgr.clientType = clientType
				mgr.clientName = ConfigurationManager.AppSettings(clientType & "clientName")
				mgr.Sync([Enum].Parse(GetType(Workbeat.Entities.EntityTypes), ent))
			End If
		Next

		' Al final de la sincronizacion, comienza a escuchar por cambios en workbeat.
		Dim strListen As String
		strListen = ConfigurationManager.AppSettings("ListenToWorkbeat")
		If IIf(String.IsNullOrWhiteSpace(strListen), False, CBool(strListen)) Then
			mgr.StartListening(clientType)
		End If


	End Sub

End Module
