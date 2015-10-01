Imports System.Collections
Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Reflection


Public MustInherit Class MessageHandler
	Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(MessageHandler))

	Public MustOverride Sub MessageHandler(e As Workbeat.WBMCS.MessageReceivedEventArgs, clientName As String)

	Public Shared Function getClientMessageHandler(clientType As String) As MessageHandler
		Dim log2 As log4net.ILog = log4net.LogManager.GetLogger(GetType(MessageHandler))

		Dim settings As NameValueCollection = DirectCast(ConfigurationManager.GetSection("workbeat.connect"), NameValueCollection)
		Dim filePath As String = settings.Get(clientType & "_DllReference")

		Dim clientWBMCSMessageHandlerClass As String = settings.Get(clientType & "_ClientWBMCSHandlerClass")

		Dim asbly As Assembly
		asbly = Assembly.LoadFrom(filePath)
		Dim dalType As Type
		Dim dalObjectName As String = clientWBMCSMessageHandlerClass

		log2.Debug("clientType:" & clientType)
		log2.Debug("filePath:" & filePath)
		log2.Debug("OBjectName: " & clientWBMCSMessageHandlerClass)

		dalType = asbly.[GetType](dalObjectName)

		If dalType Is Nothing Then
			Dim log As log4net.ILog = log4net.LogManager.GetLogger(GetType(MessageHandler))
			log.Error(String.Format("El tipo de dato " & dalObjectName & " no pudo ser encontrado"))
			Throw New ArgumentException(String.Format("El tipo de dato " & dalObjectName & " no pudo ser encontrado"))
		End If
		Return DirectCast(Activator.CreateInstance(dalType), MessageHandler)


	End Function

End Class
