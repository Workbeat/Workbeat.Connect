Imports System.Data.SQLite
Imports System.Configuration

Namespace Utilities.Sync
	Friend Class EntitySyncDateDal

		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(EntitySyncDateDal))

		Public Function getEntityLastUpdate(clientName As String, entityType As EntityTypes) As DateTime
			Dim fecha As Date = #1/1/1900#
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
				Try
					dbConn.Open()
					Dim sqlStr As String = "SELECT * FROM EntitySyncDate WHERE clientName = @clientName AND entityType = @entityType"
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("entityType", entityType))
					cmd.Parameters.Add(New SQLiteParameter("clientName", clientName))
					Dim reader As SQLiteDataReader = cmd.ExecuteReader
					If reader.Read() Then
						fecha = reader("lastUpdate").ToString
					End If
					If Not reader.IsClosed() Then
						reader.Close()
					End If
				Catch ex As Exception
					log.Error("getEntityLastUpdate:Error al acceder a la base de datos de sincronizacion. Err:" & ex.Message, ex)
				Finally
					If Not dbConn Is Nothing Then
						dbConn.Close()
					End If
				End Try
			End Using
			Return fecha
		End Function


		Public Sub setEntityLastUpdate(clientName As String, entityType As EntityTypes, fecha As DateTime)
			'Dim dbConn As SQLiteConnection = Nothing
			Using dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
				Try
					'dbConn = New SQLiteConnection("Data Source=" & ConfigurationManager.AppSettings("SQLiteDBFile") & ";Version=3;")
					dbConn.Open()
					Dim sqlStr As String = "insert into EntitySyncDate (clientName, entityType, lastUpdate) "
					sqlStr &= "SELECT @clientName, @entityType, @lastUpdate "
					sqlStr &= " WHERE NOT EXISTS(SELECT * FROM EntitySyncDate WHERE entityType=@entityType AND clientName = @clientName );"

					sqlStr &= "update EntitySyncDate set lastUpdate = @lastUpdate "
					sqlStr &= " WHERE entityType=@entityType AND clientName = @clientName;"
					Dim cmd = New SQLiteCommand(sqlStr, dbConn)
					cmd.Parameters.Add(New SQLiteParameter("entityType", entityType))
					cmd.Parameters.Add(New SQLiteParameter("clientName", clientName))
					cmd.Parameters.Add(New SQLiteParameter("lastUpdate", fecha))
					cmd.ExecuteNonQuery()
				Catch ex As Exception
					log.Error("Error al actualizar la fecha de actualizacion de la entidad " & entityType.ToString() & ":" & ex.Message, ex)
				Finally
					If Not dbConn Is Nothing Then
						dbConn.Close()
					End If
				End Try
			End Using
		End Sub

	End Class

End Namespace
