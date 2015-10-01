Public MustInherit Class WorkbeatDal

	'Protected apiObj As Workbeat.API.Client
	Private m_accessToken As String

	Public Sub New()

	End Sub

	Public Sub New(access_token As String)
		m_accessToken = access_token
		APIClient = New Workbeat.API.Client()
		APIClient.access_token = access_token
	End Sub

	Public MustOverride Sub Save(clientObject As Dto, clientName As String, clientType As String)
	Public MustOverride Sub Delete(clientObject As Dto, clientName As String, clientType As String)
	Public MustOverride Function getLastUpdated(clientName As String) As List(Of WorkbeatEntity)
	Public MustOverride Function getEntity(id As String) As Dto

	Protected m_ApiClient As Workbeat.API.Client
	Public Property APIClient() As Workbeat.API.Client
		Get
			Return m_ApiClient
		End Get
		Set(ByVal value As Workbeat.API.Client)
			m_ApiClient = value
		End Set
	End Property


	Public Property accessToken() As String
		Get
			Return m_accessToken
		End Get
		Set(ByVal value As String)
			m_accessToken = value
		End Set
	End Property

	Public Shared Function getWBDal(entityType As Workbeat.Entities.EntityTypes) As WorkbeatDal
		Dim strEntityType As String = entityType.ToString ' [Enum].GetName(Type.GetType("Workbeat.Entities.EntityTypes"), entityType)
		Dim dalObjectName As String = "Workbeat.Entities.Dal.WB" & strEntityType & "Dal, Workbeat.Entities"
		Dim dalType As Type
		dalType = Type.[GetType](dalObjectName)
		If dalType Is Nothing Then
			Throw New ArgumentException(String.Format("El tipo de dato {0} no pudo ser encontrado"), dalObjectName)
		End If
		Return DirectCast(Activator.CreateInstance(dalType), WorkbeatDal)
	End Function



End Class
