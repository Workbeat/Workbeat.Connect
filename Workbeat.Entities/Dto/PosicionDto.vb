

Public Class PosicionDto
	Inherits Dto
	Public id As Decimal
	Public nombre As String
	Public idOrganizacion As Integer
	Public nombreOrganizacion As String
	Public idPosicionReporta As System.Nullable(Of Decimal)
	Public codigo As String
	Public fechaCreacion As DateTime
	Public fechaUltimoCambio As DateTime
	Public activo As Integer



	Public Overrides Function Clone() As Dto
		Return DirectCast(Me.MemberwiseClone(), PosicionDto)
	End Function



End Class

