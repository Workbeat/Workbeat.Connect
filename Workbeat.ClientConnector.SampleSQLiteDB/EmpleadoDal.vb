Imports System.Data.SQLite
Imports System.Configuration
Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization


Namespace Dal
	Public Class EmpleadoDal
		Inherits Workbeat.Entities.Dal.ClientDal

		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(EmpleadoDal))

		Private WBAccessToken As String = ""

		Public Overrides Sub Delete(wbObject As Workbeat.Entities.Dto, clientName As String)
			Dim wbDto As Workbeat.Entities.EmpleadoDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.EmpleadoDto)
			log.Debug("Llamando a SampleSQLiteDB.EmpleadoDal.Delete(id:=" & wbDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Empleado, wbDto.id, clientName)
			If IsNumeric(map.externalId) Then
				Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
					Try
						dbConn.Open()
						Dim sqlStr As String = String.Empty
						If Not String.IsNullOrEmpty(map.externalId) AndAlso CInt(map.externalId) > 0 Then
							' NO Borrar Empleado por completo. Solamente borrar sus posiciones asociadas
							sqlStr = "delete PosicionesEmpleado where idPersona = @idEmpleado;"
							Dim cmd = New SQLiteCommand(sqlStr, dbConn)
							cmd.Parameters.Add(New SQLiteParameter("idEmpleado", CLng(map.externalId)))
							cmd.ExecuteNonQuery()
						End If
					Catch ex As Exception
						log.Error("Error al actualizar un objeto de  mapeo:" & ex.Message, ex)
					Finally
						If Not dbConn Is Nothing Then
							dbConn.Close()
						End If
					End Try
				End Using
			End If

		End Sub

		Public Overrides Function getEntity(clientId As String) As Workbeat.Entities.ClientEntity
			log.Debug("Llamando a SampleSQLiteDB.EmpleadoDal.getEntity(clientId:=" & clientId & ")")
			Dim emp As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Empleado = Nothing
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try

					Dim dto As Workbeat.Entities.EmpleadoDto
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM Empleado WHERE idPersona = @idEmpleado "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("idEmpleado", clientId))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						emp = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.Empleado
						dto = New Workbeat.Entities.EmpleadoDto

						dto.id = reader.GetInt32("idPersona")
						dto.nombre = reader.GetString("nombre")
						dto.apellidoPaterno = reader.GetString("apellidoPaterno")
						dto.apellidoMaterno = reader.GetString("apellidoMaterno")

						dto.fechaUltimoCambio = reader.GetDateTime("fechaLastUpdate")

						' fecha de nacimiento no se tiene. Se pone por default la fecha de hoy
						dto.fechaNacimiento = Today

						emp.data = dto

						'dto.activo = 1
						If Not reader.IsClosed() Then
							reader.Close()
						End If

						' sacar las posiciones
						sqlStr = "SELECT * FROM PosicionesEmpleado where idPersona = @idEmpleado"

						cmd = New SQLiteCommand(sqlStr, dbConn)
						cmd.Parameters.Add(New SQLiteParameter("idEmpleado", clientId))
						reader = cmd.ExecuteReader
						Dim posiciones As New List(Of Workbeat.Entities.PosicionEmpleadoDto)
						While reader.Read()
							Dim pos As New Workbeat.Entities.PosicionEmpleadoDto
							pos.id = reader.GetInt32("idPosicion")
							pos.fechaIngreso = reader.GetDateTime("fechaIngreso")
							' por default es planta (idTipoPlantilla = 32)
							pos.idTipoPlantilla = 32
							posiciones.Add(pos)
						End While
						dto.posiciones = posiciones.ToArray()
					End If
				Catch ex As Exception
					log.Error("Error al acceder a la base de datos de mapeo de objetos" & ex.Message, ex)
				Finally
					If Not dbConn Is Nothing Then
						dbConn.Close()
					End If
				End Try
			End Using

			Return emp

		End Function

		Public Overrides Function getLastUpdated(clientName As String) As System.Collections.Generic.List(Of Workbeat.Entities.ClientEntity)
			log.Debug("Llamando a SampleSQLiteDB.EmpleadoDal.getLastUpdated")
			Dim listaEmp As New List(Of Workbeat.Entities.ClientEntity)

			Dim emp As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Empleado = Nothing
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Workbeat.Entities.Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.Empleado, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion) * 1000

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT E.*, PE.idPosicion, PE.fechaIngreso, PE.fechaBaja, P.nombre as nombrePosicion " & _
					 " FROM Empleado As E " & _
					 " INNER JOIN PosicionesEmpleado PE on E.idPersona = PE.idPersona " & _
					 " INNER JOIN Posiciones P on PE.idPosicion = PE.idPosicion " & _
					 " WHERE PE.fechaIngreso > @fechaUltimoCambio OR PE.fechaBaja > @fechaUltimoCambio) " & _
					 " ORDER BY E.idPersona, PE.fechaBaja, PE.fechaIngreso, PE.idPosicion"
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("fechaUltimoCambio", epochDate))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					Dim currentEmpId As Integer = 0

					Dim listaPosDto As List(Of Workbeat.Entities.PosicionEmpleadoDto) = Nothing

					Dim dto As Workbeat.Entities.EmpleadoDto = Nothing
					While reader.Read()

						If currentEmpId <> reader.GetInt32("idPersona") Then
							emp = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.Empleado
							currentEmpId = reader.GetInt32("idPersona")
							dto = New Workbeat.Entities.EmpleadoDto
							dto.id = reader.GetInt32("idPersona")
							dto.nombre = reader.GetString("nombre")
							dto.apellidoPaterno = reader.GetString("apellidoPaterno")
							dto.apellidoMaterno = reader.GetString("apellidoMaterno")
							dto.fechaUltimoCambio = reader.GetDateTime("fechaLastUpdate")
							' fecha de nacimiento no se tiene. Se pone por default la fecha de hoy
							dto.fechaNacimiento = Today
							emp.data = dto
							listaEmp.Add(emp)
							listaPosDto = New List(Of Workbeat.Entities.PosicionEmpleadoDto)
						End If
						Dim pos As New Workbeat.Entities.PosicionEmpleadoDto
						pos.id = reader.GetInt32("idPosicion")
						pos.nombre = reader.GetString("nombrePosicion")
						pos.fechaIngreso = reader.GetDateTime("fechaIngreso")
						pos.fechaBaja = reader.GetDateTime("fechaBaja")
						listaPosDto.Add(pos)
						dto.posiciones = listaPosDto.ToArray()

					End While
					If Not reader.IsClosed() Then
						reader.Close()
					End If
				Catch ex As Exception
					log.Error("Error al acceder a la base de datos de mapeo de objetos" & ex.Message, ex)
				Finally
					If Not dbConn Is Nothing Then
						dbConn.Close()
					End If
				End Try
			End Using

			Return listaEmp
		End Function



		Public Overrides Sub Save(wbObject As Workbeat.Entities.Dto, clientName As String)
			Dim wbDto As Workbeat.Entities.EmpleadoDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.EmpleadoDto)
			log.Debug("Llamando a SampleSQLiteDB.EmpleadoDal.Save(WorkbeatId:=" & wbDto.id & ", nombre:=" & wbDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Empleado, wbDto.id, clientName)
			Dim empDto As Workbeat.Entities.EmpleadoDto
			Dim clEnt As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Empleado
			clEnt = DirectCast(Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.Empleado, "SQLiteSample"), Workbeat.ClientConnector.SampleSQLiteDB.Entities.Empleado)
			empDto = wbDto.Clone()
			clEnt.data = empDto
			clEnt.entityId = map.externalId

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					Dim result As Long = 0
					dbConn.Open()
					Dim sqlStr As String = String.Empty
					If empDto.id > 0 Then
						' actualizar Empleado
						sqlStr = "update Empleado set  nombre = @nombre, apellidoPaterno = @apellidoPat, apellidoMaterno = @apellidoMat, "
						sqlStr += " fechaLastUpdate=@fechaLastUpdate  "
						sqlStr += " WHERE idPersona = @idEmpleado;"

					Else
						' guardar nueva Empleado
						sqlStr = "insert into Empleado (nombre , apellidoPaterno, apellidoMaterno, fechaLastUpdate)"
						sqlStr &= "VALUES(@nombre , @apellidoPat, @apellidoMat, @fechaLastUpdate);"
						sqlStr &= "SELECT last_insert_rowid();"
					End If
					' log.Debug(sqlStr)
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("nombre", empDto.nombre))
					cmd.Parameters.Add(New SQLiteParameter("apellidoPat", empDto.apellidoPaterno))
					cmd.Parameters.Add(New SQLiteParameter("apellidoMat", empDto.apellidoMaterno))
					cmd.Parameters.Add(New SQLiteParameter("fechaLastUpdate", empDto.fechaUltimoCambio))

					If empDto.id > 0 Then
						cmd.Parameters.Add(New SQLiteParameter("idEmpleado", empDto.id))
						result = cmd.ExecuteNonQuery()
					Else
						result = cmd.ExecuteScalar()
						empDto.id = result
						Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
						newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Empleado, wbDto.id, clientName)
						newMap.externalId = result
						newMap.Save()
					End If
					cmd.Dispose()


					' Se asume que las posiciones ya existen, pues ya fueron sincronizadas anteriormente.
					If wbDto.posiciones.Length > 0 Then

						sqlStr = "insert or replace into PosicionesEmpleado (idPosicion , idPersona, fechaIngreso, fechaBaja, fechaLastUpdate)"
						sqlStr &= "VALUES(@idPosicion , @idPersona, @fechaIngreso, @fechaBaja, @fechaLastUpdate);"
						cmd = New SQLiteCommand(sqlStr, dbConn)
						Dim parFechaIngreso = New SQLiteParameter("fechaIngreso")
						cmd.Parameters.Add(parFechaIngreso)
						Dim parFechaBaja = New SQLiteParameter("fechaBaja")
						cmd.Parameters.Add(parFechaBaja)
						Dim parIdPosicion = New SQLiteParameter("idPosicion")
						cmd.Parameters.Add(parIdPosicion)
						Dim parIdPersona = New SQLiteParameter("idPersona")
						cmd.Parameters.Add(parIdPersona)
						Dim parFechaLastUpdate = New SQLiteParameter("fechaLastUpdate")
						cmd.Parameters.Add(parFechaLastUpdate)

						For Each posEmp In wbDto.posiciones
							Dim posMap As Workbeat.Entities.Utilities.ObjectMapper.Map
							posMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Posicion, posEmp.id, clientName)
							If Not String.IsNullOrWhiteSpace(posMap.externalId) Then
								parIdPersona.Value = empDto.id
								parIdPosicion.Value = posMap.externalId
								parFechaIngreso.Value = posEmp.fechaIngreso
								parFechaBaja.Value = posEmp.fechaBaja
								parFechaLastUpdate.Value = empDto.fechaUltimoCambio	'posEmp.fechaUltimoCambio no trae nada.
								cmd.ExecuteNonQuery()
							End If
						Next
						cmd.Dispose()
					End If

				Catch ex As Exception
					log.Error("Error al actualizar un objeto de  mapeo:" & ex.Message, ex)
				Finally
					If Not dbConn Is Nothing Then
						dbConn.Close()
					End If
				End Try
			End Using

		End Sub

	End Class

End Namespace
