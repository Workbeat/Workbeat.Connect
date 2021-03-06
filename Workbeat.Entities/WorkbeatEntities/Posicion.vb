﻿Namespace WorkbeatEntities
	Public Class Posicion
		Inherits WorkbeatEntity

		'Public DataType As Type = GetType(WBPosicionDto)

		Private m_data As PosicionDto
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
				Return m_data.fechaUltimoCambio
			End Get
			Set(value As Date)
				m_data.fechaUltimoCambio = value
			End Set
		End Property


		Public Overrides Property active As Boolean
			Get
				Return m_data.activo >= 1 ' activo 1 y 2 estan activos
			End Get
			Set(value As Boolean)
				m_data.activo = IIf(value, 1, 0)
			End Set
		End Property
	End Class

End Namespace
