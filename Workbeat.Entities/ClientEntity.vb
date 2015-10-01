Imports System.Collections
Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Reflection

Public MustInherit Class ClientEntity

	'Protected m_data As Dto
	Public MustOverride Property data() As Dto
	Public MustOverride Property entityId As String
	Public MustOverride Property fechaLastUpdate As DateTime
	Public MustOverride Property active As Boolean

	Public Shared Function getClientEntity(entityType As EntityTypes, clientType As String) As ClientEntity

		Dim strEntityType As String = entityType.ToString ' [Enum].GetName(Type.GetType("Workbeat.Entities.entityTypes"), entityType)

		Dim settings As NameValueCollection = DirectCast(ConfigurationManager.GetSection("workbeat.connect"), NameValueCollection)
		Dim filePath As String = settings.Get(clientType & "_DllReference")
		Dim clientEntityNamespace As String = settings.Get(clientType & "_ClientEntityNamespace")

		Dim asbly As Assembly
		asbly = Assembly.LoadFrom(filePath)
		Dim dalType As Type
		Dim dalObjectName As String = clientEntityNamespace & "." & strEntityType
		dalType = asbly.[GetType](dalObjectName)

		If dalType Is Nothing Then
			Dim log As log4net.ILog = log4net.LogManager.GetLogger(GetType(ClientEntity))
			log.Error(String.Format("El tipo de dato " & dalObjectName & " no pudo ser encontrado"))
			Throw New ArgumentException(String.Format("El tipo de dato " & dalObjectName & " no pudo ser encontrado"))
		End If
		Return DirectCast(Activator.CreateInstance(dalType), ClientEntity)


	End Function

End Class

