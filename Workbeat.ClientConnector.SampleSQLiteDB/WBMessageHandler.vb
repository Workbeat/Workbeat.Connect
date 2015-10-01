Imports System.Text
Imports System.Web.Script.Serialization

Namespace WBMCS

	Public Class WBMessageHandler
		Inherits Workbeat.Entities.MessageHandler
		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WBMessageHandler))

		Public Overrides Sub MessageHandler(e As Workbeat.WBMCS.MessageReceivedEventArgs, clientName As String)
			log.Debug("SampleSQLiteDB esta procesando el mensaje: " & e.Message.data)

			Dim js As New JavaScriptSerializer
			Select Case e.Message.messageType
				Case "org.posicion.editar"
					Dim pos As Workbeat.Entities.PosicionDto
					' e.Message.data es un JSON string. hay que deserializarlo y convertirlo a Objeto.
					pos = js.Deserialize(Of Workbeat.Entities.PosicionDto)(e.Message.data)
					Dim posdal As New Workbeat.ClientConnector.SampleSQLiteDB.Dal.PosicionDal
					posdal.Save(pos, clientName)
					log.Debug("posicion guardada: " & e.Message.data)
			End Select

		End Sub
	End Class

End Namespace
