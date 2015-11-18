Namespace WorkbeatEntities
	Public Class Empleado
		Inherits WorkbeatEntity

		Private m_data As EmpleadoDto
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
				Return m_data.id.ToString
			End Get
			Set(ByVal value As String)
				m_data.id = CDec(value)
			End Set
		End Property

		Public Overrides Property fechaLastUpdate As Date
			Get
				Dim fecha As DateTime = #1/1/1900#
				For Each pos In m_data.posiciones
					If pos.fechaUltimoCambio > fecha Then
						fecha = pos.fechaUltimoCambio
					End If
				Next
				If m_data.fechaUltimoCambio > fecha Then
					fecha = m_data.fechaUltimoCambio
				End If
				Return fecha
			End Get
			Set(value As Date)
				m_data.fechaUltimoCambio = value
			End Set
		End Property



		' activo es cuando tiene por lo menos una posicion dada de alta.
		Public Overrides Property active As Boolean
			Get
				Dim fecha As DateTime = #1/2/1970#
				For Each pos In m_data.posiciones
					If pos.fechaBaja >= fecha Then
						Return False
					End If
				Next
				Return True
			End Get
			Set(value As Boolean)
				m_data.activo = IIf(value, 1, 0)
			End Set
		End Property
	End Class
End Namespace
