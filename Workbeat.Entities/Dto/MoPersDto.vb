Imports System.Runtime.Serialization

<DataContract()> _
Public Class MoPersDto
	Inherits Dto

	<DataMember()> _
	Public idEmpleado As Decimal
	<DataMember()> _
	Public fecha As DateTime
	<DataMember()> _
	Public tipoMovimiento As String
	<DataMember()> _
	Public posicion As PosicionMoper
	<DataMember()> _
 Public idMotivoBaja As Integer
	<DataMember()> _
	Public observaciones As String

	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), MoPersDto)
	End Function

	<DataContract()> _
	Class PosicionMoper
		<DataMember()> _
		Public id As Decimal
		<DataMember()> _
		Public idTipoPlantilla As Decimal
	End Class

End Class
