Imports System.Data.SQLite
Imports System.Configuration

Namespace Utilities.ObjectMapper
	Public Class DB
		Public Shared Sub VerifyDatabase()

			If Not (System.IO.File.Exists(ConfigurationManager.AppSettings("SQLiteDBFile"))) Then
				Dim objDal As New MapDal
				objDal.CreateObjectMappingDatabase()
			End If
		End Sub
	End Class

	Public Class Map


		Private ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(GetType(Map))

		' No permite crear instancias de map, si no es mediante los metodos Factory
		Friend Sub New()

		End Sub

		Private m_ClientName As String
		Public Property clientName() As String
			Get
				Return m_ClientName
			End Get
			Set(ByVal value As String)
				m_ClientName = value
			End Set
		End Property

		Private m_entityType As EntityTypes
		Public Property entityType() As EntityTypes
			Get
				Return m_entityType
			End Get
			Set(ByVal value As EntityTypes)
				m_entityType = value
			End Set
		End Property

		Private m_WorkbeatId As String
		Public Property workbeatId() As String
			Get
				Return m_WorkbeatId
			End Get
			Set(ByVal value As String)
				m_WorkbeatId = value
			End Set
		End Property

		Private m_ExternalId As String
		Public Property externalId() As String
			Get
				Return m_ExternalId
			End Get
			Set(ByVal value As String)
				m_ExternalId = value
			End Set
		End Property


		Private m_IsNew As Boolean = True
		Public Property isNew() As Boolean
			Get
				Return m_IsNew
			End Get
			Friend Set(ByVal value As Boolean)
				m_IsNew = value
			End Set
		End Property


		Public Sub Save()
			Dim dal As New MapDal
			log.Debug("Guardando...")
			dal.Save(Me)
			m_IsNew = False
		End Sub

		Public Shared Function getMapFromWorkbeatId(entityType As EntityTypes, wbId As String, Optional clientName As String = "ExternalClient") As Map
			Dim dal As New MapDal
			Dim mapObj As Map = dal.getMapFromWorkbeatId(clientName, entityType, wbId)
			Return mapObj
		End Function

		Public Shared Function getMapFromExternalId(entityType As EntityTypes, extId As String, Optional clientName As String = "ExternalClient") As Map
			Dim dal As New MapDal
			Dim mapObj As Map = dal.getMapFromExternalId(clientName, entityType, extId)
			Return mapObj
		End Function

	End Class

End Namespace

