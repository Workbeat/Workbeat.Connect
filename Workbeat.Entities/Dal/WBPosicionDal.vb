Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Configuration

Namespace Dal

	Public Class WBPosicionDal
		Inherits WorkbeatDal


		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WBPosicionDal))


		Public Overrides Function getEntity(id As String) As Dto
			Dim result As String = APIClient.get("org/posiciones/" & id)
			Dim pos As PosicionDto
			Dim js As New JavaScriptSerializer
			pos = js.Deserialize(Of Workbeat.Entities.PosicionDto)(result)
			Return pos
		End Function

		Public Overrides Function getLastUpdated(clientName As String) As System.Collections.Generic.List(Of WorkbeatEntity)
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.Posicion, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion) * 1000
			Dim result As String = APIClient.get("org/ultimos_cambios/posiciones/", "{""actualizado_desde"":" & epochDate.ToString() & "}")
			Dim posiciones As PosicionDto()
			Dim js As New JavaScriptSerializer
			If result.IndexOf("TotalRows") > 0 Then
				Dim wbres As Workbeat.API.PagedResult(Of PosicionDto)
				wbres = js.Deserialize(Of Workbeat.API.PagedResult(Of PosicionDto))(result)
				posiciones = wbres.data
			Else
				posiciones = js.Deserialize(Of Workbeat.Entities.PosicionDto())(result)
			End If
			Dim list As New System.Collections.Generic.List(Of WorkbeatEntity)
			Dim posEntity As Workbeat.Entities.WorkbeatEntities.Posicion
			Dim pos As PosicionDto
			For Each pos In posiciones
				posEntity = New WorkbeatEntities.Posicion
				posEntity.Data = pos
				list.Add(posEntity)
			Next
			Return list
		End Function


		Public Overrides Sub Save(clientObject As Dto, clientName As String, clientType As String)
			Dim clientDto As Workbeat.Entities.PosicionDto
			clientDto = DirectCast(clientObject, Workbeat.Entities.PosicionDto)
			Dim clEnt As Workbeat.Entities.ClientEntities.Posicion
			clEnt = Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.Posicion, clientType)
			clEnt.data = clientDto
			log.Debug("Llamando a WBPosicionDal.Save(EntityId:=" & clEnt.entityId & ", nombre:=" & clientDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Posicion, clEnt.entityId, clientName)
			Dim idPadre As Integer = verificaPadre(clientObject, clientName, clientType)
			Dim posDto As Workbeat.Entities.PosicionDto
			Dim wbEnt As New Workbeat.Entities.WorkbeatEntities.Posicion
			' copiar los datos de la posicion al objeto workbeat.
			posDto = clientObject.Clone()
			posDto.idPosicionReporta = idPadre
			wbEnt.Data = posDto
			wbEnt.workbeatId = map.workbeatId
			Dim js As New JavaScriptSerializer
			Dim jsonData As String = js.Serialize(wbEnt.Data)
			Dim result As String
			If IsNumeric(wbEnt.workbeatId) Then
				' actualizar
				result = APIClient.post("org/posiciones/" & wbEnt.workbeatId, jsonData)
			Else
				' es nuevo.
				result = APIClient.post("org/posiciones/", jsonData)
				Dim newPosDto As Workbeat.Entities.PosicionDto
				newPosDto = js.Deserialize(Of Workbeat.Entities.PosicionDto)(result)
				wbEnt.Data = newPosDto
				Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
				newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Posicion, clEnt.entityId, clientName)
				newMap.workbeatId = wbEnt.workbeatId
				newMap.Save()
			End If
		End Sub


		Public Overrides Sub Delete(clientObject As Dto, clientName As String, clientType As String)
			Dim clDto As Workbeat.Entities.PosicionDto
			clDto = DirectCast(clientObject, Workbeat.Entities.PosicionDto)
			log.Debug("Llamando a WBPosicionDal.Delete(EntityId:=" & clDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Posicion, clDto.id, clientName)
			If IsNumeric(map.workbeatId) Then
				Dim result As Integer
				result = APIClient.delete("org/posiciones/" & map.workbeatId)
			End If
		End Sub



		' verificar que padre ya existe. si no, dar de alta.
		Private Function verificaPadre(clDto As Workbeat.Entities.PosicionDto, clientName As String, clientType As String) As Integer
			If clDto.idPosicionReporta <= 0 OrElse clDto.idPosicionReporta Is Nothing Then
				If clDto.idPosicionReporta Is Nothing Then
					Return 0
				Else
					Return clDto.idPosicionReporta
				End If
			End If

			Dim idpadre As Integer = 0
			Dim mapPadre As Workbeat.Entities.Utilities.ObjectMapper.Map
			mapPadre = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Posicion, clDto.idPosicionReporta, clientName)

			If IsNumeric(mapPadre.workbeatId) Then
				idpadre = CInt(mapPadre.workbeatId)
			Else
				' crear el objeto padre.
				Dim dal As Workbeat.Entities.Dal.ClientDal
				dal = Workbeat.Entities.Dal.ClientDal.getClientDal(clientType, EntityTypes.Posicion)
				Dim clEnt As Workbeat.Entities.ClientEntities.Posicion
				clEnt = dal.getEntity(clDto.idPosicionReporta)
				' guarda al padre
				Save(clEnt.data, clientName, clientType)
				mapPadre = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Posicion, clEnt.entityId, clientName)
				idpadre = CInt(mapPadre.workbeatId)
			End If
			Return idpadre
		End Function



	End Class

End Namespace
