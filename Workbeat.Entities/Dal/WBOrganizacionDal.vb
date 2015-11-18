Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Configuration

Namespace Dal

	Public Class WBOrganizacionDal
		Inherits WorkbeatDal


		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WBOrganizacionDal))


		Public Overrides Function getEntity(id As String) As Dto
			Dim result As String = APIClient.get("org/organizaciones/" & id)
			Dim org As New OrganizacionDto
			org = DirectCast(Utilities.JsonConverter.getObject(result, org), Workbeat.Entities.OrganizacionDto)
			Return org
		End Function

		Public Overrides Function getLastUpdated(clientName As String) As System.Collections.Generic.List(Of WorkbeatEntity)
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.Organizacion, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion.ToUniversalTime()) * 1000
			Dim result As String = APIClient.get("org/ultimos_cambios/organizaciones/", "{""actualizado_desde"":" & epochDate.ToString() & "}")
			Dim organizaciones As OrganizacionDto() = {}
			If result.IndexOf("TotalRows") > 0 Then
				Dim wbres As New Workbeat.API.PagedResult(Of OrganizacionDto)
				wbres = DirectCast(Utilities.JsonConverter.getObject(result, wbres), Workbeat.API.PagedResult(Of OrganizacionDto))
				organizaciones = wbres.data
			Else
				organizaciones = DirectCast(Utilities.JsonConverter.getObject(result, organizaciones), Workbeat.Entities.OrganizacionDto())
			End If
			Dim list As New System.Collections.Generic.List(Of WorkbeatEntity)
			Dim orgEntity As Workbeat.Entities.WorkbeatEntities.Organizacion
			Dim org As OrganizacionDto
			For Each org In organizaciones
				orgEntity = New WorkbeatEntities.Organizacion
				orgEntity.Data = org
				list.Add(orgEntity)
			Next
			Return list
		End Function


		Public Overrides Sub Save(clientObject As Dto, clientName As String, clientType As String)
			Dim clientDto As Workbeat.Entities.OrganizacionDto
			clientDto = DirectCast(clientObject, Workbeat.Entities.OrganizacionDto)
			Dim clEnt As Workbeat.Entities.ClientEntities.Organizacion
			clEnt = Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.Organizacion, clientType)
			clEnt.Data = clientDto
			log.Debug("Llamando a WBOrganizacionDal.Save(EntityId:=" & clEnt.entityId & ", nombre:=" & clientDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Organizacion, clEnt.entityId, clientName)

			Dim orgDto As Workbeat.Entities.OrganizacionDto
			Dim wbEnt As New Workbeat.Entities.WorkbeatEntities.Organizacion
			' copiar los datos del tipo de plantilla al objeto workbeat.
			orgDto = clientObject.Clone()
			wbEnt.Data = orgDto
			wbEnt.workbeatId = map.workbeatId
			Dim jsonData As String = Utilities.JsonConverter.getJsonObj(wbEnt.Data)
			Dim result As String
			If IsNumeric(wbEnt.workbeatId) Then
				' actualizar
				result = APIClient.post("org/organizaciones/" & wbEnt.workbeatId, jsonData)
			Else
				' es nuevo.
				result = APIClient.post("org/organizaciones/", jsonData)
				Dim newOrgDto As New Workbeat.Entities.OrganizacionDto
				newOrgDto = DirectCast(Utilities.JsonConverter.getObject(result, newOrgDto), Workbeat.Entities.OrganizacionDto)
				wbEnt.Data = newOrgDto
				Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
				newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Organizacion, clEnt.entityId, clientName)
				newMap.workbeatId = wbEnt.workbeatId
				newMap.Save()
			End If
		End Sub

		Public Overrides Sub Delete(clientObject As Dto, clientName As String, clientType As String)
			Dim clDto As Workbeat.Entities.OrganizacionDto
			clDto = DirectCast(clientObject, Workbeat.Entities.OrganizacionDto)
			log.Debug("Llamando a WBOrganizacionDal.Delete(EntityId:=" & clDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Organizacion, clDto.id, clientName)
			If IsNumeric(map.workbeatId) Then
				Dim result As Integer
				result = APIClient.delete("org/organizacion/" & map.workbeatId)
			End If
		End Sub


	End Class

End Namespace