Imports System.Web.Script

Namespace WorkbeatEntities

	Public Class ElementoAtributo

		Inherits WorkbeatEntity

		Private m_data As ElementoAtributoDto
		Public Overrides Property Data As Dto
			Get
				Return m_data
			End Get
			Set(ByVal value As Dto)
				m_data = value
			End Set
		End Property

		Public Overrides Property workbeatId() As String
			Get
				Return m_data.idAtributo.ToString & "_" & m_data.id.ToString
			End Get
			Set(ByVal value As String)
				If Not String.IsNullOrWhiteSpace(value) Then

					' el valor de id es un string compuesto por "{idAtributo}_{idElementoAtributo}"
					Dim ids() As String = value.Split("_")
					m_data.id = CDec(ids(0))
					m_data.idAtributo = CDec(ids(1))
				Else
					m_data.id = 0
				End If
			End Set
		End Property

		Public Overrides Property fechaLastUpdate As Date
			Get
				Return m_data.fechaUltimoCambio
			End Get
			Set(value As Date)
				m_data.fechaUltimoCambio = value
			End Set
		End Property


		Public Overrides Property active As Boolean
			Get
				Return m_data.activo
			End Get
			Set(value As Boolean)
				m_data.activo = IIf(value, 1, 0)
			End Set
		End Property

	End Class

End Namespace
