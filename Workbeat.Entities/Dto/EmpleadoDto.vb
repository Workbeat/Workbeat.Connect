Imports System.Runtime.Serialization

<DataContract()> _
Public Class EmpleadoDto
	Inherits Dto

	<DataMember()> _
	Public id As Decimal
	<DataMember()> _
	Public nombre As String
	<DataMember()> _
	Public apellidoPaterno As String
	<DataMember()> _
	Public apellidoMaterno As String
	<DataMember()> _
	Public fechaNacimiento As String
	<DataMember()> _
	Public posiciones() As PosicionEmpleadoDto


	<DataMember()> _
	Public Property posicion() As PosicionEmpleadoDto
		Get
			If Not posiciones Is Nothing AndAlso posiciones.Length > 0 Then
				Return posiciones(0)
			Else
				Return Nothing
			End If

		End Get
		Set(ByVal value As PosicionEmpleadoDto)
			If Not posiciones Is Nothing AndAlso posiciones.Length > 0 Then
				posiciones(0) = value
			Else
				posiciones = {value}
			End If
		End Set
	End Property

	<DataMember()> _
	Public activo As Integer
	<DataMember()> _
	Public fechaUltimoCambio As DateTime

	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), EmpleadoDto)
	End Function


End Class


Public Class PosicionEmpleadoDto
	Inherits Dto

	<DataMember()> _
	Public id As Decimal
	<DataMember()> _
	Public nombre As String
	<DataMember()> _
	Public nombreOrganizacion As String
	<DataMember()> _
	Public codigo As String
	<DataMember()> _
	Public idTipoPlantilla As Integer
	<DataMember()> _
	Public fechaIngreso As DateTime
	<DataMember()> _
	Public fechaBaja As DateTime
	<DataMember()> _
	Public fechaUltimoCambio As DateTime

	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), PosicionEmpleadoDto)
	End Function
End Class