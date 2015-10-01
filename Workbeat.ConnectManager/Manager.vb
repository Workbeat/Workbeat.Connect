Public Class Manager

	Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(Manager))

	Private m_syncStrategy As SyncStrategy = Nothing
	Public Property SyncStrategy() As SyncStrategy
		Get
			Return m_syncStrategy
		End Get
		Set(ByVal value As SyncStrategy)
			m_syncStrategy = value
		End Set
	End Property


	' Determina que tipo de cliente es.
	' Flat File, Nomipaq, SAP, etc Dependiendo de este, se da de alta el DAL del cliente por late binding
	Private m_clientType As String
	Public Property clientType() As String
		Get
			Return m_clientType
		End Get
		Set(ByVal value As String)
			m_clientType = value
		End Set
	End Property

	' Determina que cliente especifico es.
	' Puedes tener varias instancias de un tipo de cliente, las cuales deban ser sincronizadas.
	Private m_clientName As String
	Public Property clientName() As String
		Get
			Return m_clientName
		End Get
		Set(ByVal value As String)
			m_clientName = value
		End Set
	End Property


	Public Sub New()

	End Sub

	Public Sub Sync(entityType As Workbeat.Entities.EntityTypes)
		If Not SyncStrategy Is Nothing Then

			m_syncStrategy.Sync(entityType, clientType, clientName)
		Else
			log.Error("Es necesario inicializar la estrategia de sincronización antes de sincronizar.")
			Throw New Exception("Es necesario inicializar la estrategia de sincronización antes de sincronizar.")
		End If
	End Sub


	Public Sub StartListening(clientType As String)

		Dim qc As Workbeat.WBMCS.Client.QConnector = Nothing
		Try
			qc = New Workbeat.WBMCS.Client.QConnector
			' necesario para poder pasarlo al messageHandler
			qc.clientType = clientType
			qc.clientName = clientName

			' Consumiendo mensajes...
			AddHandler qc.MessageReceived, AddressOf onMessageReceived
			' Crea la conexion
			qc.CreateConnection()
			' Comienza a escuchar....
			log.Info("Comenzando a escuchar por eventos ")
			qc.StartListening()
		Catch ex As Exception
			log.Error("Error al conectar con Workbeat WBMCS: ", ex)
			If Not qc Is Nothing Then
				qc.Disconnect()
			End If
		End Try
	End Sub


	Public Sub onMessageReceived(sender As Object, e As Workbeat.WBMCS.MessageReceivedEventArgs)
		' incoming message
		Try
			log.Debug(e.Message.messageType)
			log.Debug(e.Message.data)
			Dim qcon As WBMCS.Client.QConnector
			qcon = DirectCast(sender, WBMCS.Client.QConnector)

			log.Debug("clientType:" & qcon.clientType)

			Dim msgHandler As Workbeat.Entities.MessageHandler
			msgHandler = Workbeat.Entities.MessageHandler.getClientMessageHandler(qcon.clientType)
			msgHandler.MessageHandler(e, qcon.clientName)
		Catch ex As Exception
			log.Error("Error al procesar el mensaje recibido:", ex)
		End Try

	End Sub

End Class
