Imports System.Runtime.Serialization

<DataContract()> _
Public Class OrganizacionDto
	Inherits Dto

	<DataMember()> _
	Public id As Decimal
	<DataMember()> _
	Public nombre As String
	<DataMember()> _
	Public fechaCreacion As DateTime
	<DataMember()> _
	Public fechaUltimoCambio As DateTime
	<DataMember()> _
	Public activo As Integer

	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), OrganizacionDto)
	End Function

End Class
