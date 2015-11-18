Imports System.Runtime.Serialization

<DataContract()> _
Public Class DescripcionPuestoDto
	Inherits Dto

	<DataMember()> _
	Public id As Decimal
	<DataMember()> _
	Public nombre As String
	<DataMember()> _
	Public idOrganizacion As Integer
	<DataMember()> _
	Public atributos() As ElementoAtributoDto
	<DataMember()> _
	Public mision As String
	<DataMember()> _
	Public descripcion As String
	<DataMember()> _
	Public responsabilidades As String
	<DataMember()> _
	Public edadMinima As Integer
	<DataMember()> _
	Public edadMaxima As Integer
	<DataMember()> _
	Public nivelEstudios As String
	<DataMember()> _
	Public estadoCivil As String
	<DataMember()> _
	Public estatusEstudios As String
	<DataMember()> _
	Public sexo As String
	<DataMember()> _
	Public disponibilidadViajar As String
	<DataMember()> _
	Public disponibilidadCambiarResidencia As String
	<DataMember()> _
	Public otrosRequerimientos As String
	<DataMember()> _
	Public fechaUltimoCambio As DateTime
	<DataMember()> _
	Public activo As Integer


	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), DescripcionPuestoDto)
	End Function

End Class
