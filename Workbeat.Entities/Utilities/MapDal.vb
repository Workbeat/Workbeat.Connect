Imports System.Data.SQLite
Imports System.Configuration

Namespace Utilities.ObjectMapper


	Friend Class MapDal

		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(MapDal))

		Sub New()
			If Not (System.IO.File.Exists(ConfigurationManager.AppSettings("SQLiteDBFile"))) Then
				CreateObjectMappingDatabase()
			End If
		End Sub

		Function getMapFromWorkbeatId(clientName As String, entityType As EntityTypes, workbeatId As String) As Map
			'Dim dbConn As SQLiteConnection = Nothing
			Dim objMap As New Map
			objMap.entityType = entityType
			objMap.workbeatId = workbeatId
			objMap.clientName = clientName
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
				Try
					'dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM wbObjectMap WHERE workbeatId = @workbeatId AND entityType = @entityType AND clientName = @clientName"
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("workbeatId", workbeatId))
					cmd.Parameters.Add(New SQLiteParameter("entityType", entityType))
					cmd.Parameters.Add(New SQLiteParameter("clientName", clientName))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						objMap.externalId = reader("externalId").ToString
						objMap.isNew = False
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

			Return objMap
		End Function

		Function getMapFromExternalId(clientName As String, entityType As EntityTypes, externalId As String) As Map
			' Dim dbConn As SQLiteConnection = Nothing
			Dim objMap As New Map
			objMap.entityType = entityType
			objMap.externalId = externalId
			objMap.clientName = clientName
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM wbObjectMap WHERE externalId = @externalId AND entityType = @entityType AND clientName = @clientName"
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("externalId", externalId))
					cmd.Parameters.Add(New SQLiteParameter("entityType", entityType))
					cmd.Parameters.Add(New SQLiteParameter("clientName", clientName))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						objMap.workbeatId = reader("workbeatId").ToString
						objMap.isNew = False
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

			Return objMap
		End Function

		Public Sub Save(objMap As Map)
			If objMap.isNew Then
				saveNewMap(objMap)
			Else
				updateMap(objMap)
			End If

		End Sub

		Public Sub updateMap(objMap As Map)
			'Dim dbConn As SQLiteConnection = Nothing
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
				Try
					'dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
					dbConn.Open()
					Dim sqlStr As String = "update wbObjectMap set workbeatId = @workbeatId, externalId = @externalId "
					sqlStr += " WHERE entityType=@entityType AND clientName = @clientName "
					sqlStr += " AND (workbeatId = @workbeatId or externalId = @externalId)"
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("externalId", objMap.externalId))
					cmd.Parameters.Add(New SQLiteParameter("workbeatId", objMap.workbeatId))
					cmd.Parameters.Add(New SQLiteParameter("entityType", objMap.entityType))
					cmd.Parameters.Add(New SQLiteParameter("clientName", objMap.clientName))
					cmd.ExecuteNonQuery()
				Catch ex As Exception
					log.Error("Error al actualizar un objeto de  mapeo:" & ex.Message, ex)
				Finally
					If Not dbConn Is Nothing Then
						dbConn.Close()
					End If
				End Try
			End Using
		End Sub

		Public Sub saveNewMap(objMap As Map)
			'Dim dbConn As SQLiteConnection = Nothing
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
				Try
					'dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
					dbConn.Open()
					Dim sqlStr As String = "insert into wbObjectMap (clientName, entityType, workbeatId, externalId)"
					sqlStr &= " VALUES(@clientName, @entityType, @workbeatId, @externalId)"
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("externalId", objMap.externalId))
					cmd.Parameters.Add(New SQLiteParameter("workbeatId", objMap.workbeatId))
					cmd.Parameters.Add(New SQLiteParameter("entityType", objMap.entityType))
					cmd.Parameters.Add(New SQLiteParameter("clientName", objMap.clientName))
					cmd.ExecuteNonQuery()
				Catch ex As Exception
					log.Error("Error al actualizar un objeto de  mapeo:" & ex.Message, ex)
				Finally
					If Not dbConn Is Nothing Then
						dbConn.Close()
					End If
				End Try
			End Using
		End Sub

		Public Sub CreateObjectMappingDatabase()
			log.Info("No existe base de datos en " & ConfigurationManager.AppSettings("SQLiteDBFile") & ". Creando Archivo.")
			If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SQLiteDBFile").ToString) Then
				'Dim dbConn As SQLiteConnection = Nothing
				Try
					SQLiteConnection.CreateFile(ConfigurationManager.AppSettings("SQLiteDBFile"))

					Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
						Try
							'dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
							dbConn.Open()

							Dim sqlStr As String = "CREATE TABLE wbObjectMap(clientName varchar(50), entityType INT, workbeatId VARCHAR(50), externalId VARCHAR(50));"
							sqlStr &= "CREATE TABLE EntitySyncDate (clientName VARCHAR(50), entityType INTEGER, lastUpdate DATETIME NOT NULL, PRIMARY KEY(clientName,entityType));"
							sqlStr &= "CREATE TABLE Log (LogId INTEGER PRIMARY KEY, Date DATETIME NOT NULL, Level VARCHAR(50) NOT NULL,Logger VARCHAR(255) NOT NULL, Message TEXT DEFAULT NULL);"

							Dim cmd = New SQLiteCommand(sqlStr, dbConn)
							cmd.ExecuteNonQuery()
						Catch ex As Exception
							log.Error("Error al crear la base de datos:" & ex.Message, ex)
						Finally
							If Not dbConn Is Nothing Then
								dbConn.Close()
							End If
						End Try
					End Using

				Catch ex As Exception
					log.Error("Error al crear la base de datos:" & ex.Message, ex)
				End Try

			Else
				log.Error("No existe en el app.config la entrada de SQLiteDBFile.")
				Throw New Exception("No existe en el app.config la entrada de SQLiteDBFile.")
			End If
		End Sub


	End Class

End Namespace
