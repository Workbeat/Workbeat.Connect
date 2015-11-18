Imports System.Data.SQLite
Imports System.Configuration
Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization

Namespace Dal
	Public Class OrganizacionDal
		Inherits Workbeat.Entities.Dal.ClientDal

		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(OrganizacionDal))

		Private WBAccessToken As String = ""

		Public Overrides Sub Delete(wbObject As Workbeat.Entities.Dto, clientName As String)
			Dim wbDto As Workbeat.Entities.OrganizacionDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.OrganizacionDto)
			log.Debug("Llamando a SampleSQLiteDB.OrganizacionDal.Delete(id:=" & wbDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Organizacion, wbDto.id, clientName)
			If IsNumeric(map.externalId) Then
				Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
					Try
						dbConn.Open()
						Dim sqlStr As String = String.Empty
						If Not String.IsNullOrEmpty(map.externalId) AndAlso CInt(map.externalId) > 0 Then
							' actualizar Organizacion
							sqlStr = "Delete FROM Organizacion where idOrganizacion = @idOrganizacion"

							Dim cmd = New SQLiteCommand(sqlStr, dbConn)
							cmd.Parameters.Add(New SQLiteParameter("idOrganizacion", CLng(map.externalId)))
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
			log.Debug("Llamando a SampleSQLiteDB.OrganizacionDal.getEntity(clientId:=" & clientId & ")")
			Dim pos As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Organizacion = Nothing
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM Organizacion WHERE idOrganizacion = @idOrganizacion "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("idOrganizacion", clientId))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						pos = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.Organizacion
						Dim dto As New Workbeat.Entities.OrganizacionDto

						dto.id = reader.GetInt32("idOrganizacion")
						dto.nombre = reader.GetString("nombre")
						dto.fechaCreacion = reader.GetDateTime("fechaCreacion")
						dto.fechaUltimoCambio = reader.GetDateTime("fechaUltimoCambio")
						dto.activo = 1

						pos.data = dto

					End If
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

			Return pos

		End Function

		Public Overrides Function getLastUpdated(clientName As String) As System.Collections.Generic.List(Of Workbeat.Entities.ClientEntity)
			log.Debug("Llamando a SampleSQLiteDB.OrganizacionDal.getLastUpdated")
			Dim lista As New List(Of Workbeat.Entities.ClientEntity)
			Dim pos As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Organizacion = Nothing
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Workbeat.Entities.Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.Organizacion, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion) * 1000

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM Organizacion WHERE fechaUltimoCambio> @fechaUltimoCambio "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("fechaUltimoCambio", epochDate))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						pos = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.Organizacion
						Dim dto As New Workbeat.Entities.OrganizacionDto

						dto.id = reader.GetInt32("idOrganizacion")
						dto.nombre = reader.GetString("nombre")
						dto.fechaCreacion = reader.GetDateTime("fechaCreacion")
						dto.fechaUltimoCambio = reader.GetDateTime("fechaUltimoCambio")
						dto.activo = 1
						pos.data = dto
						lista.Add(pos)
					End If
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

			'TODO: Finish(getLastUpdated)
			Return lista
		End Function

		Public Overrides Sub Save(wbObject As Workbeat.Entities.Dto, clientName As String)
			Dim wbDto As Workbeat.Entities.OrganizacionDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.OrganizacionDto)
			log.Debug("Llamando a SampleSQLiteDB.OrganizacionDal.Save(WorkbeatId:=" & wbDto.id & ", nombre:=" & wbDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Organizacion, wbDto.id, clientName)
			' Esta funcion verifica que exista la Organizacion padre y la da de alta si no.
			' es recursiva para incluir los padres y no haya errores de estructura.
			Dim orgDto As Workbeat.Entities.OrganizacionDto
			Dim clEnt As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Organizacion
			clEnt = DirectCast(Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.Organizacion, "SQLiteSample"), Workbeat.ClientConnector.SampleSQLiteDB.Entities.Organizacion)
			orgDto = wbDto.Clone()
			clEnt.data = orgDto
			clEnt.entityId = map.externalId

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteSampleClientName") & "_DBFile") & ";Version=3;")
				Try
					Dim result As Long = 0
					dbConn.Open()
					Dim sqlStr As String = String.Empty
					If orgDto.id > 0 Then
						' actualizar Organizacion
						sqlStr = "update Organizacion set  nombre = @nombre, "
						sqlStr += " fechaUltimoCambio=@fechaUltimoCambio  "
						sqlStr += " WHERE idOrganizacion = @idOrganizacion;"

					Else
						' guardar nueva Organizacion
						sqlStr = "insert into Organizacion (nombre , fechaCreacion, fechaUltimoCambio)"
						sqlStr &= "VALUES(@nombre , @fechaCreacion, @fechaUltimoCambio);"
						sqlStr &= "SELECT last_insert_rowid();"
					End If
					' log.Debug(sqlStr)
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("nombre", orgDto.nombre))
					' idPadre (idOrganizacionReporta) debe ya estar dada de alta. el id debe corresponder a un id en sqliteSample
					cmd.Parameters.Add(New SQLiteParameter("fechaCreacion", orgDto.fechaCreacion))
					cmd.Parameters.Add(New SQLiteParameter("fechaUltimoCambio", orgDto.fechaUltimoCambio))

					If orgDto.id > 0 Then
						cmd.Parameters.Add(New SQLiteParameter("idOrganizacion", orgDto.id))
						result = cmd.ExecuteNonQuery()
					Else
						result = cmd.ExecuteScalar()
						orgDto.id = result
						Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
						newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Organizacion, wbDto.id, clientName)
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