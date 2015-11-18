Imports System.Runtime.Serialization

<DataContract()> _
Public Class TipoPlantillaDto
	Inherits Dto
	<DataMember()> _
	Public id As Decimal
	<DataMember()> _
	Public nombre As String
	<DataMember()> _
	Public fechaCreacion As DateTime
	<DataMember()> _
	Public fechaUltimoCambio As DateTime


	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), TipoPlantillaDto)
	End Function

End Class
