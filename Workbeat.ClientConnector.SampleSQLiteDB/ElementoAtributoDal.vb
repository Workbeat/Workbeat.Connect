Imports System.Data.SQLite
Imports System.Configuration
Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization

Namespace Dal
	Public Class ElementoAtributoDal
		Inherits Workbeat.Entities.Dal.ClientDal

		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(ElementoAtributoDal))

		Public WBAccessToken As String = ""

		Public Overrides Sub Delete(wbObject As Workbeat.Entities.Dto, clientName As String)
			Dim wbDto As Workbeat.Entities.ElementoAtributoDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.ElementoAtributoDto)
			log.Debug("Llamando a SampleSQLiteDB.ElementoAtributoDal.Delete(id:=" & wbDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.ElementoAtributo, wbDto.id, clientName)
			If IsNumeric(map.externalId) Then
				Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
					Try
						dbConn.Open()
						Dim sqlStr As String = String.Empty
						If Not String.IsNullOrEmpty(map.externalId) AndAlso CInt(map.externalId) > 0 Then
							' actualizar ElementoAtributo
							sqlStr = "delete FROM ElementoAtributo where idElemento = @idElementoAtributo"

							Dim cmd = New SQLiteCommand(sqlStr, dbConn)
							cmd.Parameters.Add(New SQLiteParameter("idElementoAtributo", CLng(map.externalId)))
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
			log.Debug("Llamando a SampleSQLiteDB.ElementoAtributoDal.getEntity(clientId:=" & clientId & ")")
			Dim tp As Workbeat.ClientConnector.SampleSQLiteDB.Entities.ElementoAtributo = Nothing
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM ElementoAtributo WHERE idElemento = @idElementoAtributo "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("idElementoAtributo", clientId))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						tp = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.ElementoAtributo
						Dim dto As New Workbeat.Entities.ElementoAtributoDto

						dto.id = reader.GetInt32("idElementoAtributo")
						dto.idAtributo = reader.GetInt32("idAtributo")
						dto.nombre = reader.GetString("nombre")
						dto.referencia = reader.GetString("referencia")
						'dto.fechaUltimoCambio = reader.GetDateTime("fechaUltimoCambio")
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
			log.Debug("Llamando a SampleSQLiteDB.ElementoAtributoDal.getLastUpdated")
			Dim lista As New List(Of Workbeat.Entities.ClientEntity)
			Dim tp As Workbeat.ClientConnector.SampleSQLiteDB.Entities.ElementoAtributo = Nothing
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Workbeat.Entities.Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.ElementoAtributo, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion) * 1000

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM ElementoAtributo WHERE fechaUltimoCambio > @fechaUltimoCambio "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("fechaUltimoCambio", epochDate))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						tp = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.ElementoAtributo
						Dim dto As New Workbeat.Entities.ElementoAtributoDto

						dto.id = reader.GetInt32("idElementoAtributo")
						dto.nombre = reader.GetString("nombre")
						dto.nombre = reader.GetString("referencia")
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
			Dim wbDto As Workbeat.Entities.ElementoAtributoDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.ElementoAtributoDto)
			log.Debug("Llamando a SampleSQLiteDB.ElementoAtributoDal.Save(WorkbeatId:=" & wbDto.id & ", nombre:=" & wbDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.ElementoAtributo, wbDto.id, clientName)

			Dim tpDto As Workbeat.Entities.ElementoAtributoDto
			Dim clEnt As Workbeat.ClientConnector.SampleSQLiteDB.Entities.ElementoAtributo
			clEnt = DirectCast(Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.ElementoAtributo, "SQLiteSample"), Workbeat.ClientConnector.SampleSQLiteDB.Entities.ElementoAtributo)
			tpDto = wbDto.Clone()
			clEnt.data = tpDto
			clEnt.entityId = map.externalId

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					Dim result As Long = 0
					dbConn.Open()
					Dim sqlStr As String = String.Empty
					If tpDto.id > 0 Then
						' actualizar ElementoAtributo
						sqlStr = "update ElementoAtributo set  nombre = @nombre, "
						sqlStr += " referencia=@referencia, "
						sqlStr += " fechaUltimoCambio=@fechaUltimoCambio  "
						sqlStr += " WHERE idElemento = @idElementoAtributo;"

					Else
						' guardar nueva posicion
						sqlStr = "insert into ElementoAtributo (idAtributo, nombre, referencia, fechaUltimoCambio)"
						sqlStr &= "VALUES(@idAtributo, @nombre, @referencia, @fechaUltimoCambio);"
						sqlStr &= "SELECT last_insert_rowid();"
					End If
					' log.Debug(sqlStr)
					'If Not IsDate(tpDto.fechaUltimoCambio) Then
					'	tpDto.fechaUltimoCambio = Now
					'End If

					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("nombre", tpDto.nombre))
					cmd.Parameters.Add(New SQLiteParameter("idAtributo", tpDto.idAtributo))
					cmd.Parameters.Add(New SQLiteParameter("referencia", tpDto.referencia))
					cmd.Parameters.Add(New SQLiteParameter("fechaUltimoCambio", tpDto.fechaUltimoCambio))

					If tpDto.id > 0 Then
						cmd.Parameters.Add(New SQLiteParameter("idElementoAtributo", tpDto.id))
						result = cmd.ExecuteNonQuery()

					Else
						result = cmd.ExecuteScalar()
						tpDto.id = result
						Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
						newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.ElementoAtributo, wbDto.id, clientName)
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
