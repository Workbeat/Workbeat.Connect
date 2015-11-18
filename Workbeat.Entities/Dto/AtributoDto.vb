Imports System.Runtime.Serialization

<DataContract()> _
Public Class AtributoDto
	Inherits Dto

	<DataMember()> _
	Public id As Decimal
	<DataMember()> _
	Public nombre As String
	<DataMember()> _
	Public copiarDePosicionReporta As Boolean
	<DataMember()> _
	Public copiarDescripcionPuesto As Boolean
	<DataMember()> _
	Public fechaCreacion As DateTime
	<DataMember()> _
	Public fechaUltimoCambio As DateTime
	<DataMember()> _
	Public usos() As UsoDto
	<DataMember()> _
	Public activo As Integer

	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), AtributoDto)
	End Function

End Class


<DataContract()> _
 Public Class UsoDto
	Inherits Dto

	<DataMember()> _
	Public id As Integer
	<DataMember()> _
	Public nombre As String


	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), UsoDto)
	End Function
End Class
