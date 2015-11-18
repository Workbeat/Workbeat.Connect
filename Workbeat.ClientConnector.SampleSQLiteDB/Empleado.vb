
Namespace Entities
	Public Class Empleado
		Inherits Workbeat.Entities.ClientEntity

		Public Overrides Property active As Boolean
			Get
				Dim fecha As DateTime = #1/1/1900#
				For Each pos In m_data.posiciones
					If pos.fechaBaja >= fecha Then
						Return 0
					End If
				Next
				Return 1
			End Get
			Set(value As Boolean)
				m_data.activo = IIf(value, 1, 0)
			End Set
		End Property

		Private m_data As Workbeat.Entities.EmpleadoDto
		Public Overrides Property data As Workbeat.Entities.Dto
			Get
				Return m_data
			End Get
			Set(value As Workbeat.Entities.Dto)
				m_data = DirectCast(value, Workbeat.Entities.EmpleadoDto)
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
