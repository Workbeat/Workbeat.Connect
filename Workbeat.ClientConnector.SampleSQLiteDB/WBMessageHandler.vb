Imports System.Text
Imports System.Web.Script.Serialization

Namespace WBMCS

	Public Class WBMessageHandler
		Inherits Workbeat.Entities.MessageHandler
		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(WBMessageHandler))

		Public Overrides Sub MessageHandler(e As Workbeat.WBMCS.MessageReceivedEventArgs, clientName As String)
			log.Debug("SampleSQLiteDB esta procesando el mensaje: " & e.Message.data)
			Select Case e.Message.messageType
				Case "org.posicion.editar", "org.posicion.nuevo"
					Dim pos As New Workbeat.Entities.PosicionDto
					' e.Message.data es un JSON string. hay que deserializarlo y convertirlo a Objeto.
					pos = DirectCast(Workbeat.Entities.Utilities.JsonConverter.getObject(e.Message.data, pos), Workbeat.Entities.PosicionDto)
					Dim posdal As New Workbeat.ClientConnector.SampleSQLiteDB.Dal.PosicionDal
					' en el objeto no viene la fecha de ultimo cambio ni el tag de activo. Agregarlo
					pos.fechaUltimoCambio = Now
					pos.activo = 1
					posdal.Save(pos, clientName)
					log.Debug("posicion guardada: " & e.Message.data)
				Case "org.posicion.borrar"
					Dim pos As New Workbeat.Entities.PosicionDto
					pos = DirectCast(Workbeat.Entities.Utilities.JsonConverter.getObject(e.Message.data, pos), Workbeat.Entities.PosicionDto)
					Dim posdal As New Workbeat.ClientConnector.SampleSQLiteDB.Dal.PosicionDal
					' en el objeto no viene la fecha de ultimo cambio ni el tag de activo. Agregarlo
					pos.fechaUltimoCambio = Now
					pos.activo = 0
					posdal.Delete(pos, clientName)
					log.Debug("posicion borrada: " & e.Message.data)
					
				Case "org.puesto.editar", "org.puesto.nuevo"
					Dim pue As New Workbeat.Entities.DescripcionPuestoDto
					' e.Message.data es un JSON string. hay que deserializarlo y convertirlo a Objeto.
					pue = DirectCast(Workbeat.Entities.Utilities.JsonConverter.getObject(e.Message.data, pue), Workbeat.Entities.DescripcionPuestoDto)
					Dim puedal As New Workbeat.ClientConnector.SampleSQLiteDB.Dal.DescripcionPuestoDal
					' en el objeto no viene la fecha de ultimo cambio ni el tag de activo. Agregarlo
					pue.fechaUltimoCambio = Now
					pue.activo = 1

					puedal.Save(pue, clientName)
					log.Debug("puesto guardado: " & e.Message.data)

				Case "org.puesto.borrar"
					Dim pue As New Workbeat.Entities.DescripcionPuestoDto
					pue = DirectCast(Workbeat.Entities.Utilities.JsonConverter.getObject(e.Message.data, pue), Workbeat.Entities.DescripcionPuestoDto)
					Dim puedal As New Workbeat.ClientConnector.SampleSQLiteDB.Dal.DescripcionPuestoDal
					' en el objeto no viene la fecha de ultimo cambio ni el tag de activo. Agregarlo
					pue.fechaUltimoCambio = Now
					pue.activo = 0
					puedal.Delete(pue, clientName)
					log.Debug("puesto borrado: " & e.Message.data)
			End Select

		End Sub
	End Class

End Namespace
