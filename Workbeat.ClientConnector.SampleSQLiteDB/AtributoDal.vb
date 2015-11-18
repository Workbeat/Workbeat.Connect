Imports System.Data.SQLite
Imports System.Configuration
Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization

Namespace Dal
	Public Class AtributoDal
		Inherits Workbeat.Entities.Dal.ClientDal

		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(AtributoDal))

		Public WBAccessToken As String = ""

		Public Overrides Sub Delete(wbObject As Workbeat.Entities.Dto, clientName As String)
			Dim wbDto As Workbeat.Entities.AtributoDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.AtributoDto)
			log.Debug("Llamando a SampleSQLiteDB.AtributoDal.Delete(id:=" & wbDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Atributo, wbDto.id, clientName)
			If IsNumeric(map.externalId) Then
				Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
					Try
						dbConn.Open()
						Dim sqlStr As String = String.Empty
						If Not String.IsNullOrEmpty(map.externalId) AndAlso CInt(map.externalId) > 0 Then
							' actualizar Atributo
							sqlStr = "delete FROM Atributo where idAtributo = @idAtributo"

							Dim cmd = New SQLiteCommand(sqlStr, dbConn)
							cmd.Parameters.Add(New SQLiteParameter("idAtributo", CLng(map.externalId)))
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
			log.Debug("Llamando a SampleSQLiteDB.AtributoDal.getEntity(clientId:=" & clientId & ")")
			Dim tp As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Atributo = Nothing
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM Atributo WHERE idAtributo = @idAtributo "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("idAtributo", clientId))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						tp = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.Atributo
						Dim dto As New Workbeat.Entities.AtributoDto

						dto.id = reader.GetInt32("idAtributo")
						dto.nombre = reader.GetString("nombre")
						dto.fechaCreacion = #1/1/1900# ' en la BD de SQLite no hay fecha de creacion. se mete un default
						dto.fechaUltimoCambio = reader.GetDateTime("fechaUltimoCambio")
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
			log.Debug("Llamando a SampleSQLiteDB.AtributoDal.getLastUpdated")
			Dim lista As New List(Of Workbeat.Entities.ClientEntity)
			Dim tp As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Atributo = Nothing
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Workbeat.Entities.Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.Atributo, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion) * 1000

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM Atributo WHERE fechaUltimoCambio > @fechaUltimoCambio "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("fechaUltimoCambio", epochDate))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						tp = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.Atributo
						Dim dto As New Workbeat.Entities.AtributoDto

						dto.id = reader.GetInt32("idAtributo")
						dto.nombre = reader.GetString("nombre")

						dto.fechaCreacion = #1/1/1900# ' en la BD de SQLite no hay fecha de creacion. se mete un default
						dto.fechaUltimoCambio = reader.GetDateTime("fechaUltimoCambio")

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
			Dim wbDto As Workbeat.Entities.AtributoDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.AtributoDto)
			log.Debug("Llamando a SampleSQLiteDB.AtributoDal.Save(WorkbeatId:=" & wbDto.id & ", nombre:=" & wbDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Atributo, wbDto.id, clientName)

			Dim tpDto As Workbeat.Entities.AtributoDto
			Dim clEnt As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Atributo
			clEnt = DirectCast(Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.Atributo, "SQLiteSample"), Workbeat.ClientConnector.SampleSQLiteDB.Entities.Atributo)
			tpDto = wbDto.Clone()
			clEnt.data = tpDto
			clEnt.entityId = map.externalId

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					Dim result As Long = 0
					dbConn.Open()
					Dim sqlStr As String = String.Empty
					If tpDto.id > 0 Then
						' actualizar Atributo
						sqlStr = "update Atributo set  nombre = @nombre, "
						sqlStr += " fechaUltimoCambio=@fechaUltimoCambio  "
						sqlStr += " WHERE idAtributo = @idAtributo;"

					Else
						' guardar nueva posicion
						sqlStr = "insert into Atributo (nombre, fechaUltimoCambio)"
						sqlStr &= "VALUES(@nombre, @fechaUltimoCambio);"
						sqlStr &= "SELECT last_insert_rowid();"
					End If
					' log.Debug(sqlStr)
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("nombre", tpDto.nombre))
					cmd.Parameters.Add(New SQLiteParameter("fechaUltimoCambio", tpDto.fechaUltimoCambio))

					If tpDto.id > 0 Then
						cmd.Parameters.Add(New SQLiteParameter("idAtributo", tpDto.id))
						result = cmd.ExecuteNonQuery()

					Else
						result = cmd.ExecuteScalar()
						tpDto.id = result
						Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
						newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Atributo, wbDto.id, clientName)
						newMap.externalId = result
						newMap.Save()
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
