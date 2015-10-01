Imports System.Collections
Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Reflection

Namespace Dal
	Public MustInherit Class ClientDal



		Protected m_clientType As String

		Protected Sub New()
			' solo puede crearse con el factory
		End Sub

		Public MustOverride Sub Save(wbObject As Workbeat.Entities.Dto, clientName As String)
		Public MustOverride Sub Delete(wbObject As Dto, clientName As String)

		' trae el listado de 
		Public MustOverride Function getLastUpdated(clientName As String) As List(Of ClientEntity)

		Public MustOverride Function getEntity(id As String) As ClientEntity

		Public Shared Function getClientDal(clientType As String, entityType As Workbeat.Entities.EntityTypes) As ClientDal
			Dim strEntityType As String = entityType.ToString ' [Enum].GetName(Type.GetType("Workbeat.Entities.EntityTypes"), entityType)

			Dim settings As NameValueCollection = DirectCast(ConfigurationManager.GetSection("workbeat.connect"), NameValueCollection)
			Dim filePath As String = settings.Get(clientType & "_DllReference")
			Dim clientDalNamespace As String = settings.Get(clientType & "_ClientDalNamespace")

			Dim asbly As Assembly
			asbly = Assembly.LoadFrom(filePath)
			Dim dalType As Type
			Dim dalObjectName As String = clientDalNamespace & "." & strEntityType & "Dal"
			dalType = asbly.[GetType](dalObjectName)

			If dalType Is Nothing Then
				Dim log As log4net.ILog = log4net.LogManager.GetLogger(GetType(ClientDal))
				log.Error(String.Format("El tipo de dato {0} no pudo ser encontrado", [dalObjectName]))
				Throw New ArgumentException(String.Format("El tipo de dato {0} no pudo ser encontrado"), [dalObjectName])
			End If
			Return DirectCast(Activator.CreateInstance(dalType), ClientDal)
		End Function

	End Class
End Namespace
