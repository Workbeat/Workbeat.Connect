Imports Workbeat.API
Imports System.Text
Imports System.Configuration

Namespace Dal

	Public Class WBTipoPlantillaDal
		Inherits WorkbeatDal


		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WBTipoPlantillaDal))


		Public Overrides Function getEntity(id As String) As Dto
			Dim result As String = APIClient.get("org/tiposPlantilla/" & id)
			Dim tp As New TipoPlantillaDto
			tp = DirectCast(Utilities.JsonConverter.getObject(result, tp), TipoPlantillaDto)
			Return tp
		End Function

		Public Overrides Function getLastUpdated(clientName As String) As System.Collections.Generic.List(Of WorkbeatEntity)
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.TipoPlantilla, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion.ToUniversalTime()) * 1000
			Dim result As String = APIClient.get("org/ultimos_cambios/tiposPlantilla/", "{""actualizado_desde"":" & epochDate.ToString() & "}")
			Dim tiposPlantilla As TipoPlantillaDto() = {}
			If result.IndexOf("TotalRows") > 0 Then
				Dim wbres As New Workbeat.API.PagedResult(Of TipoPlantillaDto)
				wbres = DirectCast(Utilities.JsonConverter.getObject(result, wbres), Workbeat.API.PagedResult(Of TipoPlantillaDto))
				tiposPlantilla = wbres.data
			Else
				tiposPlantilla = DirectCast(Utilities.JsonConverter.getObject(result, tiposPlantilla), Workbeat.Entities.TipoPlantillaDto())
			End If
			Dim list As New System.Collections.Generic.List(Of WorkbeatEntity)
			Dim tpEntity As Workbeat.Entities.WorkbeatEntities.TipoPlantilla
			Dim tp As TipoPlantillaDto
			For Each tp In tiposPlantilla
				tpEntity = New WorkbeatEntities.TipoPlantilla
				tpEntity.Data = tp
				list.Add(tpEntity)
			Next
			Return list
		End Function


		Public Overrides Sub Save(clientObject As Dto, clientName As String, clientType As String)
			Dim clientDto As Workbeat.Entities.TipoPlantillaDto
			clientDto = DirectCast(clientObject, Workbeat.Entities.TipoPlantillaDto)
			Dim clEnt As Workbeat.Entities.ClientEntities.TipoPlantilla
			clEnt = Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.TipoPlantilla, clientType)
			clEnt.data = clientDto
			log.Debug("Llamando a WBTipoPlantillaDal.Save(EntityId:=" & clEnt.entityId & ", nombre:=" & clientDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.TipoPlantilla, clEnt.entityId, clientName)

			Dim tpDto As Workbeat.Entities.TipoPlantillaDto
			Dim wbEnt As New Workbeat.Entities.WorkbeatEntities.TipoPlantilla
			' copiar los datos del tipo de plantilla al objeto workbeat.
			tpDto = clientObject.Clone()
			wbEnt.Data = tpDto
			wbEnt.workbeatId = map.workbeatId
			Dim jsonData As String = Utilities.JsonConverter.getJsonObj(wbEnt.Data)
			Dim result As String
			If IsNumeric(wbEnt.workbeatId) Then
				' actualizar
				result = APIClient.post("org/tiposPlantilla/" & wbEnt.workbeatId, jsonData)
			Else
				' es nuevo.
				result = APIClient.post("org/tiposPlantilla/", jsonData)
				Dim newTPDto As New Workbeat.Entities.TipoPlantillaDto
				newTPDto = Utilities.JsonConverter.getObject(result, newTPDto)

				wbEnt.Data = newTPDto
				Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
				newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.TipoPlantilla, clEnt.entityId, clientName)
				newMap.workbeatId = wbEnt.workbeatId
				newMap.Save()
			End If
		End Sub

		Public Overrides Sub Delete(clientObject As Dto, clientName As String, clientType As String)
			Dim clDto As Workbeat.Entities.TipoPlantillaDto
			clDto = DirectCast(clientObject, Workbeat.Entities.TipoPlantillaDto)
			log.Debug("Llamando a WBTipoPlantillaDal.Delete(EntityId:=" & clDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.TipoPlantilla, clDto.id, clientName)
			If IsNumeric(map.workbeatId) Then
				Dim result As Integer
				result = APIClient.delete("org/tiposPlantilla/" & map.workbeatId)
			End If
		End Sub


	End Class

End Namespace