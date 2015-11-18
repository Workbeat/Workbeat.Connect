Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Configuration

Namespace Dal

	Public Class WBEmpleadoDal
		Inherits WorkbeatDal


		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WBEmpleadoDal))


		Public Overrides Function getEntity(id As String) As Dto
			Dim result As String = APIClient.get("adm/empleados/" & id)
			Dim emp As New EmpleadoDto
			emp = DirectCast(Utilities.JsonConverter.getObject(result, emp), Workbeat.Entities.EmpleadoDto)
			Return emp
		End Function

		Public Overrides Function getLastUpdated(clientName As String) As System.Collections.Generic.List(Of WorkbeatEntity)
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.Empleado, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion.ToUniversalTime()) * 1000
			Dim result As String = APIClient.get("adm/ultimos_cambios/empleados/", "{""actualizado_desde"":" & epochDate.ToString() & "}")
			Dim empleados As EmpleadoDto() = {}
			If result.IndexOf("TotalRows") > 0 Then
				Dim wbres As New Workbeat.API.PagedResult(Of EmpleadoDto)
				wbres = DirectCast(Utilities.JsonConverter.getObject(result, wbres), Workbeat.API.PagedResult(Of EmpleadoDto))
				empleados = wbres.data
			Else
				empleados = DirectCast(Utilities.JsonConverter.getObject(result, empleados), Workbeat.Entities.EmpleadoDto())
			End If
			Dim list As New System.Collections.Generic.List(Of WorkbeatEntity)
			Dim empEntity As Workbeat.Entities.WorkbeatEntities.Empleado
			Dim emp As EmpleadoDto
			For Each emp In empleados
				empEntity = New WorkbeatEntities.Empleado
				empEntity.Data = emp
				list.Add(empEntity)
			Next
			Return list
		End Function

		Public Overrides Sub Save(clientObject As Dto, clientName As String, clientType As String)
			Dim isNew As Boolean = False
			Dim clientDto As Workbeat.Entities.EmpleadoDto
			clientDto = DirectCast(clientObject, Workbeat.Entities.EmpleadoDto)
			Dim clEnt As Workbeat.Entities.ClientEntities.Empleado
			clEnt = Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.Empleado, clientType)
			clEnt.Data = clientDto
			log.Debug("Llamando a WBEmpleadoDal.Save(EntityId:=" & clEnt.entityId & ", nombre:=" & clientDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Empleado, clEnt.entityId, clientName)

			Dim empDto As Workbeat.Entities.EmpleadoDto
			Dim wbEnt As New Workbeat.Entities.WorkbeatEntities.Empleado
			' copiar los datos del empleado al objeto workbeat.
			empDto = clientObject.Clone()
			wbEnt.Data = empDto
			wbEnt.workbeatId = map.workbeatId
			Dim jsondata As String = Utilities.JsonConverter.getJsonObj(wbEnt.Data)
			Dim result As String
			If IsNumeric(wbEnt.workbeatId) Then
				' actualizar
				' TODO: Verificar los apis de empleado para actualizacion.
				result = APIClient.post("adm/empleados/" & wbEnt.workbeatId, jsondata)
			Else
				' es nuevo.
				result = APIClient.post("adm/empleados/", jsondata)
				Dim newEmpDto As New Workbeat.Entities.EmpleadoDto
				newEmpDto = DirectCast(Utilities.JsonConverter.getObject(result, newEmpDto), Workbeat.Entities.EmpleadoDto)
				wbEnt.Data = newEmpDto
				Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
				newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromExternalId(Workbeat.Entities.EntityTypes.Empleado, clEnt.entityId, clientName)
				newMap.workbeatId = wbEnt.workbeatId
				newMap.Save()
				isNew = True
			End If

			' NOTA: El dar de alta un nuevo empleado, lo crea con la asignacion a la posicion que incluye.
			' sin embargo, para actualizar posiciones, es necesario manejarlo como movimientos de personal.



		End Sub

		Public Overrides Sub Delete(clientObject As Dto, clientName As String, clientType As String)
			' TODO: Una baja de empleado es un Movimiento de personal. 
			' Hay que dar de baja al empleado de las posiciones indicadas
			' El empleado realmente no se borra.

		End Sub

	End Class

End Namespace
