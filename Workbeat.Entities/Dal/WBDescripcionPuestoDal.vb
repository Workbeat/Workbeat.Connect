Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Configuration

Namespace Dal

	Public Class WBDescripcionPuestoDal
		Inherits WorkbeatDal


		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WBDescripcionPuestoDal))


		Public Overrides Function getEntity(id As String) As Dto
			Dim result As String = APIClient.get("org/descripcionesPuesto/" & id)
			Dim dp As New DescripcionPuestoDto
			dp = DirectCast(Utilities.JsonConverter.getObject(result, dp), DescripcionPuestoDto)
			Return dp
		End Function

		Public Overrides Function getLastUpdated(clientName As String) As System.Collections.Generic.List(Of WorkbeatEntity)
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.DescripcionPuesto, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion.ToUniversalTime()) * 1000
			Dim result As String = APIClient.get("org/ultimos_cambios/descripcionesPuesto/", "{""actualizado_desde"":" & epochDate.ToString() & "}")
			Dim descripcionesPuestos As DescripcionPuestoDto() = {}
			'log.DebugFormat("************** ultima fecha actualizacion: {0}", {ultimaFechaActualizacion})
			'log.DebugFormat("************** actualizaciones desde epoch: {0}", {epochDate})
			If result.IndexOf("TotalRows") > 0 Then
				Dim wbres As New Workbeat.API.PagedResult(Of DescripcionPuestoDto)
				wbres = DirectCast(Utilities.JsonConverter.getObject(result, wbres), Workbeat.API.PagedResult(Of DescripcionPuestoDto))
				descripcionesPuestos = wbres.data
			Else
				descripcionesPuestos = DirectCast(Utilities.JsonConverter.getObject(result, descripcionesPuestos), DescripcionPuestoDto())
			End If
			Dim list As New System.Collections.Generic.List(Of WorkbeatEntity)
			Dim dpEntity As Workbeat.Entities.WorkbeatEntities.DescripcionPuesto
			Dim dp As DescripcionPuestoDto
			For Each dp In descripcionesPuestos
				dpEntity = New WorkbeatEntities.DescripcionPuesto
				dpEntity.Data = dp
				list.Add(dpEntity)
			Next
			Return list
		End Function


		Public Overrides Sub Save(clientObject As Dto, clientName As String, clientType As String)
			Dim clientDto As Workbeat.Entities.DescripcionPuestoDto
			clientDto = DirectCast(clientObject, Workbeat.Entities.DescripcionPuestoDto)
			Dim clEnt As Workbeat.Entities.ClientEntities.DescripcionPuesto
			clEnt = Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.DescripcionPuesto, clientType)
			clEnt.data = clientDto
			log.Debug("Llamando a WBDescripcionPuestoDal.Save(EntityId:=" & clEnt.entityId & ", nombre:=" & clientDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.DescripcionPuesto, clEnt.entityId, clientName)

			Dim tpDto As Workbeat.Entities.DescripcionPuestoDto
			Dim wbEnt As New Workbeat.Entities.WorkbeatEntities.DescripcionPuesto
			' copiar los datos de la descripcion del puesto al objeto workbeat.
			tpDto = clientObject.Clone()
			wbEnt.Data = tpDto
			' map de organizacion
			Dim mapOrg As Workbeat.Entities.Utilities.ObjectMapper.Map
			mapOrg = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Organizacion, tpDto.idOrganizacion, clientName)
			tpDto.idOrganizacion = mapOrg.workbeatId
			wbEnt.workbeatId = map.workbeatId
			Dim jsonData As String = Utilities.JsonConverter.getJsonObj(wbEnt.Data)
			Dim result As String
			If IsNumeric(wbEnt.workbeatId) Then
				' actualizar
				result = APIClient.post("org/descripcionesPuesto/" & wbEnt.workbeatId, jsonData)
			Else
				' es nuevo.
				result = APIClient.post("org/descripcionesPuesto/", jsonData)
				Dim newTPDto As New Workbeat.Entities.DescripcionPuestoDto
				newTPDto = DirectCast(Utilities.JsonConverter.getObject(result, newTPDto), DescripcionPuestoDto)

				wbEnt.Data = newTPDto
				Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
				newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.DescripcionPuesto, clEnt.entityId, clientName)
				newMap.workbeatId = wbEnt.workbeatId
				newMap.Save()
			End If
		End Sub

		Public Overrides Sub Delete(clientObject As Dto, clientName As String, clientType As String)
			Dim clDto As Workbeat.Entities.DescripcionPuestoDto
			clDto = DirectCast(clientObject, Workbeat.Entities.DescripcionPuestoDto)
			log.Debug("Llamando a WBDescripcionPuestoDal.Delete(EntityId:=" & clDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.DescripcionPuesto, clDto.id, clientName)
			If IsNumeric(map.workbeatId) Then
				Dim result As Integer
				result = APIClient.delete("org/descripcionesPuesto/" & map.workbeatId)
			End If
		End Sub


	End Class

End Namespace