Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Configuration

Namespace Dal

	Public Class WBAtributoDal
		Inherits WorkbeatDal


		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WBAtributoDal))


		Public Overrides Function getEntity(id As String) As Dto
			Dim result As String = APIClient.get("org/atributos/" & id)
			Dim atr As New AtributoDto
			atr = DirectCast(Utilities.JsonConverter.getObject(result, atr), Workbeat.Entities.AtributoDto)
			Return atr
		End Function

		Public Overrides Function getLastUpdated(clientName As String) As System.Collections.Generic.List(Of WorkbeatEntity)
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.Atributo, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion.ToUniversalTime()) * 1000
			Dim result As String = APIClient.get("org/ultimos_cambios/atributos/", "{""actualizado_desde"":" & epochDate.ToString() & "}")
			Dim atributos As AtributoDto() = {}
			If result.IndexOf("TotalRows") > 0 Then
				Dim wbres As New Workbeat.API.PagedResult(Of AtributoDto)
				wbres = DirectCast(Utilities.JsonConverter.getObject(result, wbres), Workbeat.API.PagedResult(Of AtributoDto))
				atributos = wbres.data
			Else
				atributos = DirectCast(Utilities.JsonConverter.getObject(result, atributos), Workbeat.Entities.AtributoDto())
			End If
			Dim list As New System.Collections.Generic.List(Of WorkbeatEntity)
			Dim atrEntity As Workbeat.Entities.WorkbeatEntities.Atributo
			Dim atr As AtributoDto
			For Each atr In atributos
				atrEntity = New WorkbeatEntities.Atributo
				atrEntity.Data = atr
				list.Add(atrEntity)
			Next
			Return list
		End Function


		Public Overrides Sub Save(clientObject As Dto, clientName As String, clientType As String)
			Dim clientDto As Workbeat.Entities.AtributoDto
			clientDto = DirectCast(clientObject, Workbeat.Entities.AtributoDto)
			Dim clEnt As Workbeat.Entities.ClientEntities.Atributo
			clEnt = Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.Atributo, clientType)
			clEnt.data = clientDto
			log.Debug("Llamando a WBAtributoDal.Save(EntityId:=" & clEnt.entityId & ", nombre:=" & clientDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Atributo, clEnt.entityId, clientName)

			Dim tpDto As Workbeat.Entities.AtributoDto
			Dim wbEnt As New Workbeat.Entities.WorkbeatEntities.Atributo
			' copiar los datos del Atributo al objeto workbeat.
			tpDto = clientObject.Clone()
			wbEnt.Data = tpDto
			wbEnt.workbeatId = map.workbeatId
			Dim jsonData As String = Utilities.JsonConverter.getJsonObj(wbEnt.Data)
			Dim result As String
			If IsNumeric(wbEnt.workbeatId) Then
				' actualizar
				result = APIClient.post("org/atributos/" & wbEnt.workbeatId, jsonData)
			Else
				' es nuevo.
				result = APIClient.post("org/atributos/", jsonData)
				Dim newAtrDto As New Workbeat.Entities.AtributoDto
				newAtrDto = DirectCast(Utilities.JsonConverter.getObject(result, newAtrDto), Workbeat.Entities.AtributoDto)
				wbEnt.Data = newAtrDto
				Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
				newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Atributo, clEnt.entityId, clientName)
				newMap.workbeatId = wbEnt.workbeatId
				newMap.Save()
			End If
		End Sub

		Public Overrides Sub Delete(clientObject As Dto, clientName As String, clientType As String)
			Dim clDto As Workbeat.Entities.AtributoDto
			clDto = DirectCast(clientObject, Workbeat.Entities.AtributoDto)
			log.Debug("Llamando a WBAtributoDal.Delete(EntityId:=" & clDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Atributo, clDto.id, clientName)
			If IsNumeric(map.workbeatId) Then
				Dim result As Integer
				result = APIClient.delete("org/atributos/" & map.workbeatId)
			End If
		End Sub


	End Class

End Namespace