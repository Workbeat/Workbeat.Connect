Imports System.Data.SQLite
Imports System.Configuration
Imports Workbeat.API
Imports System.Text
Imports System.Web.Script.Serialization

Namespace Dal
	Public Class PosicionDal
		Inherits Workbeat.Entities.Dal.ClientDal

		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(PosicionDal))

		Private WBAccessToken As String = ""

		Public Overrides Sub Delete(wbObject As Workbeat.Entities.Dto, clientName As String)
			Dim wbDto As Workbeat.Entities.PosicionDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.PosicionDto)
			log.Debug("Llamando a SampleSQLiteDB.PosicionDal.Delete(id:=" & wbDto.id & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Posicion, wbDto.id, clientName)
			If IsNumeric(map.externalId) Then
				Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteClientName") & "_DBFile") & ";Version=3;")
					Try
						dbConn.Open()
						Dim sqlStr As String = String.Empty
						If Not String.IsNullOrEmpty(map.externalId) AndAlso CInt(map.externalId) > 0 Then
							' actualizar posicion
							sqlStr = "delete Posicion where idPosiocion = @idPosicion"

							Dim cmd = New SQLiteCommand(sqlStr, dbConn)
							cmd.Parameters.Add(New SQLiteParameter("idPosicion", CLng(map.externalId)))
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
			log.Debug("Llamando a SampleSQLiteDB.PosicionDal.getEntity(clientId:=" & clientId & ")")
			Dim pos As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Posicion = Nothing
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM posiciones WHERE idPosicion = @idPosicion "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("idPosicion", clientId))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						pos = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.Posicion
						Dim dto As New Workbeat.Entities.PosicionDto

						dto.id = reader.GetInt32("idPosicion")
						dto.nombre = reader.GetString("nombre")
						dto.nombreOrganizacion = "Mi organizacion" ' se pone un default. no se usa realmente

						' como en la BD de sqlite no existe un idOrganizacion, le asignamos uno por default
						' este numero se obtiene al ver el id de la organizacion a la que pertenecen las posiciones
						dto.idOrganizacion = ConfigurationManager.AppSettings("SQLiteSample_idOrgDefault")

						' en este caso, como ejemplo, el codigo es el id de la posicion. Puede ser cualquier cosa
						dto.codigo = "POS_" & reader.GetInt32("idPosicion").ToString()

						dto.fechaCreacion = #1/1/1900# ' en la BD de SQLite no hay fecha de creacion. se mete un default
						dto.fechaUltimoCambio = reader.GetDateTime("fechaLastUpdate")
						dto.idPosicionReporta = reader.GetInt32("idPadre")
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
			log.Debug("Llamando a SampleSQLiteDB.PosicionDal.getLastUpdated")
			Dim lista As New List(Of Workbeat.Entities.ClientEntity)
			Dim pos As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Posicion = Nothing
			' traer la ultima fecha de actualizacion.
			Dim ultimaFechaActualizacion As DateTime
			ultimaFechaActualizacion = Workbeat.Entities.Utilities.Sync.SyncDate.getEntityLastUpdate(Workbeat.Entities.EntityTypes.Posicion, clientName)
			' convertir fecha a epoch date en milisegundos
			Dim epochDate As Long = DateDiff("s", "01/01/1970 00:00:00", ultimaFechaActualizacion) * 1000

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteClientName") & "_DBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM posiciones WHERE fechaUltimoCambio> @fechaUltimoCambio "
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("fechaUltimoCambio", epochDate))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						pos = New Workbeat.ClientConnector.SampleSQLiteDB.Entities.Posicion
						Dim dto As New Workbeat.Entities.PosicionDto

						dto.id = reader.GetInt32("idPosicion")
						dto.nombre = reader.GetString("nombre")
						dto.nombreOrganizacion = "Mi organizacion" ' se pone un default. no se usa realmente

						' como en la BD de sqlite no existe un idOrganizacion, le asignamos uno por default
						' este numero se obtiene al ver el id de la organizacion a la que pertenecen las posiciones
						dto.idOrganizacion = ConfigurationManager.AppSettings("SQLiteSample_idOrgDefault")

						' en este caso, como ejemplo, el codigo es el id de la posicion. Puede ser cualquier cosa
						dto.codigo = "POS_" & reader.GetInt32("idPosicion").ToString()

						dto.fechaCreacion = #1/1/1900# ' en la BD de SQLite no hay fecha de creacion. se mete un default
						dto.fechaUltimoCambio = reader.GetDateTime("fechaLastUpdate")
						dto.idPosicionReporta = reader.GetInt32("idPadre")
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
			Dim wbDto As Workbeat.Entities.PosicionDto
			wbDto = DirectCast(wbObject, Workbeat.Entities.PosicionDto)
			log.Debug("Llamando a SampleSQLiteDB.PosicionDal.Save(WorkbeatId:=" & wbDto.id & ", nombre:=" & wbDto.nombre & ")")
			Dim map As Workbeat.Entities.Utilities.ObjectMapper.Map
			map = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Posicion, wbDto.id, clientName)
			Dim idPadre As Integer = verificaPadre(wbObject, clientName)
			Dim posDto As Workbeat.Entities.PosicionDto
			Dim clEnt As Workbeat.ClientConnector.SampleSQLiteDB.Entities.Posicion
			clEnt = DirectCast(Workbeat.Entities.ClientEntity.getClientEntity(Workbeat.Entities.EntityTypes.Posicion, "SQLiteSample"), Workbeat.ClientConnector.SampleSQLiteDB.Entities.Posicion)
			posDto = wbDto.Clone()
			clEnt.data = posDto
			clEnt.entityId = map.externalId

			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings(ConfigurationManager.AppSettings("SQLiteClientName") & "_DBFile") & ";Version=3;")
				Try
					Dim result As Long = 0
					dbConn.Open()
					Dim sqlStr As String = String.Empty
					If posDto.id > 0 Then
						' actualizar posicion
						sqlStr = "update Posiciones set  nombre = @nombre, idPadre = @idPadre, "
						sqlStr += " fechaLastUpdate=@fechaLastUpdate  "
						sqlStr += " WHERE idPosicion = @idPosicion;"

					Else
						' guardar nueva posicion
						sqlStr = "insert into Posiciones (nombre , idPadre, fechaLastUpdate)"
						sqlStr &= "VALUES(@nombre , @idPadre, @fechaLastUpdate);SELECT last_insert_rowid();"

					End If

					log.Debug(sqlStr)

					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("nombre", posDto.nombre))
					' idPadre (idPosicionReporta) debe ya estar dada de alta. el id debe corresponder a un id en sqliteSample
					cmd.Parameters.Add(New SQLiteParameter("idPadre", IIf(idPadre = 0, DBNull.Value, idPadre)))	'  posDto.idPosicionReporta))
					cmd.Parameters.Add(New SQLiteParameter("fechaLastUpdate", posDto.fechaUltimoCambio))

					If posDto.id > 0 Then
						cmd.Parameters.Add(New SQLiteParameter("idPosicion", posDto.id))
						result = cmd.ExecuteNonQuery()

					Else
						result = cmd.ExecuteScalar()
						posDto.id = result
						Dim newMap As Workbeat.Entities.Utilities.ObjectMapper.Map
						newMap = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Posicion, wbDto.id, clientName)
						newMap.externalId = result
						newMap.Save()
					End If


					'SELECT last_insert_rowid()


					' TODO: En caso de insercion (nuevo) mapear el Idposicion creado al workbeatId

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

		' verificar que padre ya existe. si no, dar de alta.
		Private Function verificaPadre(wbDto As Workbeat.Entities.PosicionDto, clientName As String) As Integer
			If wbDto.idPosicionReporta = 0 OrElse wbDto.idPosicionReporta Is Nothing Then
				Return 0
			End If

			Dim idpadre As Integer = 0
			Dim mapPadre As Workbeat.Entities.Utilities.ObjectMapper.Map
			mapPadre = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Posicion, wbDto.idPosicionReporta, clientName)

			If IsNumeric(mapPadre.externalId) Then
				idpadre = CInt(mapPadre.externalId)
			Else
				' crear el objeto padre.

				Dim apiObj As New Workbeat.API.Client(ConfigurationManager.AppSettings("workbeat_api_url"))
				If WBAccessToken <> "" Then
					apiObj.setAccessToken(WBAccessToken, Now.AddHours(1))
				Else
					apiObj.Connect(ConfigurationManager.AppSettings("workbeat_client_id"), ConfigurationManager.AppSettings("workbeat_client_secret"))
					WBAccessToken = apiObj.access_token
				End If
				Dim result As String
				result = apiObj.get("/org/posiciones/" & wbDto.idPosicionReporta.ToString)

				Dim wbPos As Workbeat.Entities.PosicionDto
				Dim js As New JavaScriptSerializer
				wbPos = js.Deserialize(Of Workbeat.Entities.PosicionDto)(result)
				' guarda al padre
				Save(wbPos, clientName)
				mapPadre = Workbeat.Entities.Utilities.ObjectMapper.Map.getMapFromWorkbeatId(Workbeat.Entities.EntityTypes.Posicion, wbPos.id, clientName)
				idpadre = CInt(mapPadre.externalId)
			End If
			Return idpadre

		End Function


	End Class
End Namespace