
Namespace Entities
	Public Class Atributo
		Inherits Workbeat.Entities.ClientEntity
		

		Public Overrides Property active As Boolean
			Get
				Return 1 ' m_data.activo
			End Get
			Set(value As Boolean)
				'm_data.activo = IIf(value, 1, 0)
			End Set
		End Property

		Private m_data As Workbeat.Entities.AtributoDto
		Public Overrides Property data As Workbeat.Entities.Dto
			Get
				Return m_data
			End Get
			Set(value As Workbeat.Entities.Dto)
				m_data = DirectCast(value, Workbeat.Entities.AtributoDto)
			End Set
		End Property

		Public Overrides Property entityId As String
			Get
				Return m_data.id
			End Get
			Set(value As String)
				m_data.id = value
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

	End Class

End Namespace
