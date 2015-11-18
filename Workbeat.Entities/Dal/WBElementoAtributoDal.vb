Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Configuration

Namespace Dal

	Public Class WBElementoAtributoDal
		Inherits WorkbeatDal


		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WBElementoAtributoDal))


		Public Overrides Function getEntity(id As String) As Dto
			' id es un string compuesto por "{idAtributo}_{idElementoAtributo}"
			Dim ids() As String = id.Split("_")

			Dim result As String = APIClient.get("org/atributos/" & ids(0) & "/elementos/" & ids(1))
			Dim ea As New ElementoAtributoDto
			ea = DirectCast(Utilities.JsonConverter.getObject(result, ea), Workbeat.Entities.ElementoAtributoDto)
			Return ea
		End Function

		Public Overrides Function getLastUpdated(clientName As String) As System.Collections.Generic.List(Of WorkbeatEntity)
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.ElementoAtributo, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion.ToUniversalTime()) * 1000
			Dim result As String = APIClient.get("org/ultimos_cambios/elementosAtributo/", "{""actualizado_desde"":" & epochDate.ToString() & "}")
			Dim elementos As ElementoAtributoDto() = {}
			If result.IndexOf("TotalRows") > 0 Then
				Dim wbres As New Workbeat.API.PagedResult(Of ElementoAtributoDto)
				wbres = DirectCast(Utilities.JsonConverter.getObject(result, wbres), Workbeat.API.PagedResult(Of ElementoAtributoDto))
				elementos = wbres.data
			Else
				elementos = DirectCast(Utilities.JsonConverter.getObject(result, elementos), Workbeat.Entities.ElementoAtributoDto())
			End If
			Dim list As New System.Collections.Generic.List(Of WorkbeatEntity)
			Dim eaEntity As Workbeat.Entities.WorkbeatEntities.ElementoAtributo
			Dim ea As ElementoAtributoDto
			For Each ea In elementos
				eaEntity = New WorkbeatEntities.ElementoAtributo
				eaEntity.Data = ea
				list.Add(eaEntity)
			Next
			Return list
		End Function


		Public Overrides Sub Save(clientObject As Dto, clientName As String, clientType As String)
			Dim clientDto As Workbeat.Entities.ElementoAtributoDto
			clientDto = DirectCast(clientObject, Workbeat.Entities.ElementoAtributoDto)
			Dim clEnt As Workbeat.Entities.ClientEntities.ElementoAtributo
			clEnt = Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.ElementoAtributo, clientType)
			clEnt.data = clientDto
			log.Debug("Llamando a WBElementoAtributoDal.Save(EntityId:=" & clEnt.entityId & ", nombre:=" & clientDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map

			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.ElementoAtributo, clEnt.entityId, clientName)
			Dim eaDto As Workbeat.Entities.ElementoAtributoDto
			Dim wbEnt As New Workbeat.Entities.WorkbeatEntities.ElementoAtributo
			' copiar los datos del Elemento Atributo al objeto workbeat.
			eaDto = clientObject.Clone()
			wbEnt.Data = eaDto

			If String.IsNullOrWhiteSpace(map.workbeatId) Then
				map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Atributo, clientDto.idAtributo, clientName)
				If (map.workbeatId) Then
					eaDto.idAtributo = map.workbeatId
					eaDto.id = 0
				Else
					' error: no se puede crear un elemento de un idAtributo que no este mapeado.
					Throw New ApplicationException("No se puede crear un elemento de un atributo que no este mapeado anteriormente. Sincronice primero los atributos")
				End If

			Else
				' actualiza id e idAtributo.
				wbEnt.workbeatId = map.workbeatId
			End If

			Dim jsonData As String = Utilities.JsonConverter.getJsonObj(wbEnt.Data)
			Dim result As String
			If (eaDto.id > 0) Then
				' actualizar
				result = APIClient.post("org/atributos/" & eaDto.idAtributo & "/elementos/" & eaDto.id, jsonData)
			Else
				' es nuevo.
				result = APIClient.post("org/atributos/" & eaDto.idAtributo & "/elementos/", jsonData)
				Dim newEADto As New Workbeat.Entities.ElementoAtributoDto
				newEADto = DirectCast(Utilities.JsonConverter.getObject(result, newEADto), Workbeat.Entities.ElementoAtributoDto)
				wbEnt.Data = newEADto
				Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
				newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.ElementoAtributo, clEnt.entityId, clientName)
				newMap.workbeatId = wbEnt.workbeatId
				newMap.Save()
			End If
		End Sub

		Public Overrides Sub Delete(clientObject As Dto, clientName As String, clientType As String)
			Dim clDto As Workbeat.Entities.ElementoAtributoDto
			clDto = DirectCast(clientObject, Workbeat.Entities.ElementoAtributoDto)
			log.Debug("Llamando a WBElementoAtributoDal.Delete(EntityId:=" & clDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.ElementoAtributo, clDto.id, clientName)
			If Not String.IsNullOrWhiteSpace(map.workbeatId) Then
				' necesario instanciar las entidades para al conversion de idAtributo / idElemento.
				Dim wbEnt As New Workbeat.Entities.WorkbeatEntities.ElementoAtributo
				Dim eaDto As Workbeat.Entities.ElementoAtributoDto
				eaDto = clientObject.Clone()
				wbEnt.Data = eaDto
				wbEnt.workbeatId = map.workbeatId
				Dim result As Integer
				result = APIClient.delete("org/atributos/" & eaDto.idAtributo & "/elementos/" & eaDto.id)
			End If
		End Sub


	End Class

End Namespace