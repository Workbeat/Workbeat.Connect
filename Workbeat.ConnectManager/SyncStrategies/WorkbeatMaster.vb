Imports System.Configuration

Namespace Strategies

	Public Class WorkbeatMaster
		Inherits SyncStrategy

		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WorkbeatMaster))

		Public Overrides Function Sync(entityType As Workbeat.Entities.EntityTypes, clientType As String, clientName As String) As Integer
			log.Info("Comenzando Sincronización (Workbeat es la fuente maestra)")
			log.Info("Sincronizando Entidad " & entityType.ToString & " en el cliente:" & clientType & " : " & clientName)
			'antes de empezar, guardar la fecha de sincronizacion
			Dim syncDateStart As DateTime = Date.Now()
			Dim entityName As String = entityType.ToString
			Dim entityList As List(Of Workbeat.Entities.WorkbeatEntity)
			Dim wbDal As Workbeat.Entities.WorkbeatDal
			Dim clientDal As Workbeat.Entities.Dal.ClientDal
			Dim wbEnt As Workbeat.Entities.WorkbeatEntity

			Dim apiObj As New Workbeat.API.Client(ConfigurationManager.AppSettings("workbeat_api_url"))
			apiObj.Connect(ConfigurationManager.AppSettings("workbeat_client_id"), ConfigurationManager.AppSettings("workbeat_client_secret"))
			wbDal = Workbeat.Entities.WorkbeatDal.getWBDal(entityType)
			wbDal.APIClient = apiObj

			clientDal = Workbeat.Entities.Dal.ClientDal.getClientDal(clientType, entityType)
			' get Updated entities (since last Update) from Workbeat
			entityList = wbDal.getLastUpdated(clientName)
			log.Info("Se encontraron " & entityList.Count & " " & entityName & "(s) en Workbeat por actualizar en " & clientName)

			For Each wbEnt In entityList
				If Not wbEnt.active Then
					' Borrar en Cliente
					log.Info("Borrando " & entityName & " en el cliente. WorkbeatID:" & wbEnt.workbeatId)
					clientDal.Delete(wbEnt.data, clientName)
				Else
					' Save in client (client object DAL checks for id mapping)
					log.Info("Creando/Actualizando " & entityName & " en el cliente. WorkbeatID:" & wbEnt.workbeatId)
					clientDal.Save(wbEnt.data, clientName)
				End If
			Next
			' Actualizar fecha de sincronizacion.
			Workbeat.Entities.Utilities.Sync.SyncDate.setEntityLastUpdate(entityType, clientName, syncDateStart)
			log.Info("Fin de sincronizacion." & entityType.ToString)
			Return 1
		End Function


	End Class

End Namespace

