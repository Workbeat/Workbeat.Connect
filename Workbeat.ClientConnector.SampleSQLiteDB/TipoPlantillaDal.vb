Imports System.Data.SQLite
Imports System.Configuration
Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization

Namespace Dal
	Public Class TipoPlantillaDal
		Inherits Workbeat.Entities.Dal.ClientDal

		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(TipoPlantillaDal))

		Public WBAccessToken As String = ""

		Public Overrides Sub Delete(wbObject As Workbeat.Entities.Dto, clientName As String)
			Dim wbDto As Workbeat.Entities.TipoPlantillaDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.TipoPlantillaDto)
			log.Debug("Llamando a SampleSQLiteDB.TipoPlantillaDal.Delete(id:=" & wbDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.TipoPlantilla, wbDto.id, clientName)
			If IsNumeric(map.externalId) Then
				Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
					Try
						dbConn.Open()
						Dim sqlStr As String = String.Empty
						If Not String.IsNullOrEmpty(map.externalId) AndAlso CInt(map.externalId) > 0 Then
							' actualizar TipoPlantilla
							sqlStr = "delete FROM TipoPlantilla where idTipoPlantilla = @idTipoPlantilla"

							Dim cmd = New SQLiteCommand(sqlStr, dbConn)
							cmd.Parameters.Add(New SQLiteParameter("idTipoPlantilla", CLng(map.externalId)))
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
			log.Debug("Llamando a SampleSQLiteDB.TipoPlantillaDal.getEntity(clientId:=" & clientId & ")")
			Dim tp As Workbeat.ClientConnector.SampleSQLiteDB.Entities.TipoPlantilla = Nothing
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM TipoPlantilla WHERE idTipoPlantilla = @idTipoPlantilla "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("idTipoPlantilla", clientId))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						tp = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.TipoPlantilla
						Dim dto As New Workbeat.Entities.TipoPlantillaDto

						dto.id = reader.GetInt32("idTipoPlantilla")
						dto.nombre = reader.GetString("nombre")


						dto.fechaCreacion = #1/1/1900# ' en la BD de SQLite no hay fecha de creacion. se mete un default
						dto.fechaUltimoCambio = reader.GetDateTime("fechaLastUpdate")


						tp.data = dto

					End If
					If Not reader.IsClosed() Then
						reader.Close()
					End If
				Catch ex As Exception
					log.Error("Error al acceder a la base de datos de SqmpleSQLiteDB" & ex.Message, ex)
				Finally
					If Not dbConn Is Nothing Then
						dbConn.Close()
					End If
				End Try
			End Using

			Return tp

		End Function

		Public Overrides Function getLastUpdated(clientName As String) As System.Collections.Generic.List(Of Workbeat.Entities.ClientEntity)
			log.Debug("Llamando a SampleSQLiteDB.TipoPlantillaDal.getLastUpdated")
			Dim lista As New List(Of Workbeat.Entities.ClientEntity)
			Dim tp As Workbeat.ClientConnector.SampleSQLiteDB.Entities.TipoPlantilla = Nothing
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Workbeat.Entities.Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.TipoPlantilla, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion) * 1000

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM TipoPlantilla WHERE fechaLastUpdate > @fechaUltimoCambio "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("fechaUltimoCambio", epochDate))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						tp = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.TipoPlantilla
						Dim dto As New Workbeat.Entities.TipoPlantillaDto

						dto.id = reader.GetInt32("idTipoPlantilla")
						dto.nombre = reader.GetString("nombre")
						
						dto.fechaCreacion = #1/1/1900# ' en la BD de SQLite no hay fecha de creacion. se mete un default
						dto.fechaUltimoCambio = reader.GetDateTime("fechaLastUpdate")
						
						tp.data = dto
						lista.Add(tp)
					End If
					If Not reader.IsClosed() Then
						reader.Close()
					End If
				Catch ex As Exception
					log.Error("Error al acceder a la base de datos de SampleSQLiteDB" & ex.Message, ex)
				Finally
					If Not dbConn Is Nothing Then
						dbConn.Close()
					End If
				End Try
			End Using

			'TODO: Finish(getLastUpdated)
			Return lista
		End Function

		Public Overrides Sub Save(wbObject As Workbeat.Entities.Dto, clientName As String)
			Dim wbDto As Workbeat.Entities.TipoPlantillaDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.TipoPlantillaDto)
			log.Debug("Llamando a SampleSQLiteDB.TipoPlantillaDal.Save(WorkbeatId:=" & wbDto.id & ", nombre:=" & wbDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.TipoPlantilla, wbDto.id, clientName)
			
			Dim tpDto As Workbeat.Entities.TipoPlantillaDto
			Dim clEnt As Workbeat.ClientConnector.SampleSQLiteDB.Entities.TipoPlantilla
			clEnt = DirectCast(Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.TipoPlantilla, "SQLiteSample"), Workbeat.ClientConnector.SampleSQLiteDB.Entities.TipoPlantilla)
			tpDto = wbDto.Clone()
			clEnt.data = tpDto
			clEnt.entityId = map.externalId

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					Dim result As Long = 0
					dbConn.Open()
					Dim sqlStr As String = String.Empty
					If tpDto.id > 0 Then
						' actualizar TipoPlantilla
						sqlStr = "update TipoPlantilla set  nombre = @nombre, "
						sqlStr += " fechaLastUpdate=@fechaLastUpdate  "
						sqlStr += " WHERE idTipoPlantilla = @idTipoPlantilla;"

					Else
						' guardar nueva posicion
						sqlStr = "insert into TipoPlantilla (nombre, fechaLastUpdate)"
						sqlStr &= "VALUES(@nombre, @fechaLastUpdate);"
						sqlStr &= "SELECT last_insert_rowid();"
					End If
					' log.Debug(sqlStr)
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("nombre", tpDto.nombre))
					cmd.Parameters.Add(New SQLiteParameter("fechaLastUpdate", tpDto.fechaUltimoCambio))

					If tpDto.id > 0 Then
						cmd.Parameters.Add(New SQLiteParameter("idTipoPlantilla", tpDto.id))
						result = cmd.ExecuteNonQuery()

					Else
						result = cmd.ExecuteScalar()
						tpDto.id = result
						Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
						newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.TipoPlantilla, wbDto.id, clientName)
						newMap.externalId = result
						newMap.Save()
					End If

					' TODO: traer el departamento y guardarlo en la BD.


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
