Imports System.Configuration

Namespace Strategies

	Public Class ClientMaster
		Inherits SyncStrategy

		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WorkbeatMaster))

		Public Overrides Function Sync(entityType As Workbeat.Entities.EntityTypes, clientType As String, clientName As String) As Integer
			log.Info("Comenzando Sincronización (" & clientName & " es la fuente maestra)")
			log.Info("Sincronizando Entidad " & entityType.ToString & " en workbeat <- " & clientType & " : " & clientName)
			'antes de empezar, guardar la fecha de sincronizacion
			Dim syncDateStart As DateTime = Date.Now()
			Dim entityName As String = entityType.ToString
			Dim wbDal As Workbeat.Entities.WorkbeatDal
			Dim clientDal As Workbeat.Entities.Dal.ClientDal
			Dim clEnt As Workbeat.Entities.ClientEntity
			clEnt = Workbeat.Entities.ClientEntity.getClientEntity(entityType, clientType)
			clientDal = Workbeat.Entities.Dal.ClientDal.getClientDal(clientType, entityType)
			Dim entityList As List(Of Workbeat.Entities.ClientEntity)

			Dim apiObj As New Workbeat.API.Client(ConfigurationManager.AppSettings("workbeat_api_url"))
			apiObj.Connect(ConfigurationManager.AppSettings("workbeat_client_id"), ConfigurationManager.AppSettings("workbeat_client_secret"))
			wbDal = Workbeat.Entities.WorkbeatDal.getWBDal(entityType)
			wbDal.APIClient = apiObj

			' get Updated entities (since last Update) from Workbeat
			entityList = clientDal.getLastUpdated(clientName)
			log.Info("Se encontraron " & entityList.Count & " " & entityName & "(s) en " & clientName & " por actualizar en Workbeat.")

			For Each clEnt In entityList
				If Not clEnt.active Then
					'	Borrar en Workbeat
					log.Info("Borrando " & entityName & " en el cliente. ClientId:" & clEnt.entityId)
					wbDal.Delete(clEnt.data, clientName, clientType)
				Else
					' Save in Workbeat (client object DAL checks for id mapping)
					log.Info("Creando/Actualizando " & entityName & " en el cliente. ClientId:" & clEnt.entityId)
					wbDal.Save(clEnt.data, clientName, clientType)
				End If
			Next
			' TODO: Actualizar fecha de sincronizacion.
			Workbeat.Entities.Utilities.Sync.SyncDate.setEntityLastUpdate(entityType, clientName, syncDateStart)
			Return 1
		End Function


	End Class

End Namespace



