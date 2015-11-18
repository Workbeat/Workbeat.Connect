Imports System.Runtime.Serialization

<DataContract()> _
Public Class ElementoAtributoDto
	Inherits Dto

	<DataMember()> _
	Public id As Decimal
	<DataMember()> _
	Public idAtributo As Decimal
	<DataMember()> _
	Public nombre As String
	<DataMember()> _
	Public referencia As String
	<DataMember()> _
	Public activo As Integer
	<DataMember()> _
	Public fechaUltimoCambio As Date


	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), ElementoAtributoDto)
	End Function



End Class
