﻿Imports System.Runtime.Serialization

<DataContract()> _
Public Class PosicionDto
	Inherits Dto
	<DataMember()> _
	Public id As Decimal
	<DataMember()> _
	Public nombre As String
	<DataMember()> _
	Public idOrganizacion As Integer
	<DataMember()> _
	Public nombreOrganizacion As String
	<DataMember()> _
	Public idPosicionReporta As System.Nullable(Of Decimal)
	<DataMember()> _
	Public codigo As String
	<DataMember()> _
	Public fechaCreacion As DateTime
	<DataMember()> _
	Public fechaUltimoCambio As DateTime
	<DataMember()> _
	Public activo As Integer

	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), PosicionDto)
	End Function
End Class

